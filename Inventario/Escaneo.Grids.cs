using Inventario.Controles;

namespace Inventario
{
    public partial class Escaneo
    {
        /// <summary>Columnas, totales y agrupacion de los grids (antes en el disenador de DevExpress).</summary>
        private void ConfigurarGrids()
        {
            // gcReporte
            gcReporte.Columnas.Clear();
            gcReporte.Columnas.Add(new ColumnaGrid("ID", null, 55, 1, true) { Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcReporte.Columnas.Add(new ColumnaGrid("PO", null, 74, 0, true) { Fija = true, Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcReporte.Columnas.Add(new ColumnaGrid("POITEM", null, 50, 999, false));
            gcReporte.Columnas.Add(new ColumnaGrid("CANTIDAD", null, 101, 13, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Prendas :{0}" });
            gcReporte.Columnas.Add(new ColumnaGrid("SIZE", null, 52, 2, true));
            gcReporte.Columnas.Add(new ColumnaGrid("UPC", null, 74, 3, true));
            gcReporte.Columnas.Add(new ColumnaGrid("CARTONNUMBER", null, 143, 5, true));
            gcReporte.Columnas.Add(new ColumnaGrid("PRODUCTCODE", null, 84, 4, true));
            gcReporte.Columnas.Add(new ColumnaGrid("CARTONTYPE", null, 79, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Cajas Prepack: {0}" });
            gcReporte.Columnas.Add(new ColumnaGrid("CREATEDATE", null, 137, 7, true) { Formato = "dd-MMMM-yyyy hh:mm tt" });
            gcReporte.Columnas.Add(new ColumnaGrid("USERCREATE", null, 98, 8, true));
            gcReporte.Columnas.Add(new ColumnaGrid("CLIENTE", null, 30, 9, true));
            gcReporte.Columnas.Add(new ColumnaGrid("FACTURA", null, 39, 10, true));
            gcReporte.Columnas.Add(new ColumnaGrid("TERMINADO", null, 63, 11, true));
            gcReporte.Columnas.Add(new ColumnaGrid("codigodeBarras", null, 75, 12, true));
            gcReporte.Columnas.Add(new ColumnaGrid("MARCA", "Marca", 80, 0, true));
            gcReporte.ResumenesGrupo.Clear();
            gcReporte.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CANTIDAD", "Prendas :{0}"));
            gcReporte.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "PO", "Cajas :{0}"));
            gcReporte.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CARTONTYPE", "Cajas Prepack :{0}"));
            gcReporte.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "ID", "Cajas :{0}"));
            gcReporte.Agrupar.Clear();
            gcReporte.OrdenarPor("USERCREATE", true);

            // gcModulo
            gcModulo.Columnas.Clear();
            gcModulo.Columnas.Add(new ColumnaGrid("ID", null, 84, 1, true) { Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcModulo.Columnas.Add(new ColumnaGrid("PO", null, 84, 0, true) { Fija = true, Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcModulo.Columnas.Add(new ColumnaGrid("POITEM", null, 50, 999, false));
            gcModulo.Columnas.Add(new ColumnaGrid("CANTIDAD", null, 101, 11, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Prendas :{0}" });
            gcModulo.Columnas.Add(new ColumnaGrid("SIZE", null, 50, 2, true));
            gcModulo.Columnas.Add(new ColumnaGrid("UPC", null, 71, 3, true));
            gcModulo.Columnas.Add(new ColumnaGrid("CARTONNUMBER", null, 137, 5, true));
            gcModulo.Columnas.Add(new ColumnaGrid("PRODUCTCODE", null, 80, 4, true));
            gcModulo.Columnas.Add(new ColumnaGrid("CARTONTYPE", null, 77, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Cajas Prepack: {0}" });
            gcModulo.Columnas.Add(new ColumnaGrid("CREATEDATE", null, 130, 7, true) { Formato = "dd-MMMM-yyyy hh:mm tt" });
            gcModulo.Columnas.Add(new ColumnaGrid("USERCREATE", null, 88, 8, true));
            gcModulo.Columnas.Add(new ColumnaGrid("CLIENTE", null, 29, 8, true));
            gcModulo.Columnas.Add(new ColumnaGrid("FACTURA", null, 38, 9, true));
            gcModulo.Columnas.Add(new ColumnaGrid("TERMINADO", null, 50, 10, true));
            gcModulo.Columnas.Add(new ColumnaGrid("MARCA", "Marca", 80, 0, true));
            gcModulo.ResumenesGrupo.Clear();
            gcModulo.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CANTIDAD", "Prendas :{0}"));
            gcModulo.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "PO", "Cajas :{0}"));
            gcModulo.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CARTONTYPE", "Cajas Prepack :{0}"));
            gcModulo.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "ID", "Cajas :{0}"));
            gcModulo.Agrupar.Clear();
            gcModulo.Agrupar.Add("USERCREATE");

            // gcPO
            gcPO.Columnas.Clear();
            gcPO.Columnas.Add(new ColumnaGrid("ID", null, 62, 0, true) { Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcPO.Columnas.Add(new ColumnaGrid("PO", null, 84, 0, true) { Fija = true, Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcPO.Columnas.Add(new ColumnaGrid("POITEM", null, 50, 999, false));
            gcPO.Columnas.Add(new ColumnaGrid("CANTIDAD", null, 101, 11, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Prendas :{0}" });
            gcPO.Columnas.Add(new ColumnaGrid("SIZE", null, 47, 1, true));
            gcPO.Columnas.Add(new ColumnaGrid("UPC", null, 67, 2, true));
            gcPO.Columnas.Add(new ColumnaGrid("CARTONNUMBER", null, 128, 4, true));
            gcPO.Columnas.Add(new ColumnaGrid("PRODUCTCODE", null, 75, 3, true));
            gcPO.Columnas.Add(new ColumnaGrid("CARTONTYPE", null, 72, 5, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Cajas Prepack: {0}" });
            gcPO.Columnas.Add(new ColumnaGrid("CREATEDATE", null, 121, 6, true) { Formato = "dd-MMMM-yyyy hh:mm tt" });
            gcPO.Columnas.Add(new ColumnaGrid("USERCREATE", null, 88, 7, true));
            gcPO.Columnas.Add(new ColumnaGrid("CLIENTE", null, 28, 8, true));
            gcPO.Columnas.Add(new ColumnaGrid("FACTURA", null, 36, 9, true));
            gcPO.Columnas.Add(new ColumnaGrid("TERMINADO", null, 35, 10, true));
            gcPO.Columnas.Add(new ColumnaGrid("MARCA", "Marca", 80, 0, true));
            gcPO.ResumenesGrupo.Clear();
            gcPO.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CANTIDAD", "Prendas :{0}"));
            gcPO.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "PO", "Cajas :{0}"));
            gcPO.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CARTONTYPE", "Cajas Prepack :{0}"));
            gcPO.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "ID", "Cajas :{0}"));
            gcPO.Agrupar.Clear();
            gcPO.Agrupar.Add("PO");
            gcPO.OrdenarPor("USERCREATE", true);

            // gcPOTallasSeparadas
            gcPOTallasSeparadas.Columnas.Clear();
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("ID", null, 62, 0, true) { Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("PO", null, 84, 0, true) { Fija = true, Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas: {0}" });
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("POITEM", null, 50, 999, false));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("CANTIDAD", null, 101, 12, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Prendas :{0}" });
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("SIZE1", null, 75, 2, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("SIZE2", null, 75, 3, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("UPC", null, 67, 1, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("CARTONNUMBER", null, 128, 5, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("PRODUCTCODE", null, 75, 4, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("CARTONTYPE", null, 72, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Cajas Prepack: {0}" });
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("CREATEDATE", null, 121, 7, true) { Formato = "dd-MMMM-yyyy hh:mm tt" });
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("USERCREATE", null, 88, 8, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("CLIENTE", null, 28, 9, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("FACTURA", null, 36, 10, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("TERMINADO", null, 35, 11, true));
            gcPOTallasSeparadas.Columnas.Add(new ColumnaGrid("MARCA", "Marca", 80, 0, true));
            gcPOTallasSeparadas.ResumenesGrupo.Clear();
            gcPOTallasSeparadas.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CANTIDAD", "Prendas :{0}"));
            gcPOTallasSeparadas.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "PO", "Cajas :{0}"));
            gcPOTallasSeparadas.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "CARTONTYPE", "Cajas Prepack :{0}"));
            gcPOTallasSeparadas.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "ID", "Cajas :{0}"));
            gcPOTallasSeparadas.Agrupar.Clear();
            gcPOTallasSeparadas.Agrupar.Add("PO");
            gcPOTallasSeparadas.OrdenarPor("USERCREATE", true);

            // gcReporteAlmacen
            gcReporteAlmacen.Columnas.Clear();
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("RACK", null, 34, 0, true));
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("Nivel", null, 44, 1, true));
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("PO", null, 44, 2, true));
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("Cliente", null, 31, 3, true));
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("Facturacion", null, 33, 4, true));
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("Terminado", null, 35, 5, true));
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("Cantidad", null, 60, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "Prendas :{0}" });
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("Talla", null, 46, 7, true));
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("idCaja", null, 56, 8, true) { Resumen = TipoResumen.Conteo, FormatoResumen = "Cajas :{0}" });
            gcReporteAlmacen.Columnas.Add(new ColumnaGrid("Fecha", null, 108, 9, true) { Formato = "dd-MMMM-yyyy hh:mm tt" });
            gcReporteAlmacen.ResumenesGrupo.Clear();
            gcReporteAlmacen.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "Cantidad", "Prendas :{0}"));
            gcReporteAlmacen.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "idCaja", "Cajas :{0}"));
            gcReporteAlmacen.Agrupar.Clear();

            // dgReporteEmbarques
            dgReporteEmbarques.Columnas.Clear();
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("RACK", null, 91, 0, true));
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("Nivel", null, 91, 1, true));
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("PO", null, 91, 2, true));
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("Cliente", null, 48, 3, true));
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("Facturacion", null, 54, 4, true));
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("Terminado", null, 59, 5, true));
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("Cantidad", null, 116, 6, true) { Resumen = TipoResumen.Suma, FormatoResumen = "{0}" });
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("Talla", null, 116, 7, true));
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("idCaja", null, 116, 8, true) { Resumen = TipoResumen.Conteo, FormatoResumen = "{0}" });
            dgReporteEmbarques.Columnas.Add(new ColumnaGrid("Fecha", null, 131, 9, true) { Formato = "dd-MMMM-yyyy hh:mm tt" });
            dgReporteEmbarques.ResumenesGrupo.Clear();
            dgReporteEmbarques.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "Cantidad", "Prendas :{0}"));
            dgReporteEmbarques.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "idCaja", "Cajas :{0}"));
            dgReporteEmbarques.Agrupar.Clear();

            // dgReporteUbicacion
            dgReporteUbicacion.Columnas.Clear();
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("RACK", null, 76, 0, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("TARIMA", null, 76, 1, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("Nivel", null, 76, 2, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("idCaja", null, 56, 3, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("PO", null, 77, 4, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("ProductCode", null, 77, 5, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("Cliente", null, 48, 8, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("Facturacion", null, 48, 9, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("Terminado", null, 48, 10, true));
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("Cantidad", null, 124, 7, true) { Resumen = TipoResumen.Suma, FormatoResumen = "PRENDAS: {0}" });
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("Talla", null, 115, 6, true) { Resumen = TipoResumen.Conteo, FormatoResumen = "CAJAS: {0}" });
            dgReporteUbicacion.Columnas.Add(new ColumnaGrid("FechaEntrada", null, 92, 11, true));
            dgReporteUbicacion.ResumenesGrupo.Clear();
            dgReporteUbicacion.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "Talla", "Cajas: {0}"));
            dgReporteUbicacion.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "Cantidad", "PRENDAS: {0}"));
            dgReporteUbicacion.Agrupar.Clear();

            // dgReporteUbicacionID
            dgReporteUbicacionID.Columnas.Clear();
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("RACK", null, 75, 0, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("Nivel", null, 75, 1, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("idCaja", null, 75, 2, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("PO", null, 75, 3, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("Cliente", null, 75, 6, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("Facturacion", null, 75, 7, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("Terminado", null, 75, 8, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("Cantidad", null, 75, 5, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("Talla", null, 75, 4, true));
            dgReporteUbicacionID.Columnas.Add(new ColumnaGrid("FechaEntrada", null, 75, 9, true));
            dgReporteUbicacionID.ResumenesGrupo.Clear();
            dgReporteUbicacionID.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Conteo, "Talla", "Cajas: {0}"));
            dgReporteUbicacionID.ResumenesGrupo.Add(new ResumenGrupo(TipoResumen.Suma, "Cantidad", "PRENDAS: {0}"));
            dgReporteUbicacionID.Agrupar.Clear();
        }
    }
}
