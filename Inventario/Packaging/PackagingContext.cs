using System.Drawing;
using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class PackagingContext
    {
        public string ProfileCode { get; set; }
        public string BrandName { get; set; }
        public string UpcEscaneado { get; set; }
        public string PoNumero { get; set; }
        public long CartonNumber { get; set; }
        public decimal CantidadRequerida { get; set; }
        public decimal CantidadActual { get; set; }
        public string Estilo { get; set; }
        public string Color { get; set; }
        public string Talla { get; set; }
        public string SizeIzquierdo { get; set; }
        public string SizeDerecho { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ItemDescription { get; set; }
        public Image CodigoBarrasGenerado { get; set; }
        public string TextoCodigoGenerado { get; set; }
        public Form FormularioPadre { get; set; }
        public Funciones Funciones { get; set; }
        public bool ImprimirEtiqueta { get; set; }

        public PackagingContext()
        {
            CantidadRequerida = 0;
            CantidadActual = 0;
            CartonNumber = 0;
            ImprimirEtiqueta = true;
        }

        public bool EstaCompleto()
        {
            return CantidadActual >= CantidadRequerida && CantidadRequerida > 0;
        }
    }
}