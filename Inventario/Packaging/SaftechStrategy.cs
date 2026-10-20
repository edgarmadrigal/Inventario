using System;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class SaftechStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para SAFTECH.");
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
                poItem = "33",
                ProductCode = context.Estilo,
                Size = context.Talla,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                upc = context.UpcEscaneado,
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
            context.CodigoBarrasGenerado = null;
        }
    }
}