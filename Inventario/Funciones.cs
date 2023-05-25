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
                List<ConsultaPOResult> po = new List<ConsultaPOResult>();

                combo.DataSource = null;
                combo.Items.Clear();

                po = consulta.ConsultaPO().ToList();

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

        public List<ConsultaTodoPOResult> ConsultaTodoPO(decimal po)
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

                System.Data.Linq.ISingleResult<ConsultaPOItemResult> poItem = consulta.ConsultaPOItem(po);

                combo.DataSource = poItem;
                combo.DisplayMember = "po_item";
                combo.ValueMember = "po_item";

            }

            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);

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
                // MessageBox.Show(ex.Message);

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

        public List<ConsultaProductosNuevoResult> ConsultaProductos(string po, string poItem, string prodCd, string sizeIzquierdo, string sizeDerecho)
        {
            List<ConsultaProductosNuevoResult> Productonull = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaProductosNuevoResult> Producto = consulta.ConsultaProductosNuevo(Convert.ToInt64(po), poItem, prodCd, sizeIzquierdo.Trim(), sizeDerecho.Trim()).ToList();

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

                decimal? po_numero = 0;
                po_numero = Convert.ToDecimal(po);
                List<ConsultaProductosZumiesResult> Producto = consulta.ConsultaProductosZumies(po_numero, poItem, prodCd, size).ToList();

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

                List<ConsultaProductosTargetResult> Producto = consulta.ConsultaProductosTarget(Convert.ToInt64(po), poItem, prodCd, size).ToList();

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



        public int GuardaInventario(EtiquetaCajaModificada et, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarInventarioResult insertInventario = consulta.GuardarInventario(et.id,
                                                                                      et.po,
                                                                                      et.poItem,
                                                                                      et.Cantidad,
                                                                                      et.size_izquierdo,
                                                                                      et.size_derecho,
                                                                                      et.upc, et.Carton,
                                                                                      et.ProductCode,
                                                                                      et.TipoCarton,
                                                                                      iduser,
                                                                                      et.id_cliente,
                                                                                      et.id_factura,
                                                                                      et.id_terminado).FirstOrDefault();
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

        public int GuardaInventarioZumies(EtiquetaZUMIES et, int iduser)
        {
            int respuesta = 0;
            try
            {
                BDDataContext consulta = new BDDataContext();

                GuardarInventarioResult insertInventario = consulta.GuardarInventario(et.id,
                                                                                      et.po,
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
                                                                                      et.id_terminado).FirstOrDefault();
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

                consulta.GuardarAltaPOManual(et.po,
                                          et.Cantidad,
                                          et.upc,
                                          et.idSize,
                                          et.idusuario).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = 1;
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

                consulta.GuardarZumies(et.po,
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
                respuesta = 1;
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

                consulta.GuardarTarget(et.po,
                                          et.Cantidad,
                                          et.idSize,
                                          et.escaneado,
                                          et.idusuario,
                                          et.assembly,
                                          et.itemDescription,
                                          et.upc).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = 1;
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
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
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
                                          et.Cantidad, iduser).FirstOrDefault();
                consulta.SubmitChanges();
                respuesta = 1;
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
            return respuesta;
        }

        public List<ConsultaInventarioResult> ConsultaInventario(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaInventarioResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaInventarioResult> objin = consulta.ConsultaInventario(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public List<ConsultaAlmacenesResult> ConsultaAlmacenes(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaAlmacenesResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext
                {
                    CommandTimeout = 2500
                };
                List<ConsultaAlmacenesResult> objin = consulta.ConsultaAlmacenes(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public List<ConsultaAlmacenesDTLResult> ConsultaAlmacenesDTL(DateTime? fechaInicio, DateTime? fechaFin)
        {
            List<ConsultaAlmacenesDTLResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext
                {
                    CommandTimeout = 2500
                };
                List<ConsultaAlmacenesDTLResult> objin = consulta.ConsultaAlmacenesDTL(fechaInicio, fechaFin).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }

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

                List<ConsultaInventarioPOResult> objin = consulta.ConsultaInventarioPO(po_numero).ToList();

                return objin;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return objInv;
            }

        }
        public List<ConsultaUPCResult> ConsultaUPC(string po_numero, string upc)
        {
            List<ConsultaUPCResult> objInv = null;
            try
            {
                BDDataContext consulta = new BDDataContext();

                List<ConsultaUPCResult> objin = consulta.ConsultaUPC(po_numero, upc).ToList();

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
            SqlConnection MiConexion = new SqlConnection(@"Data Source =AIN-MSSRV\SISTEMASAIN;Initial Catalog=Inventario;uid=sa;pwd=SisAin03");
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

                int insertInventario = consulta.GuardarPrePackDetalle(pd.idPrepack, pd.size, pd.cantidad, pd.upc, pd.idusuario, pd.idSize);

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

    }
}
