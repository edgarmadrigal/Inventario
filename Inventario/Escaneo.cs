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
        private Int64? id_inventarioAnt = 0;
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
        private bool TallaMixtaTYNDALE = false;
        private DataTable tablaMixtaTYNDALE;
        // Lista que controla lo que se ve en dgvEscan para TYNDALE (filas originales + filas de tallas mixtas agregadas abajo)
        private List<ConsultaProductos_VarcharResult> listaEscaneoTYNDALE;
        private Prendas prendas = new Prendas();
        // Acumulador de caja mixta BEN DAVIS (55): Talla / Cantidad / UPC
        private DataTable tablaMixtaBENS;

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
                Text = Funciones.TituloConVersion(Text);
                ConfigurarGrids();
                PrepararFiltrosMarca();
                PrepararPestanaBajas();
                PrepararPestanaCorregir();
                PrepararExcelBENS();
                ImpresionRdlc.Precalentar("ReporteCajaBens", "ReporteCajaBensCamisas", "ReporteCajaTYNDALE", "ReporteCaja");
                f.ConsultaPO(cmbPO); cmbPO.SelectedIndex = -1; f.ConsultaCliente(cmbCliente); if (cmbCliente.Items.Count > 0) cmbCliente.SelectedIndex = 0; f.ConsultaFactura(cmbFactura);
                if (cmbFactura.Items.Count > 0) cmbFactura.SelectedIndex = 0; f.ConsultaTerminado(cmbTerminado); if (cmbTerminado.Items.Count > 0) cmbTerminado.SelectedIndex = 0; f.ConsultaPOModificar(cmbPOMod); cmbPOMod.SelectedIndex = -1;
                f.ConsultaPO(cmbPOB); cmbPOB.SelectedIndex = -1; f.ConsultaTallasZumies(cmbzTallas); if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                f.ConsultaPO(cmbPoEntradaAlmacen);
                if (cmbPoEntradaAlmacen.Items.Count > 0) cmbPoEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaCliente(cmbClienteEntradaAlmacen);
                if (cmbClienteEntradaAlmacen.Items.Count > 0) cmbClienteEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaTerminado(cmbTerminadoEntradaAlmacen);
                if (cmbTerminadoEntradaAlmacen.Items.Count > 0) cmbTerminadoEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaFactura(cmbFacturacionEntradaAlmacen);
                if (cmbFacturacionEntradaAlmacen.Items.Count > 0) cmbFacturacionEntradaAlmacen.SelectedIndex = 0;
                f.ConsultaPO(cmbPOSalidaAlmacen);
                if (cmbPOSalidaAlmacen.Items.Count > 0) cmbPOSalidaAlmacen.SelectedIndex = 0;
                f.ConsultaTallasXMarca(cmbTallaPrepack, "LEVIS");
                if (cmbTallaPrepack.Items.Count > 0) cmbTallaPrepack.SelectedIndex = 0;
                usu = usuario;
                try
                {
                    lblVersion.Text = Funciones.VersionApp();
                    lblVersion2.Text = Funciones.VersionApp();
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
                //  SE HABILITAN BOTONES PARA BAJA DE PO O CAJAS
                cmbPOB.Enabled = true;
                btnBajaPO.Enabled = true;
                txtCaja.Enabled = true;
                btnBajaCaja.Enabled = true;
                iniciando = false;


                //// Instancia del UserControl
                //ImportarEstilosPDF control = new ImportarEstilosPDF();

                //// Ajustamos para que ocupe todo el espacio
                //control.Dock = DockStyle.Fill;

                //// Lo agregamos al TabPage
                //tpAltaEstilo.Controls.Add(control);


                // Supongamos que tienes un TabControl con un TabPage llamado tabPage1
                // Y quieres mostrar dentro de él un formulario llamado FormularioHijo

                ImportarEstilosPDF frm = new ImportarEstilosPDF();

                // Lo configuramos para que se comporte como control embebido
                frm.TopLevel = false;
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.Dock = DockStyle.Fill;

                // Lo agregamos al TabPage
                tpAltaEstilo.Controls.Add(frm);

                // Finalmente lo mostramos
                frm.Show();
                CargarImpresorasDisponibles();
                PrepararTutorial();
                Tema.Aplicar(this);
                if (ModoPrueba.Activo) PrepararVentanaPrueba();

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
                Text = Funciones.TituloConVersion(Text);
                ConfigurarGrids();
                PrepararFiltrosMarca();
                PrepararPestanaBajas();
                PrepararPestanaCorregir();
                PrepararExcelBENS();
                f.ConsultaPO(cmbPO);
                cmbPO.SelectedIndex = 1;
                iniciando = false;
                Tema.Aplicar(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void txtUPCScann_KeyPress_Ejemp(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (int)Keys.Enter)
            {
                return;
            }

            PackagingContext contexto = new PackagingContext
            {
                ProfileCode = cmbPOItem.Text,
                BrandName = cmbMarca.Text,
                UpcEscaneado = txtUPCScann.Text,
                PoNumero = cmbPO.Text,
                Estilo = txtProductCode.Text,
                Talla = txtSize.Text,
                SizeIzquierdo = txtSize.Text,
                SizeDerecho = string.Empty,
                CantidadRequerida = Convert.ToDecimal(txtUnitsReq.Text),
                CantidadActual = Convert.ToDecimal(txtUnitsScan.Text),
                IdUsuario = usu[0].id,
                NombreUsuario = usu[0].nombre,
                ItemDescription = txtProductCode.Text,
                FormularioPadre = this,
                Funciones = f,
                ImprimirEtiqueta = chkImprimirEtiqueta.Checked
            };

            IPackagingStrategy estrategia = PackagingStrategyFactory.ObtenerEstrategia(contexto.ProfileCode);

            bool escaneoValido = estrategia.ValidarEscaneo(contexto);

            if (!escaneoValido)
            {
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                return;
            }

            txtUnitsScan.Text = contexto.CantidadActual.ToString();
            txtUPCScann.Text = string.Empty;
            txtUPCScann.Focus();

            if (contexto.EstaCompleto())
            {
                estrategia.ProcesarCajaIncompleta(contexto);
                LimpiarPantallaEscaneo();
            }
        }

        private void btnIncompleteCarton_Click_Ejemplo(object sender, EventArgs e)
        {
            if (id == 0 || Convert.ToInt32(txtUnitsScan.Text) <= 0 || dgvEscan.RowCount <= 0)
            {
                MessageBox.Show("No hay datos suficientes para procesar el carton incompleto.");
                return;
            }

            PackagingContext contexto = new PackagingContext
            {
                ProfileCode = cmbPOItem.Text,
                BrandName = cmbMarca.Text,
                UpcEscaneado = upc,
                PoNumero = cmbPO.Text,
                Estilo = txtProductCode.Text,
                Talla = txtSize.Text,
                SizeIzquierdo = txtSize.Text,
                SizeDerecho = string.Empty,
                CantidadActual = Convert.ToDecimal(txtUnitsScan.Text),
                CantidadRequerida = Convert.ToDecimal(txtUnitsReq.Text),
                IdUsuario = usu[0].id,
                NombreUsuario = usu[0].nombre,
                ItemDescription = txtProductCode.Text,
                FormularioPadre = this,
                Funciones = f,
                ImprimirEtiqueta = chkImprimirEtiqueta.Checked
            };

            IPackagingStrategy estrategia = PackagingStrategyFactory.ObtenerEstrategia(contexto.ProfileCode);
            estrategia.ProcesarCajaIncompleta(contexto);
            LimpiarPantallaEscaneo();
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
                    if (cmbPOItem.Text != "99" && cmbPOItem.Text != "77" && cmbPOItem.Text != "88" && cmbPOItem.Text != "55" && cmbPOItem.Text != "66" && cmbPOItem.Text != "33" && cmbPOItem.Text != "44" && cmbPOItem.Text != "22" && cmbPOItem.Text != "11")
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
                                }
                                ;

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
                        IncompleteCarton_ZUMIES();
                    }
                    else if (cmbPOItem.Text == "88")
                    {
                        IncompleteCarton_TARGET();
                    }
                    else if (cmbPOItem.Text == "55" && TallaMixta == 1)/*BEN DAVIS - CAJA MIXTA*/
                    {
                        IncompleteCarton_BENS_Mixta();
                    }
                    else if (cmbPOItem.Text == "55")/*BEN DAVIS*/
                    {
                        IncompleteCarton_BENS();
                    }

                    else if (cmbPOItem.Text == "66")
                    {
                        IncompleteCarton_ROCKY();
                    }


                    else if (cmbPOItem.Text == "33")
                    {
                        IncompleteCarton_SAFTECH();
                    }

                    else if (cmbPOItem.Text == "44")
                    {
                        IncompleteCarton_OLDNAVY();
                    }
                    else if (cmbPOItem.Text == "22")
                    {
                        IncompleteCarton_IFR(po_numero);
                    }
                    /*TYNDALE = 11 */
                    else if ((cmbPOItem.Text == "11") && TallaMixta == 1)
                    {
                        IncompleteCarton_TYNDALE_Mixta();
                    }
                    else if (cmbPOItem.Text == "11" && TallaMixta == 0)
                    {
                        IncompleteCarton_TYNDALE();
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
                            SeleccionTalla_TARGET(ref separadas);
                        }
                        else if (cmbPOItem.Text == "66")/*ROCKY*/
                        {
                            SeleccionTalla_ROCKY();
                        }
                        else if (cmbPOItem.Text == "44")/*OLDNAVY*/
                        {
                            SeleccionTalla_OLDNAVY(ref separadas);
                        }
                        else if (cmbPOItem.Text == "77")/*ZUMIES*/
                        {
                            SeleccionTalla_ZUMIES();
                        }
                        else if (cmbPOItem.Text == "55")/*bens*/
                        {
                            SeleccionTalla_BENS(ref separadas);
                        }
                        else if (marca == "IFR")
                        {
                            SeleccionTalla_IFR();
                        }
                        else if (cmbPOItem.Text == "11")/*TYNDALE*/
                        {
                            SeleccionTalla_TYNDALE();
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
            // Con la tabla vacia (p.ej. despues de cerrar una caja mixta) no hay renglon que seleccionar
            if (dgvEscan.Rows.Count > 0)
            {
                dgvEscan.Rows[0].Selected = true;
                dgvEscan.FirstDisplayedScrollingRowIndex = 0;
            }
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
                    /* BEN DAVIS (55) - CAJA MIXTA: se atiende aparte para no pasar por la logica de talla unica */
                    if (cmbPOItem.Text == "55" && TallaMixta == 1)
                    {
                        EscanearPrendaMixtaBENS();
                        return;
                    }
                    string marca = f.ConsultaMarca(cmbPO.Text, cmbPOItem.Text);
                    tpEscan.BackColor = Tema.Fondo;
                    List<ConsultaUPC_VarcharResult> ListUpc = null;
                    string po_numero = cmbPO.Text;
                    DataGridViewRow newDatarow;
                    // En caja mixta TYNDALE no se cambia la talla del combo: eso dispara SeleccionTalla_TYNDALE y borra lo acumulado
                    if (txtUnitsScan.Text == "0" && !(cmbPOItem.Text == "11" && TallaMixta == 1))
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
                        try
                        {
                            newDatarow = dgvEscan.Rows[(Convert.ToInt32(1))];
                        }
                        catch (Exception ex)
                        {
                            newDatarow = dgvEscan.Rows[0];
                            // MessageBox.Show("Favor de Escanear la prenda correcta!");
                        }
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
                    if (txtUnitsScan.Text == "0" && (cmbPOItem.Text != "99" && cmbPOItem.Text != "44" && cmbPOItem.Text != "55" && cmbPOItem.Text != "66" && cmbPOItem.Text != "77" && cmbPOItem.Text != "22" && cmbPOItem.Text != "11"))
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
                        Escaneo_PREPACK(ref marca, ref ListUpc, po_numero);
                    }
                    #endregion
                    else if ((upc == txtUPCScann.Text || PrendasExtra == 1) && cmbPOItem.Text != "99" && cmbPOItem.Text != "44" && cmbPOItem.Text != "22" && cmbPOItem.Text != "11")
                    {
                        #region ZUMIES(77) Y  TARGET(88)
                        if (cmbPOItem.Text == "77" || cmbPOItem.Text == "88")/* ZUMIES  Y  TARGET */
                        {
                            Escaneo_ZUMIES_TARGET(ref ListUpc, po_numero);
                        }
                        #endregion

                        #region ROCKY 66
                        if (cmbPOItem.Text == "66")/*ROCKY*/
                        {
                            Escaneo_ROCKY(ref ListUpc, po_numero);
                        }
                        #endregion
                        //715044695210
                        #region BENS 55
                        if (cmbPOItem.Text == "55")/*BENS*/
                        {
                            Escaneo_BENS(ref ListUpc, po_numero);
                        }
                        #endregion

                        #region SAFTECH  33
                        if (cmbPOItem.Text == "33")/*SAFTECH*/
                        {
                            Escaneo_SAFTECH(ref ListUpc, po_numero);
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
                                }
                                ;

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
                        Escaneo_OLDNAVY();
                    }
                    else if (marca == "IFR")
                    {
                        Escaneo_IFR(ref ListUpc, po_numero);
                    }
                    else if (cmbPOItem.Text == "11" || marca == "TYNDALE") /*TYNDALE*/
                    {
                        Escaneo_TYNDALE();
                    }
                    else
                    {
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
                if (TabEsaneo.SelectedTab == tpAltaPrePack)
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
                if (TabEsaneo.SelectedTab == tpAltaZumies)
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
                        tablaTarget.Columns.Add("CantidadPrendasTotal", typeof(string));
                        Tabpage = Tabpage + 1;
                        cmbMarca.SelectedIndex = 0;


                        f.ConsultaTallasXMarca(cmbzTallas, "LEVIS");
                        iniciando = false;
                    }
                    else
                    {

                    }
                }
                if (TabEsaneo.SelectedTab == ModificarPO)
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
                        tablaTarget.Columns.Add("CantidadPrendasTotal", typeof(string));


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
                if (TabEsaneo.SelectedTab == tpEscan)
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
                if (TabEsaneo.SelectedTab == tpBajaCaja)
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
                // TYNDALE e IFR: se reimprime la ultima caja con la misma logica de la pestana Reimpresion
                // (sabe imprimir cajas normales y mixtas con la etiqueta de la marca)
                if ((cmbPOItem.Text == "11" || cmbPOItem.Text == "22") && id_inventarioAnt > 0)
                {
                    string idsEscritos = txtIDReImpresion.Text;
                    txtIDReImpresion.Text = id_inventarioAnt.ToString();
                    ReimprimirUnID();
                    txtIDReImpresion.Text = idsEscritos;
                    txtUPCScann.Focus();
                    return;
                }
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
                    ReimprimirUltima_ZUMIES();
                }
                else if (cmbPOItem.Text == "88")
                {
                    ReimprimirUltima_TARGET();
                }
                else if (cmbPOItem.Text == "44")
                {
                    ReimprimirUltima_OLDNAVY();
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

        private void btnCancelCarton_Click(object sender, EventArgs e)
        {
            if (cmbPOItem.Text == "55" && TallaMixta == 1)
            {
                ReiniciarCajaMixtaBENS();
                btnImprimeOldNavy.Visible = false;
                return;
            }
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


        public class UPCResult
        {
            public string UPCCompleto { get; set; }
            public Image CodigoBarras { get; set; }
        }

        private void btnGuardarA_Click(object sender, EventArgs e)
        {

        }

        private void dgvAlta_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

        private void txtUnitsScan_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnEditar_Click_1(object sender, EventArgs e)
        {

        }
        public bool vacio;
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

        private void btnEliminar_Click_1(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

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
                else if (cmbPOItem.Text == "55") /* BEN DAVIS */
                {
                    TallaMixta = 1;
                    if (tablaMixtaBENS == null)
                    {
                        InicializarTablaMixtaBENS();

                        // Lo que ya se habia escaneado de la talla original entra como primera fila
                        int yaEscaneadas;
                        int.TryParse(txtUnitsScan.Text, out yaEscaneadas);
                        if (yaEscaneadas > 0 && !string.IsNullOrEmpty(upc))
                        {
                            List<ConsultaUPC_VarcharResult> info = f.ConsultaUPC(cmbPO.Text, upc);
                            string talla = (info != null && info.Count > 0)
                                ? ArmarTallaBENS(info[0].size_izquierdo, info[0].size_derecho)
                                : cmbSizes.Text.Trim();
                            AcumularTallaBENS(talla, upc, yaEscaneadas);
                        }

                        dgvEscan.DataSource = null;
                        dgvEscan.DataSource = tablaMixtaBENS;
                        txtUnitsReq.Text = "X";
                        txtUnitsRemai.Text = "X";
                    }
                    txtUPCScann.Focus();
                }
                // Igual que el escaneo y el cierre de caja: TYNDALE se reconoce por POItem 11 aunque la marca venga distinta
                else if (cmbPOItem.Text == "11" || (marca ?? "").Trim().ToUpper() == "TYNDALE")
                {
                    TallaMixta = 1;
                    txtUPCScann.Focus();

                    // No reiniciar el acumulador si ya existe (si presionan "Prendas Extra" varias veces, se sigue acumulando).
                    // El grid (dgvEscan) NO se toca: se queda mostrando las filas que ya tenia.
                    if (tablaMixtaTYNDALE == null)
                    {
                        tablaMixtaTYNDALE = new DataTable();
                        tablaMixtaTYNDALE.Columns.Add("Talla", typeof(string));
                        tablaMixtaTYNDALE.Columns.Add("Cantidad", typeof(int));
                        tablaMixtaTYNDALE.Columns.Add("UPC", typeof(string));

                        // Si ya habian escaneado piezas de la talla original antes de presionar "Prendas Extra",
                        // se agregan como primera fila del acumulador interno para no perderlas.
                        int cantidadYaEscaneada = 0;
                        try { cantidadYaEscaneada = Convert.ToInt32(txtUnitsScan.Text); } catch { cantidadYaEscaneada = 0; }

                        if (cantidadYaEscaneada > 0 && !string.IsNullOrEmpty(txtSize.Text))
                        {
                            tablaMixtaTYNDALE.Rows.Add(txtSize.Text.Trim(), cantidadYaEscaneada, upc.Trim());
                        }
                    }
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

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label92_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

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

        private void tpEscan_Click(object sender, EventArgs e)
        {

        }
    }

}