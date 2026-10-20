using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Inventario
{
    // Tutorial: se muestra cada vez que inician sesion (se puede omitir) y con el boton "Tutorial"
    public partial class Escaneo
    {
        private void PrepararTutorial()
        {
            // En la ventana de modo de prueba no va el tutorial: va la guia de pruebas (PrepararVentanaPrueba)
            if (ModoPrueba.Activo) return;
            AgregarBotonModoPrueba();

            // Boton en la franja de las pestanas, arriba a la derecha
            Button btnTutorial = new Button
            {
                Name = "btnTutorial",
                Text = "Tutorial",
                Size = new Size(96, 24),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TabStop = false
            };
            btnTutorial.Location = new Point(ClientSize.Width - btnTutorial.Width - 8, 2);
            btnTutorial.Click += (s, e) => MostrarTutorial();
            Controls.Add(btnTutorial);
            btnTutorial.BringToFront();

            Shown += (s, e) => MostrarTutorial();
        }

        // Modulos que le quedaron al usuario despues de quitar las pestanas de su perfil (constructor de Escaneo)
        private List<string> ModulosDelUsuario()
        {
            var m = new List<string>();
            var tabs = TabEsaneo.TabPages;
            if (tabs.Contains(tpEscan)) m.Add(Tutorial.Escaneo);
            if (tabs.Contains(tpAltaZumies)) m.Add(Tutorial.AltaPO);
            if (tabs.Contains(tpAltaPrePack)) m.Add(Tutorial.AltaPrePack);
            if (tabs.Contains(tpReimpresion)) m.Add(Tutorial.Reimpresion);
            if (tabs.Contains(tpReportes)) m.Add(Tutorial.Reportes);
            if (tabs.Contains(tpBajaCaja)) m.Add(Tutorial.BajaCaja);
            if (tabs.Contains(tpbajaPO)) m.Add(Tutorial.BajaPO);
            // Almacen siempre se ve, pero sus controles solo se habilitan para estos perfiles
            string perfil = usu != null && usu.Count > 0 ? usu[0].perfil : "";
            if (perfil == "3" || perfil == "4" || perfil == "8") m.Add(Tutorial.Almacen);
            return m;
        }

        private void MostrarTutorial()
        {
            using (Tutorial t = new Tutorial(ModulosDelUsuario())) t.ShowDialog(this);
            if (TabEsaneo.SelectedTab == tpEscan) txtUPCScann.Focus();
        }
    }
}
