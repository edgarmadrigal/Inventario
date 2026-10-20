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
    // Marca ZUMIES (POItem 77)
    public partial class Escaneo
    {
        private void IncompleteCarton_ZUMIES()
        {
            try
            {
                string[] separadas;
                separadas = cmbSizes.Text.Split('x');
                List<ConsultaProductosZumiesResult> x = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());
                if (x.Count > 0)
                {
                    EtiquetaZUMIES clase = new EtiquetaZUMIES();
                    clase.id = x[0].id;
                    clase.po = cmbPO.Text;
                    string poSoloNumeros = String.Empty;
                    foreach (var c in (cmbPO.Text.ToString()))
                    {
                        if (char.IsNumber(c))
                        {
                            poSoloNumeros += c;
                        }
                    }
                    clase.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                    clase.poItem = "77";
                    clase.ProductCode = x[0].estilo;
                    clase.Size = x[0].Talla;
                    clase.size_izquierdo = separadas[0].ToString();
                    txtCartonNumber.Text = (x[0].NumeroCaja + 1).ToString();
                    clase.cn_tag_num = Convert.ToInt32(x[0].NumeroCaja) + 1;
                    clase.CARTON_NUMBER_INICIAL = (x[0].NumeroCaja + 1).ToString();
                    clase.size_derecho = "";
                    clase.TipoCarton = "0";
                    clase.upc = x[0].upc;
                    clase.Fecha = DateTime.Now;
                    clase.CartonLeft = "";
                    clase.CartonRight = "";
                    clase.Cantidad = Convert.ToDecimal(txtUnitsScan.Text);
                    clase.Carton = Convert.ToInt32(x[0].NumeroCaja + 1);
                    clase.usuario = usu[0].nombre;
                    clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                    clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                    clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                    clase.cliente = cmbCliente.Text;
                    clase.factura = cmbFactura.Text;
                    clase.terminado = cmbTerminado.Text;
                    clase.assembly = x[0].color;
                    clase.Vendor = x[0].itemDescription;
                    clase.ESTILO = x[0].estilo;
                    clase.DESCRIPTION = x[0].itemDescription + " COLOR:" + x[0].color + " SIZE:" + x[0].Talla;
                    clase.QUANTITY = txtUnitsScan.Text;
                    clase.CARTON_NUMBER_FINAL = Convert.ToString(x[0].totalCajasPO);
                    clase.COUNTRY = "MEXICO";
                    clase.cn_tag_num = 0;

                    id_inventarioAnt = f.GuardaInventarioZumies(clase, usu[0].id);

                    contador = 0;
                    txtCartonSize.Text = "";
                    txtSize.Text = "";
                    txtProductCode.Text = x[0].estilo.ToString();

                    anterior.Add(id);
                    upc = cmbPOItem.Text;
                    PrendasExtra = 0;
                }
                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                /*TOTAL DE PRENDAS ESCANEADAS*/
                contador = 0;
                txtUPCScann.Text = string.Empty;
                txtUnitsReq.Text = txtUnitsReq.Text;
                txtUnitsRemai.Text = txtUnitsReq.Text;
                dgvEscan.Rows[0].Selected = true;
                dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                txtUPCScann.Focus();
                List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
                if (x1.Count > 0)
                {
                    List<ConsultaProductosZumiesResult> x2 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());

                    /***/
                    if (x2.Count > 0)
                    {
                        contador = 0;
                        dgvEscan.DataSource = x2;
                        txtCartonsPacked.Text = x2[0].NumeroCaja.ToString();
                        txtCartonsReamaining.Text = Convert.ToString(Convert.ToInt32(x2[0].NumeroCaja.Value.ToString()));
                        txtCartonNumber.Text = x2[0].NumeroCaja.ToString();
                        txtCartonSize.Text = x2[0].Talla.ToString();
                        dgvEscan.Columns["Cantidad"].Visible = false;
                        dgvEscan.Columns["escaneado"].Visible = false;
                        dgvEscan.Columns["NumeroCaja"].Visible = false;
                        dgvEscan.Columns["itemDescription"].Visible = false;
                        dgvEscan.Columns["cantidadCajas"].Visible = false;
                        dgvEscan.Columns["id"].Visible = false;
                        txtSize.Text = x2[0].Talla.ToString();
                        txtProductCode.Text = x2[0].estilo.ToString();
                        id = x2[0].id;
                        anterior.Add(id);
                        upc = x2[0].upc.ToString();
                        vendor = x2[0].itemDescription.ToString();
                        ///po = Convert.ToInt32(cmbPO.Text);
                        cantidad = x2[0].cantidad.ToString();
                        txtUnitsReq.Text = x2[0].cantidad.ToString();
                        txtUnitsRemai.Text = cantidad.ToString();
                        //txtUnitsScan.Text = "0";
                        txtUPCScann.Focus();
                    }
                    iniciando = true;
                    f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                    cmbPOItem.SelectedIndex = 0;
                    f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                    cmbProductCode.SelectedIndex = 0;
                    f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                    cmbSizes.SelectedIndex = idIndex;
                    iniciando = false;
                    cmbSizes.SelectedIndex = idIndex;
                    //cmbSizes.SelectedIndex = 0;


                    contador = 0;
                    EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                    claseZ.id = x1[0].id;
                    claseZ.po = x1[0].po;
                    claseZ.poInCompleto = Convert.ToDecimal(x1[0].poInCompleto);
                    claseZ.poItem = "77";
                    claseZ.ProductCode = x1[0].ProductCode;
                    claseZ.Size = x1[0].size_izquierdo;
                    claseZ.ESTILO = x1[0].ProductCode;
                    claseZ.DESCRIPTION = x1[0].TipoCarton;
                    claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                    claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                    claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                    claseZ.CARTON_NUMBER_FINAL = Convert.ToString(x[0].totalCajasPO); ;
                    claseZ.COUNTRY = "MEXICO";
                    claseZ.assembly = "*" + claseZ.CARTON_NUMBER_INICIAL.ToString() + "*";

                    List<EtiquetaZUMIES> listclaseZ = new List<EtiquetaZUMIES>();
                    listclaseZ.Add(claseZ);
                    id_inventarioAnt = claseZ.id_inventario;

                    ReporteCajaZumines report = new ReporteCajaZumines
                    {
                        DataSource = listclaseZ
                    };
                    report.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool = new ReportPrintTool(report);
                    tool.Print(); //imprime de golpe


                }
                LimpiarPantallaEscaneo();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        
        }

        private void SeleccionTalla_ZUMIES()
        {
            lblImprimirEtiqueta.Visible = false;
            chkImprimirEtiqueta.Visible = false;
            try
            {
                List<ConsultaProductosZumiesResult> x1 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, cmbSizes.Text);

                if (x1.Count > 0)
                {
                    contador = 0;
                    dgvEscan.DataSource = x1;
                    txtCartonNumber.Text = x1[0].NumeroCaja.ToString();
                    txtCartonSize.Text = x1[0].Talla.ToString();
                    dgvEscan.Columns["Cantidad"].Visible = false;
                    dgvEscan.Columns["escaneado"].Visible = false;
                    dgvEscan.Columns["NumeroCaja"].Visible = false;
                    dgvEscan.Columns["itemDescription"].Visible = false;
                    dgvEscan.Columns["cantidadCajas"].Visible = false;
                    dgvEscan.Columns["id"].Visible = false;
                    txtSize.Text = x1[0].Talla.ToString();
                    txtProductCode.Text = x1[0].estilo.ToString();
                    id = x1[0].id;
                    anterior.Add(id);
                    upc = x1[0].upc.ToString();
                    vendor = x1[0].itemDescription.ToString();
                    txtCartonsPacked.Text = x1[0].cajasEscaneadasporTalla.ToString();
                    txtCartonRq.Text = x1[0].cantidadCajas.ToString();
                    txtUnitsReq.Text = x1[0].cantidad;
                    txtUnitsScan.Text = "0";
                    txtUnitsRemai.Text = x1[0].cantidad;
                    txtUPCScann.Focus();

                }
                else
                {
                    //MessageBox.Show("Ya se escanearon todas las cajas de la Talla Zumies cmbSizes");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        
        }

        private void Escaneo_ZUMIES_TARGET(ref List<ConsultaUPC_VarcharResult> ListUpc, string po_numero)
        {
            #region ZUMIES
            /*UPC CORRECTO*/
            #region TOTAL DE PRENDAS ESCANEADAS
            if (PrendasExtra == 0 && Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) - 1 && cmbPOItem.Text != "88")
            {
                try
                {
                    /*TOTAL DE PRENDAS ESCANEADAS*/
                    ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                    id = ListUpc[0].id;
                    upc = ListUpc[0].upc;
                    string[] separadas;
                    separadas = cmbSizes.Text.Split('x');

                    List<ConsultaProductosZumiesResult> po = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

                    if (po.Count > 0)
                    {
                        contador = 0;
                        txtCartonSize.Text = "";
                        txtSize.Text = "";
                        txtProductCode.Text = po[0].estilo.ToString();
                        anterior.Add(id);
                        upc = cmbPOItem.Text;
                        EtiquetaZUMIES clase = new EtiquetaZUMIES();
                        clase.id = po[0].id;
                        clase.ProductCode = po[0].estilo;
                        clase.Size = po[0].Talla;
                        clase.cn_tag_num = Convert.ToInt32(po[0].NumeroCaja);
                        clase.CARTON_NUMBER_INICIAL = (po[0].NumeroCaja).ToString();
                        clase.Cantidad = Convert.ToDecimal(po[0].cantidad);
                        clase.Carton = po[0].NumeroCaja + 1;
                        clase.upc = po[0].upc;
                        clase.assembly = po[0].color;
                        clase.QUANTITY = Convert.ToString(po[0].cantidad);
                        clase.CARTON_NUMBER_FINAL = Convert.ToString(po[0].totalCajasPO);
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
                            clase.poItem = "88";
                            clase.size_izquierdo = ListUpc[0].Size.ToString();
                            clase.Vendor = po[0].itemDescription;
                            clase.ESTILO = po[0].estilo;
                            clase.DESCRIPTION = po[0].itemDescription;
                        }


                        clase.size_derecho = "";
                        clase.TipoCarton = "0";
                        clase.po = cmbPO.Text;
                        clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
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
                        clase.COUNTRY = "MEXICO";

                        clase.id_inventario = f.GuardaInventarioZumies(clase, usu[0].id);
                        id_inventarioAnt = clase.id_inventario;
                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                        try
                        {
                            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
                            if (x1.Count > 0)
                            {
                                List<ConsultaProductosZumiesResult> x2 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

                                if (x2.Count > 0)
                                {
                                    contador = 0;
                                    dgvEscan.DataSource = x2;
                                    txtCartonsPacked.Text = x2[0].NumeroCaja.ToString();
                                    txtCartonsReamaining.Text = Convert.ToString(Convert.ToInt32(x2[0].NumeroCaja.Value.ToString()));
                                    txtCartonNumber.Text = x2[0].NumeroCaja.ToString();
                                    txtCartonRq.Text = x2[0].cantidadCajas.ToString();
                                    txtCartonSize.Text = x2[0].Talla.ToString();
                                    dgvEscan.Columns["Cantidad"].Visible = false;
                                    dgvEscan.Columns["cantidadCajas"].Visible = false;
                                    dgvEscan.Columns["escaneado"].Visible = false;
                                    dgvEscan.Columns["NumeroCaja"].Visible = false;
                                    dgvEscan.Columns["itemDescription"].Visible = false;
                                    dgvEscan.Columns["id"].Visible = false;
                                    txtSize.Text = x2[0].Talla.ToString();
                                    txtProductCode.Text = x2[0].estilo.ToString();
                                    id = x2[0].id;
                                    anterior.Add(id);
                                    upc = x2[0].upc.ToString();
                                    vendor = x2[0].itemDescription.ToString();

                                    txtCartonsPacked.Text = x2[0].cajasEscaneadasporTalla.ToString();
                                    txtCartonRq.Text = x2[0].cantidadCajas.ToString();
                                    // txtUnitsScan.Text = "0";
                                    txtUnitsRemai.Text = x2[0].cantidad;
                                    txtUnitsReq.Text = x2[0].cantidad;
                                    txtUPCScann.Focus();
                                }
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
                                EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                                claseZ.id = x1[0].id;
                                claseZ.po = x1[0].po;
                                claseZ.poInCompleto = Convert.ToDecimal(x1[0].poInCompleto);
                                claseZ.ProductCode = x1[0].ProductCode;
                                claseZ.Size = x1[0].size_izquierdo;
                                claseZ.upc = x1[0].upc;
                                claseZ.ESTILO = x1[0].ProductCode;
                                claseZ.DESCRIPTION = x1[0].TipoCarton;
                                claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                                claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                                claseZ.CARTON_NUMBER_FINAL = clase.CARTON_NUMBER_FINAL;
                                claseZ.COUNTRY = "MEXICO";
                                claseZ.assembly = "*" + claseZ.CARTON_NUMBER_INICIAL.ToString() + "*";
                                List<EtiquetaZUMIES> listclaseZ = new List<EtiquetaZUMIES>();
                                if (cmbPOItem.Text == "77")
                                {
                                    claseZ.poItem = "77";
                                    listclaseZ.Add(claseZ);
                                    id_inventarioAnt = claseZ.id_inventario;

                                    ReporteCajaZumines report = new ReporteCajaZumines
                                    {
                                        DataSource = listclaseZ
                                    };
                                    report.PrintingSystem.ShowMarginsWarning = false;
                                    ReportPrintTool tool = new ReportPrintTool(report);
                                    tool.Print(); //imprime de golpe
                                }

                                else if (cmbPOItem.Text == "88")
                                {
                                    claseZ.assembly = x1[0].po.ToString();
                                    claseZ.Cantidad = x1[0].Cantidad;
                                    claseZ.size_izquierdo = x1[0].size_izquierdo;
                                    claseZ.itemDescription = x1[0].TipoCarton;

                                    claseZ.poItem = "88";
                                    BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                                    Codigo.BarWidth = 3;

                                    string contarDigitos = "00" + claseZ.upc.Substring(0, 11) + "1";
                                    Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                                    claseZ.codigoBarras = codigoBarras;  //cropImage;
                                    listclaseZ.Add(claseZ);
                                    id_inventarioAnt = Convert.ToInt32(claseZ.id);
                                    ReportCajaTarget report = new ReportCajaTarget
                                    {
                                        DataSource = listclaseZ
                                    };
                                    // Disable margins warning. 
                                    report.PrintingSystem.ShowMarginsWarning = false;
                                    ReportPrintTool tool = new ReportPrintTool(report);
                                    ////tool.ShowPreview();
                                    ////tool.ShowRibbonPreviewDialog(); // muestra el diseÑo 
                                    tool.Print(); //imprime de golpe


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
                                    clase2.Size = claseZ.size_izquierdo.Trim();
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

                                }
                            }
                            LimpiarPantallaEscaneo();
                            contador = 0;
                            txtUPCScann.Text = string.Empty;
                            dgvEscan.Rows[0].Selected = true;
                            dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                            txtUnitsScan.Text = (0).ToString();
                            txtUPCScann.Focus();

                            txtCartonRq.Text = x1[0].TotalCajas.ToString();
                            txtCartonsPacked.Text = x1[0].Carton.ToString();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ya se escanearon todas las cajas de la Talla Zumies txtUPCScann");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            #endregion TOTAL DE PRENDAS ESCANEADAS
            else
            {
                #region ESCANEANDO X PRENDA AUN
                /*aqui reviso el total de cajas contra las requeridas*/
                /*ESCANEANDO X PRENDA AUN*/
                if (PrendasExtra == 1)
                {
                    contador = contador + 1;
                    txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                    txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";

                }
                else
                {
                    contador = contador + 1;
                    dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text) + 1)].Selected = true;
                    dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text) + 1);
                    txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                    txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";

                }
                #endregion
            }
            #endregion
        
        }

        private void ReimprimirUltima_ZUMIES()
        {
            if (id_inventarioAnt != 0 && Convert.ToInt32(txtCartonsPacked.Text) > 0)
            {
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                List<ConsultaInventarioIDResult> x = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
                if (x.Count > 0)
                {
                    string poSoloNumeros = String.Empty;
                    foreach (var c in (x[0].po.ToString()))
                    {
                        if (char.IsNumber(c))
                        {
                            poSoloNumeros += c;
                        }
                    }
                    EtiquetaZUMIES clase = new EtiquetaZUMIES
                    {
                        id = x[0].id,
                        po = x[0].po,
                        poInCompleto = Convert.ToDecimal(poSoloNumeros),
                        poItem = "77",
                        ProductCode = x[0].ProductCode,
                        Size = x[0].size_izquierdo
                    };

                    clase.ESTILO = x[0].ProductCode;
                    clase.DESCRIPTION = x[0].TipoCarton;
                    clase.QUANTITY = Convert.ToString(x[0].Cantidad);
                    clase.cn_tag_num = Convert.ToInt32(x[0].Carton);
                    // clase.CARTON_NUMBER_INICIAL = x[0].size_derecho.ToString();
                    clase.CARTON_NUMBER_INICIAL = x[0].Carton.ToString();
                    clase.CARTON_NUMBER_FINAL = Convert.ToString(x[0].TotalCajas); ;
                    clase.COUNTRY = "MEXICO";

                    List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                    listClase.Add(clase);
                    ReporteCajaZumines report = new ReporteCajaZumines
                    {
                        DataSource = listClase
                    };
                    report.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool = new ReportPrintTool(report);
                    tool.Print();
                }
                else
                {

                }
            }
            else
            {
                MessageBox.Show("Favor de Escanear");
            }
        
        }
    }
}
