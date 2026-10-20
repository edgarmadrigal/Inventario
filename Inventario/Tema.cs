using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario
{
    // Tema visual comun de V2. Se aplica en tiempo de ejecucion sobre los controles del disenador:
    // solo cambia colores y estilos, no mueve controles, no cambia fuentes ni toca la logica.
    public static class Tema
    {
        public static readonly Color Fondo = Color.FromArgb(243, 245, 248);
        public static readonly Color Texto = Color.FromArgb(31, 41, 51);
        public static readonly Color Primario = Color.FromArgb(30, 111, 184);
        public static readonly Color PrimarioHover = Color.FromArgb(22, 88, 150);
        public static readonly Color Peligro = Color.FromArgb(196, 52, 52);
        public static readonly Color PeligroHover = Color.FromArgb(160, 38, 38);
        public static readonly Color Deshabilitado = Color.FromArgb(203, 210, 219);
        public static readonly Color Borde = Color.FromArgb(206, 213, 222);
        public static readonly Color EncabezadoGrid = Color.FromArgb(44, 62, 80);
        public static readonly Color RenglonAlterno = Color.FromArgb(245, 248, 251);
        public static readonly Color Seleccion = Color.FromArgb(200, 222, 245);

        // Botones que borran o cancelan se pintan en rojo
        private static readonly string[] palabrasPeligro = { "CANCEL", "BAJA", "BORRAR", "ELIMINAR", "QUITAR" };

        public static void Aplicar(Control raiz)
        {
            Estilo(raiz);
            Recorrer(raiz);
        }

        private static void Recorrer(Control padre)
        {
            foreach (Control c in padre.Controls)
            {
                // El visor de ReportViewer (vista previa) se deja como viene
                string ns = c.GetType().Namespace ?? "";
                if (ns.StartsWith("Microsoft.Reporting")) continue;
                Estilo(c);
                Recorrer(c);
            }
        }

        // Colores que dejo el disenador "por default" y que si se pueden reemplazar
        private static bool ColorNeutro(Color x)
        {
            // Fondo cuenta como neutro porque los controles sin color propio heredan el de su contenedor
            int a = x.ToArgb();
            return x.IsSystemColor || a == Color.Silver.ToArgb() || a == Color.LightGray.ToArgb() || a == Color.Gainsboro.ToArgb()
                || a == Color.White.ToArgb() || a == SystemColors.Control.ToArgb() || a == Fondo.ToArgb();
        }

        private static void Estilo(Control c)
        {
            if (c is Form || c is TabPage || c is Panel || c is GroupBox || c is UserControl)
            {
                if (ColorNeutro(c.BackColor)) c.BackColor = Fondo;
                if (c is TabPage) ((TabPage)c).UseVisualStyleBackColor = false;
            }
            else if (c is Button) EstiloBoton((Button)c);
            else if (c is TextBox && ((TextBox)c).ReadOnly) c.BackColor = Color.White;
            else if (c is DataGridView) EstiloGrid((DataGridView)c);
            else if (c is TabControl) EstiloPestanas((TabControl)c);
            else if (c is Label || c is CheckBox || c is RadioButton)
            {
                if (c.ForeColor == SystemColors.ControlText || c.ForeColor == Color.Black) c.ForeColor = Texto;
            }
        }

        private static void EstiloBoton(Button b)
        {
            // Botones con color propio puesto a proposito se respetan
            if (!ColorNeutro(b.BackColor) && b.BackColor != Color.Transparent) return;

            b.UseVisualStyleBackColor = false;
            b.FlatStyle = FlatStyle.Flat;
            b.Cursor = Cursors.Hand;
            if (b.Image != null || b.BackgroundImage != null)
            {
                // Botones de icono: fondo blanco con borde suave
                b.BackColor = Color.White;
                b.FlatAppearance.BorderColor = Borde;
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.MouseOverBackColor = Seleccion;
                b.FlatAppearance.MouseDownBackColor = Seleccion;
                return;
            }

            string texto = (b.Text ?? "").ToUpperInvariant();
            bool peligro = Array.Exists(palabrasPeligro, p => texto.Contains(p));
            Color normal = peligro ? Peligro : Primario;
            Color hover = peligro ? PeligroHover : PrimarioHover;

            b.ForeColor = Color.White;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = hover;
            b.FlatAppearance.MouseDownBackColor = hover;
            b.BackColor = b.Enabled ? normal : Deshabilitado;
            b.EnabledChanged += (s, e) => b.BackColor = b.Enabled ? normal : Deshabilitado;
        }

        private static void EstiloGrid(DataGridView g)
        {
            g.BackgroundColor = Color.White;
            g.BorderStyle = BorderStyle.None;
            g.GridColor = Borde;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            g.ColumnHeadersDefaultCellStyle.BackColor = EncabezadoGrid;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = EncabezadoGrid;
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
            g.RowHeadersDefaultCellStyle.BackColor = Fondo;
            g.DefaultCellStyle.SelectionBackColor = Seleccion;
            g.DefaultCellStyle.SelectionForeColor = Texto;
            if (g.AlternatingRowsDefaultCellStyle.BackColor.IsEmpty)
                g.AlternatingRowsDefaultCellStyle.BackColor = RenglonAlterno;
        }

        // Pestanas mas altas, la seleccionada en azul
        private static void EstiloPestanas(TabControl t)
        {
            if (t.DrawMode == TabDrawMode.OwnerDrawFixed) return;
            t.DrawMode = TabDrawMode.OwnerDrawFixed;
            t.Padding = new Point(12, 6);
            t.DrawItem += (s, e) =>
            {
                TabPage p = t.TabPages[e.Index];
                bool sel = e.Index == t.SelectedIndex;
                Rectangle r = e.Bounds;
                using (var fondo = new SolidBrush(sel ? Primario : Color.FromArgb(228, 233, 239)))
                    e.Graphics.FillRectangle(fondo, r);
                TextRenderer.DrawText(e.Graphics, p.Text, t.Font, r, sel ? Color.White : Texto,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            };
        }
    }
}
