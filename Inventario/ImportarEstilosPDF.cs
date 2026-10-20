using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Inventario
{

    public partial class ImportarEstilosPDF : Form
    {

        public ImportarEstilosPDF()
        {
            InitializeComponent();
            //ImportarEstilosPDF_Load(this, EventArgs.Empty);

        }

        private List<FilaPO> filasExtraidas = new List<FilaPO>();
        private readonly Funciones f = new Funciones();
        private readonly ExtraerTexto extraerTexto = new ExtraerTexto();

        private estilo estiloImportar = new estilo();

        //private void ImportarEstilosPDF_Load(object sender, EventArgs e)
        //{
        //    // Llena el combo con las marcas/tipos existentes en tu tabla `tallas`
        //    cmbTipoMarca.Items.AddRange(new string[] {
        //    /*"LEVIS", "ZUMIES", "TARGET", "CINTAS", "BENS",*/
        //    "ROCKY"/*, "OLDNAVY", "SAFTECH", "WRANGLER", "IFR"*/
        //});
        //    cmbTipoMarca.SelectedIndex = 0;
        //}

        private void btnCargarPDF_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "PDF Files|*.pdf" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                filasExtraidas = extraerTexto.ExtraerFilasDetallePDF(ofd.FileName);

                foreach (var fila in filasExtraidas)
                {
                    fila.YaExiste = ConsultaUPCExiste(fila.Upc);
                }

                dgvPreview.DataSource = null;
                dgvPreview.DataSource = filasExtraidas;

                // Colorea las filas que ya existen para que el usuario las identifique
                foreach (DataGridViewRow row in dgvPreview.Rows)
                {
                    bool yaExiste = (bool)row.Cells["YaExiste"].Value;
                    row.DefaultCellStyle.BackColor = yaExiste ? Color.LightGray : Color.White;
                }

                lblTotalFilas.Text = $"Total filas: {filasExtraidas.Count} | Nuevas: {filasExtraidas.Count(x => !x.YaExiste)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al leer el PDF: " + ex.Message);
            }
        }




        public bool ConsultaUPCExiste(string upc)
        {
            using (BDDataContext db = new BDDataContext())
            {
                return db.tallas.Any(t => t.upc == upc);
            }
        }

        private void btnGuardarTallas_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(cmbTipoMarca.Text))
            //{
            //    MessageBox.Show("Favor de seleccionar el tipo de marca.");
            //    return;
            //}

            var filasNuevas = filasExtraidas.Where(x => !x.YaExiste).ToList();

            if (filasNuevas.Count == 0)
            {
                MessageBox.Show("No hay tallas nuevas para dar de alta.");
                return;
            }

            DialogResult confirmar = MessageBox.Show(
                $"Se darán de alta {filasNuevas.Count} tallas nuevas para el estilo(s) detectado(s) con marca 'ROCKY'. ¿Continuar?",
                "Confirmar alta", MessageBoxButtons.YesNo);

            if (confirmar != DialogResult.Yes) return;
            int guardadas = 0;
            foreach (var fila in filasNuevas)
            {

                tallas nuevaTalla = new tallas
                {
                    size_izquierdo = !string.IsNullOrEmpty(fila.Dimension) ? fila.Dimension.Trim() : fila.Size.Trim(),
                    size_derecho = !string.IsNullOrEmpty(fila.Dimension) ? fila.Size.Trim() : "",
                    upc = fila.Upc,
                    tipo = "ROCKY",
                    estilo = fila.Estilo.Trim(),
                    color = fila.ColorDescripcion.Trim(),
                    descripcion = fila.DescripcionEstilo.Trim() + ' ' + fila.Descripcion2.Trim(),
                    // descripcion2 =    // <-- aquí queda CAVENDERS o vacío
                };

                estiloImportar = new estilo
                {
                    estilo1 = fila.Estilo.Trim(),
                    tipo = "ROCKY",
                    color = fila.ColorDescripcion,
                    descripcion = fila.DescripcionEstilo,
                    descripcion2 = fila.Descripcion2,
                    cantidad = fila.Cantidad.ToString()
                };

                f.GuardaTallaEstilos(nuevaTalla);


                guardadas++;
            }
            f.GuardaEstilos(estiloImportar);

            MessageBox.Show($"Se dieron de alta {guardadas} tallas nuevas correctamente.");
            btnCargarPDF_Click(sender, e); // opcional: recarga para refrescar cuáles ya existen
        }





    }
}
