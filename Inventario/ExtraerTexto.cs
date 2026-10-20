using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Inventario
{
    public class ExtraerTexto
    {
        public string ExtraerTextoPDF(string rutaPdf)
        {
            StringBuilder textoCompleto = new StringBuilder();
            using (PdfReader reader = new PdfReader(rutaPdf))
            {
                for (int pagina = 1; pagina <= reader.NumberOfPages; pagina++)
                {
                    var strategy = new LocationTextExtractionStrategy();
                    string textoPagina = PdfTextExtractor.GetTextFromPage(reader, pagina, strategy);
                    textoCompleto.AppendLine(textoPagina);
                }
            }
            return textoCompleto.ToString();
        }
        public List<FilaPO> ExtraerFilasDetallePDF(string rutaPdf)
        {
            var filas = new List<FilaPO>();
            string texto = ExtraerTextoPDF(rutaPdf);
            string[] lineas = texto.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Encabezado: "NOMBRE [fecha opcional] / Indigo [/ CAVENDERS] [/ variante]"
            var regexEncabezado = new Regex(
                @"^(?<nombre>.+?)\s*/\s*Indigo\b(?:\s*/\s*(?<cavenders>CAVENDERS))?(?:\s*/\s*(?<variante>.+))?\s*$",
                RegexOptions.IgnoreCase);

            var regexDetalle = new Regex(
                @"^(\d{5,6})\s+(\S+)\s+(\S+)\s+(\S+)\s+(?:(\S+)\s+)?(\S+)\s+(\d{12})\s+(\d+)\s*$");

            string descripcionActual = "";
            string descripcion2Actual = "";

            foreach (var lineaRaw in lineas)
            {
                string linea = lineaRaw.Trim();
                if (string.IsNullOrWhiteSpace(linea)) continue;

                var matchEncabezado = regexEncabezado.Match(linea);
                if (matchEncabezado.Success)
                {
                    // Quita la fecha tipo "11/26" o "12/26" del nombre, si viene pegada
                    string nombre = matchEncabezado.Groups["nombre"].Value.Trim();
                    nombre = Regex.Replace(nombre, @"\s+\d{1,2}/\d{2}\s*$", "").Trim();

                    descripcionActual = nombre;
                    descripcion2Actual = matchEncabezado.Groups["cavenders"].Success ? "CAVENDERS" : "";
                    continue;
                }

                var matchDetalle = regexDetalle.Match(linea);
                if (matchDetalle.Success)
                {
                    filas.Add(new FilaPO
                    {
                        PoNumero = matchDetalle.Groups[1].Value,
                        Estilo = matchDetalle.Groups[2].Value,
                        Color = matchDetalle.Groups[3].Value,
                        ColorDescripcion = matchDetalle.Groups[4].Value,
                        Dimension = matchDetalle.Groups[5].Value,
                        Size = matchDetalle.Groups[6].Value,
                        Upc = matchDetalle.Groups[7].Value,
                        Cantidad = Convert.ToInt32(matchDetalle.Groups[8].Value),
                        DescripcionEstilo = descripcionActual,
                        Descripcion2 = descripcion2Actual
                    });
                }
            }
            return filas;
        }




    }
}
