using System;
using System.Data.SqlClient;

namespace Inventario
{
    // Modo de prueba: mientras esta activo, toda la app (LINQ, datasets y consultas directas) se conecta a la base
    // InventarioPruebas en lugar de Inventario. La base se crea con Instalacion\CrearInventarioPruebas.sql.
    public static class ModoPrueba
    {
        public const string BasePruebas = "InventarioPruebas";
        public const string POPrueba = "991017";
        public const string TextoEtiqueta = "PRUEBA - NO USAR";

        public static bool Activo { get; private set; }

        // Los dos nombres de conexion que usa la app (BDDataContext/Funciones usan la 4, los DataSet la 3)
        private static readonly string[] conexiones = { "InventarioConnectionString3", "InventarioConnectionString4" };
        private static readonly string[] originales = new string[conexiones.Length];

        private static string CadenaPruebas(string cadena)
        {
            return new SqlConnectionStringBuilder(cadena) { InitialCatalog = BasePruebas }.ConnectionString;
        }

        /// <summary>Revisa que exista la base de pruebas y cambia todas las conexiones a ella.</summary>
        public static bool Entrar(out string error)
        {
            error = null;
            if (Activo) return true;
            string prueba = CadenaPruebas((string)Properties.Settings.Default[conexiones[1]]);
            try
            {
                using (var cn = new SqlConnection(prueba))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.catProductos WHERE po_numero = @po", cn))
                {
                    cmd.Parameters.AddWithValue("@po", POPrueba);
                    cn.Open();
                    if (cn.Database != BasePruebas) { error = "La conexion no quedo en la base de pruebas."; return false; }
                    if (Convert.ToInt32(cmd.ExecuteScalar()) == 0) { error = "La base de pruebas no tiene la PO " + POPrueba + "."; return false; }
                }
            }
            catch (Exception ex)
            {
                error = "No se pudo abrir la base de pruebas (" + BasePruebas + "). Hay que crearla con el script " +
                        "Instalacion\\CrearInventarioPruebas.sql.\r\n\r\nDetalle: " + ex.Message;
                return false;
            }

            for (int i = 0; i < conexiones.Length; i++)
            {
                originales[i] = (string)Properties.Settings.Default[conexiones[i]];
                Properties.Settings.Default[conexiones[i]] = CadenaPruebas(originales[i]);
            }
            Activo = true;
            return true;
        }

        /// <summary>Regresa todas las conexiones a produccion.</summary>
        public static void Salir()
        {
            if (!Activo) return;
            for (int i = 0; i < conexiones.Length; i++)
                Properties.Settings.Default[conexiones[i]] = originales[i];
            Activo = false;
        }

        /// <summary>Borra las cajas de la PO de prueba para empezar las pruebas desde cero. Solo corre en la base de pruebas.</summary>
        public static void ReiniciarCajas()
        {
            if (!Activo) return;
            using (var cn = new SqlConnection((string)Properties.Settings.Default[conexiones[1]]))
            using (var cmd = new SqlCommand(
                "IF DB_NAME() <> @base RAISERROR('No es la base de pruebas', 16, 1) ELSE BEGIN " +
                "DELETE d FROM dbo.InventarioDetalle d JOIN dbo.Inventario i ON i.id = d.id_Inventario WHERE i.po_numero = @po; " +
                "DELETE FROM dbo.Inventario WHERE po_numero = @po; " +
                "DELETE FROM dbo.idBajaPO WHERE po_numero = @po; END", cn))
            {
                cmd.Parameters.AddWithValue("@base", BasePruebas);
                cmd.Parameters.AddWithValue("@po", POPrueba);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Cajas guardadas en la base de pruebas para la PO de prueba (id, talla, caja, piezas, estado).</summary>
        public static System.Data.DataTable CajasDePrueba()
        {
            var dt = new System.Data.DataTable();
            if (!Activo) return dt;
            using (var cn = new SqlConnection((string)Properties.Settings.Default[conexiones[1]]))
            using (var da = new SqlDataAdapter(
                "SELECT id AS ID, size_izquierdo AS Talla, cn_tag_num AS Caja, CONVERT(int, cantidad) AS Piezas, " +
                "CASE WHEN id_status = 4 THEN 'Baja' ELSE '' END AS Estado " +
                "FROM dbo.Inventario WHERE po_numero = @po AND DB_NAME() = @base ORDER BY id", cn))
            {
                da.SelectCommand.Parameters.AddWithValue("@po", POPrueba);
                da.SelectCommand.Parameters.AddWithValue("@base", BasePruebas);
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>Agrega "PRUEBA - NO USAR" en negativo al borde izquierdo de cada etiqueta ZPL.</summary>
        public static string MarcarZpl(string zpl)
        {
            if (!Activo || string.IsNullOrEmpty(zpl)) return zpl;
            string marca = "^FO10,300^GB90,1200,90^FS^FO22,380^A0B,70,70^FR^FD" + TextoEtiqueta + "^FS";
            return zpl.Replace("^XZ", marca + "^XZ");
        }
    }
}
