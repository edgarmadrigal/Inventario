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

        // ---- Excel de Ben Davis (packing list por PO) en la pestana Reporte PO ----
        private ComboBox cmbPOExcelBENS;

        private void PrepararExcelBENS()
        {
            int x = btnImprimirReportePO.Right + 20;
            var lbl = new Label { Text = "PO Ben Davis:", AutoSize = true, Location = new System.Drawing.Point(x, 2) };
            cmbPOExcelBENS = new ComboBox
            {
                Location = new System.Drawing.Point(x, 18),
                Width = 170,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };
            // La lista de POs se carga la primera vez que se abre
            cmbPOExcelBENS.DropDown += (s, e) =>
            {
                if (cmbPOExcelBENS.Items.Count == 0)
                {
                    string texto = cmbPOExcelBENS.Text;
                    try { f.ConsultaPO(cmbPOExcelBENS); } catch { }
                    cmbPOExcelBENS.Text = texto;
                }
            };
            var btn = new Button
            {
                Text = "EXCEL BEN DAVIS",
                Location = new System.Drawing.Point(x, 46),
                Size = new System.Drawing.Size(128, 32),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            btn.Click += (s, e) => ExportarExcelBENS();
            var btnSummary = new Button
            {
                Text = "SUMMARY BEN DAVIS",
                Location = new System.Drawing.Point(x + 134, 46),
                Size = new System.Drawing.Size(140, 32),
                Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold)
            };
            btnSummary.Click += (s, e) => ExportarSummaryBENS();
            tpReportePO.Controls.AddRange(new Control[] { lbl, cmbPOExcelBENS, btn, btnSummary });
            lbl.BringToFront(); cmbPOExcelBENS.BringToFront(); btn.BringToFront(); btnSummary.BringToFront();
        }

        private void ExportarExcelBENS()
        {
            string po = cmbPOExcelBENS.Text.Trim();
            if (po == "")
            {
                MessageBox.Show("Escribe o selecciona la PO de Ben Davis.", "Excel Ben Davis");
                return;
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                List<Funciones.PackingBENS> filas = f.ReportePackingBENS(po);
                Cursor.Current = Cursors.Default;
                if (filas.Count == 0)
                {
                    MessageBox.Show("La PO " + po + " no tiene cajas escaneadas de Ben Davis.", "Excel Ben Davis");
                    return;
                }

                string poAI = filas[0].PO;
                using (var dlg = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = poAI + " Ben Davis " + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx" })
                {
                    if (dlg.ShowDialog(this) != DialogResult.OK) return;
                    GuardarExcelBENS(filas, poAI, dlg.FileName);

                    if (MessageBox.Show("Se guardó el Excel con " + filas.Select(r => r.idCaja).Distinct().Count() + " caja(s) y " +
                        filas.Sum(r => r.Qty) + " prenda(s).\r\n\r\n¿Abrirlo?", "Excel Ben Davis", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(dlg.FileName);
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message);
            }
        }

        // Mismo acomodo que el packing list de Ben Davis: PO#, Barcode, Box #, Style, Waist, Length, SIZE, QTY.
        // En cajas mixtas el Box # va solo en la primera fila de la caja.
        private static void GuardarExcelBENS(List<Funciones.PackingBENS> filas, string poAI, string ruta)
        {
            using (var libro = new ClosedXML.Excel.XLWorkbook())
            {
                var hoja = libro.Worksheets.Add(poAI.Length > 31 ? poAI.Substring(0, 31) : poAI);
                string[] titulos = { "PO#", "Barcode", "Box #", "Style", "Waist", "Length", "SIZE", "QTY" };
                for (int c = 0; c < titulos.Length; c++) hoja.Cell(1, c + 1).Value = titulos[c];

                int fila = 2, cajaAnterior = -1;
                foreach (var r in filas)
                {
                    hoja.Cell(fila, 1).Value = r.PO;
                    hoja.Cell(fila, 2).Value = r.Barcode;
                    if (r.idCaja != cajaAnterior) hoja.Cell(fila, 3).Value = r.Box;
                    PonerNumeroOTexto(hoja.Cell(fila, 4), r.Style);
                    PonerNumeroOTexto(hoja.Cell(fila, 5), r.Waist);
                    PonerNumeroOTexto(hoja.Cell(fila, 6), r.Length);
                    hoja.Cell(fila, 7).Value = r.Size;
                    hoja.Cell(fila, 8).Value = r.Qty;
                    cajaAnterior = r.idCaja;
                    fila++;
                }

                var tabla = hoja.Range(1, 1, fila - 1, titulos.Length);
                tabla.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                tabla.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                tabla.Style.Font.FontName = "Calibri";
                tabla.Style.Font.FontSize = 11;
                hoja.Range(1, 3, fila - 1, titulos.Length).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                var encabezado = hoja.Range(1, 1, 1, titulos.Length);
                encabezado.Style.Font.Bold = true;
                encabezado.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                hoja.Cell(1, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Left;
                tabla.SetAutoFilter();
                hoja.SheetView.FreezeRows(1);
                hoja.Columns(1, titulos.Length).AdjustToContents();
                hoja.Column(2).Width = Math.Max(hoja.Column(2).Width, 26);
                for (int c = 3; c <= titulos.Length; c++) hoja.Column(c).Width = Math.Max(hoja.Column(c).Width, 12);

                libro.SaveAs(ruta);
            }
        }

        private static void PonerNumeroOTexto(ClosedXML.Excel.IXLCell celda, string valor)
        {
            int n;
            if (int.TryParse(valor, out n)) celda.Value = n; else celda.Value = valor;
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
