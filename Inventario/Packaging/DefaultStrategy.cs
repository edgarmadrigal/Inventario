using System.Windows.Forms;

namespace Inventario.Packaging
{
    public class DefaultStrategy : IPackagingStrategy
    {
        public bool ValidarEscaneo(PackagingContext context)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            do
            {
                result = MessageBox.Show(
                    context.FormularioPadre,
                    "PRESIONA EN NO PARA CONTINUAR",
                    "Favor de Escanear la prenda correcta!",
                    buttons,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);
            }
            while (result == DialogResult.Yes);

            return false;
        }

        public void ProcesarCajaIncompleta(PackagingContext context)
        {
            MessageBox.Show("No hay una estrategia definida para el perfil: " + context.ProfileCode);
        }

        public void GenerarEtiqueta(PackagingContext context)
        {
            context.CodigoBarrasGenerado = null;
            context.TextoCodigoGenerado = string.Empty;
        }
    }
}