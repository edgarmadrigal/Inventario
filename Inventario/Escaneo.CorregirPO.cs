using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Inventario
{
    // Pestana Corregir PO: cambiar descripcion, color, estilo, vendor, ShipTo y UPC de una PO en el catalogo
    // (catProductos) y en lo ya escaneado. Se arma en codigo para no tocar el disenador.
    // Tiene los mismos permisos que la pestana Baja PO / talla: si esa se quita, esta tambien.
    public partial class Escaneo
    {
        private static readonly string[] camposEditablesCorreccion = { "estilo", "color", "descripcion", "vendor", "shipTo", "upc" };

        private TabPage tpCorregirPO;
        private ComboBox cmbPOCorr;
        private DataGridView dgvCorregir;
        private Label lblCorregirInfo;
        private List<Funciones.ProductoCorreccion> originalesCorreccion = new List<Funciones.ProductoCorreccion>();

        private void PrepararPestanaCorregir()
        {
            tpCorregirPO = new TabPage("Corregir PO") { Padding = new Padding(3) };

            var lblPO = new Label { Text = "PoNumber:", AutoSize = true, Font = new Font("Microsoft Sans Serif", 21.75F), Location = new Point(24, 33) };
            cmbPOCorr = new ComboBox
            {
                Font = new Font("Microsoft Sans Serif", 14.25F),
                Location = new Point(192, 35),
                Width = 240,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            var btnBuscar = new Button { Text = "BUSCAR", Location = new Point(450, 33), Size = new Size(130, 36), Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold) };
            btnBuscar.Click += (s, e) => CargarCorreccion();
            cmbPOCorr.SelectionChangeCommitted += (s, e) => CargarCorreccion();
            cmbPOCorr.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) CargarCorreccion(); };

            var ayuda = new Label
            {
                Text = "Edita directo en la tabla las columnas Estilo, Color, Descripción, Vendor, ShipTo y UPC (las celdas cambiadas se ponen amarillas).\r\n" +
                       "Al guardar se cambia el catálogo y TAMBIÉN las cajas ya escaneadas de ese renglón. Todo queda registrado en la bitácora.",
                Location = new Point(24, 85),
                Size = new Size(980, 40),
                Font = new Font("Microsoft Sans Serif", 10F)
            };

            dgvCorregir = new DataGridView
            {
                Location = new Point(24, 130),
                Size = new Size(980, 380),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            dgvCorregir.CellValueChanged += (s, e) => MarcarCambioCorreccion(e.RowIndex, e.ColumnIndex);
            dgvCorregir.DataError += (s, e) => e.ThrowException = false;

            lblCorregirInfo = new Label
            {
                Location = new Point(24, 520),
                Size = new Size(560, 40),
                Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold)
            };
            var btnDeshacer = new Button
            {
                Text = "DESHACER CAMBIOS",
                Location = new Point(600, 518),
                Size = new Size(170, 45),
                Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold)
            };
            btnDeshacer.Click += (s, e) => CargarCorreccion();
            var btnGuardar = new Button
            {
                Text = "GUARDAR CAMBIOS",
                Location = new Point(784, 518),
                Size = new Size(220, 45),
                Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold)
            };
            btnGuardar.Click += (s, e) => GuardarCorreccion();

            tpCorregirPO.Controls.AddRange(new Control[] { lblPO, cmbPOCorr, btnBuscar, ayuda, dgvCorregir, lblCorregirInfo, btnDeshacer, btnGuardar });
            // TabPages.Insert no hace nada si el TabControl aun no tiene handle (estamos en el constructor): se fuerza
            IntPtr crearHandle = TabEsaneo.Handle;
            TabEsaneo.TabPages.Insert(TabEsaneo.TabPages.IndexOf(tpbajaPO) + 1, tpCorregirPO);

            // Mismo permiso que Baja PO / talla
            TabEsaneo.ControlRemoved += (s, e) =>
            {
                if (e.Control == tpbajaPO && TabEsaneo.TabPages.Contains(tpCorregirPO))
                    TabEsaneo.TabPages.Remove(tpCorregirPO);
            };
            btnGuardar.Enabled = btnBajaPO.Enabled;
            btnBajaPO.EnabledChanged += (s, e) => btnGuardar.Enabled = btnBajaPO.Enabled;
            tpCorregirPO.Enter += (s, e) =>
            {
                if (cmbPOCorr.Items.Count == 0)
                {
                    try { f.ConsultaPO(cmbPOCorr); cmbPOCorr.SelectedIndex = -1; } catch { }
                }
            };
        }

        private void CargarCorreccion()
        {
            try
            {
                string po = cmbPOCorr.Text.Trim();
                if (po == "") return;
                Cursor.Current = Cursors.WaitCursor;

                originalesCorreccion = f.ConsultaProductosCorreccion(po);
                var copia = originalesCorreccion.Select(o => (Funciones.ProductoCorreccion)CopiarProducto(o)).ToList();
                dgvCorregir.DataSource = null;
                dgvCorregir.DataSource = new BindingSourceLista(copia);

                var titulos = new Dictionary<string, string>
                {
                    { "id", "ID" }, { "po_item", "POItem" }, { "marca", "Marca" }, { "talla", "Talla" }, { "estilo", "Estilo" },
                    { "color", "Color" }, { "descripcion", "Descripción" }, { "vendor", "Vendor" }, { "shipTo", "ShipTo" },
                    { "upc", "UPC" }, { "cantidad", "Cantidad" }, { "cajas", "Cajas" }
                };
                foreach (DataGridViewColumn c in dgvCorregir.Columns)
                {
                    if (titulos.ContainsKey(c.DataPropertyName)) c.HeaderText = titulos[c.DataPropertyName];
                    bool editable = camposEditablesCorreccion.Contains(c.DataPropertyName);
                    c.ReadOnly = !editable;
                    c.DefaultCellStyle.BackColor = editable ? Color.White : Color.FromArgb(235, 238, 242);
                }
                // Ancho relativo de cada columna (la tabla mide lo mismo que el resto de la pestana)
                var pesos = new Dictionary<string, float>
                {
                    { "id", 75 }, { "po_item", 50 }, { "marca", 75 }, { "talla", 70 }, { "estilo", 85 }, { "color", 80 },
                    { "descripcion", 190 }, { "vendor", 80 }, { "shipTo", 80 }, { "upc", 105 }, { "cantidad", 60 }, { "cajas", 65 }
                };
                foreach (DataGridViewColumn c in dgvCorregir.Columns)
                    if (pesos.ContainsKey(c.DataPropertyName)) c.FillWeight = pesos[c.DataPropertyName];

                lblCorregirInfo.ForeColor = Tema.Texto;
                lblCorregirInfo.Text = originalesCorreccion.Count == 0
                    ? "La PO " + po + " no está en el catálogo."
                    : originalesCorreccion.Count + " renglón(es) de la PO " + po + ". Sin cambios.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private static Funciones.ProductoCorreccion CopiarProducto(Funciones.ProductoCorreccion o)
        {
            return new Funciones.ProductoCorreccion
            {
                id = o.id, po_item = o.po_item, marca = o.marca, talla = o.talla, estilo = o.estilo, color = o.color,
                descripcion = o.descripcion, vendor = o.vendor, shipTo = o.shipTo, upc = o.upc, cantidad = o.cantidad, cajas = o.cajas
            };
        }

        // Lista enlazable para que la tabla se pueda editar
        private class BindingSourceLista : BindingSource
        {
            public BindingSourceLista(List<Funciones.ProductoCorreccion> lista) : base(new System.ComponentModel.BindingList<Funciones.ProductoCorreccion>(lista), null) { }
        }

        private List<Funciones.ProductoCorreccion> RenglonesCorreccion()
        {
            var bs = dgvCorregir.DataSource as BindingSource;
            return bs == null ? new List<Funciones.ProductoCorreccion>() : bs.Cast<Funciones.ProductoCorreccion>().ToList();
        }

        private static string Valor(Funciones.ProductoCorreccion p, string campo)
        {
            object v = typeof(Funciones.ProductoCorreccion).GetProperty(campo).GetValue(p, null);
            return (v ?? "").ToString().Trim();
        }

        private void MarcarCambioCorreccion(int fila, int columna)
        {
            if (fila < 0 || columna < 0) return;
            var col = dgvCorregir.Columns[columna];
            if (!camposEditablesCorreccion.Contains(col.DataPropertyName)) return;

            var actual = dgvCorregir.Rows[fila].DataBoundItem as Funciones.ProductoCorreccion;
            var original = actual == null ? null : originalesCorreccion.FirstOrDefault(o => o.id == actual.id);
            if (original == null) return;

            bool cambio = Valor(actual, col.DataPropertyName) != Valor(original, col.DataPropertyName);
            dgvCorregir.Rows[fila].Cells[columna].Style.BackColor = cambio ? Color.FromArgb(255, 236, 153) : Color.Empty;

            int renglones = RenglonesCambiados().Count;
            lblCorregirInfo.ForeColor = renglones > 0 ? Color.FromArgb(160, 90, 0) : Tema.Texto;
            lblCorregirInfo.Text = renglones > 0 ? renglones + " renglón(es) con cambios sin guardar." : "Sin cambios.";
        }

        private List<Funciones.ProductoCorreccion> RenglonesCambiados()
        {
            return RenglonesCorreccion().Where(a =>
            {
                var o = originalesCorreccion.FirstOrDefault(x => x.id == a.id);
                return o != null && camposEditablesCorreccion.Any(c => Valor(a, c) != Valor(o, c));
            }).ToList();
        }

        private void GuardarCorreccion()
        {
            try
            {
                dgvCorregir.EndEdit();
                var cambiados = RenglonesCambiados();
                if (cambiados.Count == 0)
                {
                    MessageBox.Show("No hay cambios para guardar.", "Corregir PO");
                    return;
                }

                // Resumen de lo que va a cambiar
                var resumen = new System.Text.StringBuilder();
                foreach (var a in cambiados.Take(15))
                {
                    var o = originalesCorreccion.First(x => x.id == a.id);
                    var campos = camposEditablesCorreccion.Where(c => Valor(a, c) != Valor(o, c))
                        .Select(c => c.ToUpper() + ": \"" + Valor(o, c) + "\" → \"" + Valor(a, c) + "\"");
                    resumen.AppendLine("•  " + a.estilo + " " + a.talla + "  " + string.Join(",  ", campos));
                }
                if (cambiados.Count > 15) resumen.AppendLine("•  ... y " + (cambiados.Count - 15) + " renglón(es) más");
                int cajas = cambiados.Sum(a => a.cajas);

                DialogResult r = MessageBox.Show("Se van a cambiar " + cambiados.Count + " renglón(es) del catálogo de la PO " + cmbPOCorr.Text.Trim() +
                    (cajas > 0 ? " y " + cajas + " caja(s) ya escaneada(s)" : "") + ":\r\n\r\n" + resumen +
                    "\r\nEl cambio queda registrado con tu usuario, fecha, hora y equipo.\r\n\r\n¿Guardar los cambios?",
                    "Corregir PO", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (r != DialogResult.Yes) return;

                string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");
                if (password != "Ain2022_")
                {
                    MessageBox.Show("el password es incorrecto");
                    return;
                }

                Cursor.Current = Cursors.WaitCursor;
                int ok = 0;
                var errores = new List<string>();
                foreach (var a in cambiados)
                {
                    try
                    {
                        var res = f.ModificarProductoPO(a, usu[0].id);
                        if (res != null && res.Resultado == 1) ok++;
                        else errores.Add(a.estilo + " " + a.talla + ": ya no existe en el catálogo");
                    }
                    catch (Exception exFila)
                    {
                        errores.Add(a.estilo + " " + a.talla + ": " + exFila.Message);
                    }
                }
                Cursor.Current = Cursors.Default;

                MessageBox.Show("Se guardaron " + ok + " renglón(es)." +
                    (errores.Count > 0 ? "\r\n\r\nNo se pudieron guardar:\r\n" + string.Join("\r\n", errores) : ""), "Corregir PO");
                CargarCorreccion();
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }
    }
}
