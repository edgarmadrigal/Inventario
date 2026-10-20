using BarcodeLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class BeansStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para BEN DAVIS.");
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

            EtiquetaBens clase = new EtiquetaBens
            {
                po = context.PoNumero,
                poItem = "55",
                ProductCode = context.Estilo,
                Size = context.Talla,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                upc = context.UpcEscaneado,
                QUANTITY = context.CantidadActual.ToString(),
                Cantidad = context.CantidadActual,
                usuario = context.NombreUsuario,
                itemDescription = context.ItemDescription,
                color = context.Color,
                Fecha = DateTime.Now
            };

            EtiquetaCajaModificada respuesta = context.Funciones.GuardaInventarioBeans(clase, context.IdUsuario);
            clase.id_string = respuesta.codigoBarrasTextoB.ToString().Trim();
            context.TextoCodigoGenerado = clase.id_string;

            GenerarEtiqueta(context);
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            if (string.IsNullOrEmpty(context.TextoCodigoGenerado))
            {
                MessageBox.Show("No se generó el código de texto para BEN DAVIS antes de crear la etiqueta.");
                return;
            }

            using (Barcode codigo = new Barcode { IncludeLabel = false })
            {
                Image codigoBarras = codigo.Encode(
                    TYPE.CODE128,
                    context.TextoCodigoGenerado,
                    Color.Black,
                    Color.White,
                    500,
                    45);

                context.CodigoBarrasGenerado = codigoBarras;
            }
        }
    }
}