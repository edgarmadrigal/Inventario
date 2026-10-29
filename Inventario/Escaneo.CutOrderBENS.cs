using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace Inventario
{
    // Captura del Cut Order de Ben Davis. El cliente lo manda escrito a mano (foto) y aqui se pasa al sistema:
    //  - La cuadricula tiene el mismo acomodo que el formulario del cliente, con la foto al lado.
    //  - Se puede llenar importando el Excel "PURCHASE ORDER" que ya se arma de la foto.
    //  - TOTAL UNITS se escribe de la foto y la suma tiene que cuadrar para poder guardar.
    //  - Al guardar queda como ORDER QTY de la PO (lo usa el Summary) y en el historial CutOrderBENS con la foto.
    public partial class Escaneo
    {
        private class BloqueCutOrder
        {
            public string Largo;       // "29", "30"... o "" para tallas que no estan en el formulario
            public string Titulo;
            public List<string> Tallas; // "32 x 29"
        }

        // Mismo orden que el formulario BEN DAVIS CUT ORDER (cintura x largo), en sus tres columnas
        private static readonly int[][] cinturasCutOrder =
        {
            new[] { 32, 33, 34, 36, 38, 40, 42, 44 },                                          // largo 29
            new[] { 29, 30, 31, 32, 33, 34, 36, 38, 40, 42, 44, 46, 48, 50 },                  // largo 30
            new[] { 29, 30, 31, 32, 33, 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60 }, // largo 32
            new[] { 32, 33, 34, 36, 38, 40, 42 },                                              // largo 34
            new[] { 34, 36, 38 }                                                               // largo 36
        };
        private static readonly int[] largosCutOrder = { 29, 30, 32, 34, 36 };

        // Paso previo del Summary: captura / revision del Cut Order (foto + cantidades).
        // Regresa false si se cancela; sinPedido = true si eligen seguir sin ORDER QTY.
        private bool PedirCutOrderBENS(string poAI, out bool sinPedido)
        {
            using (var frm = new CutOrderBENSForm(this, poAI, true))
            {
                bool ok = frm.ShowDialog(this) == DialogResult.OK;
                sinPedido = frm.SinPedido;
                return ok;
            }
        }

        private static string PoConAI(string po)
        {
            po = (po ?? "").Trim().ToUpper();
            return po == "" || po.StartsWith("AI") ? po : "AI" + po;
        }

        // ---------------- Ventana ----------------
        private class CutOrderBENSForm : Form
        {
            private readonly Escaneo principal;
            private readonly Funciones f;
            private readonly TextBox txtPO = new TextBox(), txtEstilo = new TextBox(), txtTotal = new TextBox();
            private readonly PictureBox foto = new PictureBox();
            private readonly Label lblResultado = new Label(), lblAvisos = new Label(), lblFoto = new Label();
            private readonly Button btnGuardar = new Button();
            private readonly Panel panelCuadricula = new Panel();
            private readonly Dictionary<string, TextBox> cajas = new Dictionary<string, TextBox>();
            private readonly Dictionary<string, Label> subtotales = new Dictionary<string, Label>();
            private List<BloqueCutOrder> bloques = new List<BloqueCutOrder>();
            private List<Funciones.ProductoCorreccion> catalogo = new List<Funciones.ProductoCorreccion>();
            private byte[] fotoBytes;
            private string fotoNombre;
            private bool cargando;
            private bool cambios;          // hubo cambios desde que se cargo / guardo
            private readonly bool modoSummary;
            public bool SinPedido { get; private set; }

            public CutOrderBENSForm(Escaneo principal, string po, bool modoSummary)
            {
                this.modoSummary = modoSummary;
                this.principal = principal;
                f = principal.f;
                Text = Funciones.TituloConVersion("Cut Order Ben Davis");
                StartPosition = FormStartPosition.CenterParent;
                Size = new Size(1180, 820);
                MinimumSize = new Size(1000, 600);
                Font = new Font("Microsoft Sans Serif", 9F);
                KeyPreview = true;
                KeyDown += (s, e) => { if (e.Control && e.KeyCode == Keys.V && !(ActiveControl is TextBox)) { PegarFoto(); e.Handled = true; } };

                ArmarEncabezado();
                ArmarFoto();
                ArmarPie();
                panelCuadricula.Location = new Point(470, 92);
                panelCuadricula.Size = new Size(690, 590);
                panelCuadricula.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                panelCuadricula.AutoScroll = true;
                panelCuadricula.BorderStyle = BorderStyle.FixedSingle;
                Controls.Add(panelCuadricula);

                txtPO.Text = PoConAI(po);
                // Desde el Summary la PO ya viene escogida
                txtPO.ReadOnly = modoSummary;
                CargarPO();
                Tema.Aplicar(this);
            }

            private void ArmarEncabezado()
            {
                var titulo = new Label { Text = "BEN DAVIS CUT ORDER", Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Bold), AutoSize = true, Location = new Point(12, 8) };
                Controls.Add(titulo);

                Func<string, int, Label> etiqueta = (t, x) => new Label { Text = t, AutoSize = true, Location = new Point(x, 50), Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold) };
                Controls.Add(etiqueta("CUT # (PO):", 470));
                txtPO.Location = new Point(565, 47); txtPO.Width = 110; txtPO.Font = new Font("Microsoft Sans Serif", 11F);
                txtPO.Leave += (s, e) => { string n = PoConAI(txtPO.Text); if (n != txtPO.Text) txtPO.Text = n; CargarPO(); };
                txtPO.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { txtPO.Text = PoConAI(txtPO.Text); CargarPO(); } };
                Controls.Add(txtPO);

                Controls.Add(etiqueta("LOT # (estilo):", 690));
                txtEstilo.Location = new Point(805, 47); txtEstilo.Width = 80; txtEstilo.Font = new Font("Microsoft Sans Serif", 11F);
                txtEstilo.TextChanged += (s, e) => Recalcular(true);
                Controls.Add(txtEstilo);

                Controls.Add(etiqueta("TOTAL UNITS:", 900));
                txtTotal.Location = new Point(1005, 47); txtTotal.Width = 80; txtTotal.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
                txtTotal.TextChanged += (s, e) => Recalcular(true);
                Controls.Add(txtTotal);

                var ayuda = new Label
                {
                    Text = "Copia cada casilla de la foto (Ø = vacío) y escribe el TOTAL UNITS de la hoja: la suma debe cuadrar.",
                    Location = new Point(470, 74), Size = new Size(690, 18), ForeColor = Color.FromArgb(90, 90, 90)
                };
                Controls.Add(ayuda);
            }

            private void ArmarFoto()
            {
                var btnCargar = new Button { Text = "Cargar foto...", Location = new Point(12, 46), Size = new Size(105, 30) };
                btnCargar.Click += (s, e) => CargarFotoArchivo();
                var btnPegar = new Button { Text = "Pegar foto (Ctrl+V)", Location = new Point(122, 46), Size = new Size(135, 30) };
                btnPegar.Click += (s, e) => PegarFoto();
                var btnVer = new Button { Text = "Ver grande", Location = new Point(262, 46), Size = new Size(90, 30) };
                btnVer.Click += (s, e) => VerFotoGrande();
                Controls.AddRange(new Control[] { btnCargar, btnPegar, btnVer });

                foto.Location = new Point(12, 92);
                foto.Size = new Size(445, 590);
                foto.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
                foto.SizeMode = PictureBoxSizeMode.Zoom;
                foto.BorderStyle = BorderStyle.FixedSingle;
                foto.BackColor = Color.White;
                foto.Cursor = Cursors.Hand;
                foto.DoubleClick += (s, e) => VerFotoGrande();
                Controls.Add(foto);

                lblFoto.Location = new Point(12, 300);
                lblFoto.Size = new Size(445, 60);
                lblFoto.TextAlign = ContentAlignment.MiddleCenter;
                lblFoto.Text = "Sin foto.\r\nCarga la foto del cliente o pégala con Ctrl+V.";
                lblFoto.BackColor = Color.White;
                Controls.Add(lblFoto);
                lblFoto.BringToFront();
            }

            private void ArmarPie()
            {
                lblResultado.Location = new Point(470, 688);
                lblResultado.Size = new Size(690, 24);
                lblResultado.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                lblResultado.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
                lblAvisos.Location = new Point(470, 712);
                lblAvisos.Size = new Size(690, 34);
                lblAvisos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                lblAvisos.ForeColor = Color.FromArgb(170, 95, 0);

                var btnExcel = new Button { Text = "IMPORTAR EXCEL DE LA PO", Location = new Point(12, 700), Size = new Size(200, 40), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
                btnExcel.Click += (s, e) => ImportarExcel();
                var btnHoja = new Button { Text = "HOJA PARA EL CLIENTE", Location = new Point(220, 700), Size = new Size(170, 40), Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
                btnHoja.Click += (s, e) => HojaCliente();

                btnGuardar.Text = modoSummary ? "GUARDAR Y CONTINUAR" : "GUARDAR";
                btnGuardar.Location = modoSummary ? new Point(815, 748) : new Point(925, 748);
                btnGuardar.Size = modoSummary ? new Size(230, 34) : new Size(120, 34);
                btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                btnGuardar.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
                btnGuardar.Click += (s, e) => Guardar();
                var btnCerrar = new Button { Text = "Cerrar", Location = new Point(1050, 748), Size = new Size(100, 34), Anchor = AnchorStyles.Bottom | AnchorStyles.Right, DialogResult = DialogResult.Cancel };
                CancelButton = btnCerrar;
                Controls.AddRange(new Control[] { lblResultado, lblAvisos, btnExcel, btnHoja, btnGuardar, btnCerrar });
                if (modoSummary)
                {
                    // Por si todavia no tienen la hoja del cliente
                    var btnSin = new Button { Text = "Seguir sin ORDER QTY", Location = new Point(640, 748), Size = new Size(165, 34), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
                    btnSin.Click += (s, e) =>
                    {
                        if (MessageBox.Show("El Summary saldrá sin el renglón ORDER QTY (no se podrá comparar contra lo pedido).\r\n\r\n¿Continuar así?", "Cut Order", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                        SinPedido = true;
                        DialogResult = DialogResult.OK;
                    };
                    Controls.Add(btnSin);
                }
            }

            // ---------- Cuadricula ----------
            private void ArmarCuadricula()
            {
                panelCuadricula.SuspendLayout();
                panelCuadricula.Controls.Clear();
                cajas.Clear();
                subtotales.Clear();

                bloques = new List<BloqueCutOrder>();
                for (int i = 0; i < largosCutOrder.Length; i++)
                    bloques.Add(new BloqueCutOrder
                    {
                        Largo = largosCutOrder[i].ToString(),
                        Titulo = "LARGO " + largosCutOrder[i],
                        Tallas = cinturasCutOrder[i].Select(c => c + " x " + largosCutOrder[i]).ToList()
                    });
                // Tallas de la PO que no vienen en el formulario
                var enFormulario = new HashSet<string>(bloques.SelectMany(b => b.Tallas));
                var otras = catalogo.Select(c => NormalizarTalla(c.talla)).Where(t => t != "" && !enFormulario.Contains(t)).Distinct().ToList();
                if (otras.Count > 0)
                    bloques.Add(new BloqueCutOrder { Largo = "", Titulo = "OTRAS DE LA PO", Tallas = OrdenarTallas(otras) });

                // Tres columnas como el formulario: [29, 30], [32], [34, 36, otras]
                int[][] columnas = { new[] { 0, 1 }, new[] { 2 }, Enumerable.Range(3, bloques.Count - 3).ToArray() };
                for (int c = 0; c < columnas.Length; c++)
                {
                    int x = 8 + c * 220, y = 6;
                    foreach (int b in columnas[c])
                        y = DibujarBloque(bloques[b], x, y) + 12;
                }
                panelCuadricula.ResumeLayout();
            }

            private int DibujarBloque(BloqueCutOrder bloque, int x, int y)
            {
                var enc = new Label
                {
                    Text = bloque.Titulo,
                    Location = new Point(x, y), Size = new Size(190, 20),
                    BackColor = Color.FromArgb(44, 62, 80), ForeColor = Color.White,
                    TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold)
                };
                panelCuadricula.Controls.Add(enc);
                y += 22;
                foreach (string talla in bloque.Tallas)
                {
                    var lbl = new Label { Text = talla.Replace(" x ", "x"), Location = new Point(x, y + 3), Size = new Size(70, 18), Font = new Font("Microsoft Sans Serif", 10F) };
                    var txt = new TextBox { Location = new Point(x + 75, y), Width = 80, TextAlign = HorizontalAlignment.Right, Font = new Font("Microsoft Sans Serif", 10F), Tag = talla };
                    txt.TextChanged += (s, e) => Recalcular(true);
                    txt.Enter += (s, e) => txt.SelectAll();
                    txt.KeyDown += (s, e) =>
                    {
                        if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Down) { SelectNextControl(txt, true, true, true, true); e.SuppressKeyPress = true; }
                        else if (e.KeyCode == Keys.Up) { SelectNextControl(txt, false, true, true, true); e.SuppressKeyPress = true; }
                    };
                    panelCuadricula.Controls.Add(lbl);
                    panelCuadricula.Controls.Add(txt);
                    cajas[talla] = txt;
                    y += 24;
                }
                var sub = new Label { Location = new Point(x, y + 2), Size = new Size(190, 18), TextAlign = ContentAlignment.MiddleRight, Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold) };
                panelCuadricula.Controls.Add(sub);
                subtotales[bloque.Titulo] = sub;
                return y + 20;
            }

            // ---------- Datos ----------
            private string poCargada;

            private void CargarPO()
            {
                string po = PoConAI(txtPO.Text);
                if (po == poCargada) return;
                poCargada = po;
                cargando = true;
                try
                {
                    catalogo = po == "" ? new List<Funciones.ProductoCorreccion>() : f.ConsultaProductosCorreccion(po);
                    ArmarCuadricula();

                    // Lo ultimo capturado de esta PO: ORDER QTY vigente, total y foto
                    if (po != "")
                    {
                        foreach (var s in f.ConsultaSugerenciaPedidoBENS(po))
                        {
                            TextBox t;
                            if (cajas.TryGetValue(NormalizarTalla(s.talla), out t)) t.Text = s.pedido.ToString();
                        }
                        Funciones.CutOrderBENS ultimo = null;
                        try { ultimo = f.ConsultaUltimoCutOrderBENS(po); } catch { }
                        if (ultimo != null)
                        {
                            txtTotal.Text = ultimo.totalUnits.ToString();
                            txtEstilo.Text = ultimo.estilo;
                            if (ultimo.foto != null && ultimo.foto.Length > 0) PonerFoto(ultimo.foto, ultimo.fotoNombre);
                        }
                        else if (txtEstilo.Text.Trim() == "" && catalogo.Count > 0)
                        {
                            txtEstilo.Text = catalogo.Select(c => c.estilo).FirstOrDefault(e => e != "");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Cut Order");
                }
                finally
                {
                    cargando = false;
                }
                Recalcular(false);
                cambios = false;
            }

            private Dictionary<string, int> Capturado(out List<string> invalidas)
            {
                var r = new Dictionary<string, int>();
                invalidas = new List<string>();
                foreach (var kv in cajas)
                {
                    string t = kv.Value.Text.Trim().Replace(",", "");
                    if (t == "" || t == "Ø" || t == "ø") continue;
                    int n;
                    if (!int.TryParse(t, NumberStyles.Integer, CultureInfo.InvariantCulture, out n) || n < 0) { invalidas.Add(kv.Key); continue; }
                    if (n > 0) r[kv.Key] = n;
                }
                return r;
            }

            private bool cuadra;

            private void Recalcular(bool porCambio = false)
            {
                if (cargando) return;
                if (porCambio) cambios = true;
                List<string> invalidas;
                var cap = Capturado(out invalidas);
                var tallasPO = new HashSet<string>(catalogo.Select(c => NormalizarTalla(c.talla)));
                var avisos = new List<string>();

                foreach (var kv in cajas)
                {
                    bool mala = invalidas.Contains(kv.Key);
                    bool fueraPO = cap.ContainsKey(kv.Key) && tallasPO.Count > 0 && !tallasPO.Contains(kv.Key);
                    kv.Value.BackColor = mala ? Color.FromArgb(255, 199, 199) : fueraPO ? Color.FromArgb(255, 229, 180) : cap.ContainsKey(kv.Key) ? Color.FromArgb(226, 239, 218) : Color.White;
                }
                foreach (var b in bloques)
                {
                    Label sub;
                    if (subtotales.TryGetValue(b.Titulo, out sub))
                        sub.Text = "Subtotal: " + b.Tallas.Sum(t => cap.ContainsKey(t) ? cap[t] : 0).ToString("N0");
                }

                if (invalidas.Count > 0) avisos.Add("No son número: " + string.Join(", ", invalidas.Select(t => t.Replace(" x ", "x"))));
                var fuera = cap.Keys.Where(t => tallasPO.Count > 0 && !tallasPO.Contains(t)).ToList();
                if (fuera.Count > 0) avisos.Add("No están en la PO: " + string.Join(", ", fuera.Select(t => t.Replace(" x ", "x"))));
                if (PoConAI(txtPO.Text) != "" && catalogo.Count == 0) avisos.Add("La PO todavía no está dada de alta en el sistema.");
                string estilo = txtEstilo.Text.Trim();
                if (estilo != "" && catalogo.Count > 0 && !catalogo.Any(c => c.estilo == estilo)) avisos.Add("El LOT # " + estilo + " no es el estilo de la PO (" + string.Join(", ", catalogo.Select(c => c.estilo).Distinct()) + ").");

                int suma = cap.Values.Sum();
                int total;
                bool hayTotal = int.TryParse(txtTotal.Text.Trim().Replace(",", ""), out total);
                cuadra = hayTotal && total == suma && invalidas.Count == 0 && suma > 0;
                if (!hayTotal)
                {
                    lblResultado.ForeColor = Color.FromArgb(90, 90, 90);
                    lblResultado.Text = "Suma capturada: " + suma.ToString("N0") + "   —   escribe el TOTAL UNITS de la hoja";
                }
                else if (total == suma)
                {
                    lblResultado.ForeColor = Color.FromArgb(30, 130, 60);
                    lblResultado.Text = "✔ Suma capturada " + suma.ToString("N0") + " = TOTAL UNITS " + total.ToString("N0");
                }
                else
                {
                    lblResultado.ForeColor = Color.FromArgb(190, 30, 30);
                    lblResultado.Text = "✘ Suma capturada " + suma.ToString("N0") + "  ≠  TOTAL UNITS " + total.ToString("N0") + "   (diferencia " + (suma - total).ToString("+#,##0;-#,##0") + ")";
                }
                // Sin TOTAL UNITS se pinta amarillo para que se note que falta
                txtTotal.BackColor = !hayTotal ? Color.FromArgb(255, 242, 157) : total == suma ? Color.FromArgb(226, 239, 218) : Color.FromArgb(255, 199, 199);
                lblAvisos.Text = string.Join("\r\n", avisos);
                // El boton siempre se puede presionar: si falta algo, Guardar explica que
            }

            // ---------- Foto ----------
            private void PonerFoto(byte[] bytes, string nombre)
            {
                try
                {
                    using (var ms = new MemoryStream(bytes))
                    using (var img = Image.FromStream(ms))
                        foto.Image = new Bitmap(img);
                    if (!cargando) cambios = true;
                    fotoBytes = bytes;
                    fotoNombre = nombre;
                    lblFoto.Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir la imagen: " + ex.Message, "Cut Order");
                }
            }

            private void CargarFotoArchivo()
            {
                using (var dlg = new OpenFileDialog { Filter = "Imágenes|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Todos los archivos|*.*" })
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                        PonerFoto(File.ReadAllBytes(dlg.FileName), Path.GetFileName(dlg.FileName));
            }

            private void PegarFoto()
            {
                Image img = null;
                if (Clipboard.ContainsImage()) img = Clipboard.GetImage();
                else if (Clipboard.ContainsFileDropList() && Clipboard.GetFileDropList().Count > 0)
                {
                    PonerFoto(File.ReadAllBytes(Clipboard.GetFileDropList()[0]), Path.GetFileName(Clipboard.GetFileDropList()[0]));
                    return;
                }
                if (img == null)
                {
                    MessageBox.Show("No hay una imagen copiada. En WhatsApp abre la foto, cópiala y vuelve a intentar.", "Cut Order");
                    return;
                }
                using (var ms = new MemoryStream())
                {
                    var jpg = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    var p = new EncoderParameters(1);
                    p.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);
                    img.Save(ms, jpg, p);
                    PonerFoto(ms.ToArray(), "pegada " + DateTime.Now.ToString("yyyy-MM-dd HHmm") + ".jpg");
                }
            }

            private void VerFotoGrande()
            {
                if (fotoBytes == null) return;
                string ext = Path.GetExtension(fotoNombre ?? "");
                string ruta = Path.Combine(Path.GetTempPath(), "CutOrder " + PoConAI(txtPO.Text) + " " + DateTime.Now.Ticks + (ext == "" ? ".jpg" : ext));
                File.WriteAllBytes(ruta, fotoBytes);
                System.Diagnostics.Process.Start(ruta);
            }

            // Lee el Excel "PURCHASE ORDER": talla -> cantidad, y PO# / Style si vienen.
            public static Dictionary<string, int> LeerExcelPO(string ruta, out string poExcel, out string estiloExcel)
            {
                var leidas = new Dictionary<string, int>();
                poExcel = ""; estiloExcel = "";
                using (var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var libro = new XLWorkbook(fs))
                {
                    var hoja = libro.Worksheets.First();
                    var usado = hoja.RangeUsed();
                    if (usado == null) throw new Exception("El archivo está vacío.");
                    int ultimaFila = usado.LastRow().RowNumber(), ultimaCol = usado.LastColumn().ColumnNumber();

                    int filaEnc = 0, colTalla = 0, colQty = 0, colEstilo = 0;
                    for (int r = 1; r <= Math.Min(ultimaFila, 60) && filaEnc == 0; r++)
                    {
                        int cT = 0, cQ = 0, cE = 0;
                        for (int c = 1; c <= ultimaCol; c++)
                        {
                            string v = hoja.Cell(r, c).GetString().Trim().ToUpper();
                            if (v == "SIZE" || v == "TALLA") cT = c;
                            else if (v.StartsWith("QTY") || v.StartsWith("CANTIDAD")) cQ = c;
                            else if (v == "STYLE" || v == "ESTILO") cE = c;
                            if (v.StartsWith("PO#") || v == "PO") { for (int k = c + 1; k <= ultimaCol; k++) { string p = hoja.Cell(r, k).GetString().Trim(); if (p != "") { poExcel = p; break; } } }
                        }
                        if (cT > 0 && cQ > 0) { filaEnc = r; colTalla = cT; colQty = cQ; colEstilo = cE; }
                    }
                    if (filaEnc == 0) throw new Exception("No encontré el renglón de encabezados con las columnas \"Size\" y \"Qty\".");

                    for (int r = filaEnc + 1; r <= ultimaFila; r++)
                    {
                        if (colEstilo > 0 && estiloExcel == "") estiloExcel = hoja.Cell(r, colEstilo).GetString().Trim();
                        string talla = NormalizarTalla(hoja.Cell(r, colTalla).GetString());
                        if (talla == "" || !talla.Contains(" x ")) continue;
                        double q;
                        var celda = hoja.Cell(r, colQty);
                        if (!celda.TryGetValue(out q) && !double.TryParse(celda.GetString().Replace(",", ""), NumberStyles.Any, CultureInfo.InvariantCulture, out q)) q = 0;
                        leidas[talla] = (leidas.ContainsKey(talla) ? leidas[talla] : 0) + (int)Math.Round(q);
                    }
                }
                return leidas;
            }

            // ---------- Importar el Excel "PURCHASE ORDER" ----------
            // Busca el renglon de encabezados con "Size" y "Qty" y lee talla / cantidad hacia abajo.
            // Tambien toma PO# y Style si los encuentra.
            private void ImportarExcel()
            {
                string ruta;
                using (var dlg = new OpenFileDialog { Filter = "Excel (*.xlsx;*.xlsm)|*.xlsx;*.xlsm" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    ruta = dlg.FileName;
                }
                try
                {
                    string poExcel, estiloExcel;
                    Dictionary<string, int> leidas = LeerExcelPO(ruta, out poExcel, out estiloExcel);

                    var desconocidas = leidas.Where(kv => kv.Value > 0 && !cajas.ContainsKey(kv.Key)).Select(kv => kv.Key.Replace(" x ", "x") + " (" + kv.Value + ")").ToList();
                    string poNormal = PoConAI(poExcel);
                    if (poNormal != "" && PoConAI(txtPO.Text) != "" && poNormal != PoConAI(txtPO.Text) &&
                        MessageBox.Show("El Excel es de la PO " + poNormal + " y aquí tienes la " + PoConAI(txtPO.Text) + ".\r\n\r\n¿Importarlo de todos modos?", "Cut Order", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                        return;
                    if (PoConAI(txtPO.Text) == "" && poNormal != "") { txtPO.Text = poNormal; CargarPO(); }

                    cargando = true;
                    foreach (var kv in cajas) kv.Value.Text = "";
                    foreach (var kv in leidas)
                    {
                        TextBox t;
                        if (cajas.TryGetValue(kv.Key, out t)) t.Text = kv.Value > 0 ? kv.Value.ToString() : "";
                    }
                    if (estiloExcel != "") txtEstilo.Text = estiloExcel;
                    cargando = false;
                    Recalcular(true);

                    MessageBox.Show("Se importaron " + leidas.Count(kv => kv.Value > 0) + " talla(s) con " + leidas.Values.Sum().ToString("N0") + " pieza(s)." +
                        (desconocidas.Count > 0 ? "\r\n\r\nEstas tallas del Excel no están en el formulario ni en la PO y NO se importaron:\r\n" + string.Join(", ", desconocidas) : "") +
                        "\r\n\r\nAhora escribe el TOTAL UNITS de la foto para comprobar que la captura del Excel cuadra.", "Cut Order");
                }
                catch (Exception ex)
                {
                    cargando = false;
                    MessageBox.Show("No se pudo leer el Excel: " + ex.Message, "Cut Order");
                }
            }

            // ---------- Guardar ----------
            private void Guardar()
            {
                Recalcular(false);
                if (!cuadra)
                {
                    ExplicarPorQueNoCuadra();
                    return;
                }
                // Desde el Summary, si ya estaba guardado y no se cambio nada, solo se continua
                if (modoSummary && !cambios) { DialogResult = DialogResult.OK; return; }
                string po = PoConAI(txtPO.Text);
                List<string> invalidas;
                var cap = Capturado(out invalidas);
                int total = cap.Values.Sum();

                if (lblAvisos.Text.Trim() != "" &&
                    MessageBox.Show("Hay avisos:\r\n\r\n" + lblAvisos.Text + "\r\n\r\n¿Guardar de todos modos?", "Cut Order", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
                if (fotoBytes == null &&
                    MessageBox.Show("No cargaste la foto del cliente. Se recomienda guardarla por si después hay que revisar.\r\n\r\n¿Guardar sin foto?", "Cut Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    string detalle = string.Join(";", cap.OrderBy(kv => kv.Key).Select(kv => kv.Key + "=" + kv.Value));
                    f.GuardarCutOrderBENS(po, txtEstilo.Text.Trim(), total, total, detalle, fotoBytes, fotoNombre, principal.usu[0].id);
                    // ORDER QTY vigente de la PO (el que sugiere el Summary)
                    foreach (string talla in cajas.Keys)
                        f.GuardarSugerenciaPedidoBENS(po, talla, cap.ContainsKey(talla) ? (int?)cap[talla] : null, principal.usu[0].id);
                    Cursor.Current = Cursors.Default;
                    cambios = false;
                    if (modoSummary) { DialogResult = DialogResult.OK; return; }
                    MessageBox.Show("Se guardó el Cut Order de la PO " + po + ": " + total.ToString("N0") + " piezas en " + cap.Count + " talla(s).\r\n\r\n" +
                        "Ya queda como ORDER QTY en el Summary de Ben Davis.", "Cut Order");
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    MessageBox.Show("No se pudo guardar: " + ex.Message, "Cut Order");
                }
            }

            private void ExplicarPorQueNoCuadra()
            {
                List<string> invalidas;
                var cap = Capturado(out invalidas);
                int suma = cap.Values.Sum(), total;
                bool hayTotal = int.TryParse(txtTotal.Text.Trim().Replace(",", ""), out total);

                if (PoConAI(txtPO.Text) == "")
                {
                    MessageBox.Show("Escribe el CUT # (la PO).", "Cut Order");
                    txtPO.Focus();
                }
                else if (invalidas.Count > 0)
                {
                    MessageBox.Show("Estas casillas no son número: " + string.Join(", ", invalidas.Select(t => t.Replace(" x ", "x"))) + ".", "Cut Order");
                    cajas[invalidas[0]].Focus();
                }
                else if (suma == 0)
                {
                    MessageBox.Show("No hay cantidades capturadas. Cópialas de la foto o usa \"Importar Excel de la PO\".", "Cut Order");
                }
                else if (!hayTotal)
                {
                    MessageBox.Show("Falta el TOTAL UNITS.\r\n\r\nEscribe el total que viene escrito en la hoja del cliente (dice \"TOTAL UNITS:\"). " +
                        "Sirve para comprobar que lo capturado cuadra con lo que pidió el cliente.\r\n\r\nLo capturado suma " + suma.ToString("N0") + ".", "Cut Order");
                    txtTotal.Focus();
                }
                else
                {
                    MessageBox.Show("No cuadra: lo capturado suma " + suma.ToString("N0") + " y el TOTAL UNITS de la hoja dice " + total.ToString("N0") +
                        " (diferencia " + (suma - total).ToString("+#,##0;-#,##0") + ").\r\n\r\nRevisa casilla por casilla contra la foto, o confirma el TOTAL UNITS.", "Cut Order",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTotal.Focus();
                }
            }

            // ---------- Hoja para que el cliente confirme ----------
            private void HojaCliente()
            {
                List<string> invalidas;
                var cap = Capturado(out invalidas);
                if (cap.Count == 0) { MessageBox.Show("No hay cantidades capturadas.", "Cut Order"); return; }
                string po = PoConAI(txtPO.Text);
                using (var dlg = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = (po == "" ? "Cut Order" : po) + " Cut Order para confirmar.xlsx" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    try
                    {
                        using (var libro = new XLWorkbook())
                        {
                            var hoja = libro.Worksheets.Add("Cut Order");
                            hoja.Style.Font.FontName = "Calibri";
                            hoja.Style.Font.FontSize = 11;
                            hoja.Cell(1, 1).Value = "BEN DAVIS CUT ORDER - CONFIRMATION";
                            hoja.Cell(1, 1).Style.Font.Bold = true; hoja.Cell(1, 1).Style.Font.FontSize = 14;
                            hoja.Cell(3, 1).Value = "CUT #:"; hoja.Cell(3, 2).Value = po;
                            hoja.Cell(4, 1).Value = "LOT #:"; hoja.Cell(4, 2).Value = txtEstilo.Text.Trim();
                            hoja.Cell(5, 1).Value = "DATE:"; hoja.Cell(5, 2).Value = DateTime.Now.ToString("MM/dd/yyyy");
                            hoja.Cell(6, 1).Value = "TOTAL UNITS:"; hoja.Cell(6, 2).Value = cap.Values.Sum();
                            hoja.Range(3, 1, 6, 1).Style.Font.Bold = true;
                            hoja.Cell(6, 2).Style.Font.Bold = true;
                            hoja.Range(3, 2, 6, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            int[][] columnas = { new[] { 0, 1 }, new[] { 2 }, Enumerable.Range(3, bloques.Count - 3).ToArray() };
                            for (int c = 0; c < columnas.Length; c++)
                            {
                                int col = 1 + c * 3, fila = 8;
                                foreach (int b in columnas[c])
                                {
                                    var bl = bloques[b];
                                    hoja.Cell(fila, col).Value = "SIZES"; hoja.Cell(fila, col + 1).Value = "QUANTITY";
                                    var enc = hoja.Range(fila, col, fila, col + 1);
                                    enc.Style.Font.Bold = true; enc.Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                                    int inicio = fila;
                                    foreach (string t in bl.Tallas)
                                    {
                                        fila++;
                                        hoja.Cell(fila, col).Value = t.Replace(" x ", "x");
                                        if (cap.ContainsKey(t)) hoja.Cell(fila, col + 1).Value = cap[t];
                                    }
                                    fila++;
                                    hoja.Cell(fila, col).Value = "Subtotal";
                                    hoja.Cell(fila, col + 1).Value = bl.Tallas.Sum(t => cap.ContainsKey(t) ? cap[t] : 0);
                                    hoja.Range(fila, col, fila, col + 1).Style.Font.Bold = true;
                                    var tabla = hoja.Range(inicio, col, fila, col + 1);
                                    tabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    tabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                                    hoja.Range(inicio, col + 1, fila, col + 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    fila += 2;
                                }
                                hoja.Column(col).Width = 10; hoja.Column(col + 1).Width = 11; hoja.Column(col + 2).Width = 3;
                            }
                            hoja.PageSetup.FitToPages(1, 1);
                            libro.SaveAs(dlg.FileName);
                        }
                        if (MessageBox.Show("Se guardó la hoja para el cliente.\r\n\r\n¿Abrirla?", "Cut Order", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            System.Diagnostics.Process.Start(dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Cut Order");
                    }
                }
            }
        }
    }
}
