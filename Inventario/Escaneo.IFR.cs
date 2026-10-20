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
    // Marca IFR (POItem 22)
    public partial class Escaneo
    {
        private void IncompleteCarton_IFR(string po_numero)
        {
            List<TallasMixtas> Mixeo = new List<TallasMixtas>();
            List<ConsultaUPC_VarcharResult> x2 = new List<ConsultaUPC_VarcharResult>();
            List<ConsultaUPC_VarcharResult> x3 = new List<ConsultaUPC_VarcharResult>();

            Int64 id_inventario = 0;
            string tallas = "";
            decimal? CantidadTotal = 0;
            decimal? Cantidad = 0;
            string cantidadPrendas = "";
            string estilo = "";
            int contador = 0;
            string size_izquierdo = "";
            string upc = "";
            string TallasCantidad = "";
            if (TallaMixta == 1)
            {


                List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                foreach (DataGridViewRow fila in dgvEscan.Rows)
                {
                    // Validar que la fila no sea nueva y que tenga datos
                    if (!fila.IsNewRow)
                    {
                        try
                        {
                            EtiquetaCajaModificada x = new EtiquetaCajaModificada();
                            x.size_izquierdo = fila.Cells["Talla"].Value?.ToString();
                            x.upc = fila.Cells["Codigo UPC"].Value?.ToString();


                            prendas = f.ConsultaPrendas_UPC(cmbPO.Text, x.upc);
                            if (Convert.ToInt32(prendas.diferencia) <= 0)
                            {
                                MessageBox.Show("Ya se escanearon todas las prendas de esa talla y ese po");

                                txtCartonsPacked.Text = prendas.prendasescaneadas;
                                txtCartonRq.Text = prendas.totalprendas;
                                txtCartonsReamaining.Text = prendas.diferencia;
                                lblCartonReq.Text = "Total\rRequired";
                                lblCartonsEmpacados.Text = "Total\rPacked";
                                lblCartonsRestantes.Text = "Total\rRemaining";


                                txtCartonsPacked.Visible = true;
                                txtCartonRq.Visible = true;
                                txtCartonsReamaining.Visible = true;
                                lblCartonReq.Visible = true;
                                lblCartonsEmpacados.Visible = true;
                                lblCartonsRestantes.Visible = true;
                                break;

                            }
                            x2 = f.ConsultaUPC(po_numero, x.upc);
                            x.id = x2[0].id;
                            x.po_varchar = fila.Cells["po_varchar"].Value?.ToString();
                            x.po = fila.Cells["po_varchar"].Value?.ToString();
                            x.Cantidad = Convert.ToInt32(fila.Cells["CantidadPrendas"].Value);
                            x.color = fila.Cells["Color"].Value?.ToString();
                            x.estilo = fila.Cells["Estilo"].Value?.ToString();
                            estilo = x.estilo;
                            x.ProductCode = fila.Cells["Estilo"].Value?.ToString();
                            x.poItem = "22";
                            x.mixto = 1;
                            if (contador == 0)
                            {
                                tallas = "SIZE" + "\r\n";
                                cantidadPrendas = "QTY" + "\r\n";
                                x.TipoCarton = "1";
                            }
                            listClase.Add(x);

                            CantidadTotal = CantidadTotal + x.Cantidad;
                            string resultadoTalla = "";
                            int indiceGuion = x.size_izquierdo.IndexOf('-');

                            if (indiceGuion != -1 && indiceGuion < x.size_izquierdo.Length - 1)
                            {
                                // Extraer desde el carácter después del guion
                                resultadoTalla = x.size_izquierdo.Substring(indiceGuion + 1);
                            }

                            tallas = tallas + resultadoTalla.ToString() + "\r";
                            cantidadPrendas = cantidadPrendas + x.Cantidad + "\r";

                            TallasCantidad += resultadoTalla.ToString() + ":" + x.Cantidad + ", ";

                            contador = contador + 1;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error en fila {fila.Index + 1}: {ex.Message}");
                        }
                    }
                }
                if (Convert.ToInt32(prendas.diferencia) <= 0)
                {

                }
                else
                {
                    var ifrdesc = f.ConsultaTallasColorIFR(estilo);
                    listClase[0].numeroEtiqueta1 = tallas;
                    listClase[0].numeroEtiqueta2 = cantidadPrendas;
                    Cantidad = listClase[0].Cantidad;
                    size_izquierdo = listClase[0].size_izquierdo;
                    listClase[0].size_izquierdo = "MIXTO";
                    listClase[0].Cantidad = 0;
                    listClase[0].TipoCarton = 1.ToString();
                    upc = listClase[0].upc;
                    listClase[0].upc = "";
                    listClase[0].descripcion = ifrdesc.descripcion;
                    listClase[0].descripcion2 = ifrdesc.descripcion2;
                    id_inventario = f.GuardaInventario(listClase[0], usu[0].id);
                    id_inventarioAnt = id_inventario;
                    listClase[0].Cantidad = Cantidad;
                    listClase[0].upc = upc;
                    listClase[0].size_izquierdo = size_izquierdo;

                    listClase[0].QUANTITY = Convert.ToInt32(CantidadTotal.ToString());


                    foreach (DataGridViewRow fila in dgvEscan.Rows)
                    {
                        if (!fila.IsNewRow)
                        {
                            if (listClase.Count == fila.Index)
                            {
                                break;
                            }
                            else
                            {
                                listClase[fila.Index].id_inventario = id_inventario;
                                listClase[fila.Index].TipoCarton = "MIXTO";
                                f.GuardaInventarioDetalle(listClase[fila.Index], usu[0].id);
                            }
                        }
                    }

                    List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                    listClase2.Add(listClase[0]);

                    ReporteCajaIFRMixtas report = new ReporteCajaIFRMixtas
                    {
                        DataSource = listClase2
                    };
                    // Disable margins warning. 
                    report.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool = new ReportPrintTool(report);
                    if (chkImprimirEtiqueta.Checked == true)
                    {
                        imprimir = true;
                    }

                    if (imprimir)
                    {
                        //tool.ShowPreview();
                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                        //tool.PrintDialog(); //muestra a que impresora se va a mandar
                        tool.Print(); //imprime de golpe
                                      // dgvEscan.Rows.Clear();

                    }
                    else
                    {

                    }

                    LimpiarPantallaEscaneo();
                }
            }
            else
            {


                // diferencia = Convert.ToInt32(f.ConsultaPrendas(po_numero, txtUPCScann.Text));

                //LimpiarPantallaEscaneo();
                prendas = f.ConsultaPrendas_UPC(cmbPO.Text, txtUPCScann.Text);
                if (Convert.ToInt32(prendas.diferencia) <= 0)
                {
                    //Ya se escanearon todas las prendas de esa talla y ese po
                    MessageBox.Show("Ya se escanearon todas las prendas de esa talla y ese po");

                    txtCartonsPacked.Text = prendas.prendasescaneadas;
                    txtCartonRq.Text = prendas.totalprendas;
                    txtCartonsReamaining.Text = prendas.diferencia;
                    lblCartonReq.Text = "Total\rRequired";
                    lblCartonsEmpacados.Text = "Total\rPacked";
                    lblCartonsRestantes.Text = "Total\rRemaining";


                    txtCartonsPacked.Visible = true;
                    txtCartonRq.Visible = true;
                    txtCartonsReamaining.Visible = true;
                    lblCartonReq.Visible = true;
                    lblCartonsEmpacados.Visible = true;
                    lblCartonsRestantes.Visible = true;
                }
                else
                {
                    List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);
                    EtiquetaCajaModificada clase = RellenaObjetoClaseINCOMPLETO(consulta);
                    List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                    clase.id_inventario = f.GuardaInventario(clase, usu[0].id);
                    cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                    //string codigoBarras = clase.id_inventario.ToString();
                    //clase.assembly = codigoBarras;
                    BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 7, FontStyle.Bold) };
                    Codigo.BarWidth = 3;
                    string contarDigitos = clase.id_inventario.ToString();
                    UPCResult resultado = GeneraCodigoUPC(contarDigitos.ToString());
                    clase.codigoBarras = resultado.CodigoBarras;
                    listClase.Add(clase);
                    id_inventarioAnt = clase.id_inventario; // ID de la caja (antes guardaba el ID del producto y Print Last Label reimprimia otra caja)
                    ReporteCajaIFR report = new ReporteCajaIFR
                    {
                        DataSource = listClase
                    };
                    // Disable margins warning. 
                    report.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool = new ReportPrintTool(report);
                    if (chkImprimirEtiqueta.Checked == true)
                    {
                        imprimir = true;
                    }

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

                    LimpiarPantallaEscaneo();
                }
            }
        
        }

        // Reimpresion de una caja mixta IFR: arma la etiqueta como IncompleteCarton_IFR pero con el detalle guardado
        private void ReimprimirMixtaIFR(ConsultaInventarioIDResult caja, int idInv)
        {
            List<ConsultaInventarioDetalleResult> detalle = f.ConsultaInventarioDetalle(idInv);
            if (detalle == null || detalle.Count == 0)
            {
                MessageBox.Show("No se encontró detalle para esta caja mixta.");
                return;
            }

            string tallas = "SIZE" + "\r\n";
            string cantidadPrendas = "QTY" + "\r\n";
            int cantidadTotal = 0;
            foreach (ConsultaInventarioDetalleResult d in detalle)
            {
                string talla = d.size_izquierdo ?? "";
                int guion = talla.IndexOf('-');
                if (guion != -1 && guion < talla.Length - 1) talla = talla.Substring(guion + 1);
                tallas += talla + "\r";
                cantidadPrendas += (d.Cantidad ?? 0) + "\r";
                cantidadTotal += d.Cantidad ?? 0;
            }

            string estilo = !string.IsNullOrEmpty(detalle[0].estilo) ? detalle[0].estilo : caja.estilo;
            var ifrdesc = f.ConsultaTallasColorIFR(estilo);
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                po = !string.IsNullOrEmpty(caja.po_varchar) ? caja.po_varchar : caja.po,
                po_varchar = !string.IsNullOrEmpty(caja.po_varchar) ? caja.po_varchar : caja.po,
                estilo = estilo,
                ProductCode = estilo,
                color = detalle[0].color,
                size_izquierdo = detalle[0].size_izquierdo,
                Cantidad = detalle[0].Cantidad ?? 0,
                poItem = "22",
                mixto = 1,
                TipoCarton = "1",
                numeroEtiqueta1 = tallas,
                numeroEtiqueta2 = cantidadPrendas,
                QUANTITY = cantidadTotal,
                descripcion = ifrdesc != null ? ifrdesc.descripcion : "",
                descripcion2 = ifrdesc != null ? ifrdesc.descripcion2 : "",
                id_inventario = idInv
            };

            ReporteCajaIFRMixtas report = new ReporteCajaIFRMixtas
            {
                DataSource = new List<EtiquetaCajaModificada> { clase }
            };
            report.PrintingSystem.ShowMarginsWarning = false;
            new ReportPrintTool(report).Print();
        }

        private void SeleccionTalla_IFR()
        {
            prendas = f.ConsultaPrendas_Talla(cmbPO.Text, cmbSizes.Text.Trim());

            if (Convert.ToInt32(prendas.diferencia) == 0)
            {
                //Ya se escanearon todas las prendas de esa talla y ese po
                MessageBox.Show("Ya se escanearon todas las prendas de esa talla y ese po");

                txtCartonsPacked.Text = prendas.prendasescaneadas;
                txtCartonRq.Text = prendas.totalprendas;
                txtCartonsReamaining.Text = prendas.diferencia;
                lblCartonReq.Text = "Total\rRequired";
                lblCartonsEmpacados.Text = "Total\rPacked";
                lblCartonsRestantes.Text = "Total\rRemaining";


                txtCartonsPacked.Visible = true;
                txtCartonRq.Visible = true;
                txtCartonsReamaining.Visible = true;
                lblCartonReq.Visible = true;
                lblCartonsEmpacados.Visible = true;
                lblCartonsRestantes.Visible = true;

                txtSize.Text = cmbSizes.Text.Trim();
            }
            else
            {
                //Faltan por escanear ' + CONVERT(VARCHAR(10),@DiferenciaPrendas) + ' prendas de esa talla y ese po


                List<ConsultaProductos_VarcharResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, cmbSizes.Text.ToString(), "");
                if (x.Count > 0)
                {

                    contador = 0;
                    dgvEscan.DataSource = x;

                    lblImprimirEtiqueta.Visible = true;
                    chkImprimirEtiqueta.Visible = true;
                    txtCartonsPacked.Text = x[0].CartonNumber.ToString();
                    //txtCartonNumber.Text = x[0].CartonNumber.ToString();
                    txtCartonSize.Text = x[0].Size.ToString();
                    dgvEscan.Columns["Cantidad"].Visible = false;
                    dgvEscan.Columns["ProductCode"].Visible = false;
                    dgvEscan.Columns["ProductCode1"].Visible = false;
                    dgvEscan.Columns["id"].Visible = false;
                    txtSize.Text = x[0].Size.ToString();
                    txtProductCode.Text = x[0].ProductCode.ToString();
                    id = x[0].id;
                    anterior.Add(id);
                    upc = x[0].UPC.ToString();
                    ///po = Convert.ToInt32(cmbPO.Text);
                    cantidad = x[0].cantidad;
                    txtUnitsReq.Text = cantidad.ToString();
                    txtUnitsScan.Text = "0";
                    txtUnitsRemai.Text = cantidad.ToString();
                    txtUPCScann.Focus();
                }
                txtCartonsPacked.Text = prendas.prendasescaneadas;
                txtCartonRq.Text = prendas.totalprendas;
                txtCartonsReamaining.Text = prendas.diferencia;
                lblCartonReq.Text = "Total\rRequired";
                lblCartonsEmpacados.Text = "Total\rPacked";
                lblCartonsRestantes.Text = "Total\rRemaining";


                txtCartonsPacked.Visible = true;
                txtCartonRq.Visible = true;
                txtCartonsReamaining.Visible = true;
                lblCartonReq.Visible = true;
                lblCartonsEmpacados.Visible = true;
                lblCartonsRestantes.Visible = true;
                txtUPCScann.Focus();
                ///garments
            }

        
        }

        private void Escaneo_IFR(ref List<ConsultaUPC_VarcharResult> ListUpc, string po_numero)
        {
            if (upc == txtUPCScann.Text && TallaMixta == 0)
            {
                //ifr
                #region ESCANEANDO X PRENDA AUN 
                /*ESCANEANDO X PRENDA AUN*/
                contador = contador + 1;
                dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
                dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
                txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();

                sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";
                #endregion

                #region ESCANEANDO COMPLETO
                if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text))
                {
                    prendas = f.ConsultaPrendas_UPC(cmbPO.Text, upc);
                    // diferencia = Convert.ToInt32(f.ConsultaPrendas(po_numero, txtUPCScann.Text));

                    //LimpiarPantallaEscaneo();

                    if (Convert.ToInt32(prendas.diferencia) <= 0)
                    {
                        //Ya se escanearon todas las prendas de esa talla y ese po
                        MessageBox.Show("Ya se escanearon todas las prendas de esa talla y ese po");

                        txtCartonsPacked.Text = prendas.prendasescaneadas;
                        txtCartonRq.Text = prendas.totalprendas;
                        txtCartonsReamaining.Text = prendas.diferencia;
                        lblCartonReq.Text = "Total\rRequired";
                        lblCartonsEmpacados.Text = "Total\rPacked";
                        lblCartonsRestantes.Text = "Total\rRemaining";


                        txtCartonsPacked.Visible = true;
                        txtCartonRq.Visible = true;
                        txtCartonsReamaining.Visible = true;
                        lblCartonReq.Visible = true;
                        lblCartonsEmpacados.Visible = true;
                        lblCartonsRestantes.Visible = true;
                    }
                    else
                    {
                        //Faltan por escanear ' + CONVERT(VARCHAR(10),@DiferenciaPrendas) + ' prendas de esa talla y ese po
                        //txtUnitsRemai.Text = diferencia.ToString();
                        if ((Convert.ToInt32(prendas.prendasescaneadas) + Convert.ToInt32(txtUnitsScan.Text)) <= Convert.ToInt32(prendas.totalprendas))
                        {
                            List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);
                            EtiquetaCajaModificada clase = RellenaObjetoClase(consulta);
                            List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                            clase.TipoCarton = "1";
                            clase.mixto = 0;
                            clase.poItem = "22";
                            clase.id_inventario = f.GuardaInventario(clase, usu[0].id);
                            cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                            //string codigoBarras = clase.id_inventario.ToString();
                            //clase.assembly = codigoBarras;
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 7, FontStyle.Bold) };
                            Codigo.BarWidth = 3;
                            clase.upc = f.ConsultaTallasIFRcodigoupc(clase.size_izquierdo);
                            string contarDigitos = clase.upc.ToString();
                            UPCResult resultado = GeneraCodigoUPC(contarDigitos.ToString());
                            clase.codigoBarras = resultado.CodigoBarras;
                            listClase.Add(clase);
                            id_inventarioAnt = clase.id_inventario; // ID de la caja (antes guardaba el ID del producto y Print Last Label reimprimia otra caja)
                            ReporteCajaIFR report = new ReporteCajaIFR
                            {
                                DataSource = listClase
                            };
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            if (chkImprimirEtiqueta.Checked == true)
                            {
                                imprimir = true;
                            }

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

                            LimpiarPantallaEscaneo();
                        }
                        else
                        {
                            int PrendasDemas = Convert.ToInt32(prendas.totalprendas) - (Convert.ToInt32(prendas.prendasescaneadas) + Convert.ToInt32(txtUnitsScan.Text));
                            MessageBox.Show("Deben de ser " + Convert.ToInt32(prendas.totalprendas) + " y se excedio " + PrendasDemas);
                            LimpiarPantallaEscaneo();
                        }
                        LimpiarPantallaEscaneo();
                        txtCartonsPacked.Text = prendas.prendasescaneadas;
                        txtCartonRq.Text = prendas.totalprendas;
                        txtCartonsReamaining.Text = prendas.diferencia;
                        lblCartonReq.Text = "Total\rRequired";
                        lblCartonsEmpacados.Text = "Total\rPacked";
                        lblCartonsRestantes.Text = "Total\rRemaining";


                        txtCartonsPacked.Visible = true;
                        txtCartonRq.Visible = true;
                        txtCartonsReamaining.Visible = true;
                        lblCartonReq.Visible = true;
                        lblCartonsEmpacados.Visible = true;
                        lblCartonsRestantes.Visible = true;
                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                        cmbSizes.SelectedIndex = 0;
                        cmbSizes.SelectedIndex = 1;
                        cmbSizes.SelectedIndex = idIndex;
                    }

                }
                #endregion
            }
            else if (TallaMixta == 1)
            {   /*PRENDAS MIXTAS*/ /*ESCANEANDO X PRENDA AUN*/
                try
                {
                    ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                    txtSize.Text = ListUpc[0].Size;
                }
                catch (Exception ex)
                {
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;
                    do
                    {
                        string mensaje = "PRESIONA EN NO PARA CONTINUAR";
                        string titulo = "Favor de Escanear la prenda correcta!";
                        result = MessageBox.Show(this, mensaje, titulo, buttons,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                        MessageBoxOptions.RightAlign);
                        sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                        sonido.controls.play();
                        txtUPCScann.Text = string.Empty;
                        tpEscan.BackColor = Color.Red;
                    } while (result == DialogResult.Yes);
                    ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                    txtSize.Text = ListUpc[0].Size;
                }
                int tallarepetida = 0;
                int totalPrendas = 0;
                if (contador > 0)
                {
                    //Recorre tabla de lo escaneado
                    foreach (DataGridViewRow fila in dgvEscan.Rows)
                    {
                        if (fila.Cells["Talla"].Value != null && fila.Cells["CantidadPrendas"].Value != null &&
                            fila.Cells["Talla"].Value.ToString().Equals(ListUpc[0].size_izquierdo, StringComparison.OrdinalIgnoreCase))
                        {
                            // Actualizar celda "CantidadPrendas"
                            fila.Cells["CantidadPrendas"].Value = Convert.ToInt32(fila.Cells["CantidadPrendas"].Value) + 1;
                            tallarepetida = 1;
                            //break; // Salir después de encontrar el primero
                        }
                        totalPrendas = totalPrendas + Convert.ToInt32(fila.Cells["CantidadPrendas"].Value);
                    }

                }
                if (tallarepetida == 0)
                {
                    if (contador == 0)
                    {
                        dgvEscan.DataSource = null;
                        dgvEscan.Rows.Clear();
                        dgvEscan.Refresh();
                        tablaEscaner = new DataTable();
                        tablaEscaner.Columns.Add("Talla", typeof(string));
                        tablaEscaner.Columns.Add("po_varchar", typeof(string));
                        tablaEscaner.Columns.Add("Codigo UPC", typeof(string));
                        tablaEscaner.Columns.Add("CantidadPrendas", typeof(long));
                        tablaEscaner.Columns.Add("Color", typeof(string));
                        tablaEscaner.Columns.Add("Estilo", typeof(string));
                    }
                    contador = contador + 1;
                    totalPrendas = totalPrendas + 1;
                    tablaEscaner.Rows.Add(ListUpc[0].size_izquierdo, ListUpc[0].po, ListUpc[0].upc, "1", ListUpc[0].color, ListUpc[0].estilo);
                    dgvEscan.DataSource = tablaEscaner;
                }

                dgvEscan.Refresh();
                //dgvEscan.Columns["idSize"].Visible = false;
                dgvEscan.Rows[(Convert.ToInt32(contador))].Selected = true;
                dgvEscan.FirstDisplayedScrollingRowIndex = (contador);
                txtUnitsScan.Text = (totalPrendas).ToString();
                txtUnitsRemai.Text = "X";
                txtUnitsReq.Text = "X";
                //txtCartonRq.Text = "";
                //txtCartonsPacked.Text = "";
                //txtCartonsReamaining.Text = "";
                txtCartonsPacked.Text = prendas.prendasescaneadas;
                txtCartonRq.Text = prendas.totalprendas;
                txtCartonsReamaining.Text = prendas.diferencia;
                lblCartonReq.Text = "Garments\rRequired";
                lblCartonsEmpacados.Text = "Garments\rPacked";
                lblCartonsRestantes.Text = "Garments\rRemaining";


                txtCartonsPacked.Visible = true;
                txtCartonRq.Visible = true;
                txtCartonsReamaining.Visible = true;
                lblCartonReq.Visible = true;
                lblCartonsEmpacados.Visible = true;
                lblCartonsRestantes.Visible = true;
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();

                sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";

            }
            else
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                do
                {
                    string mensaje = "PRESIONA EN NO PARA CONTINUAR";
                    string titulo = "Favor de Escanear la prenda correcta!";
                    result = MessageBox.Show(this, mensaje, titulo, buttons,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);
                    sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                    sonido.controls.play();
                    txtUPCScann.Text = string.Empty;
                    tpEscan.BackColor = Color.Red;
                } while (result == DialogResult.Yes);
                //  MessageBox.Show("Favor de Escanear la prenda correcta!");

            }

        
        }
    }
}
