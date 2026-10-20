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
    // Marca TYNDALE (POItem 11): etiquetas ZPL, Toshiba y reporte para impresoras de Windows
    public partial class Escaneo
    {
        public void ImprimirEtiquetaTYNDALE(string nombreImpresora, EtiquetaCajaModificada clase, ReportPrintTool tool)
        {
            string nombreLower = nombreImpresora.ToLower();

            // En vista previa no se manda ZPL a la impresora: se muestra el reporte en pantalla
            if (EsImpresoraVirtual(nombreImpresora) || ReportPrintTool.ModoVistaPreviaReimpresion)
            {
                ImprimirEtiquetaTYNDALE_Reporte(nombreImpresora, clase);
            }
            else if (nombreLower.Contains("zebra") || nombreLower.Contains("zt411") || nombreLower.Contains("zpl"))
            {
                ImprimirEtiquetaTYNDALE_ZPL(nombreImpresora, clase);
            }
            else if (nombreLower.Contains("toshiba") || nombreLower.Contains("b-ex4t2") || nombreLower.Contains("tec"))
            {
                // Genera el bloque ZPL para etiqueta 4x6
                string zpl = GenerarZplEtiqueta4x6_Centrada(clase, modulo: 6, anchoMax: 200);

                // Mandar directo a la impresora seleccionada (no hardcodear)
                bool ok = RawPrinterHelper.SendStringToPrinter(nombreImpresora, zpl);

                if (!ok)
                {
                    MessageBox.Show("No se pudo imprimir en ZPL. Revisa que el driver de la Toshiba esté en modo ZPL.");
                }
            }
            else
            {
                MessageBox.Show("No se reconoce el lenguaje de la impresora '" + nombreImpresora + "'.");
            }
        }

        public string GenerarZplEtiqueta4x6_Centrada(EtiquetaCajaModificada clase, int modulo = 6, int anchoMax = 200)
        {
            // Dimensiones de la etiqueta en puntos (4x6 pulgadas a 203 dpi ≈ 812 x 1218)
            int anchoEtiqueta = 812;
            int altoEtiqueta = 1218;

            // Centro horizontal
            int centroX = anchoEtiqueta / 2;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("^XA");
            sb.AppendLine("^PW812");
            sb.AppendLine("^LL1218");

            // QR con PO puro
            sb.AppendLine($"^FO{centroX - 300},100^BXN,{modulo},{anchoMax}^FD{clase.po_varchar}^FS");

            // QR con Estilo puro
            string estiloCompleto = $"{clase.estilo}-{clase.color}-{clase.size_izquierdo}";
            sb.AppendLine($"^FO{centroX + 100},100^BXN,{modulo},{anchoMax}^FD{estiloCompleto}^FS");

            // QR con Cantidad pura
            sb.AppendLine($"^FO{centroX - 100},400^BXN,{modulo},{anchoMax}^FD{clase.Cantidad}^FS");

            // Texto centrado debajo
            sb.AppendLine($"^FO{centroX - 200},700^A0N,50,50^FD{estiloCompleto}^FS");
            sb.AppendLine($"^FO{centroX - 200},780^A0N,50,50^FDLOT: {clase.po_varchar}^FS");
            sb.AppendLine($"^FO{centroX - 200},860^A0N,50,50^FDQTY: {clase.Cantidad}^FS");

            sb.AppendLine("^XZ");
            return sb.ToString();
        }
        private void ImprimirLotePruebaTYNDALE(string nombreImpresora, EtiquetaCajaModificada clase)
        {
            string estiloCompleto = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
            string cantidadQr = (clase.Cantidad > 0 ? clase.Cantidad.ToString() : "0") + "X";
            string poTexto = string.IsNullOrEmpty(clase.po_varchar) ? "SIN-PO" : clase.po_varchar;

            // Ahora sí varía la posición X real entre variantes
            var variantes = new (string etiqueta, int x)[]
            {
        ("E", 50),
        ("F", 400),
        ("G", 700),
        ("H", 950),
            };

            string zplTotal = "";

            foreach (var v in variantes)
            {
                zplTotal +=
                    "^XA" +
                    "^PW1200" + "^LL1800" + "^LH0,0" + "^PON" +
                    "~SD10" +
                    "^CF0,40" +
                    "^FO" + v.x + ",700^A0B,40,40^FDPRUEBA " + v.etiqueta + "^FS" +
                    "^FO" + v.x + ",250^BXB,20,200,0,0,0,0^FD" + poTexto + "^FS" +
                    "^FO" + v.x + ",1050^BXB,20,200,0,0,0,0^FD" + cantidadQr + "^FS" +
                    "^XZ";
            }

            RawPrinterHelper.SendStringToPrinter(nombreImpresora, zplTotal);
        }
        /*
        private string ArmarZplTYNDALE(EtiquetaCajaModificada clase)
        {
            string estiloCompleto = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
            string cantidadTexto = clase.Cantidad > 0 ? clase.Cantidad.ToString() : "0";
            string cantidadQr = cantidadTexto;
            string poTexto = string.IsNullOrEmpty(clase.po_varchar) ? "SIN-PO" : clase.po_varchar;

            return "^XA" +
                   "^PW1200" + "^LL1800" + "^LH0,0" + "^PON" +
                   "~SD10" +
                   "^CF0,50" +
                   "^FO100,400^A0B,100,120^FD" + estiloCompleto + "^FS" +

                   "^FO500,1400^BXB,34,200^FD" + poTexto + "^FS" +
                   "^FO250,585^BXB,42,200^FD" + estiloCompleto + "^FS" +
                   "^FO500,100^BXB,34,200^FD" + cantidadQr + "^FS" +

                   "^FO890,1620^A0B,55,55^FDLOT:^FS" +
                   "^FO955,1490^A0B,95,95^FD" + poTexto + "^FS" +

                   "^FO890,320^A0B,55,55^FDQTY:^FS" +
                   "^FO955,330^A0B,95,95^FD" + cantidadTexto + "^FS" +
                   "^XZ";
        }

        */
        private string ArmarZplTYNDALE(EtiquetaCajaModificada clase)
        {
            string estiloCompleto = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
            string cantidadTexto = clase.Cantidad > 0 ? clase.Cantidad.ToString() : "0";
            string poTexto = string.IsNullOrEmpty(clase.po_varchar) ? "SIN-PO" : clase.po_varchar;
            // En lugar del numero de carton (se repetia entre cajas de la misma talla y se reiniciaba por talla)
            // se imprime el id unico de la tabla Inventario. Carton/CartonLeft quedan solo como respaldo.
            string numeroCarton = clase.id_inventario > 0 ? clase.id_inventario.ToString()
                                : clase.Carton > 0 ? clase.Carton.ToString()
                                : (string.IsNullOrEmpty(clase.CartonLeft) ? "" : clase.CartonLeft);

            return "^XA" +
                   "^PW1200" + "^LL1800" + "^LH0,0" + "^PON" +
                   "~SD10" +
                   "^CF0,50" +
                   "^FO130,400^A0B,100,120^FD" + estiloCompleto + "^FS" +

                   // codigos laterales a modulo 28 desde x=470: un PO de 14x14 termina en x=862 y no tapa "LOT:" (x=890)
                   "^FO470,1400^BXB,28,200^FD" + poTexto + "^FS" +
                   "^FO250,585^BXB,42,200^FD" + estiloCompleto + "^FS" +
                   "^FO470,100^BXB,28,200^FD" + cantidadTexto + "^FS" +

                   "^FO890,1620^A0B,55,55^FDLOT:^FS" +
                   "^FO955,1490^A0B,95,95^FD" + poTexto + "^FS" +

                   "^FO890,320^A0B,55,55^FDQTY:^FS" +
                   "^FO955,330^A0B,95,95^FD" + cantidadTexto + "^FS" +

                   // esquina inferior derecha: "ID:" y el id de inventario grande pegado a la orilla.
                   // El id (fuente 80, ~48 puntos por digito) ocupa de y=40 hasta ~y=380 con 7 digitos;
                   // "ID:" arranca en y=420 para que no se empalmen
                   "^FO1110,420^A0B,45,45^FDID:^FS" +
                   "^FO1080,40^A0B,80,80^FD" + numeroCarton + "^FS" +
                   "^XZ";
        }
        private string ArmarZplTYNDALE_Toshiba(EtiquetaCajaModificada clase)
        {
            string estiloCompleto = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
            string cantidadTexto = clase.Cantidad > 0 ? clase.Cantidad.ToString() : "0";
            string cantidadQr = cantidadTexto;
            string poTexto = string.IsNullOrEmpty(clase.po_varchar) ? "SIN-PO" : clase.po_varchar;

            return "^H10\r\n^V10\r\n^XSET,DATA_MATRIX,ON\r\n\r\n^PAGE\r\n^B2D,100,100,4,4,0,0,^FDPO:16016^FS\r\n^B2D,300,100,4,4,0,0,^FDESTILO:K17PT-KHA-MDR^FS\r\n^B2D,500,100,4,4,0,0,^FDQTY:25^FS\r\n\r\n^T100,400,0,PO: 16016\r\n^T300,400,0,ESTILO: K17PT-KHA-MDR\r\n^T500,400,0,QTY: 25\r\n\r\n^PAGE\r\n^XZ\r\n";
        }

        public string GenerarTPCLPruebaECC200(
    int modulo,
    int separacion,
    string poTexto,
    string estiloCompleto,
    string cantidadTexto)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("^H10");
            sb.AppendLine("^V10");
            sb.AppendLine("^XSET,DATA_MATRIX,ON");
            sb.AppendLine();
            sb.AppendLine("^PAGE");

            // PO
            sb.AppendLine($"^B2D,100,100,{modulo},{modulo},0,0,^FDPO:{poTexto}^FS");
            sb.AppendLine($"^T100,{100 + modulo * 40},0,PO: {poTexto}");

            // Estilo
            sb.AppendLine($"^B2D,100,{100 + separacion},{modulo},{modulo},0,0,^FDESTILO:{estiloCompleto}^FS");
            sb.AppendLine($"^T100,{100 + separacion + modulo * 40},0,ESTILO: {estiloCompleto}");

            // Cantidad
            sb.AppendLine($"^B2D,100,{100 + separacion * 2},{modulo},{modulo},0,0,^FDQTY:{cantidadTexto}^FS");
            sb.AppendLine($"^T100,{100 + separacion * 2 + modulo * 40},0,QTY: {cantidadTexto}");

            sb.AppendLine();
            sb.AppendLine("^PAGE");
            sb.AppendLine("^XZ");

            return sb.ToString();
        }

        /*
        private string ArmarZplTYNDALE(EtiquetaCajaModificada clase)
        {
            string estiloCompleto = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
            string cantidadTexto = clase.Cantidad > 0 ? clase.Cantidad.ToString() : "0";
            string cantidadQr = cantidadTexto + "X"; // evita el bug de Labelary con valores terminados en "10"
            string poTexto = string.IsNullOrEmpty(clase.po_varchar) ? "SIN-PO" : clase.po_varchar;

            return "^XA" +
                   "^PW1200" + "^LL1800" + "^LH0,0" + "^PON" +
                   "^CF0,50" +
                   "^FO30,550^A0B,50,50^FD" + estiloCompleto + "^FS" +

                   "^FO180,300^BXB,8,200,20,20,0,0^FD" + cantidadQr + "^FS" +
                   "^FO140,950^BXB,14,200,26,26,0,0^FD" + estiloCompleto + "^FS" +
                   "^FO180,1500^BXB,8,200,20,20,0,0^FD" + poTexto + "^FS" +

                   "^FO420,330^A0B,30,30^FDQTY:^FS" +
                   "^FO460,330^A0B,35,35^FD" + cantidadTexto + "^FS" +

                   "^FO420,1540^A0B,30,30^FDLOT:^FS" +
                   "^FO460,1540^A0B,35,35^FD" + poTexto + "^FS" +
                   "^XZ";
        }
        */

        public void ImprimirEtiquetaTYNDALE_ZPL(string nombreImpresora, EtiquetaCajaModificada clase)
        {
            // Las impresoras virtuales (Microsoft Print to PDF, XPS) no entienden ZPL crudo; en vista previa tampoco se imprime
            if (EsImpresoraVirtual(nombreImpresora) || ReportPrintTool.ModoVistaPreviaReimpresion)
            {
                ImprimirEtiquetaTYNDALE_Reporte(nombreImpresora, clase);
                return;
            }
            RawPrinterHelper.SendStringToPrinter(nombreImpresora, ArmarZplTYNDALE(clase));
        }

        private bool EsImpresoraVirtual(string nombreImpresora)
        {
            string nombreLower = (nombreImpresora ?? "").ToLower();
            return nombreLower.Contains("pdf") || nombreLower.Contains("xps") || nombreLower.Contains("onenote");
        }

        // Imprime la etiqueta TYNDALE con el reporte RDLC (sirve para Microsoft Print to PDF y cualquier impresora de Windows)
        public void ImprimirEtiquetaTYNDALE_Reporte(string nombreImpresora, EtiquetaCajaModificada clase)
        {
            string estiloCompleto = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
            string poTexto = string.IsNullOrEmpty(clase.po_varchar) ? "SIN-PO" : clase.po_varchar;
            string cantidadTexto = clase.Cantidad > 0 ? clase.Cantidad.ToString() : "0";
            clase.itemDescription = estiloCompleto;

            BarcodeWriter writerPO = new BarcodeWriter
            {
                Format = BarcodeFormat.DATA_MATRIX,
                Options = new EncodingOptions { Height = 110, Width = 110, Margin = 0 }
            };
            clase.qr = RotarImagen90((Bitmap)writerPO.Write(poTexto));

            BarcodeWriter writerEstilo = new BarcodeWriter
            {
                Format = BarcodeFormat.DATA_MATRIX,
                Options = new EncodingOptions { Height = 160, Width = 160, Margin = 0 }
            };
            clase.qr2 = RotarImagen90((Bitmap)writerEstilo.Write(estiloCompleto));

            BarcodeWriter writerCantidad = new BarcodeWriter
            {
                Format = BarcodeFormat.DATA_MATRIX,
                Options = new EncodingOptions { Height = 110, Width = 110, Margin = 0 }
            };
            clase.qr3 = RotarImagen90((Bitmap)writerCantidad.Write(cantidadTexto));

            List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
            listClase.Add(clase);
            ReporteCajaTYNDALE report = new ReporteCajaTYNDALE { DataSource = listClase };
            report.PrintingSystem.ShowMarginsWarning = false;
            ReportPrintTool tool = new ReportPrintTool(report);
            tool.Print(nombreImpresora);
        }

        private void IncompleteCarton_TYNDALE_Mixta()
        {
            if (tablaMixtaTYNDALE == null || tablaMixtaTYNDALE.Rows.Count == 0)
            {
                MessageBox.Show("No hay tallas escaneadas para guardar/imprimir.");
                return;
            }
            string impresoraSeleccionada = cmbImpresora.Text;
            /*if (string.IsNullOrWhiteSpace(impresoraSeleccionada) ||
                !impresoraSeleccionada.ToLower().Contains("zdesigner"))
            {
                MessageBox.Show("La impresora seleccionada no parece ser la Zebra correcta.");
                return;
            }
            */
            List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
            decimal CantidadTotalTYNDALE = 0;

            foreach (DataRow row in tablaMixtaTYNDALE.Rows)
            {
                string upcFila = row["UPC"].ToString();

                prendas = f.ConsultaPrendas_UPC(cmbPO.Text, upcFila);
                if (Convert.ToInt32(prendas.diferencia) <= 0)
                {
                    MessageBox.Show("Ya se escanearon todas las prendas de la talla " + row["Talla"].ToString() + " para ese po");
                    continue;
                }

                List<ConsultaUPC_VarcharResult> x2 = f.ConsultaUPC(cmbPO.Text, upcFila);

                EtiquetaCajaModificada x = new EtiquetaCajaModificada();
                x.size_izquierdo = row["Talla"].ToString();
                x.upc = upcFila;
                x.id = x2[0].id;
                x.po_varchar = x2[0].po;
                x.po = x2[0].po;
                x.Cantidad = Convert.ToInt32(row["Cantidad"]);
                x.color = x2[0].color;
                x.estilo = x2[0].estilo;
                x.ProductCode = x2[0].estilo;
                x.poItem = "11";
                x.mixto = 1;
                x.TipoCarton = "1";
                x.itemDescription = x.estilo + "-" + x.color + "-" + x.size_izquierdo;
                x.usuario = usu[0].nombre;
                x.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                x.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                x.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                x.cliente = cmbCliente.Text;
                x.factura = cmbFactura.Text;
                x.terminado = cmbTerminado.Text;
                x.Fecha = DateTime.Now;

                listClase.Add(x);
                CantidadTotalTYNDALE += (decimal)x.Cantidad;
            }

            if (listClase.Count == 0)
            {
                MessageBox.Show("No quedaron tallas válidas para guardar.");
                return;
            }

            // Guarda el header (registro "MIXTO"), mismo patrón que IFR
            decimal cantidadOriginal = (decimal)listClase[0].Cantidad;
            string tallaOriginal = listClase[0].size_izquierdo;
            string upcOriginal = listClase[0].upc;

            listClase[0].size_izquierdo = "MIXTO";
            listClase[0].Cantidad = 0;
            listClase[0].upc = "";
            listClase[0].QUANTITY = Convert.ToInt32(CantidadTotalTYNDALE);

            int id_inventarioMixto = (int)f.GuardaInventario(listClase[0], usu[0].id);
            id_inventarioAnt = id_inventarioMixto;

            // Se regresan los datos reales de la primera talla para su detalle y su etiqueta
            listClase[0].size_izquierdo = tallaOriginal;
            listClase[0].Cantidad = cantidadOriginal;
            listClase[0].upc = upcOriginal;
            listClase[0].itemDescription = listClase[0].estilo + "-" + listClase[0].color + "-" + tallaOriginal;

            List<ConsultaInventarioIDResult> cartonInfo = f.ConsultaInventarioID(id_inventarioMixto);
            string numeroCartonMixto = (cartonInfo != null && cartonInfo.Count > 0) ? cartonInfo[0].Carton.ToString() : "";

            foreach (EtiquetaCajaModificada item in listClase)
            {
                item.id_inventario = id_inventarioMixto;
                item.TipoCarton = "MIXTO";
                item.CartonLeft = numeroCartonMixto; // ← mismo número en las N etiquetas

                f.GuardaInventarioDetalle(item, usu[0].id);
                ImprimirEtiquetaTYNDALE_ZPL(impresoraSeleccionada, item);
            }
            //MessageBox.Show("Caja mixta TYNDALE guardada e impresa: " + listClase.Count + " talla(s).");

            TallaMixta = 0;
            PrendasExtra = 0;
            tablaMixtaTYNDALE = null;
            listaEscaneoTYNDALE = null;
            dgvEscan.DataSource = null;
            LimpiarPantallaEscaneo();
            return;
        
        }

        private void IncompleteCarton_TYNDALE()
        {
            #region CAJA INCOMPLETA TYNDALE - una sola talla con menos piezas que la caja completa
            int piezasEscaneadas = Convert.ToInt32(txtUnitsScan.Text);
            prendas = f.ConsultaPrendas_UPC(cmbPO.Text, upc);

            if (Convert.ToInt32(prendas.diferencia) <= 0)
            {
                MessageBox.Show("Ya se escanearon todas las prendas de esa talla y ese po");
            }
            else if ((Convert.ToInt32(prendas.prendasescaneadas) + piezasEscaneadas) > Convert.ToInt32(prendas.totalprendas))
            {
                int PrendasDemas = (Convert.ToInt32(prendas.prendasescaneadas) + piezasEscaneadas) - Convert.ToInt32(prendas.totalprendas);
                MessageBox.Show("Deben de ser " + Convert.ToInt32(prendas.totalprendas) + " y se excedio " + PrendasDemas);
            }
            else
            {
                List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);
                EtiquetaCajaModificada clase = RellenaObjetoClase(consulta);
                clase.mixto = 0;
                clase.poItem = "11";
                clase.po_varchar = consulta[0].po_numero;
                clase.po = string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))).ToString();
                clase.poInCompleto = Convert.ToDecimal(string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))));
                clase.ProductCode = consulta[0].estilo;
                clase.Size = consulta[0].size_izquierdo;
                clase.size_derecho = "";
                clase.TipoCarton = "0";
                clase.upc = consulta[0].upc;
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
                clase.Vendor = consulta[0].itemDescription;
                clase.estilo = consulta[0].estilo;
                clase.descripcion = consulta[0].itemDescription;
                // Cantidad real escaneada (menor a la caja completa)
                clase.QUANTITY = piezasEscaneadas;
                clase.Cantidad = piezasEscaneadas;
                clase.color = consulta[0].color;
                clase.size_izquierdo = consulta[0].size_izquierdo;
                clase.itemDescription = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
                clase.id_inventario = f.GuardaInventario(clase, usu[0].id);
                List<ConsultaInventarioIDResult> cartonInfo = f.ConsultaInventarioID(Convert.ToInt32(clase.id_inventario));
                clase.CartonLeft = (cartonInfo != null && cartonInfo.Count > 0) ? cartonInfo[0].Carton.ToString() : "";
                cantidadAnterior = piezasEscaneadas;
                id_inventarioAnt = clase.id_inventario; // ID de la caja (antes guardaba el ID del producto y Print Last Label reimprimia otra caja)

                List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                listClase.Add(clase);
                ReporteCajaTYNDALE report = new ReporteCajaTYNDALE { DataSource = listClase };
                report.PrintingSystem.ShowMarginsWarning = false;
                ReportPrintTool tool = new ReportPrintTool(report);

                if (chkImprimirEtiqueta.Checked == true) { imprimir = true; }
                if (imprimir)
                {
                    ImprimirEtiquetaTYNDALE(cmbImpresora.Text, clase, tool);
                }
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
            #endregion
        
        }

        private void SeleccionTalla_TYNDALE()
        {
            prendas = f.ConsultaPrendas_Talla(cmbPO.Text, cmbSizes.Text.Trim());

            if (Convert.ToInt32(prendas.diferencia) == 0)
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
                txtSize.Text = cmbSizes.Text.Trim();
            }
            else
            {
                List<ConsultaProductos_VarcharResult> x = f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, cmbSizes.Text.ToString(), "");
                if (x.Count > 0)
                {
                    contador = 0;
                    // Lista base del grid para TYNDALE: arranque de talla/caja nueva
                    listaEscaneoTYNDALE = x;
                    tablaMixtaTYNDALE = null;
                    dgvEscan.DataSource = listaEscaneoTYNDALE;
                    lblImprimirEtiqueta.Visible = true;
                    chkImprimirEtiqueta.Visible = true;
                    txtCartonsPacked.Text = x[0].CartonNumber.ToString();
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
                    cantidad = x[0].cantidad;
                    txtUnitsReq.Text = cantidad.ToString();
                    txtUnitsScan.Text = "0";
                    txtUnitsRemai.Text = cantidad.ToString();
                    txtUPCScann.Focus();
                }
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
                txtUPCScann.Focus();
            }
        
        }

        private void Escaneo_TYNDALE()
        {
            if (TallaMixta == 1)
            {
                #region CAJA MIXTA TYNDALE - acumula por talla
                List<ConsultaUPC_VarcharResult> infoUPC = f.ConsultaUPC(cmbPO.Text, txtUPCScann.Text);

                if (infoUPC == null || infoUPC.Count == 0)
                {
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;
                    do
                    {
                        string mensaje = "PRESIONA EN NO PARA CONTINUAR";
                        string titulo = "UPC no reconocido para este PO!";
                        result = MessageBox.Show(this, mensaje, titulo, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                        sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                        sonido.controls.play();
                        txtUPCScann.Text = string.Empty;
                        tpEscan.BackColor = Color.Red;
                    } while (result == DialogResult.Yes);
                }
                else
                {
                    if (tablaMixtaTYNDALE == null)
                    {
                        tablaMixtaTYNDALE = new DataTable();
                        tablaMixtaTYNDALE.Columns.Add("Talla", typeof(string));
                        tablaMixtaTYNDALE.Columns.Add("Cantidad", typeof(int));
                        tablaMixtaTYNDALE.Columns.Add("UPC", typeof(string));
                    }

                    string tallaEscaneada = infoUPC[0].size_izquierdo.Trim();
                    string upcEscaneado = txtUPCScann.Text.Trim();

                    // 1) Acumulador interno (Talla/Cantidad/UPC) para guardar e imprimir.
                    // Se agrupa por UPC (unico por talla) para que salga 1 sola fila = 1 etiqueta por talla.
                    DataRow[] existente = tablaMixtaTYNDALE.Select("UPC = '" + upcEscaneado.Replace("'", "''") + "'");
                    if (existente.Length > 0)
                        existente[0]["Cantidad"] = Convert.ToInt32(existente[0]["Cantidad"]) + 1;
                    else
                        tablaMixtaTYNDALE.Rows.Add(tallaEscaneada, 1, upcEscaneado);

                    // 2) Grid visual: se agrega una fila nueva abajo de las que ya habia
                    if (listaEscaneoTYNDALE == null)
                        listaEscaneoTYNDALE = new List<ConsultaProductos_VarcharResult>();

                    listaEscaneoTYNDALE.Add(new ConsultaProductos_VarcharResult
                    {
                        Size = tallaEscaneada,
                        UPC = upcEscaneado,
                        ProductCode = infoUPC[0].estilo,
                        ProductCode1 = infoUPC[0].estilo,
                        CartonNumber = txtCartonsPacked.Text,
                        CartonType = "MIXTO",
                        cantidad = "1",
                        TotalUnits = 1,
                        vendor = ""
                    });

                    dgvEscan.DataSource = null;
                    dgvEscan.DataSource = listaEscaneoTYNDALE;
                    dgvEscan.FirstDisplayedScrollingRowIndex = listaEscaneoTYNDALE.Count - 1;

                    contador = contador + 1;
                    txtUnitsScan.Text = contador.ToString();
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";
                    sonido.controls.play();
                }
                #endregion
            }
            else if (upc == txtUPCScann.Text)
            {
                #region ESCANEANDO X PRENDA AUN
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
                    prendas = f.ConsultaPrendas_UPC(cmbPO.Text, upc);

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
                    }
                    else
                    {
                        if ((Convert.ToInt32(prendas.prendasescaneadas) + Convert.ToInt32(txtUnitsScan.Text)) <= Convert.ToInt32(prendas.totalprendas))
                        {
                            List<ConsultaEtiquetaResult> consulta = f.ConsultaEtiqueta(id);
                            EtiquetaCajaModificada clase = RellenaObjetoClase(consulta);
                            clase.TipoCarton = "1";
                            clase.mixto = 0;
                            clase.poItem = "11";
                            clase.po_varchar = consulta[0].po_numero;
                            clase.po = string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))).ToString();
                            clase.poInCompleto = Convert.ToDecimal(string.Concat(cmbPO.Text.Where(c => Char.IsDigit(c))));
                            clase.poItem = "11";
                            clase.ProductCode = consulta[0].estilo;
                            clase.Size = consulta[0].size_izquierdo;
                            clase.size_derecho = "";
                            clase.TipoCarton = "0";
                            clase.upc = consulta[0].upc;
                            upc = consulta[0].upc;
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
                            clase.Vendor = consulta[0].itemDescription;
                            clase.estilo = consulta[0].estilo;
                            clase.descripcion = consulta[0].itemDescription;
                            clase.QUANTITY = Convert.ToInt32(consulta[0].Cantidad);
                            clase.Cantidad = Convert.ToDecimal(consulta[0].Cantidad);
                            clase.color = consulta[0].color;
                            clase.size_izquierdo = consulta[0].size_izquierdo;
                            clase.itemDescription = clase.estilo + "-" + clase.color + "-" + clase.size_izquierdo;
                            clase.id_inventario = f.GuardaInventario(clase, usu[0].id);
                            List<ConsultaInventarioIDResult> cartonInfo = f.ConsultaInventarioID(Convert.ToInt32(clase.id_inventario));
                            clase.CartonLeft = (cartonInfo != null && cartonInfo.Count > 0) ? cartonInfo[0].Carton.ToString() : "";
                            cantidadAnterior = Convert.ToInt32(txtUnitsScan.Text);

                            List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                            listClase.Add(clase);
                            id_inventarioAnt = clase.id_inventario; // ID de la caja (antes guardaba el ID del producto y Print Last Label reimprimia otra caja)
                            ReporteCajaTYNDALE report = new ReporteCajaTYNDALE { DataSource = listClase };
                            report.PrintingSystem.ShowMarginsWarning = false;
                            ReportPrintTool tool = new ReportPrintTool(report);
                            string impresoraSeleccionada = cmbImpresora.Text;

                            if (chkImprimirEtiqueta.Checked == true) { imprimir = true; }
                            if (imprimir)
                            {
                                ImprimirEtiquetaTYNDALE(impresoraSeleccionada, clase, tool);
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
            }
            else
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                do
                {
                    string mensaje = "PRESIONA EN NO PARA CONTINUAR";
                    string titulo = "Favor de Escanear la prenda correcta!";
                    result = MessageBox.Show(this, mensaje, titulo, buttons, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                    sonido.controls.play();
                    txtUPCScann.Text = string.Empty;
                    tpEscan.BackColor = Color.Red;
                } while (result == DialogResult.Yes);
            }
        
        }
    }
}
