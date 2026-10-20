using BarcodeLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class ZumiesStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para ZUMIES.");
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
                poItem = "77",
                ProductCode = context.Estilo,
                Size = context.Talla,
                size_izquierdo = context.SizeIzquierdo,
                size_derecho = context.SizeDerecho,
                upc = context.UpcEscaneado,
                Cantidad = context.CantidadActual,
                Carton = context.CartonNumber,
                usuario = context.NombreUsuario,
                DESCRIPTION = context.ItemDescription,
                COUNTRY = "MEXICO",
                Fecha = DateTime.Now,
                cn_tag_num = Convert.ToInt32(context.CartonNumber)
            };

            context.CartonNumber = context.Funciones.GuardaInventarioZumies(clase, context.IdUsuario);
            GenerarEtiqueta(context);
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            using (Barcode codigo = new Barcode
            {
                IncludeLabel = true,
                LabelFont = new Font("Arial", 14, FontStyle.Bold)
            })
            {
                codigo.BarWidth = 3;

                string cadenaCompleta = "00" + context.UpcEscaneado.Substring(0, 11) + "1";

                Image codigoBarras = codigo.Encode(
                    TYPE.ITF14,
                    cadenaCompleta,
                    Color.Black,
                    Color.White,
                    350,
                    150);

                context.CodigoBarrasGenerado = codigoBarras;
                context.TextoCodigoGenerado = cadenaCompleta;
            }
        }
    }
}