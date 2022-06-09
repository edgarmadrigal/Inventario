using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Inventario
{
   public class EtiquetaTARGET
    {

        public EtiquetaTARGET()
        {
        }
        public int? id { get; set; }

        public int id_cliente { get; set; }


        public int id_factura { get; set; }

        public int id_terminado { get; set; }

        public string cliente { get; set; }

        public string nivel { get; set; }

        public string factura { get; set; }

        public string terminado { get; set; }

        public int id_Inventario { get; set; }

        public Bitmap qr { get; set; }

        public Image codigoBarras { get; set; }

        public System.Nullable<decimal> po { get; set; }
        public System.Nullable<decimal> poInCompleto { get; set; }

        public string assembly { get; set; }

        public string Vendor { get; set; }

        public string ShipTo { get; set; }

        public string poItem { get; set; }

        public System.Nullable<decimal> Cantidad { get; set; }

        public string size_izquierdo { get; set; }

        public string size_derecho { get; set; }


        public string numeroEtiqueta1 { get; set; }

        public string numeroEtiqueta2 { get; set; }
        public string numeroEtiqueta3 { get; set; }

        public string Size { get; set; }

        public string upc { get; set; }

        public System.Nullable<long> Carton { get; set; }

        public string CartonLeft { get; set; }

        public string CartonRight { get; set; }

        public string ProductCode { get; set; }

        public string TipoCarton { get; set; }

        public string usuario { get; set; }


        public int idusuario { get; set; }

        public System.Nullable<int> idSize { get; set; }

        public System.Nullable<System.DateTime> Fecha { get; set; }


        public string DPCI { get; set; }



    }
}


