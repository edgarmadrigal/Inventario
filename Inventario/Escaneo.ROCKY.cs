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
    // Marca ROCKY (POItem 66)
    public partial class Escaneo
    {
        private void IncompleteCarton_ROCKY()
        {
            /*TOTAL DE PRENDAS ESCANEADAS*/
            string[] separadas;

            //separadas = cmbSizes.Text.Split('x');

            string input = cmbSizes.Text;
            if (input.Contains("x"))
            {
                separadas = cmbSizes.Text.Split('x');
            }
            else
            {
                separadas = new string[1];
                separadas[0] = cmbSizes.Text; input = cmbSizes.Text;
            }
            List<ConsultaProductosZumiesResult> x = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, cmbSizes.Text);

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
                clase.po = cmbPO.Text;
                string poSoloNumeros = string.Empty;
                foreach (var c in cmbPO.Text.ToString())
                {
                    if (char.IsNumber(c))
                    {
                        poSoloNumeros += c;
                    }
                }
                clase.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                clase.poItem = "66";
                clase.ProductCode = x[0].estilo;
                clase.Size = x[0].Talla;
                clase.size_izquierdo = separadas[0].ToString().Trim();
                try { clase.size_derecho = separadas[1].ToString().Trim(); } catch { clase.size_derecho = clase.size_derecho = ""; }
                //clase.size_derecho = separadas[1].ToString().Trim();
                clase.cn_tag_num = Convert.ToInt32(x[0].NumeroCaja);
                clase.CARTON_NUMBER_INICIAL = (x[0].NumeroCaja).ToString();
                clase.TipoCarton = x[0].color;
                clase.upc = x[0].upc;
                clase.Fecha = DateTime.Now;
                clase.CartonLeft = "";
                clase.CartonRight = "";
                clase.Cantidad = Convert.ToDecimal(txtUnitsScan.Text);
                clase.Carton = x[0].NumeroCaja + 1;
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
                clase.DESCRIPTION = x[0].color;
                clase.QUANTITY = txtUnitsScan.Text;
                clase.CARTON_NUMBER_FINAL = Convert.ToString(x[0].totalCajasPO);
                clase.COUNTRY = "MEXICO";
                clase.id_inventario = f.GuardaInventarioZumies(clase, usu[0].id);
                txtCartonNumber.Text = (x[0].NumeroCaja).ToString();
                id_inventarioAnt = clase.id_inventario;
                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
                if (x1.Count > 0)
                {
                    contador = 0;
                    EtiquetaROCKY claseZ = new EtiquetaROCKY();
                    claseZ.Fecha = DateTime.Now;
                    claseZ.id = x1[0].id;
                    claseZ.po = x1[0].po;
                    poSoloNumeros = string.Empty;
                    foreach (var c in claseZ.po.ToString())
                    {
                        if (char.IsNumber(c))
                        {
                            poSoloNumeros += c;
                        }
                    }
                    claseZ.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                    claseZ.poItem = "66";
                    claseZ.ProductCode = x1[0].ProductCode;
                    claseZ.Size = x1[0].Size.Trim();
                    claseZ.color = x1[0].TipoCarton;
                    claseZ.ESTILO = x1[0].ProductCode;
                    claseZ.DESCRIPTION = x1[0].TipoCarton;
                    claseZ.QUANTITY = txtUnitsScan.Text;
                    claseZ.Cantidad = Convert.ToDecimal(txtUnitsScan.Text);
                    claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                    claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                    claseZ.CARTON_NUMBER_FINAL = x1[0].TotalCajas.ToString();
                    claseZ.COUNTRY = "MEXICO";

                    string codigoBarras = "*" + x1[0].Carton.ToString() + "*";

                    claseZ.codigoBarras = codigoBarras;

                    List<EtiquetaROCKY> listclaseZ = new List<EtiquetaROCKY>();
                    listclaseZ.Add(claseZ);
                    id_inventarioAnt = claseZ.id_inventario;

                    ReporteRocky report = new ReporteRocky
                    {
                        DataSource = listclaseZ
                    };
                    report.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool = new ReportPrintTool(report);
                    tool.Print(); //imprime de golpe
                    tool.Print(); //imprime de golpe


                    LimpiarPantallaEscaneo();
                    //ñ
                    contador = 0;
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();

                }

            }

        
        }

        private void SeleccionTalla_ROCKY()
        {
            lblImprimirEtiqueta.Visible = false;
            chkImprimirEtiqueta.Visible = false;
            try
            {
                List<ConsultaProductosZumiesResult> x1 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, cmbSizes.Text.ToString());

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
                //List<ConsultaProductosZumiesResult> x1 =
                //f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, cmbSizes.Text.ToString());

                /*if (x1.Count > 0)
                {*/


                /* }*/
                else
                {
                    MessageBox.Show("Ya se escanearon todas las cajas de la Talla Rocky");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        
        }

        private void Escaneo_ROCKY(ref List<ConsultaUPC_VarcharResult> ListUpc, string po_numero)
        {
            #region ROCKY
            /*UPC CORRECTO 015*/
            #region TOTAL DE PRENDAS ESCANEADAS
            if (PrendasExtra == 0 && Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) - 1)
            {
                try
                {
                    /*TOTAL DE PRENDAS ESCANEADAS*/
                    string[] separadas;
                    ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                    id = ListUpc[0].id;
                    upc = ListUpc[0].upc;
                    string input = cmbSizes.Text;
                    if (input.Contains("x"))
                    {
                        separadas = cmbSizes.Text.Split('x');
                    }
                    else
                    {
                        separadas = new string[1];
                        separadas[0] = cmbSizes.Text; input = cmbSizes.Text;
                    }
                    List<ConsultaProductosZumiesResult> x = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

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
                        clase.po = cmbPO.Text;
                        string poSoloNumeros = "";
                        foreach (var c in cmbPO.Text.ToString())
                        {
                            if (char.IsNumber(c))
                            {
                                poSoloNumeros += c;
                            }
                        }
                        clase.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                        clase.poItem = "66";
                        clase.ProductCode = x[0].estilo;
                        clase.Size = cmbSizes.Text.Trim();
                        ///	MAJ9878009	130259	1	IND	58	0	L	192584681840

                        clase.Size = x[0].Talla;
                        clase.size_izquierdo = separadas[0].ToString().Trim();
                        try { clase.size_derecho = separadas[1].ToString().Trim(); } catch { clase.size_derecho = clase.size_derecho = ""; }
                        ;///	MAJ9878009	130259	1	IND	58	0	L	192584681840
                        clase.cn_tag_num = Convert.ToInt32(x[0].NumeroCaja);
                        clase.CARTON_NUMBER_INICIAL = (x[0].NumeroCaja).ToString();
                        clase.TipoCarton = x[0].color;
                        clase.upc = x[0].upc;
                        clase.Fecha = DateTime.Now;
                        clase.CartonLeft = "";
                        clase.CartonRight = "";
                        clase.Cantidad = Convert.ToDecimal(x[0].cantidad);
                        clase.Carton = x[0].NumeroCaja + 1;
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
                        clase.DESCRIPTION = x[0].color;
                        clase.QUANTITY = Convert.ToString(x[0].cantidad);
                        clase.CARTON_NUMBER_FINAL = Convert.ToString(x[0].totalCajasPO);
                        clase.COUNTRY = "MEXICO";

                        clase.id_inventario = f.GuardaInventarioZumies(clase, usu[0].id);

                        txtCartonNumber.Text = (x[0].NumeroCaja).ToString();
                        id_inventarioAnt = clase.id_inventario;
                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                        try
                        {
                            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
                            if (x1.Count > 0)
                            {
                                #region CODIGO DEMAS+
                                List<ConsultaProductosZumiesResult> x2 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString().Trim());
                                if (x2.Count > 0)
                                {
                                    contador = 0;
                                    dgvEscan.DataSource = x2;
                                    txtCartonsPacked.Text = x2[0].NumeroCaja.ToString();
                                    txtCartonsReamaining.Text = Convert.ToString(Convert.ToInt32(x2[0].NumeroCaja.Value.ToString()));
                                    txtCartonNumber.Text = x2[0].NumeroCaja.ToString();
                                    txtCartonRq.Text = x2[0].cantidadCajas.ToString();
                                    txtCartonSize.Text = x2[0].Talla.ToString().Trim();
                                    dgvEscan.Columns["Cantidad"].Visible = false;
                                    dgvEscan.Columns["cantidadCajas"].Visible = false;
                                    dgvEscan.Columns["escaneado"].Visible = false;
                                    dgvEscan.Columns["NumeroCaja"].Visible = false;
                                    dgvEscan.Columns["itemDescription"].Visible = false;
                                    dgvEscan.Columns["cantidadCajas"].Visible = false;
                                    dgvEscan.Columns["id"].Visible = false;
                                    txtSize.Text = x2[0].Talla.ToString().Trim();
                                    txtProductCode.Text = x2[0].estilo.ToString();
                                    id = x2[0].id;
                                    anterior.Add(id);
                                    upc = x2[0].upc.ToString();
                                    vendor = x2[0].itemDescription.ToString();
                                    cantidad = x2[0].cantidad.ToString();
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

                                #endregion
                                contador = 0;
                                EtiquetaROCKY claseZ = new EtiquetaROCKY();
                                claseZ.Fecha = DateTime.Now;
                                claseZ.id = x1[0].id;
                                claseZ.po = x1[0].po;
                                poSoloNumeros = string.Empty;
                                foreach (var c in claseZ.po.ToString())
                                {
                                    if (char.IsNumber(c))
                                    {
                                        poSoloNumeros += c;
                                    }
                                }
                                claseZ.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                                claseZ.poItem = "66";
                                claseZ.ProductCode = x1[0].ProductCode;
                                claseZ.Size = cmbSizes.Text.Trim();
                                claseZ.color = x1[0].TipoCarton;
                                claseZ.ESTILO = x1[0].ProductCode;
                                claseZ.DESCRIPTION = x1[0].TipoCarton;
                                claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                                claseZ.Cantidad = x1[0].Cantidad;
                                claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                                claseZ.CARTON_NUMBER_FINAL = x1[0].TotalCajas.ToString();
                                claseZ.COUNTRY = "MEXICO";
                                claseZ.codigoBarras = "*" + x1[0].Carton.ToString() + "*";

                                List<EtiquetaROCKY> listclaseZ = new List<EtiquetaROCKY>();
                                listclaseZ.Add(claseZ);
                                id_inventarioAnt = claseZ.id_inventario;

                                ReporteRocky report = new ReporteRocky
                                {
                                    DataSource = listclaseZ
                                };
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);

                                tool.Print(); //imprime de golpe


                                EtiquetaROCKY claseR = new EtiquetaROCKY();
                                claseR.Fecha = DateTime.Now;
                                claseR.id = x1[0].id;
                                claseR.po = x1[0].po;
                                poSoloNumeros = string.Empty;
                                foreach (var c in claseR.po.ToString())
                                {
                                    if (char.IsNumber(c))
                                    {
                                        poSoloNumeros += c;
                                    }
                                }
                                claseR.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                                claseR.poItem = "66";
                                claseR.ProductCode = x1[0].ProductCode;
                                claseR.Size = cmbSizes.Text.Trim();
                                claseR.color = x1[0].TipoCarton;
                                claseR.ESTILO = x1[0].ProductCode;
                                claseR.DESCRIPTION = x1[0].TipoCarton;
                                claseR.QUANTITY = Convert.ToString(x1[0].Cantidad);
                                claseR.Cantidad = x1[0].Cantidad;
                                claseR.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                claseR.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                                claseR.CARTON_NUMBER_FINAL = x1[0].TotalCajas.ToString();
                                claseR.COUNTRY = "MEXICO";
                                claseR.codigoBarras = "*" + x1[0].Carton.ToString() + "*";
                                List<EtiquetaROCKY> listclaseR = new List<EtiquetaROCKY>();
                                listclaseR.Add(claseR);
                                id_inventarioAnt = claseR.id_inventario;
                                ReporteRocky report2 = new ReporteRocky
                                {
                                    DataSource = listclaseR
                                };

                                report2.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool2 = new ReportPrintTool(report2);
                                tool2.Print(); //imprime de golpe

                            }


                            LimpiarPantallaEscaneo();

                            contador = 0;
                            txtUPCScann.Text = string.Empty;
                            txtUnitsReq.Text = txtUnitsReq.Text;
                            txtUnitsScan.Text = "0";
                            txtUnitsRemai.Text = txtUnitsReq.Text;
                            dgvEscan.Rows[0].Selected = true;
                            dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                            txtUnitsScan.Text = (0).ToString();
                            txtUPCScann.Focus();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                    else
                    {
                        // MessageBox.Show("Ya se escanearon todas las cajas de la Talla");
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
    }
}
