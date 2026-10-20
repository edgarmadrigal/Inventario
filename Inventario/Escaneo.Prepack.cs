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
    // Pestana AltaPrePack (POItem 99)
    public partial class Escaneo
    {
        private void txtCantidadPrepack_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //Para obligar a que sólo se introduzcan números
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                    if (char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        //el resto de teclas pulsadas se desactivan
                        e.Handled = true;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCodigoupcPrepack_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {

                //Para obligar a que sólo se introduzcan números
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                    if (char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        //el resto de teclas pulsadas se desactivan
                        e.Handled = true;
                    }

                if (e.KeyChar == (int)Keys.Enter)
                {
                    AgregaTalla();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPoPrepack_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {
                //Para obligar a que sólo se introduzcan números
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                    if (char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        //el resto de teclas pulsadas se desactivan
                        e.Handled = true;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AgregaTalla()
        {

            try
            {
                if (txtCantidadPrepack.Text != "" && txtCodigoupcPrepack.Text != "")
                {

                    switch (cmbMarcaPrepack.SelectedItem.ToString())
                    {
                        case "LEVIS":
                            tablaPrepack.Rows.Add(cmbTallaPrepack.Text, txtCantidadPrepack.Text, txtCodigoupcPrepack.Text, cmbTallaPrepack.SelectedValue.ToString());
                            dgvPrePack2.DataSource = tablaPrepack;
                            dgvPrePack2.Columns["idSize"].Visible = false;
                            cmbTallaPrepack.SelectedIndex = 0;
                            txtCodigoupcPrepack.Text = "";

                            break;
                        case "ZUMIES":
                            f.ConsultaTallasXMarca(cmbTallaPrepack, cmbMarcaPrepack.SelectedItem.ToString());
                            txtItemDescripcionPrepack.Enabled = true;
                            cmbTallaPrepack.SelectedIndex = 0;
                            break;
                        case "ROCKY":
                            f.ConsultaTallasXMarca(cmbTallaPrepack, cmbMarcaPrepack.SelectedItem.ToString());
                            txtItemDescripcionPrepack.Enabled = true;
                            cmbTallaPrepack.SelectedIndex = 0;
                            break;
                        case "CINTAS":
                            tablaPrepack.Rows.Add(cmbTallaPrepack.Text, txtCantidadPrepack.Text, txtCodigoupcPrepack.Text, cmbTallaPrepack.SelectedValue.ToString());
                            dgvPrePack2.DataSource = tablaPrepack;
                            dgvPrePack2.Columns["idSize"].Visible = false;
                            cmbTallaPrepack.SelectedIndex = 0;
                            txtCodigoupcPrepack.Text = "";
                            break;
                        case "TARGET":

                            tablaPrepack.Rows.Add(cmbTallaPrepack.Text, txtCantidadPrepack.Text, txtCodigoupcPrepack.Text, cmbTallaPrepack.SelectedValue.ToString());
                            dgvPrePack2.DataSource = tablaPrepack;
                            dgvPrePack2.Columns["idSize"].Visible = false;
                            cmbTallaPrepack.SelectedIndex = 0;
                            txtCodigoupcPrepack.Text = "";
                            break;
                    }




                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnAgregaTalla_Click(object sender, EventArgs e)
        {
            AgregaTalla();
        }

        private void cmbSizeA_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnQuitaTalla_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPrePack2.RowCount > 0)
                {
                    RengloSelecionado = dgvPrePack2.CurrentCell.RowIndex;
                    dgvPrePack2.Rows.RemoveAt(RengloSelecionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditarPrePack_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvPrePack2.RowCount > 0)
                {
                    //RengloSelecionado = dgvPrePack2.CurrentCell.RowIndex;
                    DataGridViewRow newDatarow = dgvPrePack2.Rows[RengloSelecionado];
                    newDatarow.Cells[0].Value = cmbTallaPrepack.Text;
                    newDatarow.Cells[1].Value = txtCantidadPrepack.Text;
                    newDatarow.Cells[2].Value = txtCodigoupcPrepack.Text;
                    newDatarow.Cells[3].Value = cmbTallaPrepack.SelectedValue.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnGuardarPrepack_Click(object sender, EventArgs e)
        {
            try
            {
                if (validar() == 1)
                {
                    if (dgvPrePack2.RowCount > 0)
                    {
                        Prepack p = new Prepack();
                        int idPrepack = 0;
                        switch (cmbMarcaPrepack.SelectedItem)
                        {
                            case "LEVIS":
                                p.estilo = "99";
                                break;
                            case "TARGET":
                                p.estilo = "88";
                                p.barcode = txtBarcode.Text;
                                p.DPCI = txtItemDescripcionPrepack.Text;
                                break;
                            case "ZUMIES":
                                p.estilo = "77";
                                p.barcode = txtBarcode.Text;
                                p.DPCI = txtItemDescripcionPrepack.Text;
                                break;
                            case "ROCKY":
                                p.estilo = "66";
                                p.barcode = txtBarcode.Text;
                                p.DPCI = txtItemDescripcionPrepack.Text;
                                break;
                        }
                        p.po_numero = txtPoPrepack.Text;

                        if (p.barcode != "")
                        {
                            idPrepack = f.GuardarPrePack(p);

                            foreach (DataGridViewRow renglon in dgvPrePack2.Rows)
                            {
                                int cantidad = Convert.ToInt32(renglon.Cells[1].Value.ToString());

                                for (int i = cantidad; i > 0; i--)
                                {
                                    PrepackDetalle pd = new PrepackDetalle
                                    {
                                        idPrepack = idPrepack,
                                        size = renglon.Cells[0].Value.ToString(),
                                        cantidad = 1,
                                        upc = renglon.Cells[2].Value.ToString(),
                                        idusuario = usu[0].id,
                                        idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString())
                                    };
                                    f.GuardarPrePackDetalle(pd);
                                    altaP = 0;
                                }
                            }
                            //ActualizarPO(p.po_numero.ToString());
                            LimpiarCampos();
                            // MessageBox.Show("PREPACK: "+ idPrepack);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        public void ActualizarPO(string po_numero)
        {
            //if (!iniciando)
            //{
            //    iniciando = true;
            //    f.ConsultaPOItem(cmbPOItem, po_numero);
            //    cmbPOItem.SelectedIndex = 0;
            //    f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
            //    cmbProductCode.SelectedIndex = 0;
            //    f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
            //    iniciando = false;
            //}
        }
        public void LimpiarCampos()
        {
            try
            {
                //txtEstiloPrepack.Text = "";

                tablaPrepack = new DataTable();
                tablaPrepack.Columns.Add("Talla", typeof(string));
                tablaPrepack.Columns.Add("Cantidad", typeof(long));
                tablaPrepack.Columns.Add("Codigo UPC", typeof(string));
                tablaPrepack.Columns.Add("idSize", typeof(int));

                txtPoPrepack.Text = "";
                cmbTallaPrepack.SelectedIndex = 0;
                txtCantidadPrepack.Text = "";
                txtCodigoupcPrepack.Text = "";
                altaP = 0;

                for (int i = dgvPrePack2.Rows.Count - 1; i >= 0; i--)
                {
                    dgvPrePack2.Rows.RemoveAt(i);
                }

                foreach (DataGridViewRow item in dgvPrePack2.SelectedRows)
                {
                    dgvPrePack2.Rows.RemoveAt(item.Index);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
 // Variable utilizada para saber si hay algún TextBox vacio.
        public int validar()
        {
            try
            {
                if (string.IsNullOrEmpty(txtPoPrepack.Text) || dgvPrePack2.RowCount < 1)
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

        private void dgvPrePack2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RengloSelecionado = e.RowIndex;
                DataGridViewRow row = dgvPrePack2.Rows[RengloSelecionado];

                cmbTallaPrepack.Text = row.Cells[0].Value.ToString();
                txtCantidadPrepack.Text = row.Cells[1].Value.ToString();
                txtCodigoupcPrepack.Text = row.Cells[2].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cmbTallaPrepack_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtCodigoupcPrepack.Focus();
        }

        private void btnNuevoPrePack_Click(object sender, EventArgs e)
        {

            LimpiarCampos();
        }

        private void btnBorrarRenglonPrePack_Click(object sender, EventArgs e)
        {

            LimpiarCampos();
        }

        private void dgvPrePack2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


        private void txtCantidadPrepack_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    txtCodigoupcPrepack.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPoPrepack_KeyPress_1(object sender, KeyPressEventArgs e)
        {

            //Para obligar a que sólo se introduzcan números
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                if (char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                {
                    e.Handled = false;
                }
                else
                {
                    //el resto de teclas pulsadas se desactivan
                    e.Handled = true;
                }
        }

        private void cmbMarcaPrepack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                switch (cmbMarcaPrepack.SelectedItem.ToString())
                {
                    case "LEVIS":
                        tablaPrepack = new DataTable();
                        tablaPrepack.Columns.Add("Talla", typeof(string));
                        tablaPrepack.Columns.Add("Cantidad", typeof(long));
                        tablaPrepack.Columns.Add("Codigo UPC", typeof(string));
                        tablaPrepack.Columns.Add("idSize", typeof(int));
                        f.ConsultaTallasXMarca(cmbTallaPrepack, cmbMarcaPrepack.SelectedItem.ToString());
                        txtItemDescripcionPrepack.Enabled = false;
                        txtBarcode.Enabled = false;
                        cmbTallaPrepack.SelectedIndex = 0;
                        break;
                    case "ZUMIES":
                        f.ConsultaTallasXMarca(cmbTallaPrepack, cmbMarcaPrepack.SelectedItem.ToString());
                        txtItemDescripcionPrepack.Enabled = true;
                        cmbTallaPrepack.SelectedIndex = 0;
                        break;
                    case "ROCKY":
                        f.ConsultaTallasXMarca(cmbTallaPrepack, cmbMarcaPrepack.SelectedItem.ToString());
                        txtItemDescripcionPrepack.Enabled = true;
                        cmbTallaPrepack.SelectedIndex = 0;
                        break;
                    case "CINTAS":
                        tablaPrepack = new DataTable();
                        tablaPrepack.Columns.Add("Talla", typeof(string));
                        tablaPrepack.Columns.Add("Cantidad", typeof(long));
                        tablaPrepack.Columns.Add("Codigo UPC", typeof(string));
                        tablaPrepack.Columns.Add("idSize", typeof(int));
                        f.ConsultaTallasXMarca(cmbTallaPrepack, cmbMarcaPrepack.SelectedItem.ToString());
                        txtItemDescripcionPrepack.Enabled = false;
                        cmbTallaPrepack.SelectedIndex = 0;
                        break;
                    case "TARGET":
                        tablaPrepack = new DataTable();
                        tablaPrepack.Columns.Add("Talla", typeof(string));
                        tablaPrepack.Columns.Add("Cantidad", typeof(long));
                        tablaPrepack.Columns.Add("Codigo UPC", typeof(string));
                        tablaPrepack.Columns.Add("idSize", typeof(int));
                        f.ConsultaTallasXMarca(cmbTallaPrepack, cmbMarcaPrepack.SelectedItem.ToString());
                        txtItemDescripcionPrepack.Enabled = true;
                        cmbTallaPrepack.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void Escaneo_PREPACK(ref string marca, ref List<ConsultaUPC_VarcharResult> ListUpc, string po_numero)
        {
            /*UPC CORRECTO*/
            if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) - 1)
            {
                /*TOTAL DE PRENDAS ESCANEADAS*/
                #region TOTAL DE PRENDAS ESCANEADAS
                contador = 0;
                txtUPCScann.Text = string.Empty;
                txtUnitsReq.Text = txtUnitsReq.Text;
                txtUnitsScan.Text = "0";
                txtUnitsRemai.Text = txtUnitsReq.Text;
                dgvEscan.Rows[0].Selected = true;
                dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                txtUnitsScan.Text = (0).ToString();//2100017195
                txtCartonRq.Text = "1";
                txtCartonsPacked.Text = "0";
                txtCartonsReamaining.Text = "1";
                txtUPCScann.Focus();
                string[] separadas;

                separadas = cmbSizes.Text.Split('x');

                int? esTarget = f.ConsultaPrepack(cmbPO.Text);
                if (esTarget > 0)
                {
                    marca = "TARGET";
                }
                else
                {
                    marca = "LEVIS";
                }
                switch (marca)
                {
                    case "TARGET":
                        #region TARGET
                        contador = 0;
                        txtUPCScann.Text = string.Empty;
                        txtUnitsReq.Text = txtUnitsReq.Text;
                        txtUnitsScan.Text = "0";
                        txtUnitsRemai.Text = txtUnitsReq.Text;
                        dgvEscan.Rows[0].Selected = true;
                        dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                        txtUnitsScan.Text = (0).ToString();
                        txtUPCScann.Focus();
                        ListUpc = f.ConsultaUPC(po_numero, upc);
                        id = ListUpc[0].id;
                        upc = ListUpc[0].upc;
                        separadas = ListUpc[0].Size.Split('x');

                        int? esprepack = f.ConsultaPrepack(cmbPO.Text);

                        if (esprepack > 0)
                        {
                            List<ConsultaPrepackDetalleResult> ListprepackTarget = f.ConsultaPrepackDetalle(Convert.ToInt32(esprepack));
                            if (ListprepackTarget.Count > 0)
                            {
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                //*aquiii*/
                                List<ConsultaProductosZumiesResult> po = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString().Trim());
                                if (po[0].NumeroCaja < po[0].totalCajasPO)
                                {
                                    #region Guardar en Inventario
                                    decimal? cantidadTotalCajasPrepack = 0;
                                    string barcode = "";
                                    int? idPrepack = 0;
                                    string DPCI = "";
                                    contador = 0;
                                    txtCartonSize.Text = "";
                                    txtSize.Text = "";
                                    txtProductCode.Text = po[0].estilo.ToString();
                                    anterior.Add(id);
                                    upc = cmbPOItem.Text;
                                    EtiquetaZUMIES clase = new EtiquetaZUMIES();
                                    //clase.id = po[0].id;
                                    //clase.ProductCode = po[0].estilo;
                                    //clase.Size = po[0].Talla;
                                    clase.cn_tag_num = Convert.ToInt32(po[0].NumeroCaja);
                                    clase.Cantidad = Convert.ToDecimal(po[0].cantidad);
                                    clase.Carton = po[0].NumeroCaja + 1;
                                    clase.upc = po[0].upc;
                                    clase.assembly = po[0].color;
                                    clase.QUANTITY = Convert.ToString(po[0].cantidad);

                                    txtCartonNumber.Text = (po[0].NumeroCaja).ToString();
                                    if (cmbPOItem.Text == "77")
                                    {
                                        clase.poItem = "77";
                                        clase.size_izquierdo = separadas[0].ToString();
                                        clase.Vendor = po[0].itemDescription;
                                        clase.ESTILO = po[0].estilo;
                                        clase.DESCRIPTION = po[0].itemDescription + " COLOR:" + po[0].color + " SIZE:" + po[0].Talla;
                                    }
                                    else if (cmbPOItem.Text == "88")
                                    {
                                        clase.size_izquierdo = ListUpc[0].Size.ToString();
                                        clase.Vendor = po[0].itemDescription;
                                        clase.ESTILO = po[0].estilo;
                                        clase.DESCRIPTION = po[0].itemDescription;
                                    }


                                    foreach (ConsultaPrepackDetalleResult objPrepack in ListprepackTarget)
                                    {
                                        cantidadTotalCajasPrepack = objPrepack.cantidad + cantidadTotalCajasPrepack;
                                        barcode = objPrepack.barcode;
                                        idPrepack = objPrepack.idPrepack;
                                        DPCI = objPrepack.DPCI;
                                    }

                                    clase.id = idPrepack;
                                    clase.po = cmbPO.Text;
                                    clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                                    clase.poItem = "99";
                                    clase.ProductCode = "88";
                                    clase.Size = "ASSORTMENT";
                                    clase.size_izquierdo = "ASSORTMENT";
                                    clase.size_derecho = "";
                                    clase.TipoCarton = "1";
                                    clase.upc = barcode;
                                    clase.Fecha = DateTime.Now;
                                    clase.CartonLeft = "";
                                    clase.CartonRight = "";
                                    clase.Cantidad = Convert.ToDecimal(cantidadTotalCajasPrepack);
                                    clase.usuario = usu[0].nombre;
                                    clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                                    clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                                    clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                                    clase.cliente = "88";
                                    clase.factura = cmbFactura.Text;
                                    clase.terminado = cmbTerminado.Text;
                                    clase.Vendor = "0";
                                    clase.DPCI = DPCI.Trim();
                                    clase.ESTILO = "ASSORTMENT";
                                    clase.DESCRIPTION = DPCI.Trim();
                                    clase.itemDescription = DPCI.Trim();
                                    clase.QUANTITY = Convert.ToString(cantidadTotalCajasPrepack);
                                    clase.COUNTRY = "MEXICO";
                                    clase.id_inventario = f.GuardaInventarioZumies(clase, usu[0].id);

                                    #endregion

                                    #region RECARGAR COMBOBOX
                                    iniciando = true;
                                    f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                                    cmbPOItem.SelectedIndex = 0;
                                    f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                                    cmbProductCode.SelectedIndex = 0;
                                    f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                                    cmbSizes.SelectedText = ListUpc[0].Size;
                                    contador = 0;

                                    #endregion

                                    #region IMPRESION ETIQUETA
                                    /*A VER SI SE QUITA O NOO*/
                                    List<ConsultaProductosZumiesResult> listaPO = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());

                                    EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                                    claseZ.id = clase.id_inventario;
                                    claseZ.assembly = clase.po.ToString();
                                    claseZ.poInCompleto = clase.poInCompleto;
                                    claseZ.poItem = "99";
                                    claseZ.ProductCode = "ASSORTMENT";
                                    claseZ.Size = "ASSORTMENT";
                                    claseZ.ESTILO = "ASSORTMENT";
                                    claseZ.QUANTITY = Convert.ToString(cantidadTotalCajasPrepack);

                                    /*SI NO SE PONE X*/
                                    claseZ.CARTON_NUMBER_INICIAL = (listaPO[0].NumeroCaja).ToString();
                                    claseZ.CARTON_NUMBER_FINAL = Convert.ToString(listaPO[0].totalCajasPO);
                                    /*EN VEZ DE LISTAPO */

                                    claseZ.COUNTRY = "MEXICO";
                                    claseZ.Cantidad = Convert.ToDecimal(cantidadTotalCajasPrepack);
                                    claseZ.DPCI = clase.DPCI;
                                    claseZ.itemDescription = claseZ.DPCI;
                                    claseZ.cn_tag_num = Convert.ToInt32(clase.id_inventario);
                                    claseZ.color = "";
                                    claseZ.size_izquierdo = "ASSORTMENT";
                                    string contarDigitos = "00" + clase.upc.Substring(0, 11) + "1";
                                    claseZ.Carton = clase.id_inventario;
                                    //en ASSORTMENT NO ES EL UPC ES UNO QUE ENGLOBA TODAS LAS TALLAS

                                    //List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                                    //BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                                    //Codigo.BarWidth = 3;

                                    //Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                                    //#region  RECORTANDO IMAGEN ESPERO NO VOLVER A USARLO
                                    //// RECORTANDO IMAGEN ESPERO NO VOLVER A USARLO
                                    ////Rectangle cropRec = new Rectangle(12, 0, 320, 200);
                                    ////Image Original = codigoBarras;
                                    ////Bitmap cropImage = new Bitmap(cropRec.Width, cropRec.Height);
                                    ////Graphics g = Graphics.FromImage(cropImage);
                                    ////g.DrawImage(Original, new Rectangle(0, 0, cropRec.Width, cropRec.Height), cropRec, GraphicsUnit.Pixel);
                                    ////Original.Dispose();
                                    //#endregion

                                    //claseZ.codigoBarras = codigoBarras;  //cropImage;
                                    //listClase.Add(claseZ);
                                    //id_inventarioAnt = Convert.ToInt32(clase.id_inventario);
                                    //ReporteCajaTargetAssorment report = new ReporteCajaTargetAssorment
                                    //{
                                    //    DataSource = listClase
                                    //};
                                    //// Disable margins warning. 
                                    //report.PrintingSystem.ShowMarginsWarning = false;
                                    //ReportPrintTool tool = new ReportPrintTool(report);

                                    //tool.Print(); //imprime de golpe


                                    //////********************************************************imprime carton interno**************************************************************************************************************************////

                                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                    List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                                    EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                                    clase2.id_inventario = Convert.ToInt32(claseZ.id);
                                    clase2.po = clase.po;
                                    clase2.poInCompleto = Convert.ToInt32(claseZ.CARTON_NUMBER_INICIAL);
                                    clase2.cliente = "";
                                    clase2.factura = "";
                                    clase2.terminado = "";
                                    clase2.usuario = usu[0].nombre;
                                    clase2.ProductCode = claseZ.ProductCode;
                                    clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                    clase2.Size = "ASSORTMENT";
                                    clase2.Fecha = clase.Fecha;
                                    clase2.assembly = "*" + claseZ.id.ToString() + "*";

                                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_inventario +
                                                                                        "&po=" + clase2.po +
                                                                                        "&cl=" + clase2.cliente +
                                                                                        "&fa=" + clase2.factura +
                                                                                        "&te=" + clase2.terminado +
                                                                                        "&u=" + clase2.usuario +
                                                                                        "&pc=" + clase2.ProductCode +
                                                                                        "&c=" + clase2.Cantidad +
                                                                                        "&sz=" + clase2.size_izquierdo +
                                                                                        "&fe=" + clase2.Fecha,
                                                                                        QRCodeGenerator.ECCLevel.Q);
                                    QRCode qrCode = new QRCode(qrCodeData);
                                    BarcodeLib.Barcode Codigo2 = new BarcodeLib.Barcode
                                    {
                                        IncludeLabel = true
                                    };
                                    Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_inventario.ToString(), Color.Black, Color.White, 250, 150);
                                    clase2.qr = qrCode.GetGraphic(20);
                                    clase2.codigoBarras = codigoBarras2;
                                    listClase2.Add(clase2);
                                    ReporteCaja report2 = new ReporteCaja
                                    {
                                        DataSource = listClase2
                                    };
                                    // Disable margins warning. 
                                    report2.PrintingSystem.ShowMarginsWarning = false;
                                    ReportPrintTool tool2 = new ReportPrintTool(report2);
                                    //tool.ShowPreview();
                                    //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                    //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                    tool2.Print(); //imprime de golpe



                                    //cantidad = cantidadTotalCajasPrepack.ToString();
                                    //txtUnitsRemai.Text = cantidadTotalCajasPrepack.ToString();
                                    //txtUnitsReq.Text = cantidadTotalCajasPrepack.ToString();
                                    //txtUnitsScan.Text = "0";
                                    //txtCartonRq.Text = "1";
                                    //txtCartonsReamaining.Text = "1";
                                    LimpiarPantallaEscaneo();
                                    contador = 0;
                                    txtUPCScann.Text = string.Empty;
                                    dgvEscan.Rows[0].Selected = true;
                                    dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                                    txtUnitsScan.Text = (0).ToString();
                                    txtUPCScann.Focus();
                                    iniciando = true;
                                    f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                                    cmbPOItem.SelectedIndex = 0;
                                    f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                                    cmbProductCode.SelectedIndex = 0;
                                    f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                                    cmbSizes.SelectedIndex = idIndex;
                                    iniciando = false;
                                    cmbSizes.SelectedIndex = idIndex;
                                    contador = 0;
                                    txtCartonRq.Text = claseZ.CARTON_NUMBER_FINAL;
                                    txtCartonsPacked.Text = claseZ.CARTON_NUMBER_INICIAL;


                                    #endregion

                                }
                                else
                                {
                                    MessageBox.Show("Ya se escanearon todas las cajas de la Talla Target Prepack");

                                    LimpiarPantallaEscaneo();

                                    txtCartonRq.Text = po[0].totalCajasPO.ToString();
                                    txtCartonsPacked.Text = po[0].totalCajasPO.ToString();


                                    txtUPCScann.Focus();
                                    iniciando = true;
                                    f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                                    cmbPOItem.SelectedIndex = 0;
                                    f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                                    cmbProductCode.SelectedIndex = 0;
                                    f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                                    cmbSizes.SelectedIndex = idIndex;
                                    iniciando = false;
                                    cmbSizes.SelectedIndex = idIndex;
                                    contador = 0;
                                }
                                /*************AQUIIIIII****************/


                            }


                        }

                        #endregion
                        break;
                    case "LEVIS":
                        #region LEVIS
                        List<ConsultaProductos_VarcharResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());
                        int prodNuevosCont = 0;
                        int NumeroCarton = 0;
                        int TotalPrendas = 0;

                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        foreach (ConsultaProductos_VarcharResult a in x)
                        {
                            prodNuevosCont = prodNuevosCont + 1;
                            contador = 0;
                            txtCartonNumber.Text = a.CartonNumber.ToString();
                            txtCartonSize.Text = "";
                            txtSize.Text = "";
                            txtProductCode.Text = a.ProductCode.ToString();

                            anterior.Add(id);
                            upc = cmbPOItem.Text;

                            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                            {
                                id = a.id,
                                po = cmbPO.Text,
                                poInCompleto = Convert.ToDecimal(cmbPO.Text),
                                poItem = "99",
                                ProductCode = a.ProductCode,
                                Size = a.Size
                            };
                            separadas = a.Size.Split('x');

                            if (a.id > 0)
                            {
                            }
                            else
                            {
                                a.id = 1;
                            }
                            clase.size_derecho = separadas[1].ToString();
                            clase.size_izquierdo = separadas[0].ToString();
                            clase.TipoCarton = "0";
                            clase.upc = a.UPC;
                            clase.Fecha = DateTime.Now;
                            clase.CartonLeft = "";
                            clase.CartonRight = "";
                            clase.Cantidad = Convert.ToDecimal(a.cantidad);

                            DateTime DateObject = Convert.ToDateTime(DateTime.Now.ToString());
                            string fecha = DateObject.Day.ToString() + DateObject.Month.ToString() + DateObject.Year.ToString();
                            NumeroCarton = Convert.ToInt32(fecha);//Convert.ToInt32(a.CartonNumber + '0' + Convert.ToInt32(a.id));
                            if (prodNuevosCont >= 1)
                            {
                                clase.TipoCarton = (0).ToString();
                            }
                            else
                            {
                                clase.TipoCarton = (1).ToString();
                            }
                            clase.Carton = NumeroCarton;
                            clase.usuario = usu[0].nombre;
                            clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                            clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                            clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                            clase.cliente = cmbCliente.Text;
                            clase.factura = cmbFactura.Text;
                            clase.terminado = cmbTerminado.Text;
                            clase.assembly = "";
                            clase.Vendor = "";
                            clase.ShipTo = "";
                            clase.po_varchar = "";

                            clase.id_inventario = f.GuardaInventario(clase, usu[0].id);
                            listClase.Add(clase);
                        }
                        id_inventarioAnt = listClase[0].id_inventario;
                        listClase[0].cn_tag_num = NumeroCarton;
                        listClase[0].Cantidad = TotalPrendas;
                        ReporteCaja report = new ReporteCaja
                        {
                            DataSource = listClase
                        };
                        // Disable margins warning. 
                        report.PrintingSystem.ShowMarginsWarning = false;
                        ReportPrintTool tool = new ReportPrintTool(report);
                        if (imprimir)
                        {
                            //tool.ShowPreview();
                            //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                            //tool.PrintDialog(); //muestra a que impresora se va a mandar
                            tool.Print(); //imprime de golpe
                        }
                        else
                        {

                        }
                        #endregion 
                        break;
                }
                #endregion
            }
            else
            {
                //1222

                #region ESCANEANDO X PRENDA AUN
                /*ESCANEANDO X PRENDA AUN*/
                contador = contador + 1;
                dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text) + 1)].Selected = true;
                dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text) + 1);
                txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";
                #endregion
            }


        
        }
    }
}
