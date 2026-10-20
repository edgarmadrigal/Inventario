using BarcodeLib;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class TargetsStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UpcEscaneado))
            {
                MessageBox.Show("Favor de escanear un código válido para TARGET.");
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
                poItem = "88",
                ProductCode = context.Estilo,
                Size = context.Talla,
                size_izquierdo = context.SizeIzquierdo,
                upc = context.UpcEscaneado,
                Cantidad = context.CantidadActual,
                DPCI = context.ItemDescription,
                usuario = context.NombreUsuario,
                COUNTRY = "MEXICO",
                Fecha = DateTime.Now
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