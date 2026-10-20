using Inventario.Reportes;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class IfrStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para IFR.");
                return false;
            }

            context.CantidadActual += 1;
            return true;
        }

        public void ProcesarCajaIncompleta(PackagingContext context)
        {
            if (context.Funciones == null)
            {
                MessageBox.Show("No se encontró el componente de acceso a datos.");
                return;
            }

            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
            {
                po = context.PoNumero,
                poItem = "22",
                ProductCode = context.Estilo,
                estilo = context.Estilo,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                upc = context.UpcEscaneado,
                Cantidad = context.CantidadActual,
                usuario = context.NombreUsuario,
                color = context.Color,
                itemDescription = context.ItemDescription,
                TipoCarton = "1",
                mixto = 0,
                Fecha = DateTime.Now
            };

            long idInventario = context.Funciones.GuardaInventario(clase, context.IdUsuario);
            clase.id_inventario = idInventario;

            GenerarEtiqueta(context);
            clase.codigoBarras = context.CodigoBarrasGenerado;

            if (context.ImprimirEtiqueta)
            {
                List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada> { clase };
                ReporteCajaIFR report = new ReporteCajaIFR { DataSource = listClase };
                report.PrintingSystem.ShowMarginsWarning = false;
                ReportPrintTool tool = new ReportPrintTool(report);
                tool.Print();
            }
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            context.CodigoBarrasGenerado = null;
            context.TextoCodigoGenerado = context.UpcEscaneado;
        }
    }
}