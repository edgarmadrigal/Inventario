using BarcodeLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class CintasStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para CINTAS.");
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
                poItem = "1000",
                ProductCode = context.Estilo,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                upc = context.UpcEscaneado,
                Cantidad = context.CantidadActual,
                Carton = context.CartonNumber,
                po_varchar = "0",
                usuario = context.NombreUsuario,
                Fecha = DateTime.Now
            };

            context.CartonNumber = context.Funciones.GuardaInventario(clase, context.IdUsuario);
            GenerarEtiqueta(context);
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            using (Barcode codigo = new Barcode
            {
                IncludeLabel = true,
                RotateFlipType = RotateFlipType.Rotate90FlipY
            })
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