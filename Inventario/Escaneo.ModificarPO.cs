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
    // Pestana ModificarPO
    public partial class Escaneo
    {
        public int validarModificarPO()
        {
            try
            {
                if (string.IsNullOrEmpty(txtEstiloPOMod.Text) || string.IsNullOrEmpty(txtDescription.Text) || dgvModificarPO.RowCount < 1)
                {
                    MessageBox.Show("Favor de llenar todos los campos.");

                    return 0;

                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private void label80_Click(object sender, EventArgs e)
        {

        }

        private void cmbPOMod_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!iniciando)
                {
                    if (cmbPOMod.Text != "")
                    {
                        iniciando = true;
                        txtEstiloPOMod.Text = "";
                        txtDescription.Text = "";
                        txtColorMod.Text = "";
                        cmbTallaMod.SelectedIndex = -1;
                        txtCantPrendCaja.Text = "";
                        txtCodigoUPC.Text = "";
                        LimpiarCamposModificacionPO();
                        List<ConsultaTodoPOResult> ListPO = f.ConsultaTodoPO(cmbPOMod.Text);
                        foreach (ConsultaTodoPOResult objPO in ListPO)
                        {
                            switch (objPO.po_item)
                            {
                                case "1000":
                                    cmbMarcaPOMod.Text = "LEVIS";
                                    txtColorMod.Enabled = false;
                                    txtDescription.Enabled = false;
                                    txtEstiloPOMod.Enabled = false;
                                    f.ConsultaTallasXMarca(cmbTallaMod, "LEVIS");
                                    cmbTallaMod.SelectedIndex = 0;
                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    AgregarTallasModificacion(objPO.size_izquierdo + " x " + objPO.size_derecho, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString());
                                    break;
                                case "99":
                                    cmbMarcaPOMod.Text = "PREPACK";
                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    AgregarTallasModificacion(objPO.size_izquierdo + " x " + objPO.size_derecho, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString());
                                    break;
                                case "88":
                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    cmbMarcaPOMod.Text = "TARGET";
                                    f.ConsultaTallasXMarca(cmbTallaMod, "TARGET");
                                    cmbTallaMod.SelectedIndex = 0;
                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    txtDescription.Text = objPO.itemDescription.ToString();
                                    txtColorMod.Text = objPO.color;
                                    AgregarTallasModificacion(objPO.size_izquierdo, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString());
                                    break;
                                case "77":
                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    cmbMarcaPOMod.Text = "ZUMIES";
                                    f.ConsultaTallasXMarca(cmbTallaMod, "ZUMIES");
                                    cmbTallaMod.SelectedIndex = 0;
                                    txtzCantidadCajasModif.Visible = true;
                                    txtzCantidadCajasModif.Enabled = true;
                                    lblCantidadCajasModif.Visible = true;
                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    txtDescription.Text = objPO.itemDescription.ToString();
                                    txtColorMod.Text = objPO.color;
                                    txtzCantidadCajasModif.Text = objPO.cantidadCajas.ToString();
                                    AgregarTallasModificacion(objPO.size_izquierdo, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString(), objPO.cantidadCajas.ToString(), objPO.itemDescription.ToString(), objPO.color);
                                    break;
                                case "66":
                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    cmbMarcaPOMod.Text = "ROCKY";
                                    f.ConsultaTallasXMarca(cmbTallaMod, "ROCKY");
                                    cmbTallaMod.SelectedIndex = 0;
                                    txtzCantidadCajasModif.Visible = true;
                                    txtzCantidadCajasModif.Enabled = true;
                                    lblCantidadCajasModif.Visible = true;
                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    txtDescription.Text = objPO.itemDescription.ToString();
                                    txtColorMod.Text = objPO.color;
                                    txtzCantidadCajasModif.Text = objPO.cantidadCajas.ToString();
                                    AgregarTallasModificacion(objPO.size_izquierdo, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString(), objPO.cantidadCajas.ToString(), objPO.itemDescription.ToString(), objPO.color);
                                    break;
                                case "55":
                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    cmbMarcaPOMod.Text = "BENS";
                                    f.ConsultaTallasXMarca(cmbTallaMod, "BENS");
                                    cmbTallaMod.SelectedIndex = 0;
                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    txtDescription.Text = objPO.itemDescription.ToString();
                                    txtColorMod.Text = objPO.color;
                                    AgregarTallasModificacion(objPO.size_izquierdo, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString());
                                    break;
                                default:
                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    cmbMarcaPOMod.Text = "LEVIS";
                                    f.ConsultaTallasXMarca(cmbTallaMod, "LEVIS");
                                    cmbTallaMod.SelectedIndex = 0;
                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    AgregarTallasModificacion(objPO.size_izquierdo + " x " + objPO.size_derecho, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString());
                                    break;
                            }
                        }
                        iniciando = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        public void LimpiarCamposModificacionPO()
        {
            try
            {
                for (int i = dgvModificarPO.Rows.Count - 1; i >= 0; i--)
                {
                    dgvModificarPO.Rows.RemoveAt(i);
                }

                foreach (DataGridViewRow item in dgvModificarPO.SelectedRows)
                {
                    dgvModificarPO.Rows.RemoveAt(item.Index);

                }
                tablaAltaPO = new DataTable();
                tablaAltaPO.Columns.Add("Talla", typeof(string));
                tablaAltaPO.Columns.Add("Cantidad", typeof(long));
                tablaAltaPO.Columns.Add("Codigo UPC", typeof(string));
                tablaAltaPO.Columns.Add("idSize", typeof(int));

                tablaZumies = new DataTable();
                tablaZumies.Columns.Add("Talla", typeof(string));
                tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                tablaZumies.Columns.Add("idSize", typeof(int));
                tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                tablaZumies.Columns.Add("itemDescription", typeof(string));
                tablaZumies.Columns.Add("color", typeof(string));

                tablaBens = new DataTable();
                tablaBens.Columns.Add("Talla", typeof(string));
                tablaBens.Columns.Add("Codigo UPC", typeof(string));
                tablaBens.Columns.Add("idSize", typeof(int));
                tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                tablaBens.Columns.Add("itemDescription", typeof(string));
                tablaBens.Columns.Add("color", typeof(string));

                tablaTarget = new DataTable();
                tablaTarget.Columns.Add("Talla", typeof(string));
                tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                tablaTarget.Columns.Add("idSize", typeof(int));
                tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                tablaTarget.Columns.Add("itemDescription", typeof(string));
                tablaTarget.Columns.Add("color", typeof(string));

                altaZ = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AgregarTallasModificacion(string talla, string cantidad, string upc, string idsize, string CantidadCajas = "", string itemDescription = "", string color = "")
        {
            try
            {
                try
                {
                    String EstructuratablaZumies = "";
                    switch (cmbMarcaPOMod.Text)
                    {
                        case "ZUMIES":
                            EstructuratablaZumies = "Talla,Codigo UPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";
                            tablaZumies.Rows.Add(talla, upc, idsize, cantidad, CantidadCajas, itemDescription, color);
                            dgvModificarPO.DataSource = tablaZumies;
                            break;
                        case "ROCKY":
                            EstructuratablaZumies = "Talla,Codigo UPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";
                            tablaZumies.Rows.Add(talla, upc, idsize, cantidad, CantidadCajas, itemDescription, color);
                            dgvModificarPO.DataSource = tablaZumies;
                            break;
                        default:
                            tablaModificacion.Rows.Add(talla, cantidad, upc, idsize);
                            dgvModificarPO.DataSource = tablaModificacion;
                            break;
                            tablaAltaPO = new DataTable();
                            tablaZumies = new DataTable();
                            tablaBens = new DataTable();
                            tablaTarget = new DataTable();
                    }
                }
                catch (Exception ex)
                {
                    switch (cmbMarcaPOMod.Text)
                    {
                        case "ZUMIES":
                            tablaModificacion.Rows.Add(talla, cantidad, upc, idsize);
                            dgvModificarPO.DataSource = tablaModificacion;
                            break;
                        case "ROCKY":
                            tablaModificacion.Rows.Add(talla, cantidad, upc, idsize);
                            dgvModificarPO.DataSource = tablaModificacion;
                            break;
                        default:
                            tablaModificacion.Rows.Add(talla, cantidad, upc, idsize);
                            dgvModificarPO.DataSource = tablaModificacion;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGuardarModificacion_Click(object sender, EventArgs e)
        {

        }

        private void btnAplicarModificacionInv_Click(object sender, EventArgs e)
        {

        }

        private void dgvModificarPO_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {  // String EstructuratablaZumies = "Talla,Codigo UPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";
                RengloSelecionado = e.RowIndex;
                DataGridViewRow row = dgvModificarPO.Rows[RengloSelecionado];

                switch (cmbMarcaPOMod.Text)
                {
                    case "ZUMIES":
                        cmbTallaMod.Text = row.Cells[0].Value.ToString();
                        txtCodigoUPC.Text = row.Cells[1].Value.ToString();
                        txtCantPrendCaja.Text = row.Cells[3].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[4].Value.ToString();
                        break;
                    case "ROCKY":
                        cmbTallaMod.Text = row.Cells[0].Value.ToString();
                        txtCodigoUPC.Text = row.Cells[1].Value.ToString();
                        txtCantPrendCaja.Text = row.Cells[3].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[4].Value.ToString();
                        break;
                    default:
                        cmbTallaMod.Text = row.Cells[0].Value.ToString();
                        txtCantPrendCaja.Text = row.Cells[1].Value.ToString();
                        txtCodigoUPC.Text = row.Cells[2].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdateMod_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvModificarPO.RowCount > 0)
                {
                    DataGridViewRow newDatarow = dgvModificarPO.Rows[RengloSelecionado];
                    String EstructuratablaZumies = "";
                    switch (cmbMarcaPOMod.Text)
                    {
                        case "ZUMIES":

                            EstructuratablaZumies = "Talla,Codigo UPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";
                            newDatarow.Cells[0].Value = cmbTallaMod.Text;
                            newDatarow.Cells[1].Value = txtCodigoUPC.Text;
                            newDatarow.Cells[2].Value = cmbTallaMod.SelectedValue;
                            newDatarow.Cells[3].Value = txtCantPrendCaja.Text;
                            newDatarow.Cells[4].Value = txtzCantidadCajasModif.Text;
                            newDatarow.Cells[5].Value = txtDescription.Text;
                            newDatarow.Cells[6].Value = txtColorMod.Text;
                            break;
                        case "ROCKY":

                            EstructuratablaZumies = "Talla,Codigo UPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";
                            newDatarow.Cells[0].Value = cmbTallaMod.Text;
                            newDatarow.Cells[1].Value = txtCodigoUPC.Text;
                            newDatarow.Cells[2].Value = cmbTallaMod.SelectedValue;
                            newDatarow.Cells[3].Value = txtCantPrendCaja.Text;
                            newDatarow.Cells[4].Value = txtzCantidadCajasModif.Text;
                            newDatarow.Cells[5].Value = txtDescription.Text;
                            newDatarow.Cells[6].Value = txtColorMod.Text;
                            break;
                        default:

                            newDatarow.Cells[0].Value = cmbTallaMod.Text;
                            newDatarow.Cells[1].Value = txtCantPrendCaja.Text;
                            newDatarow.Cells[2].Value = txtCodigoUPC.Text;
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public int validarModificacionPOLEVIS()
        {
            try
            {
                if (string.IsNullOrEmpty(cmbPOMod.Text) && dgvModificarPO.RowCount < 1)
                {
                    MessageBox.Show("Favor de llenar todos los campos.");

                    return 0;

                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private void btnGuardarModificacion_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dgvModificarPO.RowCount > 0)
                {
                    bool baja = f.BajaPO(cmbPOMod.Text.ToString(), 1, 1, 1, usu[0].id);
                    if (baja)
                    {
                        MessageBox.Show("el po no existe en la base de datos.");
                    }
                    else
                    {

                        foreach (DataGridViewRow renglon in dgvModificarPO.Rows)
                        {
                            EtiquetaCajaModificada pd = new EtiquetaCajaModificada();
                            switch (cmbMarcaPOMod.SelectedItem)
                            {

                                case "LEVIS":
                                    if (validarModificarPO() == 1)
                                    {
                                        pd.po = cmbPOMod.Text;
                                        pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                        pd.upc = renglon.Cells[2].Value.ToString();
                                        pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                        pd.itemDescription = txtDescription.Text;
                                        pd.color = txtColorMod.Text;
                                        pd.estilo = txtEstiloPOMod.Text;
                                        pd.idusuario = usu[0].id;
                                        f.GuardaAltaPO(pd);
                                    }
                                    break;
                                case "WRANGLER":
                                    if (validarModificarPO() == 1)
                                    {
                                        pd.po = cmbPOMod.Text;
                                        pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                        pd.upc = renglon.Cells[2].Value.ToString();
                                        pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                        pd.itemDescription = txtDescription.Text;
                                        pd.color = txtColorMod.Text;
                                        pd.estilo = txtEstiloPOMod.Text;
                                        pd.idusuario = usu[0].id;
                                        f.GuardaAltaPO(pd);
                                    }
                                    break;
                                case "ZUMIES":
                                    if (validarModificarPO() == 1)
                                    {
                                        //String EstructuratablaZumies = "Talla,CodigoUPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";

                                        pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                        pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                        pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                        pd.itemDescription = txtDescription.Text;
                                        pd.color = txtColorMod.Text;
                                        pd.po = cmbPOMod.Text;
                                        pd.upc = renglon.Cells[2].Value.ToString();
                                        pd.idusuario = usu[0].id;
                                        pd.estilo = txtEstiloPOMod.Text;
                                        pd.idusuario = usu[0].id;
                                        f.GuardaZumies(pd);
                                    }
                                    break;
                                case "ROCKY":
                                    if (validarModificarPO() == 1)
                                    {
                                        // String EstructuratablaZumies = "Talla,CodigoUPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";

                                        pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                        pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                        pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                        pd.itemDescription = txtDescription.Text;
                                        pd.color = txtColorMod.Text;
                                        pd.po = cmbPOMod.Text;
                                        pd.upc = renglon.Cells[2].Value.ToString();
                                        pd.idusuario = usu[0].id;
                                        pd.estilo = txtEstiloPOMod.Text;
                                        pd.idusuario = usu[0].id;
                                        f.GuardaZumies(pd);
                                    }
                                    break;
                                case "TARGET":
                                    if (validarModificarPO() == 1)
                                    {
                                        pd.po = cmbPOMod.Text;
                                        pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                        pd.upc = renglon.Cells[2].Value.ToString();
                                        pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                        pd.idusuario = usu[0].id;
                                        pd.assembly = cmbPOMod.Text;
                                        f.GuardarTarget(pd);
                                    }
                                    break;
                            }
                            alta = 0;
                        }
                        f.ConsultaPO(cmbPO);
                        cmbPO.SelectedIndex = -1;
                        cmbTallaMod.Text = "";
                        txtCantPrendCaja.Text = "";
                        txtCodigoUPC.Text = "";
                        txtColorMod.Text = "";
                        txtDescription.Text = "";
                        txtEstiloPOMod.Text = "";
                        cmbPOMod.Text = "";
                        LimpiarCamposModificacionPO();
                        MessageBox.Show("Termino con Exito!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnAddMod_Click(object sender, EventArgs e)
        {
            String EstructuratablaZumies = "";
            switch (cmbMarcaPOMod.Text)
            {
                case "ZUMIES":
                    EstructuratablaZumies = "Talla,Codigo UPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";
                    AgregarTallasModificacion(cmbTallaMod.Text
                                              , txtCantPrendCaja.Text
                                              , txtCodigoUPC.Text
                                              , cmbTallaMod.SelectedValue.ToString()
                                              , txtzCantidadCajasModif.Text
                                              , txtDescription.Text
                                              , txtColorMod.Text);
                    break;
                case "ROCKY":
                    EstructuratablaZumies = "Talla,Codigo UPC,idSize,CantidadPrendas,CantidadCajas,itemDescription,color";
                    AgregarTallasModificacion(cmbTallaMod.Text
                        , txtCantPrendCaja.Text, txtCodigoUPC.Text, cmbTallaMod.SelectedValue.ToString(), txtzCantidadCajasModif.Text, txtDescription.Text, txtColorMod.Text);

                    break;
                default:
                    AgregarTallasModificacion(cmbTallaMod.Text, txtCantPrendCaja.Text, txtCodigoUPC.Text, cmbTallaMod.SelectedValue.ToString());
                    break;
            }
        }

        private void btnDelMod_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvModificarPO.RowCount > 0)
                {
                    RengloSelecionado = dgvModificarPO.CurrentCell.RowIndex;
                    dgvModificarPO.Rows.RemoveAt(RengloSelecionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancelaMod_Click(object sender, EventArgs e)
        {
            txtEstiloPOMod.Text = "";
            txtDescription.Text = "";
            txtColorMod.Text = "";
            cmbTallaMod.SelectedIndex = -1;
            txtCantPrendCaja.Text = "";
            txtCodigoUPC.Text = "";
            LimpiarCamposModificacionPO();
        }

        private void btnAplicarModificacionInv_Click_1(object sender, EventArgs e)
        {
            /*PO*/
            try
            {
                List<ConsultaInventarioPOResult> x = f.ConsultaInventarioPO(cmbPOMod.Text);
                if (x.Count > 0)
                {

                    string respuesta = f.ActualizaInventario(cmbPOMod.Text, usu[0].id);

                    if (respuesta == "0")
                    {
                        MessageBox.Show("No se Actualizo por que no hay registros");
                    }
                    else
                    {
                        MessageBox.Show("Se Actualizo Correctamente");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Cursor.Current = Cursors.Default;
            }


            try
            {
                if (dgvModificarPO.RowCount > 0)
                {
                    foreach (DataGridViewRow renglon in dgvModificarPO.Rows)
                    {
                        EtiquetaCajaModificada pd = new EtiquetaCajaModificada();
                        switch (cmbMarcaPOMod.SelectedItem)
                        {
                            case "LEVIS":
                                if (validarModificacionPOLEVIS() == 1)
                                {
                                    pd.po = cmbPOMod.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    //pd.idSize = renglon.Cells[3].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    /*TOTAL DE PRENDAS ESCANEADAS*/
                                    string[] separadas = renglon.Cells[3].Value.ToString().Split('x');
                                    /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                                    List<ConsultaProductosBensResult> x = f.ConsultaProductosBens(pd.po.ToString(), separadas[0].ToString().Trim(), separadas[1].ToString().Trim());
                                    if (x.Count > 0)
                                    {
                                        contador = 0;
                                        txtCartonSize.Text = "";
                                        txtSize.Text = "";
                                        txtProductCode.Text = x[0].Estilo.ToString();
                                        anterior.Add(id);
                                        upc = cmbPOItem.Text;
                                        EtiquetaBens clase = new EtiquetaBens();
                                        clase.id = x[0].id;
                                        clase.po_varchar = x[0].po_varchar;
                                        clase.po = cmbPO.Text;
                                        clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                                        clase.poItem = "77";
                                        clase.ProductCode = x[0].Estilo;
                                        clase.Size = x[0].size_izquierdo;
                                        clase.size_derecho = "";
                                        clase.TipoCarton = "0";
                                        clase.upc = x[0].UPC;
                                        clase.Fecha = DateTime.Now;
                                        clase.CartonLeft = "";
                                        clase.CartonRight = "";
                                        clase.usuario = usu[0].nombre;
                                        clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                                        clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                                        clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                                        clase.cliente = cmbCliente.Text;
                                        clase.factura = cmbFactura.Text;
                                        clase.terminado = cmbTerminado.Text;
                                        clase.Vendor = x[0].itemDescription;
                                        clase.ESTILO = x[0].Estilo;
                                        clase.DESCRIPTION = x[0].itemDescription;
                                        clase.QUANTITY = Convert.ToString(x[0].Cantidad);
                                        clase.Cantidad = pd.Cantidad;
                                        clase.itemDescription = x[0].itemDescription;
                                        clase.color = x[0].Color;
                                        clase.size_izquierdo = x[0].size_izquierdo;
                                        clase.size_derecho = x[0].size_derecho;
                                        clase.cn_tag_num = Convert.ToInt32(x[0].CartonNumber);
                                        EtiquetaCajaModificada respuesta = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);

                                        clase.id_string = respuesta.codigoBarrasTextoB.ToString();


                                        //clase.id_string = x[0].Estilo + clase.size_izquierdo + clase.size_derecho + clase.QUANTITY + clase.id;
                                        clase.anio = respuesta.codigoBarrasTextoB.ToString();
                                    }
                                    else
                                    {
                                        // MessageBox.Show("Ya se escanearon todas las cajas de la Talla");
                                    }
                                    //f.GuardaAltaPO(pd);
                                    //f.ConsultaPO(cmbPO);
                                    //cmbPO.SelectedIndex = -1;
                                    ///

                                }
                                break;
                            case "ZUMIES":
                                if (validarZumies() == 1)
                                {
                                    pd.po = cmbPOMod.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.idusuario = usu[0].id;
                                    pd.estilo = txtEstiloPOMod.Text;
                                    pd.itemDescription = txtDescription.Text;
                                    pd.color = txtColorMod.Text;
                                    pd.idusuario = usu[0].id;
                                    f.GuardaZumies(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;

                                }
                                break;

                            case "ROCKY":
                                if (validarZumies() == 1)
                                {
                                    pd.po = cmbPOMod.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.idusuario = usu[0].id;
                                    pd.estilo = txtEstiloPOMod.Text;
                                    pd.itemDescription = txtDescription.Text;
                                    pd.color = txtColorMod.Text;
                                    pd.idusuario = usu[0].id;
                                    f.GuardaZumies(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;

                                }
                                break;
                            case "TARGET":
                                if (validarAltaPO() == 1)
                                {
                                    pd.po = cmbPOMod.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.idusuario = usu[0].id;
                                    pd.assembly = cmbPOMod.Text;
                                    f.GuardarTarget(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;
                                }
                                break;
                        }
                        alta = 0;
                    }

                    cmbTallaMod.Text = "";
                    txtCantPrendCaja.Text = "";
                    txtCodigoUPC.Text = "";
                    txtColorMod.Text = "";
                    txtDescription.Text = "";
                    txtEstiloPOMod.Text = "";
                    cmbPOMod.Text = "";
                    LimpiarCamposModificacionPO();
                    MessageBox.Show("Termino con Exito!");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbMarcaPOMod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
