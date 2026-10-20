using System;
using System.Collections;
using System.Windows.Forms;

namespace Inventario.Reportes
{
    /// <summary>
    /// Etiqueta/reporte definido en un archivo .rdlc (carpeta Reportes). Conserva la forma de uso que tenian los
    /// reportes de DevExpress: new ReporteCaja { DataSource = lista } + ReportPrintTool.
    /// </summary>
    public abstract class ReporteRdlc
    {
        protected ReporteRdlc(string nombre) { Nombre = nombre; }

        /// <summary>Nombre del archivo .rdlc sin extension.</summary>
        public string Nombre { get; private set; }

        /// <summary>Lista de objetos a imprimir (una etiqueta por elemento).</summary>
        public object DataSource { get; set; }

        /// <summary>Imprime la marca "R" de reimpresion.</summary>
        public bool Reimpresion { get; set; }

        /// <summary>Compatibilidad con el codigo que venia de DevExpress (no tiene efecto).</summary>
        public OpcionesImpresion PrintingSystem { get; } = new OpcionesImpresion();

        internal IEnumerable Datos
        {
            get { return DataSource as IEnumerable ?? (DataSource != null ? new[] { DataSource } : new object[0]); }
        }
    }

    public class OpcionesImpresion
    {
        public bool ShowMarginsWarning { get; set; }
    }

    /// <summary>Imprime o muestra un ReporteRdlc (mismos metodos que usaba el codigo con DevExpress).</summary>
    public class ReportPrintTool
    {
        private readonly ReporteRdlc reporte;

        public ReportPrintTool(ReporteRdlc reporte) { this.reporte = reporte; }

        /// <summary>
        /// Modo "solo vista previa". Si se prende antes de llamar Print(), el reporte NO sale a la impresora:
        /// abre la vista previa marcando el reporte como reimpresion. Se usa desde el boton "Vista Previa" del
        /// tab Reimpresion para reutilizar la misma logica por marca que usa el boton "Reimprimir" sin tener
        /// que duplicar las ramas por po_item. Es un flag por hilo para que no afecte otras ventanas.
        /// </summary>
        [ThreadStatic] private static bool _modoVistaPreviaReimpresion;
        [ThreadStatic] private static bool _vistaPreviaYaMostrada;
        public static bool ModoVistaPreviaReimpresion
        {
            get { return _modoVistaPreviaReimpresion; }
            set { _modoVistaPreviaReimpresion = value; _vistaPreviaYaMostrada = false; }
        }

        /// <summary>En modo preview abre el visor una sola vez (muchas marcas llaman Print varias veces
        /// para imprimir copias: no queremos abrir una ventana por cada copia).</summary>
        private bool AbrirPreviewUnaVez()
        {
            if (!_modoVistaPreviaReimpresion) return false;
            if (!_vistaPreviaYaMostrada) { _vistaPreviaYaMostrada = true; reporte.Reimpresion = true; ShowPreview(); }
            return true;
        }

        /// <summary>Imprime en la impresora predeterminada de Windows.</summary>
        public void Print() { Print(null); }

        /// <summary>Imprime en la impresora indicada.</summary>
        public void Print(string impresora)
        {
            if (AbrirPreviewUnaVez()) return;
            ImpresionRdlc.Imprimir(reporte.Nombre, reporte.Datos, impresora, 1, reporte.Reimpresion);
        }

        /// <summary>Imprime varias copias en un solo trabajo.</summary>
        public void Print(string impresora, int copias)
        {
            if (AbrirPreviewUnaVez()) return;
            ImpresionRdlc.Imprimir(reporte.Nombre, reporte.Datos, impresora, copias, reporte.Reimpresion);
        }

        public void ShowPreview() { ImpresionRdlc.VistaPrevia(reporte.Nombre, reporte.Datos, null, reporte.Reimpresion); }

        public void ShowPreviewDialog() { ShowPreview(); }

        public void ShowRibbonPreviewDialog() { ShowPreview(); }

        /// <summary>Pregunta la impresora y luego imprime.</summary>
        public void PrintDialog()
        {
            using (var dlg = new PrintDialog { UseEXDialog = true })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    ImpresionRdlc.Imprimir(reporte.Nombre, reporte.Datos, dlg.PrinterSettings.PrinterName, dlg.PrinterSettings.Copies, reporte.Reimpresion);
            }
        }
    }
}
