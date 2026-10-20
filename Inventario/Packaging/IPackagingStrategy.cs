namespace Inventario.Packaging
{
    public interface IPackagingStrategy
    {
        bool ValidarEscaneo(PackagingContext context);

        void ProcesarCajaIncompleta(PackagingContext context);

        void GenerarEtiqueta(PackagingContext context);
    }
}