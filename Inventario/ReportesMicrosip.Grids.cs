using Inventario.Controles;

namespace Inventario
{
    public partial class ReportesMicrosip
    {
        /// <summary>Columnas, totales y agrupacion de los grids (antes en el disenador de DevExpress).</summary>
        private void ConfigurarGrids()
        {
            // gcReporte
            gcReporte.Columnas.Clear();
            gcReporte.Columnas.Add(new ColumnaGrid("FECHA", null, 76, 0, true));
            gcReporte.Columnas.Add(new ColumnaGrid("ORIGEN", null, 78, 1, true));
            gcReporte.Columnas.Add(new ColumnaGrid("DESTINO", null, 113, 2, true));
            gcReporte.Columnas.Add(new ColumnaGrid("CENTRO_DE_COSTO", null, 130, 10, true));
            gcReporte.Columnas.Add(new ColumnaGrid("DESCRIPCION", null, 113, 3, true));
            gcReporte.Columnas.Add(new ColumnaGrid("FOLIO", null, 113, 4, true));
            gcReporte.Columnas.Add(new ColumnaGrid("CLAVE_ARTICULO", null, 113, 5, true));
            gcReporte.Columnas.Add(new ColumnaGrid("NOMBRE", null, 113, 6, true));
            gcReporte.Columnas.Add(new ColumnaGrid("UNIDADES", null, 113, 7, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporte.Columnas.Add(new ColumnaGrid("COSTO_UNITARIO", null, 113, 8, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporte.Columnas.Add(new ColumnaGrid("COSTO_TOTAL", null, 113, 9, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporte.ResumenesGrupo.Clear();
            gcReporte.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "UNIDADES", "Unidades :{0}"));
            gcReporte.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "COSTO_UNITARIO", "Costo Unitario :{0}"));
            gcReporte.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "COSTO_TOTAL", "Costo Total :{0}"));
            gcReporte.Agrupar.Clear();

            // gcReporteDTL
            gcReporteDTL.Columnas.Clear();
            gcReporteDTL.Columnas.Add(new ColumnaGrid("FECHA", null, 76, 0, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("ORIGEN", null, 78, 1, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("DESTINO", null, 113, 2, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("CENTRO_DE_COSTO", null, 130, 10, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("DESCRIPCION", null, 113, 3, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("FOLIO", null, 113, 4, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("CLAVE_ARTICULO", null, 113, 5, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("NOMBRE", null, 113, 6, true));
            gcReporteDTL.Columnas.Add(new ColumnaGrid("UNIDADES", null, 113, 7, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporteDTL.Columnas.Add(new ColumnaGrid("COSTO_UNITARIO", null, 113, 8, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporteDTL.Columnas.Add(new ColumnaGrid("COSTO_TOTAL", null, 113, 9, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporteDTL.ResumenesGrupo.Clear();
            gcReporteDTL.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "UNIDADES", "Unidades :{0}"));
            gcReporteDTL.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "COSTO_UNITARIO", "Costo Unitario :{0}"));
            gcReporteDTL.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "COSTO_TOTAL", "Costo Total :{0}"));
            gcReporteDTL.Agrupar.Clear();

            // gcReporteTBG1
            gcReporteTBG1.Columnas.Clear();
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("FECHA", null, 73, 0, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("ORIGEN", null, 80, 1, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("DESTINO", null, 115, 2, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("CENTRO_DE_COSTO", null, 135, 10, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("DESCRIPCION", null, 115, 3, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("FOLIO", null, 115, 4, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("CLAVE_ARTICULO", null, 115, 5, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("NOMBRE", null, 126, 6, true));
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("UNIDADES", null, 111, 7, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("COSTO_UNITARIO", null, 111, 8, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporteTBG1.Columnas.Add(new ColumnaGrid("COSTO_TOTAL", null, 92, 9, true) { Resumen = TipoResumen.Suma, FormatoResumen = "SUM={0:0.##}" });
            gcReporteTBG1.ResumenesGrupo.Clear();
            gcReporteTBG1.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "UNIDADES", "Unidades :{0}"));
            gcReporteTBG1.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "COSTO_UNITARIO", "Costo Unitario :{0}"));
            gcReporteTBG1.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "COSTO_TOTAL", "Costo Total :{0}"));
            gcReporteTBG1.Agrupar.Clear();

            // gcComprasAIN
            gcComprasAIN.Columnas.Clear();
            gcComprasAIN.Columnas.Add(new ColumnaGrid("FECHA", null, 75, 0, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("Folio", null, 75, 1, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("MONEDA", null, 75, 2, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("Proveedor", null, 75, 3, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("Almacen", null, 75, 4, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("DESCRIPCION", null, 75, 5, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("Unidades", null, 75, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Total : {0:0.##}" });
            gcComprasAIN.Columnas.Add(new ColumnaGrid("IMPORTE_NETO", null, 75, 7, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Suma : {0}" });
            gcComprasAIN.Columnas.Add(new ColumnaGrid("FOLIO_DE_EMBARQUE", null, 75, 8, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("RFC_CURP", null, 75, 9, true));
            gcComprasAIN.Columnas.Add(new ColumnaGrid("FOLIO_PROV", null, 75, 10, true));
            gcComprasAIN.ResumenesGrupo.Clear();
            gcComprasAIN.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "IMPORTE_NETO", "Suma {0}"));
            gcComprasAIN.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "Folio", "Total: {0}"));
            gcComprasAIN.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "Unidades", "Total : {0:0.##}"));
            gcComprasAIN.Agrupar.Clear();

            // gcCompraDTL
            gcCompraDTL.Columnas.Clear();
            gcCompraDTL.Columnas.Add(new ColumnaGrid("FECHA", null, 75, 0, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("Folio", null, 75, 1, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("MONEDA", null, 75, 2, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("Proveedor", null, 75, 3, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("Almacen", null, 75, 4, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("DESCRIPCION", null, 75, 5, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("Unidades", null, 75, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Total : {0:0.##}" });
            gcCompraDTL.Columnas.Add(new ColumnaGrid("IMPORTE_NETO", null, 75, 7, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Suma : {0}" });
            gcCompraDTL.Columnas.Add(new ColumnaGrid("FOLIO_DE_EMBARQUE", null, 75, 8, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("RFC_CURP", null, 75, 9, true));
            gcCompraDTL.Columnas.Add(new ColumnaGrid("FOLIO_PROV", null, 75, 10, true));
            gcCompraDTL.ResumenesGrupo.Clear();
            gcCompraDTL.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "IMPORTE_NETO", "Suma {0}"));
            gcCompraDTL.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "Folio", "Total: {0}"));
            gcCompraDTL.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "Unidades", "Total : {0:0.##}"));
            gcCompraDTL.Agrupar.Clear();

            // gcComprasTBG
            gcComprasTBG.Columnas.Clear();
            gcComprasTBG.Columnas.Add(new ColumnaGrid("FECHA", null, 75, 0, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("Folio", null, 75, 1, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("MONEDA", null, 75, 2, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("Proveedor", null, 75, 3, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("Almacen", null, 75, 4, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("DESCRIPCION", null, 75, 5, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("Unidades", null, 75, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Total : {0:0.##}" });
            gcComprasTBG.Columnas.Add(new ColumnaGrid("IMPORTE_NETO", null, 75, 7, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Suma : {0}" });
            gcComprasTBG.Columnas.Add(new ColumnaGrid("FOLIO_DE_EMBARQUE", null, 75, 8, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("RFC_CURP", null, 75, 9, true));
            gcComprasTBG.Columnas.Add(new ColumnaGrid("FOLIO_PROV", null, 75, 10, true));
            gcComprasTBG.ResumenesGrupo.Clear();
            gcComprasTBG.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "IMPORTE_NETO", "Suma {0}"));
            gcComprasTBG.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "Folio", "Total: {0}"));
            gcComprasTBG.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "Unidades", "Total : {0:0.##}"));
            gcComprasTBG.Agrupar.Clear();
        }
    }
}
