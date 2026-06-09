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
        private string Talla = string.Empty;
        private string vendor = string.Empty;
        private readonly WindowsMediaPlayer sonido = new WindowsMediaPlayer();
        private readonly List<ConsultaUsuarioResult> usu = null;
        private int? id = 0;
        private readonly List<int?> anterior = new List<int?>();
        private int contador = 0;
        private int contadorVecesCMBPO = 0;
        private Int64 id_inventarioAnt = 0;
        private int? CantidadDividir = 0;
        private DataTable tablaPrepack = new DataTable();
        private DataTable tablaAltaPO = new DataTable();
        private DataTable tablaZumies = new DataTable();
        private DataTable tablaBens = new DataTable();
        private DataTable tablaIFR = new DataTable();
        private DataTable tablaTarget = new DataTable();
        private DataTable tablaEscaner = new DataTable();
        private DataTable tablaEstiloAlta = new DataTable();
        private DataTable tablaModificacion = new DataTable();
        private int RengloSelecionado;
        private int Tabpage = 0;
        private int alta = 0;
        private int altaP = 0;
        private int altaZ = 0;
        private int PrendasExtra = 0;
        private int TallaRepetida = 0;
        private int TallaMixta = 0;
        Boolean imprimir = false;
        private Prendas prendas= new Prendas();
        List<EtiquetaCajaModificada> cajaOlnavy = new List<EtiquetaCajaModificada>();
        List<EtiquetaCajaModificada> cajaOlnavyTotal = new List<EtiquetaCajaModificada>();
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
            tablaTarget = new DataTable();
            tablaEscaner = new DataTable();
            tablaBens = new DataTable();
            tablaIFR = new DataTable();
        }
        public Escaneo(List<ConsultaUsuarioResult> usuario)
        {
            try
            {
                InitializeComponent();
                f.ConsultaPO(cmbPO); cmbPO.SelectedIndex = -1; f.ConsultaCliente(cmbCliente); cmbCliente.SelectedIndex = 0; f.ConsultaFactura(cmbFactura);
                cmbFactura.SelectedIndex = 0; f.ConsultaTerminado(cmbTerminado); cmbTerminado.SelectedIndex = 0; f.ConsultaPOModificar(cmbPOMod); cmbPOMod.SelectedIndex = -1;
                f.ConsultaPO(cmbPOB); cmbPOB.SelectedIndex = -1; f.ConsultaTallasZumies(cmbzTallas); cmbzTallas.SelectedIndex = 0;
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
                f.ConsultaTallasXMarca(cmbTallaPrepack, "LEVIS");
                cmbTallaPrepack.SelectedIndex = 0;
                usu = usuario;
                try
                {
                    lblVersion.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                    lblVersion2.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }
                catch (Exception) { }
                if (usu[0].perfil == "1")
                {
                    btnImportarEstilo.Visible = true;
                }
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
                }
                else if (usu[0].perfil == "5")
                {
                    TabEsaneo.TabPages.RemoveAt(0);
                    TabEsaneo.TabPages.RemoveAt(4);
                    TabEsaneo.TabPages.RemoveAt(3);
                    TabEsaneo.TabPages.RemoveAt(2);
                    TabEsaneo.TabPages.RemoveAt(1);
                    //TabEsaneo.TabPages.Remove(ModificarPO);
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
                    TabEsaneo.TabPages.RemoveAt(4);
                    TabEsaneo.TabPages.RemoveAt(3);
                    TabEsaneo.TabPages.RemoveAt(2);
                    TabEsaneo.TabPages.RemoveAt(0);
                    TabEsaneo.TabPages.Remove(ModificarPO);

                    //  SE HABILITAN BOTONES PARA BAJA DE PO O CAJAS
                    cmbPOB.Enabled = true;
                    //cmbClienteB.Enabled = true;
                    //cmbFacturacionB.Enabled = true;
                    //cmbTerminadoB.Enabled = true;
                    btnBajaPO.Enabled = true;

                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;

                }
                else if (usu[0].perfil == "11")
                {
                    /*PERFIL DE ADMON*/
                    TabEsaneo.TabPages.Remove(tpReporteDiario);
                    TabEsaneo.TabPages.Remove(ModificarPO);

                    TabEsaneo.TabPages.Remove(tpEscan);

                    cmbPOB.Enabled = true;
                    btnBajaPO.Enabled = true;
                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                    //  SE ABILITAN BOTONES PARA BAJA DE PO O CAJAS
                    cmbPOB.Enabled = true;
                    btnBajaPO.Enabled = true;
                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                }
                else if (usu[0].perfil == "12")
                {
                    TabEsaneo.TabPages.Remove(tpAltaPrePack);
                    TabEsaneo.TabPages.Remove(tpReporteDiario);
                    TabEsaneo.TabPages.Remove(tpBajaCaja);
                    TabEsaneo.TabPages.Remove(tpbajaPO);
                    TabEsaneo.TabPages.Remove(tpImportaLEVIS);
                    TabEsaneo.TabPages.Remove(ModificarPO);
                    btnBajaPO.Enabled = true;
                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                    //  SE ABILITAN BOTONES PARA BAJA DE PO O CAJAS
                    cmbPOB.Enabled = true;
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
                    btnBajaPO.Enabled = true;
                    txtCaja.Enabled = true;
                    btnBajaCaja.Enabled = true;
                    //  SE ABILITAN BOTONES PARA BAJA DE PO O CAJAS
                    cmbPOB.Enabled = true;
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
                btnBajaPO.Enabled = true;
                txtCaja.Enabled = true;
                btnBajaCaja.Enabled = true;
                iniciando = false;
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
                //MessageBox.Show(ex.Message.ToString());
            }
        }



        public class TallasMixtas
        {
            // Campos privados
            public int id;
            public string sizeIzquierdo;
            public string sizeDerecho;
            public string upc;
            public string tipo;
            public string estilo;
            public string color;
            public string descripcion;
            public int cantidad;

            // Constructor para inicializar
            public TallasMixtas(int id, string sizeIzq, string sizeDer, string upc, string tipo,
                                string estilo, string color, string descripcion, int cantidad)
            {
                this.id = id;
                this.sizeIzquierdo = sizeIzq;
                this.sizeDerecho = sizeDer;
                this.upc = upc;
                this.tipo = tipo;
                this.estilo = estilo;
                this.color = color;
                this.descripcion = descripcion;
                this.cantidad = cantidad;
            }

            //// Propiedades públicas para acceder a los campos
            //public int Id => id;
            //public string SizeIzquierdo => sizeIzquierdo;
            //public string SizeDerecho => sizeDerecho;
            //public string UPC => upc;
            //public string Tipo => tipo;
            //public string Estilo => estilo;
            //public string Color => color;
            //public string Descripcion => descripcion;
            //public int Cantidad => cantidad;
        }



        private void btnIncompleteCarton_Click(object sender, EventArgs e)
        {
            try
            {
                string po_numero = cmbPO.Text;
                List<ConsultaUPCResult> ListUpc;
                if (id != 0 && Convert.ToInt32(txtUnitsScan.Text) > 0 && dgvEscan.RowCount > 0)
                {
                    if (cmbPOItem.Text != "99" && cmbPOItem.Text != "77" && cmbPOItem.Text != "88" && cmbPOItem.Text != "55" && cmbPOItem.Text != "66" && cmbPOItem.Text != "33" && cmbPOItem.Text != "44" && cmbPOItem.Text != "22")
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
                            terminado = cmbTerminado.Text,
                            estilo = consulta[0].ProductCode,
                            TipoCarton = consulta[0].color,
                            color = consulta[0].color
                        };
                        /**/
                        if (clase.poItem == "1000")
                        {

                            if (chkImprimirEtiqueta.Checked == true)
                            {
                                ///do {
                                try
                                {
                                    clase.Carton = Convert.ToInt64(Interaction.InputBox("Captura Numero de Carton", "Carton", "", 5, 5));
                                }
                                catch (Exception)
                                {
                                    /*clase.Carton = 0; MessageBox.Show("Favor de ingresar correctamente el numero de carton ya que no se ha guardado");*/
                                };

                                clase.assembly = consulta[0].Assembly;
                                clase.Vendor = consulta[0].Vendor;
                                clase.ShipTo = consulta[0].ShipTo;
                                clase.po_varchar = "0";
                                clase.id_inventario = f.GuardaInventario(clase, usu[0].id);
                                //clase.id_inventario = 116;
                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
                                                                                    "&po=" + consulta[0].po +
                                                                                    "&ca=" + clase.Carton +
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
                                Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_inventario.ToString(), Color.Black, Color.White, 270, 180);
                                clase.qr = qrCode.GetGraphic(20);
                                clase.codigoBarras = codigoBarras;
                                clase.assembly = "*" + clase.id_inventario.ToString() + "*";
                                clase.po = cmbPO.Text;
                                listClase.Add(clase);
                                id_inventarioAnt = clase.id_inventario;

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
                            else
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
                                Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_inventario.ToString(), Color.Black, Color.White, 270, 180);

                                // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                                clase.codigoBarras = codigoBarras;
                                listClase.Add(clase);
                                id_inventarioAnt = clase.id_inventario;

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
                    else if (cmbPOItem.Text == "55")/*BEN DAVIS*/
                    {
                        try
                        {
                            /*TOTAL DE PRENDAS ESCANEADAS*/
                            string[] separadas;
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
                                clase.poItem = "55";
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

                                //clase.Vendor = clase.anio;

                                clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);


                                //clase.id_string = x[0].Estilo + clase.size_izquierdo + clase.size_derecho + clase.QUANTITY + clase.id;

                                clase.assembly = "*" + clase.id_string + "*";

                                DateTime fechaEntrada = DateTime.Now;

                                clase.anio = fechaEntrada.Year + clase.id.ToString();


                                id_inventarioAnt = Convert.ToInt32(clase.id_string);
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                                List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                                listclaseZ.Add(clase);
                                //id_inventarioAnt = clase.id_inventario;
                                id_inventarioAnt = Convert.ToInt32(clase.id_string);
                                ReporteCajaBens report = new ReporteCajaBens
                                {
                                    DataSource = listclaseZ
                                };
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                tool.Print(); //imprime de golpe
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

                    else if (cmbPOItem.Text == "66")
                    {

                        /*TOTAL DE PRENDAS ESCANEADAS*/
                        string[] separadas;

                        separadas = cmbSizes.Text.Split('x');
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
                            clase.size_derecho = separadas[1].ToString().Trim();
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


                    else if (cmbPOItem.Text == "33")
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

                                clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);

                                clase.id_string = "*" + clase.id_string + "*";

                                clase.assembly = "*" + clase.id_string + "*";

                                clase.anio = DateTime.Now.Year.ToString() + clase.id_string;
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

                    else if (cmbPOItem.Text == "44")
                    {
                        // btnImprimeOldNavy.Visible = false;
                    }
                   
                    else if (cmbPOItem.Text == "22")
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
                            else { 
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
                                id_inventarioAnt = Convert.ToInt32(clase.id);
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
                            // Displays the MessageBox.
                        } while (result == DialogResult.Yes);
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
                        string marca = f.ConsultaMarca(cmbPO.Text, cmbPOItem.Text);
                        string[] separadas;
                        separadas = cmbSizes.Text.Split('x');
                        if (cmbPOItem.Text == "99")/* PREPACK */
                        {
                            lblImprimirEtiqueta.Visible = true;
                            chkImprimirEtiqueta.Visible = true;

                            separadas = cmbSizes.Text.Split('x');
                            List<ConsultaProductos_VarcharResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());
                            dgvEscan.DataSource = x;

                            dgvEscan.Columns["ProductCode1"].Visible = false;
                            dgvEscan.Columns["Vendor"].Visible = false;
                            dgvEscan.Columns["ProductCode"].Visible = false;
                            dgvEscan.Columns["CartonType"].Visible = false;
                            dgvEscan.Columns["id"].Visible = false;
                            txtCartonSize.Text = "";
                            txtSize.Text = "";
                            if (x.Count > 1)
                            {
                                foreach (ConsultaProductos_VarcharResult a in x)
                                {
                                    contador = 0;
                                    txtCartonNumber.Text = a.CartonNumber.ToString();
                                    txtProductCode.Text = a.ProductCode.ToString();
                                    id = a.id;
                                    upc = cmbPOItem.Text;
                                    anterior.Add(id);
                                }
                            }
                            txtCartonRq.Text = x[0].TotalUnits.ToString();
                            cantidad = x.Count.ToString();
                            txtUnitsRemai.Text = cantidad.ToString();
                            txtUnitsReq.Text = cantidad.ToString();
                            txtUnitsScan.Text = "0";
                            txtCartonsReamaining.Text = "1";
                            txtCartonsPacked.Text = txtCartonNumber.Text;
                            txtUPCScann.Focus();
                        }
                        else if (cmbPOItem.Text == "88")/*TARGET*/
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
                        else if (cmbPOItem.Text == "66")/*ROCKY*/
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
                        else if (cmbPOItem.Text == "44")/*OLDNAVY*/
                        {

                            lblImprimirEtiqueta.Visible = false;
                            chkImprimirEtiqueta.Visible = false;
                            try
                            {
                                separadas = cmbSizes.Text.Split('x');
                                List<ConsultaProductos_VarcharResult> x =
                                    f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());
                                if (x.Count > 0)
                                {
                                    contador = 0;
                                    dgvEscan.DataSource = x;
                                    txtCartonsPacked.Text = x[0].CartonNumber.ToString();
                                    //txtCartonNumber.Text = x[0].CartonNumber.ToString();
                                    txtCartonSize.Text = x[0].Size.ToString();
                                    dgvEscan.Columns["Cantidad"].Visible = false;
                                    dgvEscan.Columns["ProductCode1"].Visible = false;
                                    dgvEscan.Columns["Size"].Visible = false;
                                    dgvEscan.Columns["UPC"].Visible = false;
                                    dgvEscan.Columns["vendor"].Visible = false;
                                    dgvEscan.Columns["CartonType"].Visible = false;
                                    dgvEscan.Columns["CartonNumber"].Visible = false;
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
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                        else if (cmbPOItem.Text == "77")/*ZUMIES*/
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
                        else if (cmbPOItem.Text == "55")/*bens*/
                        {

                            lblImprimirEtiqueta.Visible = false;
                            chkImprimirEtiqueta.Visible = false;
                            try
                            {
                                List<ConsultaProductosBensEscaneoResult> x = f.ConsultaProductosBensEscaneo(cmbPO.Text, separadas[0].ToString().Trim(), separadas[1].ToString().Trim());
                                if (x.Count > 0)
                                {
                                    contador = 0;
                                    txtCartonSize.Text = "";
                                    txtSize.Text = "";
                                    txtProductCode.Text = x[0].Estilo.ToString();
                                    anterior.Add(id);
                                    //upc = cmbPOItem.Text;
                                    contador = 0;
                                    dgvEscan.DataSource = x;
                                    txtCartonsPacked.Text = (Convert.ToInt32(x[0].CartonNumber) - 1).ToString();
                                    //txtCartonNumber.Text = x[0].CartonNumber.ToString();
                                    // dgvEscan.Columns["Cantidad"].Visible = false;
                                    //dgvEscan.Columns["ProductCode1"].Visible = false;
                                    //dgvEscan.Columns["Size"].Visible = false;
                                    //dgvEscan.Columns["UPC"].Visible = false;
                                    //dgvEscan.Columns["vendor"].Visible = false;
                                    //dgvEscan.Columns["CartonType"].Visible = false;
                                    //dgvEscan.Columns["CartonNumber"].Visible = false;
                                    //dgvEscan.Columns["id"].Visible = false;
                                    id = x[0].id;
                                    anterior.Add(id);
                                    ///po = Convert.ToInt32(cmbPO.Text);
                                    txtUnitsReq.Text = cantidad.ToString();
                                    txtUnitsRemai.Text = cantidad.ToString();
                                    txtUnitsScan.Text = "0";
                                    txtCartonsReamaining.Text = "1";
                                    upc = x[0].UPC.ToString();
                                    txtUPCScann.Focus();
                                    txtSize.Text = cmbSizes.Text;


                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }

                        else if (marca == "IFR")
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

                        else
                        {
                            separadas = cmbSizes.Text.Split('x');
                            try
                            {
                                List<ConsultaProductos_VarcharResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());
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
                                    txtCartonRq.Text = "1";
                                    txtCartonsReamaining.Text = "1";
                                    txtUPCScann.Focus();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString());

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
            txtCartonsReamaining.Text = cantidad.ToString();
            txtUPCScann.Focus();
        }
        public EtiquetaCajaModificada RellenaObjetoClaseREIMPRETODO(List<ConsultaInventarioPOResult> consulta)
        {
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                id = consulta[0].id,
                po = consulta[0].po,
                po_varchar = consulta[0].po,
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
                color = consulta[0].Vendor,
                itemDescription = consulta[0].Assembly,
                descripcion = consulta[0].descripcion,
                descripcion2 = consulta[0].descripcion2,
                usuario = usu[0].nombre,
                marca = consulta[0].marca,
                estilo = consulta[0].ProductCode,
                id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue),
                id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue),
                id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue),
                cliente = cmbCliente.Text,
                factura = cmbFactura.Text,
                terminado = cmbTerminado.Text,
                upcCaja = consulta[0].upcCaja
            };
            try
            {
                clase.poInCompleto = Convert.ToInt64(consulta[0].poInCompleto);
            }
            catch
            {

            }

            return clase;
        }
        public EtiquetaCajaModificada RellenaObjetoClaseReimpresion(List<ConsultaInventarioIDResult> consulta)
        {
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                id = consulta[0].id,
                po = consulta[0].po,
                po_varchar = consulta[0].po,
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
                color = consulta[0].Vendor,
                itemDescription = consulta[0].Assembly,
                descripcion = consulta[0].descripcion,
                descripcion2 = consulta[0].descripcion2,
                usuario = usu[0].nombre,
                marca = consulta[0].marca,
                estilo = consulta[0].ProductCode,
                id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue),
                id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue),
                id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue),
                cliente = cmbCliente.Text,
                factura = cmbFactura.Text,
                terminado = cmbTerminado.Text,
                upcCaja = consulta[0].upcCaja
            };
            try
            {
                clase.poInCompleto = Convert.ToInt64(consulta[0].poInCompleto);
            }
            catch
            {

            }
            if (clase.estilo == "")
            {
                clase.estilo = consulta[0].estilo;
            }

            return clase;
        }
        public EtiquetaCajaModificada RellenaObjetoClase(List<ConsultaEtiquetaResult> consulta)
        {
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                id = consulta[0].id,
                po = consulta[0].po,
                poInCompleto = consulta[0].poInCompleto,
                po_varchar = consulta[0].po,
                poItem = consulta[0].poItem,
                ProductCode = consulta[0].estilo,
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
                descripcion = consulta[0].descripcion,
                descripcion2 = consulta[0].descripcion2,
                usuario = usu[0].nombre,
                marca = consulta[0].marca,
                estilo = consulta[0].ProductCode,
                id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue),
                id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue),
                id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue),
                cliente = cmbCliente.Text,
                factura = cmbFactura.Text,
                terminado = cmbTerminado.Text
            };
            if (clase.estilo == "")
            {
                clase.estilo = consulta[0].estilo;
            }

            return clase;
        }
        public EtiquetaCajaModificada RellenaObjetoClaseINCOMPLETO(List<ConsultaEtiquetaResult> consulta)
        {
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                id = consulta[0].id,
                po = consulta[0].po,
                poInCompleto = consulta[0].poInCompleto,
                po_varchar = consulta[0].po,
                poItem = consulta[0].poItem,
                ProductCode = consulta[0].estilo,
                Size = consulta[0].Size,
                size_derecho = consulta[0].size_derecho,
                size_izquierdo = consulta[0].size_izquierdo,
                TipoCarton = consulta[0].TipoCarton,
                upc = consulta[0].upc,
                Fecha = DateTime.Now,
                CartonLeft = consulta[0].CartonLeft,
                CartonRight = consulta[0].CartonRight,
                Cantidad = Convert.ToDecimal(txtUnitsScan.Text),                
                Carton = consulta[0].id,
                color = consulta[0].color,
                itemDescription = consulta[0].itemDescription,
                descripcion = consulta[0].descripcion,
                descripcion2 = consulta[0].descripcion2,
                usuario = usu[0].nombre,
                marca = consulta[0].marca,
                estilo = consulta[0].ProductCode,
                id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue),
                id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue),
                id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue),
                cliente = cmbCliente.Text,
                factura = cmbFactura.Text,
                terminado = cmbTerminado.Text
            };
            if (clase.estilo == "")
            {
                clase.estilo = consulta[0].estilo;
            }

            return clase;
        }
        private string QuitarCeroInicial(string valor)
        {
            if (valor.StartsWith("0"))
                return valor.Substring(1);
            return valor;
        }


        private void txtUPCScann_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    string marca = f.ConsultaMarca(cmbPO.Text, cmbPOItem.Text);
                    tpEscan.BackColor = Color.Silver;
                    List<ConsultaUPC_VarcharResult> ListUpc;
                    string po_numero = cmbPO.Text;
                    DataGridViewRow newDatarow;
                    if (txtUnitsScan.Text == "0")
                    {
                        try
                        {
                            ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
                            upc = ListUpc[0].upc;
                            txtSize.Text = ListUpc[0].Size;
                            Talla = txtSize.Text;
                            cmbSizes.Text = ListUpc[0].Size;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Favor de Escanear la prenda correcta!");
                        }
                    }
                    if (PrendasExtra == 1 && cmbPOItem.Text != "44")
                    {
                        newDatarow = dgvEscan.Rows[(Convert.ToInt32(1))];
                    }
                    else
                    {
                        newDatarow = dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))];
                        txtUnitsReq.Text = dgvEscan.RowCount.ToString();
                    }
                    if (cmbPOItem.Text == "99")/*PREPACK*/
                    {
                        upc = newDatarow.Cells[4].Value.ToString();
                    }
                    if (txtUnitsScan.Text == "0" && (cmbPOItem.Text != "99" && cmbPOItem.Text != "44" && cmbPOItem.Text != "55" && cmbPOItem.Text != "66" && cmbPOItem.Text != "77" && cmbPOItem.Text != "22"))
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
                    #endregion
                    else if (upc == txtUPCScann.Text && cmbPOItem.Text != "99" && cmbPOItem.Text != "44" && cmbPOItem.Text != "22")
                    {
                        #region ZUMIES(77) Y  TARGET(88)
                        if (cmbPOItem.Text == "77" || cmbPOItem.Text == "88")/* ZUMIES  Y  TARGET */
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
                        #endregion

                        #region ROCKY 66
                        if (cmbPOItem.Text == "66")/*ROCKY*/
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
                                        clase.Size = x[0].Talla;
                                        clase.size_izquierdo = separadas[0].ToString().Trim();
                                        clase.size_derecho = separadas[1].ToString().Trim();
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
                                                claseZ.Size = x1[0].Size.Trim();
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
                                                claseR.Size = x1[0].Size.Trim();
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
                        #endregion

                        //715044695210
                        #region BENS 55
                        if (cmbPOItem.Text == "55")/*BENS*/
                        {
                            #region BENS
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
                                        //upc = cmbPOItem.Text;
                                        EtiquetaBens clase = new EtiquetaBens();
                                        clase.id = x[0].id;
                                        clase.po_varchar = x[0].po_varchar;
                                        clase.po = string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))).ToString();
                                        clase.poInCompleto = Convert.ToDecimal(string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))));
                                        clase.poItem = "55";
                                        clase.ProductCode = x[0].Estilo;
                                        clase.Size = x[0].size_izquierdo;
                                        clase.size_derecho = "";
                                        clase.TipoCarton = "0";
                                        clase.upc = x[0].UPC;
                                        upc = x[0].UPC;
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

                                        clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);
                                        id_inventarioAnt = Convert.ToInt32(clase.id_string);
                                        int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                                        //clase.id_string = x[0].Estilo + clase.size_izquierdo + clase.size_derecho +  clase.QUANTITY + clase.id ;
                                        ///ASSEMBLY ES EL CODIGO DE BARRAS BENDAVIS
                                        ///Barcode format (Code 128): [CUT#/PO#]|[STYLE#]|[SIZE]|[LENGTH]|[QTY]|[CARTON#]
                                        ///Container 1: Cut SS1510, Cartons 1-100
                                        ///Container 2: Cut SS1510, Cartons 101 - 200
                                        ///Container 3: Cut SS1510, Cartons 201 - 300
                                        ///




                                        clase.assembly = "*" + clase.id_string + "*";
                                        // clase.anio = x[0].Estilo + ":" + clase.size_izquierdo + ":" + clase.size_derecho + ":" + clase.QUANTITY  + ":" + clase.id ;
                                        clase.Vendor = clase.anio;
                                        DateTime fechaEntrada = DateTime.Now;

                                        clase.anio = fechaEntrada.Year + clase.id.ToString();


                                        List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                                        listclaseZ.Add(clase);
                                        //id_inventarioAnt = clase.id_inventario;

                                        ReporteCajaBens report = new ReporteCajaBens
                                        {
                                            DataSource = listclaseZ
                                        };
                                        report.PrintingSystem.ShowMarginsWarning = false;
                                        ReportPrintTool tool = new ReportPrintTool(report);
                                        //tool.ShowPreview();
                                        //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                        tool.Print(); //imprime de golpe
                                        tool.Print(); //imprime de golpe

                                        LimpiarPantallaEscaneo();

                                        contador = 0;
                                        txtUPCScann.Text = string.Empty;
                                        txtUPCScann.Focus();
                                        cmbSizes.SelectedIndex = 1;

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
                                    txtSize.Text = Talla;
                                    cmbSizes.Text = Talla;
                                    txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                                    txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsReq.Text) - contador).ToString();
                                    txtUPCScann.Text = string.Empty;
                                    txtUPCScann.Focus();
                                    sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";

                                }
                                else
                                {
                                    contador = contador + 1;
                                    txtSize.Text = Talla;
                                    cmbSizes.Text = Talla;
                                    dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text) + 1)].Selected = true;
                                    dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text) + 1);
                                    txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                                    txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsReq.Text) - contador).ToString();
                                    txtUPCScann.Text = string.Empty;
                                    txtUPCScann.Focus();
                                    sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";

                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion

                        #region SAFTECH  33
                        if (cmbPOItem.Text == "33")/*SAFTECH*/
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

                                        clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);

                                        clase.id_string = x[0].Estilo + clase.size_izquierdo + clase.size_derecho + clase.QUANTITY + clase.id;

                                        clase.assembly = "*" + clase.id_string + "*";

                                        clase.anio = x[0].Estilo + ":" + clase.size_izquierdo + ":" + clase.size_derecho + ":" + clase.QUANTITY + ":" + clase.id;
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
                        #endregion



                        if (upc == txtUPCScann.Text && Convert.ToInt64(txtUnitsScan.Text) < Convert.ToInt64(txtUnitsReq.Text))
                        {
                            //LEVIS
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
                            }
                        }
                        else
                        if
                            (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text) && txtUPCScann.Text.ToUpper() == upc)
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
                        #region WRANGLER /LEVIS
                        if (cmbPOItem.Text == "1000" && (Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text)))
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
                                terminado = cmbTerminado.Text,
                                estilo = consulta[0].ProductCode,
                                TipoCarton = consulta[0].color,
                                color = consulta[0].color
                            };

                            if (chkImprimirEtiqueta.Checked == true)
                            {
                                ///do {
                                try
                                {
                                    clase.Carton = Convert.ToInt64(Interaction.InputBox("Captura Numero de Carton", "Carton", "", 5, 5));
                                }
                                catch (Exception)
                                {
                                    /*clase.Carton = 0; MessageBox.Show("Favor de ingresar correctamente el numero de carton ya que no se ha guardado");*/
                                };

                                clase.assembly = consulta[0].Assembly;
                                clase.Vendor = consulta[0].Vendor;
                                clase.ShipTo = consulta[0].ShipTo;
                                clase.po_varchar = "0";
                                clase.id_inventario = f.GuardaInventario(clase, usu[0].id);
                                //clase.id_inventario = 116;
                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
                                                                                    "&po=" + consulta[0].po +
                                                                                    "&ca=" + clase.Carton +
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
                                Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_inventario.ToString(), Color.Black, Color.White, 270, 180);
                                clase.qr = qrCode.GetGraphic(20);
                                clase.codigoBarras = codigoBarras;
                                clase.assembly = "*" + clase.id_inventario.ToString() + "*";
                                clase.po = cmbPO.Text;
                                listClase.Add(clase);
                                id_inventarioAnt = clase.id_inventario;

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
                            else
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
                                Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_inventario.ToString(), Color.Black, Color.White, 270, 180);

                                // Codigo.Alignment=BarcodeLib.AlignmentPositions.CENTER;

                                clase.codigoBarras = codigoBarras;
                                listClase.Add(clase);
                                id_inventarioAnt = clase.id_inventario;

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


                        #endregion

                    }
                    else if (cmbPOItem.Text == "44" && txtUPCScann.Text != "")/*OLDNAVY*/
                    {
                        #region OLDNAVY  44
                        try
                        {
                            if (PrendasExtra == 0 && Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text))
                            {
                                MessageBox.Show("Ya se escanearon el total de las prendas favor de guardar!");
                            }
                            else if (PrendasExtra == 0 && Convert.ToInt64(txtUnitsScan.Text) != Convert.ToInt64(txtUnitsReq.Text))
                            {
                                GuardarEnLista();

                                contador = contador + 1;
                                dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
                                dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
                                txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                                txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                                txtUPCScann.Text = string.Empty;
                                txtUPCScann.Focus();
                                btnImprimeOldNavy.Visible = true;
                            }
                            else if (PrendasExtra == 1 && Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text))
                            {
                                GuardarEnLista();

                                contador = contador + 1;
                                txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                                txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                                txtUPCScann.Text = string.Empty;
                                txtUPCScann.Focus();
                                btnImprimeOldNavy.Visible = true;
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }

                        #endregion
                    }
                    else if (marca == "IFR")
                    {
                        if (upc == txtUPCScann.Text && TallaMixta==0)
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
                                        id_inventarioAnt = Convert.ToInt32(clase.id);
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
                                        int PrendasDemas = Convert.ToInt32(prendas.totalprendas) - (Convert.ToInt32(prendas.prendasescaneadas)+ Convert.ToInt32(txtUnitsScan.Text));
                                        MessageBox.Show("Deben de ser "+ Convert.ToInt32(prendas.totalprendas) + " y se excedio "+PrendasDemas);
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
                            catch(Exception ex)
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
                            if (contador>0)
                            {
                                //Recorre tabla de lo escaneado
                                foreach (DataGridViewRow fila in dgvEscan.Rows)
                                {
                                    if (fila.Cells["Talla"].Value != null && fila.Cells["CantidadPrendas"].Value!=null &&
                                        fila.Cells["Talla"].Value.ToString().Equals(ListUpc[0].size_izquierdo, StringComparison.OrdinalIgnoreCase))
                                    {
                                        // Actualizar celda "CantidadPrendas"
                                        fila.Cells["CantidadPrendas"].Value = Convert.ToInt32(fila.Cells["CantidadPrendas"].Value) +1;                                        
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
                                tablaEscaner.Rows.Add(ListUpc[0].size_izquierdo,ListUpc[0].po,ListUpc[0].upc, "1", ListUpc[0].color,ListUpc[0].estilo);
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
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message.ToString());
            }
        }

        public void GuardarEnLista()
        {
            List<ConsultaUPC_VarcharResult> ListUpc;
            string po_numero = cmbPO.Text;
            ListUpc = f.ConsultaUPC(po_numero, txtUPCScann.Text);
            if (ListUpc.Count > 0)
            {
                EtiquetaCajaModificada prenda = new EtiquetaCajaModificada();
                prenda.po_varchar = ListUpc[0].po;
                prenda.poItem = "44";
                prenda.ProductCode = cmbProductCode.Text;
                prenda.upc = ListUpc[0].upc;
                prenda.usuario = usu[0].nombre;
                prenda.Cantidad = 1;
                prenda.size_izquierdo = ListUpc[0].size_izquierdo;
                prenda.size_derecho = ListUpc[0].size_derecho;
                prenda.Size = prenda.size_izquierdo + " x " + prenda.size_derecho;
                if (cajaOlnavy.Count > 0)
                {
                    for (int indice = 0; indice < Convert.ToInt32(cajaOlnavy.Count); indice++)
                    {
                        if (cajaOlnavy[indice].Size == prenda.Size)
                        {
                            prenda.Cantidad = cajaOlnavy[indice].Cantidad + 1;
                            cajaOlnavy.RemoveAt(indice);
                        }
                    }
                }
                cajaOlnavy.Add(prenda);
                txtSize.Text = prenda.Size;
            }
            else
            {
                MessageBox.Show("Favor de Escanear la prenda correcta!");
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
                    //cmbClienteB.Enabled = true;
                    //cmbFacturacionB.Enabled = true;
                    //cmbTerminadoB.Enabled = true;
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
                    {
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

                        tablaBens = new DataTable();
                        tablaBens.Columns.Add("Talla", typeof(string));
                        tablaBens.Columns.Add("Codigo UPC", typeof(string));
                        tablaBens.Columns.Add("idSize", typeof(int));
                        tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                        tablaBens.Columns.Add("itemDescription", typeof(string));
                        tablaBens.Columns.Add("color", typeof(string));

                        tablaIFR = new DataTable();
                        tablaIFR.Columns.Add("Talla", typeof(string));
                        tablaIFR.Columns.Add("Codigo UPC", typeof(string));
                        tablaIFR.Columns.Add("idSize", typeof(int));
                        tablaIFR.Columns.Add("CantidadPrendas", typeof(long));
                        tablaIFR.Columns.Add("itemDescription", typeof(string));
                        tablaIFR.Columns.Add("color", typeof(string));
                        tablaIFR.Columns.Add("CantidadPrendasTotal", typeof(string));


                        tablaTarget = new DataTable();
                        tablaTarget.Columns.Add("Talla", typeof(string));
                        tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                        tablaTarget.Columns.Add("idSize", typeof(int));
                        tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                        tablaTarget.Columns.Add("itemDescription", typeof(string));
                        tablaTarget.Columns.Add("color", typeof(string));
                        Tabpage = Tabpage + 1;
                        cmbMarca.SelectedIndex = 0;


                        f.ConsultaTallasXMarca(cmbzTallas, "LEVIS");
                        iniciando = false;
                    }
                    else
                    {

                    }
                }
                if (TabEsaneo.SelectedTab.Text.Trim() == "ModificarPO")
                {
                    if (Tabpage == 0)
                    {
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


                        tablaIFR = new DataTable();
                        tablaIFR.Columns.Add("Talla", typeof(string));
                        tablaIFR.Columns.Add("Codigo UPC", typeof(string));
                        tablaIFR.Columns.Add("idSize", typeof(int));
                        tablaIFR.Columns.Add("CantidadPrendas", typeof(long));
                        tablaIFR.Columns.Add("itemDescription", typeof(string));
                        tablaIFR.Columns.Add("color", typeof(string));
                        tablaIFR.Columns.Add("CantidadPrendasTotal", typeof(string));


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
                if (cmbPOItem.Text != "99" && cmbPOItem.Text != "77" && cmbPOItem.Text != "44")
                {
                    if (id_inventarioAnt != 0 && Convert.ToInt32(txtCartonsPacked.Text) > 0)
                    {
                        txtUPCScann.Text = string.Empty;
                        txtUPCScann.Focus();
                        List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(Convert.ToInt32(id_inventarioAnt));
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();

                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                        {
                            po = consulta[0].po,
                            poInCompleto = Convert.ToDecimal(consulta[0].poInCompleto),
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
                            id_inventario = consulta[0].id,
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
                            id_inventarioAnt = clase.id_inventario;

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
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
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
                else if (cmbPOItem.Text == "88")
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
                else if (cmbPOItem.Text == "44")
                {
                    //btnImprimeOldNavy.Visible = false;
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
                btnImprimeOldNavy.Visible = false;
            }
            btnImprimeOldNavy.Visible = false;
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
                    if (consulta[0].poItem != "77" && consulta[0].poItem != "88" && consulta[0].poItem != "44" && consulta[0].poItem != "99" && consulta[0].poItem != "55" && consulta[0].poItem != "66" && consulta[0].poItem != "33" && consulta[0].poItem != "22")
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                        {
                            poInCompleto = Convert.ToDecimal(consulta[0].poInCompleto),
                            po = consulta[0].po,
                            poItem = consulta[0].poItem,
                            ProductCode = consulta[0].ProductCode,
                            Size = consulta[0].Size,
                            size_derecho = consulta[0].size_derecho,
                            size_izquierdo = consulta[0].size_izquierdo,
                            upc = consulta[0].upc,
                            Fecha = consulta[0].create_dtm,
                            CartonLeft = consulta[0].CartonLeft,
                            CartonRight = consulta[0].CartonRight,
                            Cantidad = consulta[0].Cantidad,
                            Carton = consulta[0].Carton,
                            usuario = consulta[0].usuario,
                            id_inventario = consulta[0].id,
                            id_cliente = Convert.ToInt32(consulta[0].id_cliente),
                            id_factura = Convert.ToInt32(consulta[0].id_factura),
                            id_terminado = Convert.ToInt32(consulta[0].id_terminado),
                            cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente,
                            factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura,
                            terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado,
                            estilo = consulta[0].ProductCode,
                            color = consulta[0].TipoCarton
                        };
                        if (clase.poItem == "1000" && clase.poItem != "99")
                        {
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
                                                                                "&po=" + clase.po +
                                                                                "&ca=" + clase.Carton +
                                                                                "&fa=" + clase.factura +
                                                                                "&te=" + clase.terminado +
                                                                                "&u=" + clase.usuario +
                                                                                "&pc=" + clase.ProductCode +
                                                                                "&c=" + clase.Cantidad +
                                                                                "&sz=" + clase.Size +
                                                                                "&fe=" + clase.Fecha
                                                                                , QRCodeGenerator.ECCLevel.Q);
                            QRCode qrCode = new QRCode(qrCodeData);
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                            {
                                IncludeLabel = true
                            };
                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_inventario.ToString(), Color.Black, Color.White, 270, 180);
                            clase.qr = qrCode.GetGraphic(20);
                            clase.codigoBarras = codigoBarras;
                            clase.assembly = "*" + clase.id_inventario.ToString() + "*";
                            //clase.po = cmbPO.Text;

                            listClase.Add(clase);
                            id_inventarioAnt = clase.id_inventario;

                            ReporteCaja report = new ReporteCaja
                            {
                                DataSource = listClase
                            };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            tool.Print(); //imprime de golpe
                        }
                        /*
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
                                //clase.numeroEtiqueta1 = consulta[0].upc.Substring(0, 7);
                                //clase.numeroEtiqueta2 = consulta[0].upc.Substring(7, 7);
                                //clase.numeroEtiqueta3 = consulta[0].upc.Substring(0, 4) + " - " + consulta[0].upc.Substring(4, 2);
                                //clase.upc = "0" + consulta[0].upc.Substring(0, 6).ToString() + clase.numeroEtiqueta2;
                                clase.upc = consulta[0].upc.ToString();
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
                            id_inventarioAnt = clase.id_inventario;
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
                       */
                    }
                    else if (consulta[0].poItem == "99")
                    {
                        /*A VER SI SE QUITA O NOO*/
                        EtiquetaZUMIES claseZ = new EtiquetaZUMIES();
                        claseZ.id = consulta[0].id;
                        claseZ.assembly = consulta[0].po.ToString();
                        claseZ.poInCompleto = Convert.ToInt32(consulta[0].po);
                        claseZ.poItem = "99";
                        claseZ.ProductCode = "ASSORTMENT";
                        claseZ.Size = "ASSORTMENT";
                        claseZ.ESTILO = "ASSORTMENT";
                        claseZ.QUANTITY = Convert.ToString(consulta[0].Cantidad);

                        /*SI NO SE PONE X*/
                        claseZ.CARTON_NUMBER_INICIAL = (consulta[0].Carton).ToString();
                        claseZ.CARTON_NUMBER_FINAL = Convert.ToString(consulta[0].TotalCajas);
                        /*EN VEZ DE LISTAPO */

                        claseZ.COUNTRY = "MEXICO";
                        claseZ.Cantidad = Convert.ToDecimal(consulta[0].Cantidad);
                        claseZ.DPCI = consulta[0].TipoCarton;
                        claseZ.itemDescription = claseZ.DPCI;
                        claseZ.cn_tag_num = Convert.ToInt32(consulta[0].Carton);
                        claseZ.color = "";
                        claseZ.size_izquierdo = "ASSORTMENT";
                        string contarDigitos = "00" + consulta[0].upc.Substring(0, 11) + "1";
                        claseZ.Carton = consulta[0].Carton;
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
                        id_inventarioAnt = Convert.ToInt32(consulta[0].id);
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
                        clase2.id_inventario = Convert.ToInt32(claseZ.id);
                        clase2.po = consulta[0].po;
                        clase2.poInCompleto = Convert.ToInt32(claseZ.CARTON_NUMBER_INICIAL);
                        clase2.cliente = "";
                        clase2.factura = "";
                        clase2.terminado = "";
                        clase2.usuario = usu[0].nombre;
                        clase2.ProductCode = claseZ.ProductCode;
                        clase2.Cantidad = Convert.ToDecimal(claseZ.QUANTITY);
                        clase2.Size = "ASSORTMENT";
                        clase2.Fecha = DateTime.Now;
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

                    else if (consulta[0].poItem == "77")
                    {
                        /*zumies*/
                        List<ConsultaInventarioIDResult> x = f.ConsultaInventarioID(idInv);
                        if (x.Count > 0)
                        {
                            EtiquetaZUMIES clase = new EtiquetaZUMIES();
                            clase.id = x[0].id;
                            clase.po = x[0].po;
                            clase.poInCompleto = Convert.ToDecimal(x[0].poInCompleto);
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
                            clase.assembly = "*" + clase.CARTON_NUMBER_INICIAL.ToString() + "*";

                            List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();
                            listClase.Add(clase);
                            id_inventarioAnt = clase.id_inventario;

                            ReporteCajaZumines report = new ReporteCajaZumines
                            {
                                DataSource = listClase
                            };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            tool.Print(); //imprime de golpe

                        }
                    }

                    else if (consulta[0].poItem == "66")
                    {
                        /*ROCKY*/
                        List<ConsultaInventarioIDResult> x1 = f.ConsultaInventarioID(idInv);
                        if (x1.Count > 0)
                        {
                            EtiquetaROCKY claseZ = new EtiquetaROCKY();
                            claseZ.Fecha = DateTime.Now;
                            claseZ.id = x1[0].id;
                            claseZ.po = x1[0].po;
                            string poSoloNumeros = String.Empty;
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
                            claseZ.QUANTITY = Convert.ToString(x1[0].Cantidad);
                            claseZ.Cantidad = x1[0].Cantidad;
                            claseZ.cn_tag_num = Convert.ToInt32(x1[0].Carton);
                            claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                            claseZ.CARTON_NUMBER_FINAL = x1[0].TotalCajas.ToString();
                            claseZ.COUNTRY = "MEXICO";
                            string codigoBarras = "*" + x1[0].Carton.ToString() + "*";

                            //codigoBarras = "*" + "250" + "*";
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

                        }
                    }
                    else if (consulta[0].poItem == "88")
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
                                        string poSoloNumeros = String.Empty;
                                        foreach (var c in consulta[0].po.ToString())
                                        {
                                            if (char.IsNumber(c))
                                            {
                                                poSoloNumeros += c;
                                            }
                                        }
                                        claseZ.poInCompleto = Convert.ToDecimal(poSoloNumeros);
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

                                        claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                                        claseZ.CARTON_NUMBER_FINAL = x1[0].TotalCajas.ToString();

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
                                        clase2.id_inventario = Convert.ToInt32(claseZ.id);
                                        clase2.po = consulta[0].po.ToString();
                                        clase2.poInCompleto = Convert.ToDecimal(consulta[0].po);
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
                                    claseZ.poInCompleto = Convert.ToDecimal(x1[0].poInCompleto);
                                    claseZ.poItem = "88";
                                    claseZ.Size = x1[0].size_izquierdo;
                                    claseZ.po = x1[0].po;
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
                                    claseZ.assembly = x1[0].po.ToString();
                                    string contarDigitos = "00" + x1[0].upc.Substring(0, 11) + "1";
                                    claseZ.CARTON_NUMBER_INICIAL = x1[0].Carton.ToString();
                                    claseZ.CARTON_NUMBER_FINAL = x1[0].TotalCajas.ToString();

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
                                    clase2.poInCompleto = x1[0].Carton;
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
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
                    /*BEANS REIMPRIMIR*/
                    else if (consulta[0].poItem == "55")
                    {
                        #region BENS
                        /*UPC CORRECTO*/
                        try
                        {
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                            List<ConsultaProductosBensIDResult> x = f.ConsultaProductosBensID(idInv);
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
                                clase.po = string.Concat(x[0].po_varchar.Where(c => Char.IsDigit(c))).ToString();
                                //Convert.ToDecimal();
                                clase.po_varchar = x[0].po_varchar.Trim();
                                clase.poInCompleto = Convert.ToDecimal(string.Concat(x[0].po_varchar.Where(c => Char.IsDigit(c))));
                                clase.poItem = "55";
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

                                //clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id);

                                clase.id_string = clase.id.ToString();// x[0].Estilo + clase.size_izquierdo + clase.size_derecho +  clase.QUANTITY + 

                                clase.assembly = "*" + clase.id_string + "*";

                                clase.anio = x[0].Estilo + ":" + clase.size_izquierdo + ":" + clase.size_derecho + ":" + clase.QUANTITY + ":" + clase.id;
                                //id_inventarioAnt = Convert.ToInt32(clase.id_string);


                                DateTime fechaEntrada = DateTime.Now;

                                clase.anio = fechaEntrada.Year + clase.id.ToString();
                                //CAMBIAR EN OTROS TAMBIEN 
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                                List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                                listclaseZ.Add(clase);
                                //id_inventarioAnt = clase.id_inventario;
                                // id_inventarioAnt = Convert.ToInt32(clase.id_string);
                                ReporteCajaBens report = new ReporteCajaBens
                                {
                                    DataSource = listclaseZ
                                };
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                tool.Print(); //imprime de golpe
                                tool.Print(); //imprime de golpe

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

                        #endregion
                    }
                    /*SAFTECH  REIMPRIMIR*/
                    else if (consulta[0].poItem == "33")
                    {
                        #region SAFTECH 
                        /*UPC CORRECTO*/
                        try
                        {
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                            List<ConsultaProductosBensIDResult> x = f.ConsultaProductosBensID(idInv);
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
                                clase.po = x[0].PO;

                                clase.po_varchar = x[0].PO.ToString();
                                clase.poInCompleto = Convert.ToDecimal(x[0].PO.ToString());
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
                                clase.itemDescription = x[0].Estilo;

                                clase.color = x[0].Color;
                                clase.size_izquierdo = x[0].size_izquierdo;
                                clase.size_derecho = x[0].size_derecho;
                                clase.cn_tag_num = Convert.ToInt32(x[0].CartonNumber);

                                //clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id);
                                clase.assembly = "*" + x[0].id + "*";


                                clase.anio = DateTime.Now.Year.ToString() + x[0].id;
                                id_inventarioAnt = Convert.ToInt32(clase.id_string);
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                                List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                                listclaseZ.Add(clase);
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

                        #endregion
                    }

                    #region OLDNAVY  44
                    else if (consulta[0].poItem == "44")/*OLDNAVY*/
                    {
                        #region OLDNAVY                        
                        try
                        {
                            /*TOTAL DE PRENDAS ESCANEADAS*/
                            string[] separadas;
                            separadas = cmbSizes.Text.Split('x');
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                            List<ConsultaProductosOLDNAVYResult> x = f.ConsultaProductosOLDNAVY(consulta[0].po_varchar, consulta[0].size_izquierdo.Trim(), consulta[0].size_derecho.Trim());
                            if (x.Count > 0)
                            {
                                contador = 0;
                                txtCartonSize.Text = "";
                                txtSize.Text = "";
                                txtProductCode.Text = x[0].Estilo.ToString();
                                anterior.Add(id);
                                upc = cmbPOItem.Text;
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
                                clase.id = x[0].id;
                                clase.po_varchar = x[0].po_varchar;
                                // clase.po = Convert.ToDecimal(cmbPO.Text);
                                //  clase.poInCompleto = Convert.ToDecimal(cmbPO.Text);
                                clase.poItem = "44";
                                clase.ProductCode = x[0].Estilo;
                                clase.TipoCarton = "0";
                                clase.upc = x[0].UPC;
                                clase.Fecha = DateTime.Now;
                                clase.CartonLeft = "";
                                clase.CartonRight = "";
                                clase.usuario = usu[0].nombre;
                                clase.cliente = cmbCliente.Text;
                                clase.factura = cmbFactura.Text;
                                clase.terminado = cmbTerminado.Text;
                                clase.Vendor = x[0].itemDescription;
                                clase.Cantidad = Convert.ToDecimal(x[0].Cantidad);
                                clase.itemDescription = x[0].itemDescription;
                                clase.color = x[0].Color;
                                clase.size_izquierdo = x[0].size_izquierdo;
                                clase.size_derecho = x[0].size_derecho;
                                clase.Size = clase.size_izquierdo + " x " + clase.size_derecho;
                                clase.cn_tag_num = Convert.ToInt32(x[0].CartonNumber);

                                clase.assembly = "*" + idInv + "*";
                                id_inventarioAnt = Convert.ToInt32(idInv);
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id +
                                                                                 "&po=" + clase.po_varchar +
                                                                                 "&u=" + clase.usuario +
                                                                                 "&pc=" + clase.ProductCode +
                                                                                 "&c=" + clase.Cantidad +
                                                                                 "&sz=" + clase.size_izquierdo + " x " + clase.size_derecho +
                                                                                 "&fe=" + clase.Fecha,
                                                                                 QRCodeGenerator.ECCLevel.Q);
                                QRCode qrCode = new QRCode(qrCodeData);

                                List<EtiquetaCajaModificada> listclaseZ = new List<EtiquetaCajaModificada>();
                                listclaseZ.Add(clase);
                                //id_inventarioAnt = clase.id_inventario;
                                id_inventarioAnt = Convert.ToInt32(idInv);
                                ReporteCajaOLDNAVY report = new ReporteCajaOLDNAVY
                                {
                                    DataSource = listclaseZ
                                };
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                tool.Print(); //imprime de golpe

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

                        #endregion
                    }
                    #endregion


                    /* IFR */
                    else if (consulta[0].poItem == "22")
                    {
                        //

                        EtiquetaCajaModificada clase = RellenaObjetoClaseReimpresion(consulta);
                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();



                        int? mixto=f.ConsultaMixto(clase.po, clase.estilo);

                        if (mixto == 1)
                        {

                        }
                        else
                        {
                            //string codigoBarras = clase.id_inventario.ToString();
                            //clase.assembly = codigoBarras;
                            //string contarDigitos = Regex.Replace(clase.upcCaja ?? "", @"\D", "");
                            UPCResult resultado = GeneraCodigoUPC(clase.upcCaja.ToString());
                            clase.codigoBarras = resultado.CodigoBarras;
                            listClase.Add(clase);
                            ReporteCajaIFR report = new ReporteCajaIFR
                            {
                                DataSource = listClase
                            };
                            // Disable margins warning. 
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);

                            tool.Print();

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

        public class UPCResult
        {
            public string UPCCompleto { get; set; }
            public Image CodigoBarras { get; set; }
        }

        public static UPCResult GeneraCodigoUPC(string upc)
        {
            // Limpia caracteres no numéricos
            string limpio = upc; 
            //Regex.Replace(upc ?? "", @"\D", "");

            if (string.IsNullOrEmpty(limpio))
                throw new ArgumentException("UPC vacío");

            // Si vienen 11 dígitos, calculamos el dígito de control
            if (limpio.Length == 11)
            {
                int checkDigit = CalculaCheckDigit(limpio);
                limpio += checkDigit.ToString();
            }

            // Si vienen 12 dígitos, validamos el dígito de control
            if (limpio.Length == 12 && !RevisaUPCA(limpio))
                throw new ArgumentException("UPC inválido: dígito de control incorrecto");

            // BarcodeLib espera 11 dígitos
            string onceDigitos = limpio.Substring(0, 11);

            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
            {
                IncludeLabel = true,
                LabelFont = new Font("Arial", 12, FontStyle.Bold),
                Alignment = BarcodeLib.AlignmentPositions.CENTER,
                LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER
            };

            // Ajusta el ancho y alto
            Codigo.BarWidth = 3;
            Codigo.Height = 120;
            Codigo.Width = 250;



            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.UPCA, onceDigitos);

            return new UPCResult
            {
                UPCCompleto = limpio,
                CodigoBarras = codigoBarras
            };
        }

        // Calcula el dígito de control UPC-A
        public static int CalculaCheckDigit(string upc11)
        {
            int sumOdd = 0, sumEven = 0;

            for (int i = 0; i < 11; i++)
            {
                int digit = upc11[i] - '0';
                if ((i % 2) == 0) sumOdd += digit;
                else sumEven += digit;
            }

            int total = (sumOdd * 3) + sumEven;
            return (10 - (total % 10)) % 10;
        }


        public static bool RevisaUPCA(string upc)
        {
            if (upc.Length != 12 || !long.TryParse(upc, out _))
                return false;

            int sumOdd = 0;
            int sumEven = 0;

            // Recorremos los primeros 11 dígitos
            for (int i = 0; i < 11; i++)
            {
                int digit = upc[i] - '0';
                if ((i % 2) == 0) // posiciones impares (0,2,4...) en índice
                    sumOdd += digit;
                else
                    sumEven += digit;
            }

            int total = (sumOdd * 3) + sumEven;
            int checkDigit = (10 - (total % 10)) % 10;

            // Comparar con el último dígito
            return checkDigit == (upc[11] - '0');
        }



        private void btnBajaPO_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja el PO " + cmbPOB.Text + " " + "1" + " " + "1" + " " + "1", "Baja PO", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");

                    if (password == "Ain2022_")
                    {
                        bool baja = f.BajaPO(cmbPOB.Text, 1, 1, 1, usu[0].id);
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
            //    clase.id_inventario = f.GuardaInventario(clase, usu[0].id);


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
                 * 
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
                            poInCompleto = Convert.ToDecimal(consulta[0].poInCompleto),
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
                            id_inventario = consulta[0].id,
                            id_cliente = Convert.ToInt32(consulta[0].id_cliente),
                            id_factura = Convert.ToInt32(consulta[0].id_factura),
                            id_terminado = Convert.ToInt32(consulta[0].id_terminado),
                            cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente,
                            factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura,
                            terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado
                        };
                        QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
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
                     clase.id_inventario = consulta[0].id;
                     clase.id_cliente = Convert.ToInt32(consulta[0].id_cliente);
                     clase.id_factura = Convert.ToInt32(consulta[0].id_factura);
                     clase.id_terminado = Convert.ToInt32(consulta[0].id_terminado);
                     clase.cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente;
                     clase.factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura;
                     clase.terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado;
                     QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
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
                        poInCompleto = Convert.ToDecimal(consulta2[0].poInCompleto),
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
                        id_inventario = consulta2[0].id,
                        id_cliente = Convert.ToInt32(consulta2[0].id_cliente),
                        id_factura = Convert.ToInt32(consulta2[0].id_factura),
                        id_terminado = Convert.ToInt32(consulta2[0].id_terminado),
                        cliente = consulta2[0].cliente == string.Empty ? "NA" : consulta2[0].cliente,
                        factura = consulta2[0].factura == string.Empty ? "NA" : consulta2[0].factura,
                        terminado = consulta2[0].terminado == string.Empty ? "NA" : consulta2[0].terminado
                    };
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_inventario +
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
                                po = dr[0].ToString(),
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
                            clase.id_inventario = f.GuardaProducto(clase, usu[0].id);
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
                                    po = dr[0].ToString(),
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
                                clase.id_inventario = f.GuardaProductoLEVIS(clase, usu[0].id);
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


        public void importarContrato()
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
                    /*Validar tallas que existan si no que el sistema avise sobre esa talla ,y que el upc empieze con 19*/
                    /*
                    switch (cmbMarca.SelectedItem)
                    {
                        case "LEVIS":
                            if (validarAltaPO() == 1)
                            {
                                pd.po = txtzPOZumies.Text;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idusuario = usu[0].id;
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                TallaRepetida = f.GuardaAltaPO(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                            }
                            break;
                        case "ZUMIES":
                            if (validarZumies() == 1)
                            {
                                pd.po = txtzPOZumies.Text;
                                pd.estilo = txtzEstilo.Text;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[2].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                pd.itemDescription = renglon.Cells[5].Value.ToString();
                                pd.color = renglon.Cells[6].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                TallaRepetida = f.GuardaZumies(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada Favor de dar de Baja la Talla");
                                    TallaRepetida = 0;
                                }
                            }
                            break;
                        case "ROCKY":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;

                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[2].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                pd.Size = renglon.Cells[0].Value.ToString();
                                pd.itemDescription = renglon.Cells[5].Value.ToString();
                                pd.color = renglon.Cells[6].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                pd.marca = "ROCKY";
                                TallaRepetida = f.GuardarPO(pd);/// guardar para ROCKY Y OLDNAVY

                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                                //cmbPO.SelectedIndex = -1;
                            }
                            break;
                        case "OLDNAVY":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;
                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                pd.CantidadCajas = Convert.ToDecimal(0);
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.color = renglon.Cells[5].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                pd.marca = "OLDNAVY";
                                TallaRepetida = f.GuardarPO(pd);/// guardar para ROCKY Y OLDNAVY
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                                //cmbPO.SelectedIndex = -1;

                            }
                            break;
                        case "TARGET":
                            if (validarAltaPO() == 1)
                            {
                                pd.po = txtzPOZumies.Text;
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                pd.assembly = txtzPOZumies.Text;
                                TallaRepetida = f.GuardarTarget(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                            }
                            break;
                        case "BENS":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;
                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                //pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.color = renglon.Cells[5].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                TallaRepetida = f.GuardaBENS(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //cmbPO.SelectedIndex = -1;
                            }
                            break;
                        case "SAFTECH":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;
                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                //pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.color = renglon.Cells[5].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                TallaRepetida = f.GuardarSaftech(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //cmbPO.SelectedIndex = -1;
                            }
                            break;
                    }*/
                    f.ConsultaPO(cmbPO);
                    cmbPO.SelectedIndex = -1;
                    MessageBox.Show("Termino con Exito!");

                    if (Contador > 1)
                    {
                        try
                        {
                            string upc = dr[3].ToString();
                            if (upc.Length > 5)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
                                clase.po = dr[0].ToString();
                                clase.poInCompleto = Convert.ToDecimal(clase.po);
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
                                clase.id_inventario = f.GuardaProductoLEVIS(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                    /*****/
                }
            }
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
        public void BuscarPOTallasSeparadas()
        {

            try
            {
                List<ConsultaInventarioTallaSeparadaResult> inv = f.ConsultaInventarioTallasSeparadas(dtpFechaInicioPOTallasSeparadas.Value.Date, dtpFechaFinPOTallasSeparadas.Value.Date);

                gcPOTallasSeparadas.DataSource = inv;
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
            int idPrepack = 0;
            int banderaprepack = 0;
            string po = "";
            int bandera = 0;
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
                                if (dr[6].ToString() == "")
                                {
                                    /* NO ES PREPACK*/
                                    EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
                                    clase.esprepack = 0;
                                    clase.po = dr[0].ToString();
                                    po = clase.po.ToString();
                                    clase.upc = dr[1].ToString();
                                    clase.size_izquierdo = dr[2].ToString().Trim();
                                    clase.DPCI = dr[3].ToString();
                                    clase.Cantidad = Convert.ToInt64(dr[4].ToString());
                                    clase.CantidadCajas = Convert.ToInt64(dr[5].ToString());
                                    clase.barcode = dr[6].ToString();
                                    clase.Fecha = DateTime.Now;
                                    clase.usuario = usu[0].nombre;
                                    clase.id_inventario = f.GuardaProductoTARGET(clase, usu[0].id);
                                }
                                else
                                {
                                    if (idPrepack == 0)
                                    {
                                        Prepack p = new Prepack();
                                        p.po_numero = dr[0].ToString();
                                        p.estilo = "88";
                                        p.DPCI = dr[3].ToString();
                                        p.barcode = dr[6].ToString();
                                        idPrepack = f.GuardarPrePack(p);
                                    }
                                    /* ES PREPACK*/
                                    int cantidad = Convert.ToInt32(dr[4].ToString());
                                    for (int i = cantidad; i > 0; i--)
                                    {
                                        PrepackDetalle pd = new PrepackDetalle();
                                        pd.idPrepack = idPrepack;
                                        pd.size = dr[2].ToString().Trim();
                                        pd.cantidad = 1;
                                        pd.CantidadCajas = 0;
                                        if (bandera == 0)
                                        {
                                            pd.CantidadCajas = Convert.ToInt64(dr[5].ToString());
                                            bandera = bandera + 1;
                                        }
                                        pd.upc = dr[1].ToString();
                                        pd.idusuario = usu[0].id;
                                        pd.idSize = Convert.ToInt32(0);

                                        f.GuardarPrePackDetalle(pd);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }

            Cursor.Current = Cursors.Default;
            MessageBox.Show("Termino con Exito!");
            ActualizarPO(po);
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
                                    po = dr[9].ToString(),
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

                                // clase.id_inventario = f.GuardaProductoTARGET(clase, usu[0].id);
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
                btnzGuardar.Enabled = false;
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
                                    pd.po = txtzPOZumies.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.estilo = txtzEstilo.Text;
                                    pd.color = txtzColor.Text;

                                    TallaRepetida = f.GuardaAltaPO(pd);
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //f.ConsultaPO(cmbPO);
                                }
                                break;
                            case "WRANGLER":
                                if (validarAltaPO() == 1)
                                {
                                    pd.po = txtzPOZumies.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.estilo = txtzEstilo.Text;
                                    pd.color = txtzColor.Text;

                                    TallaRepetida = f.GuardaWrangler(pd);
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //f.ConsultaPO(cmbPO);
                                }
                                break;
                            case "ZUMIES":
                                if (validarZumies() == 1)
                                {
                                    pd.po = txtzPOZumies.Text;
                                    pd.estilo = txtzEstilo.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                    pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.itemDescription = renglon.Cells[5].Value.ToString();
                                    pd.color = renglon.Cells[6].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    TallaRepetida = f.GuardaZumies(pd);
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada Favor de dar de Baja la Talla");
                                        TallaRepetida = 0;
                                    }
                                }
                                break;
                            case "ROCKY":
                                if (validarAltaPO() == 1)
                                {
                                    string poCompleto = txtzPOZumies.Text;
                                    string poSoloNumeros = String.Empty;

                                    pd.po = txtzPOZumies.Text;
                                    pd.po_varchar = poCompleto.Trim();
                                    pd.estilo = cmbEstilo.Text;
                                    pd.Vendor = poCompleto;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                    pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[2].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                    pd.Size = renglon.Cells[0].Value.ToString();
                                    pd.itemDescription = renglon.Cells[5].Value.ToString();
                                    pd.color = renglon.Cells[6].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    pd.marca = "ROCKY";
                                    TallaRepetida = f.GuardarPO(pd);/// guardar para ROCKY Y OLDNAVY

                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //f.ConsultaPO(cmbPO);
                                    //cmbPO.SelectedIndex = -1;
                                }
                                break;
                            case "OLDNAVY":
                                if (validarAltaPO() == 1)
                                {
                                    string poCompleto = txtzPOZumies.Text;
                                    string poSoloNumeros = String.Empty;
                                    pd.po = txtzPOZumies.Text;
                                    pd.po_varchar = poCompleto.Trim();
                                    pd.estilo = cmbEstilo.Text;
                                    pd.Vendor = poCompleto;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.CantidadCajas = Convert.ToDecimal(0);
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.color = renglon.Cells[5].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    pd.marca = "OLDNAVY";
                                    TallaRepetida = f.GuardarPO(pd);/// guardar para ROCKY Y OLDNAVY
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //f.ConsultaPO(cmbPO);
                                    //cmbPO.SelectedIndex = -1;

                                }
                                break;
                            case "TARGET":
                                if (validarAltaPO() == 1)
                                {
                                    pd.po = txtzPOZumies.Text;
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    pd.assembly = txtzPOZumies.Text;
                                    TallaRepetida = f.GuardarTarget(pd);///**/
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //f.ConsultaPO(cmbPO);
                                }
                                break;
                            case "BENS":
                                if (validarAltaPO() == 1)
                                {
                                    string poCompleto = txtzPOZumies.Text;
                                    string poSoloNumeros = String.Empty;
                                    pd.po = txtzPOZumies.Text;
                                    pd.po_varchar = poCompleto.Trim();
                                    pd.estilo = cmbEstilo.Text;
                                    pd.Vendor = poCompleto;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    //pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.color = renglon.Cells[5].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    TallaRepetida = f.GuardaBENS(pd);
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //cmbPO.SelectedIndex = -1;
                                }
                                break;
                            case "SAFTECH":
                                if (validarAltaPO() == 1)
                                {
                                    string poCompleto = txtzPOZumies.Text;
                                    string poSoloNumeros = String.Empty;
                                    pd.po = txtzPOZumies.Text;
                                    pd.po_varchar = poCompleto.Trim();
                                    pd.estilo = cmbEstilo.Text;
                                    pd.Vendor = poCompleto;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    //pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.color = renglon.Cells[5].Value.ToString();
                                    pd.escaneado = 0;
                                    pd.idusuario = usu[0].id;
                                    TallaRepetida = f.GuardarSaftech(pd);
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //cmbPO.SelectedIndex = -1;
                                }
                                break;
                            case "IFR":
                                if (validarIFR() == 1)
                                {
                                    string poCompleto = txtzPOZumies.Text;
                                    string poSoloNumeros = String.Empty;
                                    pd.po = txtzPOZumies.Text;
                                    pd.po_varchar = poCompleto.Trim();
                                    pd.estilo = cmbEstilo.Text;
                                    pd.Vendor = poCompleto;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[6].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.Size = renglon.Cells[0].Value.ToString();
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.color = renglon.Cells[5].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    pd.marca = "IFR";
                                    TallaRepetida = f.GuardaIFR(pd);


                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                }
                                break;
                        }
                        alta = 0;
                    }
                    f.ConsultaPO(cmbPO);
                    LimpiarCamposZumies();
                    cmbPO.SelectedIndex = -1;
                    cmbMarca.SelectedIndex = 0;
                    cmbMarca.SelectedIndex = 4;
                    ActualizarPO(txtzPOZumies.Text);
                    f.ConsultaPO(cmbPOB);
                    MessageBox.Show("Termino con Exito!");
                    cmbMarca.SelectedItem = "LEVIS";
                }
                btnzGuardar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnzGuardar.Enabled = true;
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

                cmbTallaMod.Text = "";
                txtCantPrendCaja.Text = "";
                txtCodigoUPC.Text = "";
                txtColorMod.Text = "";
                txtDescription.Text = "";
                txtEstiloPOMod.Text = "";
                cmbPOMod.Text = "";

                dgvZumies.Columns["itemDescription"].Visible = true;
                dgvZumies.Columns["color"].Visible = true;

                LimpiarCamposModificacionPO();

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
        public int validarIFR()
        {
            try
            {
                if (string.IsNullOrEmpty(txtzPOZumies.Text) || dgvZumies.RowCount < 1)
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
                    case "ZUMIES":

                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != "" && txtzCantidadCajas.Text != ""))
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

                                tablaZumies.Rows.Add(cmbzTallas.Text
                                                    , txtzCantidadCajas.Text
                                                    , txtzUPC.Text
                                                    , cmbzTallas.SelectedValue.ToString()
                                                    , txtzCantidadPrendas.Text
                                                    , txtzItemDescripcion.Text
                                                    , txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
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


                    case "ROCKY":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
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
                                //dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                //dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;
                    case "OLDNAVY":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaBens = new DataTable();
                                tablaBens.Columns.Add("Talla", typeof(string));
                                tablaBens.Columns.Add("Codigo UPC", typeof(string));
                                tablaBens.Columns.Add("idSize", typeof(int));
                                tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                                tablaBens.Columns.Add("itemDescription", typeof(string));
                                tablaBens.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                dgvZumies.Columns["itemDescription"].Visible = false;
                                dgvZumies.Columns["color"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                dgvZumies.Columns["itemDescription"].Visible = false;
                                dgvZumies.Columns["color"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;

                    case "BENS":

                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaBens = new DataTable();
                                tablaBens.Columns.Add("Talla", typeof(string));
                                tablaBens.Columns.Add("Codigo UPC", typeof(string));
                                tablaBens.Columns.Add("idSize", typeof(int));
                                tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                                tablaBens.Columns.Add("itemDescription", typeof(string));
                                tablaBens.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;

                    case "SAFTECH":

                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaBens = new DataTable();
                                tablaBens.Columns.Add("Talla", typeof(string));
                                tablaBens.Columns.Add("Codigo UPC", typeof(string));
                                tablaBens.Columns.Add("idSize", typeof(int));
                                tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                                tablaBens.Columns.Add("itemDescription", typeof(string));
                                tablaBens.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;

                    case "WRANGLER":
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


                    case "IFR":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaIFR = new DataTable();
                                tablaIFR.Columns.Add("Talla", typeof(string));
                                tablaIFR.Columns.Add("Codigo UPC", typeof(string));
                                tablaIFR.Columns.Add("idSize", typeof(int));
                                tablaIFR.Columns.Add("CantidadPrendas", typeof(long));
                                tablaIFR.Columns.Add("itemDescription", typeof(string));
                                tablaIFR.Columns.Add("color", typeof(string));
                                tablaIFR.Columns.Add("CantidadPrendasTotal", typeof(string));
                                altaZ = altaZ + 1;
                                tablaIFR.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text, txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaIFR;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                            }
                            else
                            {
                                tablaIFR.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text, txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaIFR;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                            }
                        }
                        else
                        {

                        }
                        break;
                    case "LEVIS":
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
                            newDatarow.Cells[1].Value = txtzCantidadCajas.Text;
                            newDatarow.Cells[2].Value = txtzUPC.Text;
                            newDatarow.Cells[3].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[4].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[5].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[6].Value = txtzColor.Text;
                            break;

                        case "ROCKY":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzCantidadCajas.Text;
                            newDatarow.Cells[2].Value = txtzUPC.Text;
                            newDatarow.Cells[3].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[4].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[5].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[6].Value = txtzColor.Text;
                            break;
                        case "OLDNAVY":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzCantidadCajas.Text;
                            newDatarow.Cells[2].Value = txtzUPC.Text;
                            newDatarow.Cells[3].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[4].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[5].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[6].Value = txtzColor.Text;
                            break;


                        case "BENS":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;


                        case "SAFTECH":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;


                        case "LEVIS":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;

                        case "WRANGLER":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;


                        case "IFR":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;


                            /* REVISAR!!!!**/
                            break;
                        default:
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;



                            //cmbzTallas.Text = row.Cells[0].Value.ToString();
                            //txtzUPC.Text = row.Cells[1].Value.ToString();
                            //txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                            //txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                            //txtzColor.Text = row.Cells[5].Value.ToString();
                            //txtzCantidadCajas.Text = row.Cells[1].Value.ToString();




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

                switch (cmbMarca.Text)
                {
                    case "TARGET":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;

                        break;
                    case "ZUMIES":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[2].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        txtzColor.Text = row.Cells[6].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[4].Value.ToString();
                        break;
                    case "ROCKY":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        break;

                    case "OLDNAVY":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        break;
                    case "BENS":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;
                        break;
                    case "SAFTECH":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;
                        break;


                    default:
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;
                        break;
                }
                //if cmbMarca.Text="ZUMIES"

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cmbzTallas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMarca.Text == "BENS" || cmbMarca.Text == "ROCKY" || cmbMarca.Text == "OLDNAVY" || cmbMarca.Text == "SAFTECH")
            {
                if (iniciando)
                { }
                else
                {
                    txtzUPC.Text = f.ConsultaUPCBENS(Convert.ToInt32(cmbzTallas.SelectedValue));
                    txtzCantidadPrendas.Focus();
                }
                // Convert.ToInt32(cmbCliente.SelectedValue)

            }
            else if (cmbMarca.Text == "IFR")
            {
                txtzUPC.Text = f.ConsultaUPCIFR(Convert.ToInt32(cmbzTallas.SelectedValue));
                txtzCantidadPrendas.Focus();

            }
            else
            {
                txtzUPC.Focus();
            }


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
                            string poSoloNumeros = String.Empty;
                            foreach (var c in (clase.po.ToString()))
                            {
                                if (char.IsNumber(c))
                                {
                                    poSoloNumeros += c;
                                }
                            }
                            clase.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                            clase.poItem = "77";
                            clase.ProductCode = a.ProductCode;
                            clase.Size = a.size_izquierdo;
                            clase.ESTILO = a.ProductCode;
                            clase.DESCRIPTION = a.TipoCarton;
                            clase.QUANTITY = Convert.ToString(a.Cantidad);
                            clase.CARTON_NUMBER_INICIAL = a.NumeroInicial.ToString();
                            clase.CARTON_NUMBER_FINAL = Convert.ToString(a.TotalCajas);
                            clase.COUNTRY = "MEXICO";
                            clase.assembly = "*" + clase.CARTON_NUMBER_INICIAL.ToString() + "*";

                            List<EtiquetaZUMIES> listClase = new List<EtiquetaZUMIES>();

                            listClase.Add(clase);
                            id_inventarioAnt = clase.id_inventario;

                            ReporteCajaZumines report = new ReporteCajaZumines
                            {
                                DataSource = listClase
                            };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            tool.Print(); //imprime de golpe
                        }
                    }
                    else if (x[0].poItem == "88" || x[0].poItem == "99")
                    {
                        Cursor.Current = Cursors.WaitCursor;

                        List<EtiquetaCajaModificada> listClase2 = new List<EtiquetaCajaModificada>();
                        List<EtiquetaZUMIES> listClaseTARGET = new List<EtiquetaZUMIES>();
                        foreach (ConsultaInventarioPOResult a in x)
                        {
                            EtiquetaZUMIES clase = new EtiquetaZUMIES();
                            clase.id = a.id;
                            clase.po = a.po;
                            clase.poInCompleto = a.NumeroInicial;
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
                            clase.assembly = a.po.ToString();

                            clase.CARTON_NUMBER_INICIAL = a.NumeroInicial.ToString();
                            clase.CARTON_NUMBER_FINAL = a.TotalCajas.ToString();

                            string contarDigitos = "00" + a.upc.Substring(0, 11) + "1";

                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 14, FontStyle.Bold) };
                            Codigo.BarWidth = 3;

                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.ITF14, contarDigitos, Color.Black, Color.White, 350, 150);

                            clase.codigoBarras = codigoBarras;//cropImage;
                            listClaseTARGET.Add(clase);

                            //////********************************************************imprime carton interno**************************************************************************************************************************////

                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada();
                            clase2.id_inventario = Convert.ToInt32(clase.id);
                            clase2.po = clase.po;
                            clase2.poInCompleto = Convert.ToInt32(clase.CARTON_NUMBER_INICIAL);
                            clase2.cliente = "";
                            clase2.factura = "";
                            clase2.terminado = "";
                            clase2.usuario = usu[0].nombre;
                            clase2.ProductCode = clase.ProductCode;
                            clase2.Cantidad = Convert.ToDecimal(clase.QUANTITY);
                            clase2.Size = clase.size_izquierdo;
                            clase2.Fecha = DateTime.Now;

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
                            clase2.assembly = "*" + clase.id.ToString() + "*";
                            listClase2.Add(clase2);
                            /////////////////////////////////////////////////////////////////////////////////*/*/*/*//////////////////////////

                        }


                        //id_inventarioAnt = Convert.ToInt32(idInv);
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



                        Cursor.Current = Cursors.WaitCursor;
                        Cursor.Current = Cursors.Default;
                    }
                    if (x[0].poItem == "55")
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        /*BEANS REIMPRIMIR*/
                        #region BENS
                        /*UPC CORRECTO*/
                        try
                        {
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS REIMPRESION DE TODOOO*/

                            // List<ConsultaProductosBensIDResult> x = f.ConsultaProductosBensID(idInv);
                            foreach (ConsultaInventarioPOResult a in x)
                            {
                                contador = 0;
                                txtCartonSize.Text = "";
                                txtSize.Text = "";
                                txtProductCode.Text = a.Estilo.ToString();
                                //anterior.Add(id);
                                upc = cmbPOItem.Text;
                                EtiquetaBens clase = new EtiquetaBens();
                                clase.id = a.id;
                                clase.po = txtPOImprimir.Text;

                                clase.po_varchar = a.po_varchar.Trim();
                                clase.poInCompleto = Convert.ToDecimal(txtPOImprimir.Text);
                                clase.poItem = "55";
                                clase.ProductCode = a.Estilo;
                                clase.Size = a.size_izquierdo;
                                clase.size_derecho = "";
                                clase.TipoCarton = "0";
                                clase.upc = a.upc;
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
                                clase.Vendor = a.itemDescription;
                                clase.ESTILO = a.Estilo;
                                clase.DESCRIPTION = a.itemDescription;
                                clase.QUANTITY = Convert.ToString(a.Cantidad);
                                clase.Cantidad = Convert.ToDecimal(a.Cantidad);
                                clase.itemDescription = a.itemDescription;
                                clase.color = a.Color;
                                clase.size_izquierdo = a.size_izquierdo;
                                clase.size_derecho = a.size_derecho;
                                clase.cn_tag_num = Convert.ToInt32(a.NumeroInicial);

                                clase.id_string = clase.id.ToString();// x[0].Estilo + clase.size_izquierdo + clase.size_derecho +  clase.QUANTITY +
                                clase.assembly = "*" + clase.id_string + "*";
                                clase.anio = clase.id.ToString();// x[0].Estilo + ":" + clase.size_izquierdo + ":" + clase.size_derecho + ":" + clase.QUANTITY  + ":" + 
                                id_inventarioAnt = Convert.ToInt32(a.id);
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                                listclaseZ.Add(clase);
                                //id_inventarioAnt = clase.id_inventario;
                                id_inventarioAnt = Convert.ToInt32(clase.id_string);
                                ReporteCajaBens report = new ReporteCajaBens
                                {
                                    DataSource = listclaseZ
                                };
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                tool.Print(); //imprime de golpe
                                tool.Print(); //imprime de golpe

                            }
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        #endregion
                        Cursor.Current = Cursors.Default;
                    }
                    if (x[0].poItem == "33")
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        /*SAFTECH   REIMPRIMIR*/
                        #region SAFTECH  
                        /*UPC CORRECTO*/
                        try
                        {
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS REIMPRESION DE TODOOO*/
                            // List<ConsultaProductosBensIDResult> x = f.ConsultaProductosBensID(idInv);
                            foreach (ConsultaInventarioPOResult a in x)
                            {
                                contador = 0;
                                txtCartonSize.Text = "";
                                txtSize.Text = "";
                                txtProductCode.Text = a.ProductCode.ToString();
                                //anterior.Add(id);
                                upc = cmbPOItem.Text;
                                EtiquetaBens clase = new EtiquetaBens();
                                clase.id = a.id;
                                clase.po = txtPOImprimir.Text;

                                clase.po_varchar = a.po_varchar.Trim();
                                clase.poInCompleto = Convert.ToDecimal(txtPOImprimir.Text);
                                clase.poItem = "33";
                                clase.ProductCode = a.Estilo;
                                clase.Size = a.size_izquierdo;
                                clase.size_derecho = "";
                                clase.TipoCarton = "0";
                                clase.upc = a.upc;
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
                                clase.Vendor = a.itemDescription;
                                clase.ESTILO = a.ProductCode;
                                clase.DESCRIPTION = a.itemDescription;
                                clase.QUANTITY = Convert.ToString(a.Cantidad);
                                clase.Cantidad = Convert.ToDecimal(a.Cantidad);
                                clase.itemDescription = a.TipoCarton;
                                clase.color = a.Color;
                                clase.size_izquierdo = a.size_izquierdo;
                                clase.size_derecho = a.size_derecho;
                                clase.cn_tag_num = Convert.ToInt32(a.NumeroInicial);


                                //clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id);
                                clase.assembly = "*" + a.id + "*";

                                clase.anio = DateTime.Now.Year.ToString() + a.id;
                                id_inventarioAnt = Convert.ToInt32(a.id);
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

                            }
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                        #endregion
                        Cursor.Current = Cursors.Default;
                    }

                    if (x[0].poItem == "66")
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        /*rocky   REIMPRIMIR*/
                        #region ROCKY  
                        /*UPC CORRECTO*/
                        try
                        {
                            foreach (ConsultaInventarioPOResult a in x)
                            {
                                EtiquetaROCKY claseZ = new EtiquetaROCKY();
                                claseZ.Fecha = DateTime.Now;
                                claseZ.id = a.id;
                                claseZ.po = a.po;
                                string poSoloNumeros = String.Empty;
                                foreach (var c in a.po.ToString())
                                {
                                    if (char.IsNumber(c))
                                    {
                                        poSoloNumeros += c;
                                    }
                                }
                                claseZ.poInCompleto = Convert.ToDecimal(poSoloNumeros);
                                claseZ.poItem = "66";
                                claseZ.ProductCode = a.ProductCode;
                                claseZ.Size = a.Size.Trim();
                                claseZ.color = a.TipoCarton;
                                claseZ.ESTILO = a.ProductCode;
                                claseZ.DESCRIPTION = a.TipoCarton;
                                claseZ.QUANTITY = Convert.ToString(a.Cantidad);
                                claseZ.Cantidad = a.Cantidad;
                                claseZ.cn_tag_num = Convert.ToInt32(a.Carton);
                                claseZ.CARTON_NUMBER_INICIAL = a.Carton.ToString();
                                claseZ.CARTON_NUMBER_FINAL = a.TotalCajas.ToString();
                                claseZ.COUNTRY = "MEXICO";
                                string codigoBarras = "*" + a.Carton.ToString() + "*";
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

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        #endregion
                        Cursor.Current = Cursors.Default;


                    }
                    #region OLDNAVY  44
                    else if (x[0].poItem == "44")/*OLDNAVY*/
                    {
                        #region OLDNAVY 
                        try
                        {
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE OLDNAVY REIMPRESION DE TODOOO*/
                            // List<ConsultaProductosBensIDResult> x = f.ConsultaProductosBensID(idInv);
                            foreach (ConsultaInventarioPOResult a in x)
                            {

                                contador = 0;
                                txtCartonSize.Text = "";
                                txtSize.Text = "";
                                txtProductCode.Text = a.ProductCode.ToString();
                                //anterior.Add(id);
                                upc = cmbPOItem.Text;
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
                                clase.id = a.id;
                                //clase.po = Convert.ToDecimal(txtPOImprimir.Text);

                                clase.po_varchar = a.po_varchar.Trim();
                                // clase.poInCompleto = Convert.ToDecimal(txtPOImprimir.Text);
                                clase.poItem = "44";
                                clase.ProductCode = a.Estilo;
                                clase.TipoCarton = "0";
                                clase.upc = a.upc;
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
                                clase.Vendor = a.itemDescription;
                                /*
                                clase.ESTILO = a.ProductCode;
                                clase.DESCRIPTION = a.itemDescription;
                                clase.QUANTITY = Convert.ToString(a.Cantidad);
                                */
                                clase.Cantidad = Convert.ToDecimal(a.Cantidad);
                                clase.itemDescription = a.TipoCarton;
                                clase.color = a.Color;
                                clase.size_izquierdo = a.size_izquierdo;
                                clase.size_derecho = a.size_derecho;
                                clase.Size = clase.size_izquierdo + " x " + clase.size_derecho;
                                clase.cn_tag_num = Convert.ToInt32(a.NumeroInicial);
                                clase.assembly = "*" + a.id + "*";

                                // clase.anio = DateTime.Now.Year.ToString() + a.id;
                                id_inventarioAnt = Convert.ToInt32(a.id);
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);
                                ////QRCodeGenerator qrGenerator = new QRCodeGenerator();
                                //QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id +
                                //                                                 "&po=" + clase.po_varchar +
                                //                                                 "&u=" + clase.usuario +
                                //                                                 "&pc=" + clase.ProductCode +
                                //                                                 "&c=" + clase.Cantidad +
                                //                                                 "&sz=" + clase.size_izquierdo+" x " + clase.size_derecho+
                                //                                                 "&fe=" + clase.Fecha,
                                //                                                 QRCodeGenerator.ECCLevel.Q);
                                //QRCode qrCode = new QRCode(qrCodeData);
                                //BarcodeLib.Barcode Codigo2 = new BarcodeLib.Barcode
                                //{
                                //    IncludeLabel = true
                                //};


                                List<EtiquetaCajaModificada> listclaseZ = new List<EtiquetaCajaModificada>();
                                listclaseZ.Add(clase);
                                //id_inventarioAnt = clase.id_inventario;
                                id_inventarioAnt = Convert.ToInt32(clase.id);
                                ReporteCajaOLDNAVY report = new ReporteCajaOLDNAVY
                                {
                                    DataSource = listclaseZ
                                };
                                report.PrintingSystem.ShowMarginsWarning = false;
                                ReportPrintTool tool = new ReportPrintTool(report);
                                //tool.ShowPreview();
                                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                                tool.Print(); //imprime de golpe

                            }
                        }

                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        #endregion
                    }
                    #endregion



                    else if (x[0].poItem == "1000")
                    {
                        QRCodeGenerator qrGenerator = new QRCodeGenerator();
                        List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                        foreach (ConsultaInventarioPOResult a in x)
                        {
                            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                            {
                                poInCompleto = Convert.ToDecimal(a.poInCompleto),
                                po = a.po,
                                poItem = a.poItem,
                                ProductCode = a.ProductCode,
                                Size = a.Size,
                                size_derecho = a.size_derecho,
                                size_izquierdo = a.size_izquierdo,
                                upc = a.upc,
                                Fecha = a.create_dtm,
                                CartonLeft = a.CartonLeft,
                                CartonRight = a.CartonRight,
                                Cantidad = a.Cantidad,
                                Carton = a.NumeroInicial,
                                usuario = a.usuario,
                                id_inventario = a.id,
                                id_cliente = Convert.ToInt32(a.id_cliente),
                                id_factura = Convert.ToInt32(a.id_factura),
                                id_terminado = Convert.ToInt32(a.id_terminado),
                                cliente = a.cliente == 0 ? "1" : a.cliente.ToString(),
                                factura = a.factura == 0 ? "1" : a.factura.ToString(),
                                terminado = a.terminado == 0 ? "1" : a.terminado.ToString(),
                                estilo = a.ProductCode,
                                color = a.TipoCarton
                            };
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
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
                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39, clase.id_inventario.ToString(), Color.Black, Color.White, 270, 180);
                            clase.qr = qrCode.GetGraphic(20);
                            clase.codigoBarras = codigoBarras;
                            clase.assembly = "*" + clase.id_inventario.ToString() + "*";
                            listClase.Add(clase);
                            id_inventarioAnt = clase.id_inventario;
                        }

                        ReporteCaja report = new ReporteCaja
                        {
                            DataSource = listClase
                        };
                        report.PrintingSystem.ShowMarginsWarning = false;
                        ReportPrintTool tool = new ReportPrintTool(report);
                        tool.Print(); //imprime de golpe
                    }
                    
                    
                    else if (x[0].poItem=="22")
                    {
                        foreach (ConsultaInventarioPOResult a in x)
                        {

                            EtiquetaCajaModificada clase = RellenaObjetoClaseREIMPRETODO(x);

                            List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                           // string contarDigitos = Regex.Replace(clase.upcCaja ?? "", @"\D", "");
                            UPCResult resultado = GeneraCodigoUPC(clase.upcCaja.ToString());
                            clase.codigoBarras = resultado.CodigoBarras;

                            listClase.Add(clase);

                            ReporteCajaIFR report = new ReporteCajaIFR
                            {
                                DataSource = listClase
                            };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            tool.Print(); //imprime de golpe
                        }

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
            ///***IFR PRENDAS MIXTAS
            if (cmbPO.Text != "")
            {
                string marca = f.ConsultaMarca(cmbPO.Text, cmbPOItem.Text);
                if (marca == "IFR")
                {
                    TallaMixta = 1;
                }
            }
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
                        txtzColor.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzItemDescripcion.Text = "";
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        txtzUPC.Enabled = true;
                        break;
                    case "ZUMIES":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        iniciando = true;
                        f.ConsultaTallasZumies(cmbzTallas);
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        break;
                    case "ROCKY":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        /* f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                         cmbzTallas.SelectedIndex = 0;*/
                        iniciando = true;
                        f.ConsultaEstiloRocky(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        txtzUPC.Enabled = false;
                        break;
                    case "OLDNAVY":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        iniciando = true;
                        f.ConsultaEstiloOldNavy(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        txtzUPC.Enabled = false;
                        txtzCantidadPrendas.Text = "18";
                        break;
                    case "CINTAS":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
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
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        break;

                    case "BENS":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        /* f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                         cmbzTallas.SelectedIndex = 0;*/
                        iniciando = true;
                        f.ConsultaEstiloBens(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        txtzUPC.Enabled = false;
                        break;
                    case "SAFTECH":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        /* f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                         cmbzTallas.SelectedIndex = 0;*/
                        iniciando = true;
                        f.ConsultaEstiloSaftech(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        break;
                    case "WRANGLER":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        f.ConsultaTallasXMarca(cmbzTallas, "WRANGLER");
                        cmbzTallas.SelectedIndex = 0;
                        txtzUPC.Enabled = true;
                        break;

                    case "IFR":
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        f.ConsultaEstiloIFR(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        cmbzTallas.Visible = true;
                        cmbzTallas.Enabled = true;
                        f.ConsultaTallasIFR(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        txtzColor.Enabled = true;
                        txtzColor.Visible = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzItemDescripcion.Visible = true;
                        txtzCantidadPrendas.Enabled = true;
                        txtzUPC.Enabled = false;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        lblCantidadCajas.Text = "Cantidad Total\nprendas x Talla:";
                        //lblCantidadCajasModif
                        txtzEstilo.Enabled = false;
                        iniciando = true;
                        iniciando = false;
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

        private void button6_Click_1(object sender, EventArgs e)
        {
            #region TARGET IMPRESION MANUAL
            /*
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
            */
            #endregion

            btnImprimeOldNavy.Visible = false;
            int contadorOldNavy = 0;
            foreach (EtiquetaCajaModificada prenda in cajaOlnavy)
            {
                prenda.Fecha = DateTime.Now;

                if (contadorOldNavy > 0)
                {
                }
                else
                {
                    prenda.TipoCarton = "1";
                    prenda.cn_tag_num = 1;
                    contadorOldNavy = 1;
                }

                f.GuardaInventario(prenda, usu[0].id);
            }
            cajaOlnavy = new List<EtiquetaCajaModificada>();
            cajaOlnavy.Clear();
            PrendasExtra = 0;
            LimpiarPantallaEscaneo();

            contador = 0;
            txtUPCScann.Text = string.Empty;
            btnImprimeOldNavy.Visible = false;
            txtUPCScann.Focus();



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
                //importarROCKY();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                                        clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id); clase.id = Convert.ToInt32(clase.id_string);
                                        clase.id_string = x[0].Estilo + clase.size_izquierdo + clase.size_derecho + clase.QUANTITY + clase.id;
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

        private void btnBajaTalla_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("Seguro que deseas dar de baja la talla " + cmbTallaBaja.Text + " del po " + cmbPOB.Text + " ", "Baja Talla", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string password = Microsoft.VisualBasic.Interaction.InputBox("Teclea el password ", "Contraseña", "");

                    if (password == "Ain2022_")
                    {
                        bool baja = f.BajaTallaPO(cmbPOB.Text, cmbTallaBaja.Text, usu[0].id);
                        if (baja)
                        {
                            MessageBox.Show("se elimino correctamente la talla ." + cmbTallaBaja.Text);
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
                if (iniciando)
                {

                }
                else
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


        private void cmbEstilo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (iniciando)
            {

            }
            else
            {
                /*
                 *  LEVIS
                    ZUMIES
                    TARGET
                    CINTAS
                    BENS
                    ROCKY
                    OLDNAVY
                 */
                ConsultaTallasColorResult x = new ConsultaTallasColorResult();
                ConsultaTallasColorIFRResult ColorIFR = new ConsultaTallasColorIFRResult();
                switch (cmbMarca.SelectedItem.ToString())
                {
                    case "LEVIS":
                        iniciando = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "ZUMIES":
                        iniciando = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "TARGET":
                        iniciando = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "BENS":
                        iniciando = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "SAFTECH":
                        iniciando = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "ROCKY":
                        iniciando = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasxMarcaxEstilo(cmbzTallas, "ROCKY", cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "OLDNAVY":
                        iniciando = true;
                        x = f.ConsultaTallasColor(cmbEstilo.SelectedValue.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasxMarcaxEstilo(cmbzTallas, "OLDNAVY", cmbEstilo.SelectedValue.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;

                    case "IFR":
                        iniciando = true;
                        ColorIFR = f.ConsultaTallasColorIFR(cmbEstilo.Text.ToString());
                        txtzColor.Text = ColorIFR.color;
                        txtzItemDescripcion.Text = ColorIFR.descripcion + "," + ColorIFR.descripcion2;
                        txtzCantidadPrendas.Text = ColorIFR.cantidadPrendas;

                        f.ConsultaTallasIFR(cmbzTallas, cmbEstilo.Text.ToString());
                        iniciando = false;
                        break;
                }

            }

        }

        private void btnConsultaTallasSeparadas_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarPOTallasSeparadas();
            Cursor.Current = Cursors.Default;
        }

        private void cmbMarcaPOMod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tpImportaLEVIS_Click(object sender, EventArgs e)
        {

        }

        private void btnTodaslasTallas_Click(object sender, EventArgs e)
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

                //foreach de todas las tallas

                int tallasCombo = cmbzTallas.Items.Count;
                for (int i = 0; i == tallasCombo; i++)
                {
                    cmbzTallas.SelectedIndex = i;
                    txtzUPC.Text = f.ConsultaUPCBENS(Convert.ToInt32(cmbzTallas.SelectedValue));
                    tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                }

                dgvZumies.DataSource = tablaZumies;
                //dgvZumies.Columns["idSize"].Visible = false;
                cmbzTallas.SelectedIndex = 0;
                txtzUPC.Text = "";

            }
            else
            {
                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                dgvZumies.DataSource = tablaZumies;
                //dgvZumies.Columns["idSize"].Visible = false;
                cmbzTallas.SelectedIndex = 0;
                txtzUPC.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label92_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardarEstiloAlta_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelarEstiloAlta_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregarEstiloAlta_Click(object sender, EventArgs e)
        {
            try
            {
                tablaEstiloAlta = new DataTable();
                tablaEstiloAlta.Columns.Add("Talla", typeof(string));
                tablaEstiloAlta.Columns.Add("Codigo UPC", typeof(string));
                tablaEstiloAlta.Columns.Add("idSize", typeof(int));
                tablaEstiloAlta.Columns.Add("CantidadPrendas", typeof(long));
                tablaEstiloAlta.Columns.Add("itemDescription", typeof(string));
                tablaEstiloAlta.Columns.Add("color", typeof(string));
                altaZ = altaZ + 1;
                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                dgvZumies.DataSource = tablaTarget;
                dgvZumies.Columns["idSize"].Visible = false;
                cmbzTallas.SelectedIndex = 0;
                txtzUPC.Text = "";

            }
            catch (Exception ex)
            {

            }
        }

        private void btnEliminaEstiloAlta_Click(object sender, EventArgs e)
        {

        }

        private void btnImportarEstilo_Click(object sender, EventArgs e)
        {
            try
            {
                importarEstilo();
                Cursor.Current = Cursors.Default;

                f.ConsultaPO(cmbPO);
                cmbPO.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


        public void importarEstilo()
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
            string estilo1 = "";
            string tipo = "";
            string color = "";
            string descripcion = "";

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
                                tallas clase = new tallas
                                {
                                    size_izquierdo = dr[0].ToString(),
                                    size_derecho = dr[1].ToString(),
                                    upc = dr[2].ToString(),
                                    tipo = dr[3].ToString(),
                                    estilo = dr[4].ToString(),
                                    color = dr[5].ToString(),
                                    descripcion = dr[6].ToString(),
                                };
                                f.GuardaTallaEstilos(clase);



                            }
                            estilo1 = dr[4].ToString();
                            tipo = dr[3].ToString();
                            color = dr[5].ToString();
                            descripcion = dr[6].ToString();
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            estilo Nuevoestilo = new estilo();

            Nuevoestilo.estilo1 = estilo1;
            Nuevoestilo.tipo = tipo;
            Nuevoestilo.color = color;
            Nuevoestilo.descripcion = descripcion;

            f.GuardaEstilos(Nuevoestilo);


            MessageBox.Show("Termino con Exito!");
        }

        private void chkImprimirEtiqueta_CheckedChanged(object sender, EventArgs e)
        {
            if (chkImprimirEtiqueta.Checked == true)
            {
                chkImprimirEtiqueta.Image = Properties.Resources.Check_Mark;
                //MessageBox.Show("Esta chekeado");
                imprimir = true;

            }
            else
            {
                chkImprimirEtiqueta.Image = Properties.Resources.Uncheckmark;
                //MessageBox.Show("No esta chekeado!");
                imprimir = false;
            }
        }

        private void Escaneo_Load(object sender, EventArgs e)
        {

        }
    }

}