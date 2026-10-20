using Microsoft.Reporting.WinForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Inventario.Reportes
{
    /// <summary>
    /// Impresion de reportes RDLC (Microsoft ReportViewer, gratuito). Reemplaza a DevExpress XtraReports:
    /// imprimir directo, vista previa y exportar a PDF. Los .rdlc van incrustados en el ejecutable
    /// (carpeta Reportes), asi la instalacion ClickOnce no necesita archivos sueltos.
    /// </summary>
    public static class ImpresionRdlc
    {
        private static readonly XNamespace R = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
        private static readonly Dictionary<string, XDocument> cache = new Dictionary<string, XDocument>();

        /// <summary>Carga la definicion del reporte (recurso incrustado Inventario.Reportes.{nombre}.rdlc o ruta de archivo).</summary>
        public static Func<string, Stream> AbrirDefinicion = nombre =>
        {
            if (File.Exists(nombre)) return File.OpenRead(nombre);
            Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Inventario.Reportes." + nombre + ".rdlc");
            if (s == null) throw new FileNotFoundException("No se encontro el reporte " + nombre);
            return s;
        };

        private static XDocument Definicion(string nombre)
        {
            lock (cache)
            {
                XDocument d;
                if (!cache.TryGetValue(nombre, out d))
                {
                    using (Stream s = AbrirDefinicion(nombre)) d = XDocument.Load(s);
                    cache[nombre] = d;
                }
                return d;
            }
        }

        // Un LocalReport ya compilado por reporte: crear uno nuevo en cada etiqueta recompila las expresiones (~0.5 s).
        private static readonly Dictionary<string, LocalReport> compilados = new Dictionary<string, LocalReport>();

        /// <summary>Arma el LocalReport con los datos (lista de objetos de cualquier clase).</summary>
        public static LocalReport Crear(string nombre, IEnumerable datos, bool reimpresion = false)
        {
            XDocument def = Definicion(nombre);
            DataTable tabla = ATabla(datos);
            AjustarImagenes(def, tabla);
            lock (compilados)
            {
                LocalReport lr;
                if (!compilados.TryGetValue(nombre, out lr))
                {
                    lr = new LocalReport();
                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(def.ToString(SaveOptions.DisableFormatting))))
                        lr.LoadReportDefinition(ms);
                    compilados[nombre] = lr;
                }
                lr.DataSources.Clear();
                lr.DataSources.Add(new ReportDataSource("Datos", tabla));
                if (def.Descendants(R + "ReportParameter").Any(p => (string)p.Attribute("Name") == "Reimpresion"))
                    lr.SetParameters(new ReportParameter("Reimpresion", reimpresion ? "true" : "false"));
                lr.Refresh();
                return lr;
            }
        }

        /// <summary>
        /// Compila y renderiza en segundo plano los reportes de etiquetas al abrir el programa,
        /// para que la primera etiqueta no tarde 3-4 segundos.
        /// </summary>
        public static void Precalentar(params string[] nombres)
        {
            var hilo = new System.Threading.Thread(() =>
            {
                foreach (string nombre in nombres)
                {
                    try
                    {
                        Warning[] w;
                        lock (compilados)
                            Crear(nombre, new List<object>()).Render("Image", "<DeviceInfo><OutputFormat>EMF</OutputFormat></DeviceInfo>", (n, ext, enc, mime, seek) => new MemoryStream(), out w);
                    }
                    catch { }
                }
            }) { IsBackground = true, Priority = System.Threading.ThreadPriority.BelowNormal };
            hilo.Start();
        }

        /// <summary>Imprime directo (equivale a ReportPrintTool.Print()).</summary>
        public static void Imprimir(string nombre, IEnumerable datos, string impresora = null, int copias = 1, bool reimpresion = false)
        {
            // Driver Zebra ignora PrinterSettings.Copies: duplicamos la data N veces para que
            // el reporte genere N paginas y se impriman todas en un solo trabajo.
            int n = Math.Max(1, copias);
            IEnumerable datosExpandidos = datos;
            if (n > 1)
            {
                var lista = new List<object>();
                for (int c = 0; c < n; c++)
                    foreach (object o in datos) lista.Add(o);
                datosExpandidos = lista;
            }
            lock (compilados) ImprimirLocal(Crear(nombre, datosExpandidos, reimpresion), impresora, 1, null);
        }

        /// <summary>
        /// Imprime a un archivo con una impresora virtual (p.ej. "Microsoft Print to PDF") sin preguntar donde guardarlo.
        /// Pasa por el mismo camino que la impresion real, por eso sirve para revisar etiquetas.
        /// </summary>
        public static void ImprimirAArchivo(string nombre, IEnumerable datos, string impresora, string archivo, bool reimpresion = false)
        {
            lock (compilados) ImprimirLocal(Crear(nombre, datos, reimpresion), impresora, 1, archivo);
        }

        /// <summary>Si se asigna, la vista previa se dibuja dentro de este control en vez de abrir otra ventana.</summary>
        public static Control DestinoVistaPrevia;

        /// <summary>Vista previa con el visor de ReportViewer (equivale a ShowPreview()).</summary>
        public static void VistaPrevia(string nombre, IEnumerable datos, string titulo = null, bool reimpresion = false)
        {
            Control destino = DestinoVistaPrevia;
            Form forma = destino == null
                ? new Form { Text = titulo ?? nombre, Width = 900, Height = 750, StartPosition = FormStartPosition.CenterScreen, WindowState = FormWindowState.Maximized }
                : null;
            var visor = new ReportViewer { Dock = DockStyle.Fill, ProcessingMode = ProcessingMode.Local };
            XDocument def = Definicion(nombre);
            DataTable tabla = ATabla(datos);
            AjustarImagenes(def, tabla);
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(def.ToString(SaveOptions.DisableFormatting))))
                visor.LocalReport.LoadReportDefinition(ms);
            visor.LocalReport.DataSources.Add(new ReportDataSource("Datos", tabla));
            if (def.Descendants(R + "ReportParameter").Any(p => (string)p.Attribute("Name") == "Reimpresion"))
                visor.LocalReport.SetParameters(new ReportParameter("Reimpresion", reimpresion ? "true" : "false"));
            visor.SetDisplayMode(DisplayMode.PrintLayout);
            if (destino != null)
            {
                foreach (Control viejo in destino.Controls.Cast<Control>().ToList()) viejo.Dispose();
                visor.ZoomMode = ZoomMode.PageWidth;
                destino.Controls.Add(visor);
                destino.Visible = true;
                destino.BringToFront();
                visor.RefreshReport();
                return;
            }
            visor.ZoomMode = ZoomMode.PageWidth;
            forma.Controls.Add(visor);
            visor.RefreshReport();
            forma.Show();
        }

        /// <summary>Exporta a PDF y regresa los bytes.</summary>
        public static byte[] Pdf(string nombre, IEnumerable datos)
        {
            string mime, enc, ext; string[] streams; Warning[] w;
            lock (compilados) return Crear(nombre, datos).Render("PDF", null, out mime, out enc, out ext, out streams, out w);
        }

        // ---------------- impresion directa (EMF por pagina) ----------------
        private static void ImprimirLocal(LocalReport lr, string impresora, int copias, string archivo)
        {
            ReportPageSettings ps = lr.GetDefaultPageSettings();
            float anchoIn = ps.PaperSize.Width / 100f, altoIn = ps.PaperSize.Height / 100f;
            // En reportes horizontales (6x4) ReportViewer regresa el papel ya volteado (4x6);
            // si no se corrige, el EMF sale de 4" de ancho y la etiqueta se parte en dos paginas
            if (ps.IsLandscape && anchoIn < altoIn) { float t = anchoIn; anchoIn = altoIn; altoIn = t; }
            string di = string.Format(CultureInfo.InvariantCulture,
                "<DeviceInfo><OutputFormat>EMF</OutputFormat><PageWidth>{0}in</PageWidth><PageHeight>{1}in</PageHeight>" +
                "<MarginTop>{2}in</MarginTop><MarginLeft>{3}in</MarginLeft><MarginRight>{4}in</MarginRight><MarginBottom>{5}in</MarginBottom></DeviceInfo>",
                anchoIn, altoIn, ps.Margins.Top / 100f, ps.Margins.Left / 100f, ps.Margins.Right / 100f, ps.Margins.Bottom / 100f);

            var paginas = new List<Stream>();
            Warning[] warns;
            lr.Render("Image", di, (n, ext, enc, mime, seek) => { var m = new MemoryStream(); paginas.Add(m); return m; }, out warns);
            if (paginas.Count == 0) return;
            var metas = paginas.Select(p => { p.Position = 0; return new Metafile(p); }).ToList();

            using (var pd = new PrintDocument())
            {
                pd.DocumentName = "Etiqueta";
                if (!string.IsNullOrWhiteSpace(impresora)) pd.PrinterSettings.PrinterName = impresora;
                if (!string.IsNullOrEmpty(archivo)) { pd.PrinterSettings.PrintToFile = true; pd.PrinterSettings.PrintFileName = archivo; }
                pd.PrintController = new StandardPrintController(); // sin ventana de "Imprimiendo..."
                // Drivers Zebra ignoran PrinterSettings.Copies y a veces truncan trabajos multi-pagina.
                // Hacemos N trabajos separados, uno por copia.
                pd.PrinterSettings.Copies = 1;
                pd.PrinterSettings.Collate = true;
                // Las etiquetas horizontales (6x4) se giran aqui y se mandan en papel vertical (4x6):
                // el driver de la Zebra esta fijo en vertical y no se le toca la configuracion
                bool horizontal = anchoIn > altoIn;
                float papelAncho = horizontal ? altoIn : anchoIn, papelAlto = horizontal ? anchoIn : altoIn;
                pd.DefaultPageSettings.Landscape = false;
                pd.DefaultPageSettings.PaperSize = new PaperSize("Etiqueta", (int)Math.Round(papelAncho * 100), (int)Math.Round(papelAlto * 100));
                pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                pd.OriginAtMargins = false;
                int i = 0;
                pd.PrintPage += (s, e) =>
                {
                    // el EMF ya trae el tamano fisico; se dibuja desde la esquina del papel
                    var g = e.Graphics;
                    g.PageUnit = GraphicsUnit.Display;
                    float hx = e.PageSettings.HardMarginX, hy = e.PageSettings.HardMarginY;
                    g.TranslateTransform(-hx, -hy);
                    if (horizontal)
                    {
                        // 90 grados a la izquierda: la parte de arriba de la etiqueta queda en el borde izquierdo
                        g.TranslateTransform(0, papelAlto * 100f);
                        g.RotateTransform(-90);
                    }
                    g.DrawImage(metas[i], new RectangleF(0, 0, anchoIn * 100f, altoIn * 100f));
                    if (global::Inventario.ModoPrueba.Activo)
                    {
                        // Franja negra arriba con "PRUEBA - NO USAR" para que la etiqueta no se use en una caja real
                        var franja = new RectangleF(0, 0, anchoIn * 100f, 30f);
                        g.FillRectangle(Brushes.Black, franja);
                        using (var letra = new Font("Arial", 13f, FontStyle.Bold, GraphicsUnit.Point))
                        using (var centro = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                            g.DrawString(global::Inventario.ModoPrueba.TextoEtiqueta, letra, Brushes.White, franja, centro);
                    }
                    i++;
                    e.HasMorePages = i < metas.Count;
                    if (!e.HasMorePages) i = 0;
                };
                int totalCopias = Math.Max(1, copias);
                for (int c = 0; c < totalCopias; c++)
                    pd.Print();
            }
            foreach (var m in metas) m.Dispose();
            foreach (var p in paginas) p.Dispose();
        }

        // ---------------- datos ----------------
        public static DataTable ATabla(IEnumerable datos)
        {
            Type t = null;
            foreach (object o in datos) { if (o != null) { t = o.GetType(); break; } }
            if (t == null)
            {
                Type lista = datos.GetType();
                t = lista.IsArray ? lista.GetElementType() : lista.GetGenericArguments().FirstOrDefault() ?? typeof(object);
            }
            var dt = new DataTable("Datos");
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0)
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToList();
            foreach (var p in props)
            {
                Type pt = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (typeof(Image).IsAssignableFrom(pt) || pt == typeof(byte[])) pt = typeof(byte[]);
                else if (!(pt.IsPrimitive || pt == typeof(string) || pt == typeof(decimal) || pt == typeof(DateTime))) pt = typeof(string);
                dt.Columns.Add(p.Name, pt);
            }
            foreach (object o in datos)
            {
                DataRow r = dt.NewRow();
                foreach (var p in props)
                {
                    object v = p.GetValue(o, null);
                    if (v is Image) v = Png((Image)v);
                    else if (v != null && dt.Columns[p.Name].DataType == typeof(string) && !(v is string)) v = v.ToString();
                    // DevExpress tomaba un "\r" suelto como salto de renglon y RDLC no (p.ej. la lista
                    // SIZE/QTY de la caja mixta IFR salia pegada en un renglon); se normaliza a "\r\n"
                    if (v is string && ((string)v).IndexOf('\r') >= 0)
                        v = ((string)v).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
                    r[p.Name] = v ?? DBNull.Value;
                }
                dt.Rows.Add(r);
            }
            return dt;
        }

        private static byte[] Png(Image img)
        {
            using (var ms = new MemoryStream()) { img.Save(ms, ImageFormat.Png); return ms.ToArray(); }
        }

        /// <summary>
        /// DevExpress centraba las imagenes (Zoom/Center) y RDLC no. Cada imagen se rellena al tamano de su caja,
        /// centrada, para que RDLC la dibuje igual. En modo "centrar" no se escala (codigos de barras 1:1).
        /// </summary>
        private static void AjustarImagenes(XDocument def, DataTable tabla)
        {
            foreach (XElement img in def.Descendants(R + "Image"))
            {
                XElement cp = img.Descendants(R + "CustomProperty").FirstOrDefault(c => (string)c.Element(R + "Name") == "AjusteImagen");
                if (cp == null) continue;
                string valor = (string)img.Element(R + "Value") ?? "";
                var m = System.Text.RegularExpressions.Regex.Match(valor, @"Fields!(\w+)\.Value");
                if (!m.Success || !tabla.Columns.Contains(m.Groups[1].Value)) continue;
                string[] partes = ((string)cp.Element(R + "Value")).Split(';');
                string modo = partes[0];
                float wIn = float.Parse(partes[1], CultureInfo.InvariantCulture), hIn = float.Parse(partes[2], CultureInfo.InvariantCulture);
                string col = m.Groups[1].Value;
                foreach (DataRow r in tabla.Rows)
                {
                    byte[] b = r[col] as byte[];
                    if (b == null || b.Length == 0) continue;
                    r[col] = Rellenar(b, modo, wIn, hIn);
                }
            }
        }

        private static byte[] Rellenar(byte[] png, string modo, float wIn, float hIn)
        {
            using (var ms = new MemoryStream(png))
            using (var src = new Bitmap(ms))
            {
                float dpiX = src.HorizontalResolution > 1 ? src.HorizontalResolution : 96f;
                float dpiY = src.VerticalResolution > 1 ? src.VerticalResolution : 96f;
                int lienzoW, lienzoH;
                if (modo == "squeeze")
                    modo = (src.Width / dpiX <= wIn + 0.001f && src.Height / dpiY <= hIn + 0.001f) ? "centrar" : "zoom";
                if (modo == "centrar")
                {
                    // tamano fisico real de la imagen, centrada en la caja; se recorta si no cabe (igual que DevExpress)
                    lienzoW = Math.Max(1, (int)Math.Round(wIn * dpiX));
                    lienzoH = Math.Max(1, (int)Math.Round(hIn * dpiY));
                }
                else
                {
                    // zoom: lienzo con la misma proporcion que la caja (en pixeles de la imagen, sin perder nitidez);
                    // RDLC lo estira parejo y la imagen queda centrada como en DevExpress
                    if ((float)src.Width / src.Height > wIn / hIn)
                    {
                        lienzoW = src.Width;
                        lienzoH = Math.Max(src.Height, (int)Math.Round(src.Width * hIn / wIn));
                    }
                    else
                    {
                        lienzoH = src.Height;
                        lienzoW = Math.Max(src.Width, (int)Math.Round(src.Height * wIn / hIn));
                    }
                }
                using (var dst = new Bitmap(lienzoW, lienzoH))
                {
                    dst.SetResolution(dpiX, dpiY);
                    using (Graphics g = Graphics.FromImage(dst))
                    {
                        g.Clear(Color.Transparent);
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                        int x = (lienzoW - src.Width) / 2, y = (lienzoH - src.Height) / 2;
                        g.DrawImage(src, new Rectangle(x, y, src.Width, src.Height), new Rectangle(0, 0, src.Width, src.Height), GraphicsUnit.Pixel);
                    }
                    using (var outMs = new MemoryStream()) { dst.Save(outMs, ImageFormat.Png); return outMs.ToArray(); }
                }
            }
        }
    }
}
