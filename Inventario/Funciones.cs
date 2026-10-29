using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
namespace Inventario
{
    public class Funciones
    {
        public List<ConsultaInventarioDetalleResult> ConsultaInventarioDetalle(int idInventario)
        {
            List<ConsultaInventarioDetalleResult> lista = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                lista = consulta.ConsultaInventarioDetalle(idInventario).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return lista;
        }
        public void ConsultaUbicacion(ComboBox combo)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ubicacion_Entrada_ConsultaUbicacionResult> po = consulta.ubicacion_Entrada_ConsultaUbicacion().ToList();

                combo.DisplayMember = "nombre";
                combo.ValueMember = "id";
                combo.DataSource = po;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void ConsultaCliente(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                List<ConsultaClienteResult> po = consulta.ConsultaCliente().ToList();

                combo.DisplayMember = "descripcion";
                combo.ValueMember = "numero";
                combo.DataSource = po;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        public void ConsultaTerminado(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                List<ConsultaTerminadoResult> po = consulta.ConsultaTerminado().ToList();

                combo.DisplayMember = "descripcion";
                combo.ValueMember = "numero";
                combo.DataSource = po;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        public void ConsultaFactura(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                List<ConsultaFacturaResult> po = consulta.ConsultaFactura().ToList();

                combo.DisplayMember = "descripcion";
                combo.ValueMember = "numero";
                combo.DataSource = po;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        public void ConsultaPO(ComboBox combo)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();
                List<ConsultaPO_VarcharResult> po = new List<ConsultaPO_VarcharResult>();

                combo.DataSource = null;
                combo.Items.Clear();
                po = consulta.ConsultaPO_Varchar().ToList();

                combo.DisplayMember = "po_numero";
                combo.ValueMember = "po_numero";
                combo.DataSource = po;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }
        public void ConsultaPOModificar(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();
                List<ConsultaPOModificarResult> po = new List<ConsultaPOModificarResult>();

                combo.DataSource = null;
                combo.Items.Clear();

                po = consulta.ConsultaPOModificar().ToList();

                combo.DisplayMember = "po_numero";
                combo.ValueMember = "po_numero";
                combo.DataSource = po;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public List<ConsultaTodoPOResult> ConsultaTodoPO(string po)
        {
            List<ConsultaTodoPOResult> TodoPO = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaTodoPOResult> polista = consulta.ConsultaTodoPO(po).ToList();

                return polista;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return TodoPO;
            }
        }

        public void ConsultaPOItem(ComboBox combo, string po)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaPOItem_VarcharResult> poItem = consulta.ConsultaPOItem_Varchar(po);

                combo.DataSource = poItem;
                combo.DisplayMember = "po_item";
                combo.ValueMember = "po_item";

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void ConsultaProductCode(ComboBox combo, string po, string poItem)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaProductCodeResult> productCode = consulta.ConsultaProductCode(po, poItem);

                combo.DataSource = productCode;
                combo.DisplayMember = "prod_cd";
                combo.ValueMember = "prod_cd";

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void ConsultaSizes(ComboBox combo, string po, string poItem, string prodCd)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaSizesResult> size = consulta.ConsultaSizes(po, poItem, prodCd);

                combo.DataSource = size;
                combo.DisplayMember = "size";
                combo.ValueMember = "size";

            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);

            }
        }

        public List<ConsultaProductos_VarcharResult> ConsultaProductos(string po, string poItem, string prodCd, string sizeIzquierdo, string sizeDerecho)
        {
            List<ConsultaProductos_VarcharResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaProductos_VarcharResult> Producto = consulta.ConsultaProductos_Varchar(po, poItem, prodCd, sizeIzquierdo.Trim(), sizeDerecho.Trim()).ToList();

                return Producto;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return Productonull;
            }
        }
        public List<ConsultaProductosZumiesResult> ConsultaProductosZumies(string po, string poItem, string prodCd, string size)
        {
            List<ConsultaProductosZumiesResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                //decimal? po_numero = 0;
                //po_numero = Convert.ToInt64(po);
                List<ConsultaProductosZumiesResult> Producto = consulta.ConsultaProductosZumies(po, poItem, prodCd, size).ToList();

                return Producto;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return Productonull;
            }
        }

        public List<ConsultaProductosOLDNAVYResult> ConsultaProductosOLDNAVY(string po, string sizeIzquierdo, string sizeDerecho)
        {
            List<ConsultaProductosOLDNAVYResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaProductosOLDNAVYResult> Producto = consulta.ConsultaProductosOLDNAVY(po, sizeIzquierdo.Trim(), sizeDerecho.Trim()).ToList();

                return Producto;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return Productonull;
            }
        }
        public List<ConsultaProductosTargetResult> ConsultaProductosTarget(string po, string poItem, string prodCd, string size)
        {
            List<ConsultaProductosTargetResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaProductosTargetResult> Producto = consulta.ConsultaProductosTarget(po, poItem, prodCd, size).ToList();

                return Producto;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return Productonull;
            }
        }
        public List<ConsultaUsuarioResult> ConsultaUsuario(string usuario, string password)
        {
            List<ConsultaUsuarioResult> objusuario = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaUsuarioResult> objusu = consulta.ConsultaUsuario(usuario, password).ToList();

                return objusu;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objusuario;
            }

        }
        public List<ConsultaEtiquetaResult> ConsultaEtiqueta(int? id)
        {
            List<ConsultaEtiquetaResult> objusuario = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaEtiquetaResult> objusu = consulta.ConsultaEtiqueta(id).ToList();

                return objusu;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objusuario;
            }

        }

        public int? ConsultaPrepack(string po)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();

                ConsultaPrepackResult objusu = consulta.ConsultaPrepack(po).FirstOrDefault();

                return objusu.ESPREPACK;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return 0;
            }

        }

        public int? ConsultaMixto(string po, string estilo)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();

                ConsultaMixtoResult objusu = consulta.ConsultaMixto(po, estilo).FirstOrDefault();

                return objusu.mixto;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return 0;
            }

        }

        public List<ConsultaPrepackDetalleResult> ConsultaPrepackDetalle(int idPrepack)
        {
            List<ConsultaPrepackDetalleResult> listprepackDetalle = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaPrepackDetalleResult> listprepack = consulta.ConsultaPrepackDetalle(idPrepack).ToList();

                return listprepack;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return listprepackDetalle;
            }
        }

        public Int64 GuardaInventario(EtiquetaCajaModificada et, int iduser)
        {
            Int64 respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarInventarioResult insertInventario = consulta.GuardarInventario(et.id,
                                                                                      et.po,
                                                                                      et.po_varchar,
                                                                                      et.poItem,
                                                                                      et.Cantidad,
                                                                                      et.size_izquierdo,
                                                                                      et.size_derecho,
                                                                                      et.upc,
                                                                                      et.Carton,
                                                                                      et.ProductCode,
                                                                                      et.TipoCarton,
                                                                                      iduser,
                                                                                      et.id_cliente,
                                                                                      et.id_factura,
                                                                                      et.id_terminado,
                                                                                      et.mixto).FirstOrDefault();
                consulta.SubmitChanges();
                if (et.poItem == "22")
                {
                    respuesta = Convert.ToInt64(insertInventario.Column1);
                }
                else
                {
                    respuesta = Convert.ToInt32(insertInventario.Column1);
                }
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public Int64 GuardaInventarioDetalle(EtiquetaCajaModificada et, int iduser)
        {
            Int64 respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarInventarioDetalleResult insertInventario = consulta.GuardarInventarioDetalle(et.id_inventario
                                                                                      , et.estilo
                                                                                      , et.poItem
                                                                                      , et.po
                                                                                      , et.Cantidad
                                                                                      , et.size_izquierdo
                                                                                      , et.size_derecho
                                                                                      , et.upc).FirstOrDefault();
                consulta.SubmitChanges();
                if (et.poItem == "22")
                {
                    respuesta = Convert.ToInt64(insertInventario.Column1);
                }
                else
                {
                    respuesta = Convert.ToInt32(insertInventario.Column1);
                }
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaInventarioZumies(EtiquetaZUMIES et, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarInventarioResult insertInventario = consulta.GuardarInventario(et.id,
                                                                                      et.po,
                                                                                      "",
                                                                                      et.poItem,
                                                                                      et.Cantidad,
                                                                                      et.size_izquierdo,
                                                                                      et.size_derecho,
                                                                                      et.upc,
                                                                                      et.cn_tag_num,
                                                                                      et.ProductCode,
                                                                                      et.DESCRIPTION,
                                                                                      iduser,
                                                                                      et.id_cliente,
                                                                                      et.id_factura,
                                                                                      et.id_terminado,
                                                                                      0).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(insertInventario.Column1);
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaProducto(EtiquetaCajaModificada et, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarProductos2Result insertInventario = consulta.GuardarProductos2(et.po,
                                                                                      et.Cantidad,
                                                                                      et.size_izquierdo,
                                                                                      et.size_derecho,
                                                                                      et.assembly,
                                                                                      et.upc,
                                                                                      et.Carton,
                                                                                      et.ProductCode, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(insertInventario.Column1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaAltaPO(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarAltaPOManualResult Alta = consulta.GuardarAltaPOManual(et.po,
                                        et.estilo,
                                        et.Cantidad,
                                        et.upc,
                                        et.idSize,
                                        et.itemDescription,
                                        et.color,
                                        et.idusuario).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaWrangler(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarWranglerResult Alta = consulta.GuardarWrangler(et.po,
                                        et.estilo,
                                        et.Cantidad,
                                        et.upc,
                                        et.idSize,
                                        et.itemDescription,
                                        et.color,
                                        et.idusuario,
                                        et.CantidadCajas).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public int GuardaZumies(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarZumiesResult Alta = consulta.GuardarZumies(et.po,
                                        et.estilo,
                                        et.Cantidad,
                                        et.CantidadCajas,
                                        et.upc,
                                        et.idSize,
                                        et.itemDescription,
                                        et.color,
                                        et.escaneado,
                                        et.idusuario).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaIFR(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarIFRResult Alta = consulta.GuardarIFR(et.po,
                                                                et.Cantidad,
                                                                et.idSize,
                                                                et.idusuario,
                                                                et.upc,
                                                                et.color,
                                                                et.estilo,
                                                                et.CantidadCajas
                                                                ).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaTYNDALE(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarTYNDALEResult Alta = consulta.GuardarTYNDALE(et.po,
                                                                et.Cantidad,
                                                                et.idSize,
                                                                et.idusuario,
                                                                et.upc,
                                                                et.color,
                                                                et.estilo,
                                                                et.CantidadCajas
                                                                ).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public int GuardaBENS(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarBensResult Alta = consulta.GuardarBens(et.po, et.po_varchar,
                                          et.estilo,
                                          et.Cantidad,
                                          //et.CantidadCajas,
                                          et.upc,
                                          et.idSize,
                                          et.itemDescription,
                                          et.color,
                                          et.escaneado,
                                          et.idusuario
                                          ).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public int GuardarSaftech(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarSaftechResult Alta = consulta.GuardarSaftech(et.po, et.po_varchar,
                                          et.estilo,
                                          et.Cantidad,
                                          //et.CantidadCajas,
                                          et.upc,
                                          et.idSize,
                                          et.itemDescription,
                                          et.color,
                                          et.escaneado,
                                          et.idusuario
                                          ).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardarPO(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarPOResult Alta = consulta.GuardarPO(et.po, et.po_varchar,
                                          et.estilo,
                                          et.Cantidad,
                                          et.CantidadCajas,
                                          et.upc,
                                          et.idSize,
                                          et.itemDescription,
                                          et.color,
                                          et.escaneado,
                                          et.idusuario,
                                          et.marca
                                          ).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardarTarget(EtiquetaCajaModificada et)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();


                GuardarTargetResult Alta = consulta.GuardarTarget(et.po,
                                          et.Cantidad,
                                          et.idSize,
                                          et.escaneado,
                                          et.idusuario,
                                          et.assembly,
                                          et.itemDescription,
                                          et.upc).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(Alta.RESPUESTA);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaProductoLEVIS(EtiquetaCajaModificada et, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                consulta.GuardarProductos2(et.po,
                                          et.Cantidad,
                                          et.size_izquierdo,
                                          et.size_derecho,
                                          et.assembly,
                                          et.upc,
                                          et.Carton,
                                          et.ProductCode, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = 1;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardaProductoTARGET(EtiquetaCajaModificada et, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                consulta.GuardarProductosTARGET(et.po,
                                          et.size_izquierdo,
                                          et.size_derecho,
                                          et.upc,
                                          et.DPCI,
                                          et.Cantidad,
                                          et.CantidadCajas,
                                          et.esprepack,
                                          et.barcode,
                                          iduser).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = 1;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public List<ConsultaInventarioTallaSeparadaResult> ConsultaInventarioTallasSeparadas(DateTime? fechaInicio, DateTime? fechaFin, string marca = null)
        {
            List<ConsultaInventarioTallaSeparadaResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaInventarioTallaSeparadaResult> objin;
                try { objin = consulta.ConsultaInventarioTallaSeparada(fechaInicio, fechaFin, marca).ToList(); }
                catch (System.Data.SqlClient.SqlException sx) when (sx.Number == 8144)
                {
                    // El SP del servidor aun no tiene @marca (falta correr Fase1_ReportesMarca.sql): se llama sin marca
                    objin = consulta.ExecuteQuery<ConsultaInventarioTallaSeparadaResult>("EXEC dbo.ConsultaInventarioTallaSeparada {0}, {1}", fechaInicio, fechaFin).ToList();
                    var mapa = MapaMarcas();
                    foreach (var r in objin) { string m; r.MARCA = r.POITEM != null && mapa.TryGetValue(r.POITEM.Trim(), out m) ? m : null; }
                    if (!string.IsNullOrEmpty(marca)) objin = objin.Where(r => r.MARCA == marca).ToList();
                }

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }

        /// <summary>
        /// Marca por clave de po_item (clienteMarca). Hay claves repetidas (77 = LEVIS y ZUMIES, 1000 = LEVIS y CINTAS):
        /// se toma la que no es LEVIS, igual que los stored procedures de reportes.
        /// </summary>
        public Dictionary<string, string> MapaMarcas()
        {
            BDDataContext consulta = new BDDataContext();
            return consulta.clienteMarca.ToList()
                .Where(c => !string.IsNullOrWhiteSpace(c.clave) && !string.IsNullOrWhiteSpace(c.nombre))
                .GroupBy(c => c.clave.Trim())
                .ToDictionary(g => g.Key, g => g.OrderBy(c => c.nombre.Trim() == "LEVIS" ? 1 : 0).ThenBy(c => c.id).First().nombre.Trim());
        }

        /// <summary>Marcas para el filtro de los reportes.</summary>
        public List<string> ConsultaMarcasReporte()
        {
            return MapaMarcas().Values.Distinct().OrderBy(m => m).ToList();
        }

        public List<ConsultaInventarioResult> ConsultaInventario(DateTime? fechaInicio, DateTime? fechaFin, string marca = null)
        {
            List<ConsultaInventarioResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaInventarioResult> objin;
                try { objin = consulta.ConsultaInventario(fechaInicio, fechaFin, marca).ToList(); }
                catch (System.Data.SqlClient.SqlException sx) when (sx.Number == 8144)
                {
                    // El SP del servidor aun no tiene @marca (falta correr Fase1_ReportesMarca.sql): se llama sin marca
                    objin = consulta.ExecuteQuery<ConsultaInventarioResult>("EXEC dbo.ConsultaInventario {0}, {1}", fechaInicio, fechaFin).ToList();
                    var mapa = MapaMarcas();
                    foreach (var r in objin) { string m; r.MARCA = r.POITEM != null && mapa.TryGetValue(r.POITEM.Trim(), out m) ? m : null; }
                    if (!string.IsNullOrEmpty(marca)) objin = objin.Where(r => r.MARCA == marca).ToList();
                }

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        //public List<ConsultaAlmacenesResult> ConsultaAlmacenes(DateTime? fechaInicio, DateTime? fechaFin)
        //{
        //    List<ConsultaAlmacenesResult> objInv = null;
        //    try
        //    {
        //        BDDataContext consulta = new BDDataContext
        //        {
        //            CommandTimeout = 2500
        //        };
        //        List<ConsultaAlmacenesResult> objin = consulta.ConsultaAlmacenes(fechaInicio, fechaFin).ToList();

        //        return objin;

        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);

        //        return objInv;
        //    }

        //}
        //public List<ConsultaAlmacenesDTLResult> ConsultaAlmacenesDTL(DateTime? fechaInicio, DateTime? fechaFin)
        //{
        //    List<ConsultaAlmacenesDTLResult> objInv = null;
        //    try
        //    {
        //        BDDataContext consulta = new BDDataContext
        //        {
        //            CommandTimeout = 2500
        //        };
        //        List<ConsultaAlmacenesDTLResult> objin = consulta.ConsultaAlmacenesDTL(fechaInicio, fechaFin).ToList();

        //        return objin;

        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);

        //        return objInv;
        //    }

        //}

        public List<ConsultaAlmacenesFolioComprasResult> ConsultaComprasAIN(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaAlmacenesFolioComprasResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext
                {
                    CommandTimeout = 2500
                };
                List<ConsultaAlmacenesFolioComprasResult> objin = consulta.ConsultaAlmacenesFolioCompras(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }


        public List<ConsultaAlmacenesFolioComprasTBGResult> ConsultaComprasTBG(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaAlmacenesFolioComprasTBGResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext
                {
                    CommandTimeout = 2500
                };
                List<ConsultaAlmacenesFolioComprasTBGResult> objin = consulta.ConsultaAlmacenesFolioComprasTBG(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }


        public List<ConsultaAlmacenesFolioComprasDTLResult> ConsultaComprasDTL(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaAlmacenesFolioComprasDTLResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext
                {
                    CommandTimeout = 2500
                };
                List<ConsultaAlmacenesFolioComprasDTLResult> objin = consulta.ConsultaAlmacenesFolioComprasDTL(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public List<ConsultaAlmacenesTBGResult> ConsultaAlmacenesTBG(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaAlmacenesTBGResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext
                {
                    CommandTimeout = 2500
                };
                List<ConsultaAlmacenesTBGResult> objin = consulta.ConsultaAlmacenesTBG(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public List<ConsultaInventarioIDResult> ConsultaInventarioID(int id)
        {
            List<ConsultaInventarioIDResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaInventarioIDResult> objin = consulta.ConsultaInventarioID(id).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public List<ConsultaInventarioPOResult> ConsultaInventarioPO(string po_numero)
        {
            List<ConsultaInventarioPOResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                consulta.CommandTimeout = 120; // segundos; ajusta según lo que tarde en SSMS

                List<ConsultaInventarioPOResult> objin = consulta.ConsultaInventarioPO(po_numero).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public List<ConsultaUPC_VarcharResult> ConsultaUPC(string po_numero, string upc)
        {
            List<ConsultaUPC_VarcharResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaUPC_VarcharResult> objin = consulta.ConsultaUPC_Varchar(po_numero, upc).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }
        }

        public bool BajaPO(string po, int id_Cliente, int id_Facturacion, int id_Terminado, int iduser)
        {
            bool respuesta = false;
            try
            {
                BDDataContext consulta = new BDDataContext();

                BajaPOResult respuesta1 = consulta.BajaPO(po, id_Cliente, id_Facturacion, id_Terminado, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                if (respuesta1.Column1 == 0)
                {
                    respuesta = false;

                }
                else
                {
                    respuesta = true;
                }
            }

            catch (Exception ex)
            {
                respuesta = false;
                MessageBox.Show(ex.Message);

            }
            return respuesta;


        }

        public class BajaTallaEstiloResult
        {
            // 1 borrado, 3 borrado junto con sus cajas, 2 solo consulta, 0 no encontrada, -1 tiene cajas y no se pidio borrarlas
            public int Resultado { get; set; }
            public int Productos { get; set; }
            public int Cajas { get; set; }
        }

        // Version instalada (la de ClickOnce); si se corre desde Visual Studio, la del ensamblado marcada "local".
        // Se muestra en el titulo de las ventanas para saber que version tiene quien llama.
        public static string VersionApp()
        {
            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                    return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch { }
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " (local)";
        }

        public static string TituloConVersion(string titulo)
        {
            return titulo + "  -  Versión " + VersionApp();
        }

        // Datos del equipo desde donde se hace una baja, para la bitacora (BitacoraBajas)
        public static string[] EquipoActual()
        {
            string ip = "";
            try
            {
                ip = string.Join(", ", System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
                    .Where(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(a))
                    .Select(a => a.ToString()));
            }
            catch { }
            return new[] { Environment.MachineName, ip, Environment.UserDomainName + "\\" + Environment.UserName };
        }

        // Baja de una talla de UN estilo (BajaTallaPO borraba la talla en todos los estilos de la PO).
        // Con borrarCajas tambien da de baja las cajas escaneadas de esa talla; el SP lo deja en BitacoraBajas.
        public BajaTallaEstiloResult BajaTallaEstilo(string po, string poItem, string estilo, string talla, int iduser, bool soloConsultar, bool borrarCajas = false)
        {
            string[] equipo = EquipoActual();
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<BajaTallaEstiloResult>("EXEC dbo.BajaTallaEstilo {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
                po, poItem, estilo, talla, iduser, soloConsultar, borrarCajas, equipo[0], equipo[1], equipo[2]).FirstOrDefault();
        }

        public class BajaCajasPOResult
        {
            public int Resultado { get; set; } // 1 borrado, 2 solo consulta, 0 la PO no tiene cajas activas
            public int Cajas { get; set; }
            public int Prendas { get; set; }
        }

        // Da de baja todas las cajas escaneadas de la PO sin borrar la PO del catalogo; el SP lo deja en BitacoraBajas
        public BajaCajasPOResult BajaCajasPO(string po, int iduser, bool soloConsultar)
        {
            string[] equipo = EquipoActual();
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<BajaCajasPOResult>("EXEC dbo.BajaCajasPO {0}, {1}, {2}, {3}, {4}, {5}",
                po, iduser, soloConsultar, equipo[0], equipo[1], equipo[2]).FirstOrDefault();
        }

        // Borra SOLO la PO del catalogo (catProductos); las cajas escaneadas se quedan. El SP lo deja en BitacoraBajas
        public BajaTallaEstiloResult BajaCatalogoPO(string po, int iduser, bool soloConsultar)
        {
            string[] equipo = EquipoActual();
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<BajaTallaEstiloResult>("EXEC dbo.BajaCatalogoPO {0}, {1}, {2}, {3}, {4}, {5}",
                po, iduser, soloConsultar, equipo[0], equipo[1], equipo[2]).FirstOrDefault();
        }

        public class ProductoCorreccion
        {
            public int id { get; set; }
            public string po_item { get; set; }
            public string marca { get; set; }
            public string talla { get; set; }
            public string estilo { get; set; }
            public string color { get; set; }
            public string descripcion { get; set; }
            public string vendor { get; set; }
            public string shipTo { get; set; }
            public string upc { get; set; }
            public int cantidad { get; set; }
            public int cajas { get; set; }
        }

        public class ModificarProductoResult
        {
            public int Resultado { get; set; }
            public int Cajas { get; set; }
        }

        // Renglones del catalogo de la PO para la pestana Corregir PO
        public List<ProductoCorreccion> ConsultaProductosCorreccion(string po)
        {
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<ProductoCorreccion>("EXEC dbo.ConsultaProductosCorreccion {0}", po).ToList();
        }

        // Cambia un renglon del catalogo y lo propaga a lo escaneado; el SP deja cada cambio en BitacoraCambios
        public ModificarProductoResult ModificarProductoPO(ProductoCorreccion p, int iduser)
        {
            string[] equipo = EquipoActual();
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<ModificarProductoResult>("EXEC dbo.ModificarProductoPO {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                p.id, p.estilo ?? "", p.color ?? "", p.descripcion ?? "", p.vendor ?? "", p.shipTo ?? "", p.upc ?? "", iduser,
                equipo[0], equipo[1], equipo[2]).FirstOrDefault();
        }

        public class PackingBENS
        {
            public string PO { get; set; }
            public string Barcode { get; set; }
            public int Box { get; set; }
            public string Style { get; set; }
            public string Waist { get; set; }
            public string Length { get; set; }
            public string Size { get; set; }
            public int Qty { get; set; }
            public int Mixta { get; set; }
            public int idCaja { get; set; }
            public string Color { get; set; }
        }

        // Packing list de Ben Davis por PO (cajas normales y una fila por talla en las mixtas)
        public List<PackingBENS> ReportePackingBENS(string po)
        {
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<PackingBENS>("EXEC dbo.ReportePackingBENS {0}", po).ToList();
        }

        // ---- Sugerencias del Summary de Ben Davis (ORDER QTY por talla y peso por prenda, por PO) ----
        public class SugerenciaPedidoBENS
        {
            public string talla { get; set; }
            public int pedido { get; set; }
        }

        public class SugerenciaPesoBENS
        {
            public decimal pesoBruto { get; set; }
            public decimal pesoNeto { get; set; }
        }

        public List<SugerenciaPedidoBENS> ConsultaSugerenciaPedidoBENS(string po)
        {
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<SugerenciaPedidoBENS>("EXEC dbo.ConsultaSugerenciaPedidoBENS {0}", po).ToList();
        }

        public SugerenciaPesoBENS ConsultaSugerenciaPesoBENS(string po)
        {
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<SugerenciaPesoBENS>("EXEC dbo.ConsultaSugerenciaPesoBENS {0}", po).FirstOrDefault();
        }

        // pedido null borra la sugerencia de esa talla
        public void GuardarSugerenciaPedidoBENS(string po, string talla, int? pedido, int iduser)
        {
            BDDataContext consulta = new BDDataContext();
            // ExecuteQuery no acepta parametros null: sin pedido se manda NULL directo
            if (pedido.HasValue)
                consulta.ExecuteQuery<int>("EXEC dbo.GuardarSugerenciaPedidoBENS {0}, {1}, {2}, {3}", po, talla, pedido.Value, iduser).FirstOrDefault();
            else
                consulta.ExecuteQuery<int>("EXEC dbo.GuardarSugerenciaPedidoBENS {0}, {1}, NULL, {2}", po, talla, iduser).FirstOrDefault();
        }

        public void GuardarSugerenciaPesoBENS(string po, decimal bruto, decimal neto, int iduser)
        {
            BDDataContext consulta = new BDDataContext();
            consulta.ExecuteQuery<int>("EXEC dbo.GuardarSugerenciaPesoBENS {0}, {1}, {2}, {3}", po, bruto, neto, iduser).FirstOrDefault();
        }

        // ---- Cut Order de Ben Davis (captura de la foto que manda el cliente) ----
        public class CutOrderBENS
        {
            public int id { get; set; }
            public DateTime fecha { get; set; }
            public string po { get; set; }
            public string estilo { get; set; }
            public int totalUnits { get; set; }
            public int totalCapturado { get; set; }
            public string detalle { get; set; }
            public byte[] foto { get; set; }
            public string fotoNombre { get; set; }
            public string usuario { get; set; }
        }

        public CutOrderBENS ConsultaUltimoCutOrderBENS(string po)
        {
            BDDataContext consulta = new BDDataContext();
            return consulta.ExecuteQuery<CutOrderBENS>("EXEC dbo.ConsultaUltimoCutOrderBENS {0}", po).FirstOrDefault();
        }

        public void GuardarCutOrderBENS(string po, string estilo, int totalUnits, int totalCapturado, string detalle, byte[] foto, string fotoNombre, int iduser)
        {
            string[] equipo = EquipoActual();
            BDDataContext consulta = new BDDataContext();
            // ExecuteQuery no acepta parametros null: sin foto se manda NULL directo
            if (foto != null)
                consulta.ExecuteQuery<int>("EXEC dbo.GuardarCutOrderBENS {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                    po, estilo ?? "", totalUnits, totalCapturado, detalle ?? "", foto, fotoNombre ?? "", iduser, equipo[0], equipo[1], equipo[2]).FirstOrDefault();
            else
                consulta.ExecuteQuery<int>("EXEC dbo.GuardarCutOrderBENS {0}, {1}, {2}, {3}, {4}, NULL, NULL, {5}, {6}, {7}, {8}",
                    po, estilo ?? "", totalUnits, totalCapturado, detalle ?? "", iduser, equipo[0], equipo[1], equipo[2]).FirstOrDefault();
        }

        public void RegistrarBitacoraBaja(string tipo, string po, int iduser)
        {
            string[] equipo = EquipoActual();
            BDDataContext consulta = new BDDataContext();
            consulta.ExecuteQuery<int>("EXEC dbo.RegistrarBitacoraBaja @tipo={0}, @po={1}, @idUser={2}, @maquina={3}, @ip={4}, @usuarioWindows={5}",
                tipo, po, iduser, equipo[0], equipo[1], equipo[2]).FirstOrDefault();
        }

        public bool BajaTallaPO(string po, string talla, int iduser)
        {
            bool respuesta = false;
            try
            {
                BDDataContext consulta = new BDDataContext();

                BajaTallaPOResult respuesta1 = consulta.BajaTallaPO(po, talla, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                if (respuesta1.Column1 == 0)
                {
                    respuesta = false;

                }
                else
                {
                    respuesta = true;
                }
            }

            catch (Exception ex)
            {
                respuesta = false;
                MessageBox.Show(ex.Message);

            }
            return respuesta;


        }

        public bool BajaCaja(string po, int iduser)
        {
            bool respuesta = false;
            try
            {
                BDDataContext consulta = new BDDataContext();

                BajaCaja2Result respuesta1 = consulta.BajaCaja2(po, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                if (respuesta1.Column1 == 0)
                {
                    respuesta = false;

                }
                else
                {
                    respuesta = true;
                }
            }

            catch (Exception ex)
            {
                respuesta = false;
                MessageBox.Show(ex.Message);

            }
            return respuesta;
        }

        public void ConsultaTallas(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasResult> poItem = consulta.ConsultaTallas();

                combo.DataSource = poItem;
                combo.DisplayMember = "size";
                combo.ValueMember = "id";

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }


        public void ConsultaTallasIFR(ComboBox combo, string estilo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasIFRResult> poItem = consulta.ConsultaTallasIFR(estilo);

                //Corregi el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "size";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public void ConsultaTallasTYNDALE(ComboBox combo, string estilo, string color)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasTYNDALEResult> poItem = consulta.ConsultaTallasTYNDALE(estilo, color);

                //Corregi el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "size";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public System.Data.Linq.ISingleResult<ConsultaTallasColorTYNDALEResult> ConsultaColorTYNDALE(ComboBox combo, string estilo)
        {
            try
            {
                System.Data.Linq.ISingleResult<ConsultaTallasColorTYNDALEResult> poItem = null;
                BDDataContext consulta = new BDDataContext();

                poItem = consulta.ConsultaTallasColorTYNDALE(estilo);

                //Corregi el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "color";
                combo.DataSource = poItem;

                return poItem;

            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);
                return null;

            }
        }



        public void ConsultaTallasXMarca(ComboBox combo, string marca)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasXMarcaResult> poItem = consulta.ConsultaTallasXMarca(marca);

                //Corregi el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "size";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public void ConsultaEstiloBens(ComboBox combo)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();
                System.Data.Linq.ISingleResult<ConsultaEstiloBensResult> poItem = consulta.ConsultaEstiloBens();

                //Corregir el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "estilo";
                combo.DataSource = poItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }

        public void ConsultaEstiloSaftech(ComboBox combo)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();
                System.Data.Linq.ISingleResult<ConsultaEstiloSaftechResult> poItem = consulta.ConsultaEstiloSaftech();

                //Corregir el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "estilo";
                combo.DataSource = poItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }


        public void ConsultaEstiloRocky(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaEstiloRockyResult> poItem = consulta.ConsultaEstiloRocky();

                //Corregir el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "estilo";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public void ConsultaEstiloIFR(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaEstiloIFRResult> poItem = consulta.ConsultaEstiloIFR();

                //Corregir el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "estilo";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public void ConsultaEstiloTYNDALE(ComboBox combo)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();
                System.Data.Linq.ISingleResult<ConsultaEstiloTYNDALEResult> poItem = consulta.ConsultaEstiloTYNDALE();
                //Corregir el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "estilo";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public void ConsultaEstiloOldNavy(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaEstiloOldNavyResult> poItem = consulta.ConsultaEstiloOldNavy();

                //Corregir el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "estilo";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public void ConsultaTallasBENS(ComboBox combo, string marca)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasBENSResult> poItem = consulta.ConsultaTallasBENS(marca);

                //Corregi el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "size";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }

        public void ConsultaTallasxMarcaxEstilo(ComboBox combo, string marca, string estilo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasxMarcaxEstiloResult> poItem = consulta.ConsultaTallasxMarcaxEstilo(estilo, marca);

                //Corregi el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "size";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }

        public void ConsultaTallasRocky(ComboBox combo, string marca)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasRockyResult> poItem = consulta.ConsultaTallasRocky(marca);

                //Corregi el orden de inicializacion de variables
                combo.ValueMember = "id";
                combo.DisplayMember = "size";
                combo.DataSource = poItem;


            }

            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos " + ex.Message);

            }
        }
        public void ConsultaTallasZumies(ComboBox combo)
        {
            try
            {

                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTallasZumiesResult> poItem = consulta.ConsultaTallasZumies();

                combo.DataSource = poItem;
                combo.DisplayMember = "size";
                combo.ValueMember = "id";

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public void ConsultaTipoCaja(ComboBox combo)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();

                System.Data.Linq.ISingleResult<ConsultaTipoCajaResult> poItem = consulta.ConsultaTipoCaja();

                combo.DataSource = poItem;
                combo.DisplayMember = "descripcion";
                combo.ValueMember = "id";

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        public List<ubicacion_Entrada_ConsultaUbicacionIDResult> ConsultaUbicacionID(int idUbicacion)
        {
            List<ubicacion_Entrada_ConsultaUbicacionIDResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                List<ubicacion_Entrada_ConsultaUbicacionIDResult> objin = consulta.ubicacion_Entrada_ConsultaUbicacionID(idUbicacion).ToList();
                return objin;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return objInv;
            }
        }

        public List<ubicacion_Entrada_ConsultaUbicacionDetalleIDResult> ConsultaUbicacionDetalleID(int idUbicacion)
        {
            List<ubicacion_Entrada_ConsultaUbicacionDetalleIDResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                List<ubicacion_Entrada_ConsultaUbicacionDetalleIDResult> objin = consulta.ubicacion_Entrada_ConsultaUbicacionDetalleID(idUbicacion).ToList();
                return objin;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return objInv;
            }
        }

        public int GuardaUbicacion(int id, int id_caja, string po, int id_Cliente, int id_Facturacion, int id_Terminado, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ubicacion_Entrada_GuardarUbicacionResult insertInventario = consulta.ubicacion_Entrada_GuardarUbicacion(id, id_caja, po, id_Cliente, id_Facturacion, id_Terminado, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = insertInventario.Column1;
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }



        public DataTable ConsultaTablaGeneral(string NombreSP, List<clsParametro> lst)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter miadapter = new SqlDataAdapter();
            SqlConnection MiConexion = new SqlConnection(global::Inventario.Properties.Settings.Default.InventarioConnectionString4);
            DataTable dt = new DataTable();
            SqlDataAdapter da;
            try
            {
                da = new SqlDataAdapter(NombreSP, MiConexion);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                if (lst != null)
                {
                    for (int i = 0; i < lst.Count; i++)
                    {
                        da.SelectCommand.Parameters.AddWithValue(lst[i].Nombre, lst[i].Valor);
                    }
                }
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public void ConsultaTallas(CheckedListBox clbT)
        {
            try
            {
                BDDataContext consulta = new BDDataContext();
                List<ConsultaTallasResult> Listtallas = consulta.ConsultaTallas().ToList();

                foreach (ConsultaTallasResult talla in Listtallas)
                {
                    clbT.Items.Add(talla.size);
                    //clbT.fill
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public List<ubicacion_Salida_ConsultaPOTallasCantidadResult> ConsultaPOTallasCantidad(string po, string talla)
        {
            List<ubicacion_Salida_ConsultaPOTallasCantidadResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ubicacion_Salida_ConsultaPOTallasCantidadResult> objin = consulta.ubicacion_Salida_ConsultaPOTallasCantidad(po, talla).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }
        }

        public ubicacion_Entrada_ComprobarCajaResult ComprobarCaja(int idCaja)
        {
            ubicacion_Entrada_ComprobarCajaResult objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                ubicacion_Entrada_ComprobarCajaResult objin = consulta.ubicacion_Entrada_ComprobarCaja(idCaja).FirstOrDefault();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }
        }

        public int GuardarSalida(int id_caja, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();
                consulta.ubicacion_Salida_Guardar(id_caja, iduser);
                consulta.SubmitChanges();
                respuesta = 1;
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public int Terminar(int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();
                consulta.ubicacion_Salida_Terminar(iduser);
                consulta.SubmitChanges();
                respuesta = 1;
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public int MoverUbicacion(int iduser, int idUbicacion, int idCaja)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ubicacion_MoverUbicacion_GuardarResult respuesta1 = consulta.ubicacion_MoverUbicacion_Guardar(iduser, idUbicacion, idCaja).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.Column1;
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public string ConsultaMarca(string po_numero, string po_item)
        {
            string respuesta = "";
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaMarcaResult respuesta1 = consulta.ConsultaMarca(po_numero, po_item).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.marca;
            }
            catch (Exception ex)
            {
                respuesta = "";
                // MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public int ComprobarCajaPO(string po, string cliente, string factura, string terminado, int idCaja, bool? pOSolamente)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ubicacion_Entrada_ComprobarCajaPOResult respuesta1 = consulta.ubicacion_Entrada_ComprobarCajaPO(idCaja, po, cliente, factura, terminado, pOSolamente).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.Column1;
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public List<ubicacion_ReporteAlmacen_ConsultaResult> ConsultaAlmacen(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ubicacion_ReporteAlmacen_ConsultaResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ubicacion_ReporteAlmacen_ConsultaResult> objin = consulta.ubicacion_ReporteAlmacen_Consulta(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }

        public List<ubicacion_Entrada_ConsultaUbicacionDetalleResult> ConsultaUbicacion()
        {
            List<ubicacion_Entrada_ConsultaUbicacionDetalleResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ubicacion_Entrada_ConsultaUbicacionDetalleResult> objin = consulta.ubicacion_Entrada_ConsultaUbicacionDetalle().ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }

        public List<ubicacion_ReporteEmbarques_ConsultaResult> ConsultaEmbarques(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ubicacion_ReporteEmbarques_ConsultaResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ubicacion_ReporteEmbarques_ConsultaResult> objin = consulta.ubicacion_ReporteEmbarques_Consulta(fechaInicio, fechaFin).ToList();

                return objin;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }
        }




        public ubicacion_Dividir_CajaIDResult DividirCaja(int idCaja, int idUser, int cantidad, int restante)
        {
            ubicacion_Dividir_CajaIDResult respuesta = new ubicacion_Dividir_CajaIDResult();
            try
            {
                BDDataContext consulta = new BDDataContext();
                respuesta = consulta.ubicacion_Dividir_CajaID(idCaja, idUser, cantidad, restante).FirstOrDefault();
                consulta.SubmitChanges();
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public ubicacion_Dividir_ConsultaCajaIDResult ConsultaCajaID(int idCaja)
        {
            ubicacion_Dividir_ConsultaCajaIDResult respuesta = new ubicacion_Dividir_ConsultaCajaIDResult();
            try
            {
                BDDataContext consulta = new BDDataContext();
                respuesta = consulta.ubicacion_Dividir_ConsultaCajaID(idCaja).FirstOrDefault();
                consulta.SubmitChanges();
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public List<ConsultaInventarioPorHoraResult> ConsultaInventarioPorHora(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaInventarioPorHoraResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaInventarioPorHoraResult> objin = consulta.ConsultaInventarioPorHora(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public int ConsultarSalida(int idCaja)
        {
            ubicacion_Salida_ConsultarSalidaResult objInv = new ubicacion_Salida_ConsultarSalidaResult();
            try
            {
                BDDataContext consulta = new BDDataContext();

                ubicacion_Salida_ConsultarSalidaResult objin = consulta.ubicacion_Salida_ConsultarSalida(idCaja).FirstOrDefault();

                return objin.Column1;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return 0;
            }

        }
        public int ConsultarEntrada(int idCaja)
        {
            ubicacion_Entrada_ConsultarEntradaResult objInv = new ubicacion_Entrada_ConsultarEntradaResult();
            try
            {
                BDDataContext consulta = new BDDataContext();

                ubicacion_Entrada_ConsultarEntradaResult objin = consulta.ubicacion_Entrada_ConsultarEntrada(idCaja).FirstOrDefault();

                return objin.Column1;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return 0;
            }

        }

        public int GuardarPrePack(Prepack p)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarPrePackResult insertInventario = consulta.GuardarPrePack(p.po_numero, p.estilo, p.DPCI, p.barcode).FirstOrDefault();

                consulta.SubmitChanges();
                respuesta = Convert.ToInt32(insertInventario.Column1);
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public int GuardarPrePackDetalle(PrepackDetalle pd)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                int insertInventario = consulta.GuardarPrePackDetalle(pd.idPrepack, pd.size, pd.cantidad, pd.CantidadCajas, pd.upc, pd.idusuario, pd.idSize);

                consulta.SubmitChanges();
                // respuesta = Convert.ToInt32(insertInventario.Column1);
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public string ConsultaCajasBaja(string po_numero, string talla)
        {
            string respuesta = "";
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaCajasBajaResult respuesta1 = consulta.ConsultaCajasBaja(po_numero, talla).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.numeroCaja;
            }
            catch (Exception ex)
            {
                respuesta = "";
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public string ConsultaNumeroCajaPO(string po_numero, string NumeroCaja)
        {
            int? respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaNumeroCajaPOResult respuesta1 = consulta.ConsultaNumeroCajaPO(po_numero, NumeroCaja).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.numeroCaja;
            }
            catch (Exception ex)
            {
                respuesta = 0;
                MessageBox.Show(ex.Message);
            }
            return respuesta.ToString();
        }



        public ConsultaTallasColorResult ConsultaTallasColor(string estilo)
        {
            ConsultaTallasColorResult respuesta = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaTallasColorResult respuesta1 = consulta.ConsultaTallasColor(estilo).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1;
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public string ConsultaTallasIFRcodigoupc(string size_izquierdo)
        {
            string respuesta = "";
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaTallasIFRcodigoupcResult respuesta1 = consulta.ConsultaTallasIFRcodigoupc(size_izquierdo).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.upcCaja;
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }



        public ConsultaTallasColorIFRResult ConsultaTallasColorIFR(string estilo)
        {
            ConsultaTallasColorIFRResult respuesta = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaTallasColorIFRResult respuesta1 = consulta.ConsultaTallasColorIFR(estilo).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1;
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }



        public ConsultaColorResult ConsultaColor(string estilo, string marca)
        {
            ConsultaColorResult respuesta = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaColorResult respuesta1 = consulta.ConsultaColor(estilo, marca).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1;
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public ConsultaTallasColorTYNDALEResult ConsultaTallasColorTYNDALE(string estilo)
        {
            ConsultaTallasColorTYNDALEResult respuesta = null;
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaTallasColorTYNDALEResult respuesta1 = consulta.ConsultaTallasColorTYNDALE(estilo).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1;
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public Prendas ConsultaPrendas_UPC(string po_numero, string upc)
        {
            Prendas respuesta = new Prendas();
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaPrendas_UPCResult respuesta1 = consulta.ConsultaPrendas_UPC(po_numero, upc).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta.diferencia = respuesta1.diferencia;
                respuesta.prendasescaneadas = respuesta1.Escaneadas.ToString();
                respuesta.totalprendas = respuesta1.Total.ToString();
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }
        public Prendas ConsultaPrendas_Talla(string po_numero, string talla)
        {
            Prendas respuesta = new Prendas();
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaPrendas_TallaResult respuesta1 = consulta.ConsultaPrendas_Talla(po_numero, talla).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta.diferencia = respuesta1.diferencia;
                respuesta.prendasescaneadas = respuesta1.Escaneadas.ToString();
                respuesta.totalprendas = respuesta1.Total.ToString();
            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public string ConsultaUPCBENS(int? id)
        {
            String respuesta = "";
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaUPCBENSResult respuesta1 = consulta.ConsultaUPCBENS(id).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.upc;
            }
            catch (Exception ex)
            {
                respuesta = "";
                MessageBox.Show(ex.Message);
            }
            return respuesta.ToString();
        }

        public string ConsultaUPCIFR(int? id)
        {
            String respuesta = "";
            try
            {
                BDDataContext consulta = new BDDataContext();
                ConsultaUPCIFRResult respuesta1 = consulta.ConsultaUPCIFR(id).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = respuesta1.upc;
            }
            catch (Exception ex)
            {
                respuesta = "";
                MessageBox.Show(ex.Message);
            }
            return respuesta.ToString();
        }

        public string ConsultaUPCTYNDALE(int? id)
        {
            String respuesta = "";
            try
            {
                BDDataContext consulta = new BDDataContext();
                // Primera fila o null si no hay resultados
                var respuesta1 = consulta.ConsultaUPCTYNDALE(id).FirstOrDefault();
                // SubmitChanges no es necesario para operaciones de solo lectura
                // consulta.SubmitChanges();
                // Comprobar null antes de acceder a propiedades
                respuesta = respuesta1?.upc ?? "";
            }
            catch (Exception ex)
            {
                respuesta = "";
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }



        public List<ConsultaProductosBensResult> ConsultaProductosBens(string po, string size_izquierdo, string size_derecho)
        {
            List<ConsultaProductosBensResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                //decimal? po_numero = 0;
                //po_numero = Convert.ToInt64(po);
                List<ConsultaProductosBensResult> Producto = consulta.ConsultaProductosBens(po.ToString(), size_izquierdo, size_derecho).ToList();

                return Producto;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return Productonull;
            }
        }

        public List<ConsultaProductosBensEscaneoResult> ConsultaProductosBensEscaneo(string po, string size_izquierdo, string size_derecho)
        {
            List<ConsultaProductosBensEscaneoResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaProductosBensEscaneoResult> Producto = consulta.ConsultaProductosBensEscaneo(po.ToString(), size_izquierdo, size_derecho).ToList();

                return Producto;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return Productonull;
            }
        }
        public List<ConsultaProductosBensIDResult> ConsultaProductosBensID(int? id)
        {
            List<ConsultaProductosBensIDResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaProductosBensIDResult> Producto = consulta.ConsultaProductosBensID(id).ToList();

                return Producto;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return Productonull;
            }
        }


        /// <summary>
        /// Siguiente numero de carton Ben Davis para el PO (mismo criterio que GuardarInventarioBeans:
        /// MAX(cn_tag_num)+1 de las cajas no canceladas del po_numero).
        /// </summary>
        public int SiguienteCartonBens(string po_numero)
        {
            BDDataContext consulta = new BDDataContext();
            long? maximo = consulta.Inventario
                .Where(i => i.po_numero == po_numero && i.id_status != 4)
                .Max(i => i.cn_tag_num);
            return Convert.ToInt32((maximo ?? 0) + 1);
        }

        public EtiquetaCajaModificada GuardaInventarioBeans(EtiquetaBens et, int iduser)
        {
            EtiquetaCajaModificada respuesta = new EtiquetaCajaModificada();
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarInventarioBeansResult insertInventario = consulta.GuardarInventarioBeans(et.id,
                                                                                      et.po,
                                                                                       et.po_varchar,
                                                                                       et.poItem,
                                                                                      et.Cantidad,
                                                                                      et.size_izquierdo,
                                                                                      et.size_derecho,
                                                                                      et.upc,
                                                                                      et.cn_tag_num,
                                                                                      et.ESTILO,
                                                                                      iduser,
                                                                                      et.color,
                                                                                      et.itemDescription
                                                                                      ).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta.codigoBarrasTextoB = insertInventario.codigoBarrasTexto.ToString();

            }
            catch (Exception ex)
            {
                respuesta = null;
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public string GuardaTallaEstilos(tallas t)
        {
            string respuesta = "";
            try
            {

                BDDataContext consulta = new BDDataContext();

                consulta.tallas.InsertOnSubmit(t);
                consulta.SubmitChanges();
            }
            catch (Exception ex)
            {
                respuesta = "";
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }


        public string GuardaEstilos(estilo e)
        {
            string respuesta = "";
            try
            {

                BDDataContext consulta = new BDDataContext();

                consulta.estilo.InsertOnSubmit(e);
                consulta.SubmitChanges();
            }
            catch (Exception ex)
            {
                respuesta = "";
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public string ActualizaInventario(string po, int iduser)
        {
            string respuesta = "";
            try
            {
                BDDataContext consulta = new BDDataContext();

                ActualizaInventarioPOResult insertInventario = consulta.ActualizaInventarioPO(po, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = insertInventario.id.ToString();
            }
            catch (Exception ex)
            {
                respuesta = "";
                MessageBox.Show(ex.Message);
            }
            return respuesta;
        }



    }
}
