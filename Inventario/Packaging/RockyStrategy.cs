using System;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class RockyStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para ROCKY.");
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

            EtiquetaZUMIES clase = new EtiquetaZUMIES
            {
                po = context.PoNumero,
                poItem = "66",
                ProductCode = context.Estilo,
                Size = context.Talla,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                upc = context.UpcEscaneado,
                Cantidad = context.CantidadActual,
                Carton = context.CartonNumber,
                usuario = context.NombreUsuario,
                TipoCarton = context.Color,
                COUNTRY = "MEXICO",
                Fecha = DateTime.Now
            };

            context.CartonNumber = context.Funciones.GuardaInventarioZumies(clase, context.IdUsuario);
            GenerarEtiqueta(context);
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            context.TextoCodigoGenerado = "*" + context.CartonNumber.ToString() + "*";
            context.CodigoBarrasGenerado = null;
        }
    }
}