using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario
{
    // Modo de prueba: abre otra ventana de Escaneo conectada a la base InventarioPruebas, con una franja naranja,
    // etiquetas marcadas "PRUEBA - NO USAR" y la guia de pruebas paso a paso.
    public partial class Escaneo
    {
        private GuiaPruebas guiaPruebas;

        private void AgregarBotonModoPrueba()
        {
            Button btnModoPrueba = new Button
            {
                Name = "btnModoPrueba",
                Text = "Modo de prueba",
                Size = new Size(120, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TabStop = false
            };
            btnModoPrueba.Location = new Point(ClientSize.Width - btnModoPrueba.Width - 8 - 96 - 6, 2);
            btnModoPrueba.Click += (s, e) => EntrarModoPrueba();
            Controls.Add(btnModoPrueba);
            btnModoPrueba.BringToFront();
        }

        private void EntrarModoPrueba()
        {
            if (MessageBox.Show(this,
                    "Vas a entrar al Modo de prueba.\r\n\r\n" +
                    "• Todo lo que hagas se guarda en la base de PRUEBAS, nada en producción.\r\n" +
                    "• Las etiquetas salen marcadas \"" + ModoPrueba.TextoEtiqueta + "\".\r\n" +
                    "• A la derecha tendrás la guía con las pruebas paso a paso.\r\n\r\n¿Entrar?",
                    "Modo de prueba", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string error;
            if (!ModoPrueba.Entrar(out error)) { MessageBox.Show(this, error, "Modo de prueba", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try
            {
                using (Escaneo prueba = new Escaneo(usu)) prueba.ShowDialog(this);
            }
            finally
            {
                ModoPrueba.Salir();
            }
            MessageBox.Show(this, "Saliste del Modo de prueba. Ya estás otra vez en producción.", "Modo de prueba");
        }

        // Se llama al final del constructor cuando la ventana se abre en modo de prueba
        private void PrepararVentanaPrueba()
        {
            Text = "PRUEBA · " + Text;
            WindowState = FormWindowState.Maximized;

            Panel franja = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.FromArgb(230, 126, 34) };
            Label aviso = new Label
            {
                Text = "MODO DE PRUEBA  ·  Todo se guarda en la base de pruebas, nada en producción  ·  PO de prueba " + ModoPrueba.POPrueba,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 11f),
                AutoSize = true,
                Location = new Point(12, 9)
            };
            Button salir = new Button
            {
                Text = "Salir del modo de prueba",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(160, 80, 10),
                Cursor = Cursors.Hand,
                Size = new Size(190, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                UseVisualStyleBackColor = false
            };
            salir.FlatAppearance.BorderSize = 0;
            salir.Location = new Point(franja.Width - salir.Width - 8, 5);
            salir.Click += (s, e) => Close();
            franja.Controls.Add(aviso);
            franja.Controls.Add(salir);
            franja.Resize += (s, e) => salir.Left = franja.Width - salir.Width - 8;

            var tallas = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("SMRG", "840160731860"),
                new KeyValuePair<string, string>("MDRG", "840160731853"),
                new KeyValuePair<string, string>("LGRG", "840160731839"),
            };
            List<GuiaPruebas.Prueba> pruebas = GuiaPruebas.PruebasTYNDALE();
            AsignarAccionesTYNDALE(pruebas);
            guiaPruebas = new GuiaPruebas("TYNDALE", pruebas, tallas) { Dock = DockStyle.Right };
            guiaPruebas.Escanear += SimularEscaneo;
            guiaPruebas.IrA += IrAPestanaPrueba;
            guiaPruebas.PasarIds += ids =>
            {
                IrAPestanaPrueba("reimpresion");
                txtIDReImpresion.Text = ids;
            };

            Controls.Add(guiaPruebas);
            Controls.Add(franja);
            tpApparelInternational.BringToFront();   // el contenido normal llena lo que queda
        }

        private void IrAPestanaPrueba(string pestana)
        {
            TabPage destino = null;
            switch (pestana)
            {
                case "escaneo": destino = tpEscan; break;
                case "reimpresion": destino = tpReimpresion; break;
                case "bajacaja": destino = tpBajaCaja; break;
                case "reportes": destino = tpReportes; break;
            }
            if (destino == null || !TabEsaneo.TabPages.Contains(destino)) return;
            tpApparelInternational.SelectedTab = tabTerminado;
            if (TabEsaneo.SelectedTab != destino) TabEsaneo.SelectedTab = destino;
        }

        // ------------------------------------------------------------- "Hacerlo por mi" de TYNDALE
        private const string UpcSMRG = "840160731860", UpcMDRG = "840160731853", UpcLGRG = "840160731839";

        private void AsignarAccionesTYNDALE(List<GuiaPruebas.Prueba> p)
        {
            // El orden es el de GuiaPruebas.PruebasTYNDALE()
            p[0].Accion = () => PrepararCajaPrueba("SMRG");
            // Con 0 prendas, la primera define la talla: primero una SMRG y luego la MDRG equivocada
            p[1].Accion = () => { PrepararSiHaceFalta("SMRG"); Escanear(UpcSMRG, 1); SimularEscaneo(UpcMDRG); btnCancelCarton.PerformClick(); };
            p[2].Accion = () => { PrepararSiHaceFalta("SMRG"); Escanear(UpcSMRG, 2); btnCancelCarton.PerformClick(); };
            p[3].Accion = () => { PrepararSiHaceFalta("SMRG"); Escanear(UpcSMRG, 3); };
            p[4].Accion = () => { PrepararSiHaceFalta("SMRG"); Escanear(UpcSMRG, 3); };
            p[5].Accion = () => { PrepararSiHaceFalta("SMRG"); Escanear(UpcSMRG, 3); };
            p[6].Accion = () => { IrAPestanaPrueba("escaneo"); btnPrintLast.PerformClick(); };
            p[7].Accion = () => { PrepararCajaPrueba("MDRG"); Escanear(UpcMDRG, 2); btnIncompleteCarton.PerformClick(); };
            p[8].Accion = () =>
            {
                PrepararSiHaceFalta("MDRG");
                btnPrendasExtra.PerformClick();
                Escanear(UpcMDRG, 1);
                Escanear(UpcLGRG, 2);
                btnIncompleteCarton.PerformClick();
            };
            // p[9] (leer la etiqueta con la pistola) se hace a mano
            p[10].Accion = () => ReimprimirCajasPrueba(false, 1);
            p[11].Accion = () => ReimprimirCajasPrueba(false, 1, 3);
            p[12].Accion = () => ReimprimirCajasPrueba(false, 4);
            p[13].Accion = () => ReimprimirCajasPrueba(true, 1);
            p[14].Accion = () =>
            {
                string id = IdCajaPrueba(3);
                if (id == null) return;
                IrAPestanaPrueba("bajacaja");
                txtCaja.Text = id;
                btnBajaCaja.PerformClick();
            };
            p[15].Accion = () =>
            {
                IrAPestanaPrueba("reportes");
                dtpFechaInicio.Value = DateTime.Today;
                dtpFechaFinal.Value = DateTime.Today;
                if (cmbMarcaReporte != null)
                {
                    int i = cmbMarcaReporte.FindStringExact("TYNDALE");
                    if (i >= 0) cmbMarcaReporte.SelectedIndex = i;
                }
                btnBuscar.PerformClick();
            };
        }

        private static bool ElegirEnCombo(ComboBox c, string texto)
        {
            for (int i = 0; i < c.Items.Count; i++)
            {
                if (c.GetItemText(c.Items[i]).Trim() != texto) continue;
                if (c.SelectedIndex != i) c.SelectedIndex = i;
                Application.DoEvents();
                return true;
            }
            return false;
        }

        // Elige la PO de prueba, PoItem, estilo y talla; deja palomeado "Imprimir Etiquetas" y una impresora Zebra
        private void PrepararCajaPrueba(string talla)
        {
            IrAPestanaPrueba("escaneo");
            if (!ElegirEnCombo(cmbPO, ModoPrueba.POPrueba)) throw new Exception("No aparece la PO " + ModoPrueba.POPrueba + " en PoNumber.");
            ElegirEnCombo(cmbPOItem, "11");
            ElegirEnCombo(cmbProductCode, "K176T");
            if (!ElegirEnCombo(cmbSizes, talla)) throw new Exception("No aparece la talla " + talla + " en Sizes.");
            chkImprimirEtiqueta.Checked = true;
            string imp = cmbImpresora.Text.ToLower();
            if (!(imp.Contains("zebra") || imp.Contains("zdesigner") || imp.Contains("zt411") || imp.Contains("zpl")))
            {
                foreach (object o in cmbImpresora.Items)
                {
                    string n = cmbImpresora.GetItemText(o).ToLower();
                    if (n.Contains("zebra") || n.Contains("zdesigner") || n.Contains("zt411")) { cmbImpresora.SelectedItem = o; break; }
                }
            }
            txtUPCScann.Focus();
        }

        private void PrepararSiHaceFalta(string talla)
        {
            if (cmbPO.Text.Trim() != ModoPrueba.POPrueba || cmbSizes.Text.Trim() != talla) PrepararCajaPrueba(talla);
        }

        private void Escanear(string upc, int veces)
        {
            for (int i = 0; i < veces; i++) { SimularEscaneo(upc); Application.DoEvents(); }
        }

        // ID de la caja de prueba con ese numero de caja (la que no esta dada de baja)
        private string IdCajaPrueba(int numeroCaja)
        {
            foreach (System.Data.DataRow r in ModoPrueba.CajasDePrueba().Rows)
                if (Convert.ToInt32(r["Caja"]) == numeroCaja && Convert.ToString(r["Estado"]) != "Baja") return Convert.ToString(r["ID"]);
            MessageBox.Show("Todavía no existe la caja " + numeroCaja + ". Primero haz las pruebas de escaneo que la crean.");
            return null;
        }

        private void ReimprimirCajasPrueba(bool vistaPrevia, params int[] cajas)
        {
            var ids = new List<string>();
            foreach (int c in cajas) { string id = IdCajaPrueba(c); if (id == null) return; ids.Add(id); }
            IrAPestanaPrueba("reimpresion");
            txtIDReImpresion.Text = string.Join(Environment.NewLine, ids);
            if (vistaPrevia) btnPreviewZPL.PerformClick(); else btnImprimirRe.PerformClick();
        }

        // Escribe el UPC en el cuadro de escaneo y da Enter, igual que la pistola
        private void SimularEscaneo(string upc)
        {
            IrAPestanaPrueba("escaneo");
            txtUPCScann.Focus();
            txtUPCScann.Text = upc;
            txtUPCScann_KeyPress(txtUPCScann, new KeyPressEventArgs((char)Keys.Enter));
            guiaPruebas.RefrescarCajas();
        }
    }
}
