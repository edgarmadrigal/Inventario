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
    // Pestanas de reportes: inventario, PO, modulos, tallas separadas, almacen, embarques y ubicacion
    public partial class Escaneo
    {
        // ---- filtro por marca (se arma en codigo para no tocar el disenador) ----
        private ComboBox cmbMarcaReporte, cmbMarcaPO, cmbMarcaModulos, cmbMarcaTallas;

        private void PrepararFiltrosMarca()
        {
            List<string> marcas;
            try { marcas = f.ConsultaMarcasReporte(); } catch { marcas = new List<string>(); }
            cmbMarcaReporte = FiltroMarca(tpReportes, btnBuscar, marcas, () => Buscar());
            cmbMarcaPO = FiltroMarca(tpReportePO, btnActualizaPO, marcas, () => BuscarPOReporte());
            cmbMarcaModulos = FiltroMarca(tbpReporteModulo, btnActualizaPOS, marcas, () => BuscarModuloReporte());
            cmbMarcaTallas = FiltroMarca(tpPOTallasSeparadas, btnConsultaTallasSeparadas, marcas, () => BuscarPOTallasSeparadas());
        }

        private ComboBox FiltroMarca(Control pestana, Control botonBuscar, List<string> marcas, Action buscar)
        {
            int x = botonBuscar.Right + 8;
            var lbl = new Label { Text = "Marca:", AutoSize = true, Location = new System.Drawing.Point(x, botonBuscar.Top - 2) };
            var cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, Location = new System.Drawing.Point(x, botonBuscar.Top + 14) };
            cmb.Items.Add("(todas)");
            foreach (string m in marcas) cmb.Items.Add(m);
            cmb.SelectedIndex = 0;
            // al cambiar la marca se vuelve a buscar con las mismas fechas
            cmb.SelectionChangeCommitted += (s, e) => { Cursor.Current = Cursors.WaitCursor; buscar(); Cursor.Current = Cursors.Default; };
            pestana.Controls.Add(lbl);
            pestana.Controls.Add(cmb);
            lbl.BringToFront(); cmb.BringToFront();
            return cmb;
        }

        private static string MarcaFiltro(ComboBox cmb)
        {
            return cmb == null || cmb.SelectedIndex <= 0 ? null : cmb.Text;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Buscar();
            Cursor.Current = Cursors.Default;
        }

        public void Buscar()
        {
            try
            {
                List<ConsultaInventarioResult> inv = f.ConsultaInventario(dtpFechaInicio.Value.Date, dtpFechaFinal.Value.Date, MarcaFiltro(cmbMarcaReporte));
                gcReporte.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ShowGridPreview(global::Inventario.Controles.GridReporte grid)
        {
            grid.VistaPrevia(grid.Titulo);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            ShowGridPreview(gcReporte);
        }

        private void btnBuscarAlmacen_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                BuscarReporteAlmacen();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BuscarReporteAlmacen()
        {
            try
            {
                List<ubicacion_ReporteAlmacen_ConsultaResult> inv = f.ConsultaAlmacen(dtpFechaInicioAlmacen.Value.Date, dtpFechaFinalAlmacen.Value.Date);
                gcReporteAlmacen.DataSource = inv;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBuscarEmbarques_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (!iniciando)
                {
                    ConsultaReporteEmbarques();
                }
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ConsultaReporteEmbarques()
        {
            try
            {
                List<ubicacion_ReporteEmbarques_ConsultaResult> inv = f.ConsultaEmbarques(dtpFechaInicioEmbarques.Value.Date, dtpFechaFinalEmbarques.Value.Date);
                dgReporteEmbarques.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReporteDiario_Click(object sender, EventArgs e)
        {
            List<ConsultaInventarioPorHoraResult> inv = f.ConsultaInventarioPorHora(dtpReporteDiario.Value.Date, dtpReporteDiario.Value.Date);

            ReporteDiario report2 = new ReporteDiario
            {
                DataSource = inv
            };
            report2.PrintingSystem.ShowMarginsWarning = false;
            ReportPrintTool tool2 = new ReportPrintTool(report2);
            tool2.ShowPreview();
            //tool2.Print();


        }

        private void btnReporteUbicacion_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                BuscarReporteUbicacion();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BuscarReporteUbicacion()
        {
            try
            {
                List<ubicacion_Entrada_ConsultaUbicacionDetalleResult> inv = f.ConsultaUbicacion();
                dgReporteUbicacion.DataSource = inv;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirUbicacion_Click(object sender, EventArgs e)
        {
            ShowGridPreview(dgReporteUbicacion);
        }

        private void btnBuscarUbicacion_Click(object sender, EventArgs e)
        {
            try
            {
                consultaUbicacionID();
                txtIDCaja.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void consultaUbicacionID()
        {
            try
            {
                int ubicacionID = Convert.ToInt32(txtUbicacionReporte.Text);
                if (ubicacionID > 0)
                {
                    List<ubicacion_Entrada_ConsultaUbicacionIDResult> seccionid = f.ConsultaUbicacionID(ubicacionID);
                    if (seccionid.Count > 0)
                    {
                        List<ubicacion_Entrada_ConsultaUbicacionDetalleIDResult> ubicacionDetalle = f.ConsultaUbicacionDetalleID(ubicacionID);
                        dgReporteUbicacionID.DataSource = ubicacionDetalle;
                        //dgReporteUbicacion.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtUbicacionReporte_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    consultaUbicacionID();
                    txtIDCaja.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirUbicacionR_Click(object sender, EventArgs e)
        {
            ShowGridPreview(dgReporteUbicacionID);
        }


        public void BuscarPOReporte()
        {

            try
            {
                List<ConsultaInventarioResult> inv = f.ConsultaInventario(dtpFechaInicioPO.Value.Date, dtpFechaFinPO.Value.Date, MarcaFiltro(cmbMarcaPO));

                gcPO.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void BuscarPOTallasSeparadas()
        {

            try
            {
                List<ConsultaInventarioTallaSeparadaResult> inv = f.ConsultaInventarioTallasSeparadas(dtpFechaInicioPOTallasSeparadas.Value.Date, dtpFechaFinPOTallasSeparadas.Value.Date, MarcaFiltro(cmbMarcaTallas));

                gcPOTallasSeparadas.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimirReportePO_Click(object sender, EventArgs e)
        {

            ShowGridPreview(gcPO);
        }

        private void btnActualizaPOS_Click(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;
            BuscarModuloReporte();
            Cursor.Current = Cursors.Default;
        }


        public void BuscarModuloReporte()
        {

            try
            {
                List<ConsultaInventarioResult> inv = f.ConsultaInventario(dtpFechaInicioModulos.Value.Date, dtpFechaFinModulos.Value.Date, MarcaFiltro(cmbMarcaModulos));

                gcModulo.DataSource = inv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImprimeModulos_Click(object sender, EventArgs e)
        {

            ShowGridPreview(gcModulo);
        }

        private void btnActualizaPO_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarPOReporte();
            Cursor.Current = Cursors.Default;
        }

        private void btnConsultaTallasSeparadas_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            BuscarPOTallasSeparadas();
            Cursor.Current = Cursors.Default;
        }
    }
}
