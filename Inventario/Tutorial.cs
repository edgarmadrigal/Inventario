using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario
{
    // Tutorial de bienvenida: se abre al iniciar sesion (se puede omitir) y desde el boton "Tutorial" de Escaneo.
    // Solo explica los modulos a los que el usuario tiene permiso (los recibe en "modulos").
    public class Tutorial : Form
    {
        // Claves de modulo que entiende el tutorial
        public const string Escaneo = "escaneo", AltaPO = "altapo", AltaPrePack = "altaprepack", Reimpresion = "reimpresion",
            Reportes = "reportes", BajaCaja = "bajacaja", BajaPO = "bajapo", Almacen = "almacen";

        private class Paso
        {
            public string Titulo, Texto;
            public Paso(string titulo, string texto) { Titulo = titulo; Texto = texto; }
        }

        private readonly List<Paso> pasos = new List<Paso>();
        private int actual;
        private readonly Label lblTitulo = new Label();
        private readonly Label lblPaso = new Label();
        private readonly Label lblTexto = new Label();
        private readonly Panel puntos = new Panel();
        private readonly Button btnAnterior = new Button();
        private readonly Button btnSiguiente = new Button();
        private readonly LinkLabel lnkOmitir = new LinkLabel();

        public Tutorial(ICollection<string> modulos)
        {
            ArmarPasos(modulos);

            Text = "Tutorial";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(640, 420);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 10.5f);
            KeyPreview = true;

            var encabezado = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Tema.Primario };
            lblPaso.SetBounds(28, 14, 580, 22);
            lblPaso.ForeColor = Color.FromArgb(205, 225, 245);
            lblPaso.Font = new Font("Segoe UI", 9.5f);
            lblTitulo.SetBounds(26, 36, 590, 38);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Font = new Font("Segoe UI Semibold", 17f);
            encabezado.Controls.Add(lblPaso);
            encabezado.Controls.Add(lblTitulo);

            lblTexto.SetBounds(28, 104, 584, 230);
            lblTexto.ForeColor = Tema.Texto;

            puntos.SetBounds(28, 350, 300, 16);
            puntos.Paint += DibujarPuntos;

            lnkOmitir.Text = "Omitir tutorial";
            lnkOmitir.SetBounds(28, 378, 140, 24);
            lnkOmitir.LinkColor = Color.FromArgb(96, 108, 122);
            lnkOmitir.ActiveLinkColor = Tema.Primario;
            lnkOmitir.LinkBehavior = LinkBehavior.HoverUnderline;
            lnkOmitir.LinkClicked += (s, e) => Close();

            EstiloBoton(btnAnterior, "Anterior", Color.White, Tema.Texto);
            btnAnterior.FlatAppearance.BorderSize = 1;
            btnAnterior.FlatAppearance.BorderColor = Tema.Borde;
            btnAnterior.SetBounds(374, 368, 116, 38);
            btnAnterior.Click += (s, e) => Mostrar(actual - 1);

            EstiloBoton(btnSiguiente, "Siguiente", Tema.Primario, Color.White);
            btnSiguiente.FlatAppearance.MouseOverBackColor = Tema.PrimarioHover;
            btnSiguiente.SetBounds(498, 368, 116, 38);
            btnSiguiente.Click += (s, e) => { if (actual == pasos.Count - 1) Close(); else Mostrar(actual + 1); };

            Controls.AddRange(new Control[] { encabezado, lblTexto, puntos, lnkOmitir, btnAnterior, btnSiguiente });
            AcceptButton = btnSiguiente;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) Close();
                else if (e.KeyCode == Keys.Left && actual > 0) Mostrar(actual - 1);
                else if (e.KeyCode == Keys.Right && actual < pasos.Count - 1) Mostrar(actual + 1);
            };
            Mostrar(0);
        }

        private void ArmarPasos(ICollection<string> m)
        {
            pasos.Add(new Paso("Bienvenido a Inventario V2",
                "Este sistema registra cada caja que se empaca, imprime su etiqueta según la marca del cliente " +
                "y genera los reportes de producción.\r\n\r\n" +
                "Este tutorial te explica lo básico de los módulos que tienes disponibles. Puedes omitirlo cuando " +
                "quieras y volver a verlo con el botón \"Tutorial\" de arriba a la derecha."));

            var areas = new List<string>();
            if (m.Contains(Escaneo) || m.Contains(AltaPO) || m.Contains(AltaPrePack) || m.Contains(Reimpresion)
                || m.Contains(Reportes) || m.Contains(BajaCaja) || m.Contains(BajaPO))
                areas.Add("•  Terminado: aquí están los módulos de producción que usas.");
            if (m.Contains(Almacen))
                areas.Add("•  Almacén: entradas, salidas, ubicaciones y embarques.");
            if (areas.Count > 0)
                pasos.Add(new Paso("Las pestañas",
                    "Arriba están las áreas del sistema:\r\n\r\n" + string.Join("\r\n", areas) + "\r\n\r\n" +
                    "Solo verás las pestañas a las que tienes permiso."));

            if (m.Contains(Escaneo))
            {
                pasos.Add(new Paso("Escaneo: preparar la caja",
                    "En la pestaña Escaneo:\r\n\r\n" +
                    "1.  Elige el PoNumber.\r\n" +
                    "2.  Elige el PoItem Number, el ProductCode y la talla (Sizes).\r\n" +
                    "3.  Revisa que esté seleccionada la impresora correcta.\r\n\r\n" +
                    "Cliente, Facturación y Terminado se quedan en NA si no aplican."));
                pasos.Add(new Paso("Escaneo: escanear prendas",
                    "Con el cursor en el cuadro UPC, escanea cada prenda.\r\n\r\n" +
                    "•  Units Scanned sube y Units Remaining baja con cada prenda.\r\n" +
                    "•  Al completar la caja, el sistema la guarda y la etiqueta sale sola.\r\n" +
                    "•  Si escaneas una prenda equivocada, la pantalla se pone en rojo y suena una alerta: " +
                    "presiona \"No\" para continuar y escanea la prenda correcta."));
                pasos.Add(new Paso("Escaneo: botones",
                    "•  Incomplete Carton: cierra la caja con las piezas que lleva (menos de las requeridas) y la imprime.\r\n" +
                    "•  Print Last Label: vuelve a imprimir la etiqueta de la última caja.\r\n" +
                    "•  Cancel Carton: reinicia el conteo de la caja que estás escaneando. No guarda nada.\r\n" +
                    "•  Prendas Extra: caja mixta con varias tallas (IFR, BEN DAVIS y TYNDALE)."));
            }

            var altas = new List<string>();
            if (m.Contains(AltaPO)) altas.Add("•  Alta PO: captura la marca, el estilo, el PO, las tallas, la cantidad por caja y el UPC, y guarda.");
            if (m.Contains(AltaPrePack)) altas.Add("•  Alta PrePack: captura los PO que se empacan en surtido (varias tallas por caja).");
            if (altas.Count > 0)
                pasos.Add(new Paso("Alta de PO", string.Join("\r\n\r\n", altas) + "\r\n\r\n" +
                    "Revisa los datos antes de guardar: con ellos se arman las etiquetas."));

            if (m.Contains(Reimpresion))
                pasos.Add(new Paso("Reimpresión",
                    "En la pestaña Reimpresión puedes volver a imprimir etiquetas:\r\n\r\n" +
                    "•  Escribe o pega uno o varios ID de caja (uno por renglón) y presiona IMPRIMIR.\r\n" +
                    "•  Vista Previa muestra la etiqueta antes de imprimirla.\r\n" +
                    "•  Con el número de PO puedes imprimir todas las etiquetas del contrato."));

            if (m.Contains(Reportes))
                pasos.Add(new Paso("Reportes",
                    "En la pestaña Reportes elige las fechas y la marca, y presiona buscar.\r\n\r\n" +
                    "•  Buscar: filtra lo que se ve en la tabla.\r\n" +
                    "•  Agrupar por: junta los renglones con subtotales.\r\n" +
                    "•  Excel e Imprimir: sacan el reporte tal como lo estás viendo."));

            var bajas = new List<string>();
            if (m.Contains(BajaCaja)) bajas.Add("•  Baja caja: da de baja una caja por su ID.");
            if (m.Contains(BajaPO)) bajas.Add("•  Baja PO / talla: da de baja un PO completo o una talla.");
            if (bajas.Count > 0)
                pasos.Add(new Paso("Bajas", string.Join("\r\n", bajas) + "\r\n\r\n" +
                    "Las bajas no se pueden deshacer desde el sistema: revisa bien antes de confirmar."));

            if (m.Contains(Almacen))
                pasos.Add(new Paso("Almacén",
                    "En la pestaña Almacén:\r\n\r\n" +
                    "•  Entrada: escanea el ID de cada caja y elige su ubicación.\r\n" +
                    "•  Salida: elige el PO y escanea las cajas que salen; al final presiona Terminar.\r\n" +
                    "•  Mover ubicación, Dividir caja e Imprimir tarimas para acomodar el almacén.\r\n" +
                    "•  Reporte almacén y Reporte embarques para consultar lo que hay y lo que salió."));

            pasos.Add(new Paso("¿Necesitas ayuda?",
                "Puedes volver a ver este tutorial cuando quieras con el botón \"Tutorial\" " +
                "de arriba a la derecha.\r\n\r\n" +
                "Si algo no funciona, contacta a Edgar Madrigal al 871 896 1844."));
        }

        private static void EstiloBoton(Button b, string texto, Color fondo, Color letra)
        {
            b.Text = texto;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = fondo;
            b.ForeColor = letra;
            b.Cursor = Cursors.Hand;
            b.UseVisualStyleBackColor = false;
        }

        private void Mostrar(int i)
        {
            actual = i;
            lblPaso.Text = "Paso " + (i + 1) + " de " + pasos.Count;
            lblTitulo.Text = pasos[i].Titulo;
            lblTexto.Text = pasos[i].Texto;
            btnAnterior.Visible = i > 0;
            btnSiguiente.Text = i == pasos.Count - 1 ? "Terminar" : "Siguiente";
            lnkOmitir.Visible = i < pasos.Count - 1;
            puntos.Invalidate();
            btnSiguiente.Focus();
        }

        private void DibujarPuntos(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            for (int i = 0; i < pasos.Count; i++)
            {
                using (var b = new SolidBrush(i == actual ? Tema.Primario : Tema.Borde))
                    e.Graphics.FillEllipse(b, i * 18, 3, 10, 10);
            }
        }
    }
}
