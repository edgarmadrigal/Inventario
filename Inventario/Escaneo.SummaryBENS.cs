using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace Inventario
{
    // Summary de Ben Davis por PO (lo pide Ben Davis con cada PO embarcada), boton en la pestana Reporte PO.
    //  - Arriba: por estilo, rangos de cartones CONSECUTIVOS con el mismo contenido, piezas por talla,
    //    piezas por carton, total y peso bruto / neto (kg por prenda, embarques da 0.75 y 0.71).
    //  - Abajo: ORDER QTY (se captura al generar, la PO de Ben Davis no lo guarda) contra SHIPPED.
    public partial class Escaneo
    {
        private decimal pesoBrutoBENS = 0.75m, pesoNetoBENS = 0.71m;
        // ORDER QTY capturado en esta sesion, por PO y talla, para no volver a escribirlo
        private readonly Dictionary<string, Dictionary<string, int>> pedidoBENS = new Dictionary<string, Dictionary<string, int>>();

        private class RangoCartones
        {
            public string Estilo, Color;
            public int Desde, Hasta, Cajas;
            public Dictionary<string, int> PorTalla;   // piezas por carton de cada talla
            public int PorCarton { get { return PorTalla.Values.Sum(); } }
            public int Total { get { return PorCarton * Cajas; } }
        }

        private void ExportarSummaryBENS()
        {
            string po = cmbPOExcelBENS.Text.Trim();
            if (po == "")
            {
                MessageBox.Show("Escribe o selecciona la PO de Ben Davis.", "Summary Ben Davis");
                return;
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                List<Funciones.PackingBENS> filas = f.ReportePackingBENS(po);
                List<Funciones.ProductoCorreccion> catalogo = f.ConsultaProductosCorreccion(po);
                Cursor.Current = Cursors.Default;
                if (filas.Count == 0)
                {
                    MessageBox.Show("La PO " + po + " no tiene cajas escaneadas de Ben Davis.", "Summary Ben Davis");
                    return;
                }
                string poAI = filas[0].PO;

                // Tallas: las del catalogo de la PO mas las escaneadas, en orden de talla
                List<string> tallas = OrdenarTallas(catalogo.Select(c => NormalizarTalla(c.talla)).Concat(filas.Select(r => NormalizarTalla(r.Size))));

                Dictionary<string, int> pedido;
                if (!pedidoBENS.TryGetValue(poAI, out pedido)) pedido = new Dictionary<string, int>();
                CargarSugerenciasBENS(poAI, pedido);
                if (!CapturarPedidoBENS(poAI, tallas, filas, pedido)) return;
                pedidoBENS[poAI] = pedido;
                GuardarSugerenciasBENS(poAI, tallas, pedido);

                List<RangoCartones> rangos = ArmarRangos(filas);

                using (var dlg = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = poAI + " Summary Ben Davis " + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    GuardarSummaryBENS(poAI, tallas, rangos, pedido, dlg.FileName);
                    if (MessageBox.Show("Se guardó el Summary de la PO " + poAI + ".\r\n\r\n¿Abrirlo?", "Summary Ben Davis", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        // Lo que se capturo la vez pasada para esta PO (de la base, sirve en cualquier equipo).
        // Si la base aun no tiene las tablas de sugerencias, se queda con lo de esta sesion y los pesos de siempre.
        private void CargarSugerenciasBENS(string poAI, Dictionary<string, int> pedido)
        {
            try
            {
                foreach (var s in f.ConsultaSugerenciaPedidoBENS(poAI))
                    pedido[NormalizarTalla(s.talla)] = s.pedido;
                var peso = f.ConsultaSugerenciaPesoBENS(poAI);
                if (peso != null)
                {
                    pesoBrutoBENS = peso.pesoBruto;
                    pesoNetoBENS = peso.pesoNeto;
                }
            }
            catch { }
        }

        private void GuardarSugerenciasBENS(string poAI, List<string> tallas, Dictionary<string, int> pedido)
        {
            try
            {
                foreach (string t in tallas)
                {
                    int p;
                    f.GuardarSugerenciaPedidoBENS(poAI, t, pedido.TryGetValue(t, out p) ? (int?)p : null, usu[0].id);
                }
                f.GuardarSugerenciaPesoBENS(poAI, pesoBrutoBENS, pesoNetoBENS, usu[0].id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("El Summary se va a generar, pero no se pudo guardar lo capturado como sugerencia: " + ex.Message, "Summary Ben Davis");
            }
        }

        private static string NormalizarTalla(string talla)
        {
            return string.Join(" x ", (talla ?? "").Split(new[] { 'x', 'X' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).Where(p => p != ""));
        }

        // Pantalones (cintura x largo) por numero; letras (S, M, L...) en su orden; lo demas alfabetico
        private static List<string> OrdenarTallas(IEnumerable<string> tallas)
        {
            string[] letras = { "XXS", "XS", "S", "M", "L", "XL", "2XL", "XXL", "3XL", "XXXL", "4XL", "5XL" };
            Func<string, int> numero = s => { int n; return int.TryParse(s, out n) ? n : int.MaxValue; };
            return tallas.Where(t => t != "").Distinct()
                .OrderBy(t => Array.IndexOf(letras, t.ToUpper()) >= 0 ? Array.IndexOf(letras, t.ToUpper()) : 100)
                .ThenBy(t => numero(t.Split(new[] { " x " }, StringSplitOptions.None)[0]))
                .ThenBy(t => t.Contains(" x ") ? numero(t.Split(new[] { " x " }, StringSplitOptions.None)[1]) : 0)
                .ThenBy(t => t)
                .ToList();
        }

        // Cartones en orden; se juntan los consecutivos del mismo estilo/color con el mismo contenido
        private static List<RangoCartones> ArmarRangos(List<Funciones.PackingBENS> filas)
        {
            var cajas = filas.GroupBy(r => r.idCaja)
                .Select(g => new
                {
                    Box = g.First().Box,
                    Estilo = g.First().Style,
                    Color = (g.First().Color ?? "").Trim(),
                    PorTalla = g.GroupBy(r => NormalizarTalla(r.Size)).ToDictionary(t => t.Key, t => t.Sum(r => r.Qty))
                })
                .OrderBy(c => c.Estilo).ThenBy(c => c.Box)
                .ToList();

            var rangos = new List<RangoCartones>();
            foreach (var c in cajas)
            {
                RangoCartones ultimo = rangos.LastOrDefault();
                bool igual = ultimo != null && ultimo.Estilo == c.Estilo && ultimo.Color == c.Color && ultimo.Hasta + 1 == c.Box
                             && ultimo.PorTalla.Count == c.PorTalla.Count
                             && ultimo.PorTalla.All(kv => c.PorTalla.ContainsKey(kv.Key) && c.PorTalla[kv.Key] == kv.Value);
                if (igual)
                {
                    ultimo.Hasta = c.Box;
                    ultimo.Cajas++;
                }
                else
                {
                    rangos.Add(new RangoCartones { Estilo = c.Estilo, Color = c.Color, Desde = c.Box, Hasta = c.Box, Cajas = 1, PorTalla = c.PorTalla });
                }
            }
            return rangos;
        }

        // Ventana para escribir lo pedido por talla y el peso por prenda
        private bool CapturarPedidoBENS(string poAI, List<string> tallas, List<Funciones.PackingBENS> filas, Dictionary<string, int> pedido)
        {
            using (var frm = new Form
            {
                Text = "Summary Ben Davis - " + poAI,
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(470, 560),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Font = new Font("Microsoft Sans Serif", 10F)
            })
            {
                var lbl = new Label
                {
                    Text = "Escribe lo pedido (ORDER QTY) de cada talla. Si no lo tienes, déjalo vacío y el renglón sale en blanco.",
                    Location = new Point(12, 10),
                    Size = new Size(430, 40)
                };
                var grid = new DataGridView
                {
                    Location = new Point(12, 55),
                    Size = new Size(430, 330),
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    RowHeadersVisible = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.CellSelect
                };
                grid.Columns.Add("talla", "Talla");
                grid.Columns.Add("enviado", "Enviado");
                grid.Columns.Add("pedido", "Pedido (ORDER QTY)");
                grid.Columns["talla"].ReadOnly = true;
                grid.Columns["enviado"].ReadOnly = true;
                grid.Columns["talla"].DefaultCellStyle.BackColor = grid.Columns["enviado"].DefaultCellStyle.BackColor = Color.FromArgb(235, 238, 242);
                foreach (string t in tallas)
                {
                    int enviado = filas.Where(r => NormalizarTalla(r.Size) == t).Sum(r => r.Qty);
                    int p;
                    grid.Rows.Add(t, enviado, pedido.TryGetValue(t, out p) ? (object)p : null);
                }

                var lblBruto = new Label { Text = "Peso bruto (kg por prenda):", Location = new Point(12, 400), AutoSize = true };
                var txtBruto = new TextBox { Text = pesoBrutoBENS.ToString(CultureInfo.InvariantCulture), Location = new Point(250, 397), Width = 80 };
                var lblNeto = new Label { Text = "Peso neto (kg por prenda):", Location = new Point(12, 432), AutoSize = true };
                var txtNeto = new TextBox { Text = pesoNetoBENS.ToString(CultureInfo.InvariantCulture), Location = new Point(250, 429), Width = 80 };
                var btnOk = new Button { Text = "GENERAR", Location = new Point(222, 470), Size = new Size(110, 36), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "Cancelar", Location = new Point(338, 470), Size = new Size(104, 36), DialogResult = DialogResult.Cancel };
                frm.Controls.AddRange(new Control[] { lbl, grid, lblBruto, txtBruto, lblNeto, txtNeto, btnOk, btnCancel });
                frm.AcceptButton = btnOk;
                frm.CancelButton = btnCancel;
                Tema.Aplicar(frm);

                while (true)
                {
                    if (frm.ShowDialog(this) != DialogResult.OK) return false;
                    grid.EndEdit();

                    decimal bruto, neto;
                    if (!LeerDecimal(txtBruto.Text, out bruto) || !LeerDecimal(txtNeto.Text, out neto))
                    {
                        MessageBox.Show("Revisa los pesos: deben ser números, por ejemplo 0.75", "Summary Ben Davis");
                        continue;
                    }

                    var nuevo = new Dictionary<string, int>();
                    string error = null;
                    foreach (DataGridViewRow r in grid.Rows)
                    {
                        string texto = Convert.ToString(r.Cells["pedido"].Value).Trim();
                        if (texto == "") continue;
                        int n;
                        if (!int.TryParse(texto, out n) || n < 0) { error = "El pedido de la talla " + r.Cells["talla"].Value + " no es un número válido."; break; }
                        nuevo[r.Cells["talla"].Value.ToString()] = n;
                    }
                    if (error != null)
                    {
                        MessageBox.Show(error, "Summary Ben Davis");
                        continue;
                    }

                    pesoBrutoBENS = bruto;
                    pesoNetoBENS = neto;
                    pedido.Clear();
                    foreach (var kv in nuevo) pedido[kv.Key] = kv.Value;
                    return true;
                }
            }
        }

        private static bool LeerDecimal(string texto, out decimal valor)
        {
            return decimal.TryParse((texto ?? "").Trim().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out valor) && valor >= 0;
        }

        private void GuardarSummaryBENS(string poAI, List<string> tallas, List<RangoCartones> rangos, Dictionary<string, int> pedido, string ruta)
        {
            var gris = XLColor.FromArgb(217, 217, 217);
            int nT = tallas.Count;
            // Columnas: A estilo, B-C carton desde/hasta, D ctns, E color, tallas, por carton, total pcs, G.W., N.W.
            int cTalla1 = 6, cPorCarton = cTalla1 + nT, cTotal = cPorCarton + 1, cGW = cTotal + 1, cNW = cGW + 1;

            using (var libro = new XLWorkbook())
            {
                var hoja = libro.Worksheets.Add("Summary " + (poAI.Length > 20 ? poAI.Substring(0, 20) : poAI));
                hoja.Style.Font.FontName = "Calibri";
                hoja.Style.Font.FontSize = 10;

                hoja.Cell(1, 1).Value = "SUMMARY PACKING LIST  -  PO " + poAI;
                hoja.Cell(1, 1).Style.Font.Bold = true;
                hoja.Cell(1, 1).Style.Font.FontSize = 13;
                hoja.Cell(2, 1).Value = "Fecha: " + DateTime.Now.ToString("dd/MM/yyyy");

                // ---------- Encabezado (2 renglones) ----------
                int e1 = 4, e2 = 5;
                Action<int, int, int, int, string> titulo = (f1, c1, f2, c2, texto) =>
                {
                    var r = hoja.Range(f1, c1, f2, c2);
                    r.Merge();
                    r.Value = texto;
                };
                titulo(e1, 1, e2, 1, "STYLE NO.");
                titulo(e1, 2, e2, 3, "CTN NO.");
                titulo(e1, 4, e2, 4, "CTNS");
                titulo(e1, 5, e2, 5, "COLOR");
                titulo(e1, cTalla1, e1, cPorCarton - 1, "SIZE");
                for (int i = 0; i < nT; i++) hoja.Cell(e2, cTalla1 + i).Value = tallas[i];
                hoja.Cell(e1, cPorCarton).Value = "QTY PCS"; hoja.Cell(e2, cPorCarton).Value = "PER CTN";
                hoja.Cell(e1, cTotal).Value = "TOTAL PCS"; hoja.Cell(e2, cTotal).Value = "QTY";
                hoja.Cell(e1, cGW).Value = "TOTAL G.W."; hoja.Cell(e2, cGW).Value = "KGS";
                hoja.Cell(e1, cNW).Value = "TOTAL N.W."; hoja.Cell(e2, cNW).Value = "KGS";
                var enc = hoja.Range(e1, 1, e2, cNW);
                enc.Style.Font.Bold = true;
                enc.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                enc.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                enc.Style.Alignment.WrapText = true;
                hoja.Range(e2, cTalla1, e2, cPorCarton - 1).Style.Fill.BackgroundColor = gris;

                // ---------- Rangos por estilo ----------
                int fila = e2 + 1;
                foreach (var estilo in rangos.GroupBy(r => r.Estilo))
                {
                    int inicio = fila;
                    foreach (var r in estilo)
                    {
                        hoja.Cell(fila, 2).Value = r.Desde;
                        hoja.Cell(fila, 3).Value = r.Hasta;
                        hoja.Cell(fila, 4).Value = r.Cajas;
                        for (int i = 0; i < nT; i++)
                        {
                            int q;
                            if (r.PorTalla.TryGetValue(tallas[i], out q) && q > 0) hoja.Cell(fila, cTalla1 + i).Value = q;
                        }
                        hoja.Cell(fila, cPorCarton).Value = r.PorCarton;
                        hoja.Cell(fila, cTotal).Value = r.Total;
                        fila++;
                    }
                    int fin = fila - 1;
                    int piezasEstilo = estilo.Sum(r => r.Total);
                    // Estilo, color y pesos van una sola vez por estilo (celdas combinadas)
                    PonerCombinado(hoja.Range(inicio, 1, fin, 1), PonerNumeroOTextoValor(estilo.Key), true);
                    PonerCombinado(hoja.Range(inicio, 5, fin, 5), string.Join(" / ", estilo.Select(r => r.Color).Where(c => c != "").Distinct()), true);
                    PonerCombinado(hoja.Range(inicio, cGW, fin, cGW), Math.Round(piezasEstilo * pesoBrutoBENS, 2), false);
                    PonerCombinado(hoja.Range(inicio, cNW, fin, cNW), Math.Round(piezasEstilo * pesoNetoBENS, 2), false);
                }

                // ---------- TOTAL ----------
                int filaTotal = fila;
                hoja.Cell(filaTotal, 3).Value = "TOTAL";
                hoja.Cell(filaTotal, 4).Value = rangos.Sum(r => r.Cajas);
                for (int i = 0; i < nT; i++)
                {
                    string t = tallas[i];
                    hoja.Cell(filaTotal, cTalla1 + i).Value = rangos.Sum(r => r.PorTalla.ContainsKey(t) ? r.PorTalla[t] * r.Cajas : 0);
                }
                int totalPiezas = rangos.Sum(r => r.Total);
                hoja.Cell(filaTotal, cTotal).Value = totalPiezas;
                hoja.Cell(filaTotal, cGW).Value = Math.Round(totalPiezas * pesoBrutoBENS, 2);
                hoja.Cell(filaTotal, cNW).Value = Math.Round(totalPiezas * pesoNetoBENS, 2);
                hoja.Range(filaTotal, 1, filaTotal, cNW).Style.Font.Bold = true;
                hoja.Range(filaTotal, 1, filaTotal, cNW).Style.Border.TopBorder = XLBorderStyleValues.Medium;

                var tabla = hoja.Range(e1, 1, filaTotal, cNW);
                tabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                hoja.Range(e2 + 1, 2, filaTotal, cNW).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                hoja.Range(e2 + 1, cGW, filaTotal, cNW).Style.NumberFormat.Format = "#,##0.##";

                // ---------- ORDER QTY vs SHIPPED ----------
                int q1 = filaTotal + 3, q2 = q1 + 1, qOrder = q1 + 2, qShip = q1 + 3;
                titulo(q1, 4, q2, 5, "QUANTITY");
                titulo(q1, cTalla1, q1, cPorCarton - 1, "SIZE");
                for (int i = 0; i < nT; i++) hoja.Cell(q2, cTalla1 + i).Value = tallas[i];
                hoja.Cell(q1, cTotal).Value = "TOTAL";
                hoja.Cell(q2, cTotal).Value = "PCS";
                hoja.Range(qOrder, 4, qOrder, 5).Merge().Value = "ORDER QTY";
                hoja.Range(qShip, 4, qShip, 5).Merge().Value = "SHIPPED";
                for (int i = 0; i < nT; i++)
                {
                    int p;
                    if (pedido.TryGetValue(tallas[i], out p)) hoja.Cell(qOrder, cTalla1 + i).Value = p;
                    hoja.Cell(qShip, cTalla1 + i).Value = hoja.Cell(filaTotal, cTalla1 + i).Value;
                }
                if (pedido.Count > 0) hoja.Cell(qOrder, cTotal).Value = pedido.Where(kv => tallas.Contains(kv.Key)).Sum(kv => kv.Value);
                hoja.Cell(qShip, cTotal).Value = totalPiezas;

                var cuadro = hoja.Range(q1, 4, qShip, cTotal);
                cuadro.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cuadro.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                cuadro.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cuadro.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                hoja.Range(q1, 4, q2, cTotal).Style.Font.Bold = true;
                hoja.Range(q2, cTalla1, q2, cPorCarton - 1).Style.Font.Underline = XLFontUnderlineValues.Single;
                hoja.Range(qShip, 4, qShip, cTotal).Style.Font.Bold = true;

                // ---------- SUMMARY por inseam (pantalones: renglon = largo, columna = cintura) ----------
                int ultimaFila = qShip;
                if (tallas.All(t => t.Contains(" x ")))
                    ultimaFila = SummaryPorInseam(hoja, rangos, qShip + 3);

                // ---------- Anchos e impresion ----------
                hoja.Column(1).Width = 11;
                hoja.Columns(2, 4).Width = 7;
                hoja.Column(5).Width = 14;
                hoja.Columns(cTalla1, cPorCarton - 1).Width = 7.5;
                hoja.Columns(cPorCarton, cNW).Width = 9;
                hoja.Row(e1).Height = 28;
                hoja.PageSetup.PageOrientation = XLPageOrientation.Landscape;
                hoja.PageSetup.FitToPages(1, 0);
                hoja.PageSetup.SetRowsToRepeatAtTop(e1, e2);

                libro.SaveAs(ruta);
            }
        }

        // Como el SUMMARY que manda Ben Davis: por color, un renglon por INSEAM (largo) y una columna por cintura,
        // con el total de piezas embarcadas, total por renglon y total general. Regresa la ultima fila usada.
        private static int SummaryPorInseam(IXLWorksheet hoja, List<RangoCartones> rangos, int fila)
        {
            Func<string, int> numero = s => { int n; return int.TryParse(s, out n) ? n : int.MaxValue; };
            var piezas = rangos.SelectMany(r => r.PorTalla.Select(kv => new
            {
                Color = r.Color,
                Cintura = kv.Key.Split(new[] { " x " }, StringSplitOptions.None)[0],
                Largo = kv.Key.Split(new[] { " x " }, StringSplitOptions.None)[1],
                Pzas = kv.Value * r.Cajas
            })).ToList();
            List<string> cinturas = piezas.Select(p => p.Cintura).Distinct().OrderBy(numero).ThenBy(c => c).ToList();
            List<string> largos = piezas.Select(p => p.Largo).Distinct().OrderBy(numero).ThenBy(l => l).ToList();
            int cInseam = 2, cCintura1 = 3, cTotal = cCintura1 + cinturas.Count;

            hoja.Cell(fila, 1).Value = "SUMMARY";
            hoja.Cell(fila, 1).Style.Font.Bold = true;
            hoja.Cell(fila, 1).Style.Font.Underline = XLFontUnderlineValues.Single;
            int fEnc = fila + 2;
            hoja.Cell(fEnc - 1, cCintura1).Value = "SIZES";
            hoja.Cell(fEnc, 1).Value = "COLOURS";
            hoja.Cell(fEnc, cInseam).Value = "INSEAM";
            for (int i = 0; i < cinturas.Count; i++) hoja.Cell(fEnc, cCintura1 + i).Value = PonerNumeroOTextoValor(cinturas[i]);
            hoja.Cell(fEnc, cTotal).Value = "TOTAL";
            hoja.Range(fEnc - 1, 1, fEnc, cTotal).Style.Font.Bold = true;

            int f = fEnc + 1;
            foreach (var color in piezas.GroupBy(p => p.Color))
            {
                int inicio = f;
                foreach (string largo in largos)
                {
                    var delLargo = color.Where(p => p.Largo == largo).ToList();
                    if (delLargo.Count == 0) continue;
                    hoja.Cell(f, cInseam).Value = largo + "\"";
                    for (int i = 0; i < cinturas.Count; i++)
                    {
                        int q = delLargo.Where(p => p.Cintura == cinturas[i]).Sum(p => p.Pzas);
                        if (q > 0) hoja.Cell(f, cCintura1 + i).Value = q;
                    }
                    hoja.Cell(f, cTotal).Value = delLargo.Sum(p => p.Pzas);
                    f++;
                }
                PonerCombinado(hoja.Range(inicio, 1, f - 1, 1), color.Key == "" ? "-" : color.Key, true);
            }
            hoja.Cell(f, cTotal - 1).Value = "TOTAL";
            hoja.Cell(f, cTotal).Value = piezas.Sum(p => p.Pzas);
            hoja.Range(f, cTotal - 1, f, cTotal).Style.Font.Bold = true;

            var tabla = hoja.Range(fEnc, 1, f, cTotal);
            tabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            hoja.Range(fEnc, cInseam, f, cTotal).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            return f;
        }

        private static object PonerNumeroOTextoValor(string valor)
        {
            int n;
            return int.TryParse(valor, out n) ? (object)n : valor;
        }

        private static void PonerCombinado(IXLRange rango, object valor, bool negritas)
        {
            rango.Merge();
            rango.FirstCell().Value = valor;
            rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rango.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rango.Style.Alignment.WrapText = true;
            if (negritas) rango.Style.Font.Bold = true;
        }
    }
}
