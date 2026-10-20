using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Inventario.Controles
{
    public enum TipoResumen { Ninguno, Suma, Conteo, Promedio, Minimo, Maximo }

    /// <summary>Configuracion de una columna del GridReporte.</summary>
    public class ColumnaGrid
    {
        public ColumnaGrid(string campo, string titulo = null, int ancho = 100, int orden = 0, bool visible = true)
        {
            Campo = campo; Titulo = string.IsNullOrEmpty(titulo) ? campo : titulo; Ancho = ancho; Orden = orden; Visible = visible;
        }
        public string Campo { get; set; }
        public string Titulo { get; set; }
        public int Ancho { get; set; }
        public int Orden { get; set; }
        public bool Visible { get; set; }
        /// <summary>Columna congelada a la izquierda.</summary>
        public bool Fija { get; set; }
        /// <summary>Formato .NET, p.ej. "N0", "dd/MM/yyyy".</summary>
        public string Formato { get; set; }
        public TipoResumen Resumen { get; set; }
        /// <summary>Formato del total al pie, p.ej. "Cajas: {0}".</summary>
        public string FormatoResumen { get; set; }
    }

    /// <summary>Subtotal que se muestra en el renglon de cada grupo, p.ej. "Prendas: {0}".</summary>
    public class ResumenGrupo
    {
        public ResumenGrupo(TipoResumen tipo, string campo, string formato) { Tipo = tipo; Campo = campo; Formato = formato; }
        public TipoResumen Tipo { get; set; }
        public string Campo { get; set; }
        public string Formato { get; set; }
    }

    /// <summary>
    /// Grid de reportes gratuito (reemplazo de DevExpress XtraGrid) sobre DataGridView en modo virtual:
    /// busqueda, filtro por columna, agrupar con subtotales, totales al pie, orden por encabezado,
    /// copiar celdas, exportar a Excel e imprimir.
    /// </summary>
    public class GridReporte : UserControl
    {
        private readonly DataGridView grid = new DataGridView();
        private readonly Panel barra = new Panel();
        private readonly TextBox txtBuscar = new TextBox();
        private readonly ComboBox cmbAgrupar = new ComboBox();
        private readonly Button btnExcel = new Button();
        private readonly Button btnImprimir = new Button();
        private readonly Button btnExpandir = new Button();
        private readonly Label lblConteo = new Label();
        private readonly Panel panelFiltros = new Panel();
        private readonly Panel panelPie = new Panel();
        private readonly Timer temporizador = new Timer { Interval = 250 };

        private DataTable origen = new DataTable();
        private readonly List<object> filas = new List<object>(); // DataRow o Grupo
        private readonly Dictionary<string, TextBox> filtros = new Dictionary<string, TextBox>();
        private readonly HashSet<string> colapsados = new HashSet<string>();
        private string ordenCampo; private bool ordenDesc;
        private object dataSource;

        private class Grupo
        {
            public int Nivel; public string Clave; public string Texto; public int Cuenta;
        }

        public GridReporte()
        {
            Columnas = new List<ColumnaGrid>();
            ResumenesGrupo = new List<ResumenGrupo>();
            Agrupar = new List<string>();
            MostrarFiltros = true;
            MostrarPie = true;

            BuildUi();
        }

        // ---------------- configuracion ----------------
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ColumnaGrid> Columnas { get; private set; }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ResumenGrupo> ResumenesGrupo { get; private set; }
        /// <summary>Campos por los que se agrupa al cargar (el usuario puede cambiarlo en "Agrupar por").</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<string> Agrupar { get; private set; }
        [DefaultValue(true)] public bool MostrarFiltros { get; set; }
        [DefaultValue(true)] public bool MostrarPie { get; set; }
        /// <summary>Titulo para la impresion y el archivo de Excel.</summary>
        public string Titulo { get; set; }

        public void OrdenarPor(string campo, bool descendente) { ordenCampo = campo; ordenDesc = descendente; }

        /// <summary>Lista de objetos, DataTable, DataView o BindingSource.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                if (DesignMode) return;
                origen = ATabla(value);
                AplicarConfiguracion();
                Reconstruir();
            }
        }

        /// <summary>Filas visibles (despues de filtros), en el orden mostrado.</summary>
        public IEnumerable<DataRow> FilasVisibles { get { return filas.OfType<DataRow>(); } }

        // ---------------- interfaz ----------------
        private void BuildUi()
        {
            SuspendLayout();
            barra.Dock = DockStyle.Top; barra.Height = 32; barra.Padding = new Padding(4);
            var lblB = new Label { Text = "Buscar:", AutoSize = true, Location = new Point(4, 9) };
            txtBuscar.Location = new Point(56, 5); txtBuscar.Width = 200;
            txtBuscar.TextChanged += (s, e) => { temporizador.Stop(); temporizador.Start(); };
            var lblA = new Label { Text = "Agrupar por:", AutoSize = true, Location = new Point(270, 9) };
            cmbAgrupar.Location = new Point(345, 5); cmbAgrupar.Width = 170; cmbAgrupar.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAgrupar.SelectedIndexChanged += (s, e) =>
            {
                if (cargandoCombo) return;
                Agrupar.Clear();
                var item = cmbAgrupar.SelectedItem as ColumnaGrid;
                if (item != null) Agrupar.Add(item.Campo);
                colapsados.Clear();
                Reconstruir();
            };
            btnExpandir.Text = "Contraer todo"; btnExpandir.Location = new Point(522, 4); btnExpandir.Width = 95; btnExpandir.Visible = false;
            btnExpandir.Click += (s, e) => AlternarTodo();
            btnExcel.Text = "Excel"; btnExcel.Location = new Point(622, 4); btnExcel.Width = 60;
            btnExcel.Click += (s, e) => ExportarExcel();
            btnImprimir.Text = "Imprimir"; btnImprimir.Location = new Point(687, 4); btnImprimir.Width = 70;
            btnImprimir.Click += (s, e) => VistaPrevia(Titulo);
            lblConteo.AutoSize = true; lblConteo.Location = new Point(765, 9);
            barra.Controls.AddRange(new Control[] { lblB, txtBuscar, lblA, cmbAgrupar, btnExpandir, btnExcel, btnImprimir, lblConteo });

            panelFiltros.Dock = DockStyle.Top; panelFiltros.Height = 24;
            panelPie.Dock = DockStyle.Bottom; panelPie.Height = 24; panelPie.BackColor = SystemColors.ControlLight;

            grid.Dock = DockStyle.Fill;
            grid.VirtualMode = true;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToOrderColumns = true;
            grid.AutoGenerateColumns = false;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            grid.MultiSelect = true;
            grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
            grid.BackgroundColor = SystemColors.Window;
            grid.BorderStyle = BorderStyle.None;
            grid.RowTemplate.Height = 22;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(204, 228, 247);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, grid, new object[] { true });
            grid.CellValueNeeded += Grid_CellValueNeeded;
            grid.RowPrePaint += Grid_RowPrePaint;
            grid.CellPainting += Grid_CellPainting;
            grid.CellClick += Grid_CellClick;
            grid.ColumnHeaderMouseClick += Grid_ColumnHeaderMouseClick;
            grid.ColumnWidthChanged += (s, e) => Reubicar();
            grid.ColumnDisplayIndexChanged += (s, e) => Reubicar();
            grid.Scroll += (s, e) => { if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll) Reubicar(); };
            grid.SizeChanged += (s, e) => Reubicar();

            temporizador.Tick += (s, e) => { temporizador.Stop(); Reconstruir(); };

            Controls.Add(grid);
            Controls.Add(panelFiltros);
            Controls.Add(panelPie);
            Controls.Add(barra);
            ResumeLayout();
        }

        private bool cargandoCombo;

        private void AplicarConfiguracion()
        {
            // columnas configuradas que existen en los datos; si no hay configuracion, todas las columnas
            var cols = Columnas.Where(c => origen.Columns.Contains(c.Campo)).ToList();
            if (cols.Count == 0)
                cols = origen.Columns.Cast<DataColumn>().Select((c, i) => new ColumnaGrid(c.ColumnName, c.ColumnName, 100, i)).ToList();

            grid.Columns.Clear();
            // Las fijas se agregan primero porque DataGridView se queja ("La columna no se puede agregar
            // porque esta inmovilizada y situada despues de una columna que no esta inmovilizada") si
            // metemos una columna Frozen=true despues de otra Frozen=false.
            foreach (var c in cols.Where(c => c.Visible).OrderBy(c => c.Fija ? 0 : 1).ThenBy(c => c.Orden))
            {
                var dc = new DataGridViewTextBoxColumn
                {
                    Name = c.Campo, HeaderText = c.Titulo, Width = Math.Max(30, c.Ancho), Tag = c,
                    SortMode = DataGridViewColumnSortMode.Programmatic,
                    Frozen = c.Fija
                };
                if (!string.IsNullOrEmpty(c.Formato)) dc.DefaultCellStyle.Format = c.Formato;
                Type t = origen.Columns[c.Campo].DataType;
                if (t != typeof(string) && t != typeof(DateTime) && t != typeof(bool)) dc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.Columns.Add(dc);
            }
            // DisplayIndex por el orden configurado, pero manteniendo las fijas a la izquierda
            int idx = 0;
            foreach (DataGridViewColumn col in grid.Columns.Cast<DataGridViewColumn>()
                        .OrderBy(x => x.Frozen ? 0 : 1)
                        .ThenBy(x => ((ColumnaGrid)x.Tag).Orden).ToList())
                col.DisplayIndex = idx++;

            // filtros
            panelFiltros.Controls.Clear(); filtros.Clear();
            panelFiltros.Visible = MostrarFiltros;
            foreach (DataGridViewColumn col in grid.Columns)
            {
                var tb = new TextBox { BorderStyle = BorderStyle.FixedSingle, Tag = col.Name, Font = new Font(Font.FontFamily, 8.25f) };
                tb.TextChanged += (s, e) => { temporizador.Stop(); temporizador.Start(); };
                panelFiltros.Controls.Add(tb);
                filtros[col.Name] = tb;
            }
            // pie
            panelPie.Visible = MostrarPie && cols.Any(c => c.Resumen != TipoResumen.Ninguno);
            panelPie.Controls.Clear();
            foreach (DataGridViewColumn col in grid.Columns)
                panelPie.Controls.Add(new Label { Tag = col.Name, AutoSize = false, TextAlign = ContentAlignment.MiddleRight, Font = new Font(Font, FontStyle.Bold), Height = 22 });

            // combo de agrupar
            cargandoCombo = true;
            cmbAgrupar.Items.Clear();
            cmbAgrupar.Items.Add("(sin agrupar)");
            foreach (var c in cols.Where(c => c.Visible).OrderBy(c => c.Titulo)) cmbAgrupar.Items.Add(c);
            cmbAgrupar.DisplayMember = "Titulo";
            var g = Agrupar.FirstOrDefault();
            object sel = cols.FirstOrDefault(c => c.Campo == g);
            cmbAgrupar.SelectedItem = sel ?? cmbAgrupar.Items[0];
            cargandoCombo = false;
        }

        // ---------------- datos ----------------
        private static DataTable ATabla(object fuente)
        {
            var bs = fuente as BindingSource;
            if (bs != null) fuente = bs.DataSource is Type ? null : bs.List;
            if (fuente == null) return new DataTable();
            var dt = fuente as DataTable;
            if (dt != null) return dt;
            var dv = fuente as DataView;
            if (dv != null) return dv.ToTable();
            var lista = fuente as IEnumerable;
            if (lista == null) return new DataTable();
            Type t = null;
            Type tipoLista = fuente.GetType();
            if (tipoLista.IsGenericType) t = tipoLista.GetGenericArguments()[0];
            else if (tipoLista.IsArray) t = tipoLista.GetElementType();
            if (t == null) foreach (object o in lista) { if (o != null) { t = o.GetType(); break; } }
            var tabla = new DataTable();
            if (t == null) return tabla;
            var props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.GetIndexParameters().Length == 0)
                .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToList();
            foreach (var p in props)
            {
                Type pt = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (!(pt.IsPrimitive || pt == typeof(string) || pt == typeof(decimal) || pt == typeof(DateTime))) pt = typeof(string);
                tabla.Columns.Add(p.Name, pt);
            }
            tabla.BeginLoadData();
            foreach (object o in lista)
            {
                var r = tabla.NewRow();
                foreach (var p in props)
                {
                    object v = p.GetValue(o, null);
                    if (v != null && tabla.Columns[p.Name].DataType == typeof(string) && !(v is string)) v = v.ToString();
                    r[p.Name] = v ?? DBNull.Value;
                }
                tabla.Rows.Add(r);
            }
            tabla.EndLoadData();
            return tabla;
        }

        private static string Texto(object v, string formato)
        {
            if (v == null || v == DBNull.Value) return "";
            if (!string.IsNullOrEmpty(formato) && v is IFormattable) return ((IFormattable)v).ToString(formato, CultureInfo.CurrentCulture);
            return Convert.ToString(v, CultureInfo.CurrentCulture);
        }

        private bool Coincide(DataRow r, string buscar)
        {
            foreach (var kv in filtros)
            {
                string f = kv.Value.Text.Trim();
                if (f.Length == 0) continue;
                var col = (ColumnaGrid)grid.Columns[kv.Key].Tag;
                if (Texto(r[kv.Key], col.Formato).IndexOf(f, StringComparison.CurrentCultureIgnoreCase) < 0) return false;
            }
            if (buscar.Length == 0) return true;
            foreach (DataGridViewColumn c in grid.Columns)
                if (Texto(r[c.Name], ((ColumnaGrid)c.Tag).Formato).IndexOf(buscar, StringComparison.CurrentCultureIgnoreCase) >= 0) return true;
            return false;
        }

        private void Reconstruir()
        {
            string buscar = txtBuscar.Text.Trim();
            IEnumerable<DataRow> datos = origen.Rows.Cast<DataRow>().Where(r => Coincide(r, buscar));
            var grupos = Agrupar.Where(g => origen.Columns.Contains(g)).ToList();

            IOrderedEnumerable<DataRow> ordenado = null;
            foreach (var g in grupos)
                ordenado = ordenado == null ? datos.OrderBy(r => r[g] as IComparable ?? "", Comparador.Instancia) : ordenado.ThenBy(r => r[g] as IComparable ?? "", Comparador.Instancia);
            if (!string.IsNullOrEmpty(ordenCampo) && origen.Columns.Contains(ordenCampo))
            {
                Func<DataRow, IComparable> k = r => r[ordenCampo] as IComparable ?? "";
                if (ordenado == null) ordenado = ordenDesc ? datos.OrderByDescending(k, Comparador.Instancia) : datos.OrderBy(k, Comparador.Instancia);
                else ordenado = ordenDesc ? ordenado.ThenByDescending(k, Comparador.Instancia) : ordenado.ThenBy(k, Comparador.Instancia);
            }
            List<DataRow> lista = (ordenado ?? datos).ToList();

            filas.Clear();
            if (grupos.Count == 0) filas.AddRange(lista.Cast<object>());
            else AgregarGrupos(lista, grupos, 0, "");

            grid.RowCount = 0;
            grid.RowCount = filas.Count;
            foreach (DataGridViewColumn c in grid.Columns)
                c.HeaderCell.SortGlyphDirection = c.Name == ordenCampo ? (ordenDesc ? SortOrder.Descending : SortOrder.Ascending) : SortOrder.None;
            lblConteo.Text = lista.Count.ToString("N0") + " registros" + (lista.Count != origen.Rows.Count ? " de " + origen.Rows.Count.ToString("N0") : "");
            btnExpandir.Visible = grupos.Count > 0;
            ActualizarPie(lista);
            grid.Invalidate();
            Reubicar();
        }

        private void AgregarGrupos(List<DataRow> lista, List<string> grupos, int nivel, string prefijo)
        {
            string campo = grupos[nivel];
            var col = Columnas.FirstOrDefault(c => c.Campo == campo);
            string titulo = col != null ? col.Titulo : campo;
            foreach (var g in lista.GroupBy(r => Texto(r[campo], col != null ? col.Formato : null)))
            {
                var elementos = g.ToList();
                string clave = prefijo + "\u001f" + g.Key;
                var resumen = ResumenesGrupo.Where(rg => origen.Columns.Contains(rg.Campo) || rg.Tipo == TipoResumen.Conteo)
                    .Select(rg => string.Format(rg.Formato ?? "{0}", Calcular(rg.Tipo, rg.Campo, elementos))).ToList();
                filas.Add(new Grupo
                {
                    Nivel = nivel, Clave = clave, Cuenta = elementos.Count,
                    Texto = titulo + ": " + (g.Key == "" ? "(vacio)" : g.Key) + "    (" + elementos.Count.ToString("N0") + ")" + (resumen.Count > 0 ? "    " + string.Join("   ", resumen) : "")
                });
                if (colapsados.Contains(clave)) continue;
                if (nivel + 1 < grupos.Count) AgregarGrupos(elementos, grupos, nivel + 1, clave);
                else filas.AddRange(elementos.Cast<object>());
            }
        }

        private object Calcular(TipoResumen tipo, string campo, List<DataRow> filasCalc)
        {
            if (tipo == TipoResumen.Conteo) return filasCalc.Count;
            if (!origen.Columns.Contains(campo)) return "";
            var nums = filasCalc.Select(r => r[campo]).Where(v => v != null && v != DBNull.Value)
                .Select(v => { decimal d; return decimal.TryParse(Convert.ToString(v, CultureInfo.CurrentCulture), NumberStyles.Any, CultureInfo.CurrentCulture, out d) ? (decimal?)d : null; })
                .Where(d => d.HasValue).Select(d => d.Value).ToList();
            if (nums.Count == 0) return 0;
            switch (tipo)
            {
                case TipoResumen.Suma: return nums.Sum();
                case TipoResumen.Promedio: return Math.Round(nums.Average(), 2);
                case TipoResumen.Minimo: return nums.Min();
                case TipoResumen.Maximo: return nums.Max();
            }
            return "";
        }

        private void ActualizarPie(List<DataRow> lista)
        {
            foreach (Label l in panelPie.Controls)
            {
                var col = (ColumnaGrid)grid.Columns[(string)l.Tag].Tag;
                l.Text = col.Resumen == TipoResumen.Ninguno ? "" : string.Format(col.FormatoResumen ?? "{0}", Calcular(col.Resumen, col.Campo, lista));
            }
        }

        private void Reubicar()
        {
            int x0 = grid.Left - grid.HorizontalScrollingOffset;
            foreach (DataGridViewColumn c in grid.Columns)
            {
                Rectangle r = grid.GetColumnDisplayRectangle(c.Index, false);
                bool visible = r.Width > 0;
                TextBox tb; if (filtros.TryGetValue(c.Name, out tb)) { tb.Visible = visible; if (visible) tb.SetBounds(r.X, 1, r.Width, 22); }
                foreach (Label l in panelPie.Controls) if ((string)l.Tag == c.Name) { l.Visible = visible; if (visible) l.SetBounds(r.X, 1, r.Width, 22); }
            }
        }

        // ---------------- eventos del grid ----------------
        private void Grid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= filas.Count) return;
            var r = filas[e.RowIndex] as DataRow;
            if (r == null) { e.Value = null; return; }
            e.Value = r[grid.Columns[e.ColumnIndex].Name];
        }

        private void Grid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var g = e.RowIndex < filas.Count ? filas[e.RowIndex] as Grupo : null;
            if (g == null) return;
            Rectangle fila = new Rectangle(0, e.RowBounds.Top, grid.Width, e.RowBounds.Height);
            using (var fondo = new SolidBrush(g.Nivel == 0 ? Color.FromArgb(225, 235, 245) : Color.FromArgb(238, 243, 248)))
                e.Graphics.FillRectangle(fondo, fila);
            string signo = colapsados.Contains(g.Clave) ? "+ " : "– ";
            using (var f = new Font(grid.Font, FontStyle.Bold))
                TextRenderer.DrawText(e.Graphics, signo + g.Texto, f, new Rectangle(6 + g.Nivel * 18, fila.Top, fila.Width - 10, fila.Height), Color.FromArgb(20, 40, 80), TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            e.Handled = true;
        }

        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < filas.Count && filas[e.RowIndex] is Grupo) e.Handled = true; // ya se pinto el renglon completo
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= filas.Count) return;
            var g = filas[e.RowIndex] as Grupo;
            if (g == null) return;
            if (!colapsados.Remove(g.Clave)) colapsados.Add(g.Clave);
            int primera = grid.FirstDisplayedScrollingRowIndex;
            Reconstruir();
            if (primera >= 0 && primera < grid.RowCount) grid.FirstDisplayedScrollingRowIndex = primera;
        }

        private void AlternarTodo()
        {
            bool hayAbiertos = filas.OfType<Grupo>().Any(g => !colapsados.Contains(g.Clave));
            if (hayAbiertos) { foreach (var g in filas.OfType<Grupo>().Where(x => x.Nivel == 0)) colapsados.Add(g.Clave); btnExpandir.Text = "Expandir todo"; }
            else { colapsados.Clear(); btnExpandir.Text = "Contraer todo"; }
            Reconstruir();
        }

        private void Grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            string campo = grid.Columns[e.ColumnIndex].Name;
            ordenDesc = campo == ordenCampo ? !ordenDesc : false;
            ordenCampo = campo;
            Reconstruir();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F)) { txtBuscar.Focus(); txtBuscar.SelectAll(); return true; }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ---------------- Excel ----------------
        public void ExportarExcel()
        {
            using (var dlg = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = (string.IsNullOrEmpty(Titulo) ? "Reporte" : Titulo) + " " + DateTime.Now.ToString("yyyy-MM-dd HHmm") + ".xlsx" })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                Cursor = Cursors.WaitCursor;
                try
                {
                    ExportarExcel(dlg.FileName);
                    System.Diagnostics.Process.Start(dlg.FileName);
                }
                catch (Exception ex) { MessageBox.Show(this, "No se pudo exportar: " + ex.Message); }
                finally { Cursor = Cursors.Default; }
            }
        }

        /// <summary>Exporta lo que se ve (filtros, grupos y totales) a un archivo .xlsx.</summary>
        public void ExportarExcel(string ruta)
        {
            var cols = grid.Columns.Cast<DataGridViewColumn>().OrderBy(c => c.DisplayIndex).ToList();
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Reporte");
                for (int i = 0; i < cols.Count; i++) ws.Cell(1, i + 1).Value = cols[i].HeaderText;
                ws.Row(1).Style.Font.Bold = true;
                int fila = 2;
                foreach (object o in filas)
                {
                    var g = o as Grupo;
                    if (g != null)
                    {
                        ws.Cell(fila, 1).Value = new string(' ', g.Nivel * 3) + g.Texto;
                        ws.Range(fila, 1, fila, cols.Count).Style.Fill.BackgroundColor = XLColor.FromArgb(225, 235, 245);
                        ws.Range(fila, 1, fila, cols.Count).Style.Font.Bold = true;
                        fila++; continue;
                    }
                    var r = (DataRow)o;
                    for (int i = 0; i < cols.Count; i++)
                    {
                        object v = r[cols[i].Name];
                        if (v == DBNull.Value || v == null) continue;
                        var celda = ws.Cell(fila, i + 1);
                        if (v is DateTime) celda.Value = (DateTime)v;
                        else if (v is string) celda.Value = "'" + v; // conserva ceros a la izquierda (UPC, PO)
                        else celda.Value = Convert.ToDouble(v, CultureInfo.InvariantCulture);
                        string fmt = ((ColumnaGrid)cols[i].Tag).Formato;
                        if (v is DateTime) celda.Style.DateFormat.Format = string.IsNullOrEmpty(fmt) ? "dd/mm/yyyy hh:mm" : fmt.Replace("tt", "AM/PM");
                    }
                    fila++;
                }
                // totales
                bool hayTotales = false;
                for (int i = 0; i < cols.Count; i++)
                {
                    var c = (ColumnaGrid)cols[i].Tag;
                    if (c.Resumen == TipoResumen.Ninguno) continue;
                    ws.Cell(fila, i + 1).Value = string.Format(c.FormatoResumen ?? "{0}", Calcular(c.Resumen, c.Campo, FilasVisibles.ToList()));
                    hayTotales = true;
                }
                if (hayTotales) ws.Row(fila).Style.Font.Bold = true;
                ws.SheetView.FreezeRows(1);
                ws.Range(1, 1, Math.Max(1, fila - 1), cols.Count).SetAutoFilter();
                ws.Columns().AdjustToContents(1, Math.Min(fila, 500));
                wb.SaveAs(ruta);
            }
        }

        // ---------------- impresion ----------------
        public void VistaPrevia(string titulo = null)
        {
            var cols = grid.Columns.Cast<DataGridViewColumn>().OrderBy(c => c.DisplayIndex).ToList();
            if (cols.Count == 0) return;
            int total = cols.Sum(c => c.Width);
            var doc = new PrintDocument { DocumentName = titulo ?? Titulo ?? "Reporte" };
            doc.DefaultPageSettings.Landscape = total > 750;
            doc.DefaultPageSettings.Margins = new Margins(40, 40, 50, 50);
            int indice = 0, pagina = 0;
            var lista = filas.ToList();
            var totales = cols.Select(c => { var cg = (ColumnaGrid)c.Tag; return cg.Resumen == TipoResumen.Ninguno ? "" : string.Format(cg.FormatoResumen ?? "{0}", Calcular(cg.Resumen, cg.Campo, FilasVisibles.ToList())); }).ToList();
            doc.BeginPrint += (s, e) => { indice = 0; pagina = 0; };
            doc.PrintPage += (s, e) =>
            {
                var g = e.Graphics; Rectangle m = e.MarginBounds;
                float escala = Math.Min(1f, (float)m.Width / total);
                using (var fTit = new Font("Arial", 12, FontStyle.Bold))
                using (var fEnc = new Font("Arial", 8 * Math.Max(0.75f, escala), FontStyle.Bold))
                using (var fDat = new Font("Arial", 8 * Math.Max(0.75f, escala)))
                using (var pen = new Pen(Color.Silver))
                {
                    pagina++;
                    float y = m.Top;
                    g.DrawString((titulo ?? Titulo ?? "Reporte"), fTit, Brushes.Black, m.Left, y);
                    g.DrawString("Pagina " + pagina + "   " + DateTime.Now.ToString("g"), fDat, Brushes.Gray, m.Right - 200, y + 4);
                    y += 26;
                    float alto = fDat.GetHeight(g) + 4;
                    float x = m.Left;
                    foreach (var c in cols)
                    {
                        float w = c.Width * escala;
                        g.FillRectangle(Brushes.Gainsboro, x, y, w, alto);
                        g.DrawRectangle(pen, x, y, w, alto);
                        g.DrawString(c.HeaderText, fEnc, Brushes.Black, new RectangleF(x + 2, y + 2, w - 4, alto), new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap });
                        x += w;
                    }
                    y += alto;
                    while (indice < lista.Count && y + alto < m.Bottom - alto)
                    {
                        object o = lista[indice];
                        var grp = o as Grupo;
                        if (grp != null)
                        {
                            g.FillRectangle(new SolidBrush(Color.FromArgb(225, 235, 245)), m.Left, y, total * escala, alto);
                            g.DrawString(new string(' ', grp.Nivel * 3) + grp.Texto, fEnc, Brushes.Black, m.Left + 2, y + 2);
                        }
                        else
                        {
                            var r = (DataRow)o; x = m.Left;
                            foreach (var c in cols)
                            {
                                float w = c.Width * escala;
                                var cg = (ColumnaGrid)c.Tag;
                                var sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
                                if (c.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight) sf.Alignment = StringAlignment.Far;
                                g.DrawString(Texto(r[c.Name], cg.Formato), fDat, Brushes.Black, new RectangleF(x + 2, y + 2, w - 4, alto), sf);
                                g.DrawLine(pen, x, y + alto, x + w, y + alto);
                                x += w;
                            }
                        }
                        y += alto; indice++;
                    }
                    e.HasMorePages = indice < lista.Count;
                    if (!e.HasMorePages && totales.Any(t => t != ""))
                    {
                        x = m.Left;
                        for (int i = 0; i < cols.Count; i++)
                        {
                            float w = cols[i].Width * escala;
                            g.DrawString(totales[i], fEnc, Brushes.Black, new RectangleF(x + 2, y + 4, w - 4, alto), new StringFormat { Alignment = StringAlignment.Far, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap });
                            x += w;
                        }
                    }
                }
            };
            using (var dlg = new PrintPreviewDialog { Document = doc, Width = 1100, Height = 800, UseAntiAlias = true })
            {
                ((Form)dlg).WindowState = FormWindowState.Maximized;
                dlg.ShowDialog(this);
            }
        }

        private class Comparador : IComparer<IComparable>
        {
            public static readonly Comparador Instancia = new Comparador();
            public int Compare(IComparable a, IComparable b)
            {
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;
                if (a.GetType() != b.GetType()) return string.Compare(a.ToString(), b.ToString(), StringComparison.CurrentCultureIgnoreCase);
                if (a is string) return string.Compare((string)a, (string)b, StringComparison.CurrentCultureIgnoreCase);
                return a.CompareTo(b);
            }
        }
    }
}
