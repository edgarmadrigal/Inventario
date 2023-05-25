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
        private readonly Funciones f = new Funciones();
        private bool iniciando = true;
        private string cantidad = "0";
        private int cantidadAnterior = 0;
        private string upc = string.Empty;
        private string vendor = string.Empty;
        private readonly WindowsMediaPlayer sonido = new WindowsMediaPlayer();
        private readonly List<ConsultaUsuarioResult> usu = null;
        private int? id = 0;
        private readonly List<int?> anterior = new List<int?>();
        private int contador = 0;
        private int contadorVecesCMBPO = 0;
        private int id_InventarioAnt = 0;
        private int? CantidadDividir = 0;
        private DataTable tablaPrepack = new DataTable();
        private DataTable tablaAltaPO = new DataTable();
        private DataTable tablaZumies = new DataTable();
        private DataTable tablaTarget = new DataTable();
        private DataTable tablaModificacion = new DataTable();
        private int RengloSelecionado;
        private int Tabpage = 0;
        private int alta = 0;
        private int altaP = 0;
        private int altaZ = 0;
        private int PrendasExtra = 0;
        /*1000 ES LEVIS CINTAS IMPORTADO Y TODO LO QUE SE DE DE ALTA MANUAL */
        /*99 ES PREPACK*/
        /*88  ES TARGET */
        /*77 ES ZUMIES*/
        /*--------*/
        public void InicializarTablas()
        {
            tablaPrepack = new DataTable();
            tablaAltaPO = new DataTable();
            tablaZumies = new DataTable();
            tablaTarget= new DataTable();
        }
        public Escaneo(List<ConsultaUsuarioResult> usuario)
        {
            try
            {
                InitializeComponent();


                f.ConsultaPO(cmbPO);
                cmbPO.SelectedIndex = -1;
                f.ConsultaCliente(cmbCliente);
                cmbCliente.SelectedIndex = 0;
                f.ConsultaFactura(cmbFactura);

                cmbFactura.SelectedIndex = 0;
                f.ConsultaTerminado(cmbTerminado);
                cmbTerminado.SelectedIndex = 0;

                f.ConsultaPOModificar(cmbPOMod);
                cmbPOMod.SelectedIndex = -1;

                /**/
                f.ConsultaPO(cmbPOB);
                cmbPOB.SelectedIndex = 0;
                f.ConsultaCliente(cmbClienteB);
                cmbClienteB.SelectedIndex = 0;
                f.ConsultaFactura(cmbFacturacionB);
                cmbFacturacionB.SelectedIndex = 0;
                f.ConsultaTerminado(cmbTerminadoB);
                cmbTerminadoB.SelectedIndex = 0;
                //f.ConsultaTallas(cmbSizeA);
                //cmbSizeA.SelectedIndex = 0;

                f.ConsultaTallasZumies(cmbzTallas);
                cmbzTallas.SelectedIndex = 0;
                //f.ConsultaTipoCaja(cmbTipoCajaA);
                //cmbTipoCajaA.SelectedIndex = 15;
                /**/
                f.ConsultaPO(cmbPoEntradaAlmacen);
                cmbPoEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaCliente(cmbClienteEntradaAlmacen);
                cmbClienteEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaTerminado(cmbTerminadoEntradaAlmacen);
                cmbTerminadoEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaFactura(cmbFacturacionEntradaAlmacen);
                cmbFacturacionEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaPO(cmbPOSalidaAlmacen);
                cmbPOSalidaAlmacen.SelectedIndex = 0;
                f.ConsultaTallas(clbT);

                f.ConsultaTallasXMarca(cmbTallaPrepack, "LEVIS");
                cmbTallaPrepack.SelectedIndex = 0;

                usu = usuario;
                try
                {
                    lblVersion.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                    lblVersion2.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
                catch (Exception) { }

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
                    TabEsaneo.TabPages.Remove(ModificarPO);
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
                    TabEsaneo.TabPages.Remove(ModificarPO);
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
                    TabEsaneo.TabPages.Remove(ModificarPO);

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
                    TabEsaneo.TabPages.Remove(ModificarPO);

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
                    /*PERFIL DE ADMON*/
                    TabEsaneo.TabPages.Remove(tpReporteDiario);
                    TabEsaneo.TabPages.Remove(ModificarPO);
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
                    //button6.Enabled = true;
                    //button6.Visible = true;
                }
                else if (usu[0].perfil == "12")
                {
                    TabEsaneo.TabPages.Remove(tpAltaPrePack);
                    TabEsaneo.TabPages.Remove(tpReporteDiario);
                    TabEsaneo.TabPages.Remove(tpBajaCaja);
                    TabEsaneo.TabPages.Remove(tpbajaPO);
                    TabEsaneo.TabPages.Remove(tpImportaLEVIS);
                    TabEsaneo.TabPages.Remove(ModificarPO);
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
                else if (usu[0].perfil == "20")
                {

                    TabEsaneo.TabPages.Remove(tpAltaPrePack);
                    TabEsaneo.TabPages.Remove(tpReporteDiario);
                    TabEsaneo.TabPages.Remove(tpBajaCaja);
                    TabEsaneo.TabPages.Remove(tpbajaPO);
                    TabEsaneo.TabPages.Remove(tpImportaLEVIS);
                    TabEsaneo.TabPages.Remove(ModificarPO);
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
                    TabEsaneo.TabPages.Remove(ModificarPO);
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
                f.ConsultaPO(cmbPO);
                cmbPO.SelectedIndex = 1;
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
                    if (cmbPO.Text != "")
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
            }
            catch (Exception ex)
            {
                //  MessageBox.Show(ex.Message.ToString());
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
                //MessageBox.Show(ex.Message.ToString());
            }
        }
        private void btnIncompleteCarton_Click(object sender, EventArgs e)
        {
            try
            {
                string po_numero = cmbPO.Text;
                if (id != 0 && Convert.ToInt32(txtUnitsScan.Text) > 0 && dgvEscan.RowCount > 0)
                {
                    if (cmbPOItem.Text != "99" && cmbPOItem.Text != "77" && cmbPOItem.Text != "88")
                    {
                        txtUPCScann.Text = string.Empty;
                        txtUPCScann.Focus();
                        List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);
                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                        {
                            id = consulta[0].id,
                            po = consulta[0].po,
                            poInCompleto = consulta[0].poInCompleto,
                            poItem = consulta[0].poItem,
                            ProductCode = consulta[0].ProductCode,
                            Size = consulta[0].Size,
                            size_derecho = consulta[0].size_derecho,
                            size_izquierdo = consulta[0].size_izquierdo,
                            TipoCarton = consulta[0].TipoCarton,
                            upc = consulta[0].upc,
                            Fecha = DateTime.Now,
                            CartonLeft = consulta[0].CartonLeft,
                            CartonRight = consulta[0].CartonRight,
                            Cantidad = Convert.ToInt32(txtUnitsScan.Text),
                            Carton = consulta[0].Carton,
                            usuario = usu[0].nombre,
                            id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue),
                            id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue),
                            id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue),
                            cliente = cmbCliente.Text,
                            factura = cmbFactura.Text,
                            terminado = cmbTerminado.Text
                        };
                        /**/
                        if (clase.poItem == "1000")
                        {
                            do
                            {
                                try
                                {
                                    clase.Carton = Convert.ToInt64(Interaction.InputBox("Captura Numero de Carton", "Carton", "", 5, 5));

                                }
                                catch (Exception)
                                {
                                    /*clase.Carton = 0; MessageBox.Show("Favor de ingresar correctamente el numero de carton ya que no se ha guardado");*/
                                };

                            } while (clase.Carton == 0 || clase.Carton.ToString().Length > 10);

                        }
                        if ((clase.poItem != "1000" && clase.poItem != "99") || (clase.poItem == "1000" && clase.Carton != 0 && clase.poItem != "99"))
                        {
                            clase.assembly = consulta[0].Assembly;
                            clase.Vendor = consulta[0].Vendor;
                            clase.ShipTo = consulta[0].ShipTo;
                            clase.id_Inventario = f.GuardaInventario(clase, usu[0].id);

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
                                BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                                {
                                    IncludeLabel = true
                                };
                                Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 200, 100);
                                clase.qr = qrCode.GetGraphic(20);
                                clase.codigoBarras = codigoBarras;
                                listClase.Add(clase);
                                id_InventarioAnt = clase.id_Inventario;

                                ReporteCaja report = new ReporteCaja
                                {
                                    DataSource = listClase
                                };
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
                                    catch (Exception)
                                    {
                                        clase.upc = consulta[0].upc;
                                    }
                                }
                                cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                                {
                                    IncludeLabel = true,
                                    RotateFlipType = RotateFlipType.Rotate90FlipY
                                };
                                //string contarDigitos = "12345678911234";
                                Image codigoBarras =
                                //Codigo.Encode(BarcodeLib.TYPE.CODE39, contarDigitos, Color.Black, Color.White, 270, 180);
                                Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 270, 180);

                                // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                                clase.codigoBarras = codigoBarras;
                                listClase.Add(clase);
                                id_InventarioAnt = clase.id_Inventario;

                                ReporteCintas report = new ReporteCintas
                                {
                                    DataSource = listClase
                                };
                                // Disable margins warning. 
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                //tool.Print(); //imprime de golpe
                                LimpiarPantallaEscaneo();

                            }
                        }

                    }
                    else if (cmbPOItem.Text == "77" && cmbPOItem.Text != "99")/*PREPACK*/
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
                                clase.po = Convert.ToDecimal(cmbPO.Text);
                                clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
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

                                id_InventarioAnt = f.GuardaInventarioZumies(clase, usu[0].id);

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
                            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(id_InventarioAnt);
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
                                    txtUnitsScan.Text = "0";
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
                                claseZ.poInCompleto = x1[0].poInCompleto;
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

                                List<EtiquetaZUMIES> listclaseZ = new List<EtiquetaZUMIES>();
                                listclaseZ.Add(claseZ);
                                id_InventarioAnt = claseZ.id_Inventario;

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
                    else if (cmbPOItem.Text == "88")
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
                                clase.po = Convert.ToDecimal(cmbPO.Text);
                                clase.id_Inventario = f.GuardaInventarioZumies(clase, usu[0].id);

                                id_InventarioAnt = clase.id_Inventario;
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                try
                                {
                                    List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(id_InventarioAnt);
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
                                        claseZ.poInCompleto = x1[0].poInCompleto;
                                        claseZ.poItem = "88";
                                        claseZ.ProductCode = x1[0].ProductCode;
                                        claseZ.Size = x1[0].size_izquierdo;
                                        claseZ.ESTILO = x1[0].ProductCode;
                                        claseZ.QUANTITY = txtUnitsScan.Text;
                                        claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                        claseZ.CARTON_NUMBER_INICIAL = id_InventarioAnt.ToString();
                                        claseZ.CARTON_NUMBER_FINAL = "___";
                                        claseZ.COUNTRY = "MEXICO";
                                        claseZ.Carton = id_InventarioAnt;
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
                                        id_InventarioAnt = Convert.ToInt32(clase.id);
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
                                        clase2.id_Inventario = Convert.ToInt32(clase.id);
                                        clase2.po = x1[0].po;
                                        clase2.poInCompleto = claseZ.po;
                                        clase2.cliente = "";
                                        clase2.factura = "";
                                        clase2.terminado = "";
                                        clase2.usuario = usu[0].nombre;
                                        clase2.ProductCode = claseZ.ProductCode;
                                        clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                        clase2.Size = claseZ.size_izquierdo;
                                        clase2.Fecha = x1[0].create_dtm;
                                        clase2.assembly = "*" + clase.id.ToString() + "*";
                                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                                        Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
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
                                MessageBox.Show("Ya se escanearon todas las cajas de la Talla");
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        #endregion TOTAL DE PRENDAS ESCANEADAS
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
            if (cmbPO.Text == "")
            {

            }
            else
            {
                try
                {
                    if (!iniciando && cmbPO.SelectedIndex > -1 && cmbPOItem.SelectedIndex > -1 && cmbProductCode.SelectedIndex > -1 && cmbSizes.SelectedIndex > -1)
                    {
                        string[] separadas;
                        separadas = cmbSizes.Text.Split('x');
                        if (cmbPOItem.Text == "99")/* PREPACK */
                        {




                            separadas = cmbSizes.Text.Split('x');
                            List<ConsultaProductosNuevoResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());
                            dgvEscan.DataSource = x;

                            dgvEscan.Columns["ProductCode1"].Visible = false;
                            dgvEscan.Columns["Vendor"].Visible = false;
                            dgvEscan.Columns["ProductCode"].Visible = false;
                            dgvEscan.Columns["CartonType"].Visible = false;
                            dgvEscan.Columns["id"].Visible = false;
                            if (x.Count > 1)
                            {
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
                            }
                            cantidad = x.Count.ToString();
                            txtUnitsRemai.Text = cantidad.ToString();
                            txtUnitsReq.Text = cantidad.ToString();
                            txtUnitsScan.Text = "0";
                            txtCartonRq.Text = "1";
                            txtCartonsReamaining.Text = "1";
                            txtUPCScann.Focus();
                        }
                        else if (cmbPOItem.Text == "88")/*TARGET*/
                        {

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
                                List<ConsultaProductosTargetResult> x1 = f.ConsultaProductosTarget(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());
                                if (x1.Count > 0)
                                {
                                    contador = 0;
                                    dgvEscan.DataSource = x1;
                                    txtCartonsPacked.Text = x1[0].NumeroCaja.ToString();
                                    txtCartonNumber.Text = x1[0].NumeroCaja.ToString();
                                    txtCartonSize.Text = x1[0].Talla.ToString();
                                    dgvEscan.Columns["Cantidad"].Visible = false;
                                    dgvEscan.Columns["escaneado"].Visible = false;
                                    dgvEscan.Columns["NumeroCaja"].Visible = false;
                                    dgvEscan.Columns["itemDescription"].Visible = false;
                                    //dgvEscan.Columns["cantidadCajas"].Visible = false;
                                    dgvEscan.Columns["id"].Visible = false;
                                    txtSize.Text = x1[0].Talla.ToString();
                                    txtProductCode.Text = x1[0].estilo.ToString();
                                    id = x1[0].id;
                                    anterior.Add(id);
                                    upc = x1[0].upc.ToString();
                                    vendor = x1[0].itemDescription.ToString();
                                    cantidad = x1[0].cantidad;
                                    txtUnitsReq.Text = x1[0].cantidad.ToString();
                                    txtUnitsScan.Text = "0";
                                    txtUnitsRemai.Text = cantidad.ToString();
                                    txtUPCScann.Focus();
                                }
                                else
                                {
                                    MessageBox.Show("Ya se escanearon todas las cajas de la Talla");
                                }
                            }

                        }
                        else if (cmbPOItem.Text == "77")/*ZUMIES*/
                        {
                            try
                            {
                                List<ConsultaProductosZumiesResult> x1 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString());

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
                                    MessageBox.Show("Ya se escanearon todas las cajas de la Talla");
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }

                        }
                        else
                        {

                            separadas = cmbSizes.Text.Split('x');
                            List<ConsultaProductosNuevoResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());
                            if (x.Count > 0)
                            {
                                contador = 0;
                                dgvEscan.DataSource = x;
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
                                txtCartonRq.Text = "1";
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
            //txtCartonRq.Text = "1";
            //txtCartonsPacked.Text = "0";
            txtCartonsReamaining.Text = cantidad.ToString();
            txtUPCScann.Focus();
        }
        public EtiquetaCajaModificada RellenaObjetoClase(List<ConsultaEtiquetaResult> consulta)
        {
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                id = consulta[0].id,
                po = consulta[0].po,
                poInCompleto = consulta[0].poInCompleto,
                poItem = consulta[0].poItem,
                ProductCode = consulta[0].ProductCode,
                Size = consulta[0].Size,
                size_derecho = consulta[0].size_derecho,
                size_izquierdo = consulta[0].size_izquierdo,
                TipoCarton = consulta[0].TipoCarton,
                upc = consulta[0].upc,
                Fecha = DateTime.Now,
                CartonLeft = consulta[0].CartonLeft,
                CartonRight = consulta[0].CartonRight,
                Cantidad = consulta[0].Cantidad,
                Carton = consulta[0].id,
                color = consulta[0].color,
                itemDescription = consulta[0].itemDescription,
                usuario = usu[0].nombre,
                estilo = consulta[0].ProductCode,
                id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue),
                id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue),
                id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue),
                cliente = cmbCliente.Text,
                factura = cmbFactura.Text,
                terminado = cmbTerminado.Text
            };
            return clase;
        }

        private void txtUPCScann_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    tpEscan.BackColor = Color.Silver;
                    List<ConsultaUPCResult> ListUpc;
                    string po_numero = cmbPO.Text;

                    DataGridViewRow newDatarow;
                    if (PrendasExtra == 1)
                    {
                        newDatarow = dgvEscan.Rows[(Convert.ToInt32(1))];
                    }
                    else
                    {
                        newDatarow = dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))];
                    }
                    if (cmbPOItem.Text == "99")/*PREPACK*/
                    {
                        upc = newDatarow.Cells[4].Value.ToString();


                    }
                    if (txtUnitsScan.Text == "0" && cmbPOItem.Text != "99")
                    {
                        //SOLO UNA VEZ 
                        ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                        if (ListUpc.Count > 0)
                        {
                            upc = ListUpc[0].upc;
                            id = ListUpc[0].id;
                            List<ConsultaProductosTargetResult> x2 = f.ConsultaProductosTarget(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

                            /***/
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
                                cantidad = x2[0].cantidad.ToString();
                                txtUnitsReq.Text = x2[0].cantidad.ToString();
                                txtUnitsRemai.Text = cantidad.ToString();
                                txtUnitsScan.Text = "0";
                                txtUPCScann.Focus();
                            }
                            iniciando = true;
                            f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                            cmbPOItem.SelectedIndex = 0;
                            f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                            cmbProductCode.SelectedIndex = 0;
                            iniciando = false;

                            cmbSizes.Text = ListUpc[0].Size;
                        }
                    }
                    #region PREPACK
                    if (upc == txtUPCScann.Text && cmbPOItem.Text == "99")/* CUANDO ES PREPACK*/
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
                            string marca = "";

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
                                    /*TOTAL DE PRENDAS ESCANEADAS*/
                                    /*
                                     * 
                                     * 
                                     * CONSULTAR DE LA TABLA DE PREPACK Y PREPACK DETALLE
                                     * 
                                     * 
                                     * 
                                     */
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

                                    int? esprepack = f.ConsultaPrepack(cmbPO.Text);

                                    if (esprepack > 0)
                                    {
                                        List<ConsultaPrepackDetalleResult> ListprepackTarget = f.ConsultaPrepackDetalle(Convert.ToInt32(esprepack));
                                        if (ListprepackTarget.Count > 0)
                                        {
                                            //dgvEscan.DataSource = ListprepackTarget;
                                            //comentar lo de arriba
                                            //dgvEscan.Columns["idSize"].Visible = false;
                                            //dgvEscan.Columns["idUsuario"].Visible = false;
                                            //dgvEscan.Columns["fecha"].Visible = false;
                                            //dgvEscan.Columns["idPrepack"].Visible = false;
                                            //dgvEscan.Columns["id"].Visible = false;

                                            decimal? cantidadTotalCajasPrepack = 0;
                                            string barcode = "";
                                            int? idPrepack = 0;
                                            string DPCI = "";
                                            EtiquetaZUMIES clase = new EtiquetaZUMIES();
                                            foreach (ConsultaPrepackDetalleResult objPrepack in ListprepackTarget)
                                            {
                                                cantidadTotalCajasPrepack = objPrepack.cantidad + cantidadTotalCajasPrepack;
                                                barcode = objPrepack.barcode;
                                                idPrepack = objPrepack.idPrepack;
                                                DPCI = objPrepack.DPCI;

                                            }

                                            clase = new EtiquetaZUMIES();
                                            clase.id = idPrepack;
                                            clase.po = Convert.ToDecimal(cmbPO.Text);
                                            clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                                            clase.poItem = "88";
                                            clase.ProductCode = barcode;
                                            clase.Size = "ASSORTMENT";
                                            clase.size_izquierdo = "ASSORTMENT";
                                            clase.size_derecho = "";
                                            clase.TipoCarton = "1";
                                            clase.upc = barcode;
                                            clase.Fecha = DateTime.Now;
                                            clase.CartonLeft = "";
                                            clase.CartonRight = "";
                                            clase.Cantidad = Convert.ToDecimal(cantidadTotalCajasPrepack);
                                            clase.Carton = 0;
                                            clase.usuario = usu[0].nombre;
                                            clase.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                                            clase.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                                            clase.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                                            clase.cliente = cmbCliente.Text;
                                            clase.factura = cmbFactura.Text;
                                            clase.terminado = cmbTerminado.Text;
                                            clase.Vendor = "0";
                                            clase.DPCI = DPCI.Trim();
                                            clase.ESTILO = "ASSORTMENT";
                                            clase.DESCRIPTION = DPCI.Trim();
                                            clase.itemDescription = DPCI.Trim();
                                            clase.QUANTITY = Convert.ToString(cantidadTotalCajasPrepack);
                                            clase.COUNTRY = "MEXICO";
                                            clase.id_Inventario = f.GuardaInventarioZumies(clase, usu[0].id);
                                            iniciando = true;
                                            f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                                            cmbPOItem.SelectedIndex = 0;
                                            f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                                            cmbProductCode.SelectedIndex = 0;
                                            f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                                            cmbSizes.SelectedText = ListUpc[0].Size;
                                            contador = 0;

                                            int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                                            EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                                            claseZ.id = clase.id_Inventario;
                                            claseZ.assembly = clase.po.ToString();
                                            claseZ.poInCompleto = clase.poInCompleto;
                                            claseZ.poItem = "88";
                                            claseZ.ProductCode = "ASSORTMENT";
                                            claseZ.Size = "ASSORTMENT";
                                            claseZ.ESTILO = "ASSORTMENT";
                                            claseZ.QUANTITY = Convert.ToString(cantidadTotalCajasPrepack);
                                            claseZ.CARTON_NUMBER_INICIAL = clase.id_Inventario.ToString();
                                            claseZ.CARTON_NUMBER_FINAL = "___";
                                            claseZ.COUNTRY = "MEXICO";
                                            claseZ.Cantidad = Convert.ToDecimal(cantidadTotalCajasPrepack);
                                            claseZ.DPCI = clase.DPCI;
                                            claseZ.itemDescription = claseZ.DPCI;
                                            claseZ.cn_tag_num = Convert.ToInt32(clase.id_Inventario);
                                            claseZ.color = "";
                                            claseZ.size_izquierdo = "ASSORTMENT";
                                            string contarDigitos = "00" + clase.ProductCode.Substring(0, 11) + "1";
                                            claseZ.Carton = clase.id_Inventario;
                                            //en ASSORTMENT NO ES EL UPC ES UNO QUE ENGLOBA TODAS LAS TALLAS

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
                                            id_InventarioAnt = Convert.ToInt32(clase.id_Inventario);
                                            ReporteCajaTargetAssorment report = new ReporteCajaTargetAssorment
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
                                            clase2.id_Inventario = Convert.ToInt32(claseZ.id);
                                            clase2.po = clase.po;
                                            clase2.poInCompleto = clase.po;
                                            clase2.cliente = "";
                                            clase2.factura = "";
                                            clase2.terminado = "";
                                            clase2.usuario = usu[0].nombre;
                                            clase2.ProductCode = claseZ.ProductCode;
                                            clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                            clase2.Size = "ASSORTMENT";
                                            clase2.Fecha = clase.Fecha;
                                            clase2.assembly = "*" + claseZ.id.ToString() + "*";

                                            QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                                            Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
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



                                            cantidad = cantidadTotalCajasPrepack.ToString();
                                            txtUnitsRemai.Text = cantidadTotalCajasPrepack.ToString();
                                            txtUnitsReq.Text = cantidadTotalCajasPrepack.ToString();
                                            txtUnitsScan.Text = "0";
                                            txtCartonRq.Text = "1";
                                            txtCartonsReamaining.Text = "1";
                                            txtUPCScann.Focus();
                                        }

                                        LimpiarPantallaEscaneo();

                                    }
                                    else
                                    {

                                    }
                                    #endregion
                                    break;
                                case "LEVIS":
                                    #region LEVIS
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

                                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                                        {
                                            id = a.id,
                                            po = Convert.ToDecimal(cmbPO.Text),
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
                                        clase.TipoCarton = (1).ToString();
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
                                        clase.id_Inventario = f.GuardaInventario(clase, usu[0].id);
                                    }
                                    #endregion 
                                    break;
                            }



                            #endregion

                        }
                        else
                        {
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
                    #endregion
                    else if (upc == txtUPCScann.Text && cmbPOItem.Text != "99")
                    {
                        if (cmbPOItem.Text == "77")/*ZUMIES*/
                        {
                            #region ZUMIES
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
                                        clase.po = Convert.ToDecimal(cmbPO.Text);
                                        clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                                        clase.poItem = "77";
                                        clase.ProductCode = x[0].estilo;
                                        clase.Size = x[0].Talla;
                                        clase.size_izquierdo = separadas[0].ToString();
                                        clase.cn_tag_num = Convert.ToInt32(x[0].NumeroCaja);
                                        clase.CARTON_NUMBER_INICIAL = (x[0].NumeroCaja).ToString();
                                        clase.size_derecho = "";
                                        clase.TipoCarton = "0";
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
                                        clase.DESCRIPTION = x[0].itemDescription + " COLOR:" + x[0].color + " SIZE:" + x[0].Talla;
                                        clase.QUANTITY = Convert.ToString(x[0].cantidad);                                        
                                        clase.CARTON_NUMBER_FINAL = Convert.ToString(x[0].totalCajasPO);
                                        clase.COUNTRY = "MEXICO";

                                        clase.id_Inventario = f.GuardaInventarioZumies(clase, usu[0].id);

                                        txtCartonNumber.Text = (x[0].NumeroCaja).ToString();


                                        id_InventarioAnt = clase.id_Inventario;
                                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                        try
                                        {
                                            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(id_InventarioAnt);
                                            if (x1.Count > 0)
                                            {


                                                List<ConsultaProductosZumiesResult> x2 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

                                                /***/
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
                                                    txtUnitsScan.Text = "0";
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
                                                claseZ.poInCompleto = x1[0].poInCompleto;
                                                claseZ.poItem = "77";
                                                claseZ.ProductCode = x1[0].ProductCode;
                                                claseZ.Size = x1[0].size_izquierdo;
                                                claseZ.ESTILO = x1[0].ProductCode;
                                                claseZ.DESCRIPTION = x1[0].TipoCarton;
                                                claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                                                claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                                claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                                                claseZ.CARTON_NUMBER_FINAL = clase.CARTON_NUMBER_FINAL;
                                                claseZ.COUNTRY = "MEXICO";

                                                List<EtiquetaZUMIES> listclaseZ = new List<EtiquetaZUMIES>();
                                                listclaseZ.Add(claseZ);
                                                id_InventarioAnt = claseZ.id_Inventario;

                                                ReporteCajaZumines report = new ReporteCajaZumines
                                                {
                                                    DataSource = listclaseZ
                                                };
                                                report.PrintingSystem.ShowMarginsWarning = false;
                                                ReportPrintTool tool = new ReportPrintTool(report);
                                                tool.Print(); //imprime de golpe
                                            }
                                            LimpiarPantallaEscaneo();

                                            contador = 0;
                                            txtUPCScann.Text = string.Empty;


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
                                         MessageBox.Show("Ya se escanearon todas las cajas de la Talla");
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



                        if (cmbPOItem.Text == "66")/*ROCKY*/
                        {
                            #region ROCKY
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
                                        clase.po = Convert.ToDecimal(cmbPO.Text);
                                        clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                                        clase.poItem = "77";
                                        clase.ProductCode = x[0].estilo;
                                        clase.Size = x[0].Talla;
                                        clase.size_izquierdo = separadas[0].ToString();
                                        clase.cn_tag_num = Convert.ToInt32(x[0].NumeroCaja);
                                        clase.CARTON_NUMBER_INICIAL = (x[0].NumeroCaja).ToString();
                                        clase.size_derecho = "";
                                        clase.TipoCarton = "0";
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
                                        clase.DESCRIPTION = x[0].itemDescription + " COLOR:" + x[0].color + " SIZE:" + x[0].Talla;
                                        clase.QUANTITY = Convert.ToString(x[0].cantidad);
                                        clase.CARTON_NUMBER_FINAL = "___";
                                        clase.COUNTRY = "MEXICO";

                                        clase.id_Inventario = f.GuardaInventarioZumies(clase, usu[0].id);

                                        txtCartonNumber.Text = (x[0].NumeroCaja).ToString();


                                        id_InventarioAnt = clase.id_Inventario;
                                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                        try
                                        {
                                            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(id_InventarioAnt);
                                            if (x1.Count > 0)
                                            {


                                                List<ConsultaProductosZumiesResult> x2 = f.ConsultaProductosZumies(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

                                                /***/
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
                                                    txtUnitsScan.Text = "0";
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
                                                claseZ.poInCompleto = x1[0].poInCompleto;
                                                claseZ.poItem = "77";
                                                claseZ.ProductCode = x1[0].ProductCode;
                                                claseZ.Size = x1[0].size_izquierdo;
                                                claseZ.ESTILO = x1[0].ProductCode;
                                                claseZ.DESCRIPTION = x1[0].TipoCarton;
                                                claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                                                claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                                claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                                                claseZ.CARTON_NUMBER_FINAL = "___";
                                                claseZ.COUNTRY = "MEXICO";

                                                List<EtiquetaZUMIES> listclaseZ = new List<EtiquetaZUMIES>();
                                                listclaseZ.Add(claseZ);
                                                id_InventarioAnt = claseZ.id_Inventario;

                                                ReporteCajaZumines report = new ReporteCajaZumines
                                                {
                                                    DataSource = listclaseZ
                                                };
                                                report.PrintingSystem.ShowMarginsWarning = false;
                                                ReportPrintTool tool = new ReportPrintTool(report);
                                                tool.Print(); //imprime de golpe
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

                        else if (cmbPOItem.Text == "88") /*------------------------------TARGETS -----------------------------------------TARGET-------------------TARGET-------------------------TARGET-----------TARGET--------------------------------------------TARGET------NUEVO*/
                        {
                            # region TARGETS
                            #region TOTAL DE PRENDAS ESCANEADAS
                            if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) - 1)
                            {
                                try
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
                                    txtUPCScann.Focus();
                                    ListUpc = f.ConsultaUPC(po_numero, upc);
                                    id = ListUpc[0].id;
                                    upc = ListUpc[0].upc;

                                    List<ConsultaProductosTargetResult> x = f.ConsultaProductosTarget(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

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
                                        clase.size_izquierdo = ListUpc[0].Size.ToString();
                                        clase.size_derecho = "";
                                        clase.TipoCarton = "0";
                                        clase.upc = x[0].upc;
                                        clase.Fecha = DateTime.Now;
                                        clase.CartonLeft = "";
                                        clase.CartonRight = "";
                                        clase.Cantidad = Convert.ToDecimal(x[0].cantidad);
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
                                        clase.QUANTITY = Convert.ToString(x[0].cantidad);
                                        clase.COUNTRY = "MEXICO";
                                        clase.po = Convert.ToDecimal(cmbPO.Text);
                                        clase.id_Inventario = f.GuardaInventarioZumies(clase, usu[0].id);

                                        id_InventarioAnt = clase.id_Inventario;
                                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                        try
                                        {
                                            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(id_InventarioAnt);
                                            if (x1.Count > 0)
                                            {
                                                List<ConsultaProductosTargetResult> x2 = f.ConsultaProductosTarget(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());

                                                /***/
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
                                                    cantidad = x2[0].cantidad.ToString();
                                                    txtUnitsReq.Text = x2[0].cantidad.ToString();
                                                    txtUnitsRemai.Text = cantidad.ToString();
                                                    txtUnitsScan.Text = "0";
                                                    txtUPCScann.Focus();
                                                }
                                                iniciando = true;
                                                f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
                                                cmbPOItem.SelectedIndex = 0;
                                                f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                                                cmbProductCode.SelectedIndex = 0;
                                                f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                                                cmbSizes.SelectedText = ListUpc[0].Size;
                                                contador = 0;
                                                EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                                                claseZ.id = x1[0].id;
                                                claseZ.assembly = x1[0].Assembly;
                                                claseZ.poInCompleto = x1[0].poInCompleto;
                                                claseZ.poItem = "88";
                                                claseZ.ProductCode = x1[0].ProductCode;
                                                claseZ.Size = x1[0].size_izquierdo;
                                                claseZ.ESTILO = x1[0].ProductCode;
                                                claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                                                claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                                claseZ.CARTON_NUMBER_INICIAL = id_InventarioAnt.ToString();
                                                claseZ.CARTON_NUMBER_FINAL = "___";
                                                claseZ.COUNTRY = "MEXICO";
                                                claseZ.Carton = id_InventarioAnt;
                                                claseZ.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                                claseZ.DPCI = x1[0].TipoCarton;
                                                claseZ.itemDescription = claseZ.DPCI;
                                                claseZ.color = "";
                                                claseZ.size_izquierdo = ListUpc[0].Size.ToString();
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
                                                id_InventarioAnt = Convert.ToInt32(clase.id);
                                                ReportCajaTarget report = new ReportCajaTarget
                                                {
                                                    DataSource = listClase
                                                };
                                                // Disable margins warning. 
                                                report.PrintingSystem.ShowMarginsWarning = false;
                                                ReportPrintTool tool = new ReportPrintTool(report);

                                                ////tool.ShowPreview();
                                                ////tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                                tool.Print(); //imprime de golpe



                                                /********************************************************************************IMPRIMIR ETIQUETA INTERNA************************************************************************************************/

                                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                                List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                                                EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                                                clase2.id_Inventario = Convert.ToInt32(claseZ.id);
                                                clase2.po = clase.po;
                                                clase2.poInCompleto = clase.po;
                                                clase2.cliente = "";
                                                clase2.factura = "";
                                                clase2.terminado = "";
                                                clase2.usuario = usu[0].nombre;
                                                clase2.ProductCode = clase.ProductCode;
                                                clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                                clase2.Size = claseZ.size_izquierdo;
                                                clase2.Fecha = clase.Fecha;
                                                clase2.assembly = "*" + clase.id_Inventario.ToString() + "*";

                                                QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                                                Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
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
                                            LimpiarPantallaEscaneo();

                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message.ToString());
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Ya se escanearon todas las cajas de la Talla");
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
                                    if (txtUnitsScan.Text == "0" && cmbPOItem.Text != "99" && cmbPOItem.Text != "1000")
                                    {
                                        ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                                        id = ListUpc[0].id;
                                        upc = ListUpc[0].upc;
                                        List<ConsultaProductosTargetResult> x2 = f.ConsultaProductosTarget(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, ListUpc[0].Size.ToString());
                                        if (x2.Count > 0)
                                        {
                                            dgvEscan.DataSource = x2;
                                        }
                                    }
                                    /***/
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
                            #region TARGET
                            //clase.id_Inventario = 116;
                            //cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                            //BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                            //{
                            //    IncludeLabel = true
                            //};
                            ////"0196365820190"
                            //clase.Carton = Convert.ToInt64(id);
                            //string contarDigitos = "00" + clase.upc.Substring(0, 11) + "1";


                            //Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 200, 100);
                            ////Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, clase.id_Inventario.ToString(), Color.Black, Color.White, 200, 100);

                            //clase.codigoBarras = codigoBarras;
                            //listClase.Add(clase);
                            //id_InventarioAnt = Convert.ToInt32(clase.id);
                            //ReportCajaTarget report = new ReportCajaTarget
                            //{
                            //    DataSource = listClase
                            //};
                            //// Disable margins warning. 
                            //report.PrintingSystem.ShowMarginsWarning = false;
                            //ReportPrintTool tool = new ReportPrintTool(report);
                            ////tool.ShowPreview();
                            ////tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                            ////tool.PrintDialog(); //muestra a que impresora se va a mandar
                            //tool.Print(); //imprime de golpe
                            //LimpiarPantallaEscaneo();
                            #endregion
                            #endregion
                        }
                        if (upc == txtUPCScann.Text)
                        {
                            #region ESCANEANDO X PRENDA AUN TODAS LAS MARCAS
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

                            if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text))

                            {
                                #region ESCANEANDO COMPLETO
                                List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);
                                EtiquetaCajaModificada clase = RellenaObjetoClase(consulta);

                                List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();

                                if ((clase.poItem != "1000" && clase.poItem != "99") || (clase.poItem == "1000" && clase.Carton != 0 && clase.poItem != "99"))
                                {
                                    #region INVENTARIO VIEJO
                                    clase.assembly = consulta[0].Assembly;
                                    clase.Vendor = consulta[0].Vendor;
                                    clase.ShipTo = consulta[0].ShipTo;
                                    // clase.color = 
                                    //clase.itemDescription = 

                                    clase.id_Inventario = f.GuardaInventario(clase, usu[0].id);

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
                                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                                        {
                                            IncludeLabel = true
                                        };
                                        Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, clase.id_Inventario.ToString(), Color.Black, Color.White, 200, 100);
                                        clase.qr = qrCode.GetGraphic(20);
                                        clase.codigoBarras = codigoBarras;
                                        clase.po = Convert.ToDecimal(cmbPO.Text);
                                        listClase.Add(clase);
                                        id_InventarioAnt = clase.id_Inventario;

                                        ReporteCaja report = new ReporteCaja
                                        {
                                            DataSource = listClase
                                        };
                                        // Disable margins warning. 
                                        report.PrintingSystem.ShowMarginsWarning = false;
                                        ReportPrintTool tool = new ReportPrintTool(report);
                                        //tool.ShowPreview();
                                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                        //tool.PrintDialog(); //muestra a que impresora se va a mandar
                                        tool.Print(); //imprime de golpe
                                        LimpiarPantallaEscaneo();
                                        #endregion
                                    }

                                    else if (clase.poItem == "1000") /*CINTAS*/
                                    {
                                        #region CINTAS CARTON

                                        do
                                        {
                                            try
                                            {
                                                clase.Carton = Convert.ToInt64(Interaction.InputBox("Captura Numero de Carton", "Carton", "", 5, 5));

                                            }
                                            catch (Exception)
                                            { //clase.Carton = 0; MessageBox.Show("Favor de ingresar correctamente el numero de carton ya que no se ha guardado");
                                            };

                                        } while (clase.Carton == 0 || clase.Carton.ToString().Length > 10);

                                        #endregion

                                        #region CINTAS
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
                                            catch (Exception)
                                            {
                                                clase.upc = consulta[0].upc;
                                            }
                                        }
                                        cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                                        {
                                            IncludeLabel = true,
                                            RotateFlipType = RotateFlipType.Rotate90FlipY
                                        };
                                        Image codigoBarras =
                                        Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_Inventario.ToString(), Color.Black, Color.White, 270, 180);
                                        // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                                        clase.codigoBarras = codigoBarras;
                                        listClase.Add(clase);
                                        id_InventarioAnt = clase.id_Inventario;

                                        ReporteCintas report = new ReporteCintas
                                        {
                                            DataSource = listClase
                                        };
                                        // Disable margins warning. 
                                        report.PrintingSystem.ShowMarginsWarning = false;
                                        ReportPrintTool tool = new ReportPrintTool(report);
                                        LimpiarPantallaEscaneo();
                                        #endregion

                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        if (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) && txtUPCScann.Text.ToUpper() == upc)
                        {
                            #region ESCANEANDO COMPLETO
                            contador = 0;
                            txtUPCScann.Text = string.Empty;
                            txtUnitsReq.Text = cantidad.ToString();
                            //txtUnitsScan.Text = "0";
                            txtUnitsRemai.Text = cantidad.ToString();
                            dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
                            dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
                            txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                            txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                            txtUPCScann.Focus();
                            LimpiarPantallaEscaneo();
                            #endregion
                        }
                    }
                    else
                    {
                        sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                        sonido.controls.play();
                        txtUPCScann.Text = string.Empty;
                        tpEscan.BackColor = Color.Red;

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
                if (TabEsaneo.SelectedTab.Text.Trim() == "BajaPO")
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
                if (TabEsaneo.SelectedTab.Text.Trim() == "AltaPrePack")
                {
                    if (Tabpage == 0)
                    {
                        Tabpage = Tabpage + 1;
                        tablaPrepack = new DataTable();
                        tablaPrepack.Columns.Add("Talla", typeof(string));
                        tablaPrepack.Columns.Add("Cantidad", typeof(long));
                        tablaPrepack.Columns.Add("Codigo UPC", typeof(string));
                        tablaPrepack.Columns.Add("DPCI", typeof(string));
                        tablaPrepack.Columns.Add("idSize", typeof(int));
                        contadorVecesCMBPO = 0;

                    }
                    else
                    {

                    }
                }
                if (TabEsaneo.SelectedTab.Text.Trim() == "AltaPO")
                {
                    if (Tabpage == 0)
                    {                           // --------- ERROR SELECTED INDEX -----------
                        iniciando = true;
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



                        tablaTarget = new DataTable();
                        tablaTarget.Columns.Add("Talla", typeof(string));
                        tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                        tablaTarget.Columns.Add("idSize", typeof(int));
                        tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                        tablaTarget.Columns.Add("itemDescription", typeof(string));
                        tablaTarget.Columns.Add("color", typeof(string));


                        Tabpage = Tabpage + 1;


                        cmbMarca.SelectedIndex = 0;
                        iniciando = false;

                    }
                    else
                    {

                    }
                }
                if (TabEsaneo.SelectedTab.Text.Trim() == "ModificarPO")
                {
                    if (Tabpage == 0)
                    {                           // --------- ERROR SELECTED INDEX -----------

                        tablaModificacion = new DataTable();
                        tablaModificacion.Columns.Add("Talla", typeof(string));
                        tablaModificacion.Columns.Add("Cantidad", typeof(long));
                        tablaModificacion.Columns.Add("Codigo UPC", typeof(string));
                        tablaModificacion.Columns.Add("idSize", typeof(string));
                        Tabpage = Tabpage + 1;


                        cmbMarca.SelectedIndex = 0;
                        iniciando = false;

                    }
                    else
                    {

                    }
                }
                if (TabEsaneo.SelectedTab.Text.Trim() == "AltaZumies")
                {
                    if (Tabpage == 0)
                    {
                        tablaZumies = new DataTable();
                        tablaZumies.Columns.Add("Talla", typeof(string));
                        tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                        tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                        tablaZumies.Columns.Add("idSize", typeof(int));
                        tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                        tablaZumies.Columns.Add("itemDescription", typeof(string));
                        tablaZumies.Columns.Add("color", typeof(string));
                        Tabpage = Tabpage + 1;
                    }
                    else
                    {

                    }
                }
                if (TabEsaneo.SelectedTab.Text.Trim() == "Escaneo")
                {
                    contadorVecesCMBPO = 0;
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
                if (TabEsaneo.SelectedTab.Text.Trim() == "BajaCaja")
                {
                    if (usu[0].perfil == "1" || usu[0].perfil == "4")
                    {
                        txtCaja.Enabled = true;
                        btnBajaCaja.Enabled = true;
                    }
                }
                if (TabEsaneo.SelectedTab.Text.Trim() == "ABC")
                {
                    if (usu[0].perfil == "1" || usu[0].perfil == "4")
                    {
                        //btnNuevo.Enabled = true;
                        ////btnEliminar.Enabled = true;
                        ////btnEditar.Enabled = true;
                        ////btnGuardarA.Enabled = true;
                        //btnVistaPrevia.Enabled = true;
                        ////
                        //txtCantidadA.Enabled = true;
                        //txtPoNA.Enabled = true;
                        //txtUPCA.Enabled = true;
                        //// txtPoItemNA.Enabled = true;
                        //cmbTipoCajaA.Enabled = true;
                        ////txtPCA.Enabled = true;
                        //cmbSizeA.Enabled = true;
                        //txtID.Enabled = false;
                    }
                    else
                    {
                        //btnNuevo.Enabled = false;
                        //btnEliminar.Enabled = false;
                        //btnEditar.Enabled = false;
                        //btnGuardarA.Enabled = false;
                        //btnVistaPrevia.Enabled = false;
                        ////
                        //txtCantidadA.Enabled = false;
                        //txtPoNA.Enabled = false;
                        //txtUPCA.Enabled = false;
                        ////txtPoItemNA.Enabled = false;
                        //cmbTipoCajaA.Enabled = false;
                        ////txtPCA.Enabled = false;
                        //cmbSizeA.Enabled = false;
                        //txtID.Enabled = false;

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
                if (cmbPOItem.Text != "99" && cmbPOItem.Text != "77")
                {
                    if (id_InventarioAnt != 0 && Convert.ToInt32(txtCartonsPacked.Text) > 0)
                    {
                        txtUPCScann.Text = string.Empty;
                        txtUPCScann.Focus();
                        List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(id_InventarioAnt);
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();

                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                        {
                            po = consulta[0].po,
                            poInCompleto = consulta[0].poInCompleto,
                            poItem = consulta[0].poItem,
                            ProductCode = consulta[0].ProductCode,
                            Size = consulta[0].Size,
                            size_derecho = consulta[0].size_derecho,
                            size_izquierdo = consulta[0].size_izquierdo,
                            TipoCarton = consulta[0].TipoCarton,
                            upc = consulta[0].upc,
                            Fecha = consulta[0].create_dtm,
                            CartonLeft = consulta[0].CartonLeft,
                            CartonRight = consulta[0].CartonRight,
                            Cantidad = consulta[0].Cantidad,
                            Carton = consulta[0].Carton,
                            usuario = consulta[0].usuario,
                            id_Inventario = consulta[0].id,
                            id_cliente = Convert.ToInt32(consulta[0].id_cliente),
                            id_factura = Convert.ToInt32(consulta[0].id_factura),
                            id_terminado = Convert.ToInt32(consulta[0].id_terminado),
                            cliente = consulta[0].cliente,
                            factura = consulta[0].factura,
                            terminado = consulta[0].terminado
                        };
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

                            cantidadAnterior = Convert.ToInt32(consulta[0].Cantidad);
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                            {
                                IncludeLabel = true,
                                RotateFlipType = RotateFlipType.Rotate90FlipY
                            };
                            clase.assembly = consulta[0].Assembly;
                            clase.Vendor = consulta[0].Vendor;
                            clase.ShipTo = consulta[0].ShipTo;
                            Image codigoBarras =
                            Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.Carton.ToString(), Color.Black, Color.White, 270, 180);
                            // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                            clase.codigoBarras = codigoBarras;
                            listClase.Add(clase);
                            id_InventarioAnt = clase.id_Inventario;

                            ReporteCintas report = new ReporteCintas
                            {
                                DataSource = listClase
                            };
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
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                            {
                                IncludeLabel = true
                            };
                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, consulta[0].id.ToString(), Color.Black, Color.White, 200, 100);
                            clase.qr = qrCode.GetGraphic(20);
                            clase.codigoBarras = codigoBarras;
                            listClase.Add(clase);
                            ReporteCaja report = new ReporteCaja
                            {
                                DataSource = listClase
                            };
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
                else if (cmbPOItem.Text == "77")
                {
                    if (id_InventarioAnt != 0 && Convert.ToInt32(txtCartonsPacked.Text) > 0)
                    {
                        txtUPCScann.Text = string.Empty;
                        txtUPCScann.Focus();
                        List<ConsultaInventarioIDResult> x = f.ConsultaInventarioID(id_InventarioAnt);
                        if (x.Count > 0)
                        {
                            EtiquetaZUMIES clase = new EtiquetaZUMIES
                            {
                                id = x[0].id,
                                po = x[0].po,
                                poInCompleto = x[0].poInCompleto,
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
                else if (cmbPOItem.Text == "88")
                {
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(id_InventarioAnt);
                    if (x1.Count > 0)
                    {

                        contador = 0;
                        EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                        claseZ.poInCompleto = x1[0].poInCompleto;
                        claseZ.poItem = "88";
                        claseZ.Size = x1[0].size_izquierdo;
                        claseZ.Carton = id_InventarioAnt;
                        claseZ.id = id_InventarioAnt;
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
                        //id_InventarioAnt = Convert.ToInt32(idInv);
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
                        clase2.id_Inventario = Convert.ToInt32(claseZ.id);
                        clase2.po = x1[0].po;
                        clase2.poInCompleto = x1[0].po;
                        clase2.cliente = "";
                        clase2.factura = "";
                        clase2.terminado = "";
                        clase2.usuario = usu[0].nombre;
                        clase2.ProductCode = claseZ.ProductCode;
                        clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                        clase2.Size = claseZ.size_izquierdo;
                        clase2.Fecha = x1[0].create_dtm;
                        clase2.assembly = "*" + claseZ.id.ToString() + "*";

                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                        Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
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

            PrintableComponentLink link = new PrintableComponentLink(new PrintingSystem())
            {
                Component = grid,

                Landscape = true
            };

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
                try { idInv = Convert.ToInt32(txtIDReImpresion.Text); } catch (Exception) { idInv = 0; }
                List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(idInv);
                if (consulta.Count > 0)
                {
                    if (consulta[0].poItem != "77" && consulta[0].poItem != "88")
                    {

                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                        {
                            poInCompleto = consulta[0].poInCompleto,
                            po = consulta[0].po,
                            poItem = consulta[0].poItem,
                            ProductCode = consulta[0].ProductCode,
                            Size = consulta[0].Size,
                            size_derecho = consulta[0].size_derecho,
                            size_izquierdo = consulta[0].size_izquierdo,
                            TipoCarton = consulta[0].TipoCarton,
                            upc = consulta[0].upc,
                            Fecha = consulta[0].create_dtm,
                            CartonLeft = consulta[0].CartonLeft,
                            CartonRight = consulta[0].CartonRight,
                            Cantidad = consulta[0].Cantidad,
                            Carton = consulta[0].Carton,
                            usuario = consulta[0].usuario,
                            id_Inventario = consulta[0].id,
                            id_cliente = Convert.ToInt32(consulta[0].id_cliente),
                            id_factura = Convert.ToInt32(consulta[0].id_factura),
                            id_terminado = Convert.ToInt32(consulta[0].id_terminado),
                            cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente,
                            factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura,
                            terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado
                        };

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
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                            {
                                IncludeLabel = true
                            };
                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                               , consulta[0].id.ToString()
                                                               , Color.Black
                                                               , Color.White, 200, 100);

                            clase.qr = qrCode.GetGraphic(20);
                            clase.codigoBarras = codigoBarras;


                            listClase.Add(clase);
                            ReporteCaja report = new ReporteCaja
                            {
                                DataSource = listClase
                            };
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
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
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                            {
                                IncludeLabel = true,
                                RotateFlipType = RotateFlipType.Rotate90FlipY
                            };
                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.Carton.ToString(), Color.Black, Color.White, 270, 180);
                            clase.codigoBarras = codigoBarras;
                            listClase.Add(clase);
                            id_InventarioAnt = clase.id_Inventario;
                            ReporteCintas report = new ReporteCintas
                            {
                                DataSource = listClase
                            };
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            tool.Print(); //imprime de golpe
                        }
                        if (cbLimpiar.Checked == true)
                        {
                            txtIDReImpresion.Text = string.Empty;
                            txtIDReImpresion.Focus();
                        }
                    }
                    else if (consulta[0].poItem == "77")
                    {
                        /*zumies*/
                        List<ConsultaInventarioIDResult> x = f.ConsultaInventarioID(idInv);
                        if (x.Count > 0)
                        {
                            EtiquetaZUMIES clase = new EtiquetaZUMIES();
                            clase.id = x[0].id;
                            clase.po = x[0].po;
                            clase.poInCompleto = x[0].poInCompleto;
                            clase.poItem = "77";
                            clase.ProductCode = x[0].ProductCode;
                            clase.Size = x[0].size_izquierdo;
                            clase.ESTILO = x[0].ProductCode;
                            clase.DESCRIPTION = x[0].TipoCarton;
                            clase.QUANTITY = Convert.ToString(x[0].Cantidad);
                            clase.cn_tag_num = Convert.ToInt32(x[0].Carton);
                            clase.CARTON_NUMBER_INICIAL = x[0].Carton.ToString();
                            clase.CARTON_NUMBER_FINAL = Convert.ToString(x[0].TotalCajas);
                            clase.COUNTRY = "MEXICO";

                            List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                            listClase.Add(clase);
                            id_InventarioAnt = clase.id_Inventario;

                            ReporteCajaZumines report = new ReporteCajaZumines
                            {
                                DataSource = listClase
                            };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            tool.Print(); //imprime de golpe

                        }
                    }
                    else if (consulta[0].poItem == "88" )
                    {
                        /*target*/
                        try
                        {
                            List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(idInv);

                            if (x1.Count > 0)
                            {
                                int? esPrepack = f.ConsultaPrepack(x1[0].po.ToString());

                                if (esPrepack > 0)
                                {
                                        List<ConsultaPrepackDetalleResult> ListprepackTarget = f.ConsultaPrepackDetalle(Convert.ToInt32(esPrepack));
                                        if (ListprepackTarget.Count > 0)
                                        {
                                            decimal? cantidadTotalCajasPrepack = 0;
                                            string barcode = "";
                                            int? idPrepack = 0;
                                            string DPCI = "";
                                            EtiquetaZUMIES clase = new EtiquetaZUMIES();
                                            foreach (ConsultaPrepackDetalleResult objPrepack in ListprepackTarget)
                                            {
                                                cantidadTotalCajasPrepack = objPrepack.cantidad + cantidadTotalCajasPrepack;
                                                barcode = objPrepack.barcode;
                                                idPrepack = objPrepack.idPrepack;
                                                DPCI = objPrepack.DPCI;
                                            }


                                            EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                                            claseZ.id = idInv;
                                            claseZ.assembly = consulta[0].po.ToString();
                                            claseZ.poInCompleto = consulta[0].poInCompleto;
                                            claseZ.poItem = "88";
                                            claseZ.ProductCode = "ASSORTMENT"; 
                                            claseZ.Size = "ASSORTMENT";
                                            claseZ.ESTILO = "ASSORTMENT";
                                            claseZ.QUANTITY = Convert.ToString(cantidadTotalCajasPrepack);
                                            claseZ.CARTON_NUMBER_INICIAL = idInv.ToString();
                                            claseZ.CARTON_NUMBER_FINAL = "___";
                                            claseZ.COUNTRY = "MEXICO";
                                            claseZ.Cantidad = Convert.ToDecimal(cantidadTotalCajasPrepack);
                                            claseZ.DPCI = DPCI.Trim();
                                            claseZ.itemDescription = claseZ.DPCI;
                                            claseZ.cn_tag_num = Convert.ToInt32(idInv);
                                            claseZ.color = "";
                                            claseZ.size_izquierdo = "ASSORTMENT";
                                            string contarDigitos = "00" + barcode.Substring(0, 11) + "1";
                                            claseZ.Carton = idInv;
                                            //en ASSORTMENT NO ES EL UPC ES UNO QUE ENGLOBA TODAS LAS TALLAS

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
                                           

                                            //////********************************************************imprime carton interno**************************************************************************************************************************////

                                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                            List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                                            EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                                            clase2.id_Inventario = Convert.ToInt32(claseZ.id);
                                            clase2.po = Convert.ToDecimal(consulta[0].po.ToString());
                                            clase2.poInCompleto = consulta[0].po;
                                            clase2.cliente = "";
                                            clase2.factura = "";
                                            clase2.terminado = "";
                                            clase2.usuario = usu[0].nombre;
                                            clase2.ProductCode = claseZ.ProductCode;
                                            clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                            clase2.Size = "ASSORTMENT";
                                            clase2.Fecha = consulta[0].create_dtm;
                                            clase2.assembly = "*" + claseZ.id.ToString() + "*";
                                            clase2.size_izquierdo = clase2.Size;

                                            QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                                            Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
                                            clase2.qr = qrCode.GetGraphic(20);
                                            clase2.codigoBarras = codigoBarras2;
                                            listClase2.Add(clase2);



                                        ReporteCajaTargetAssorment report = new ReporteCajaTargetAssorment
                                        {
                                            DataSource = listClase
                                        };
                                        // Disable margins warning. 
                                        report.PrintingSystem.ShowMarginsWarning = false;
                                        ReportPrintTool tool = new ReportPrintTool(report);

                                        tool.Print(); //imprime de golpe



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
                                    contador = 0;
                                    EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                                    claseZ.poInCompleto = x1[0].poInCompleto;
                                    claseZ.poItem = "88";
                                    claseZ.Size = x1[0].size_izquierdo;
                                    claseZ.Carton = idInv;
                                    claseZ.id = idInv;
                                    claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                                    claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                                    claseZ.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                    if (x1[0].TipoCarton.Contains('/'))
                                    {
                                        x1[0].TipoCarton = (x1[0].TipoCarton.Replace('/', '-')).Trim();
                                    }
                                    else
                                    {

                                    }

                                    claseZ.DPCI = x1[0].TipoCarton.Trim();
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
                                    //id_InventarioAnt = Convert.ToInt32(idInv);
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
                                    clase2.id_Inventario = Convert.ToInt32(claseZ.id);
                                    clase2.po = x1[0].po;
                                    clase2.poInCompleto = x1[0].po;
                                    clase2.cliente = "";
                                    clase2.factura = "";
                                    clase2.terminado = "";
                                    clase2.usuario = usu[0].nombre;
                                    clase2.ProductCode = claseZ.ProductCode;
                                    clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                    clase2.Size = claseZ.size_izquierdo;
                                    clase2.Fecha = x1[0].create_dtm;
                                    clase2.assembly = "*" + claseZ.id.ToString() + "*";

                                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                                    Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
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
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
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
                //MessageBox.Show(ex.Message);
            }
        }

        private void btnBajaPO_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja el PO " + cmbPOB.Text + " " + cmbClienteB.Text + " " + cmbFacturacionB.Text + " " + cmbTerminadoB.Text, "Baja PO", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");

                    if (password == "Ain2022_")
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
            //    clase.id_Inventario = f.GuardaInventario(clase, usu[0].id);


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

        private void txtIDReImpresion_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    int idInv = 0;
                    try { idInv = Convert.ToInt32(txtIDReImpresion.Text); } catch (Exception) { idInv = 0; }

                    List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(idInv);
                    if (consulta.Count > 0)
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();

                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                        {
                            poInCompleto = consulta[0].poInCompleto,
                            po = consulta[0].po,
                            poItem = consulta[0].poItem,
                            ProductCode = consulta[0].ProductCode,
                            Size = consulta[0].Size,
                            size_derecho = consulta[0].size_derecho,
                            size_izquierdo = consulta[0].size_izquierdo,
                            TipoCarton = consulta[0].TipoCarton,
                            upc = consulta[0].upc,
                            Fecha = consulta[0].create_dtm,
                            CartonLeft = consulta[0].CartonLeft,
                            CartonRight = consulta[0].CartonRight,
                            Cantidad = consulta[0].Cantidad,
                            Carton = consulta[0].Carton,
                            usuario = consulta[0].usuario,
                            id_Inventario = consulta[0].id,
                            id_cliente = Convert.ToInt32(consulta[0].id_cliente),
                            id_factura = Convert.ToInt32(consulta[0].id_factura),
                            id_terminado = Convert.ToInt32(consulta[0].id_terminado),
                            cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente,
                            factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura,
                            terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado
                        };
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
                        BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                        {
                            IncludeLabel = true
                        };
                        Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                           , consulta[0].id.ToString()
                                                           , Color.Black
                                                           , Color.White, 200, 100);

                        clase.qr = qrCode.GetGraphic(20);
                        clase.codigoBarras = codigoBarras;


                        listClase.Add(clase);
                        ReporteCaja report = new ReporteCaja
                        {
                            DataSource = listClase
                        };
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
                if (e.KeyChar == (int)Keys.Enter)
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
                    EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada
                    {
                        poInCompleto = consulta2[0].poInCompleto,
                        po = consulta2[0].po,
                        poItem = consulta2[0].poItem,
                        ProductCode = consulta2[0].ProductCode,
                        Size = consulta2[0].Size,
                        size_derecho = consulta2[0].size_derecho,
                        size_izquierdo = consulta2[0].size_izquierdo,
                        TipoCarton = consulta2[0].TipoCarton,
                        upc = consulta2[0].upc,
                        Fecha = consulta2[0].create_dtm,
                        CartonLeft = consulta2[0].CartonLeft,
                        CartonRight = consulta2[0].CartonRight,
                        Cantidad = consulta2[0].Cantidad,
                        Carton = consulta2[0].Carton,
                        usuario = consulta2[0].usuario,
                        id_Inventario = consulta2[0].id,
                        id_cliente = Convert.ToInt32(consulta2[0].id_cliente),
                        id_factura = Convert.ToInt32(consulta2[0].id_factura),
                        id_terminado = Convert.ToInt32(consulta2[0].id_terminado),
                        cliente = consulta2[0].cliente == string.Empty ? "NA" : consulta2[0].cliente,
                        factura = consulta2[0].factura == string.Empty ? "NA" : consulta2[0].factura,
                        terminado = consulta2[0].terminado == string.Empty ? "NA" : consulta2[0].terminado
                    };
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
                    BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                    {
                        IncludeLabel = true
                    };
                    Image codigoBarras2 = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                       , consulta2[0].id.ToString()
                                                       , Color.Black
                                                       , Color.White, 200, 100);

                    clase2.qr = qrCode.GetGraphic(20);
                    clase2.codigoBarras = codigoBarras2;


                    listclase2.Add(clase2);
                    ReporteCaja report2 = new ReporteCaja
                    {
                        DataSource = listclase2
                    };
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
                if (e.KeyChar == (int)Keys.Enter)
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
            //if (txtID.Text != "0")
            //{

            //}
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            //if (txtID.Text != "0")
            //{

            //}
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
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

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
                            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                            {
                                po = Convert.ToInt64(dr[0].ToString()),
                                poInCompleto = Convert.ToInt64(dr[0].ToString().PadRight(6)),
                                poItem = null,
                                ProductCode = dr[9].ToString()
                            };
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
                            clase.id_Inventario = f.GuardaProducto(clase, usu[0].id);
                        }
                        catch (Exception)
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

            f.ConsultaPO(cmbPO);
            cmbPO.SelectedIndex = -1;
            f.ConsultaPO(cmbPOB);
            MessageBox.Show("Termino con Exito!");
        }

        private void btnReporteDiario_Click(object sender, EventArgs e)
        {
            List<ConsultaInventarioPorHoraResult> inv = f.ConsultaInventarioPorHora(dtpReporteDiario.Value.Date, dtpReporteDiario.Value.Date);

            ReporteDiario report2 = new ReporteDiario
            {
                DataSource = inv
            };
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
                if (e.KeyChar == (int)Keys.Enter)
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

        private void btnEditar_Click_1(object sender, EventArgs e)
        {

        }


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
                        }
                        p.po_numero = Convert.ToDecimal(txtPoPrepack.Text);

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
            /*
            if (!iniciando)
            {
                iniciando = true;
                f.ConsultaPOItem(cmbPOItem, po_numero);
                cmbPOItem.SelectedIndex = 0;
                f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
                cmbProductCode.SelectedIndex = 0;
                f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
                iniciando = false;
            }
            */
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

                f.ConsultaPO(cmbPO);
                cmbPO.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void importarLEVIS()
        {

            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

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
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                                {
                                    po = Convert.ToInt64(dr[0].ToString()),
                                    poInCompleto = 0,
                                    poItem = null,
                                    ProductCode = "",
                                    Size = "",
                                    size_izquierdo = dr[1].ToString(),
                                    size_derecho = dr[2].ToString(),
                                    TipoCarton = null,
                                    assembly = "",
                                    Vendor = "",
                                    ShipTo = ""
                                };
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
                                clase.id_Inventario = f.GuardaProductoLEVIS(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
        }


        public void importarROCKY()
        {

            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

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
                                clase.poInCompleto = clase.po;
                                clase.poItem = "77";
                                clase.ProductCode = "";
                                clase.Size = "";
                                clase.size_izquierdo = dr[1].ToString();
                                clase.size_derecho = dr[2].ToString();
                                clase.TipoCarton = null;
                                clase.assembly = "";
                                clase.Vendor = "";
                                clase.ShipTo = "";
                                clase.upc = upc;
                                clase.Fecha = DateTime.Now;
                                clase.CartonLeft = "";
                                clase.CartonRight = "";
                                clase.Cantidad = Convert.ToInt64(dr[4].ToString());
                                clase.Carton = null;
                                clase.usuario = usu[0].nombre;
                                clase.id_Inventario = f.GuardaProductoLEVIS(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
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
                if (string.IsNullOrEmpty(txtzPOZumies.Text) && dgvZumies.RowCount < 1)
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
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

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
                            string upc = dr[2].ToString();
                            if (upc.Length > 5)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                                clase.po = Convert.ToInt64(dr[0].ToString());
                                clase.DPCI = dr[3].ToString();
                                clase.barcode = dr[3].ToString();
                                clase.size_izquierdo = dr[3].ToString();
                                clase.size_derecho = "";
                                clase.Cantidad = Convert.ToInt64(dr[4].ToString());
                                clase.upc = dr[1].ToString();
                                clase.Fecha = DateTime.Now;
                                clase.usuario = usu[0].nombre;
                                clase.id_Inventario = f.GuardaProductoTARGET(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
        }

        private void btnActualizaPO_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarPOReporte();
            Cursor.Current = Cursors.Default;
        }

        private void btnImportarZumies_Click(object sender, EventArgs e)
        {
            try
            {
                importarZumies();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void importarZumies()
        {
            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

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
                            if (dpci.Length > 1)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                                {
                                    po = Convert.ToInt64(dr[9].ToString()),
                                    poInCompleto = 0,
                                    poItem = null,
                                    ProductCode = dr[9].ToString(),
                                    Size = dr[18].ToString(),
                                    size_izquierdo = dr[18].ToString(),
                                    size_derecho = "",
                                    TipoCarton = null,
                                    assembly = dr[46].ToString(),
                                    Vendor = dr[7].ToString(), /*upc 2*/
                                    ShipTo = dr[53].ToString(),
                                    upc = dr[2].ToString(),
                                    Fecha = DateTime.Now,
                                    CartonLeft = "",
                                    CartonRight = "",
                                    Cantidad = Convert.ToInt64(dr[31].ToString()),
                                    Carton = null,
                                    usuario = usu[0].nombre
                                };

                                // clase.id_Inventario = f.GuardaProductoTARGET(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
        }


        private void btnzGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvZumies.RowCount > 0)
                {
                    foreach (DataGridViewRow renglon in dgvZumies.Rows)
                    {
                        EtiquetaCajaModificada pd = new EtiquetaCajaModificada();
                        switch (cmbMarca.SelectedItem)
                        {
                            case "LEVIS":
                                if (validarAltaPO() == 1)
                                {
                                    pd.po = Convert.ToDecimal(txtzPOZumies.Text);
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    f.GuardaAltaPO(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;
                                }
                                break;
                            case "ZUMIES":
                                if (validarZumies() == 1)
                                {
                                    /*MODIFICAR ESTA MAL
                                       tablaZumies.Columns.Add("Talla", typeof(string));
                                        tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                                        tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                                        tablaZumies.Columns.Add("idSize", typeof(int));
                                        tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                                        tablaZumies.Columns.Add("itemDescription", typeof(string));
                                        tablaZumies.Columns.Add("color", typeof(string));
                                     
                                     */
                                    pd.po = Convert.ToDecimal(txtzPOZumies.Text);
                                    pd.estilo = txtzEstilo.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                    pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.itemDescription = renglon.Cells[5].Value.ToString();
                                    pd.color = renglon.Cells[6].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    f.GuardaZumies(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;
                                }
                                break;
                            case "TARGET":
                                if (validarAltaPO() == 1)
                                {
                                    pd.po = Convert.ToDecimal(txtzPOZumies.Text);
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    pd.assembly = txtzPOZumies.Text;
                                    f.GuardarTarget(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;
                                }
                                break;
                        }
                        alta = 0;
                    }
                    ActualizarPO(txtzPOZumies.Text);
                    LimpiarCamposZumies();

                    MessageBox.Show("Termino con Exito!");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LimpiarCamposZumies()
        {
            try
            {
                for (int i = dgvZumies.Rows.Count - 1; i >= 0; i--)
                {
                    dgvZumies.Rows.RemoveAt(i);
                }

                foreach (DataGridViewRow item in dgvZumies.SelectedRows)
                {
                    dgvZumies.Rows.RemoveAt(item.Index);

                }

                tablaZumies = new DataTable();
                tablaZumies.Columns.Add("Talla", typeof(string));
                tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                tablaZumies.Columns.Add("idSize", typeof(int));
                tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                tablaZumies.Columns.Add("itemDescription", typeof(string));
                tablaZumies.Columns.Add("color", typeof(string));

                tablaAltaPO = new DataTable();
                tablaAltaPO.Columns.Add("Talla", typeof(string));
                tablaAltaPO.Columns.Add("Cantidad", typeof(long));
                tablaAltaPO.Columns.Add("Codigo UPC", typeof(string));
                tablaAltaPO.Columns.Add("idSize", typeof(int));


                txtzPOZumies.Text = "";
                txtzEstilo.Text = "";
                //cmbzTallas.SelectedIndex = 0;
                txtzCantidadPrendas.Text = "";
                txtzCantidadCajas.Text = "";
                txtzColor.Text = "";
                txtzItemDescripcion.Text = "";
                txtzUPC.Text = "";
                altaZ = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public int validarZumies()
        {
            try
            {
                if (string.IsNullOrEmpty(txtzPOZumies.Text) || string.IsNullOrEmpty(txtzEstilo.Text) || dgvZumies.RowCount < 1)
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
        private void btnzAgrega_Click(object sender, EventArgs e)
        {

            AgregarTallaZumies();
        }

        public void AgregarTallaZumies()
        {


            try
            {
                switch (cmbMarca.SelectedItem)
                {
                    case "TARGET":
                         if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != "" ))
                        {
                            if (altaZ == 0)
                            {
                                tablaTarget = new DataTable();
                                tablaTarget.Columns.Add("Talla", typeof(string));
                                tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                                tablaTarget.Columns.Add("idSize", typeof(int));
                                tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                                tablaTarget.Columns.Add("itemDescription", typeof(string));
                                tablaTarget.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaTarget.Rows.Add(cmbzTallas.Text,  txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        
                        break;
                    case "ZUMIES":

                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != "" && txtzCantidadCajas.Text != "") )
                        {
                            if (altaZ == 0)
                            {
                                tablaZumies = new DataTable();
                                tablaZumies.Columns.Add("Talla", typeof(string));
                                tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                                tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                                tablaZumies.Columns.Add("idSize", typeof(int));
                                tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                                tablaZumies.Columns.Add("itemDescription", typeof(string));
                                tablaZumies.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }else
                            {
                                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;
                    default:
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaTarget = new DataTable();
                                tablaTarget.Columns.Add("Talla", typeof(string));
                                tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                                tablaTarget.Columns.Add("idSize", typeof(int));
                                tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                                tablaTarget.Columns.Add("itemDescription", typeof(string));
                                tablaTarget.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                        }
                        else
                        {

                        }
                        break;
                }

                
                    
                    




                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void btnzActualiza_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvZumies.RowCount > 0)
                {
                    DataGridViewRow newDatarow = dgvZumies.Rows[RengloSelecionado];
                    switch (cmbMarca.SelectedItem)
                    {
                        case "TARGET":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;

                            break;
                        case "ZUMIES":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = txtzCantidadCajas.Text;
                            newDatarow.Cells[3].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[4].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[5].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[6].Value = txtzColor.Text;
                            break;
                        default:
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;
                    }
                    //RengloSelecionado = dgvPrePack2.CurrentCell.RowIndex;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtzUPC_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    AgregarTallaZumies();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnzQuitar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvZumies.RowCount > 0)
                {
                    RengloSelecionado = dgvZumies.CurrentCell.RowIndex;
                    dgvZumies.Rows.RemoveAt(RengloSelecionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void txtzCantidadPrendas_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    txtzUPC.Focus();
                }
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
                    txtzUPC.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtzCantidadCajas_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtzItemDescripcion_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void dgvZumies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RengloSelecionado = e.RowIndex;
                DataGridViewRow row = dgvZumies.Rows[RengloSelecionado];

                int target = 0;
                cmbzTallas.Text = row.Cells[0].Value.ToString();
                txtzUPC.Text = row.Cells[1].Value.ToString();
                txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                txtzColor.Text = row.Cells[5].Value.ToString();
                txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        private void cmbzTallas_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtzUPC.Focus();


        }
        public void ImprimirTodasEtiquetas()
        {

            string po_numero = txtPOImprimir.Text;
            try
            {
                List<ConsultaInventarioPOResult> x = f.ConsultaInventarioPO(po_numero);
                if (x.Count > 0)
                {
                    if (x[0].poItem == "77")
                    {
                        foreach (ConsultaInventarioPOResult a in x)
                        {
                            EtiquetaZUMIES clase = new EtiquetaZUMIES();

                            clase.id = a.id;
                            clase.po = a.po;
                            clase.poInCompleto = a.poInCompleto;
                            clase.poItem = "77";
                            clase.ProductCode = a.ProductCode;
                            clase.Size = a.size_izquierdo;

                            clase.ESTILO = a.ProductCode;
                            clase.DESCRIPTION = a.TipoCarton;
                            clase.QUANTITY = Convert.ToString(a.Cantidad);
                            clase.CARTON_NUMBER_INICIAL = a.NumeroInicial.ToString();
                            clase.CARTON_NUMBER_FINAL = Convert.ToString(a.TotalCajas);
                            clase.COUNTRY = "MEXICO";

                            List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();

                            listClase.Add(clase);
                            id_InventarioAnt = clase.id_Inventario;

                            ReporteCajaZumines report = new ReporteCajaZumines
                            {
                                DataSource = listClase
                            };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            tool.Print(); //imprime de golpe
                        }
                    }
                    else if (x[0].poItem == "88")
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        int? esPrepack = f.ConsultaPrepack(x[0].id.ToString());

                            if (esPrepack > 0)
                            {
                            foreach (ConsultaInventarioPOResult a in x)
                            {
                                List<ConsultaPrepackDetalleResult> ListprepackTarget = f.ConsultaPrepackDetalle(Convert.ToInt32(a.id));
                                    if (ListprepackTarget.Count > 0)
                                    {
                                        decimal? cantidadTotalCajasPrepack = 0;
                                        string barcode = "";
                                        int? idPrepack = 0;
                                        string DPCI = "";
                                        EtiquetaZUMIES clase = new EtiquetaZUMIES();
                                        foreach (ConsultaPrepackDetalleResult objPrepack in ListprepackTarget)
                                        {
                                            cantidadTotalCajasPrepack = objPrepack.cantidad + cantidadTotalCajasPrepack;
                                            barcode = objPrepack.barcode;
                                            idPrepack = objPrepack.idPrepack;
                                            DPCI = objPrepack.DPCI;
                                        }


                                        EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                                        claseZ.id = a.id;
                                        claseZ.assembly = a.po.ToString();
                                        claseZ.poInCompleto = a.poInCompleto;
                                        claseZ.poItem = "88";
                                        claseZ.ProductCode = "ASSORTMENT";
                                        claseZ.Size = "ASSORTMENT";
                                        claseZ.ESTILO = "ASSORTMENT";
                                        claseZ.QUANTITY = Convert.ToString(cantidadTotalCajasPrepack);
                                        claseZ.CARTON_NUMBER_INICIAL = a.id.ToString();
                                        claseZ.CARTON_NUMBER_FINAL = "___";
                                        claseZ.COUNTRY = "MEXICO";
                                        claseZ.Cantidad = Convert.ToDecimal(cantidadTotalCajasPrepack);
                                        claseZ.DPCI = DPCI;
                                        claseZ.itemDescription = claseZ.DPCI;
                                        claseZ.cn_tag_num = Convert.ToInt32(a.id);
                                        claseZ.color = "";
                                        claseZ.size_izquierdo = "ASSORTMENT";
                                        string contarDigitos = "00" + barcode.Substring(0, 11) + "1";
                                        claseZ.Carton = a.id;
                                        //en ASSORTMENT NO ES EL UPC ES UNO QUE ENGLOBA TODAS LAS TALLAS

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


                                        //////********************************************************imprime carton interno**************************************************************************************************************************////

                                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                        List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                                        EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                                        clase2.id_Inventario = Convert.ToInt32(claseZ.id);
                                        clase2.po = Convert.ToDecimal(a.po.ToString());
                                        clase2.poInCompleto = a.po;
                                        clase2.cliente = "";
                                        clase2.factura = "";
                                        clase2.terminado = "";
                                        clase2.usuario = usu[0].nombre;
                                        clase2.ProductCode = claseZ.ProductCode;
                                        clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                                        clase2.Size = "ASSORTMENT";
                                        clase2.Fecha = a.create_dtm;
                                        clase2.assembly = claseZ.po.ToString() ;
                                        clase2.size_izquierdo = clase2.Size;

                                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                                        Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
                                        clase2.qr = qrCode.GetGraphic(20);
                                        clase2.codigoBarras = codigoBarras2;
                                        listClase2.Add(clase2);
                                    }
                                }
                            }
                            else
                            {



                            List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                            List<EtiquetaZUMIES> listClaseTARGET = new List<EtiquetaZUMIES>();
                            foreach (ConsultaInventarioPOResult a in x)
                            {
                                EtiquetaZUMIES clase = new EtiquetaZUMIES();
                                clase.id = a.id;
                                clase.po = a.po;
                                clase.poInCompleto = a.poInCompleto;
                                clase.poItem = x[0].poItem;
                                clase.Carton = a.id;
                                clase.ProductCode = a.ProductCode;
                                clase.Size = a.size_izquierdo;
                                clase.ESTILO = a.ProductCode;
                                clase.QUANTITY = Convert.ToString(a.Cantidad);
                                clase.Cantidad = a.Cantidad;
                                //clase.cn_tag_num = a.Carton;
                                if (a.TipoCarton.Contains('/'))
                                {
                                    a.TipoCarton = (a.TipoCarton.Replace('/', '-')).Trim();
                                }
                                else
                                {

                                }
                                clase.DPCI = a.TipoCarton.Trim();
                                clase.DESCRIPTION = a.TipoCarton.Trim();
                                clase.itemDescription = clase.DPCI.Trim();
                                clase.color = "";
                                clase.size_izquierdo = a.size_izquierdo;
                                clase.assembly = a.ProductCode;


                                string contarDigitos = "00" + a.upc.Substring(0, 11) + "1";

                                BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                                Codigo.BarWidth = 3;

                                Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                                clase.codigoBarras = codigoBarras;//cropImage;
                                listClaseTARGET.Add(clase);

                                //////********************************************************imprime carton interno**************************************************************************************************************************////

                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                                clase2.id_Inventario = Convert.ToInt32(clase.id);
                                clase2.po = clase.po;
                                clase2.poInCompleto = clase.po;
                                clase2.cliente = "";
                                clase2.factura = "";
                                clase2.terminado = "";
                                clase2.usuario = usu[0].nombre;
                                clase2.ProductCode = clase.ProductCode;
                                clase2.Cantidad = Convert.ToDecimal(clase.QUANTITY);
                                clase2.Size = clase.size_izquierdo;
                                clase2.Fecha = DateTime.Now;

                                QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_Inventario +
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
                                Image codigoBarras2 = Codigo2.Encode(BarcodeLib.TYPE.CODE39, clase2.id_Inventario.ToString(), Color.Black, Color.White, 250, 150);
                                clase2.qr = qrCode.GetGraphic(20);
                                clase2.codigoBarras = codigoBarras2;
                                clase2.assembly = "*" + clase.id.ToString() + "*";
                                listClase2.Add(clase2);
                                /////////////////////////////////////////////////////////////////////////////////*/*/*/*//////////////////////////

                            }


                            //id_InventarioAnt = Convert.ToInt32(idInv);
                            ReportCajaTarget report = new ReportCajaTarget
                            {
                                DataSource = listClaseTARGET
                            };
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);

                            tool.Print(); //imprime de golpe

                            //////********************************************************imprime carton interno**************************************************************************************************************************////

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
                                           //////////////////////////////////////////////////////////////////////////////////////////////////////*/



                            //string contarDigitos = "00" + x1[0].upc.Substring(0, 11) + "1";

                            //List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                            //BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                            //Codigo.BarWidth = 3;

                            //Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                            //claseZ.codigoBarras = codigoBarras;//cropImage;
                            //listClase.Add(claseZ);
                            ////id_InventarioAnt = Convert.ToInt32(idInv);
                            //ReportCajaTarget report = new ReportCajaTarget
                            //{
                            //    DataSource = listClase
                            //};
                            //// Disable margins warning. 
                            //report.PrintingSystem.ShowMarginsWarning = false;
                            //ReportPrintTool tool = new ReportPrintTool(report);

                            //tool.Print(); //imprime de golpe

                        }


                        Cursor.Current = Cursors.Default;
                    }
                    else
                    {
                        MessageBox.Show("Favor de ingresar el numero correctamente.");

                        Cursor.Current = Cursors.Default;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Cursor.Current = Cursors.Default;
            }








            txtUPCScann.Text = string.Empty;
            txtUPCScann.Focus();
            int idInv = 0;
            try { idInv = Convert.ToInt32(txtIDReImpresion.Text); } catch (Exception) { idInv = 0; }
            List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(idInv);

        }

        private void btnImprimirTodasEtiquetas_Click(object sender, EventArgs e)
        {
            ImprimirTodasEtiquetas();
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

        private void txtPOImprimir_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    ImprimirTodasEtiquetas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tpReporteSyncFusion_Click(object sender, EventArgs e)
        {

        }

        private void cmbPO_KeyPress(object sender, KeyPressEventArgs e)
        {
            //try
            //{
            //    if (e.KeyChar == (int)Keys.Enter)
            //    {
            //            iniciando = true;
            //            f.ConsultaPOItem(cmbPOItem, cmbPO.Text);
            //            cmbPOItem.SelectedIndex = 0;
            //            f.ConsultaProductCode(cmbProductCode, cmbPO.Text, cmbPOItem.Text);
            //            cmbProductCode.SelectedIndex = 0;
            //            f.ConsultaSizes(cmbSizes, cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text);
            //            cmbSizes.SelectedIndex = -1;
            //            iniciando = false;
            //            cmbSizes.SelectedIndex = 0;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //  MessageBox.Show(ex.Message.ToString());
            //}
        }

        private void cmbPO_Enter(object sender, EventArgs e)
        {
        }

        private void btnPrendasExtra_Click(object sender, EventArgs e)
        {
            PrendasExtra = 1;
        }

        private void txtAltaPO_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtzPOZumies_KeyPress(object sender, KeyPressEventArgs e)
        {


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

        private void cmbMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                switch (cmbMarca.SelectedItem.ToString())
                {
                    case "LEVIS":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "ZUMIES":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzCantidadCajas.Enabled = true;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "CINTAS":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "TARGET":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzUPC.Enabled = true;
                        txtzCantidadPrendas.Enabled = true;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzCantidadPrendas.Text = "12";
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        break;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Bitmap img = new Bitmap(Application.StartupPath + @"\fondo1.jpeg");
            //groupBox4.BackgroundImage = img;
            //groupBox4.BackgroundImageLayout = ImageLayout.Stretch;

        }


        private void button7_Click(object sender, EventArgs e)
        {
            //Bitmap img = new Bitmap(Application.StartupPath + @"\fondo2.jpeg");
            //groupBox3.BackgroundImage = img;
            //groupBox3.BackgroundImageLayout = ImageLayout.Stretch;

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

        private void button6_Click_1(object sender, EventArgs e)
        {
            // int i = 701780;
            int i = 701810;


            while (i < 704810)
            {
                EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                claseZ.id = 1;
                claseZ.assembly = "";
                claseZ.poInCompleto = 5362339;
                claseZ.poItem = "88";
                claseZ.ProductCode = "ASSORTMENT";
                claseZ.Size = "";
                claseZ.ESTILO = "ASSORTMENT";
                claseZ.QUANTITY = "1";
                claseZ.cn_tag_num = 1;
                claseZ.CARTON_NUMBER_INICIAL = "1";
                claseZ.CARTON_NUMBER_FINAL = "___";
                claseZ.Carton = i;
                claseZ.COUNTRY = "MEXICO";
                claseZ.DPCI = "014-08-0057";
                claseZ.itemDescription = "014-19-0130";
                claseZ.color = "";
                claseZ.assembly = "5362339";
                string contarDigitos = "10490141901309";

                List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                Codigo.BarWidth = 3;

                Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                claseZ.codigoBarras = codigoBarras;  //cropImage;
                listClase.Add(claseZ);
                ReporteCajaTargetAssorment report = new ReporteCajaTargetAssorment
                {
                    DataSource = listClase
                };
                // Disable margins warning. 
                report.PrintingSystem.ShowMarginsWarning = false;
                ReportPrintTool tool = new ReportPrintTool(report);

                //tool.ShowPreview();
                ////tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                tool.Print(); //imprime de golpe
                i = i + 1;
            }
        }

        private void btnzCancelar_Click(object sender, EventArgs e)
        {

            LimpiarCamposZumies();
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

                        List<ConsultaTodoPOResult> ListPO = f.ConsultaTodoPO(Convert.ToDecimal(cmbPOMod.Text));


                        foreach (ConsultaTodoPOResult objPO in ListPO)
                        {
                            switch (objPO.po_item)
                            {
                                case "1000":
                                    cmbMarcaPOMod.Text = "LEVIS";
                                    txtColorMod.Enabled = false;
                                    txtDescription.Enabled = false;
                                    txtEstiloPOMod.Enabled = false;
                                    f.ConsultaTallasXMarca(cmbTallaMod, cmbMarcaPOMod.Text);
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
                                    cmbMarcaPOMod.Text = "TARGET";

                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    f.ConsultaTallasXMarca(cmbTallaMod, cmbMarcaPOMod.Text);
                                    cmbTallaMod.SelectedIndex = 0;

                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    txtDescription.Text = objPO.itemDescription.ToString();
                                    txtColorMod.Text = objPO.color;
                                    AgregarTallasModificacion(objPO.size_izquierdo, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString());

                                    break;
                                case "77":
                                    cmbMarcaPOMod.Text = "ZUMIES";

                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    f.ConsultaTallasXMarca(cmbTallaMod, cmbMarcaPOMod.Text);
                                    cmbTallaMod.SelectedIndex = 0;

                                    txtEstiloPOMod.Text = objPO.prod_cd.ToString();
                                    txtDescription.Text = objPO.itemDescription.ToString();
                                    txtColorMod.Text = objPO.color;
                                    AgregarTallasModificacion(objPO.size_izquierdo, objPO.cantidad.ToString(), objPO.upc, objPO.idSize.ToString());

                                    break;
                                default:
                                    cmbMarcaPOMod.Text = "LEVIS";
                                    txtColorMod.Enabled = true;
                                    txtDescription.Enabled = true;
                                    txtEstiloPOMod.Enabled = true;
                                    f.ConsultaTallasXMarca(cmbTallaMod, cmbMarcaPOMod.Text);
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

                tablaModificacion = new DataTable();
                tablaModificacion.Columns.Add("Talla", typeof(string));
                tablaModificacion.Columns.Add("Cantidad", typeof(long));
                tablaModificacion.Columns.Add("Codigo UPC", typeof(string));
                tablaModificacion.Columns.Add("idSize", typeof(string));

                altaZ = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AgregarTallasModificacion(string talla, string cantidad, string upc, string idsize)
        {
            try
            {
                try
                {
                    tablaModificacion.Rows.Add(talla, cantidad, upc, idsize);
                    dgvModificarPO.DataSource = tablaModificacion;
                }
                catch (Exception ex)
                {
                    tablaModificacion = new DataTable();
                    tablaModificacion.Columns.Add("Talla", typeof(string));
                    tablaModificacion.Columns.Add("Cantidad", typeof(long));
                    tablaModificacion.Columns.Add("Codigo UPC", typeof(string));
                    tablaModificacion.Columns.Add("idSize", typeof(string));



                    tablaModificacion.Rows.Add(talla, cantidad, upc, idsize);
                    dgvModificarPO.DataSource = tablaModificacion;

                    dgvModificarPO.Columns["idSize"].Visible = false;
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
            {
                RengloSelecionado = e.RowIndex;
                DataGridViewRow row = dgvModificarPO.Rows[RengloSelecionado];


                cmbTallaMod.Text = row.Cells[0].Value.ToString();
                txtCantPrendCaja.Text = row.Cells[1].Value.ToString();
                txtCodigoUPC.Text = row.Cells[2].Value.ToString();

                txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
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
                    //RengloSelecionado = dgvPrePack2.CurrentCell.RowIndex;
                    DataGridViewRow newDatarow = dgvModificarPO.Rows[RengloSelecionado];
                    newDatarow.Cells[0].Value = cmbTallaMod.Text;
                    newDatarow.Cells[1].Value = txtCantPrendCaja.Text;
                    newDatarow.Cells[2].Value = txtCodigoUPC.Text;
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
                    foreach (DataGridViewRow renglon in dgvModificarPO.Rows)
                    {
                        EtiquetaCajaModificada pd = new EtiquetaCajaModificada();
                        switch (cmbMarcaPOMod.SelectedItem)
                        {
                            case "LEVIS":
                                if (validarModificacionPOLEVIS() == 1)
                                {
                                    pd.po = Convert.ToDecimal(cmbPOMod.Text);
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.idusuario = usu[0].id;
                                    ///BORRAR PO
                                    ///
                                    f.GuardaAltaPO(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;
                                }
                                break;
                            case "ZUMIES":
                                if (validarZumies() == 1)
                                {
                                    pd.po = Convert.ToDecimal(cmbPOMod.Text);
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.idusuario = usu[0].id;


                                    pd.estilo = txtEstiloPOMod.Text;
                                    pd.itemDescription = txtDescription.Text;
                                    pd.color = txtColorMod.Text;
                                    pd.idusuario = usu[0].id;
                                    ///BORRAR PO
                                    ///
                                    f.GuardaZumies(pd);
                                    f.ConsultaPO(cmbPO);
                                    cmbPO.SelectedIndex = -1;
                                }
                                break;
                            case "TARGET":
                                if (validarAltaPO() == 1)
                                {
                                    pd.po = Convert.ToDecimal(cmbPOMod.Text);
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.idusuario = usu[0].id;
                                    pd.assembly = cmbPOMod.Text;
                                    ///BORRAR PO
                                    ///
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

        private void btnAddMod_Click(object sender, EventArgs e)
        {
            AgregarTallasModificacion(cmbTallaMod.Text, txtCantPrendCaja.Text, txtCodigoUPC.Text, cmbTallaMod.SelectedValue.ToString());

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

        private void btnImportarTARGET_Click(object sender, EventArgs e)
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

        private void btnImportar_Click_1(object sender, EventArgs e)
        {
            try
            {
                importarROCKY();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAplicarModificacionInv_Click_1(object sender, EventArgs e)
        {

        }

        private void btnBajaTalla_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja la talla "+ cmbTallaBaja.Text +" del po " + cmbPOB.Text + " ", "Baja Talla", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");

                    if (password == "Ain2022_")
                    {
                        bool baja = f.BajaTallaPO(cmbPOB.Text, cmbTallaBaja.Text, usu[0].id);
                        if (baja)
                        {
                            MessageBox.Show("se elimino correctamente la talla ."+ cmbTallaBaja.Text);
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

        private void cmbPOB_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
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