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
    // Marca BEN DAVIS (POItem 55): caja mixta
    public partial class Escaneo
    {
        private void InicializarTablaMixtaBENS()
        {
            tablaMixtaBENS = new DataTable();
            tablaMixtaBENS.Columns.Add("Talla", typeof(string));
            tablaMixtaBENS.Columns.Add("Cantidad", typeof(int));
            tablaMixtaBENS.Columns.Add("UPC", typeof(string));
        }

        private string ArmarTallaBENS(string izq, string der)
        {
            izq = (izq ?? "").Trim();
            der = (der ?? "").Trim();
            return (der == "" || der == "*") ? izq : izq + "x" + der;   // camisas: solo izquierda
        }

        private void AcumularTallaBENS(string talla, string upcTalla, int cantidadTalla)
        {
            DataRow[] existente = tablaMixtaBENS.Select("UPC = '" + upcTalla.Replace("'", "''") + "'");
            if (existente.Length > 0)
                existente[0]["Cantidad"] = Convert.ToInt32(existente[0]["Cantidad"]) + cantidadTalla;
            else
                tablaMixtaBENS.Rows.Add(talla, cantidadTalla, upcTalla);
        }

        private int TotalPrendasMixtaBENS()
        {
            int total = 0;
            if (tablaMixtaBENS != null)
                foreach (DataRow r in tablaMixtaBENS.Rows) total += Convert.ToInt32(r["Cantidad"]);
            return total;
        }

        // Caja normal: 2 etiquetas iguales, una por cada cara de la caja.
        private const int CopiasEtiquetaBENS = 2;
        // Caja mixta (requisito del cliente): maximo 4 tallas y una sola etiqueta por talla.
        private const int MaxTallasCajaMixtaBENS = 4;
        private const int CopiasEtiquetaMixtaBENS = 2;

        // Tamano del codigo de barras en pulgadas (el cuadro en ReporteCajaBens/Camisas mide 3.96 x 0.55, todo el ancho
        // de la etiqueta, y usa CenterImage). Con ese ancho caben barras de 2 puntos a 203 dpi y de 3 puntos a 300 dpi.
        private const float AnchoCodigoBENS = 4.0f;
        private const float AltoCodigoBENS = 0.75f;

        /// <summary>
        /// Code128 nitido para la impresora: cada barra mide un numero entero de puntos y lleva zona
        /// de silencio a los lados. Antes se generaba a 800 px y el reporte lo reescalaba (ZoomImage),
        /// lo que deformaba el ancho de las barras y algunas etiquetas no se podian escanear.
        /// </summary>
        // Resolucion de la impresora predeterminada; preguntarle al driver tarda ~0.25 s, se lee una vez por sesion.
        private static int dpiImpresoraBENS;

        private Image CodigoBarrasBENS(string texto)
        {
            if (dpiImpresoraBENS == 0)
            {
                int dpiLeido = 203;
                try
                {
                    int x = new System.Drawing.Printing.PrinterSettings().DefaultPageSettings.PrinterResolution.X;
                    if (x >= 150) dpiLeido = x;
                }
                catch { }
                dpiImpresoraBENS = dpiLeido;
            }
            int dpi = dpiImpresoraBENS;

            // ancho del codigo en modulos (barra mas delgada = 1)
            BarcodeLib.Barcode medida = new BarcodeLib.Barcode { IncludeLabel = false, BarWidth = 1, Height = 10 };
            int modulos;
            using (Image m = medida.Encode(BarcodeLib.TYPE.CODE128, texto, Color.Black, Color.White))
                modulos = m.Width;

            const int zonaSilencio = 10; // modulos a cada lado
            int anchoPuntos = (int)(AnchoCodigoBENS * dpi);
            int altoPuntos = (int)(AltoCodigoBENS * dpi);
            int modulo = Math.Max(1, anchoPuntos / (modulos + zonaSilencio * 2));

            BarcodeLib.Barcode codigo = new BarcodeLib.Barcode { IncludeLabel = false, BarWidth = modulo, Height = altoPuntos };
            Image barras = codigo.Encode(BarcodeLib.TYPE.CODE128, texto, Color.Black, Color.White);

            Bitmap final = new Bitmap(barras.Width + zonaSilencio * 2 * modulo, altoPuntos);
            using (Graphics g = Graphics.FromImage(final))
            {
                g.Clear(Color.White);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImageUnscaled(barras, zonaSilencio * modulo, 0);
            }
            barras.Dispose();
            final.SetResolution(dpi, dpi); // tamano fisico exacto: el reporte no lo reescala
            return final;
        }

        /// <summary>
        /// Texto del codigo de barras Ben Davis para PANTALONES: PO|ESTILO|CINTURA|LARGO|QTY|CARTON (pipes).
        /// Mismo formato que arma GuardarInventarioBeans.
        /// </summary>
        private static string TextoCodigoBENS(string poVarchar, string estilo, string izq, string der, decimal cantidad, int carton)
        {
            return string.Concat((poVarchar ?? "").Trim(), "|", (estilo ?? "").Trim(), "|", (izq ?? "").Trim(), "|", (der ?? "").Trim(), "|",
                                 Convert.ToInt32(cantidad), "|", carton.ToString().PadLeft(5, '0'));
        }

        /// <summary>
        /// Texto del codigo de barras Ben Davis para CAMISAS: ESTILO:TALLA:*:QTY:CARTON (dos puntos, sin PO, LARGO="*").
        /// Formato especificado por el cliente (QIMA): "124:XL:*:30:000001", Code128,
        /// [STYLE#]:[SIZE]:[LENGTH]:[QTY]:[BOX#] y cada etiqueta debe ser unica (incrementar carton).
        /// </summary>
        private static string TextoCodigoBENSCamisa(string estilo, string talla, decimal cantidad, int carton)
        {
            return string.Concat((estilo ?? "").Trim(), "|", (talla ?? "").Trim(), "|*|",
                                 Convert.ToInt32(cantidad), "|", carton.ToString().PadLeft(5, '0'));
        }

        /// <summary>
        /// Imprime una etiqueta Ben Davis (todas las copias en un solo trabajo). En reimpresion la etiqueta lleva
        /// una "R" en una esquina libre para distinguirla de la original.
        /// </summary>
        private void ImprimirEtiquetaBENS(ReporteRdlc report, int copias, bool reimpresion)
        {
            report.Reimpresion = reimpresion;
            new ReportPrintTool(report).Print(null, copias);
        }

        // Reimpresion de caja normal: camisa (sin LENGTH) va en su plantilla y con el codigo de camisa,
        // igual que al escanearla; pantalon sale con el codigo guardado.
        private void ImprimirReimpresionBENS(List<EtiquetaBens> lista)
        {
            EtiquetaBens et = lista[0];
            string der = (et.size_derecho ?? "").Trim();
            if (der == "" || der == "*")
            {
                et.size_derecho = "*";
                et.id_string = TextoCodigoBENSCamisa(et.ESTILO ?? et.ProductCode, et.size_izquierdo, et.Cantidad ?? 0m, Convert.ToInt32(et.cn_tag_num));
                et.anio = et.id_string;
                et.codigoBarras = CodigoBarrasBENS(et.id_string);
                ImprimirEtiquetaBENS(new ReporteCajaBensCamisas { DataSource = lista }, CopiasEtiquetaBENS, true);
            }
            else
                ImprimirEtiquetaBENS(new ReporteCajaBens { DataSource = lista }, CopiasEtiquetaBENS, true);
        }

        private static bool EsMixtaBENS(string sizeIzquierdo)
        {
            return string.Equals((sizeIzquierdo ?? "").Trim(), "MIXTO", StringComparison.OrdinalIgnoreCase);
        }

        // El renglon padre "MIXTO" no tiene tallas ni cantidades: se reimprime una etiqueta por talla del
        // detalle, con el carton del padre, igual que al escanear la caja mixta.
        private void ReimprimirMixtaBENS(int idInventario, int numeroCarton)
        {
            List<ConsultaInventarioDetalleResult> detalle = f.ConsultaInventarioDetalle(idInventario);
            if (detalle == null || detalle.Count == 0)
            {
                MessageBox.Show("No se encontro el detalle de tallas de la caja mixta " + idInventario + ".");
                return;
            }
            // En vista previa se juntan todas las tallas en un solo reporte (una hoja por talla);
            // al imprimir se manda talla por talla con sus copias.
            bool vistaPrevia = ReportPrintTool.ModoVistaPreviaReimpresion;
            List<EtiquetaBens> previaPantalon = new List<EtiquetaBens>();
            List<EtiquetaBens> previaCamisa = new List<EtiquetaBens>();
            foreach (ConsultaInventarioDetalleResult item in detalle)
            {
                string poRef = (item.po ?? "").Trim();
                string poDigitos = string.Concat(poRef.Where(c => char.IsDigit(c)));
                string izq = (item.size_izquierdo ?? "").Trim();
                string der = (item.size_derecho ?? "").Trim();
                bool camisa = der == "" || der == "*";
                if (camisa) der = "*";
                decimal cantidad = item.Cantidad ?? 0;

                List<ConsultaProductosBensResult> producto = f.ConsultaProductosBens(poRef, izq, camisa ? "" : der);
                bool hayProducto = producto != null && producto.Count > 0;

                EtiquetaBens etiqueta = new EtiquetaBens();
                etiqueta.id = item.id;
                etiqueta.po = poDigitos;
                etiqueta.po_varchar = poRef;
                etiqueta.poInCompleto = string.IsNullOrEmpty(poDigitos) ? 0m : Convert.ToDecimal(poDigitos);
                etiqueta.poItem = "55";
                etiqueta.ProductCode = item.estilo;
                etiqueta.ESTILO = item.estilo;
                etiqueta.Size = izq;
                etiqueta.size_izquierdo = izq;
                etiqueta.size_derecho = der;
                etiqueta.color = hayProducto ? producto[0].Color : item.color;
                etiqueta.TipoCarton = "1";
                etiqueta.upc = item.upc;
                etiqueta.Fecha = DateTime.Now;
                etiqueta.usuario = usu[0].nombre;
                etiqueta.QUANTITY = Convert.ToInt32(cantidad).ToString();
                etiqueta.Cantidad = cantidad;
                etiqueta.DESCRIPTION = hayProducto ? producto[0].itemDescription : "";
                etiqueta.itemDescription = etiqueta.DESCRIPTION;
                etiqueta.cn_tag_num = numeroCarton;
                etiqueta.id_string = camisa
                    ? TextoCodigoBENSCamisa(item.estilo, izq, cantidad, numeroCarton)
                    : TextoCodigoBENS(poRef, item.estilo, izq, der, cantidad, numeroCarton);
                etiqueta.anio = etiqueta.id_string;
                etiqueta.Vendor = etiqueta.id_string;
                etiqueta.assembly = etiqueta.id_string;
                etiqueta.codigoBarras = CodigoBarrasBENS(etiqueta.id_string);

                if (vistaPrevia)
                {
                    (camisa ? previaCamisa : previaPantalon).Add(etiqueta);
                    continue;
                }
                List<EtiquetaBens> lista = new List<EtiquetaBens> { etiqueta };
                if (camisa)
                    ImprimirEtiquetaBENS(new ReporteCajaBensCamisas { DataSource = lista }, CopiasEtiquetaMixtaBENS, true);
                else
                    ImprimirEtiquetaBENS(new ReporteCajaBens { DataSource = lista }, CopiasEtiquetaMixtaBENS, true);
            }
            if (previaPantalon.Count > 0)
                ImprimirEtiquetaBENS(new ReporteCajaBens { DataSource = previaPantalon }, 1, true);
            if (previaCamisa.Count > 0)
                ImprimirEtiquetaBENS(new ReporteCajaBensCamisas { DataSource = previaCamisa }, 1, true);
        }

        private static bool EsImpresoraZebra(string n)
        {
            if (string.IsNullOrEmpty(n)) return false;
            string u = n.ToUpper();
            return u.Contains("ZDESIGNER") || u.Contains("ZEBRA") || u.Contains("ZT") || u.Contains("ZD")
                && !u.Contains("PDF") && !u.Contains("XPS") && !u.Contains("ONENOTE") && !u.Contains("FAX");
        }

        private static int ResolucionImpresoraActual()
        {
            try { return new System.Drawing.Printing.PrinterSettings().DefaultPageSettings.PrinterResolution.X; }
            catch { return 203; }
        }

        /// <summary>
        /// Arma el ZPL para la etiqueta Ben Davis segun la plantilla oficial (QIMA): barcode arriba +
        /// cuadricula 2x3 con STYLE#/COLOR, WAIST/LENGTH, QTY/CUT#. Camisas usan LENGTH="*". Si es
        /// reimpresion, pone una "R" chica en la esquina.
        /// </summary>
        private static string ArmarZplBENS(object et, bool reimpresion, int dpi)
        {
            // Lee campos por reflexion para no acoplarnos al tipo concreto (EtiquetaBens o EtiquetaCajaModificada)
            Func<string, string> leer = n => { var p = et.GetType().GetProperty(n); object v = p == null ? null : p.GetValue(et); return v == null ? "" : v.ToString(); };
            string estilo = leer("ESTILO"); if (estilo == "") estilo = leer("ProductCode");
            string color = leer("color").ToUpper();
            string waist = leer("size_izquierdo");
            string length = leer("size_derecho"); if (length == "") length = "*";
            string qty = leer("QUANTITY"); if (qty == "") qty = leer("Cantidad");
            string cut = leer("po_varchar");
            string descr = leer("itemDescription").ToUpper();
            if (descr == "") descr = leer("DESCRIPTION").ToUpper();
            string codigo = leer("id_string");

            // Dot por mm segun el DPI: 203 dpi = 8 dots/mm, 300 dpi = ~12 dots/mm. Trabajamos en dots.
            int pw = 4 * dpi;    // 4 pulgadas de ancho
            int ll = 6 * dpi;    // 6 pulgadas de alto
            int margen = (int)(0.1 * dpi);
            int gridTop = (int)(1.1 * dpi);  // tras el barcode
            int cellW = (pw - 2 * margen) / 2;
            int cellH = (int)(1.3 * dpi);

            Func<int,int,int,int,string> gb = (x,y,w,h) => "^FO" + x + "," + y + "^GB" + w + "," + h + ",3^FS";
            Func<string,int,int,int,string,string> rotuloCentrado = (texto, x, y, fs, align) => {
                int w = cellW; int ancho = fs; int alto = fs;
                return "^FO" + x + "," + y + "^A0N," + alto + "," + ancho + "^FB" + w + ",1,0," + align + ",0^FD" + texto + "^FS";
            };

            var sb = new System.Text.StringBuilder();
            sb.Append("^XA^PON^PW").Append(pw).Append("^LL").Append(ll).Append("^LH0,0~SD15^CF0,50");
            // --- Barcode arriba (3.8 in x 0.4 in centrado) ---
            int bcH = (int)(0.5 * dpi);
            int bcY = (int)(0.1 * dpi);
            int bcX = (int)(0.1 * dpi);
            // Ajustar modulo (BY) para 300 dpi: BY=3 funciona bien a 300 dpi. A 203 dpi usar BY=2.
            int by = dpi >= 300 ? 3 : 2;
            sb.Append("^FO").Append(bcX).Append(",").Append(bcY).Append("^BY").Append(by)
              .Append("^BCN,").Append(bcH).Append(",N,N,N^FD").Append(codigo).Append("^FS");
            // Texto del codigo bajo el barcode
            sb.Append("^FO").Append(bcX).Append(",").Append(bcY + bcH + 10)
              .Append("^A0N,").Append(dpi >= 300 ? 40 : 28).Append(",").Append(dpi >= 300 ? 40 : 28)
              .Append("^FB").Append(pw - 2 * bcX).Append(",1,0,C,0^FD").Append(codigo).Append("^FS");

            // --- Cuadricula 2x3 ---
            string[,] labels = new string[,] {
                { "STYLE#", "COLOR" },
                { "WAIST",  "LENGTH" },
                { "QTY",    "CUT#" }
            };
            string[,] values = new string[,] {
                { estilo, color },
                { waist,  length },
                { qty,    cut }
            };
            int fsLabel = dpi >= 300 ? 36 : 24;
            int fsValue = dpi >= 300 ? 90 : 60;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    int x = margen + c * cellW;
                    int y = gridTop + r * cellH;
                    sb.Append(gb(x, y, cellW, cellH));
                    // etiqueta (header)
                    sb.Append("^FO").Append(x).Append(",").Append(y + 10)
                      .Append("^A0N,").Append(fsLabel).Append(",").Append(fsLabel)
                      .Append("^FB").Append(cellW).Append(",1,0,C,0^FD").Append(labels[r,c]).Append("^FS");
                    // valor (grande)
                    sb.Append("^FO").Append(x).Append(",").Append(y + fsLabel + 25)
                      .Append("^A0N,").Append(fsValue).Append(",").Append(fsValue)
                      .Append("^FB").Append(cellW).Append(",1,0,C,0^FD").Append(values[r,c]).Append("^FS");
                }
            }
            // Debajo del STYLE pintamos la descripcion chica (p.ej. "GORILLA CUT")
            if (descr != "")
            {
                int x = margen; int y = gridTop + cellH - 50;
                sb.Append("^FO").Append(x).Append(",").Append(y)
                  .Append("^A0N,").Append(dpi >= 300 ? 32 : 22).Append(",").Append(dpi >= 300 ? 32 : 22)
                  .Append("^FB").Append(cellW).Append(",1,0,C,0^FD").Append(descr).Append("^FS");
            }
            // Marca de reimpresion
            if (reimpresion)
            {
                int rSize = (int)(0.3 * dpi);
                int rx = pw - margen - rSize;
                int ry = gridTop + 3 * cellH + 10;
                sb.Append("^FO").Append(rx).Append(",").Append(ry)
                  .Append("^GB").Append(rSize).Append(",").Append(rSize).Append(",3^FS");
                sb.Append("^FO").Append(rx).Append(",").Append(ry + 5)
                  .Append("^A0N,").Append(rSize - 10).Append(",").Append(rSize - 10)
                  .Append("^FB").Append(rSize).Append(",1,0,C,0^FDR^FS");
            }
            sb.Append("^XZ");
            return sb.ToString();
        }

        private void EscanearPrendaMixtaBENS()
        {
            tpEscan.BackColor = Tema.Fondo;
            string upcEscaneado = txtUPCScann.Text.Trim();
            List<ConsultaUPC_VarcharResult> info = f.ConsultaUPC(cmbPO.Text, upcEscaneado);

            if (info == null || info.Count == 0)
            {
                DialogResult result;
                do
                {
                    result = MessageBox.Show(this, "PRESIONA EN NO PARA CONTINUAR", "UPC no reconocido para este PO!",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                    sonido.controls.play();
                    txtUPCScann.Text = string.Empty;
                    tpEscan.BackColor = Color.Red;
                } while (result == DialogResult.Yes);
                return;
            }

            if (tablaMixtaBENS == null) InicializarTablaMixtaBENS();

            // Requisito del cliente: una caja mixta lleva maximo 4 tallas distintas
            bool tallaNueva = tablaMixtaBENS.Select("UPC = '" + info[0].upc.Replace("'", "''") + "'").Length == 0;
            if (tallaNueva && tablaMixtaBENS.Rows.Count >= MaxTallasCajaMixtaBENS)
            {
                sonido.URL = Application.StartupPath + @"\mp3\error.mp3";
                sonido.controls.play();
                tpEscan.BackColor = Color.Red;
                MessageBox.Show(this, "La caja mixta ya tiene " + MaxTallasCajaMixtaBENS + " tallas. Cierra esta caja (Incomplete Carton) y empieza otra para esta talla.",
                    "Maximo " + MaxTallasCajaMixtaBENS + " tallas por caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                return;
            }

            AcumularTallaBENS(ArmarTallaBENS(info[0].size_izquierdo, info[0].size_derecho), info[0].upc, 1);

            dgvEscan.DataSource = null;
            dgvEscan.DataSource = tablaMixtaBENS;

            int total = TotalPrendasMixtaBENS();
            contador = total;
            txtUnitsScan.Text = total.ToString();
            txtUnitsReq.Text = "X";
            txtUnitsRemai.Text = "X";
            txtSize.Text = "MIXTO";

            txtUPCScann.Text = string.Empty;
            txtUPCScann.Focus();
            sonido.URL = Application.StartupPath + @"\mp3\correct.mp3";
            sonido.controls.play();
        }

        private void ReiniciarCajaMixtaBENS()
        {
            TallaMixta = 0;
            PrendasExtra = 0;
            tablaMixtaBENS = null;
            contador = 0;
            try
            {
                // Recarga el grid normal de Ben Davis para la talla seleccionada
                int idx = cmbSizes.SelectedIndex;
                cmbSizes.SelectedIndex = -1;
                cmbSizes.SelectedIndex = idx < 0 ? 0 : idx;
                LimpiarPantallaEscaneo();
            }
            catch { }
        }

        private void GuardarCajaMixtaBENS()
        {
            if (tablaMixtaBENS == null || tablaMixtaBENS.Rows.Count == 0)
            {
                MessageBox.Show("No hay tallas escaneadas para guardar/imprimir.");
                return;
            }

            List<EtiquetaCajaModificada> detalle = new List<EtiquetaCajaModificada>();
            decimal total = 0;
            string resumenTallas = "";

            foreach (DataRow row in tablaMixtaBENS.Rows)
            {
                string upcFila = row["UPC"].ToString();
                List<ConsultaUPC_VarcharResult> info = f.ConsultaUPC(cmbPO.Text, upcFila);
                if (info == null || info.Count == 0) continue;

                int cant = Convert.ToInt32(row["Cantidad"]);
                EtiquetaCajaModificada x = new EtiquetaCajaModificada();
                x.id = info[0].id;
                x.po = info[0].po;
                x.po_varchar = info[0].po;
                x.poItem = "55";
                x.ProductCode = info[0].estilo;
                x.estilo = info[0].estilo;
                x.color = info[0].color;
                x.size_izquierdo = info[0].size_izquierdo;
                x.size_derecho = info[0].size_derecho;
                x.Size = row["Talla"].ToString();
                x.upc = upcFila;
                x.Cantidad = cant;
                x.mixto = 1;
                x.TipoCarton = "MIXTO";
                x.usuario = usu[0].nombre;
                x.id_cliente = cmbCliente.Text == "NA" ? 1 : Convert.ToInt32(cmbCliente.SelectedValue);
                x.id_factura = cmbFactura.Text == "NA" ? 1 : Convert.ToInt32(cmbFactura.SelectedValue);
                x.id_terminado = cmbTerminado.Text == "NA" ? 1 : Convert.ToInt32(cmbTerminado.SelectedValue);
                x.cliente = cmbCliente.Text;
                x.factura = cmbFactura.Text;
                x.terminado = cmbTerminado.Text;
                x.Fecha = DateTime.Now;
                detalle.Add(x);

                total += cant;
                resumenTallas += row["Talla"] + ":" + cant + "  ";
            }

            if (detalle.Count == 0)
            {
                MessageBox.Show("No quedaron tallas validas para guardar.");
                return;
            }

            // po_numero de Ben Davis se guarda solo con digitos (igual que GuardarInventarioBeans),
            // asi la caja mixta comparte la misma secuencia de cartones que las cajas normales.
            string poNumeros = string.Concat(cmbPO.Text.Where(c => char.IsDigit(c)));
            int numeroCarton = f.SiguienteCartonBens(poNumeros);

            // Header "MIXTO" (mismo patron que IFR/TYNDALE)
            EtiquetaCajaModificada header = new EtiquetaCajaModificada();
            header.id = detalle[0].id;
            header.po = poNumeros;
            header.Carton = numeroCarton;
            header.po_varchar = detalle[0].po_varchar;
            header.poItem = "55";
            header.ProductCode = detalle[0].ProductCode;
            header.estilo = detalle[0].estilo;
            header.color = detalle[0].color;
            header.size_izquierdo = "MIXTO";
            header.size_derecho = "";
            header.Size = "MIXTO";
            header.upc = "";
            header.Cantidad = 0;
            header.QUANTITY = Convert.ToInt32(total);
            header.mixto = 1;
            header.TipoCarton = "1";
            header.usuario = usu[0].nombre;
            header.id_cliente = detalle[0].id_cliente;
            header.id_factura = detalle[0].id_factura;
            header.id_terminado = detalle[0].id_terminado;
            header.cliente = detalle[0].cliente;
            header.factura = detalle[0].factura;
            header.terminado = detalle[0].terminado;
            header.Fecha = DateTime.Now;

            var idInventario = f.GuardaInventario(header, usu[0].id);
            if (idInventario == 0)
            {
                MessageBox.Show("No se pudo guardar la caja mixta. No se imprimio ninguna etiqueta.");
                return;
            }

            foreach (EtiquetaCajaModificada item in detalle)
            {
                item.id_inventario = idInventario;
                f.GuardaInventarioDetalle(item, usu[0].id);
            }

            // Una etiqueta por talla (maximo 4), todas con el mismo numero de carton y el mismo
            // formato de codigo de barras que las cajas normales: PO|ESTILO|CINTURA|LARGO|QTY|CARTON
            decimal poDec;
            decimal.TryParse(poNumeros, out poDec);
            foreach (EtiquetaCajaModificada item in detalle)
            {
                string izq = (item.size_izquierdo ?? "").Trim();
                string der = (item.size_derecho ?? "").Trim();
                bool camisa = der == "" || der == "*";
                if (camisa) der = "*";

                List<ConsultaProductosBensResult> producto = f.ConsultaProductosBens(cmbPO.Text, izq, camisa ? "" : der);
                string descripcion = (producto != null && producto.Count > 0) ? producto[0].itemDescription : "";

                EtiquetaBens etiqueta = new EtiquetaBens();
                etiqueta.id = Convert.ToInt32(item.id);
                etiqueta.po = poNumeros;
                etiqueta.po_varchar = (item.po_varchar ?? "").Trim();
                etiqueta.poInCompleto = poDec;
                etiqueta.poItem = "55";
                etiqueta.ProductCode = item.estilo;
                etiqueta.ESTILO = item.estilo;
                etiqueta.Size = izq;
                etiqueta.size_izquierdo = izq;
                etiqueta.size_derecho = der;
                etiqueta.color = item.color;
                etiqueta.TipoCarton = "1";
                etiqueta.upc = item.upc;
                etiqueta.Fecha = DateTime.Now;
                etiqueta.CartonLeft = "";
                etiqueta.CartonRight = "";
                etiqueta.usuario = usu[0].nombre;
                etiqueta.id_cliente = header.id_cliente;
                etiqueta.id_factura = header.id_factura;
                etiqueta.id_terminado = header.id_terminado;
                etiqueta.cliente = header.cliente;
                etiqueta.factura = header.factura;
                etiqueta.terminado = header.terminado;
                etiqueta.QUANTITY = Convert.ToInt32(item.Cantidad).ToString();
                etiqueta.Cantidad = Convert.ToDecimal(item.Cantidad);
                etiqueta.DESCRIPTION = descripcion;
                etiqueta.itemDescription = descripcion;
                etiqueta.cn_tag_num = numeroCarton;
                // CAMISAS vs PANTALONES: camisas usan formato con dos puntos (ESTILO:TALLA:*:QTY:CARTON) por pedido del cliente.
                etiqueta.id_string = camisa
                    ? TextoCodigoBENSCamisa(item.estilo, izq, Convert.ToDecimal(item.Cantidad), numeroCarton)
                    : TextoCodigoBENS(etiqueta.po_varchar, item.estilo, izq, der, Convert.ToDecimal(item.Cantidad), numeroCarton);
                etiqueta.anio = etiqueta.id_string;
                etiqueta.Vendor = etiqueta.id_string;
                etiqueta.assembly = etiqueta.id_string;
                etiqueta.codigoBarras = CodigoBarrasBENS(etiqueta.id_string);

                List<EtiquetaBens> lista = new List<EtiquetaBens> { etiqueta };
                if (camisa)
                    ImprimirEtiquetaBENS(new ReporteCajaBensCamisas { DataSource = lista }, CopiasEtiquetaMixtaBENS, false);
                else
                    ImprimirEtiquetaBENS(new ReporteCajaBens { DataSource = lista }, CopiasEtiquetaMixtaBENS, false);
            }

            id_inventarioAnt = Convert.ToInt64(idInventario);
            ReiniciarCajaMixtaBENS();
        }

        private void IncompleteCarton_BENS_Mixta()
        {
            GuardarCajaMixtaBENS();
        
        }

        private void IncompleteCarton_BENS()
        {
            try
            {
                /*TOTAL DE PRENDAS ESCANEADAS*/
                string[] separadas;

                string input = cmbSizes.Text;
                string talla1 = "";
                string talla2 = "";
                if (input.Contains("x"))
                {
                    separadas = cmbSizes.Text.Split('x');
                    talla1 = separadas[0].ToString().Trim();
                    talla2 = separadas[1].ToString().Trim();
                }
                else
                {
                    separadas = new string[1];
                    separadas[0] = cmbSizes.Text; input = cmbSizes.Text;

                    talla1 = cmbSizes.Text;
                    talla2 = "";
                }
                /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                List<ConsultaProductosBensResult> x = f.ConsultaProductosBens(cmbPO.Text, talla1, talla2);

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

                    EtiquetaCajaModificada respuesta = f.GuardaInventarioBeans(clase, usu[0].id);
                    clase.id_string = respuesta.codigoBarrasTextoB.ToString().Trim();

                    // CAMISAS: el cliente (QIMA) pide formato ESTILO:TALLA:*:QTY:CARTON (dos puntos, sin PO).
                    // El SP genera el formato con pipes para pantalones; para camisas lo rearmamos aqui.
                    bool esCamisa = string.IsNullOrEmpty(clase.size_derecho) || clase.size_derecho == "*";
                    if (esCamisa)
                        clase.id_string = TextoCodigoBENSCamisa(clase.ProductCode, clase.size_izquierdo, clase.Cantidad ?? 0m, Convert.ToInt32(clase.cn_tag_num));

                    clase.codigoBarras = CodigoBarrasBENS(clase.id_string);
                    //BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                    //{
                    //    IncludeLabel = false
                    //};
                    //Codigo.Height = 70; // en vez de 45

                    ////Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE128, clase.id_string.ToString(), Color.Black, Color.White, 500, 45);
                    //Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE128, clase.id_string.ToString(), Color.Black, Color.White, 370, 45);
                    //clase.codigoBarras = codigoBarras;

                    // AI1425]676]32]32]1]00092




                    DateTime fechaEntrada = DateTime.Now;

                    clase.anio = respuesta.codigoBarrasTextoB.ToString();


                    //id_inventarioAnt = Convert.ToInt32(clase.id);
                    int idIndex = Convert.ToInt32(cmbSizes.SelectedIndex);

                    List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                    listclaseZ.Add(clase);
                    //id_inventarioAnt = clase.id_inventario;
                    //id_inventarioAnt = Convert.ToInt32(clase.id_string);
                    ImprimirEtiquetaBENS(new ReporteCajaBens { DataSource = listclaseZ }, CopiasEtiquetaBENS, false);

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

        private void SeleccionTalla_BENS(ref string[] separadas)
        {
            lblImprimirEtiqueta.Visible = false;
            chkImprimirEtiqueta.Visible = false;
            try
            {

                string input = cmbSizes.Text;
                string talla1 = "";
                string talla2 = "";
                if (input.Contains("x"))
                {
                    separadas = cmbSizes.Text.Split('x');
                    talla1 = separadas[0].ToString().Trim();
                    talla2 = separadas[1].ToString().Trim();
                }
                else
                {
                    separadas = new string[1];
                    separadas[0] = cmbSizes.Text; input = cmbSizes.Text;

                    talla1 = cmbSizes.Text;
                    talla2 = "";
                }
                List<ConsultaProductosBensEscaneoResult> x = f.ConsultaProductosBensEscaneo(cmbPO.Text, talla1, talla2);
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

        private void Escaneo_BENS(ref List<ConsultaUPC_VarcharResult> ListUpc, string po_numero)
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
                    string input = cmbSizes.Text;
                    string talla1 = "";
                    string talla2 = "";
                    int camisas = 0;
                    if (input.Contains("x"))
                    {
                        separadas = cmbSizes.Text.Split('x');
                        talla1 = separadas[0].ToString().Trim();
                        talla2 = separadas[1].ToString().Trim();
                    }
                    else
                    {
                        separadas = new string[1];
                        separadas[0] = cmbSizes.Text; input = cmbSizes.Text;
                        camisas = 1;
                        talla1 = cmbSizes.Text;
                        talla2 = "";

                    }
                    /*AQUI ME QUEDE CON DE LA ETIQUETA DE BENS*/
                    List<ConsultaProductosBensResult> x = f.ConsultaProductosBens(cmbPO.Text, talla1, talla2);
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
                        //clase.size_derecho = x[0].size_derecho;

                        if (x[0].size_derecho.Trim() == "")
                        {
                            clase.size_derecho = "*";
                        }
                        else
                        {
                            clase.size_derecho = x[0].size_derecho;
                        }

                        clase.cn_tag_num = Convert.ToInt32(x[0].CartonNumber);

                        EtiquetaCajaModificada respuesta = f.GuardaInventarioBeans(clase, usu[0].id);

                        clase.id_string = respuesta.codigoBarrasTextoB.ToString().Trim();
                        clase.anio = respuesta.codigoBarrasTextoB.ToString().Trim();
                        clase.assembly = clase.id_string;

                        clase.codigoBarras = CodigoBarrasBENS(clase.id_string);

                        clase.Vendor = clase.anio;
                        DateTime fechaEntrada = DateTime.Now;


                        List<EtiquetaBens> listclaseZ = new List<EtiquetaBens>();
                        listclaseZ.Add(clase);

                        if (camisas == 1)
                            ImprimirEtiquetaBENS(new ReporteCajaBensCamisas { DataSource = listclaseZ }, CopiasEtiquetaBENS, false);
                        else
                            ImprimirEtiquetaBENS(new ReporteCajaBens { DataSource = listclaseZ }, CopiasEtiquetaBENS, false);

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
    }
}
