using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Deployment.Application;
using WMPLib;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;

namespace Inventario
{
    public partial class ReporteCubrebocas : Form
    {
        Funciones f = new Funciones();

        public ReporteCubrebocas()
        {
            InitializeComponent();
            txtCartonNumber.Text = ULTIMO();
        }

        static Image Creapdf417(String dd)
        {
            

            BarcodePDF417 pdf417 = new BarcodePDF417();
            pdf417.Options = BarcodePDF417.PDF417_USE_ASPECT_RATIO;
            pdf417.ErrorLevel = 8;
            pdf417.Options = BarcodePDF417.PDF417_FORCE_BINARY;
            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            byte[] isoBytes = iso.GetBytes(dd);
            pdf417.Text = isoBytes;
            // pdf417.SetText(contenido);
            Image codigoBarras =pdf417.CreateDrawingImage(Color.Black, Color.White);
            return codigoBarras;
        }

        public string ULTIMO()
        {

            ConsultaCartonNumberResult consulta = f.ConsultaCarton();
            return consulta.Ultimo.ToString();
        }


        private void btnImprimir_Click(object sender, EventArgs e)
        {

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            List<EtiquetaNueva> listClase = new List<EtiquetaNueva>();
            EtiquetaNueva clase = new EtiquetaNueva();
            /*A */
            clase.ASupplyName = txtASupplyName.Text;
            clase.Asupp = txtAsupp.Text;

            /*B */
            clase.Bqty = txtBqty.Text;
            clase.BUOM = txtBUOM.Text;
            clase.BContainer = txtBContainer.Text;
            clase.BGrossWeight = txtBGrossWeight.Text;
            clase.BGrossWGTUOM = txtBGrossWGTUOM.Text;
            clase.BDateShipping = txtBDateShipping.Value.Date;
            clase.BLotBatch = txtBLotBatch.Text;
            clase.BSHIFT = txtBSHIFT.Text;
            clase.BWC = txtBWC.Text;
            /*C */
            clase.CPart = txtCPart.Text;
            /*D */
            clase.Dstrloc3 = txtDstrloc3.Text;
            clase.DASNNumber = txtDASNNumber.Text;
            /*E */
            clase.ESuppArea = txtESuppArea.Text;
            clase.ESuppPartNumber = txtESuppPartNumber.Text;
            clase.ESuppPartDescription = txtESuppPartDescription.Text;
            clase.ESerial = txtESerial.Text;
            clase.EMadein = txtEMadein.Text;
            clase.EUserID = txtEUserID.Text;
            clase.EOptionalLabel = txtEOptionalLabel.Text;
            clase.ECSN = txtECSN.Text;
            clase.EPlantCode = txtEPlantCode.Text;
            clase.EEngineeringAlertNumber = txtEEngineeringAlertNumber.Text;
            clase.EDocCode = txtEDocCode.Text;

          
            clase.codigoBarrasSERIALNO = CodigoBarras39(txtCartonNumber.Text,200,100);
            clase.codigoBarrasDELIVERY = CodigoBarras128(txtDASNNumber.Text, 200, 100);
            clase.codigoBarrasPART = CodigoBarras128(txtCPart.Text, 200, 100);
            clase.codigoBarrasQTY = CodigoBarras128(txtBqty.Text, 200, 100);
            clase.codigoBarrasSUPPLIER = CodigoBarras128(txtAsupp.Text, 200, 100);

            Image codigo2D = Creapdf417(txtCodigo2D.Text);

            clase.codigoBarras2D = codigo2D;

            listClase.Add(clase);
                ReporteAlmacen report = new ReporteAlmacen();
                report.DataSource = listClase;
                // Disable margins warning. 
                report.PrintingSystem.ShowMarginsWarning = false;
                ReportPrintTool tool = new ReportPrintTool(report);
                tool.ShowPreview();
                //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                //tool.PrintDialog(); //muestra a que impresora se va a mandar
                //tool.Print(); //imprime de golpe
            
            txtCartonNumber.Text = ULTIMO();

        

        }
        public Image CodigoBarras39(string texto,int ancho , int alto)
        {
            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
            Codigo.IncludeLabel = true;
            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                , ULTIMO()
                                                , Color.Black
                                                , Color.White, ancho, alto);
            return codigoBarras;

        }

        public Image CodigoBarras128(string texto, int ancho, int alto)
        {
            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
            Codigo.IncludeLabel = true;
            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE128
                                                , ULTIMO()
                                                , Color.Black
                                                , Color.White, ancho, alto);
            return codigoBarras;

        }
    }
}
