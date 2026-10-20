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
    // Pestana Reimpresion: reimprimir etiquetas por ID o por PO
    public partial class Escaneo
    {
        public EtiquetaCajaModificada RellenaObjetoClaseREIMPRETODO(List<ConsultaInventarioPOResult> consulta)
        {
            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                id = consulta[0].id,
                po = consulta[0].po.ToString(),
                po_varchar = consulta[0].po.ToString(),
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

        // Acepta uno o varios IDs separados por renglón, coma, punto y coma o espacio
        private void btnImprimirRe_Click(object sender, EventArgs e)
        {
            string[] ids = txtIDReImpresion.Text.Split(new[] { '\r', '\n', ',', ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (ids.Length <= 1)
            {
                txtIDReImpresion.Text = ids.Length == 1 ? ids[0] : string.Empty;
                ReimprimirUnID();
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            foreach (string idTexto in ids)
            {
                txtIDReImpresion.Text = idTexto;
                ReimprimirUnID();
            }
            Cursor.Current = Cursors.Default;
            txtIDReImpresion.Text = string.Empty;
            txtIDReImpresion.Focus();
            MessageBox.Show("Se mandaron a reimprimir " + ids.Length + " ID(s).");
        }

        private void ReimprimirUnID()
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
                    if (consulta[0].poItem != "77" && consulta[0].poItem != "88" && consulta[0].poItem != "44" && consulta[0].poItem != "99" && consulta[0].poItem != "55" && consulta[0].poItem != "66" && consulta[0].poItem != "33" && consulta[0].poItem != "22" && consulta[0].poItem != "11")
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
                            // Usar CODE128 (más compacto) para que el código quepa en una etiqueta 4x6
                            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE128, clase.id_inventario.ToString(), Color.Black, Color.White, 270, 180);
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
                    else if (consulta[0].poItem == "55")
                    {
                        #region BENS
                        /*UPC CORRECTO*/
                        try
                        {
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                            List<ConsultaProductosBensIDResult> x = f.ConsultaProductosBensID(idInv);
                            if (x.Count > 0 && EsMixtaBENS(x[0].size_izquierdo))
                            {
                                ReimprimirMixtaBENS(idInv, x[0].CartonNumber.HasValue ? Convert.ToInt32(x[0].CartonNumber.Value) : 0);
                            }
                            else if (x.Count > 0)
                            {
                                contador = 0;
                                txtCartonSize.Text = "";
                                txtSize.Text = "";
                                txtProductCode.Text = x[0].Estilo.ToString();
                                anterior.Add(id);
                                upc = cmbPOItem.Text;
                                // Para cajas MIXTAS el padre guarda po_varchar como NULL y el PO real queda en PO (po_numero).
                                // Preferimos po_varchar, pero si viene vacio tomamos PO. Si no, la concatenacion y los Trim
                                // tiraban ArgumentNullException al querer reimprimir el header de una mixta.
                                string poRef = (x[0].po_varchar ?? x[0].PO ?? "").Trim();
                                string poDigitos = string.Concat(poRef.Where(c => Char.IsDigit(c)));
                                EtiquetaBens clase = new EtiquetaBens();
                                clase.id = x[0].id;
                                clase.po = poDigitos;
                                clase.po_varchar = poRef;
                                clase.poInCompleto = string.IsNullOrEmpty(poDigitos) ? 0m : Convert.ToDecimal(poDigitos);
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
                                clase.cn_tag_num = x[0].CartonNumber.HasValue ? Convert.ToInt32(x[0].CartonNumber.Value) : 0;
                                //clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id);
                                clase.id_string = x[0].codigoBarras.ToString();// x[0].Estilo + clase.size_izquierdo + clase.size_derecho +  clase.QUANTITY + 
                                //clase.assembly = "*" + clase.id_string + "*";
                                //clase.anio = x[0].Estilo + ":" + clase.size_izquierdo + ":" + clase.size_derecho + ":" + clase.QUANTITY + ":" + clase.id;

                                clase.codigoBarras = CodigoBarrasBENS(clase.id_string.ToString());



                                DateTime fechaEntrada = DateTime.Now;

                                clase.anio = clase.id_string;
                                //CAMBIAR EN OTROS TAMBIEN 
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                                List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                                listclaseZ.Add(clase);
                                ImprimirReimpresionBENS(listclaseZ);

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



                        int? mixto = f.ConsultaMixto(clase.po, clase.estilo);

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
                        if (consulta[0].size_izquierdo == "MIXTO")
                        {
                            // Caja mixta: la etiqueta se arma con el detalle guardado de la caja (antes leia lo que hubiera en pantalla)
                            ReimprimirMixtaIFR(consulta[0], idInv);
                        }
                        else
                        {

                            clase.TipoCarton = "1";
                            clase.mixto = 0;
                            clase.poItem = "22";
                            clase.id_inventario = idInv;
                            //cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);
                            //string codigoBarras = clase.id_inventario.ToString();
                            //clase.assembly = codigoBarras;
                            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode { IncludeLabel = true, LabelFont = new Font("Arial", 7, FontStyle.Bold) };
                            Codigo.BarWidth = 3;

                            UPCResult resultado = GeneraCodigoUPC(clase.upcCaja.ToString());
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

                            tool.Print(); //imprime de golpe

                        }


                    }

                    /* IFR */
                    else if (consulta[0].poItem == "11")
                    {
                        List<ConsultaEtiquetaResult> consulta2 = f.ConsultaEtiqueta(consulta[0].id_CatProductos);
                        EtiquetaCajaModificada clase = RellenaObjetoClase(consulta2);
                        // Mixta es la caja guardada como "MIXTO" (antes se preguntaba si la PO tenia alguna mixta,
                        // y entonces las cajas normales de esa PO fallaban con "No se encontro detalle")
                        if (consulta[0].size_izquierdo == "MIXTO")
                        {
                            List<ConsultaInventarioDetalleResult> detalle = f.ConsultaInventarioDetalle(consulta[0].id);

                            if (detalle == null || detalle.Count == 0)
                            {
                                MessageBox.Show("No se encontró detalle para esta caja mixta.");
                                return;
                            }

                            string impresoraSeleccionada = cmbImpresora.Text;
                            // Mismos nombres que acepta ImprimirEtiquetaTYNDALE para mandar ZPL (en produccion se llama "Zebra ZT411 (300 dpi)...")
                            string impresoraLower = (impresoraSeleccionada ?? "").ToLower();
                            if (string.IsNullOrWhiteSpace(impresoraSeleccionada) ||
                                !(impresoraLower.Contains("zdesigner") || impresoraLower.Contains("zebra") || impresoraLower.Contains("zt411") || impresoraLower.Contains("zpl")))
                            {
                                MessageBox.Show("La impresora seleccionada no parece ser la Zebra correcta.");
                                return;
                            }
                            foreach (ConsultaInventarioDetalleResult item in detalle)
                            {
                                EtiquetaCajaModificada claseTalla = new EtiquetaCajaModificada
                                {
                                    po_varchar = item.po,
                                    estilo = item.estilo,
                                    color = item.color,
                                    size_izquierdo = item.size_izquierdo,
                                    Cantidad = item.Cantidad ?? 0,
                                    id_inventario = Convert.ToInt64(consulta[0].id), // mismo id de caja para las N tallas
                                    CartonLeft = consulta[0].Carton.ToString() // â† mismo número para las N tallas
                                };

                                ImprimirEtiquetaTYNDALE_ZPL(impresoraSeleccionada, claseTalla);
                            }

                            MessageBox.Show("Se reimprimieron " + detalle.Count + " etiqueta(s) de esta caja mixta.");
                        }
                        else
                        {
                            clase.TipoCarton = "1";
                            clase.mixto = 0;
                            clase.poItem = "11";
                            clase.po_varchar = consulta2[0].po_numero;
                            clase.po = consulta2[0].po_numero;
                            clase.poInCompleto = 0;
                            clase.poItem = "11";
                            clase.ProductCode = consulta2[0].estilo;
                            clase.Size = consulta2[0].size_izquierdo;
                            clase.size_derecho = "";
                            clase.TipoCarton = "0";
                            clase.upc = consulta2[0].upc;
                            upc = consulta2[0].upc;
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
                            clase.Vendor = consulta2[0].itemDescription;
                            clase.estilo = consulta2[0].estilo;
                            clase.descripcion = consulta2[0].itemDescription;
                            clase.QUANTITY = Convert.ToInt32(consulta[0].Cantidad);
                            clase.Cantidad = Convert.ToDecimal(consulta[0].Cantidad);
                            clase.itemDescription = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
                            clase.color = consulta2[0].color;
                            clase.size_izquierdo = consulta2[0].size_izquierdo;
                            clase.CartonLeft = consulta[0].Carton.ToString();
                            clase.id_inventario = Convert.ToInt64(consulta[0].id);
                            BarcodeWriter writerPO = new BarcodeWriter
                            {
                                Format = BarcodeFormat.DATA_MATRIX,
                                Options = new EncodingOptions { Height = 110, Width = 110, Margin = 0 }
                            };
                            clase.qr = RotarImagen90((Bitmap)writerPO.Write(clase.po_varchar));

                            BarcodeWriter writerEstilo = new BarcodeWriter
                            {
                                Format = BarcodeFormat.DATA_MATRIX,
                                Options = new EncodingOptions { Height = 160, Width = 160, Margin = 0 }
                            };
                            clase.qr2 = RotarImagen90((Bitmap)writerEstilo.Write(clase.estilo + clase.color + clase.size_izquierdo));

                            BarcodeWriter writerCantidad = new BarcodeWriter
                            {
                                Format = BarcodeFormat.DATA_MATRIX,
                                Options = new EncodingOptions { Height = 110, Width = 110, Margin = 0 }
                            };
                            clase.qr3 = RotarImagen90((Bitmap)writerCantidad.Write(clase.Cantidad.ToString()));
                            List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                            listClase.Add(clase);
                            id_inventarioAnt = Convert.ToInt32(clase.id);
                            ReporteCajaTYNDALE report = new ReporteCajaTYNDALE { DataSource = listClase };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            string impresoraSeleccionada = cmbImpresora.Text;
                            //ImprimirLotePruebaTYNDALE(impresoraSeleccionada, clase);
                            ImprimirEtiquetaTYNDALE(impresoraSeleccionada, clase, tool);
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

        /// <summary>
        /// Vista Previa (tab Reimpresion). Reutiliza toda la logica por marca que ya tiene btnImprimirRe_Click:
        /// prende el modo "solo vista previa" del ReportPrintTool, delega al handler de reimprimir y, en
        /// cuanto termina, apaga el modo. Asi cada marca abre el visor de su etiqueta sin duplicar codigo.
        /// Si falta el ID o no se encuentra, muestra el mismo mensaje que al reimprimir.
        /// </summary>
        private Panel panelVistaPrevia;

        // La vista previa se dibuja en un panel encima de pictureBoxPreview (se crea en codigo para no tocar el disenador).
        private Panel PanelVistaPrevia()
        {
            if (panelVistaPrevia == null)
            {
                panelVistaPrevia = new Panel
                {
                    Bounds = pictureBoxPreview.Bounds,
                    Anchor = pictureBoxPreview.Anchor,
                    BorderStyle = BorderStyle.FixedSingle
                };
                pictureBoxPreview.Parent.Controls.Add(panelVistaPrevia);
                pictureBoxPreview.Visible = false;
            }
            return panelVistaPrevia;
        }

        private void btnPreviewZPL_Click(object sender, EventArgs e)
        {
            try
            {
                global::Inventario.Reportes.ImpresionRdlc.DestinoVistaPrevia = PanelVistaPrevia();
                global::Inventario.Reportes.ReportPrintTool.ModoVistaPreviaReimpresion = true;
                btnImprimirRe_Click(sender, e);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally
            {
                global::Inventario.Reportes.ReportPrintTool.ModoVistaPreviaReimpresion = false;
                global::Inventario.Reportes.ImpresionRdlc.DestinoVistaPrevia = null;
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
                            clase.po = a.po.ToString();
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
                            clase.CARTON_NUMBER_INICIAL = a.NumeroInicial.HasValue ? a.NumeroInicial.Value.ToString() : string.Empty;
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
                            clase.po = a.po.ToString();
                            clase.poInCompleto = a.NumeroInicial ?? 0m;
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
                            /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS REIMPRESION DE TODO*/

                            foreach (ConsultaInventarioPOResult a in x)
                            {
                                if (EsMixtaBENS(a.size_izquierdo))
                                {
                                    ReimprimirMixtaBENS(Convert.ToInt32(a.id), Convert.ToInt32(a.CartonNumber));
                                    continue;
                                }
                                contador = 0;
                                txtCartonSize.Text = "";
                                txtSize.Text = "";
                                txtProductCode.Text = a.Estilo.ToString();
                                //anterior.Add(id);
                                upc = cmbPOItem.Text;
                                EtiquetaBens clase = new EtiquetaBens();
                                clase.id = a.id;
                                clase.po = string.Concat(a.po_varchar.Where(c => Char.IsDigit(c))).ToString();
                                //Convert.ToDecimal();
                                clase.po_varchar = a.po_varchar.Trim();
                                clase.poInCompleto = Convert.ToDecimal(string.Concat(a.po_varchar.Where(c => Char.IsDigit(c))));
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
                                clase.cn_tag_num = Convert.ToInt32(a.CartonNumber);

                                //clase.id_string = f.GuardaInventarioBeans(clase, usu[0].id);

                                clase.id_string = a.codigoBarras.ToString();// a.Estilo + clase.size_izquierdo + clase.size_derecho +  clase.QUANTITY + 


                                //clase.assembly = "*" + clase.id_string + "*";

                                //clase.anio = a.Estilo + ":" + clase.size_izquierdo + ":" + clase.size_derecho + ":" + clase.QUANTITY + ":" + clase.id;



                                clase.codigoBarras = CodigoBarrasBENS(clase.id_string.ToString());

                                DateTime fechaEntrada = DateTime.Now;

                                clase.anio = clase.id_string;
                                //CAMBIAR EN OTROS TAMBIEN 
                                int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);


                                List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                                listclaseZ.Add(clase);
                                ImprimirReimpresionBENS(listclaseZ);
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
                                claseZ.po = a.po.ToString();
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
                                po = a.po.ToString(),
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
                                Carton = a.NumeroInicial.HasValue ? Convert.ToInt64(a.NumeroInicial.Value) : 0,
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


                    else if (x[0].poItem == "22")
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
    }
}
