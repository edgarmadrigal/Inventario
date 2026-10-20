namespace Inventario
{


    public class FilaPO
    {
        public string PoNumero { get; set; }
        public string Estilo { get; set; }
        public string Color { get; set; }
        public string ColorDescripcion { get; set; }
        public string Dimension { get; set; }
        public string Size { get; set; }
        public string Upc { get; set; }
        public int Cantidad { get; set; }
        public string DescripcionEstilo { get; set; }
        public string Descripcion2 { get; set; }   // <-- CAVENDERS va aquí
        public bool YaExiste { get; set; }
    }
}
