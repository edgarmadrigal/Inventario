using System;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class OldNavyStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para OLD NAVY.");
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
                po_varchar = context.PoNumero,
                poItem = "44",
                ProductCode = context.Estilo,
                upc = context.UpcEscaneado,
                Cantidad = 1,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                Size = context.SizeIzquierdo + " x " + context.SizeDerecho,
                usuario = context.NombreUsuario,
                Fecha = DateTime.Now
            };

            context.Funciones.GuardaInventario(clase, context.IdUsuario);
            GenerarEtiqueta(context);
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            context.TextoCodigoGenerado = "*" + context.CartonNumber.ToString() + "*";
            context.CodigoBarrasGenerado = null;
        }
    }
}