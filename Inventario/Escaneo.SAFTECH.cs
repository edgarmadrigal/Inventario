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
    // Marca SAFTECH (POItem 33)
    public partial class Escaneo
    {
        private void IncompleteCarton_SAFTECH()
        {
            try
            {
                /*TOTAL DE PRENDAS ESCANEADAS*/
                string[] separadas;
                // ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                //// id = ListUpc[0].id;
                // upc = ListUpc[0].upc;
                separadas = cmbSizes.Text.Split('x');

                /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                List<ConsultaProductosBensResult> x = f.ConsultaProductosBens(cmbPO.Text, separadas[0].ToString().Trim(), separadas[1].ToString().Trim());
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
                    clase.poItem = "33";
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
                    clase.QUANTITY = Convert.ToString(txtUnitsScan.Text);
                    clase.Cantidad = Convert.ToDecimal(txtUnitsScan.Text);
                    clase.itemDescription = x[0].itemDescription;
                    clase.color = x[0].Color;
                    clase.size_izquierdo = x[0].size_izquierdo;
                    clase.size_derecho = x[0].size_derecho;
                    clase.cn_tag_num = Convert.ToInt32(x[0].CartonNumber);

                    EtiquetaCajaModificada respuesta = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);

                    clase.id_string = respuesta.codigoBarrasTextoB.ToString();

                    //clase.id_string = "*" + clase.id_string + "*";

                    //clase.assembly = "*" + clase.id_string + "*";

                    //clase.anio = DateTime.Now.Year.ToString() + clase.id_string;

                    clase.anio = respuesta.codigoBarrasTextoB.ToString();
                    clase.Vendor = clase.anio;



                    id_inventarioAnt = Convert.ToInt32(clase.id_string);
                    int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);



                    List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                    listclaseZ.Add(clase);
                    //id_inventarioAnt = clase.id_inventario;
                    id_inventarioAnt = Convert.ToInt32(clase.id_string);
                    ReporteCajaSaftech report = new ReporteCajaSaftech
                    {
                        DataSource = listclaseZ
                    };
                    report.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool = new ReportPrintTool(report);
                    //tool.ShowPreview();
                    //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                    tool.Print(); //imprime de golpe

                    LimpiarPantallaEscaneo();

                    contador = 0;
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();

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

        private void Escaneo_SAFTECH(ref List<ConsultaUPC_VarcharResult> ListUpc, string po_numero)
        {
            #region SAFTECH 
            /*UPC CORRECTO*/
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
                    separadas = cmbSizes.Text.Split('x');
                    /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                    List<ConsultaProductosBensResult> x = f.ConsultaProductosBens(cmbPO.Text, separadas[0].ToString().Trim(), separadas[1].ToString().Trim());
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
                        clase.po = string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))).ToString();
                        //Convert.ToDecimal();
                        clase.po_varchar = x[0].po_varchar.Trim();
                        clase.poInCompleto = Convert.ToDecimal(string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))));
                        clase.poItem = "33";
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
                        clase.Cantidad = Convert.ToDecimal(x[0].Cantidad);
                        clase.itemDescription = x[0].itemDescription;
                        clase.color = x[0].Color;
                        clase.size_izquierdo = x[0].size_izquierdo;
                        clase.size_derecho = x[0].size_derecho;
                        clase.cn_tag_num = Convert.ToInt32(x[0].CartonNumber);

                        EtiquetaCajaModificada respuesta = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);

                        clase.id_string = respuesta.codigoBarrasTextoB.ToString();

                        //clase.id_string = x[0].Estilo + clase.size_izquierdo + clase.size_derecho + clase.QUANTITY + clase.id;

                        //clase.assembly = "*" + clase.id_string + "*";

                        //clase.anio = x[0].Estilo + ":" + clase.size_izquierdo + ":" + clase.size_derecho + ":" + clase.QUANTITY + ":" + clase.id;

                        clase.anio = respuesta.codigoBarrasTextoB.ToString();
                        clase.Vendor = clase.anio;



                        id_inventarioAnt = Convert.ToInt32(clase.id_string);
                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                        List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                        listclaseZ.Add(clase);
                        //id_inventarioAnt = clase.id_inventario;
                        id_inventarioAnt = Convert.ToInt32(clase.id_string);
                        ReporteCajaSaftech report = new ReporteCajaSaftech
                        {
                            DataSource = listclaseZ
                        };
                        report.PrintingSystem.ShowMarginsWarning = false;
                        ReportPrintTool tool = new ReportPrintTool(report);
                        //tool.ShowPreview();
                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                        tool.Print(); //imprime de golpe

                        LimpiarPantallaEscaneo();

                        contador = 0;
                        txtUPCScann.Text = string.Empty;
                        txtUPCScann.Focus();

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
