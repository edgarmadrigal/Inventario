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
    // Marca TARGET (POItem 88)
    public partial class Escaneo
    {
        private void IncompleteCarton_TARGET()
        {
            /*------------------------------TARGETS---NUEVO*/
            #region TOTAL DE PRENDAS ESCANEADAS
            try
            {
                /*TOTAL DE PRENDAS ESCANEADAS*/
                string[] separadas;
                separadas = cmbSizes.Text.Split('x');
                List<ConsultaProductosTargetResult> x = f.ConsultaProductosTarget(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());
                if (x.Count > 0)
                {
                    contador = 0;
                    txtCartonSize.Text = "";
                    txtSize.Text = "";
                    txtProductCode.Text = x[0].estilo.ToString();
                    anterior.Add(id);
                    upc = cmbPOItem.Text;

                    EtiquetaZUMIES clase = new EtiquetaZUMIES();
                    clase.id = x[0].id;
                    clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                    clase.poItem = "88";
                    clase.ProductCode = x[0].po_numero;
                    clase.Size = x[0].Talla;
                    clase.size_izquierdo = separadas[0].ToString();
                    clase.size_derecho = "";
                    clase.TipoCarton = "0";
                    clase.upc = x[0].upc;
                    clase.Fecha = DateTime.Now;
                    clase.CartonLeft = "";
                    clase.CartonRight = "";
                    clase.Cantidad = Convert.ToDecimal(txtUnitsScan.Text);
                    clase.Carton = 0;
                    clase.usuario = usu[0].nombre;
                    clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                    clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                    clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                    clase.cliente = cmbCliente.Text;
                    clase.factura = cmbFactura.Text;
                    clase.terminado = cmbTerminado.Text;
                    clase.Vendor = x[0].itemDescription;
                    clase.ESTILO = x[0].estilo;
                    clase.DESCRIPTION = x[0].itemDescription;
                    clase.QUANTITY = txtUnitsScan.Text;
                    clase.COUNTRY = "MEXICO";
                    clase.po = cmbPO.Text;
                    clase.id_inventario = f.GuardaInventarioZumies(clase, usu[0].id);

                    id_inventarioAnt = clase.id_inventario;
                    int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                    try
                    {
                        List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
                        if (x1.Count > 0)
                        {
                            List<ConsultaProductosTargetResult> x2 = f.ConsultaProductosTarget(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());
                            if (x2.Count > 0)
                            {
                                contador = 0;
                                dgvEscan.DataSource = x2;
                                txtCartonsPacked.Text = x2[0].NumeroCaja.ToString();
                                txtCartonsReamaining.Text = Convert.ToString(Convert.ToInt32(x2[0].NumeroCaja.Value.ToString()));
                                txtCartonNumber.Text = x2[0].NumeroCaja.ToString();
                                //txtCartonRq.Text = x2[0].cantidadCajas.ToString();
                                txtCartonSize.Text = x2[0].Talla.ToString();
                                dgvEscan.Columns["Cantidad"].Visible = false;
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
                                ///po = Convert.ToInt32(cmbPO.Text);
                                cantidad = txtUnitsScan.Text;
                                txtUnitsReq.Text = x2[0].cantidad.ToString();
                                txtUnitsRemai.Text = cantidad.ToString();
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

                            cmbSizes.SelectedValue = separadas[0].ToString();
                            EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                            claseZ.id = x1[0].id;
                            claseZ.assembly = x1[0].Assembly;
                            claseZ.poInCompleto = Convert.ToDecimal(x1[0].poInCompleto);
                            claseZ.poItem = "88";
                            claseZ.ProductCode = x1[0].ProductCode;
                            claseZ.Size = x1[0].size_izquierdo;
                            claseZ.ESTILO = x1[0].ProductCode;
                            claseZ.QUANTITY = txtUnitsScan.Text;
                            claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                            claseZ.CARTON_NUMBER_INICIAL = id_inventarioAnt.ToString();
                            claseZ.CARTON_NUMBER_FINAL = "___";
                            claseZ.COUNTRY = "MEXICO";
                            claseZ.Carton = id_inventarioAnt;
                            claseZ.Cantidad = Convert.ToDecimal(txtUnitsScan.Text);
                            claseZ.DPCI = x1[0].TipoCarton.Trim();
                            claseZ.itemDescription = claseZ.DPCI.Trim();
                            claseZ.color = "";
                            claseZ.size_izquierdo = separadas[0].ToString();
                            claseZ.assembly = x1[0].ProductCode;
                            string contarDigitos = "00" + clase.upc.Substring(0, 11) + "1";

                            List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                            Codigo.BarWidth = 3;

                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                            #region  RECORTANDO IMAGEN ESPERO NO VOLVER A USARLO
                            // RECORTANDO IMAGEN ESPERO NO VOLVER A USARLO
                            //Rectangle cropRec = new Rectangle(12, 0, 320, 200);
                            //Image Original = codigoBarras;
                            //Bitmap cropImage = new Bitmap(cropRec.Width, cropRec.Height);
                            //Graphics g = Graphics.FromImage(cropImage);
                            //g.DrawImage(Original, new Rectangle(0, 0, cropRec.Width, cropRec.Height), cropRec, GraphicsUnit.Pixel);
                            //Original.Dispose();
                            #endregion

                            claseZ.codigoBarras = codigoBarras;  //cropImage;
                            listClase.Add(claseZ);
                            id_inventarioAnt = Convert.ToInt32(clase.id);
                            ReportCajaTarget report = new ReportCajaTarget
                            {
                                DataSource = listClase
                            };
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);

                            tool.Print(); //imprime de golpe



                            //////********************************************************imprime carton interno**************************************************************************************************************************////

                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                            EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                            clase2.id_inventario = Convert.ToInt32(clase.id);
                            clase2.po = x1[0].po;
                            clase2.poInCompleto = Convert.ToDecimal(claseZ.po);
                            clase2.cliente = "";
                            clase2.factura = "";
                            clase2.terminado = "";
                            clase2.usuario = usu[0].nombre;
                            clase2.ProductCode = claseZ.ProductCode;
                            clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                            clase2.Size = claseZ.size_izquierdo;
                            clase2.Fecha = x1[0].create_dtm;
                            clase2.assembly = "*" + clase.id.ToString() + "*";
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


                            txtUnitsScan.Text = "0";
                        }

                        contador = 0;
                        txtUPCScann.Text = string.Empty;
                        txtUnitsReq.Text = txtUnitsReq.Text;
                        txtUnitsScan.Text = "0";
                        txtUnitsRemai.Text = txtUnitsReq.Text;
                        dgvEscan.Rows[0].Selected = true;
                        dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                        txtUPCScann.Focus();
                        LimpiarPantallaEscaneo();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Ya se escanearon todas las cajas de la Talla Target");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            #endregion TOTAL DE PRENDAS ESCANEADAS
        
        }

        private void SeleccionTalla_TARGET(ref string[] separadas)
        {
            lblImprimirEtiqueta.Visible = false;
            chkImprimirEtiqueta.Visible = false;
            /*CONSULTA SI ES PREPACK(ASSORTMENT) O NO*/
            int? esprepack = f.ConsultaPrepack(cmbPO.Text);
            if (esprepack > 0)
            {
                List<ConsultaPrepackDetalleResult> ListprepackTarget = f.ConsultaPrepackDetalle(Convert.ToInt32(esprepack));
                if (ListprepackTarget.Count > 0)
                {
                    dgvEscan.DataSource = ListprepackTarget;

                    dgvEscan.Columns["idSize"].Visible = false;
                    dgvEscan.Columns["idUsuario"].Visible = false;
                    dgvEscan.Columns["fecha"].Visible = false;
                    dgvEscan.Columns["idPrepack"].Visible = false;
                    dgvEscan.Columns["id"].Visible = false;

                    decimal? cantidadTotalCajasPrepack = 0;
                    foreach (ConsultaPrepackDetalleResult objPrepack in ListprepackTarget)
                    {
                        cantidadTotalCajasPrepack = objPrepack.cantidad + cantidadTotalCajasPrepack;
                    }

                    cantidad = cantidadTotalCajasPrepack.ToString();
                    txtUnitsRemai.Text = cantidadTotalCajasPrepack.ToString();
                    txtUnitsReq.Text = cantidadTotalCajasPrepack.ToString();
                    txtUnitsScan.Text = "0";
                    txtCartonRq.Text = "1";
                    txtCartonsReamaining.Text = "1";
                    txtUPCScann.Focus();
                }

            }
            else
            {
                separadas = cmbSizes.Text.Split('x');
                List<ConsultaProductosZumiesResult> x1 =
                    f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());
                if (x1.Count > 0)
                {
                    contador = 0;
                    dgvEscan.DataSource = x1;
                    txtCartonsPacked.Text = x1[0].NumeroCaja.ToString();
                    txtCartonNumber.Text = x1[0].NumeroCaja.ToString();
                    txtCartonSize.Text = x1[0].Talla.ToString();
                    txtSize.Text = x1[0].Talla.ToString();
                    txtProductCode.Text = x1[0].itemDescription.ToString();
                    id = x1[0].id;
                    anterior.Add(id);
                    upc = x1[0].upc.ToString();
                    vendor = "";
                    cantidad = x1[0].cantidad;
                    txtUnitsReq.Text = x1[0].cantidad.ToString();
                    txtUnitsScan.Text = "0";
                    txtUnitsRemai.Text = cantidad.ToString();
                    txtCartonRq.Text = x1[0].totalCajasPO.ToString();
                    txtCartonsPacked.Text = x1[0].NumeroCaja.ToString();
                    txtUPCScann.Focus();
                }
                else
                {
                    MessageBox.Show("Ya se escanearon todas las cajas de la Talla prepack Target");
                }
            }

        
        }

        private void ReimprimirUltima_TARGET()
        {
            txtUPCScann.Text = string.Empty;
            txtUPCScann.Focus();
            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
            if (x1.Count > 0)
            {

                contador = 0;
                EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                claseZ.poInCompleto = Convert.ToDecimal(x1[0].poInCompleto);
                claseZ.poItem = "88";
                claseZ.Size = x1[0].size_izquierdo;
                claseZ.Carton = id_inventarioAnt;
                claseZ.id = Convert.ToInt32(id_inventarioAnt);
                claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                claseZ.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                claseZ.DPCI = x1[0].TipoCarton;
                claseZ.itemDescription = claseZ.DPCI;
                claseZ.color = "";
                claseZ.size_izquierdo = x1[0].size_izquierdo;
                claseZ.assembly = x1[0].ProductCode;
                string contarDigitos = "00" + x1[0].upc.Substring(0, 11) + "1";

                List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                Codigo.BarWidth = 3;

                Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                claseZ.codigoBarras = codigoBarras;//cropImage;
                listClase.Add(claseZ);
                //id_inventarioAnt = Convert.ToInt32(idInv);
                ReportCajaTarget report = new ReportCajaTarget
                {
                    DataSource = listClase
                };
                // Disable margins warning. 
                report.PrintingSystem.ShowMarginsWarning = false;
                ReportPrintTool tool = new ReportPrintTool(report);

                tool.Print(); //imprime de golpe
                              //////********************************************************imprime carton interno**************************************************************************************************************************////

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                clase2.id_inventario = Convert.ToInt32(claseZ.id);
                clase2.po = x1[0].po;
                clase2.poInCompleto = Convert.ToDecimal(x1[0].po);
                clase2.cliente = "";
                clase2.factura = "";
                clase2.terminado = "";
                clase2.usuario = usu[0].nombre;
                clase2.ProductCode = claseZ.ProductCode;
                clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                clase2.Size = claseZ.size_izquierdo;
                clase2.Fecha = x1[0].create_dtm;
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
    }
}
