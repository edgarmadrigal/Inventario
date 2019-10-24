using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Inventario
{
    public partial class ReportesMicrosip : Form
    {
        Funciones f = new Funciones();
        public ReportesMicrosip()
        {
            InitializeComponent();
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscar_Click_1(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Buscar();
            Cursor.Current = Cursors.Default;
        }

        public void Buscar()
        {
            try
            {
                List<ConsultaAlmacenesResult> inv = f.ConsultaAlmacenes(dtpFechaInicio.Value.Date, dtpFechaFinal.Value.Date);

                gcReporte.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimir_Click_1(object sender, EventArgs e)
        {
            ShowGridPreview(gcReporte);
        }


        private void ShowGridPreview(DevExpress.XtraGrid.GridControl grid)
        {
            // Check whether or not the Grid Control can be printed. 
            if (!grid.IsPrintingAvailable)
            {
                MessageBox.Show("The 'DevExpress.XtraPrinting' Library is not found", "Error");
                return;
            }
            // Opens the Preview window. 

            PrintableComponentLink link = new PrintableComponentLink(new PrintingSystem());

            link.Component = grid;

            link.Landscape = true;

            link.ShowPreview();

        }

        private void btnBuscarDTL_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarDTL();
            Cursor.Current = Cursors.Default;
        }
        public void BuscarDTL()
        {
            try
            {
                List<ConsultaAlmacenesDTLResult> inv = f.ConsultaAlmacenesDTL(dtpFechaInicioDTL.Value.Date, dtpFechaFinalDTL.Value.Date);

                gcReporteDTL.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirDTL_Click(object sender, EventArgs e)
        {
            ShowGridPreview(gcReporteDTL);
        }

        private void btnBuscarTBG_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarTBG();
            Cursor.Current = Cursors.Default;
        }
        public void BuscarTBG()
        {
            try
            {
                List<ConsultaAlmacenesTBGResult> inv = f.ConsultaAlmacenesTBG(dtpFechaInicioTBG.Value.Date, dtpFechaFinTBG.Value.Date);

                gcReporteTBG1.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirTBG_Click(object sender, EventArgs e)
        {
            ShowGridPreview(gcReporteTBG1);
        }

        private void btnBuscarComprasAIN_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarComprasAIN();
            Cursor.Current = Cursors.Default;
        }
        public void BuscarComprasAIN()
        {
            try
            {
                List<ConsultaAlmacenesFolioComprasResult> inv = f.ConsultaComprasAIN(dtpFechaInicioComprasAIN.Value.Date, dtpFechaFinComprasAIN.Value.Date);

                gcComprasAIN.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirComprasAIN_Click(object sender, EventArgs e)
        {
            ShowGridPreview(gcComprasAIN);
        }

        private void btnBuscarComprasTBG_Click(object sender, EventArgs e)
        {
            try
            {
                List<ConsultaAlmacenesFolioComprasTBGResult> inv = f.ConsultaComprasTBG(dtpFechaInicioComprasTBG.Value.Date, dtpFechaFinComprasTBG.Value.Date);

                gcComprasTBG.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnImprimirComprasTBG_Click(object sender, EventArgs e)
        {
            ShowGridPreview(gcComprasTBG);

        }

        private void btnBuscarComprasDTL_Click(object sender, EventArgs e)
        {
            try
            {
                List<ConsultaAlmacenesFolioComprasDTLResult> inv = f.ConsultaComprasDTL(dtpFechaInicioComprasDTL.Value.Date, dtpFechaFinComprasDTL.Value.Date);

                gcCompraDTL.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnImprimirComprasDTL_Click(object sender, EventArgs e)
        {
            ShowGridPreview(gcCompraDTL);
        }
    }
}
