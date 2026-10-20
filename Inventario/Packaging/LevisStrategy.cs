using BarcodeLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class LevisStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para LEVIS.");
                return false;
            }

            if (context.CantidadActual >= context.CantidadRequerida && context.CantidadRequerida > 0)
            {
                MessageBox.Show("Ya se completó la cantidad requerida para este carton de LEVIS.");
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
                poItem = context.ProfileCode,
                ProductCode = context.Estilo,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                Size = context.Talla,
                upc = context.UpcEscaneado,
                Cantidad = context.CantidadActual,
                Carton = context.CartonNumber,
                usuario = context.NombreUsuario,
                color = context.Color,
                itemDescription = context.ItemDescription,
                Fecha = DateTime.Now
            };

            context.CartonNumber = context.Funciones.GuardaInventario(clase, context.IdUsuario);
            GenerarEtiqueta(context);
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            using (Barcode codigo = new Barcode { IncludeLabel = true })
            {
                Image codigoBarras = codigo.Encode(
                    TYPE.CODE39,
                    context.CartonNumber.ToString(),
                    Color.Black,
                    Color.White,
                    270,
                    180);

                context.CodigoBarrasGenerado = codigoBarras;
                context.TextoCodigoGenerado = context.CartonNumber.ToString();
            }
        }
    }
}