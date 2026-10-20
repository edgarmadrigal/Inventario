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
    // Almacen: entrada, salida, mover ubicacion y etiqueta de tarima (varios handlers no estan conectados a controles)
    public partial class Escaneo
    {
        private void cmbPoEntradaAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                ConsultaCajasPO();
            }
        }

        public void ConsultaCajasPO()
        {
            try
            {
                txtIDCaja.Focus();
                Cursor.Current = Cursors.Default;
                List<clsParametro> parametro = new List<clsParametro>
                {
                    new clsParametro("@PO", cmbPoEntradaAlmacen.Text),
                    new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@POSolamente", cbPOSolamente.Checked)
                };

                dgvAlmacen.DataSource = f.ConsultaTablaGeneral("ubicacion_Entrada_ConsultaCajas", parametro);
                dgvAlmacen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                Cursor.Current = Cursors.WaitCursor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void xtpSalida_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtUbicacionID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                consultaUbicacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void txtSeccionID_Leave(object sender, EventArgs e)
        {
            try
            {
                consultaUbicacion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void consultaUbicacion()
        {
            try
            {
                int ubicacionID = 0;
                if (txtUbicacionID.SelectedValue.ToString() != string.Empty)
                {
                    ubicacionID = Convert.ToInt32(txtUbicacionID.SelectedValue);
                    if (ubicacionID > 0)
                    {
                        List<ubicacion_Entrada_ConsultaUbicacionIDResult> seccionid = f.ConsultaUbicacionID(ubicacionID);
                        if (seccionid.Count > 0)
                        {
                            txtNivel.Text = seccionid[0].nivel.ToString();
                            List<ubicacion_Entrada_ConsultaUbicacionDetalleIDResult> ubicacionDetalle = f.ConsultaUbicacionDetalleID(ubicacionID);
                            dgvUbicacion.DataSource = ubicacionDetalle;
                            dgvUbicacion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            int totalCajasTarima = 0;
                            totalCajasTarima = ubicacionDetalle.Count;
                            txtTotalTarima.Text = totalCajasTarima.ToString();
                            //BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                            //Codigo.IncludeLabel = true;
                            //Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, txtUbicacionID.Text, Color.Black, Color.White, 350, 200);
                            //pictureBox3.BackgroundImage =codigoBarras;
                            txtTotalEscaneado.Text = "0";
                            txtIDCaja.Text = "";

                            //lblError.Items.Clear();
                        }
                        else { txtNivel.Text = string.Empty; dgvUbicacion.DataSource = null; }
                    }
                }
                else
                {
                    txtNivel.Text = string.Empty;
                    dgvUbicacion.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbClienteEntradaAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                List<clsParametro> parametro = new List<clsParametro>
                {
                    new clsParametro("@PO", cmbPoEntradaAlmacen.Text),
                    new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@POSolamente", cbPOSolamente.Checked)
                };
                dgvAlmacen.DataSource = f.ConsultaTablaGeneral("ubicacion_Entrada_ConsultaCajas", parametro);
                dgvAlmacen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void cmbFacturacionEntradaAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                List<clsParametro> parametro = new List<clsParametro>
                {
                    new clsParametro("@PO", cmbPoEntradaAlmacen.Text),
                    new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@POSolamente", cbPOSolamente.Checked)
                };
                dgvAlmacen.DataSource = f.ConsultaTablaGeneral("ubicacion_Entrada_ConsultaCajas", parametro);
                dgvAlmacen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void cmbTerminadoEntradaAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                List<clsParametro> parametro = new List<clsParametro>
                {
                    new clsParametro("@PO", cmbPoEntradaAlmacen.Text),
                    new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()),
                    new clsParametro("@POSolamente", cbPOSolamente.Checked)
                };
                dgvAlmacen.DataSource = f.ConsultaTablaGeneral("ubicacion_Entrada_ConsultaCajas", parametro);
                dgvAlmacen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                guardarUbicacion();
            }
        }

        private void txtIDCaja_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    guardarUbicacion();
                    txtIDCaja.Text = string.Empty;
                    ConsultaCajasPO();
                    consultaUbicacion();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void guardarUbicacion()
        {
            try
            {
                if (txtUbicacionID.SelectedValue.ToString() != string.Empty && txtIDCaja.Text != string.Empty)
                {
                    if (txtNivel.Text != string.Empty)
                    {
                        int ubicacion_id = Convert.ToInt32(txtUbicacionID.SelectedValue.ToString());
                        int caja_id = Convert.ToInt32(txtIDCaja.Text);
                        int estaentablapo = 0;
                        bool? pOSolamente = false;
                        bool? ContadorEntrada = false;
                        ContadorEntrada = cbContadorEntrada.Checked;
                        pOSolamente = cbPOSolamente.Checked;
                        int yaesta = 0;
                        if (ContadorEntrada == true)
                        {
                            yaesta = 0;
                        }
                        else
                        {
                            yaesta = f.ConsultarEntrada(caja_id);
                        }

                        if (yaesta == 1)
                        {
                            txtIDCaja.Text = string.Empty;
                            lblError.Items.Add(" la Caja ya esta escaneada" + caja_id + "!" + DateTime.Now.ToString());
                        }
                        else
                        {

                            //comprobar si esta en po la caja
                            estaentablapo = f.ComprobarCajaPO(cmbPoEntradaAlmacen.Text
                                             , cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()
                                             , cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()
                                             , cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()
                                             , caja_id
                                             , pOSolamente);
                            if (estaentablapo == 1)
                            {
                                ubicacion_Entrada_ComprobarCajaResult comprobar = f.ComprobarCaja(caja_id);
                                if (comprobar.nombre != null)
                                {
                                    lblError.Items.Add(" Esta caja " + txtIDCaja.Text + " esta en la ubicacion " + comprobar.nombre + " y la moviste a R" + txtUbicacionID.SelectedValue.ToString() + "  " + DateTime.Now.ToString());
                                    txtTotalEscaneado.Text = (Convert.ToInt32(txtTotalEscaneado.Text) + 1).ToString();
                                    //GUARDA UBICACION DE LA CAJA
                                    int r = f.GuardaUbicacion(ubicacion_id
                                                              , caja_id
                                                              , cmbPoEntradaAlmacen.Text
                                                              , (cmbClienteEntradaAlmacen.Text == "NA" ? 1 : Convert.ToInt32(cmbClienteEntradaAlmacen.SelectedValue))
                                                              , (cmbFacturacionEntradaAlmacen.Text == "NA" ? 1 : Convert.ToInt32(cmbFacturacionEntradaAlmacen.SelectedValue))
                                                              , (cmbTerminadoEntradaAlmacen.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminadoEntradaAlmacen.SelectedValue))
                                                              , usu[0].id);

                                    if (r == 0)
                                    {
                                        lblError.Items.Add(" La capacidad de la ubicacion esta llena! " + caja_id + " " + DateTime.Now.ToString());
                                    }
                                }
                                else
                                {
                                    txtTotalEscaneado.Text = (Convert.ToInt32(txtTotalEscaneado.Text) + 1).ToString();
                                    //GUARDA UBICACION DE LA CAJA
                                    int r = f.GuardaUbicacion(ubicacion_id
                                                              , caja_id
                                                              , cmbPoEntradaAlmacen.Text
                                                              , (cmbClienteEntradaAlmacen.Text == "NA" ? 1 : Convert.ToInt32(cmbClienteEntradaAlmacen.SelectedValue))
                                                              , (cmbFacturacionEntradaAlmacen.Text == "NA" ? 1 : Convert.ToInt32(cmbFacturacionEntradaAlmacen.SelectedValue))
                                                              , (cmbTerminadoEntradaAlmacen.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminadoEntradaAlmacen.SelectedValue))
                                                              , usu[0].id);

                                    if (r == 0)
                                    {
                                        lblError.Items.Add(" La capacidad de la ubicacion esta llena! " + caja_id + " " + DateTime.Now.ToString());
                                    }
                                    else
                                    {
                                        //RECARGA TABLA
                                        if (!iniciando)
                                        {
                                            List<clsParametro> parametro = new List<clsParametro>
                                            {
                                                new clsParametro("@PO", cmbPoEntradaAlmacen.Text),
                                                new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()),
                                                new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()),
                                                new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()),

                                                new clsParametro("@POSolamente", cbPOSolamente.Checked)
                                            };
                                            dgvAlmacen.DataSource = f.ConsultaTablaGeneral("ubicacion_Entrada_ConsultaCajas", parametro);
                                            dgvAlmacen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                                            List<ubicacion_Entrada_ConsultaUbicacionDetalleIDResult> ubicacionDetalle = f.ConsultaUbicacionDetalleID(ubicacion_id);
                                            dgvUbicacion.DataSource = ubicacionDetalle;
                                            dgvUbicacion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lblError.Items.Add(" la Caja " + caja_id + " no es de este PO!" + DateTime.Now.ToString());
                            }
                        }
                    }
                    else
                    {
                        lblError.Items.Add(" Favor de escanear o teclear una ubicacion correcta!" + DateTime.Now.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Items.Add(ex.Message + DateTime.Now.ToString());
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    txtCajaidMover.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void txtCajaidMover_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    //cambiar de ubicacion
                    MoverUbicacion();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clbT_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            if (!iniciando)
            {
                string talla = string.Empty;
                if (e.NewValue == CheckState.Checked)
                {
                    List<clsParametro> parametro = new List<clsParametro>
                    {
                        new clsParametro("@po", cmbPOSalidaAlmacen.Text)
                    };
                    foreach (string s in clbT.CheckedItems)
                    {
                        talla += s + ",";
                    }
                    talla += clbT.SelectedItem.ToString() + ",";
                    parametro.Add(new clsParametro("@talla", talla));
                    DataTable x = f.ConsultaTablaGeneral("ubicacion_Salida_ConsultaPOTallasCantidad", parametro);
                    if (x.Rows.Count > 0)
                    {
                        dgvSalida.DataSource = x;
                        dgvSalida.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
                else
                {
                    List<clsParametro> parametro = new List<clsParametro>
                    {
                        new clsParametro("@po", cmbPOSalidaAlmacen.Text)
                    };
                    List<object> objChecked = clbT.CheckedItems.Cast<object>().ToList();

                    objChecked.Remove(clbT.SelectedItem.ToString());

                    foreach (string s in objChecked)
                    {
                        talla += s + ",";
                    }
                    parametro.Add(new clsParametro("@talla", talla));
                    DataTable x = f.ConsultaTablaGeneral("ubicacion_Salida_ConsultaPOTallasCantidad", parametro);
                    if (x.Rows.Count > 0)
                    {
                        dgvSalida.DataSource = x;
                        dgvSalida.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    else
                    {
                        dgvSalida.DataSource = null;

                    }
                }
            }
        }

        private void cmbPOSalidaAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!iniciando)
                {
                    string talla = string.Empty;
                    if (clbT.CheckedItems.Count > 0)
                    {
                        List<clsParametro> parametro = new List<clsParametro>
                        {
                            new clsParametro("@po", cmbPOSalidaAlmacen.Text)
                        };
                        foreach (string s in clbT.CheckedItems)
                        {
                            talla += s + ",";
                        }
                        parametro.Add(new clsParametro("@talla", talla));
                        DataTable x = f.ConsultaTablaGeneral("ubicacion_Salida_ConsultaPOTallasCantidad", parametro);
                        if (x.Rows.Count > 0)
                        {
                            dgvSalida.DataSource = x;
                            dgvSalida.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }
                        else
                        {
                            dgvSalida.DataSource = null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSeleccionarTodo_Click(object sender, EventArgs e)
        {
            try
            {
                ActualizarTablaSalida();
                clbT.SelectedIndex = 0;
                Cursor.Current = Cursors.WaitCursor;
                for (int i = 0; i < clbT.Items.Count; i++)
                {
                    clbT.SetItemChecked(i, true);
                }
                Cursor.Current = Cursors.Default;
                txtIDCajaSalida.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDeseleccionarTodo_Click(object sender, EventArgs e)
        {
            try
            {
                ActualizarTablaSalida();
                Cursor.Current = Cursors.WaitCursor;
                for (int i = 0; i < clbT.Items.Count; i++)
                {
                    clbT.SetItemChecked(i, false);
                }
                dgvSalida.DataSource = null;
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtIDCajaSalida_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    guardarSalida();
                }
            }
            catch (Exception ex)
            {
                lblErrorEmb.Items.Add(ex.Message);
            }
        }

        public void guardarSalida()
        {
            if (txtIDCajaSalida.Text != string.Empty)
            {
                int id_caja = txtIDCajaSalida.Text == string.Empty ? 0 : Convert.ToInt32(txtIDCajaSalida.Text);
                int estaentablapo = 0;
                bool? pOSolamente = true;

                bool? ContadorSalida = false;
                ContadorSalida = cbContadorSalida.Checked;
                int yaesta = 0;
                if (ContadorSalida == true)
                {
                    yaesta = 0;
                }
                else
                {
                    yaesta = f.ConsultarSalida(id_caja);
                }

                if (yaesta == 1)
                {
                    txtIDCajaSalida.Text = string.Empty;
                    lblErrorEmb.Items.Add(" la Caja ya esta escaneada" + id_caja + "!" + DateTime.Now.ToString());
                }
                else
                {
                    //comprobar si esta en po la caja
                    estaentablapo = f.ComprobarCajaPO(cmbPOSalidaAlmacen.Text
                                     , "1"
                                     , "1"
                                     , "1"
                                     , id_caja
                                     , pOSolamente);
                    if (estaentablapo == 1)
                    {
                        f.GuardarSalida(id_caja, usu[0].id);
                        txtTotalEscaneadoEmb.Text = (Convert.ToInt32(txtTotalEscaneadoEmb.Text) + 1).ToString();
                        txtIDCajaSalida.Text = string.Empty;
                    }
                    else
                    {

                        txtIDCajaSalida.Text = string.Empty;
                        lblErrorEmb.Items.Add(" la Caja " + id_caja + " no es de este PO!" + DateTime.Now.ToString());
                    }

                    // ActualizarTablaSalida();
                }
                txtIDCajaSalida.Text = string.Empty;
            }
        }

        private void btnGuardarSalida_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                guardarSalida();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ActualizarTablaSalida()
        {
            try
            {
                if (!iniciando)
                {
                    string talla = string.Empty;
                    if (clbT.CheckedItems.Count > 0)
                    {
                        List<clsParametro> parametro = new List<clsParametro>
                        {
                            new clsParametro("@po", cmbPOSalidaAlmacen.Text)
                        };
                        foreach (string s in clbT.CheckedItems)
                        {
                            talla += s + ",";
                        }
                        parametro.Add(new clsParametro("@talla", talla));
                        DataTable x = f.ConsultaTablaGeneral("ubicacion_Salida_ConsultaPOTallasCantidad", parametro);
                        if (x.Rows.Count > 0)
                        {
                            dgvSalida.DataSource = x;
                            dgvSalida.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }
                        else
                        {
                            dgvSalida.DataSource = null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnTerminar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                f.Terminar(usu[0].id);
                ActualizarTablaSalida();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGuardarMovimiento_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                MoverUbicacion();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void MoverUbicacion()
        {
            if (txtNuevaUbicacion.Text != string.Empty && txtCajaidMover.Text != string.Empty)
            {
                int ubicacionID = txtNuevaUbicacion.Text != string.Empty ? Convert.ToInt32(txtNuevaUbicacion.Text) : 0;
                int cajaid = txtCajaidMover.Text != string.Empty ? Convert.ToInt32(txtCajaidMover.Text) : 0;
                int x = f.MoverUbicacion(usu[0].id, ubicacionID, cajaid);
                if (x == 1)
                {
                    txtCajaidMover.Text = "";
                    txtCajaidMover.Focus();
                    // MessageBox.Show("Se cambio correctamente a la ubicacion: " + ubicacionID + " la caja: " + cajaid);
                }
                else
                {
                    MessageBox.Show("La capacidad de la ubicacion esta llena!");
                }
            }
        }

        private void btnBajaAlmacen_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja la caja " + txtBajaCajaID.Text.Trim(), "Baja Caja", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    bool baja = f.BajaCaja(txtBajaCajaID.Text, usu[0].id);
                    if (baja)
                    {
                        MessageBox.Show("se elimino correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("la caja no existe en la base de datos.");
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

        private void btnImprimirEtiquetaAlmacen_Click(object sender, EventArgs e)
        {
            try
            {
                imprimeEtiquetaTarima();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void imprimeEtiquetaTarima()
        {
            try
            {
                int id = 0;
                try { id = Convert.ToInt32(txtIDReimpresionAlmacen.Text); } catch (Exception) { id = 0; }
                List<ubicacion_Entrada_ConsultaUbicacionIDResult> consulta = f.ConsultaUbicacionID(id);

                if (consulta.Count > 0)
                {
                    string nivel = consulta[0].nivel.ToString();
                    BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                    {
                        IncludeLabel = true
                    };
                    Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                       , consulta[0].id.ToString()
                                                       , Color.Black
                                                       , Color.White
                                                       , 345
                                                       , 60);
                    //370
                    //520
                    List<EtiquetaCajaModificada> lem = new List<EtiquetaCajaModificada>();
                    EtiquetaCajaModificada em = new EtiquetaCajaModificada
                    {
                        codigoBarras = codigoBarras,
                        nivel = nivel,
                        TipoCarton = consulta[0].nombre,
                        ProductCode = consulta[0].descripcion
                    };
                    lem.Add(em);

                    ReporteAlmacen report = new ReporteAlmacen
                    {
                        DataSource = lem
                    };
                    report.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool = new ReportPrintTool(report);
                    tool.ShowPreview();
                    // tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                    //tool.PrintDialog(); //muestra a que impresora se va a mandar
                    //tool.Print(); //imprime de golpe
                    txtIDReimpresionAlmacen.Text = "";
                }
                else
                {
                    MessageBox.Show("Favor de ingresar el numero correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //private void txtUbicacionID_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Cursor.Current = Cursors.WaitCursor;
        //        consultaUbicacion();
        //        Cursor.Current = Cursors.Default;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (!iniciando)
                {
                    ConsultaCajasPO();
                    consultaUbicacion();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCaja_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {
                if (e.KeyChar == (int)Keys.Enter)
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
                            MessageBox.Show("la caja no existe en la base de datos.");
                        }
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtNuevaUbicacion_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbPOSolamente_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!iniciando)
                {
                    ConsultaCajasPO();
                    //txtIDCaja.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtUbicacionID_SelectedIndexChanged(object sender, EventArgs e)
        {
            consultaUbicacion();
            txtIDCaja.Focus();
        }

        private void txtIDReimpresionAlmacen_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    imprimeEtiquetaTarima();
                    txtIDReimpresionAlmacen.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
