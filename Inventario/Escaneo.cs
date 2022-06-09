using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using Excel;
using Microsoft.VisualBasic;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;


namespace Inventario
{
    public partial class Escaneo : Form
    {
        Funciones f = new Funciones();
        bool iniciando = true;
        string cantidad = "0";
        int cantidadAnterior = 0;
        string upc = string.Empty;
        string vendor = string.Empty;
        WindowsMediaPlayer sonido = new WindowsMediaPlayer();
        List<ConsultaUsuarioResult> usu = null;
        int? id = 0;
        List<int?> anterior = new List<int?>();
        int contador = 0;
        bool agregar = false;
        int id_InventarioAnt = 0;
        int? CantidadDividir = 0;
        int po = 0;
        int po2 = 0;
        DataTable tablaPrepack = new DataTable();
        DataTable tablaAltaPO = new DataTable();

        int RengloSelecionado;
        int Tabpage = 0;
        int alta = 0;
        int altaP = 0;
        /*99 ES PREPACK*/
        /*1000 ES LEVIS CINTAS IMPORTADO Y TODO LO QUE SE DE DE ALTA MANUAL */
        /*88  ES TARGET */ 
        public void InicializarTablas()
        {

             tablaPrepack = new DataTable();
             tablaAltaPO = new DataTable();
        }
        public Escaneo(List<ConsultaUsuarioResult> usuario)
        {
            try
            {
                InitializeComponent();
                f.ConsultaPO(this.cmbPO);
                cmbPO.SelectedIndex = -1;
                f.ConsultaCliente(this.cmbCliente);
                cmbCliente.SelectedIndex = 0;
                f.ConsultaFactura(this.cmbFactura);

                cmbFactura.SelectedIndex = 0;
                f.ConsultaTerminado(this.cmbTerminado);
                cmbTerminado.SelectedIndex = 0;

                /**/
                f.ConsultaPO(this.cmbPOB);
                cmbPOB.SelectedIndex = 0;
                f.ConsultaCliente(this.cmbClienteB);
                cmbClienteB.SelectedIndex = 0;
                f.ConsultaFactura(this.cmbFacturacionB);
                cmbFacturacionB.SelectedIndex = 0;
                f.ConsultaTerminado(this.cmbTerminadoB);
                cmbTerminadoB.SelectedIndex = 0;


                f.ConsultaTallas(this.cmbSizeA);
                cmbSizeA.SelectedIndex = 0;


                f.ConsultaTallas(this.cmbTallaPrepack);
                cmbTallaPrepack.SelectedIndex = 0;


                f.ConsultaTallas(this.cmbAltaPOTalla);
                cmbAltaPOTalla.SelectedIndex = 0;


                f.ConsultaTipoCaja(this.cmbTipoCajaA);
                cmbTipoCajaA.SelectedIndex = 15;

                /**/
                f.ConsultaPO(this.cmbPoEntradaAlmacen);
                cmbPoEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaCliente(this.cmbClienteEntradaAlmacen);
                cmbClienteEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaTerminado(cmbTerminadoEntradaAlmacen);
                cmbTerminadoEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaFactura(cmbFacturacionEntradaAlmacen);
                cmbFacturacionEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaPO(cmbPOSalidaAlmacen);
                cmbPOSalidaAlmacen.SelectedIndex = 0;
                f.ConsultaTallas(clbT);

                this.usu = usuario;
                try
                {
                    lblVersion.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                    lblVersion2.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
                catch (Exception ex) { }

                if (usu[0].perfil == "4" || usu[0].perfil == "3" || usu[0].perfil == "8")
                {
                    cmbPoEntradaAlmacen.Enabled = true;
                    cmbClienteEntradaAlmacen.Enabled = true;
                    cmbFacturacionEntradaAlmacen.Enabled = true;
                    cmbTerminadoEntradaAlmacen.Enabled = true;
                    txtUbicacionID.Enabled = true;
                    txtIDCaja.Enabled = true;
                    btnGuardar.Enabled = true;
                    btnSeleccionarTodo.Enabled = true;
                    btnDeseleccionarTodo.Enabled = true;
                    clbT.Enabled = true;
                    cmbPOSalidaAlmacen.Enabled = true;
                    txtIDCajaSalida.Enabled = true;
                    btnGuardarSalida.Enabled = true;
                    btnTerminar.Enabled = true;
                    dgvSalida.Enabled = true;
                    txtNuevaUbicacion.Enabled = true;
                    txtCajaidMover.Enabled = true;
                    txtBajaCajaID.Enabled = true;
                    btnBajaAlmacen.Enabled = true;
                    btnGuardarMovimiento.Enabled = true;
                    dtpFechaInicioAlmacen.Enabled = true;
                    dtpFechaInicioAlmacen.Enabled = true;
                    btnBuscarAlmacen.Enabled = true;
                    dgvReporteAlmacen.Enabled = true;
                    dtpFechaInicioEmbarques.Enabled = true;
                    dtpFechaInicioEmbarques.Enabled = true;
                    btnBuscarEmbarques.Enabled = true;
                    dgReporteEmbarques.Enabled = true;
                    btnImprimirEtiquetaAlmacen.Enabled = true;
                    txtIDReimpresionAlmacen.Enabled = true;
                    txtUbicacionID.Enabled = true;
                    gcReporteAlmacen.Enabled = true;
                    dtpFechaFinalAlmacen.Enabled = true;
                    f.ConsultaUbicacion(txtUbicacionID);
                    txtUbicacionID.SelectedIndex = 0;
                    dtpFechaInicioEmbarques.Enabled = true;
                    dtpFechaFinalEmbarques.Enabled = true;
                    btnBuscarEmbarques.Enabled = true;
                    dgReporteEmbarques.Enabled = true;
                    txtCajaIDDividir.Enabled = true;
                    txtPiezas.Enabled = true;
                    btnGuardarDivision.Enabled = true;
                    txtIDReimpresionAlmacen.Enabled = true;
                    txtUbicacionID.Enabled = true;
                    btnActualizar.Enabled = true;
                }

                if (usu[0].perfil == "6")
                {
                    TabEsaneo.TabPages.RemoveAt(0);
                    TabEsaneo.TabPages.RemoveAt(3);
                    TabEsaneo.TabPages.RemoveAt(2);
                    btnBajaCaja.Enabled = true;
                    txtCaja.Enabled = true;
                }
                else if (usu[0].perfil == "5")
                {
                    TabEsaneo.TabPages.RemoveAt(0);
                    TabEsaneo.TabPages.RemoveAt(4);
                    TabEsaneo.TabPages.RemoveAt(3);
                    TabEsaneo.TabPages.RemoveAt(2);
                    TabEsaneo.TabPages.RemoveAt(1);
                }
                else if (usu[0].perfil == "8")
                {
                }
                else if (usu[0].perfil == "9")
                {
                    TabEsaneo.TabPages.RemoveAt(7);
                    TabEsaneo.TabPages.RemoveAt(6);
                    TabEsaneo.TabPages.RemoveAt(5);
                    TabEsaneo.TabPages.RemoveAt(4);
                    TabEsaneo.TabPages.RemoveAt(3);
                    TabEsaneo.TabPages.RemoveAt(2);

                }
                else if (usu[0].perfil == "10")
                {
                    TabEsaneo.TabPages.RemoveAt(7);
                    TabEsaneo.TabPages.RemoveAt(6);
                    //  TabEsaneo.TabPages.RemoveAt(5);
                    TabEsaneo.TabPages.RemoveAt(4);
                    TabEsaneo.TabPages.RemoveAt(3);
                    TabEsaneo.TabPages.RemoveAt(2);
                    TabEsaneo.TabPages.RemoveAt(0);
                    TabEsaneo.TabPages.Remove(tpImportar);

                    //  SE ABILITAN BOTONES PARA BAJA DE PO O CAJAS
                    cmbPOB.Enabled = true;
                    cmbClienteB.Enabled = true;
                    cmbFacturacionB.Enabled = true;
                    cmbTerminadoB.Enabled = true;
                    btnBajaPO.Enabled = true;

                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;

                }
                else if (usu[0].perfil == "11")
                {

                    TabEsaneo.TabPages.Remove(tpReporteDiario);
                    TabEsaneo.TabPages.Remove(tpBajaCaja);
                    TabEsaneo.TabPages.Remove(tpAlta);
                    //TabEsaneo.TabPages.Remove(tpImportar);
                    // TabEsaneo.TabPages.Remove(tpbajaPO);
                    TabEsaneo.TabPages.Remove(tpReimpresion);
                    cmbPOB.Enabled = true;
                    cmbClienteB.Enabled = true;
                    cmbFacturacionB.Enabled = true;
                    cmbTerminadoB.Enabled = true;
                    btnBajaPO.Enabled = true;
                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                    //  SE ABILITAN BOTONES PARA BAJA DE PO O CAJAS
                    cmbPOB.Enabled = true;
                    cmbClienteB.Enabled = true;
                    cmbFacturacionB.Enabled = true;
                    cmbTerminadoB.Enabled = true;
                    btnBajaPO.Enabled = true;

                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                }
                else if (usu[0].perfil == "12")
                {
                    TabEsaneo.TabPages.Remove(tpAltaPrePack);
                    TabEsaneo.TabPages.Remove(tpImportar);
                    TabEsaneo.TabPages.Remove(tpReporteDiario);
                    TabEsaneo.TabPages.Remove(tpBajaCaja);
                    TabEsaneo.TabPages.Remove(tpbajaPO);
                    TabEsaneo.TabPages.Remove(tbpTarget);
                    //TabEsaneo.TabPages.Remove(tpBajaPOLEVIS);
                    TabEsaneo.TabPages.Remove(tpAlta);
                    TabEsaneo.TabPages.Remove(tpReimpresion);
                    TabEsaneo.TabPages.Remove(tpImportarLevis);
                    TabEsaneo.TabPages.Remove(tpAltaPO);

                    cmbPOB.Enabled = true;
                    cmbClienteB.Enabled = true;
                    cmbFacturacionB.Enabled = true;
                    cmbTerminadoB.Enabled = true;
                    btnBajaPO.Enabled = true;
                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                    //  SE ABILITAN BOTONES PARA BAJA DE PO O CAJAS
                    cmbPOB.Enabled = true;
                    cmbClienteB.Enabled = true;
                    cmbFacturacionB.Enabled = true;
                    cmbTerminadoB.Enabled = true;
                    btnBajaPO.Enabled = true;

                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                }
                else
                {
                    TabEsaneo.TabPages.RemoveAt(3);
                }
                //  SE ABILITAN BOTONES PARA BAJA DE PO O CAJAS
                cmbPOB.Enabled = true;
                cmbClienteB.Enabled = true;
                cmbFacturacionB.Enabled = true;
                cmbTerminadoB.Enabled = true;
                btnBajaPO.Enabled = true;

                txtCaja.Enabled = true;
                btnBajaCaja.Enabled = true;
                iniciando = false;

                //ConsultaCajasPO();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public Escaneo()
        {
            try
            {
                InitializeComponent();
                f.ConsultaPO(this.cmbPO);
                cmbPO.SelectedIndex = -1;
                iniciando = false;



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Seguro que deseas Cerrar el programa", "Cerrar", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }

        private void cmbPO_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!iniciando)
                {
                    iniciando = true;
                    f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                    cmbPOItem.SelectedIndex = 0;
                    f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                    cmbProductCode.SelectedIndex = 0;
                    f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                    cmbSizes.SelectedIndex = -1;
                    iniciando = false;
                    cmbSizes.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbPOItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!iniciando && cmbPO.SelectedIndex > -1 && cmbPOItem.SelectedIndex > -1)
                {
                    iniciando = true;
                    f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                    cmbProductCode.SelectedIndex = 0;
                    cmbProductCode.Focus();
                    iniciando = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnIncompleteCarton_Click(object sender, EventArgs e)
        {
            try
            {
                if (id != 0 && Convert.ToInt32(txtUnitsScan.Text) > 0 && dgvEscan.RowCount > 0)
                {
                    if (cmbPOItem.Text != "99")
                    {
                        txtUPCScann.Text = string.Empty;
                        txtUPCScann.Focus();
                        List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);
                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                        clase.id = consulta[0].id;
                        clase.po = consulta[0].po;
                        clase.poInCompleto = consulta[0].poInCompleto;
                        clase.poItem = consulta[0].poItem;
                        clase.ProductCode = consulta[0].ProductCode;
                        clase.Size = consulta[0].Size;
                        clase.size_derecho = consulta[0].size_derecho;
                        clase.size_izquierdo = consulta[0].size_izquierdo;
                        clase.TipoCarton = consulta[0].TipoCarton;
                        clase.upc = consulta[0].upc;
                        clase.Fecha = DateTime.Now;
                        clase.CartonLeft = consulta[0].CartonLeft;
                        clase.CartonRight = consulta[0].CartonRight;
                        clase.Cantidad = Convert.ToInt32(txtUnitsScan.Text);
                        clase.Carton = consulta[0].Carton;
                        clase.usuario = usu[0].nombre;
                        clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                        clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                        clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                        clase.cliente = cmbCliente.Text;
                        clase.factura = cmbFactura.Text;
                        clase.terminado = cmbTerminado.Text;
                        /**/


                        if (clase.poItem == "1000")
                        {
                            do
                            {
                                try
                                {
                                    clase.Carton = Convert.ToInt64(Interaction.InputBox("Captura Numero de Carton", "Carton", "", 5, 5));

                                }
                                catch (Exception ex)
                                { clase.Carton = 0; MessageBox.Show("Favor de ingresar correctamente el numero de carton ya que no se ha guardado"); };

                            } while (clase.Carton == 0 || clase.Carton.ToString().Length > 10);

                        }
                        if ((clase.poItem != "1000" && clase.poItem != "99") || (clase.poItem == "1000" && clase.Carton != 0 && clase.poItem != "99"))
                        {
                            clase.assembly = consulta[0].Assembly;
                            clase.Vendor = consulta[0].Vendor;
                            clase.ShipTo = consulta[0].ShipTo;
                            clase.id_Inventario = f.GuardaInventario(clase, this.usu[0].id);

                            if (clase.poItem != "1000" && clase.poItem != "99")
                            {
                                //clase.id_Inventario = 116;
                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_Inventario +
                                                                                  "&po=" + consulta[0].po +
                                                                                  "&cl=" + cmbCliente.Text +
                                                                                  "&fa=" + cmbFactura.Text +
                                                                                  "&te=" + cmbTerminado.Text +
                                                                                  "&u=" + clase.usuario +
                                                                                  "&pc=" + consulta[0].ProductCode +
                                                                                  "&c=" + Convert.ToInt32(txtUnitsScan.Text) +
                                                                                  "&sz=" + consulta[0].Size +
                                                                                  "&fe=" + DateTime.Now,
                                                                                  QRCodeGenerator.ECCLevel.Q);

                                cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                QRCode qrCode = new QRCode(qrCodeData);
                                BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                                Codigo.IncludeLabel = true;
                                Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 200, 100);
                                clase.qr = qrCode.GetGraphic(20);
                                clase.codigoBarras = codigoBarras;
                                listClase.Add(clase);
                                id_InventarioAnt = clase.id_Inventario;

                                ReporteCaja report = new ReporteCaja();
                                report.DataSource = listClase;
                                // Disable margins warning. 
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                tool.Print(); //imprime de golpe
                                LimpiarPantallaEscaneo();
                            }
                            else if (clase.poItem == "1000")
                            {
                                if (consulta[0].upc.Length > 14)
                                {
                                    clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 9);
                                    clase.numeroEtiqueta2 = consulta[0].upc.Substring(9, 7);
                                    clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 5) + " - " + consulta[0].upc.Substring(6, 2);
                                    clase.upc = "0" + consulta[0].upc.Substring(0, 8).ToString() + clase.numeroEtiqueta2;
                                }
                                else
                                {
                                    try
                                    {
                                        clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 7);
                                        clase.numeroEtiqueta2 = consulta[0].upc.Substring(7, 7);//aquiii
                                        clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 4) + " - " + consulta[0].upc.Substring(4, 2);
                                        clase.upc = "0" + consulta[0].upc.Substring(0, 6).ToString() + clase.numeroEtiqueta2;

                                    }
                                    catch (Exception ex)
                                    {
                                        clase.upc = consulta[0].upc;
                                    }
                                }
                                cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                                Codigo.IncludeLabel = true;
                                Codigo.RotateFlipType = RotateFlipType.Rotate90FlipY;
                                Image codigoBarras =
                                Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 270, 180);
                                // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                                clase.codigoBarras = codigoBarras;
                                listClase.Add(clase);
                                id_InventarioAnt = clase.id_Inventario;

                                ReporteCintas report = new ReporteCintas();
                                report.DataSource = listClase;
                                // Disable margins warning. 
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                //tool.Print(); //imprime de golpe
                                LimpiarPantallaEscaneo();
                                /*
                                contador = 0;
                                txtUPCScann.Text = string.Empty;
                                txtUnitsReq.Text = cantidad.ToString();
                                txtUnitsScan.Text = "0";
                                txtUnitsRemai.Text = cantidad.ToString();
                                dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
                                dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
                                txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text)).ToString();
                                txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text)).ToString();
                                txtCartonRq.Text = "0";
                                txtCartonsPacked.Text = "0";
                                txtCartonsReamaining.Text = "0";
                                txtUPCScann.Focus();
                                */

                            }





                            /*     aqui 

                            if (clase.poItem == "1000")
                                    {
                                        do
                                        {
                                            try
                                            {
                                                clase.Carton = Convert.ToInt64(Interaction.InputBox("Captura Numero de Carton", "Carton", "", 5, 5));

                                            }
                                            catch (Exception ex)
                                            { clase.Carton = 0; MessageBox.Show("Favor de ingresar correctamente el numero de carton ya que no se ha guardado"); };

                                        } while (clase.Carton == 0 || clase.Carton.ToString().Length > 10);
                                    }


                                    if ((clase.poItem != "1000") || (clase.poItem == "1000" && clase.Carton != 0))
                                    {

                                        clase.assembly = consulta[0].Assembly;
                                        clase.Vendor = consulta[0].Vendor;
                                        clase.ShipTo = consulta[0].ShipTo;
                                        clase.id_Inventario = f.GuardaInventario(clase, this.usu[0].id);


                                        if (clase.poItem != "1000")
                                        {
                                            //clase.id_Inventario = 116;
                                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                            QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_Inventario +
                                                                                              "&po=" + consulta[0].po +
                                                                                              "&cl=" + cmbCliente.Text +
                                                                                              "&fa=" + cmbFactura.Text +
                                                                                              "&te=" + cmbTerminado.Text +
                                                                                              "&u=" + clase.usuario +
                                                                                              "&pc=" + consulta[0].ProductCode +
                                                                                              "&c=" + Convert.ToInt32(txtUnitsScan.Text) +
                                                                                              "&sz=" + consulta[0].Size +
                                                                                              "&fe=" + DateTime.Now,
                                                                                              QRCodeGenerator.ECCLevel.Q);
                                            QRCode qrCode = new QRCode(qrCodeData);
                                            cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                                            Codigo.IncludeLabel = true;
                                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 200, 100);
                                            clase.qr = qrCode.GetGraphic(20);
                                            clase.codigoBarras = codigoBarras;
                                            listClase.Add(clase);
                                            id_InventarioAnt = clase.id_Inventario;

                                            ReporteCaja report = new ReporteCaja();
                                            report.DataSource = listClase;
                                            // Disable margins warning. 
                                            report.PrintingSystem.ShowMarginsWarning = false;
                                            ReportPrintTool tool = new ReportPrintTool(report);
                                            //tool.ShowPreview();
                                            //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                            //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                            tool.Print(); //imprime de golpe
                                        }
                                        else if (clase.poItem == "1000")
                                        {
                                            if (consulta[0].upc.Length > 14)
                                            {

                                                clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 9);
                                                clase.numeroEtiqueta2 = consulta[0].upc.Substring(9, 7);
                                                clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 5) + " - " + consulta[0].upc.Substring(6, 2);
                                                clase.upc = "0" + consulta[0].upc.Substring(0, 8).ToString() + clase.numeroEtiqueta2;
                                            }
                                            else
                                            {
                                                clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 7);
                                                clase.numeroEtiqueta2 = consulta[0].upc.Substring(7, 7);
                                                clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 4) + " - " + consulta[0].upc.Substring(4, 2);
                                                clase.upc = "0" + consulta[0].upc.Substring(0, 6).ToString() + clase.numeroEtiqueta2;
                                            }

                                            cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                                            Codigo.IncludeLabel = true;
                                            Codigo.RotateFlipType = RotateFlipType.Rotate90FlipY;
                                            Image codigoBarras =
                                            Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.Carton.ToString(), Color.Black, Color.White, 270, 180);
                                            // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                                            clase.codigoBarras = codigoBarras;
                                            listClase.Add(clase);
                                            id_InventarioAnt = clase.id_Inventario;

                                            ReporteCintas report = new ReporteCintas();
                                            report.DataSource = listClase;
                                            // Disable margins warning. 
                                            report.PrintingSystem.ShowMarginsWarning = false;
                                            ReportPrintTool tool = new ReportPrintTool(report);
                                            //tool.ShowPreview();
                                            //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                            //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                            //tool.Print(); //imprime de golpe
                                        }



                                        contador = 0;
                                        dgvEscan.Rows[0].Selected = true;
                                        dgvEscan.FirstDisplayedScrollingRowIndex = 0;
                                        txtUnitsScan.Text = (0).ToString();
                                        txtUnitsRemai.Text = cantidad.ToString();
                                        txtUPCScann.Text = string.Empty;
                                        txtUPCScann.Focus();
                            */
                        }

                    }

                    else
                    {
                        sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                        sonido.controls.play();
                        MessageBox.Show("Favor de escanear");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbProductCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!iniciando && cmbPO.SelectedIndex > -1 && cmbPOItem.SelectedIndex > -1 && cmbProductCode.SelectedIndex > -1)
                {
                    iniciando = true;
                    f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                    cmbSizes.SelectedIndex = -1;
                    cmbSizes.Focus();
                    cmbSizes.SelectedIndex = 0;
                    iniciando = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void cmbSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!iniciando && cmbPO.SelectedIndex > -1 && cmbPOItem.SelectedIndex > -1 && cmbProductCode.SelectedIndex > -1 && cmbSizes.SelectedIndex > -1)
                {
                    string[] separadas;

                    separadas = cmbSizes.Text.Split('x');

                    List<ConsultaProductosNuevoResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());

                    if (cmbPOItem.Text == "99")/* PREPACK */
                    {
                        dgvEscan.DataSource = x;
                        foreach (ConsultaProductosNuevoResult a in x)
                        {
                            contador = 0;
                            txtCartonNumber.Text = a.CartonNumber.ToString();
                            txtCartonSize.Text = "";
                            txtSize.Text = "";
                            txtProductCode.Text = a.ProductCode.ToString();
                            id = a.id;
                            anterior.Add(id);
                            upc = cmbPOItem.Text;
                        }
                        cantidad = x.Count.ToString();
                        txtUnitsRemai.Text = cantidad.ToString();
                        txtUnitsReq.Text = cantidad.ToString();
                        txtUnitsScan.Text = "0";
                        txtCartonRq.Text = "1";
                        txtCartonsPacked.Text = "0";
                        txtCartonsReamaining.Text = "1";
                        txtUPCScann.Focus();
                    }
                    else if (cmbPOItem.Text == "88")/*TARGET*/
                    {
                        List<ConsultaProductosNuevoResult> x1 = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), "");

                        if (x1.Count > 0)
                        {
                            contador = 0;
                            dgvEscan.DataSource = x1;
                            txtCartonNumber.Text = x1[0].CartonNumber.ToString();
                            txtCartonSize.Text = x1[0].Size.ToString();
                            dgvEscan.Columns["Cantidad"].Visible = false;
                            dgvEscan.Columns["ProductCode"].Visible = false;
                            dgvEscan.Columns["ProductCode1"].Visible = false;
                            dgvEscan.Columns["id"].Visible = false;
                            txtSize.Text = x1[0].Size.ToString();
                            txtProductCode.Text = x1[0].ProductCode.ToString();
                            id = x1[0].id;
                            anterior.Add(id);
                            upc = x1[0].UPC.ToString();
                            vendor = x1[0].vendor.ToString();
                            ///po = Convert.ToInt32(cmbPO.Text);
                            cantidad = x1[0].cantidad;
                            txtUnitsReq.Text = cantidad.ToString();
                            txtUnitsScan.Text = "0";
                            txtUnitsRemai.Text = cantidad.ToString();
                            txtCartonRq.Text = "1";
                            txtCartonsPacked.Text = "0";
                            txtCartonsReamaining.Text = "1";
                            txtUPCScann.Focus();
                        }
                    }
                    else
                    {
                        if (x.Count > 0)
                        {
                            contador = 0;
                            dgvEscan.DataSource = x;
                            txtCartonNumber.Text = x[0].CartonNumber.ToString();
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
                            txtCartonRq.Text = "1";
                            txtCartonsPacked.Text = "0";
                            txtCartonsReamaining.Text = "1";
                            txtUPCScann.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void LimpiarPantallaEscaneo()
        {
            contador = 0;
            txtUPCScann.Text = string.Empty;
            txtUnitsReq.Text = cantidad.ToString();
            txtUnitsScan.Text = "0";
            txtUnitsRemai.Text = cantidad.ToString();
            dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
            dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
            txtCartonRq.Text = "1";
            txtCartonsPacked.Text = "0";
            txtCartonsReamaining.Text = cantidad.ToString();
            txtUPCScann.Focus();
        }
        public EtiquetaCajaModificada RellenaObjetoClase(List<ConsultaEtiquetaResult> consulta)
        {
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
            clase.id = consulta[0].id;
            clase.po = consulta[0].po;
            clase.poInCompleto = consulta[0].poInCompleto;
            clase.poItem = consulta[0].poItem;
            clase.ProductCode = consulta[0].ProductCode;
            clase.Size = consulta[0].Size;
            clase.size_derecho = consulta[0].size_derecho;
            clase.size_izquierdo = consulta[0].size_izquierdo;
            clase.TipoCarton = consulta[0].TipoCarton;
            clase.upc = consulta[0].upc;
            clase.Fecha = DateTime.Now;
            clase.CartonLeft = consulta[0].CartonLeft;
            clase.CartonRight = consulta[0].CartonRight;
            clase.Cantidad = consulta[0].Cantidad;
            clase.Carton = consulta[0].Carton;
            clase.usuario = usu[0].nombre;
            clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
            clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
            clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
            clase.cliente = cmbCliente.Text;
            clase.factura = cmbFactura.Text;
            clase.terminado = cmbTerminado.Text;
            return clase;
        }

        private void txtUPCScann_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    DataGridViewRow newDatarow = dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))];
                    upc = newDatarow.Cells[4].Value.ToString();
                    if (upc == txtUPCScann.Text)
                    {
                        if (cmbPOItem.Text == "99")/*PREPACK*/
                        {
                            /*UPC CORRECTO*/
                            if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) - 1)
                            {
                                /*TOTAL DE PRENDAS ESCANEADAS*/
                                contador = 0;
                                txtUPCScann.Text = string.Empty;
                                txtUnitsReq.Text = txtUnitsReq.Text;
                                txtUnitsScan.Text = "0";
                                txtUnitsRemai.Text = txtUnitsReq.Text;
                                dgvEscan.Rows[0].Selected = true;
                                dgvEscan.FirstDisplayedScrollingRowIndex = (0);
                                txtUnitsScan.Text = (0).ToString();
                                txtCartonRq.Text = "1";
                                txtCartonsPacked.Text = "0";
                                txtCartonsReamaining.Text = "1";
                                txtUPCScann.Focus();
                                string[] separadas;

                                separadas = cmbSizes.Text.Split('x');

                                List<ConsultaProductosNuevoResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());

                                int prodNuevosCont = 0;
                                int NumeroCarton = 0;
                                //dgvEscan.DataSource = x;
                                foreach (ConsultaProductosNuevoResult a in x)
                                {
                                    prodNuevosCont = prodNuevosCont + 1;
                                    contador = 0;
                                    txtCartonNumber.Text = a.CartonNumber.ToString();
                                    txtCartonSize.Text = "";
                                    txtSize.Text = "";
                                    txtProductCode.Text = a.ProductCode.ToString();

                                    anterior.Add(id);
                                    upc = cmbPOItem.Text;

                                    EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
                                    clase.id = a.id;
                                    clase.po = Convert.ToDecimal(cmbPO.Text);
                                    clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                                    clase.poItem = "99";
                                    clase.ProductCode = a.ProductCode;
                                    clase.Size = a.Size;
                                    separadas = a.Size.Split('x');

                                    clase.size_derecho = separadas[1].ToString();
                                    clase.size_izquierdo = separadas[0].ToString();
                                    clase.TipoCarton = "0";
                                    clase.upc = a.UPC;
                                    clase.Fecha = DateTime.Now;
                                    clase.CartonLeft = "";
                                    clase.CartonRight = "";
                                    clase.Cantidad = Convert.ToDecimal(a.cantidad);
                                    if (prodNuevosCont == 1)
                                    {
                                        NumeroCarton = Convert.ToInt32(a.CartonNumber + '0' + Convert.ToInt32(a.id));
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
                                    clase.id_Inventario = f.GuardaInventario(clase, this.usu[0].id);
                                }

                            }
                            else
                            {
                                /*ESCANEANDO X PRENDA AUN*/
                                contador = contador + 1;
                                dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text) + 1)].Selected = true;
                                dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text) + 1);
                                txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                                txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                                txtUPCScann.Text = string.Empty;
                                txtUPCScann.Focus();
                                sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";
                            }


                        }
                        if (upc == txtUPCScann.Text)
                        {
                            /*ESCANEANDO X PRENDA AUN*/
                            contador = contador + 1;
                            dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
                            dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
                            txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                            txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                            txtUPCScann.Text = string.Empty;
                            txtUPCScann.Focus();

                            sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";

                            if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text))
                            {

                                List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);

                                EtiquetaCajaModificada clase = RellenaObjetoClase(consulta);

                                List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                                if (clase.poItem == "1000") /*CINTAS*/
                                {
                                    do
                                    {
                                        try
                                        {
                                            clase.Carton = Convert.ToInt64(Interaction.InputBox("Captura Numero de Carton", "Carton", "", 5, 5));

                                        }
                                        catch (Exception ex)
                                        { clase.Carton = 0; MessageBox.Show("Favor de ingresar correctamente el numero de carton ya que no se ha guardado"); };

                                    } while (clase.Carton == 0 || clase.Carton.ToString().Length > 10);

                                }
                                if ((clase.poItem != "1000" && clase.poItem != "99") || (clase.poItem == "1000" && clase.Carton != 0 && clase.poItem != "99"))
                                {

                                    clase.assembly = consulta[0].Assembly;
                                    clase.Vendor = consulta[0].Vendor;
                                    clase.ShipTo = consulta[0].ShipTo;
                                    clase.id_Inventario = f.GuardaInventario(clase, this.usu[0].id);

                                    if (clase.poItem != "1000" && clase.poItem != "99" && clase.poItem != "88") /*INVENTARIO VIEJO*/
                                    {
                                        //clase.id_Inventario = 116;
                                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_Inventario +
                                                                                          "&po=" + consulta[0].po +
                                                                                          "&cl=" + cmbCliente.Text +
                                                                                          "&fa=" + cmbFactura.Text +
                                                                                          "&te=" + cmbTerminado.Text +
                                                                                          "&u=" + clase.usuario +
                                                                                          "&pc=" + consulta[0].ProductCode +
                                                                                          "&c=" + Convert.ToInt32(txtUnitsScan.Text) +
                                                                                          "&sz=" + consulta[0].Size +
                                                                                          "&fe=" + DateTime.Now,
                                                                                          QRCodeGenerator.ECCLevel.Q);

                                        cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                        QRCode qrCode = new QRCode(qrCodeData);
                                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                                        Codigo.IncludeLabel = true;
                                        Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 200, 100);
                                        clase.qr = qrCode.GetGraphic(20);
                                        clase.codigoBarras = codigoBarras;
                                        listClase.Add(clase);
                                        id_InventarioAnt = clase.id_Inventario;

                                        ReporteCaja report = new ReporteCaja();
                                        report.DataSource = listClase;
                                        // Disable margins warning. 
                                        report.PrintingSystem.ShowMarginsWarning = false;
                                        ReportPrintTool tool = new ReportPrintTool(report);
                                        //tool.ShowPreview();
                                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                        //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                        tool.Print(); //imprime de golpe
                                        LimpiarPantallaEscaneo();
                                    }
                                    else if (clase.poItem != "1000" && clase.poItem != "99" && clase.poItem == "88") 
                                        /*TARGETS -------------------NUEVO*/
                                    {
                                        //clase.id_Inventario = 116;

                                        cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);

                                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                                        Codigo.IncludeLabel = true;
                                        Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.EAN13, clase.id_Inventario.ToString(), Color.Black, Color.White, 200, 100);

                                        clase.codigoBarras = codigoBarras;
                                        listClase.Add(clase);
                                        id_InventarioAnt = clase.id_Inventario;

                                        ReportCajaTarget report = new ReportCajaTarget();
                                        report.DataSource = listClase;
                                        // Disable margins warning. 
                                        report.PrintingSystem.ShowMarginsWarning = false;
                                        ReportPrintTool tool = new ReportPrintTool(report);
                                        //tool.ShowPreview();
                                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                        //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                        tool.Print(); //imprime de golpe
                                        LimpiarPantallaEscaneo();
                                    }
                                    else if (clase.poItem == "1000") /*CINTAS*/
                                    {
                                        if (consulta[0].upc.Length > 14)
                                        {
                                            clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 9);
                                            clase.numeroEtiqueta2 = consulta[0].upc.Substring(9, 7);
                                            clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 5) + " - " + consulta[0].upc.Substring(6, 2);
                                            clase.upc = "0" + consulta[0].upc.Substring(0, 8).ToString() + clase.numeroEtiqueta2;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 7);
                                                clase.numeroEtiqueta2 = consulta[0].upc.Substring(7, 7);//aquiii
                                                clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 4) + " - " + consulta[0].upc.Substring(4, 2);
                                                clase.upc = "0" + consulta[0].upc.Substring(0, 6).ToString() + clase.numeroEtiqueta2;

                                            }
                                            catch (Exception ex)
                                            {
                                                clase.upc = consulta[0].upc;
                                            }
                                        }
                                        cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                                        Codigo.IncludeLabel = true;
                                        Codigo.RotateFlipType = RotateFlipType.Rotate90FlipY;
                                        Image codigoBarras =
                                        Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 270, 180);
                                        // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                                        clase.codigoBarras = codigoBarras;
                                        listClase.Add(clase);
                                        id_InventarioAnt = clase.id_Inventario;

                                        ReporteCintas report = new ReporteCintas();
                                        report.DataSource = listClase;
                                        // Disable margins warning. 
                                        report.PrintingSystem.ShowMarginsWarning = false;
                                        ReportPrintTool tool = new ReportPrintTool(report);
                                        LimpiarPantallaEscaneo();

                                    }
                                }

                            }
                        }
                        else
                        if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) && txtUPCScann.Text.ToUpper() == upc)
                        {

                            contador = 0;
                            txtUPCScann.Text = string.Empty;
                            txtUnitsReq.Text = cantidad.ToString();
                            txtUnitsScan.Text = "0";
                            txtUnitsRemai.Text = cantidad.ToString();
                            dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
                            dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
                            txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                            txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                            txtCartonRq.Text = "1";
                            txtCartonsPacked.Text = "0";
                            txtCartonsReamaining.Text = "1";
                            txtUPCScann.Focus();
                            LimpiarPantallaEscaneo();
                        }
                    } 
                    else
                    {

                        sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                        sonido.controls.play();
                        txtUPCScann.Text = string.Empty;
                        MessageBox.Show("Favor de Escanear la prenda correcta!");

                    }
                }


                ///aqui

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TabEsaneo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.TabEsaneo.SelectedTab.Text.Trim() == "BajaPO")
                {
                  /*  if (usu[0].perfil == "1" || usu[0].perfil == "4")
                    {*/
                        cmbPOB.Enabled = true;
                        cmbClienteB.Enabled = true;
                        cmbFacturacionB.Enabled = true;
                        cmbTerminadoB.Enabled = true;
                        btnBajaPO.Enabled = true;
                   /* }*/
                }
                if (this.TabEsaneo.SelectedTab.Text.Trim() == "AltaPrePack")
                {
                    if (Tabpage == 0)
                    {
                        Tabpage = Tabpage + 1;
                        tablaPrepack = new DataTable();
                        tablaPrepack.Columns.Add("Talla", typeof(string));
                        tablaPrepack.Columns.Add("Cantidad", typeof(Int64));
                        tablaPrepack.Columns.Add("Codigo UPC", typeof(string));
                        tablaPrepack.Columns.Add("idSize", typeof(int));

                        

                        /*
                         * 
                         *   tablaPrepack.Rows.Add(cmbTallaPrepack.Text, txtCantidadPrepack.Text, txtCodigoupcPrepack.Text, cmbTallaPrepack.SelectedValue.ToString());
                             dgvPrePack2.DataSource = tablaPrepack;
                             dgvPrePack2.Columns["idSize"].Visible = false;
                             cmbTallaPrepack.SelectedIndex = 0;
                             txtCodigoupcPrepack.Text = "";
                         * */
                    }
                    else
                    {

                    }
                }
                if (this.TabEsaneo.SelectedTab.Text.Trim() == "AltaPO")
                {
                    if (Tabpage == 0)
                    {

                        tablaAltaPO = new DataTable();
                        tablaAltaPO.Columns.Add("Talla", typeof(string));
                        tablaAltaPO.Columns.Add("Cantidad", typeof(Int64));
                        tablaAltaPO.Columns.Add("Codigo UPC", typeof(string));
                        tablaAltaPO.Columns.Add("idSize", typeof(int));
                        Tabpage = Tabpage + 1;
                    }
                    else
                    {

                    }
                }

                if (this.TabEsaneo.SelectedTab.Text.Trim() == "Escaneo")
                {

                    //iniciando = true;
                    //f.ConsultaPO(this.cmbPO);
                    //iniciando = false;
                    /*
                    cmbPO.DataSource = null;
                    cmbPO.Items.Clear();

                    int count = cmbPO.Items.Count - 1;
                    for (int i = count; i > 0; i--)
                    {
                        cmbPO.Items.RemoveAt(i);
                    }

                    cmbPO.BeginUpdate();
                    cmbPO.EndUpdate();
                    cmbPO.ResetText();
                    cmbPO.SelectedIndex = -1;
                    //iniciando = true;
                    */

                }

                if (this.TabEsaneo.SelectedTab.Text.Trim() == "BajaCaja")
                {
                    if (usu[0].perfil == "1" || usu[0].perfil == "4")
                    {
                        txtCaja.Enabled = true;
                        btnBajaCaja.Enabled = true;
                    }
                }
                if (this.TabEsaneo.SelectedTab.Text.Trim() == "ABC")
                {
                    if (usu[0].perfil == "1" || usu[0].perfil == "4")
                    {
                        btnNuevo.Enabled = true;
                        //btnEliminar.Enabled = true;
                        //btnEditar.Enabled = true;
                        //btnGuardarA.Enabled = true;
                        btnVistaPrevia.Enabled = true;
                        //
                        txtCantidadA.Enabled = true;
                        txtPoNA.Enabled = true;
                        txtUPCA.Enabled = true;
                        // txtPoItemNA.Enabled = true;
                        cmbTipoCajaA.Enabled = true;
                        //txtPCA.Enabled = true;
                        cmbSizeA.Enabled = true;
                        txtID.Enabled = false;
                    }
                    else
                    {
                        btnNuevo.Enabled = false;
                        btnEliminar.Enabled = false;
                        btnEditar.Enabled = false;
                        btnGuardarA.Enabled = false;
                        btnVistaPrevia.Enabled = false;
                        //
                        txtCantidadA.Enabled = false;
                        txtPoNA.Enabled = false;
                        txtUPCA.Enabled = false;
                        //txtPoItemNA.Enabled = false;
                        cmbTipoCajaA.Enabled = false;
                        //txtPCA.Enabled = false;
                        cmbSizeA.Enabled = false;
                        txtID.Enabled = false;

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnPrintLast_Click(object sender, EventArgs e)
        {
            try
            {
                //id_InventarioAnt = 102;
                //txtUnitsScan.Text = "5";
                if (cmbPOItem.Text != "99")
                {
                    if (id_InventarioAnt != 0 && Convert.ToInt32(txtUnitsScan.Text) > 0)
                    {
                        txtUPCScann.Text = string.Empty;
                        txtUPCScann.Focus();
                        List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(id_InventarioAnt);
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();

                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                        clase.po = consulta[0].po;
                        clase.poInCompleto = consulta[0].poInCompleto;
                        clase.poItem = consulta[0].poItem;
                        clase.ProductCode = consulta[0].ProductCode;
                        clase.Size = consulta[0].Size;
                        clase.size_derecho = consulta[0].size_derecho;
                        clase.size_izquierdo = consulta[0].size_izquierdo;
                        clase.TipoCarton = consulta[0].TipoCarton;
                        clase.upc = consulta[0].upc;
                        clase.Fecha = consulta[0].create_dtm;
                        clase.CartonLeft = consulta[0].CartonLeft;
                        clase.CartonRight = consulta[0].CartonRight;
                        clase.Cantidad = consulta[0].Cantidad;
                        clase.Carton = consulta[0].Carton;
                        clase.usuario = consulta[0].usuario;
                        clase.id_Inventario = consulta[0].id;
                        clase.id_cliente = Convert.ToInt32(consulta[0].id_cliente);
                        clase.id_factura = Convert.ToInt32(consulta[0].id_factura);
                        clase.id_terminado = Convert.ToInt32(consulta[0].id_terminado);
                        clase.cliente = consulta[0].cliente;
                        clase.factura = consulta[0].factura;
                        clase.terminado = consulta[0].terminado;
                        if (clase.poItem == "1000")
                        {
                            if (consulta[0].upc.Length > 14)
                            {

                                clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 9);
                                clase.numeroEtiqueta2 = consulta[0].upc.Substring(9, 7);
                                clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 5) + " - " + consulta[0].upc.Substring(6, 2);
                                clase.upc = "0" + consulta[0].upc.Substring(0, 8).ToString() + clase.numeroEtiqueta2;
                            }
                            else
                            {


                                clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 7);
                                clase.numeroEtiqueta2 = consulta[0].upc.Substring(7, 7);
                                clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 4) + " - " + consulta[0].upc.Substring(4, 2);
                                clase.upc = "0" + consulta[0].upc.Substring(0, 6).ToString() + clase.numeroEtiqueta2;
                            }

                            cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                            Codigo.IncludeLabel = true;
                            Codigo.RotateFlipType = RotateFlipType.Rotate90FlipY;
                            clase.assembly = consulta[0].Assembly;
                            clase.Vendor = consulta[0].Vendor;
                            clase.ShipTo = consulta[0].ShipTo;
                            Image codigoBarras =
                            Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.Carton.ToString(), Color.Black, Color.White, 270, 180);
                            // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                            clase.codigoBarras = codigoBarras;
                            listClase.Add(clase);
                            id_InventarioAnt = clase.id_Inventario;

                            ReporteCintas report = new ReporteCintas();
                            report.DataSource = listClase;
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            //tool.ShowPreview();
                            //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                            //tool.PrintDialog(); //muestra a que impresora se va a mandar
                            //tool.Print(); //imprime de golpe

                        }
                        else if (clase.poItem != "1000" && clase.poItem != "99")
                        {
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_Inventario +
                                                                              "&po=" + consulta[0].po +
                                                                              "&cl=" + clase.cliente +
                                                                              "&fa=" + clase.factura +
                                                                              "&te=" + clase.terminado +
                                                                              "&u=" + clase.usuario +
                                                                              "&pc=" + consulta[0].ProductCode +
                                                                              "&c=" + clase.Cantidad +
                                                                              "&sz=" + consulta[0].Size +
                                                                              "&fe=" + clase.Fecha, QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                            Codigo.IncludeLabel = true;
                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, consulta[0].id.ToString(), Color.Black, Color.White, 200, 100);
                            clase.qr = qrCode.GetGraphic(20);
                            clase.codigoBarras = codigoBarras;
                            listClase.Add(clase);
                            ReporteCaja report = new ReporteCaja();
                            report.DataSource = listClase;
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            //tool.ShowPreview();
                            //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                            //tool.PrintDialog(); //muestra a que impresora se va a mandar
                            tool.Print(); //imprime de golpe

                        }
                    }
                    else
                    {
                        MessageBox.Show("Favor de Escanear");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Alta_Click(object sender, EventArgs e)
        {

        }
        #region Comentario
        /*Aqui empieza

        
        public void limpiaTodo()
        {
            try
            {
                txtNombre.Text = "";
                txtDireccion.Text = "";
                txtBuscar.Text = "";
                txtNumeroEstacion.Text = "";
                chbEstatusEstacion.Checked = false;
                dgvResultado.DataSource = null;
                imprimirObjeto = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void limpiaDatos()
        {
            try
            {
                txtNombre.Text = "";
                txtDireccion.Text = "";
                txtNumeroEstacion.Text = "";
                chbEstatusEstacion.Checked = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void activarDatos()
        {
            try
            {
                txtNombre.ReadOnly = false;
                txtDireccion.ReadOnly = false;
                chbEstatusEstacion.Enabled = true;

                txtNombre.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void desactivarDatos()
        {
            try
            {
                txtNombre.ReadOnly = true;
                txtDireccion.ReadOnly = true;
                chbEstatusEstacion.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool ValidateForm()
        {
            bool respuesta = false;

            try
            {
                var textBoxes = dgvAlta.Controls.Cast<Control>()
                                         .OfType<TextBox>()
                                         .OrderBy(control => control.TabIndex);

                foreach (var textBox in textBoxes)
                {

                    var fieldName4 = textBox.Name;

                    if (fieldName4 == "txtNumeroComodato"
                        || fieldName4 == "txtBuscar"
                        || fieldName4 == "txtObservaciones"
                        || fieldName4 == "ttxCalle"
                        || fieldName4 == "txtCalle"
                         || fieldName4 == "ttxColonia"
                        || fieldName4 == "txtColonia"
                        || fieldName4 == "txtCiudad"
                        || fieldName4 == "txtNoInterior"
                        || fieldName4 == "ttxNoInterior"
                        || fieldName4 == "txtNoExterior"
                           || fieldName4 == "ttxNoExterior"
                            || fieldName4 == "ttxCiudad"
                             || fieldName4 == "cbmEstado"
                             || fieldName4 == "cmbTipoTanque"
                             || fieldName4 == "cmbMarca"
                             || fieldName4 == "txtNoSerie"
                             || fieldName4 == "txtCapacidad"
                        ) { }

                    else if (string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        textBox.Focus();
                        var fieldName = textBox.Name.Substring(3);
                        MessageBox.Show((string.Format("Campo '{0}' no debe estar vacio.", fieldName)));
                        return false;
                    }
                }
                respuesta = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                respuesta = false;
            }

            return respuesta;
        }

        public void buscarDatosBancos()
        {
            try
            {
                panelEstadoActual.Text = "Buscando Registro ...";
                // btnGuardar.Enabled = false;

                //Parametro de búsqueda del StoredProcedure
                string parametroBusqueda = txtBuscar.Text;
                
                    //Llamada al metodo ActualizaTabla
                    funciones.ConsultaTabla(dgvResultado, "spSIAF_mtto_ConsultarEstacion", "Descripcion", parametroBusqueda, empresa.empresa_idEmpresa);
                    if (dgvResultado.RowCount > 0)
                    {

                        panelEstadoActual.Text = "Registro (s) encontrado (s) ...";
                        limpiaDatos();
                    }
                    else
                    {
                        panelEstadoActual.Text = "No se encontró ningún registro ...";
                        limpiaDatos();
                    }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (permisos.permisoModulo_consulta)
                {
                    buscarDatosBancos();
                }
                else
                {
                    panelEstadoActual.Text = "No tienes los permisos suficientes para realizar esta acción ...";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Editar();
        }

        public void mostrarDatos()
        {
            try
            {
                limpiaDatos();
                if (imprimirObjeto) { }
                else
                {
                    Modifico = false;
                    txtNumeroEstacion.Text = dgvResultado.CurrentRow.Cells["Numero"].Value.ToString();
                    txtNombre.Text = dgvResultado.CurrentRow.Cells["Nombre"].Value.ToString();
                    txtDireccion.Text = dgvResultado.CurrentRow.Cells["Direccion"].Value.ToString();
                    chbEstatusEstacion.Checked = Convert.ToBoolean(dgvResultado.CurrentRow.Cells["Estatus"].Value);
                    desactivarDatos();
                    panelEstadoActual.Text = "Vizualizando Registro ...";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void dgvEstaciones_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            mostrarDatos();
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (permisos.permisoModulo_consulta)
                {
                    if ((int)e.KeyChar == (int)Keys.Enter)
                    {
                        buscarDatosBancos();
                    }
                }
                else
                {
                    panelEstadoActual.Text = "No tienes los permisos suficientes para realizar esta acción ...";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtDireccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((int)e.KeyChar == (int)Keys.Enter)
            {
                Guardar();
            }
        }

        public void Nuevo()
        {
            try
            {
                if (permisos.permisoModulo_alta)
                {
                    limpiaTodo();
                    panelEstadoActual.Text = "Agregando Registro ...";
                    chbEstatusEstacion.Checked = true;
                    activarDatos();
                    Modifico = true;
                    quiereCerrar = false;


                }

                else
                {
                    panelEstadoActual.Text = "No tienes los permisos suficientes para realizar esta acción ...";
                    //txtBuscar.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void Editar()
        {
            try
            {
                if (permisos.permisoModulo_cambio)
                {

                    if (txtNumeroEstacion.Text != "")
                    {
                        txtNombre.Focus();
                        panelEstadoActual.Text = "Editando Registro ...";
                        activarDatos();
                        Modifico = true;
                        quiereCerrar = false;
                    }
                    else
                    {
                        txtBuscar.Focus();
                        panelEstadoActual.Text = "Necesita seleccionar un registro para realizar esta acción ...";
                    }


                }
                else
                {
                    panelEstadoActual.Text = "No tienes los permisos suficientes para realizar esta acción ...";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Guardar()
        {
            try
            {
                if (txtNombre.ReadOnly != true)
                {
                    if (ValidateForm())
                    {
                        estacion estacion = new estacion();



                        estacion.estacion_idEstacion = txtNumeroEstacion.Text == "" ? estacion.estacion_idEstacion = 0 : Convert.ToInt32(txtNumeroEstacion.Text);
                        if (chbEstatusEstacion.Checked == true) { estacion.estacion_estatus = true; } else { estacion.estacion_estatus = false; }
                        estacion.estacion_nombre = txtNombre.Text;
                        estacion.estacion_direccion = txtDireccion.Text;
                        estacion.estacion_idEmpresa = empresa.empresa_idEmpresa;

                        if (estacion.estacion_idEstacion == 0 && permisos.permisoModulo_alta)
                        {
                            bool respuesta = funciones.mttoAgregarEstacion(estacion);

                            if (respuesta)
                            {
                                panelEstadoActual.Text = "Registro Agregado Correctamente ...";
                                string parametroBusqueda = txtNombre.Text;
                                funciones.ConsultaTabla(dgvResultado, "spSIAF_mtto_ConsultarEstacion", "Descripcion", parametroBusqueda, empresa.empresa_idEmpresa);
                                limpiaDatos();
                                txtNombre.Focus();
                                Modifico = false;
                                if (quiereCerrar)
                                {
                                    this.Dispose();
                                }


                            }
                        }
                        else if (estacion.estacion_idEstacion != 0 && permisos.permisoModulo_cambio)
                        {


                            bool respuesta = funciones.mttoActualizarEstacion(estacion);

                            if (respuesta)
                            {
                               // panelEstadoActual.Text = "Registro Actualizado Correctamente ...";
                                string parametroBusqueda = txtNombre.Text;
                                funciones.ConsultaTabla(dgvResultado, "spSIAF_mtto_ConsultarEstacion", "Descripcion", parametroBusqueda, empresa.empresa_idEmpresa);
                                limpiaDatos();
                                desactivarDatos();
                                Modifico = false;

                            }

                        }
                        else
                        {
                        }

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


        private void NuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Nuevo();
        }

        private void EditarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Editar();
        }

        private void GuardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Guardar();
        }
        private void dgvResultado_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAlta.Focused)
                mostrarDatos();
        }
*/
        #endregion

        private void btnCancelCarton_Click(object sender, EventArgs e)
        {
            if (dgvEscan.RowCount > 0)
            {
                contador = 0;
                dgvEscan.Rows[0].Selected = true;
                dgvEscan.FirstDisplayedScrollingRowIndex = 0;
                txtUnitsScan.Text = (0).ToString();
                txtUnitsRemai.Text = cantidad.ToString();
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Buscar();
            Cursor.Current = Cursors.Default;
        }

        public void Buscar()
        {
            try
            {
                List<ConsultaInventarioResult> inv = f.ConsultaInventario(dtpFechaInicio.Value.Date, dtpFechaFinal.Value.Date);

                gcReporte.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowGridPreview(DevExpress.XtraGrid.GridControl grid)
        {
            // Check whether or not the Grid Control can be printed. 
            if (!grid.IsPrintingAvailable)
            {
                MessageBox.Show("The 'DevExpress.XtraPrinting' Library is not found", "Error");
                return;
            }
            // Opens the Preview window. 

            PrintableComponentLink link = new PrintableComponentLink(new PrintingSystem());

            link.Component = grid;

            link.Landscape = true;

            link.ShowPreview();

        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            ShowGridPreview(gcReporte);
        }

        private void btnImprimirRe_Click(object sender, EventArgs e)
        {
            try
            {
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                int idInv = 0;
                try { idInv = Convert.ToInt32(txtIDReImpresion.Text); } catch (Exception ex) { idInv = 0; }

                List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(idInv);
                if (consulta.Count > 0)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();

                    List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                    EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                    clase.poInCompleto = consulta[0].poInCompleto;
                    clase.po = consulta[0].po;
                    clase.poItem = consulta[0].poItem;
                    clase.ProductCode = consulta[0].ProductCode;
                    clase.Size = consulta[0].Size;
                    clase.size_derecho = consulta[0].size_derecho;
                    clase.size_izquierdo = consulta[0].size_izquierdo;
                    clase.TipoCarton = consulta[0].TipoCarton;
                    clase.upc = consulta[0].upc;
                    clase.Fecha = consulta[0].create_dtm;
                    clase.CartonLeft = consulta[0].CartonLeft;
                    clase.CartonRight = consulta[0].CartonRight;
                    clase.Cantidad = consulta[0].Cantidad;
                    clase.Carton = consulta[0].Carton;
                    clase.usuario = consulta[0].usuario;
                    clase.id_Inventario = consulta[0].id;
                    clase.id_cliente = Convert.ToInt32(consulta[0].id_cliente);
                    clase.id_factura = Convert.ToInt32(consulta[0].id_factura);
                    clase.id_terminado = Convert.ToInt32(consulta[0].id_terminado);
                    clase.cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente;
                    clase.factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura;
                    clase.terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado;


                    if (clase.poItem != "1000" && clase.poItem != "99")
                    {

                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_Inventario +
                                                                      "&po=" + clase.po +
                                                                      "&cl=" + clase.cliente +
                                                                      "&fa=" + clase.factura +
                                                                      "&te=" + clase.terminado +
                                                                      "&u=" + clase.usuario +
                                                                      "&pc=" + clase.ProductCode +
                                                                      "&c=" + clase.Cantidad +
                                                                      "&sz=" + clase.Size +
                                                                      "&fe=" + clase.Fecha, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                        Codigo.IncludeLabel = true;
                        Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                           , consulta[0].id.ToString()
                                                           , Color.Black
                                                           , Color.White, 200, 100);

                        clase.qr = qrCode.GetGraphic(20);
                        clase.codigoBarras = codigoBarras;


                        listClase.Add(clase);
                        ReporteCaja report = new ReporteCaja();
                        report.DataSource = listClase;
                        // Disable margins warning. 
                        report.PrintingSystem.ShowMarginsWarning = false;
                        ReportPrintTool tool = new ReportPrintTool(report);
                        //tool.ShowPreview();
                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                        //tool.PrintDialog(); //muestra a que impresora se va a mandar
                        tool.Print(); //imprime de golpe
                    }
                    else if (clase.poItem == "1000")
                    {
                        if (consulta[0].upc.Length > 14)
                        {
                            clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 9);
                            clase.numeroEtiqueta2 = consulta[0].upc.Substring(9, 7);
                            clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 5) + " - " + consulta[0].upc.Substring(6, 2);
                            clase.upc = "0" + consulta[0].upc.Substring(0, 8).ToString() + clase.numeroEtiqueta2;
                        }
                        else
                        {
                            clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 7);
                            clase.numeroEtiqueta2 = consulta[0].upc.Substring(7, 7);
                            clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 4) + " - " + consulta[0].upc.Substring(4, 2);
                            clase.upc = "0" + consulta[0].upc.Substring(0, 6).ToString() + clase.numeroEtiqueta2;
                        }

                        clase.assembly = consulta[0].Assembly;
                        clase.Vendor = consulta[0].Vendor;
                        clase.ShipTo = consulta[0].ShipTo;
                        // cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                        Codigo.IncludeLabel = true;
                        Codigo.RotateFlipType = RotateFlipType.Rotate90FlipY;
                        Image codigoBarras =
                        Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.Carton.ToString(), Color.Black, Color.White, 270, 180);
                        // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                        clase.codigoBarras = codigoBarras;
                        listClase.Add(clase);
                        id_InventarioAnt = clase.id_Inventario;

                        ReporteCintas report = new ReporteCintas();
                        report.DataSource = listClase;
                        // Disable margins warning. 
                        report.PrintingSystem.ShowMarginsWarning = false;
                        ReportPrintTool tool = new ReportPrintTool(report);
                        //tool.ShowPreview();
                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                        //tool.PrintDialog(); //muestra a que impresora se va a mandar
                        tool.Print(); //imprime de golpe
                    }
                    if (cbLimpiar.Checked == true)
                    {
                        txtIDReImpresion.Text = string.Empty;
                        txtIDReImpresion.Focus();
                    }
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

        private void btnBajaPO_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja el PO " + cmbPOB.Text + " " + cmbClienteB.Text + " " + cmbFacturacionB.Text + " " + cmbTerminadoB.Text, "Baja PO", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    bool baja = f.BajaPO(cmbPOB.Text, Convert.ToInt32(cmbClienteB.SelectedValue), Convert.ToInt32(cmbFacturacionB.SelectedValue), Convert.ToInt32(cmbTerminadoB.SelectedValue), usu[0].id);
                    if (baja)
                    {
                        MessageBox.Show("se elimino correctamente.");
                    }
                    else
                    {
                        MessageBox.Show("el po no existe en la base de datos.");
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
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

            clase.po = Convert.ToInt32(txtPoNA.Text);
            //clase.poItem = txtPoItemNA.Text;
            clase.Cantidad = Convert.ToInt32(txtCantidadA.Text);
            clase.Size = cmbSizeA.SelectedValue.ToString();
            clase.upc = txtUPCA.Text;
            clase.ProductCode = txtProductCode.Text;
            clase.TipoCarton = cmbTipoCajaA.Text;
            clase.Fecha = DateTime.Now;
            //clase.Carton = Convert.ToInt32(txtNumCajaA.Text);
            clase.usuario = usu[0].nombre;
            if (agregar)
            {
                clase.id_Inventario = f.GuardaInventario(clase, this.usu[0].id);


            }
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

        private void btnVistaPrevia_Click(object sender, EventArgs e)
        {
            BuscarPO();
        }


        public void BuscarPO()
        {
            try
            {
                /*
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
            txtPoNA.ReadOnly = false;
            txtUPCA.ReadOnly = false;
            txtCantidadA.ReadOnly = false;
            cmbTipoCajaA.Enabled = true;
            cmbSizeA.Enabled = true;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
        }

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
                List<clsParametro> parametro = new List<clsParametro>();
                parametro.Add(new clsParametro("@PO", cmbPoEntradaAlmacen.Text));
                parametro.Add(new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@POSolamente", cbPOSolamente.Checked));

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
                List<clsParametro> parametro = new List<clsParametro>();
                parametro.Add(new clsParametro("@PO", cmbPoEntradaAlmacen.Text));
                parametro.Add(new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@POSolamente", cbPOSolamente.Checked));
                dgvAlmacen.DataSource = f.ConsultaTablaGeneral("ubicacion_Entrada_ConsultaCajas", parametro);
                dgvAlmacen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void cmbFacturacionEntradaAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                List<clsParametro> parametro = new List<clsParametro>();
                parametro.Add(new clsParametro("@PO", cmbPoEntradaAlmacen.Text));
                parametro.Add(new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@POSolamente", cbPOSolamente.Checked));
                dgvAlmacen.DataSource = f.ConsultaTablaGeneral("ubicacion_Entrada_ConsultaCajas", parametro);
                dgvAlmacen.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            }
        }

        private void cmbTerminadoEntradaAlmacen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                List<clsParametro> parametro = new List<clsParametro>();
                parametro.Add(new clsParametro("@PO", cmbPoEntradaAlmacen.Text));
                parametro.Add(new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()));
                parametro.Add(new clsParametro("@POSolamente", cbPOSolamente.Checked));
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
                if ((int)e.KeyChar == (int)Keys.Enter)
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
                                            List<clsParametro> parametro = new List<clsParametro>();
                                            parametro.Add(new clsParametro("@PO", cmbPoEntradaAlmacen.Text));
                                            parametro.Add(new clsParametro("@Cliente", cmbClienteEntradaAlmacen.Text == "NA" ? "1" : cmbClienteEntradaAlmacen.SelectedValue.ToString()));
                                            parametro.Add(new clsParametro("@Factura", cmbFacturacionEntradaAlmacen.Text == "NA" ? "1" : cmbFacturacionEntradaAlmacen.SelectedValue.ToString()));
                                            parametro.Add(new clsParametro("@Terminado", cmbTerminadoEntradaAlmacen.Text == "NA" ? "1" : cmbTerminadoEntradaAlmacen.SelectedValue.ToString()));

                                            parametro.Add(new clsParametro("@POSolamente", cbPOSolamente.Checked));
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
                if ((int)e.KeyChar == (int)Keys.Enter)
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
                if ((int)e.KeyChar == (int)Keys.Enter)
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
                    List<clsParametro> parametro = new List<clsParametro>();
                    parametro.Add(new clsParametro("@po", cmbPOSalidaAlmacen.Text));
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
                    List<clsParametro> parametro = new List<clsParametro>();
                    parametro.Add(new clsParametro("@po", cmbPOSalidaAlmacen.Text));
                    List<Object> objChecked = clbT.CheckedItems.Cast<Object>().ToList();

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
                        List<clsParametro> parametro = new List<clsParametro>();
                        parametro.Add(new clsParametro("@po", cmbPOSalidaAlmacen.Text));
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
                if ((int)e.KeyChar == (int)Keys.Enter)
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
                        List<clsParametro> parametro = new List<clsParametro>();
                        parametro.Add(new clsParametro("@po", cmbPOSalidaAlmacen.Text));
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
                try { id = Convert.ToInt32(txtIDReimpresionAlmacen.Text); } catch (Exception ex) { id = 0; }
                List<ubicacion_Entrada_ConsultaUbicacionIDResult> consulta = f.ConsultaUbicacionID(id);

                if (consulta.Count > 0)
                {
                    string nivel = consulta[0].nivel.ToString();
                    BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                    Codigo.IncludeLabel = true;
                    Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                       , consulta[0].id.ToString()
                                                       , Color.Black
                                                       , Color.White
                                                       , 345
                                                       , 60);
                    //370
                    //520
                    List<EtiquetaCajaModificada> lem = new List<EtiquetaCajaModificada>();
                    EtiquetaCajaModificada em = new EtiquetaCajaModificada();
                    em.codigoBarras = codigoBarras;
                    em.nivel = nivel;
                    em.TipoCarton = consulta[0].nombre;
                    em.ProductCode = consulta[0].descripcion;
                    lem.Add(em);

                    ReporteAlmacen report = new ReporteAlmacen();
                    report.DataSource = lem;
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

        private void btnBuscarAlmacen_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                BuscarReporteAlmacen();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BuscarReporteAlmacen()
        {
            try
            {
                List<ubicacion_ReporteAlmacen_ConsultaResult> inv = f.ConsultaAlmacen(dtpFechaInicioAlmacen.Value.Date, dtpFechaFinalAlmacen.Value.Date);
                gcReporteAlmacen.DataSource = inv;

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
                if ((int)e.KeyChar == (int)Keys.Enter)
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

        private void txtIDReImpresion_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    int idInv = 0;
                    try { idInv = Convert.ToInt32(txtIDReImpresion.Text); } catch (Exception ex) { idInv = 0; }

                    List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(idInv);
                    if (consulta.Count > 0)
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();

                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                        clase.poInCompleto = consulta[0].poInCompleto;
                        clase.po = consulta[0].po;
                        clase.poItem = consulta[0].poItem;
                        clase.ProductCode = consulta[0].ProductCode;
                        clase.Size = consulta[0].Size;
                        clase.size_derecho = consulta[0].size_derecho;
                        clase.size_izquierdo = consulta[0].size_izquierdo;
                        clase.TipoCarton = consulta[0].TipoCarton;
                        clase.upc = consulta[0].upc;
                        clase.Fecha = consulta[0].create_dtm;
                        clase.CartonLeft = consulta[0].CartonLeft;
                        clase.CartonRight = consulta[0].CartonRight;
                        clase.Cantidad = consulta[0].Cantidad;
                        clase.Carton = consulta[0].Carton;
                        clase.usuario = consulta[0].usuario;
                        clase.id_Inventario = consulta[0].id;
                        clase.id_cliente = Convert.ToInt32(consulta[0].id_cliente);
                        clase.id_factura = Convert.ToInt32(consulta[0].id_factura);
                        clase.id_terminado = Convert.ToInt32(consulta[0].id_terminado);
                        clase.cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente;
                        clase.factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura;
                        clase.terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado;
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_Inventario +
                                                                          "&po=" + clase.po +
                                                                          "&cl=" + clase.cliente +
                                                                          "&fa=" + clase.factura +
                                                                          "&te=" + clase.terminado +
                                                                          "&u=" + clase.usuario +
                                                                          "&pc=" + clase.ProductCode +
                                                                          "&c=" + clase.Cantidad +
                                                                          "&sz=" + clase.Size +
                                                                          "&fe=" + clase.Fecha, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(qrCodeData);
                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                        Codigo.IncludeLabel = true;
                        Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                           , consulta[0].id.ToString()
                                                           , Color.Black
                                                           , Color.White, 200, 100);

                        clase.qr = qrCode.GetGraphic(20);
                        clase.codigoBarras = codigoBarras;


                        listClase.Add(clase);
                        ReporteCaja report = new ReporteCaja();
                        report.DataSource = listClase;
                        // Disable margins warning. 
                        report.PrintingSystem.ShowMarginsWarning = false;
                        ReportPrintTool tool = new ReportPrintTool(report);
                        //tool.ShowPreview();
                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                        //tool.PrintDialog(); //muestra a que impresora se va a mandar
                        tool.Print(); //imprime de golpe
                        if (cbLimpiar.Checked == true)
                        {
                            txtIDReImpresion.Text = string.Empty;
                            txtIDReImpresion.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Favor de ingresar el numero correctamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBuscarEmbarques_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (!iniciando)
                {
                    ConsultaReporteEmbarques();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ConsultaReporteEmbarques()
        {
            try
            {
                List<ubicacion_ReporteEmbarques_ConsultaResult> inv = f.ConsultaEmbarques(dtpFechaInicioEmbarques.Value.Date, dtpFechaFinalEmbarques.Value.Date);
                dgReporteEmbarques.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCajaIDDividir_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    ConsultaDivision();
                    txtPiezas.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ConsultaDivision()
        {
            try
            {
                if (txtCajaIDDividir.Text != string.Empty)
                {
                    txtRestante.Text = string.Empty; txtPiezas.Text = string.Empty; CantidadDividir = 0;
                    int idCaja = Convert.ToInt32(txtCajaIDDividir.Text);
                    ubicacion_Dividir_ConsultaCajaIDResult r = f.ConsultaCajaID(idCaja);
                    if (r.cantidad > 0)
                    {
                        txtRestante.Text = r.cantidad.ToString();
                        txtPiezas.Text = string.Empty;
                        CantidadDividir = r.cantidad;
                    }
                    else { txtRestante.Text = string.Empty; txtPiezas.Text = string.Empty; CantidadDividir = 0; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnGuardarDivision_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRestante.Text != "0" && txtRestante.Text != "")
                {
                    DialogResult dialogResult = MessageBox.Show("Seguro que deseas Dividir esta caja " + txtCajaIDDividir.Text, "Cerrar", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (txtPiezas.Text != string.Empty && txtRestante.Text != string.Empty && txtCajaIDDividir.Text != string.Empty)
                        {
                            int idCaja = Convert.ToInt32(txtCajaIDDividir.Text);
                            int piezas = Convert.ToInt32(txtPiezas.Text);
                            int restante = Convert.ToInt32(txtRestante.Text);
                            ubicacion_Dividir_CajaIDResult r = f.DividirCaja(idCaja, usu[0].id, piezas, restante);
                            if (r.Caja1 != null && r.Caja2 != null)
                            {
                                ImprimirEtiquetasDivididas(r.Caja1, r.Caja2);
                                txtCajaIDDividir.Text = "";
                                txtPiezas.Text = "";
                                txtRestante.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                            }
                        }
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ImprimirEtiquetasDivididas(int? caja1, int? caja2)
        {
            try
            {
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                int cajaid1 = Convert.ToInt32(caja1);
                int cajaid2 = Convert.ToInt32(caja2);
                /* List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(cajaid1);
                 if (consulta.Count > 0)
                 {
                     QRCodeGenerator qrGenerator = new QRCodeGenerator();

                     List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                     EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                     clase.poInCompleto = consulta[0].poInCompleto;
                     clase.po = consulta[0].po;
                     clase.poItem = consulta[0].poItem;
                     clase.ProductCode = consulta[0].ProductCode;
                     clase.Size = consulta[0].Size;
                     clase.size_derecho = consulta[0].size_derecho;
                     clase.size_izquierdo = consulta[0].size_izquierdo;
                     clase.TipoCarton = consulta[0].TipoCarton;
                     clase.upc = consulta[0].upc;
                     clase.Fecha = consulta[0].create_dtm;
                     clase.CartonLeft = consulta[0].CartonLeft;
                     clase.CartonRight = consulta[0].CartonRight;
                     clase.Cantidad = consulta[0].Cantidad;
                     clase.Carton = consulta[0].Carton;
                     clase.usuario = consulta[0].usuario;
                     clase.id_Inventario = consulta[0].id;
                     clase.id_cliente = Convert.ToInt32(consulta[0].id_cliente);
                     clase.id_factura = Convert.ToInt32(consulta[0].id_factura);
                     clase.id_terminado = Convert.ToInt32(consulta[0].id_terminado);
                     clase.cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente;
                     clase.factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura;
                     clase.terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado;
                     QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_Inventario +
                                                                       "&po=" + clase.po +
                                                                       "&cl=" + clase.cliente +
                                                                       "&fa=" + clase.factura +
                                                                       "&te=" + clase.terminado +
                                                                       "&u=" + clase.usuario +
                                                                       "&pc=" + clase.ProductCode +
                                                                       "&c=" + clase.Cantidad +
                                                                       "&sz=" + clase.Size +
                                                                       "&fe=" + clase.Fecha, QRCodeGenerator.ECCLevel.Q);
                     QRCode qrCode = new QRCode(qrCodeData);
                     BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                     Codigo.IncludeLabel = true;
                     Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                        , consulta[0].id.ToString()
                                                        , Color.Black
                                                        , Color.White, 200, 100);

                     clase.qr = qrCode.GetGraphic(20);
                     clase.codigoBarras = codigoBarras;


                     listClase.Add(clase);
                     ReporteCaja report = new ReporteCaja();
                     report.DataSource = listClase;
                     // Disable margins warning. 
                     report.PrintingSystem.ShowMarginsWarning = false;
                     ReportPrintTool tool = new ReportPrintTool(report);
                     tool.ShowPreview();
                     //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                     //tool.PrintDialog(); //muestra a que impresora se va a mandar
                     tool.Print(); //imprime de golpe
                 }
                 */
                List<ConsultaInventarioIDResult> consulta2 = f.ConsultaInventarioID(cajaid2);
                if (consulta2.Count > 0)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();

                    List<EtiquetaCajaModificada> listclase2 = new List<EtiquetaCajaModificada>();
                    EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();

                    clase2.poInCompleto = consulta2[0].poInCompleto;
                    clase2.po = consulta2[0].po;
                    clase2.poItem = consulta2[0].poItem;
                    clase2.ProductCode = consulta2[0].ProductCode;
                    clase2.Size = consulta2[0].Size;
                    clase2.size_derecho = consulta2[0].size_derecho;
                    clase2.size_izquierdo = consulta2[0].size_izquierdo;
                    clase2.TipoCarton = consulta2[0].TipoCarton;
                    clase2.upc = consulta2[0].upc;
                    clase2.Fecha = consulta2[0].create_dtm;
                    clase2.CartonLeft = consulta2[0].CartonLeft;
                    clase2.CartonRight = consulta2[0].CartonRight;
                    clase2.Cantidad = consulta2[0].Cantidad;
                    clase2.Carton = consulta2[0].Carton;
                    clase2.usuario = consulta2[0].usuario;
                    clase2.id_Inventario = consulta2[0].id;
                    clase2.id_cliente = Convert.ToInt32(consulta2[0].id_cliente);
                    clase2.id_factura = Convert.ToInt32(consulta2[0].id_factura);
                    clase2.id_terminado = Convert.ToInt32(consulta2[0].id_terminado);
                    clase2.cliente = consulta2[0].cliente == string.Empty ? "NA" : consulta2[0].cliente;
                    clase2.factura = consulta2[0].factura == string.Empty ? "NA" : consulta2[0].factura;
                    clase2.terminado = consulta2[0].terminado == string.Empty ? "NA" : consulta2[0].terminado;
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
                                                                      "&po=" + clase2.po +
                                                                      "&cl=" + clase2.cliente +
                                                                      "&fa=" + clase2.factura +
                                                                      "&te=" + clase2.terminado +
                                                                      "&u=" + clase2.usuario +
                                                                      "&pc=" + clase2.ProductCode +
                                                                      "&c=" + clase2.Cantidad +
                                                                      "&sz=" + clase2.Size +
                                                                      "&fe=" + clase2.Fecha, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                    Codigo.IncludeLabel = true;
                    Image codigoBarras2 = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                       , consulta2[0].id.ToString()
                                                       , Color.Black
                                                       , Color.White, 200, 100);

                    clase2.qr = qrCode.GetGraphic(20);
                    clase2.codigoBarras = codigoBarras2;


                    listclase2.Add(clase2);
                    ReporteCaja report2 = new ReporteCaja();
                    report2.DataSource = listclase2;
                    // Disable margins warning. 
                    report2.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool2 = new ReportPrintTool(report2);
                    tool2.ShowPreview();
                    //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                    //tool.PrintDialog(); //muestra a que impresora se va a mandar
                    tool2.Print(); //imprime de golpe
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCajaIDDividir_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ConsultaDivision();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPiezas_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPiezas.Text != string.Empty)
                {
                    int piezas = Convert.ToInt32(txtPiezas.Text);
                    if (piezas < CantidadDividir && piezas > -1)
                    {
                        txtRestante.Text = (CantidadDividir - piezas).ToString();
                    }
                    else if (piezas < 0)
                    {
                        txtPiezas.Text = string.Empty;
                        MessageBox.Show("Favor de ingresar una caja correcta");
                    }
                    else
                    {
                        MessageBox.Show("Favor de ingresar un numero de piezas menor de " + CantidadDividir);
                        txtRestante.Text = CantidadDividir.ToString();
                        txtPiezas.Text = string.Empty;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPiezas_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    if (txtRestante.Text != "0" && txtRestante.Text != "")
                    {
                        DialogResult dialogResult = MessageBox.Show("Seguro que deseas Dividir esta caja " + txtCajaIDDividir.Text, "Cerrar", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (txtPiezas.Text != string.Empty && txtRestante.Text != string.Empty && txtCajaIDDividir.Text != string.Empty)
                            {
                                int idCaja = Convert.ToInt32(txtCajaIDDividir.Text);
                                int piezas = Convert.ToInt32(txtPiezas.Text);
                                int restante = Convert.ToInt32(txtRestante.Text);
                                ubicacion_Dividir_CajaIDResult r = f.DividirCaja(idCaja, usu[0].id, piezas, restante);
                                if (r.Caja1 != null && r.Caja2 != null)
                                {
                                    ImprimirEtiquetasDivididas(r.Caja1, r.Caja2);
                                    txtCajaIDDividir.Text = "";
                                    txtPiezas.Text = "";
                                    txtRestante.Text = "";
                                }
                                else
                                {
                                    MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                                }
                            }
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                        }
                    }
                    else
                    {
                        MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void xtpDividirCaja_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnGuardarA_Click(object sender, EventArgs e)
        {

        }

        private void dgvAlta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

        private void btnLog_Click(object sender, EventArgs e)
        {
            lblError.Items.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            txtTotalEscaneado.Text = "0";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            lblErrorEmb.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            txtTotalEscaneadoEmb.Text = "0";
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (txtID.Text != "0")
            {

            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (txtID.Text != "0")
            {

            }
        }
        /// <summary>
        /// PARA importar EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                importar();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void importar()
        {

            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog();
            ope.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (ope.ShowDialog() == DialogResult.Cancel)
                return;
            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();

            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 2)
                    {
                        string upc = dr[6].ToString();
                        upc = upc.Replace("-", "");
                        upc = upc.Replace(" ", "");
                        try
                        {
                            EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                            clase.po = Convert.ToInt64(dr[0].ToString());
                            clase.poInCompleto = Convert.ToInt64(dr[0].ToString().PadRight(6));
                            clase.poItem = null;
                            clase.ProductCode = dr[9].ToString();
                            string[] separadas;
                            separadas = dr[6].ToString().Split('-');
                            clase.Size = separadas[1].ToString().Substring(0, 5).TrimStart('0') + "x" + separadas[1].ToString().Substring(5, 2);
                            clase.size_izquierdo = separadas[1].ToString().Substring(0, 5).TrimStart('0');
                            clase.size_derecho = separadas[1].ToString().Substring(5, 2);
                            clase.TipoCarton = null;
                            clase.assembly = dr[10].ToString();
                            clase.Vendor = dr[2].ToString();
                            clase.ShipTo = dr[3].ToString();
                            clase.upc = upc;
                            clase.Fecha = DateTime.Now;
                            clase.CartonLeft = dr[7].ToString();
                            clase.CartonRight = separadas[0].ToString() + separadas[1].ToString();
                            clase.Cantidad = Convert.ToInt64(dr[11].ToString());
                            clase.Carton = null;
                            clase.usuario = usu[0].nombre;
                            clase.id_Inventario = f.GuardaProducto(clase, this.usu[0].id);
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            cmbPO.DataSource = null;
            cmbPO.Items.Clear();
            cmbPOB.DataSource = null;
            cmbPOB.Items.Clear();

            f.ConsultaPO(this.cmbPO);
            f.ConsultaPO(this.cmbPOB);
            MessageBox.Show("Termino con Exito!");
        }

        private void btnReporteDiario_Click(object sender, EventArgs e)
        {
            List<ConsultaInventarioPorHoraResult> inv = f.ConsultaInventarioPorHora(dtpReporteDiario.Value.Date, dtpReporteDiario.Value.Date);

            ReporteDiario report2 = new ReporteDiario();
            report2.DataSource = inv;
            report2.PrintingSystem.ShowMarginsWarning = false;
            ReportPrintTool tool2 = new ReportPrintTool(report2);
            tool2.ShowPreview();
            //tool2.Print();


        }

        private void btnReporteUbicacion_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                BuscarReporteUbicacion();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BuscarReporteUbicacion()
        {
            try
            {
                List<ubicacion_Entrada_ConsultaUbicacionDetalleResult> inv = f.ConsultaUbicacion();
                dgReporteUbicacion.DataSource = inv;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirUbicacion_Click(object sender, EventArgs e)
        {
            ShowGridPreview(dgReporteUbicacion);
        }

        private void btnBuscarUbicacion_Click(object sender, EventArgs e)
        {
            try
            {
                consultaUbicacionID();
                txtIDCaja.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void consultaUbicacionID()
        {
            try
            {
                int ubicacionID = Convert.ToInt32(txtUbicacionReporte.Text);
                if (ubicacionID > 0)
                {
                    List<ubicacion_Entrada_ConsultaUbicacionIDResult> seccionid = f.ConsultaUbicacionID(ubicacionID);
                    if (seccionid.Count > 0)
                    {
                        List<ubicacion_Entrada_ConsultaUbicacionDetalleIDResult> ubicacionDetalle = f.ConsultaUbicacionDetalleID(ubicacionID);
                        dgReporteUbicacionID.DataSource = ubicacionDetalle;
                        //dgReporteUbicacion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtUbicacionReporte_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    consultaUbicacionID();
                    txtIDCaja.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirUbicacionR_Click(object sender, EventArgs e)
        {
            ShowGridPreview(dgReporteUbicacionID);
        }

        private void txtUnitsScan_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtIDReimpresionAlmacen_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if ((int)e.KeyChar == (int)Keys.Enter)
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

        private void btnEditar_Click_1(object sender, EventArgs e)
        {

        }


        private void txtCantidadPrepack_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //Para obligar a que sólo se introduzcan números
                if (Char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
              if (Char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
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
                if (Char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
              if (Char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                {
                    e.Handled = false;
                }
                else
                {
                    //el resto de teclas pulsadas se desactivan
                    e.Handled = true;
                }

                if ((int)e.KeyChar == (int)Keys.Enter)
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
                if (Char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
              if (Char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
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

                    if (altaP == 0)
                    {

                        tablaPrepack = new DataTable();
                        tablaPrepack.Columns.Add("Talla", typeof(string));
                        tablaPrepack.Columns.Add("Cantidad", typeof(Int64));
                        tablaPrepack.Columns.Add("Codigo UPC", typeof(string));
                        tablaPrepack.Columns.Add("idSize", typeof(int));
                        altaP = altaP + 1;
                    }
                    else
                    {

                    }

                    tablaPrepack.Rows.Add(cmbTallaPrepack.Text, txtCantidadPrepack.Text, txtCodigoupcPrepack.Text, cmbTallaPrepack.SelectedValue.ToString());
                    dgvPrePack2.DataSource = tablaPrepack;
                    dgvPrePack2.Columns["idSize"].Visible = false;
                    cmbTallaPrepack.SelectedIndex = 0;
                    txtCodigoupcPrepack.Text = "";
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

                        //p.estilo = txtEstiloPrepack.Text;
                        p.po_numero = Convert.ToDecimal(txtPoPrepack.Text);

                        idPrepack = f.GuardarPrePack(p);

                        foreach (DataGridViewRow renglon in dgvPrePack2.Rows)
                        {
                           
                            int cantidad = Convert.ToInt32(renglon.Cells[1].Value.ToString());

                            for (int i = cantidad; i > 0; i--)
                            {
                                PrepackDetalle pd = new PrepackDetalle();
                                pd.idPrepack = idPrepack;
                                pd.size = renglon.Cells[0].Value.ToString();
                                pd.cantidad = 1;
                                pd.upc = renglon.Cells[2].Value.ToString();
                                pd.idusuario = this.usu[0].id;
                                pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                f.GuardarPrePackDetalle(pd);
                                altaP = 0;
                            }

                        }

                        LimpiarCampos();

                        // MessageBox.Show("PREPACK: "+ idPrepack);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        public void LimpiarCampos()
        {
            try
            {
                //txtEstiloPrepack.Text = "";

                tablaPrepack = new DataTable();
                tablaPrepack.Columns.Add("Talla", typeof(string));
                tablaPrepack.Columns.Add("Cantidad", typeof(Int64));
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
        public bool vacio; // Variable utilizada para saber si hay algún TextBox vacio.
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

        private void txtEstiloPrepack_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtUPCScann_TextChanged(object sender, EventArgs e)
        {

        }

        private void Escan_Click(object sender, EventArgs e)
        {

        }

        private void dgvEscan_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tpApparelInternational_Click(object sender, EventArgs e)
        {

        }

        private void btnImportarLevis_Click(object sender, EventArgs e)
        {
            try
            {
                importarLEVIS();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void importarLEVIS()
        {

            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog();
            ope.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (ope.ShowDialog() == DialogResult.Cancel)
                return;
            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();

            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 1)
                    {

                        try
                        {
                            string upc = dr[3].ToString();
                            if (upc.Length > 5)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                                clase.po = Convert.ToInt64(dr[0].ToString());
                                clase.poInCompleto = 0;
                                clase.poItem = null;
                                clase.ProductCode = "";
                                clase.Size = "";
                                clase.size_izquierdo = dr[1].ToString();
                                clase.size_derecho = dr[2].ToString();
                                clase.TipoCarton = null;
                                clase.assembly = "";
                                clase.Vendor = "";
                                clase.ShipTo = "";
                                if (upc.Length < 12)
                                {
                                    upc = "0" + upc;
                                    // MessageBox.Show("Debe ingresar como minimo 12 caracteres");
                                }
                                else
                                {

                                }
                                clase.upc = upc;
                                clase.Fecha = DateTime.Now;
                                clase.CartonLeft = "";
                                clase.CartonRight = "";
                                clase.Cantidad = Convert.ToInt64(dr[4].ToString());
                                clase.Carton = null;
                                clase.usuario = usu[0].nombre;
                                clase.id_Inventario = f.GuardaProductoLEVIS(clase, this.usu[0].id);
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarPOReporte();
            Cursor.Current = Cursors.Default;
        }


        public void BuscarPOReporte()
        {

            try
            {
                List<ConsultaInventarioResult> inv = f.ConsultaInventario(dtpFechaInicioPO.Value.Date, dtpFechaFinPO.Value.Date);

                gcPO.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirReportePO_Click(object sender, EventArgs e)
        {

            ShowGridPreview(gcPO);
        }

        private void btnActualizaPOS_Click(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            BuscarModuloReporte();
            Cursor.Current = Cursors.Default;
        }


        public void BuscarModuloReporte()
        {

            try
            {
                List<ConsultaInventarioResult> inv = f.ConsultaInventario(dtpFechaInicioModulos.Value.Date, dtpFechaFinModulos.Value.Date);

                gcModulo.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimeModulos_Click(object sender, EventArgs e)
        {

            ShowGridPreview(gcModulo);
        }

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {

        }
        public int validarAltaPO()
        {
            try
            {
                if (string.IsNullOrEmpty(txtAltaPO.Text) || dgvAltaPOManual.RowCount < 1)
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

        private void btnGuardarAltaPO_Click(object sender, EventArgs e)
        {
            try
            {
                if (validarAltaPO() == 1)
                {
                    if (dgvAltaPOManual.RowCount > 0)
                    {
                        foreach (DataGridViewRow renglon in dgvAltaPOManual.Rows)
                        {

                            int cantidad = 0;
                            try { cantidad = Convert.ToInt32(renglon.Cells[1].Value.ToString()); }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }

                            /*
                             * SE COMENTA POR SI SE NECESITA PRENDA POR PRENDA
                            for (int i = cantidad; i > 0; i--)
                            {
                            */
                            //EL PO NO SE MUEVE
                                EtiquetaCajaModificada pd = new EtiquetaCajaModificada();
                                try { pd.po = Convert.ToDecimal(txtAltaPO.Text); }
                                catch (Exception ex) { MessageBox.Show("Favor de capturar correctamente la cantidad " + ex.Message); }
                                try { pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString()); }
                                catch (Exception ex) { MessageBox.Show("Favor de capturar correctamente la cantidad " + ex.Message); }
                                pd.upc = renglon.Cells[2].Value.ToString();
                                pd.idusuario = this.usu[0].id;
                                pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                f.GuardaAltaPO(pd);
                                alta = 0;
                           /* }*/

                        }


                        LimpiarCamposAltaPO();

                        // MessageBox.Show("PREPACK: "+ idPrepack);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnAltaPOAgregarTabla_Click(object sender, EventArgs e)
        {
            AgregarTallaManual();

        }

        public void AgregarTallaManual()
        {

            try
            {
                if (txtAltaPOCantidad.Text != "" && txtAltaPOUPC.Text != "")
                {
                    if (alta == 0)
                    {

                        tablaAltaPO = new DataTable();
                        tablaAltaPO.Columns.Add("Talla", typeof(string));
                        tablaAltaPO.Columns.Add("Cantidad", typeof(Int64));
                        tablaAltaPO.Columns.Add("Codigo UPC", typeof(string));
                        tablaAltaPO.Columns.Add("idSize", typeof(int));
                        alta = alta + 1;
                    }
                    else
                    {

                    }
                    tablaAltaPO.Rows.Add(cmbAltaPOTalla.Text, txtAltaPOCantidad.Text, txtAltaPOUPC.Text, cmbAltaPOTalla.SelectedValue.ToString());
                    dgvAltaPOManual.DataSource = tablaAltaPO;
                    dgvAltaPOManual.Columns["idSize"].Visible = false;
                    cmbAltaPOTalla.SelectedIndex = 0;
                    txtAltaPOUPC.Text = "";
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

        private void btnAltaPOActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvAltaPOManual.RowCount > 0)
                {
                    //RengloSelecionado = dgvPrePack2.CurrentCell.RowIndex;
                    DataGridViewRow newDatarow = dgvAltaPOManual.Rows[RengloSelecionado];
                    newDatarow.Cells[0].Value = cmbAltaPOTalla.Text;
                    newDatarow.Cells[1].Value = txtAltaPOCantidad.Text;
                    newDatarow.Cells[2].Value = txtAltaPOUPC.Text;
                    newDatarow.Cells[3].Value = cmbAltaPOTalla.SelectedValue.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAltaPOBorrarTabla_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvAltaPOManual.RowCount > 0)
                {
                    RengloSelecionado = dgvAltaPOManual.CurrentCell.RowIndex;
                    dgvAltaPOManual.Rows.RemoveAt(RengloSelecionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvAltaPOManual_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RengloSelecionado = e.RowIndex;
                DataGridViewRow row = dgvAltaPOManual.Rows[RengloSelecionado];

                cmbAltaPOTalla.Text = row.Cells[0].Value.ToString();
                txtAltaPOCantidad.Text = row.Cells[1].Value.ToString();
                txtAltaPOUPC.Text = row.Cells[2].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAltaPOCancelar_Click(object sender, EventArgs e)
        {

            LimpiarCamposAltaPO();
        }
        public void LimpiarCamposAltaPO()
        {
            try
            {
                tablaAltaPO = new DataTable();
                tablaAltaPO.Columns.Add("Talla", typeof(string));
                tablaAltaPO.Columns.Add("Cantidad", typeof(Int64));
                tablaAltaPO.Columns.Add("Codigo UPC", typeof(string));
                tablaAltaPO.Columns.Add("idSize", typeof(int));

                txtAltaPO.Text = "";
                cmbAltaPOTalla.SelectedIndex = 0;
                txtAltaPOCantidad.Text = "";
                txtAltaPOUPC.Text = "";
                for (int i = dgvAltaPOManual.Rows.Count - 1; i >= 0; i--)
                {
                    dgvAltaPOManual.Rows.RemoveAt(i);
                }

                foreach (DataGridViewRow item in dgvAltaPOManual.SelectedRows)
                {
                    dgvAltaPOManual.Rows.RemoveAt(item.Index);

                }
                alta = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtAltaPOUPC_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {

                //Para obligar a que sólo se introduzcan números
                if (Char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
              if (Char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                {
                    e.Handled = false;
                }
                else
                {
                    //el resto de teclas pulsadas se desactivan
                    e.Handled = true;
                }

                if ((int)e.KeyChar == (int)Keys.Enter)
                {
                    AgregarTallaManual();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                importarTARGET();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void importarTARGET()
        {
            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog();
            ope.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            if (ope.ShowDialog() == DialogResult.Cancel)
            return;
            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();

            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 1)
                    {
                        try
                        {
                            string dpci = dr[8].ToString();
                            if (dpci.Length >1)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                                clase.po = Convert.ToInt64(dr[9].ToString());
                                clase.poInCompleto = 0;
                                clase.poItem = null;
                                clase.ProductCode = dr[9].ToString(); 
                                clase.Size = dr[18].ToString();
                                clase.size_izquierdo = dr[18].ToString();
                                clase.size_derecho = "";
                                clase.TipoCarton = null;
                                clase.assembly = dr[46].ToString();
                                clase.Vendor = dr[7].ToString(); /*upc 2*/
                                clase.ShipTo = dr[53].ToString();
                                clase.upc = dr[2].ToString();
                                clase.Fecha = DateTime.Now;
                                clase.CartonLeft = "";
                                clase.CartonRight = "";
                                clase.Cantidad = Convert.ToInt64(dr[31].ToString());
                                clase.Carton = null;
                                clase.usuario = usu[0].nombre;
                                
                                clase.id_Inventario = f.GuardaProductoTARGET(clase, this.usu[0].id);
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
        }

    }
}
