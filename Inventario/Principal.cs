using System;
using System.Windows.Forms;

namespace Inventario
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
            IsMdiContainer = true;
        }

        private void eSCANEOToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            Escaneo et = new Escaneo();
            et.MdiParent = this;
            // et.Dock = DockStyle.Fill; ///MAXIMIZADO POR DEFAULT
            et.Show();
        }

        private void sALIRToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Seguro que deseas Cerrar el programa", "Cerrar", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
                Application.Exit();


            }
            else if (dialogResult == DialogResult.No)
            {

                //do something else
            }
        }
    }
}
