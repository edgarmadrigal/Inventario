using System;
using System.Drawing;
using System.Linq;

namespace Inventario
{
    public class EtiquetaNueva
    {
        public EtiquetaNueva()
        {
        }
        /// <summary>
        /// ////////A
        /// </summary>
        
        public string ASupplyName { get; set; }
        public string Asupp { get; set; }

        /// <summary>
        /// ////////B
        /// </summary>
        public string Bqty { get; set; }

        public string BUOM { get; set; }

        public string BContainer { get; set; }

        public string BGrossWeight { get; set; }

        public string BGrossWGTUOM { get; set; }

        public System.Nullable<System.DateTime> BDateShipping { get; set; }

        public string BLotBatch { get; set; }

        public string BSHIFT { get; set; }

        public string BWC { get; set; }

        /// <summary>
        /// ////////C
        /// </summary>
        public string CPart { get; set; }

        /// <summary>
        /// ////////D
        /// </summary>
        public string Dstrloc3 { get; set; }

        public string DASNNumber { get; set; }

        /// <summary>
        /// ////////E
        /// </summary>
        public string ESuppArea { get; set; }

        public string ESuppPartNumber { get; set; }

        public string ESuppPartDescription { get; set; }

        public string ESerial { get; set; }

        public string EMadein { get; set; }
        public string EUserID { get; set; }

        public string EOptionalLabel { get; set; }

        public string ECSN { get; set; }
        public string EPlantCode { get; set; }

        public string EEngineeringAlertNumber { get; set; }

        public string EDocCode { get; set; }

        public string CartonNumber { get; set; }


        public Image codigoBarrasSUPPLIER { get; set; }


        public Image codigoBarrasQTY { get; set; }


        public Image codigoBarrasPART { get; set; }

        public Image codigoBarrasDELIVERY { get; set; }


        public Image codigoBarrasSERIALNO { get; set; }

       public Image codigoBarras2D { get; set; }



    }
}
