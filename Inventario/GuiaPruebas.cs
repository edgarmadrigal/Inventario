using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Inventario
{
    // Panel de guia del Modo de prueba: lista de pruebas por marca con palomita, instrucciones paso a paso,
    // botones para simular escaneos y la lista de cajas de prueba. El avance se guarda en la PC.
    public class GuiaPruebas : Panel
    {
        public class Prueba
        {
            public string Titulo, Hacer, Esperado, Pestana;
            /// <summary>Lo que hace el boton "Hacerlo por mi" (null = la prueba se hace a mano).</summary>
            public Action Accion;
            public Prueba(string pestana, string titulo, string hacer, string esperado)
            { Pestana = pestana; Titulo = titulo; Hacer = hacer; Esperado = esperado; }
        }

        // Estado de cada prueba: 0 pendiente, 1 quedo bien, 2 fallo
        private int[] estado;
        private int actual;
        private readonly string marca;
        private readonly List<Prueba> pruebas;
        private readonly string archivoAvance;

        private readonly Label lblAvance = new Label();
        private readonly Panel barra = new Panel();
        private readonly Panel lista = new Panel();
        private readonly Label lblTitulo = new Label();
        private readonly Label lblHacer = new Label();
        private readonly Label lblEsperado = new Label();
        private readonly DataGridView dgvCajas = new DataGridView();
        private Button btnHacer;

        /// <summary>Simula un escaneo con este UPC (lo escribe en el cuadro UPC y da Enter).</summary>
        public event Action<string> Escanear;
        /// <summary>Cambia a la pestana indicada ("escaneo", "reimpresion", "bajacaja", "reportes").</summary>
        public event Action<string> IrA;
        /// <summary>Pasa estos IDs al cuadro de Reimpresion.</summary>
        public event Action<string> PasarIds;

        public GuiaPruebas(string marca, List<Prueba> pruebas, IEnumerable<KeyValuePair<string, string>> tallas)
        {
            this.marca = marca;
            this.pruebas = pruebas;
            estado = new int[pruebas.Count];
            archivoAvance = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                         "InventarioV2", "pruebas_" + marca + ".txt");
            CargarAvance();

            Width = 360;
            BackColor = Color.White;
            Padding = new Padding(14, 10, 14, 10);
            Font = new Font("Segoe UI", 9.5f);
            AutoScroll = true;

            int y = 10;
            var titulo = new Label { Text = "Guía de pruebas · " + marca, Font = new Font("Segoe UI Semibold", 13f), ForeColor = Tema.Texto, AutoSize = true, Location = new Point(14, y) };
            Controls.Add(titulo); y += 32;

            lblAvance.SetBounds(14, y, 320, 20); lblAvance.ForeColor = Color.FromArgb(96, 108, 122);
            Controls.Add(lblAvance); y += 22;
            barra.SetBounds(14, y, 320, 10); barra.Paint += DibujarBarra;
            Controls.Add(barra); y += 20;

            // Simular escaneo de cada talla (sin prendas)
            var lblSim = new Label { Text = "Simular escaneo (sin prendas):", AutoSize = true, Location = new Point(14, y), ForeColor = Tema.Texto };
            Controls.Add(lblSim); y += 22;
            int x = 14;
            foreach (var t in tallas)
            {
                var b = Boton(t.Key, Tema.Primario, Color.White);
                b.SetBounds(x, y, 100, 32);
                string upc = t.Value;
                b.Click += (s, e) => Escanear?.Invoke(upc);
                Controls.Add(b); x += 106;
            }
            y += 44;

            // Lista de pruebas
            lista.SetBounds(14, y, 320, 250);
            lista.AutoScroll = true;
            lista.BackColor = Tema.Fondo;
            Controls.Add(lista); y += 258;

            // Detalle de la prueba seleccionada
            lblTitulo.SetBounds(14, y, 320, 26); lblTitulo.Font = new Font("Segoe UI Semibold", 11f); lblTitulo.ForeColor = Tema.Primario;
            Controls.Add(lblTitulo); y += 28;
            Controls.Add(Etiqueta("Qué hacer:", 14, y)); y += 20;
            lblHacer.SetBounds(14, y, 320, 150); lblHacer.ForeColor = Tema.Texto;
            Controls.Add(lblHacer); y += 152;
            Controls.Add(Etiqueta("Debe pasar:", 14, y)); y += 20;
            lblEsperado.SetBounds(14, y, 320, 80); lblEsperado.ForeColor = Color.FromArgb(25, 110, 60);
            Controls.Add(lblEsperado); y += 84;

            // Hace la prueba sola (elige PO/talla, simula escaneos, presiona los botones); la persona solo revisa
            btnHacer = Boton("▶  Hacerlo por mí", Color.FromArgb(230, 126, 34), Color.White);
            btnHacer.Font = new Font("Segoe UI Semibold", 11f);
            btnHacer.SetBounds(14, y, 320, 44);
            btnHacer.Click += (s, e) => HacerPrueba();
            Controls.Add(btnHacer); y += 52;

            var btnBien = Boton("✓  Quedó bien", Color.FromArgb(34, 139, 74), Color.White);
            btnBien.SetBounds(14, y, 156, 40);
            btnBien.Click += (s, e) => Marcar(1);
            var btnFallo = Boton("✗  Falló", Tema.Peligro, Color.White);
            btnFallo.SetBounds(178, y, 156, 40);
            btnFallo.Click += (s, e) => Marcar(2);
            Controls.Add(btnBien); Controls.Add(btnFallo); y += 52;

            // Cajas de prueba
            var lblCajas = Etiqueta("Cajas de prueba guardadas:", 14, y);
            var btnRefrescar = Boton("Actualizar", Color.White, Tema.Texto);
            btnRefrescar.FlatAppearance.BorderSize = 1; btnRefrescar.FlatAppearance.BorderColor = Tema.Borde;
            btnRefrescar.SetBounds(254, y - 4, 80, 26);
            btnRefrescar.Click += (s, e) => RefrescarCajas();
            Controls.Add(lblCajas); Controls.Add(btnRefrescar); y += 26;
            dgvCajas.SetBounds(14, y, 320, 150);
            dgvCajas.ReadOnly = true; dgvCajas.AllowUserToAddRows = false; dgvCajas.RowHeadersVisible = false;
            dgvCajas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCajas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Controls.Add(dgvCajas); y += 156;
            var btnIds = Boton("Pasar IDs seleccionados a Reimpresión", Tema.Primario, Color.White);
            btnIds.SetBounds(14, y, 320, 32);
            btnIds.Click += (s, e) =>
            {
                var ids = dgvCajas.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index)
                                  .Select(r => Convert.ToString(r.Cells["ID"].Value)).ToList();
                if (ids.Count == 0) { MessageBox.Show("Selecciona una o varias cajas de la lista."); return; }
                PasarIds?.Invoke(string.Join(Environment.NewLine, ids));
            };
            Controls.Add(btnIds); y += 44;

            var btnReiniciar = Boton("Reiniciar pruebas (borra las cajas de prueba)", Color.White, Tema.Peligro);
            btnReiniciar.FlatAppearance.BorderSize = 1; btnReiniciar.FlatAppearance.BorderColor = Tema.Peligro;
            btnReiniciar.SetBounds(14, y, 320, 32);
            btnReiniciar.Click += (s, e) => Reiniciar();
            Controls.Add(btnReiniciar); y += 44;

            ArmarLista();
            int primera = Array.IndexOf(estado, 0);
            Seleccionar(primera >= 0 ? primera : 0);
            RefrescarCajas();
        }

        private static Label Etiqueta(string texto, int x, int y)
        {
            return new Label { Text = texto, AutoSize = true, Location = new Point(x, y), Font = new Font("Segoe UI Semibold", 9.5f), ForeColor = Tema.Texto };
        }

        private static Button Boton(string texto, Color fondo, Color letra)
        {
            var b = new Button { Text = texto, FlatStyle = FlatStyle.Flat, BackColor = fondo, ForeColor = letra, Cursor = Cursors.Hand, UseVisualStyleBackColor = false };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void ArmarLista()
        {
            lista.Controls.Clear();
            for (int i = 0; i < pruebas.Count; i++)
            {
                int indice = i;
                var renglon = new Label { Text = Icono(i) + "   " + (i + 1) + ". " + pruebas[i].Titulo };
                renglon.SetBounds(0, i * 30, 300, 28);
                renglon.TextAlign = ContentAlignment.MiddleLeft;
                renglon.Padding = new Padding(6, 0, 0, 0);
                renglon.Cursor = Cursors.Hand;
                renglon.ForeColor = estado[i] == 1 ? Color.FromArgb(34, 139, 74) : estado[i] == 2 ? Tema.Peligro : Tema.Texto;
                renglon.BackColor = i == actual ? Tema.Seleccion : Tema.Fondo;
                renglon.Font = i == actual ? new Font("Segoe UI Semibold", 9.5f) : new Font("Segoe UI", 9.5f);
                renglon.Click += (s, e) => Seleccionar(indice);
                lista.Controls.Add(renglon);
            }
            int hechas = estado.Count(e => e != 0), bien = estado.Count(e => e == 1), fallas = estado.Count(e => e == 2);
            lblAvance.Text = hechas + " de " + pruebas.Count + " pruebas   ·   ✓ " + bien + "   ✗ " + fallas;
            barra.Invalidate();
        }

        private void HacerPrueba()
        {
            Prueba p = pruebas[actual];
            if (p.Accion == null) return;
            Cursor.Current = Cursors.WaitCursor;
            try { p.Accion(); }
            catch (Exception ex) { MessageBox.Show("No se pudo hacer la prueba automáticamente: " + ex.Message); }
            finally { Cursor.Current = Cursors.Default; }
            RefrescarCajas();
        }

        private string Icono(int i) { return estado[i] == 1 ? "✓" : estado[i] == 2 ? "✗" : "○"; }

        private void Seleccionar(int i)
        {
            actual = i;
            Prueba p = pruebas[i];
            lblTitulo.Text = (i + 1) + ". " + p.Titulo;
            lblHacer.Text = p.Hacer;
            lblEsperado.Text = p.Esperado;
            if (btnHacer != null)
            {
                btnHacer.Enabled = p.Accion != null;
                btnHacer.Text = p.Accion != null ? "▶  Hacerlo por mí" : "Esta prueba se hace a mano";
                btnHacer.BackColor = p.Accion != null ? Color.FromArgb(230, 126, 34) : Tema.Deshabilitado;
            }
            ArmarLista();
            if (i < lista.Controls.Count) lista.ScrollControlIntoView(lista.Controls[i]);
            if (!string.IsNullOrEmpty(p.Pestana)) IrA?.Invoke(p.Pestana);
        }

        private void Marcar(int valor)
        {
            estado[actual] = valor;
            GuardarAvance();
            RefrescarCajas();
            int siguiente = -1;
            for (int k = 1; k <= estado.Length; k++)
            {
                int j = (actual + k) % estado.Length;
                if (estado[j] == 0) { siguiente = j; break; }
            }
            if (siguiente >= 0) { Seleccionar(siguiente); return; }

            ArmarLista();
            int fallas = estado.Count(e => e == 2);
            if (fallas == 0)
                MessageBox.Show("¡Listo! " + marca + " pasó las " + pruebas.Count + " pruebas. ✓", "Pruebas terminadas", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Terminaste las pruebas de " + marca + ".\r\n\r\nFallaron " + fallas + ":\r\n\r\n" +
                    string.Join("\r\n", pruebas.Select((p, k) => estado[k] == 2 ? "✗ " + (k + 1) + ". " + p.Titulo : null).Where(t => t != null)) +
                    "\r\n\r\nMándale foto de esta lista a Edgar.", "Pruebas terminadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void RefrescarCajas()
        {
            try
            {
                dgvCajas.DataSource = ModoPrueba.CajasDePrueba();
                if (dgvCajas.Columns.Contains("ID")) dgvCajas.Columns["ID"].FillWeight = 140;
            }
            catch (Exception ex) { MessageBox.Show("No se pudieron leer las cajas de prueba: " + ex.Message); }
        }

        private void Reiniciar()
        {
            if (MessageBox.Show("Se van a borrar las cajas de prueba de la PO " + ModoPrueba.POPrueba +
                    " (solo en la base de pruebas) y se van a desmarcar todas las pruebas. ¿Continuar?",
                    "Reiniciar pruebas", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { ModoPrueba.ReiniciarCajas(); }
            catch (Exception ex) { MessageBox.Show("No se pudieron borrar las cajas de prueba: " + ex.Message); return; }
            estado = new int[pruebas.Count];
            GuardarAvance();
            RefrescarCajas();
            Seleccionar(0);
        }

        private void CargarAvance()
        {
            try
            {
                if (!File.Exists(archivoAvance)) return;
                string[] partes = File.ReadAllText(archivoAvance).Split(',');
                for (int i = 0; i < estado.Length && i < partes.Length; i++) int.TryParse(partes[i], out estado[i]);
            }
            catch { }
        }

        private void GuardarAvance()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(archivoAvance));
                File.WriteAllText(archivoAvance, string.Join(",", estado));
            }
            catch { }
        }

        private void DibujarBarra(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Tema.Borde), 0, 0, barra.Width, barra.Height);
            if (estado.Length == 0) return;
            float ancho = barra.Width / (float)estado.Length;
            for (int i = 0; i < estado.Length; i++)
            {
                if (estado[i] == 0) continue;
                using (var b = new SolidBrush(estado[i] == 1 ? Color.FromArgb(34, 139, 74) : Tema.Peligro))
                    e.Graphics.FillRectangle(b, i * ancho, 0, ancho, barra.Height);
            }
        }

        // ------------------------------------------------------------------ pruebas por marca
        public static List<Prueba> PruebasTYNDALE()
        {
            string po = ModoPrueba.POPrueba;
            return new List<Prueba>
            {
                new Prueba("escaneo", "Preparar la caja",
                    "En Escaneo elige:\r\n• PoNumber: " + po + "\r\n• PoItem Number: 11\r\n• ProductCode: K176T\r\n• Sizes: SMRG\r\n" +
                    "Selecciona la impresora Zebra y palomea \"Imprimir Etiquetas\".",
                    "Units Required dice 3 y Total Required dice 6."),
                new Prueba("escaneo", "Prenda equivocada",
                    "Simula SMRG 1 vez y luego MDRG 1 vez (la primera prenda de la caja define la talla, por eso va primero una SMRG). " +
                    "Al final presiona \"Cancel Carton\" para dejar el conteo en 0.",
                    "Con la MDRG la pantalla se pone roja, suena la alerta y sale el aviso \"Favor de Escanear la prenda correcta\". " +
                    "Presiona \"No\" para seguir. No se guarda nada."),
                new Prueba("escaneo", "Cancelar caja",
                    "Simula SMRG 2 veces y presiona \"Cancel Carton\".",
                    "Units Scanned regresa a 0. No sale etiqueta y no aparece ninguna caja nueva en la lista de abajo."),
                new Prueba("escaneo", "Caja completa (caja 1)",
                    "Simula SMRG 3 veces.",
                    "Sale 1 etiqueta con la franja PRUEBA: LOT " + po + ", QTY 3, K176T-LBL-SMRG y CARTON 1. En la lista aparece la caja 1."),
                new Prueba("escaneo", "Segunda caja (número consecutivo)",
                    "Simula SMRG 3 veces otra vez.",
                    "Sale la etiqueta con CARTON 2 (el número NO se repite) y sale el aviso de que ya se escanearon todas las prendas de la talla: es normal, SMRG ya quedó completa."),
                new Prueba("escaneo", "Talla completa (no deja pasarse)",
                    "La talla SMRG ya tiene 6 de 6. Simula SMRG 3 veces más.",
                    "Sale el aviso \"Ya se escanearon todas las prendas de esa talla y ese po\". No sale etiqueta ni se guarda caja."),
                new Prueba("escaneo", "Print Last Label",
                    "Presiona \"Print Last Label\".",
                    "Vuelve a salir la etiqueta de la última caja guardada (CARTON 2, SMRG, QTY 3)."),
                new Prueba("escaneo", "Caja incompleta (caja 3)",
                    "Cambia Sizes a MDRG. Simula MDRG 2 veces y presiona \"Incomplete Carton\".",
                    "Sale la etiqueta con QTY 2, K176T-LBL-MDRG y CARTON 3."),
                new Prueba("escaneo", "Caja mixta (caja 4)",
                    "Presiona \"Prendas Extra\". Simula MDRG 1 vez y LGRG 2 veces. Presiona \"Incomplete Carton\".",
                    "Salen 2 etiquetas, una por talla (MDRG QTY 1 y LGRG QTY 2), las dos con CARTON 4."),
                new Prueba("", "Leer la etiqueta con la pistola",
                    "Abre el Bloc de notas y escanea con la pistola los 3 códigos cuadrados de la etiqueta de la caja 1.",
                    "Se lee: " + po + ", K176T-LBL-SMRG y 3."),
                new Prueba("reimpresion", "Reimprimir 1 caja",
                    "En la lista de abajo selecciona la caja 1 y presiona \"Pasar IDs seleccionados a Reimpresión\". Luego IMPRIMIR.",
                    "Sale la misma etiqueta de la caja 1 (CARTON 1)."),
                new Prueba("reimpresion", "Reimprimir varias cajas",
                    "Selecciona las cajas 1 y 3 en la lista (Ctrl + clic), pásalas a Reimpresión y presiona IMPRIMIR.",
                    "Salen las 2 etiquetas y el aviso \"Se mandaron a reimprimir 2 ID(s)\"."),
                new Prueba("reimpresion", "Reimprimir caja mixta",
                    "Pasa a Reimpresión el ID de la caja 4 (la mixta) y presiona IMPRIMIR.",
                    "Salen 2 etiquetas, una por talla, las dos con CARTON 4."),
                new Prueba("reimpresion", "Vista previa",
                    "Pasa el ID de la caja 1 y presiona \"Vista Previa\".",
                    "Se ve la etiqueta en pantalla y NO se imprime."),
                new Prueba("bajacaja", "Dar de baja una caja",
                    "En la pestaña Baja caja pon el ID de la caja 3 y dala de baja. Luego presiona \"Actualizar\" en la lista.",
                    "En la lista, la caja 3 dice \"Baja\"."),
                new Prueba("reportes", "Reporte",
                    "En Reportes elige la fecha de hoy, marca TYNDALE y busca.",
                    "Aparecen las cajas de prueba de la PO " + po + "."),
            };
        }
    }
}
