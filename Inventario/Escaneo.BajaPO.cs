using Inventario.Reportes;
using Excel;
using Inventario.Packaging;
using Microsoft.VisualBasic;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;
using ZXing;
using ZXing.Common;

namespace Inventario
{
    // Pestanas BajaPO / BajaTalla / BajaCaja
    public partial class Escaneo
    {
        private static readonly Color ColorBajaTalla = Color.FromArgb(214, 120, 30);
        private static readonly Color ColorBajaCajas = Color.FromArgb(125, 60, 152);
        private static readonly Color ColorBajaCatalogo = Color.FromArgb(52, 73, 94);

        // Separa en la pestana la baja de PO completa (rojo) y la baja de una talla (naranja) para que no se confundan.
        // Se arma en codigo para no tocar el disenador; los controles son los mismos del disenador, solo se reacomodan.
        private void PrepararPestanaBajas()
        {
            var aviso = new Label
            {
                Text = "Escoge la PO y después usa la sección de la baja que necesitas.",
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 11F),
                Location = new Point(cmbPOB.Right + 20, cmbPOB.Top + 8)
            };
            tpbajaPO.Controls.Add(aviso);

            Panel secPO = SeccionBaja("BAJA DE PO COMPLETA", Tema.Peligro, new Point(24, 110), new Size(430, 225));
            secPO.Controls.Add(TextoBaja(
                "Da de baja TODA la PO seleccionada:\r\n" +
                "•  Se borran TODAS sus tallas y estilos del catálogo.\r\n" +
                "•  TODAS sus cajas escaneadas quedan dadas de baja.",
                new Point(15, 50), new Size(400, 80)));
            MoverA(secPO, btnBajaPO, 15, 155);
            BotonBaja(btnBajaPO, "DAR DE BAJA PO COMPLETA", new Size(400, 55), null);

            // Borrar solo lo escaneado: la PO se queda para volver a escanear. Mismo permiso que la baja de PO.
            Panel secCajas = SeccionBaja("BORRAR CAJAS ESCANEADAS", ColorBajaCajas, new Point(24, 350), new Size(430, 225));
            secCajas.Controls.Add(TextoBaja(
                "Da de baja TODAS las cajas escaneadas de la PO seleccionada.\r\n" +
                "La PO, sus estilos y tallas se quedan para volver a escanear.",
                new Point(15, 50), new Size(400, 80)));
            var btnBajaCajasPO = BotonExtra(secCajas, "BORRAR CAJAS ESCANEADAS DE LA PO", new Point(15, 155), new Size(400, 55), ColorBajaCajas);
            btnBajaCajasPO.Click += btnBajaCajasPO_Click;

            Panel secTalla = SeccionBaja("BAJA DE UNA TALLA", ColorBajaTalla, new Point(480, 110), new Size(520, 280));
            secTalla.Controls.Add(TextoBaja(
                "Solo borra la talla elegida de ese estilo. Si ya tiene cajas escaneadas te avisa y puedes borrarlas también.",
                new Point(15, 48), new Size(490, 45)));
            MoverA(secTalla, label82, 15, 96);
            MoverA(secTalla, cmbItemB, 190, 96);
            MoverA(secTalla, label83, 15, 138);
            MoverA(secTalla, cmbProduct, 190, 138);
            MoverA(secTalla, label32, 15, 182);
            MoverA(secTalla, cmbTallaBaja, 190, 180);
            cmbItemB.Width = cmbProduct.Width = cmbTallaBaja.Width = 310;
            MoverA(secTalla, btnBajaTalla, 15, 222);
            BotonBaja(btnBajaTalla, "DAR DE BAJA ESTA TALLA", new Size(490, 48), ColorBajaTalla);

            // Borrar solo el catalogo de la PO (como hacia antes): las cajas escaneadas se quedan. Mismo permiso que la baja de PO.
            Panel secCatalogo = SeccionBaja("BORRAR SOLO LA PO DEL CATÁLOGO", ColorBajaCatalogo, new Point(480, 405), new Size(520, 170));
            secCatalogo.Controls.Add(TextoBaja(
                "Borra la PO (estilos y tallas) del catálogo. Las cajas escaneadas NO se tocan.",
                new Point(15, 48), new Size(490, 45)));
            var btnBajaCatalogoPO = BotonExtra(secCatalogo, "BORRAR SOLO LA PO DEL CATÁLOGO", new Point(15, 102), new Size(490, 52), ColorBajaCatalogo);
            btnBajaCatalogoPO.Click += btnBajaCatalogoPO_Click;

            tpbajaPO.Controls.Add(secPO);
            tpbajaPO.Controls.Add(secCajas);
            tpbajaPO.Controls.Add(secTalla);
            tpbajaPO.Controls.Add(secCatalogo);
        }

        // Botones nuevos de la pestana: llevan el mismo permiso que la baja de PO (btnBajaPO se habilita segun el usuario)
        private Button BotonExtra(Panel seccion, string texto, Point ubicacion, Size tamano, Color color)
        {
            var b = new Button();
            MoverA(seccion, b, ubicacion.X, ubicacion.Y);
            BotonBaja(b, texto, tamano, color);
            b.Enabled = btnBajaPO.Enabled;
            btnBajaPO.EnabledChanged += (s, e) => b.Enabled = btnBajaPO.Enabled;
            return b;
        }

        private static Panel SeccionBaja(string titulo, Color color, Point ubicacion, Size tamano)
        {
            var p = new Panel { Location = ubicacion, Size = tamano, BorderStyle = BorderStyle.FixedSingle };
            p.Controls.Add(new Label
            {
                Text = "  " + titulo,
                Dock = DockStyle.Top,
                Height = 40,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            });
            return p;
        }

        private static Label TextoBaja(string texto, Point ubicacion, Size tamano)
        {
            return new Label { Text = texto, Location = ubicacion, Size = tamano, Font = new Font("Microsoft Sans Serif", 11F) };
        }

        private static void MoverA(Control contenedor, Control c, int x, int y)
        {
            c.Parent = contenedor;
            c.Location = new Point(x, y);
        }

        // Quita el icono de "eliminar" (era igual en los dos botones) y deja texto.
        // Sin color propio, Tema lo pinta rojo por decir "BAJA"; con color propio se pinta aqui.
        private static void BotonBaja(Button b, string texto, Size tamano, Color? color)
        {
            b.BackgroundImage = null;
            b.Text = texto;
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.Size = tamano;
            b.Font = new Font("Microsoft Sans Serif", 13F, FontStyle.Bold);
            b.BackColor = SystemColors.Window;
            if (color == null) return;

            Color normal = color.Value;
            b.UseVisualStyleBackColor = false;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(normal, 0.1f);
            b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(normal, 0.1f);
            b.ForeColor = Color.White;
            b.Cursor = Cursors.Hand;
            b.BackColor = b.Enabled ? normal : Tema.Deshabilitado;
            b.EnabledChanged += (s, e) => b.BackColor = b.Enabled ? normal : Tema.Deshabilitado;
        }

        // Ultima confirmacion antes de borrar algo escaneado: hay que escribir BORRAR
        private bool ConfirmarBorrar(string que)
        {
            string escrito = Microsoft.VisualBasic.Interaction.InputBox(
                "Vas a borrar " + que + ".\r\n\r\nPara confirmar escribe BORRAR (en mayúsculas):", "Confirmar borrado", "");
            if (escrito.Trim() == "BORRAR") return true;
            MessageBox.Show("No escribiste BORRAR. No se borró nada.", "Confirmar borrado");
            return false;
        }

        private void btnBajaCatalogoPO_Click(object sender, EventArgs e)
        {
            try
            {
                string po = cmbPOB.Text.Trim();
                if (po == "")
                {
                    MessageBox.Show("Selecciona la PO.", "Borrar PO del catálogo");
                    return;
                }

                Funciones.BajaTallaEstiloResult previa = f.BajaCatalogoPO(po, usu[0].id, true);
                if (previa == null || previa.Resultado == 0)
                {
                    MessageBox.Show("La PO " + po + " no está en el catálogo.", "Borrar PO del catálogo");
                    return;
                }

                DialogResult dialogResult = MessageBox.Show("Seguro que deseas borrar la PO " + po + " del catálogo?\r\n\r\n" +
                    "•  Se borrarán " + previa.Productos + " registro(s) del catálogo (estilos y tallas).\r\n" +
                    "•  Las " + previa.Cajas + " caja(s) escaneada(s) NO se borran.\r\n\r\n" +
                    "La baja queda registrada con tu usuario, fecha, hora y equipo.", "Borrar PO del catálogo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dialogResult != DialogResult.Yes) return;

                string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");
                if (password != "Ain2022_")
                {
                    MessageBox.Show("el password es incorrecto");
                    return;
                }

                Funciones.BajaTallaEstiloResult baja = f.BajaCatalogoPO(po, usu[0].id, false);
                if (baja != null && baja.Resultado == 1)
                    MessageBox.Show("Se borró la PO " + po + " del catálogo (" + baja.Productos + " registro(s)). Las cajas escaneadas se quedaron.", "Borrar PO del catálogo");
                else
                    MessageBox.Show("No se borró nada: la PO " + po + " ya no está en el catálogo.", "Borrar PO del catálogo");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBajaCajasPO_Click(object sender, EventArgs e)
        {
            try
            {
                string po = cmbPOB.Text.Trim();
                if (po == "")
                {
                    MessageBox.Show("Selecciona la PO.", "Borrar cajas escaneadas");
                    return;
                }

                Funciones.BajaCajasPOResult previa = f.BajaCajasPO(po, usu[0].id, true);
                if (previa == null || previa.Resultado == 0)
                {
                    MessageBox.Show("La PO " + po + " no tiene cajas escaneadas activas.", "Borrar cajas escaneadas");
                    return;
                }

                // Siempre se le recuerda lo que se va a borrar
                DialogResult dialogResult = MessageBox.Show("ATENCIÓN: vas a borrar TODO LO ESCANEADO de la PO " + po + ":\r\n\r\n" +
                    "•  " + previa.Cajas + " caja(s) escaneada(s)\r\n" +
                    "•  " + previa.Prendas + " prenda(s)\r\n\r\n" +
                    "La PO, sus estilos y tallas NO se borran; se podrá volver a escanear desde cero.\r\n" +
                    "La baja queda registrada con tu usuario, fecha, hora y equipo.\r\n\r\n" +
                    "¿Quieres borrar todas las cajas escaneadas de la PO?", "Borrar cajas escaneadas",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dialogResult != DialogResult.Yes) return;

                string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");
                if (password != "Ain2022_")
                {
                    MessageBox.Show("el password es incorrecto");
                    return;
                }
                if (!ConfirmarBorrar(previa.Cajas + " caja(s) escaneada(s) de la PO " + po)) return;

                Funciones.BajaCajasPOResult baja = f.BajaCajasPO(po, usu[0].id, false);
                if (baja != null && baja.Resultado == 1)
                    MessageBox.Show("Se borraron " + baja.Cajas + " caja(s) escaneada(s) de la PO " + po + ". La PO sigue disponible para escanear.", "Borrar cajas escaneadas");
                else
                    MessageBox.Show("No se borró nada: la PO " + po + " ya no tiene cajas escaneadas activas.", "Borrar cajas escaneadas");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBajaPO_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja la PO COMPLETA " + cmbPOB.Text + "?\r\n\r\n" +
                    "Se borrarán TODAS sus tallas y estilos del catálogo y TODAS sus cajas quedarán dadas de baja.\r\n\r\n" +
                    "Si solo quieres quitar una talla, usa la sección BAJA DE UNA TALLA.", "Baja PO COMPLETA",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.Yes)
                {
                    string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");

                    if (password == "Ain2022_")
                    {
                        // La baja de PO siempre da de baja lo escaneado: se confirma escribiendo BORRAR
                        if (!ConfirmarBorrar("TODAS las cajas escaneadas de la PO " + cmbPOB.Text)) return;

                        bool baja = f.BajaPO(cmbPOB.Text, 1, 1, 1, usu[0].id);
                        if (baja)
                        {
                            try { f.RegistrarBitacoraBaja("PO COMPLETA", cmbPOB.Text, usu[0].id); }
                            catch (Exception exBitacora) { MessageBox.Show("La PO se dio de baja, pero no se pudo guardar en la bitácora: " + exBitacora.Message); }
                            MessageBox.Show("se elimino correctamente.");
                        }
                        else
                        {
                            MessageBox.Show("el po no existe en la base de datos.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("el password es incorrecto");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Guardar()
        {
            //EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            //{
            //    po = Convert.ToInt32(txtPoNA.Text),
            //    //clase.poItem = txtPoItemNA.Text;
            //    Cantidad = Convert.ToInt32(txtCantidadA.Text),
            //    Size = cmbSizeA.SelectedValue.ToString(),
            //    upc = txtUPCA.Text,
            //    ProductCode = txtProductCode.Text,
            //    TipoCarton = cmbTipoCajaA.Text,
            //    Fecha = DateTime.Now,
            //    //clase.Carton = Convert.ToInt32(txtNumCajaA.Text);
            //    usuario = usu[0].nombre
            //};
            //if (agregar)
            //{
            //    clase.id_inventario = f.GuardaInventario(clase, usu[0].id);


            //}
        }

        private void btnBajaCaja_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja la caja " + txtCaja.Text.Trim(), "Baja Caja", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    bool baja = f.BajaCaja(txtCaja.Text, usu[0].id);
                    if (baja)
                    {
                        MessageBox.Show("se elimino correctamente.");
                        txtCaja.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("se elimino correctamente.");
                        txtCaja.Text = string.Empty;
                        //MessageBox.Show("la caja no existe en la base de datos.");
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnVistaPrevia_Click(object sender, EventArgs e)
        {
            BuscarPO();
        }


        public void BuscarPO()
        {
            try
            {
                /*
                 * 
                   //panelEstadoActual.Text = "Buscando Registro ...";
                   //panelEstadoActual.Text = "";
                   DataTable resultado = new DataTable();
                   List<clsParametro> parametro = new List<clsParametro>();
                   parametro.Add(new clsParametro("@Valor", parametroBusqueda));
                   parametro.Add(new clsParametro("@Parametro", cmbBusqueda.SelectedItem));
                   parametro.Add(new clsParametro("@EmpresaID", empresa.empresa_idEmpresa));
                   dgvAlta.DataSource = f.ConsultaTablaGeneral("spSIAF_mtto_ConsultarRuta", parametro);
                   if (dgvAlta.RowCount > 0)
                   {
                       dgvAlta.Columns["EmpresaID"].Visible = false;
                       panelEstadoActual.Text = "Registro (s) encontrado (s) ...";
                       dgvResultado.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                       limpiaBusqueda();
                   }
                   else
                   {
                       panelEstadoActual.Text = "No se encontró ningún registro ...";
                       dgvResultado.DataSource = null;
                       limpiaBusqueda();
                   }
                   */

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnNuevo_Click_1(object sender, EventArgs e)
        {
            //txtPoNA.ReadOnly = false;
            //txtUPCA.ReadOnly = false;
            //txtCantidadA.ReadOnly = false;
            //cmbTipoCajaA.Enabled = true;
            //cmbSizeA.Enabled = true;
            //btnEditar.Enabled = false;
            //btnEliminar.Enabled = false;
        }

        private void btnBajaTalla_Click(object sender, EventArgs e)
        {
            try
            {
                string po = cmbPOB.Text.Trim(), poItem = cmbItemB.Text.Trim(), estilo = cmbProduct.Text.Trim(), talla = cmbTallaBaja.Text.Trim();
                if (po == "" || poItem == "" || estilo == "" || talla == "")
                {
                    MessageBox.Show("Selecciona PO, POItem, estilo y talla.", "Baja Talla");
                    return;
                }
                string descripcion = "la talla " + talla + " del estilo " + estilo + " (POItem " + poItem + ") de la PO " + po;

                // Primero solo se consulta: cuantos productos se borrarian y si ya hay cajas escaneadas
                Funciones.BajaTallaEstiloResult previa = f.BajaTallaEstilo(po, poItem, estilo, talla, usu[0].id, true);
                if (previa == null || previa.Resultado == 0)
                {
                    MessageBox.Show("No se encontró " + descripcion + ".", "Baja Talla");
                    return;
                }
                bool conCajas = previa.Cajas > 0;
                DialogResult dialogResult;
                if (conCajas)
                {
                    // Siempre se le recuerda que tambien se borra lo escaneado
                    dialogResult = MessageBox.Show("ATENCIÓN: " + descripcion + " YA TIENE " + previa.Cajas + " CAJA(S) ESCANEADA(S).\r\n\r\n" +
                        "Si continúas se borrará la talla del catálogo Y TAMBIÉN se darán de baja esas " + previa.Cajas + " caja(s) escaneada(s).\r\n" +
                        "En cajas mixtas solo se quita esta talla; las demás tallas de la caja se quedan.\r\n\r\n" +
                        "La baja queda registrada con tu usuario, fecha, hora y equipo.\r\n\r\n" +
                        "¿Quieres borrar la talla Y sus cajas escaneadas?", "Baja Talla con cajas escaneadas",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                }
                else
                {
                    dialogResult = MessageBox.Show("Seguro que deseas dar de baja " + descripcion + "?\r\n\r\n" +
                        "Se borrará(n) " + previa.Productos + " registro(s) del catálogo. No tiene cajas escaneadas.", "Baja Talla",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                }
                if (dialogResult != DialogResult.Yes) return;

                string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");
                if (password != "Ain2022_")
                {
                    MessageBox.Show("el password es incorrecto");
                    return;
                }
                if (conCajas && !ConfirmarBorrar(previa.Cajas + " caja(s) escaneada(s) de " + descripcion)) return;

                Funciones.BajaTallaEstiloResult baja = f.BajaTallaEstilo(po, poItem, estilo, talla, usu[0].id, false, conCajas);
                if (baja != null && (baja.Resultado == 1 || baja.Resultado == 3))
                {
                    MessageBox.Show("Se dio de baja " + descripcion +
                        (baja.Resultado == 3 ? " y " + baja.Cajas + " caja(s) escaneada(s)" : "") + ".", "Baja Talla");
                    // Se recargan las tallas del estilo para que ya no aparezca la borrada
                    f.ConsultaSizes(cmbTallaBaja, po, poItem, estilo);
                    if (cmbTallaBaja.Items.Count > 0) cmbTallaBaja.SelectedIndex = 0;
                }
                else if (baja != null && baja.Resultado == -1)
                {
                    MessageBox.Show("No se dio de baja: mientras confirmabas se escanearon " + baja.Cajas + " caja(s) de esa talla. Vuelve a intentarlo.", "Baja Talla");
                }
                else
                {
                    MessageBox.Show("No se pudo dar de baja " + descripcion + ".", "Baja Talla");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbPOB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (iniciando)
                {

                }
                else
                {
                    if (cmbPOB.Text != "")
                    {
                        f.ConsultaPOItem(cmbItemB, cmbPOB.Text);
                        cmbItemB.SelectedIndex = 0;
                        f.ConsultaProductCode(cmbProduct, cmbPOB.Text, cmbItemB.Text);
                        cmbProduct.SelectedIndex = 0;
                        f.ConsultaSizes(cmbTallaBaja, cmbPOB.Text, cmbItemB.Text, cmbProduct.Text);
                        cmbTallaBaja.SelectedIndex = -1;
                        cmbTallaBaja.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbItemB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbPOB.SelectedIndex > -1 && cmbItemB.SelectedIndex > -1)
                {
                    f.ConsultaProductCode(cmbProduct, cmbPOB.Text, cmbItemB.Text);
                    cmbProduct.SelectedIndex = 0;
                    cmbProduct.Focus();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                if (cmbPOB.SelectedIndex > -1 && cmbItemB.SelectedIndex > -1 && cmbProduct.SelectedIndex > -1)
                {
                    f.ConsultaSizes(cmbTallaBaja, cmbPOB.Text, cmbItemB.Text, cmbProduct.Text);
                    cmbTallaBaja.SelectedIndex = -1;
                    cmbTallaBaja.Focus();
                    cmbTallaBaja.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void label82_Click(object sender, EventArgs e)
        {

        }
    }
}
