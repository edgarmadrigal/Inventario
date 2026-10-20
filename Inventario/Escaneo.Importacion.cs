using Inventario.Reportes;
using Excel;
using Inventario.Packaging;
using Microsoft.VisualBasic;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;
using ZXing;
using ZXing.Common;

namespace Inventario
{
    // Pestana Importar Archivo: LEVIS, TARGET, Zumies, contratos y estilos desde Excel
    public partial class Escaneo
    {
        /// <summary>
        /// PARA importar EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportar_Click(object sender, EventArgs e)
        {
            try
            {
                importar();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void importar()
        {

            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();

            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 2)
                    {
                        string upc = dr[6].ToString();
                        upc = upc.Replace("-", "");
                        upc = upc.Replace(" ", "");
                        try
                        {
                            EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                            {
                                po = dr[0].ToString(),
                                poInCompleto = Convert.ToInt64(dr[0].ToString().PadRight(6)),
                                poItem = null,
                                ProductCode = dr[9].ToString()
                            };
                            string[] separadas;
                            separadas = dr[6].ToString().Split('-');
                            clase.Size = separadas[1].ToString().Substring(0, 5).TrimStart('0') + "x" + separadas[1].ToString().Substring(5, 2);
                            clase.size_izquierdo = separadas[1].ToString().Substring(0, 5).TrimStart('0');
                            clase.size_derecho = separadas[1].ToString().Substring(5, 2);
                            clase.TipoCarton = null;
                            clase.assembly = dr[10].ToString();
                            clase.Vendor = dr[2].ToString();
                            clase.ShipTo = dr[3].ToString();
                            clase.upc = upc;
                            clase.Fecha = DateTime.Now;
                            clase.CartonLeft = dr[7].ToString();
                            clase.CartonRight = separadas[0].ToString() + separadas[1].ToString();
                            clase.Cantidad = Convert.ToInt64(dr[11].ToString());
                            clase.Carton = null;
                            clase.usuario = usu[0].nombre;
                            clase.id_inventario = f.GuardaProducto(clase, usu[0].id);
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            cmbPO.DataSource = null;
            cmbPO.Items.Clear();
            cmbPOB.DataSource = null;
            cmbPOB.Items.Clear();

            f.ConsultaPO(cmbPO);
            cmbPO.SelectedIndex = -1;
            f.ConsultaPO(cmbPOB);
            MessageBox.Show("Termino con Exito!");
        }

        private void btnImportarLevis_Click(object sender, EventArgs e)
        {
            try
            {
                importarLEVIS();
                Cursor.Current = Cursors.Default;

                f.ConsultaPO(cmbPO);
                cmbPO.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void importarLEVIS()
        {

            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();

            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 1)
                    {

                        try
                        {
                            string upc = dr[3].ToString();
                            if (upc.Length > 5)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                                {
                                    po = dr[0].ToString(),
                                    poInCompleto = 0,
                                    poItem = null,
                                    ProductCode = "",
                                    Size = "",
                                    size_izquierdo = dr[1].ToString(),
                                    size_derecho = dr[2].ToString(),
                                    TipoCarton = null,
                                    assembly = "",
                                    Vendor = "",
                                    ShipTo = ""
                                };
                                if (upc.Length < 12)
                                {
                                    upc = "0" + upc;
                                    // MessageBox.Show("Debe ingresar como minimo 12 caracteres");
                                }
                                else
                                {

                                }
                                clase.upc = upc;
                                clase.Fecha = DateTime.Now;
                                clase.CartonLeft = "";
                                clase.CartonRight = "";
                                clase.Cantidad = Convert.ToInt64(dr[4].ToString());
                                clase.Carton = null;
                                clase.usuario = usu[0].nombre;
                                clase.id_inventario = f.GuardaProductoLEVIS(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
        }


        public void importarContrato()
        {
            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    /*Validar tallas que existan si no que el sistema avise sobre esa talla ,y que el upc empieze con 19*/
                    /*
                    switch (cmbMarca.SelectedItem)
                    {
                        case "LEVIS":
                            if (validarAltaPO() == 1)
                            {
                                pd.po = txtzPOZumies.Text;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idusuario = usu[0].id;
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                TallaRepetida = f.GuardaAltaPO(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                            }
                            break;
                        case "ZUMIES":
                            if (validarZumies() == 1)
                            {
                                pd.po = txtzPOZumies.Text;
                                pd.estilo = txtzEstilo.Text;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[2].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                pd.itemDescription = renglon.Cells[5].Value.ToString();
                                pd.color = renglon.Cells[6].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                TallaRepetida = f.GuardaZumies(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada Favor de dar de Baja la Talla");
                                    TallaRepetida = 0;
                                }
                            }
                            break;
                        case "ROCKY":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;

                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[4].Value.ToString());
                                pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[2].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[3].Value.ToString());
                                pd.Size = renglon.Cells[0].Value.ToString();
                                pd.itemDescription = renglon.Cells[5].Value.ToString();
                                pd.color = renglon.Cells[6].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                pd.marca = "ROCKY";
                                TallaRepetida = f.GuardarPO(pd);/// guardar para ROCKY Y OLDNAVY

                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                                //cmbPO.SelectedIndex = -1;
                            }
                            break;
                        case "OLDNAVY":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;
                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                pd.CantidadCajas = Convert.ToDecimal(0);
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.color = renglon.Cells[5].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                pd.marca = "OLDNAVY";
                                TallaRepetida = f.GuardarPO(pd);/// guardar para ROCKY Y OLDNAVY
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                                //cmbPO.SelectedIndex = -1;

                            }
                            break;
                        case "TARGET":
                            if (validarAltaPO() == 1)
                            {
                                pd.po = txtzPOZumies.Text;
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                pd.assembly = txtzPOZumies.Text;
                                TallaRepetida = f.GuardarTarget(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //f.ConsultaPO(cmbPO);
                            }
                            break;
                        case "BENS":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;
                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                //pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.color = renglon.Cells[5].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                TallaRepetida = f.GuardaBENS(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //cmbPO.SelectedIndex = -1;
                            }
                            break;
                        case "SAFTECH":
                            if (validarAltaPO() == 1)
                            {
                                string poCompleto = txtzPOZumies.Text;
                                string poSoloNumeros = String.Empty;
                                pd.po = txtzPOZumies.Text;
                                pd.po_varchar = poCompleto.Trim();
                                pd.estilo = cmbEstilo.Text;
                                pd.Vendor = poCompleto;
                                pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                //pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[1].Value.ToString());
                                pd.upc = renglon.Cells[1].Value.ToString();
                                pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                pd.itemDescription = renglon.Cells[4].Value.ToString();
                                pd.color = renglon.Cells[5].Value.ToString();
                                pd.escaneado = 0;
                                pd.idusuario = usu[0].id;
                                TallaRepetida = f.GuardarSaftech(pd);
                                if (TallaRepetida > 0)
                                {
                                    MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                    TallaRepetida = 0;
                                }
                                //cmbPO.SelectedIndex = -1;
                            }
                            break;
                    }*/
                    f.ConsultaPO(cmbPO);
                    cmbPO.SelectedIndex = -1;
                    MessageBox.Show("Termino con Exito!");

                    if (Contador > 1)
                    {
                        try
                        {
                            string upc = dr[3].ToString();
                            if (upc.Length > 5)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
                                clase.po = dr[0].ToString();
                                clase.poInCompleto = Convert.ToDecimal(clase.po);
                                clase.poItem = "77";
                                clase.ProductCode = "";
                                clase.Size = "";
                                clase.size_izquierdo = dr[1].ToString();
                                clase.size_derecho = dr[2].ToString();
                                clase.TipoCarton = null;
                                clase.assembly = "";
                                clase.Vendor = "";
                                clase.ShipTo = "";
                                clase.upc = upc;
                                clase.Fecha = DateTime.Now;
                                clase.CartonLeft = "";
                                clase.CartonRight = "";
                                clase.Cantidad = Convert.ToInt64(dr[4].ToString());
                                clase.Carton = null;
                                clase.usuario = usu[0].nombre;
                                clase.id_inventario = f.GuardaProductoLEVIS(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                    /*****/
                }
            }
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                importarTARGET();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void importarTARGET()
        {
            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            Cursor.Current = Cursors.WaitCursor;
            int idPrepack = 0;
            int banderaprepack = 0;
            string po = "";
            int bandera = 0;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 1)
                    {
                        try
                        {
                            string upc = dr[2].ToString();
                            if (upc.Length > 5)
                            {
                                if (dr[6].ToString() == "")
                                {
                                    /* NO ES PREPACK*/
                                    EtiquetaCajaModificada clase = new EtiquetaCajaModificada();
                                    clase.esprepack = 0;
                                    clase.po = dr[0].ToString();
                                    po = clase.po.ToString();
                                    clase.upc = dr[1].ToString();
                                    clase.size_izquierdo = dr[2].ToString().Trim();
                                    clase.DPCI = dr[3].ToString();
                                    clase.Cantidad = Convert.ToInt64(dr[4].ToString());
                                    clase.CantidadCajas = Convert.ToInt64(dr[5].ToString());
                                    clase.barcode = dr[6].ToString();
                                    clase.Fecha = DateTime.Now;
                                    clase.usuario = usu[0].nombre;
                                    clase.id_inventario = f.GuardaProductoTARGET(clase, usu[0].id);
                                }
                                else
                                {
                                    if (idPrepack == 0)
                                    {
                                        Prepack p = new Prepack();
                                        p.po_numero = dr[0].ToString();
                                        p.estilo = "88";
                                        p.DPCI = dr[3].ToString();
                                        p.barcode = dr[6].ToString();
                                        idPrepack = f.GuardarPrePack(p);
                                    }
                                    /* ES PREPACK*/
                                    int cantidad = Convert.ToInt32(dr[4].ToString());
                                    for (int i = cantidad; i > 0; i--)
                                    {
                                        PrepackDetalle pd = new PrepackDetalle();
                                        pd.idPrepack = idPrepack;
                                        pd.size = dr[2].ToString().Trim();
                                        pd.cantidad = 1;
                                        pd.CantidadCajas = 0;
                                        if (bandera == 0)
                                        {
                                            pd.CantidadCajas = Convert.ToInt64(dr[5].ToString());
                                            bandera = bandera + 1;
                                        }
                                        pd.upc = dr[1].ToString();
                                        pd.idusuario = usu[0].id;
                                        pd.idSize = Convert.ToInt32(0);

                                        f.GuardarPrePackDetalle(pd);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }

            Cursor.Current = Cursors.Default;
            MessageBox.Show("Termino con Exito!");
            ActualizarPO(po);
        }

        private void btnImportarZumies_Click(object sender, EventArgs e)
        {
            try
            {
                importarZumies();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void importarZumies()
        {
            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();

            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 1)
                    {
                        try
                        {
                            string dpci = dr[8].ToString();
                            if (dpci.Length > 1)
                            {
                                EtiquetaCajaModificada clase = new EtiquetaCajaModificada
                                {
                                    po = dr[9].ToString(),
                                    poInCompleto = 0,
                                    poItem = null,
                                    ProductCode = dr[9].ToString(),
                                    Size = dr[18].ToString(),
                                    size_izquierdo = dr[18].ToString(),
                                    size_derecho = "",
                                    TipoCarton = null,
                                    assembly = dr[46].ToString(),
                                    Vendor = dr[7].ToString(), /*upc 2*/
                                    ShipTo = dr[53].ToString(),
                                    upc = dr[2].ToString(),
                                    Fecha = DateTime.Now,
                                    CartonLeft = "",
                                    CartonRight = "",
                                    Cantidad = Convert.ToInt64(dr[31].ToString()),
                                    Carton = null,
                                    usuario = usu[0].nombre
                                };

                                // clase.id_inventario = f.GuardaProductoTARGET(clase, usu[0].id);
                            }
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            MessageBox.Show("Termino con Exito!");
        }

        private void btnImportarTARGET_Click(object sender, EventArgs e)
        {
            try
            {
                importarTARGET();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImportar_Click_1(object sender, EventArgs e)
        {
            try
            {
                //importarROCKY();
                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tpImportaLEVIS_Click(object sender, EventArgs e)
        {

        }

        private void btnGuardarEstiloAlta_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelarEstiloAlta_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregarEstiloAlta_Click(object sender, EventArgs e)
        {
            try
            {
                tablaEstiloAlta = new DataTable();
                tablaEstiloAlta.Columns.Add("Talla", typeof(string));
                tablaEstiloAlta.Columns.Add("Codigo UPC", typeof(string));
                tablaEstiloAlta.Columns.Add("idSize", typeof(int));
                tablaEstiloAlta.Columns.Add("CantidadPrendas", typeof(long));
                tablaEstiloAlta.Columns.Add("itemDescription", typeof(string));
                tablaEstiloAlta.Columns.Add("color", typeof(string));
                altaZ = altaZ + 1;
                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                dgvZumies.DataSource = tablaTarget;
                dgvZumies.Columns["idSize"].Visible = false;
                cmbzTallas.SelectedIndex = 0;
                txtzUPC.Text = "";

            }
            catch (Exception ex)
            {

            }
        }

        private void btnEliminaEstiloAlta_Click(object sender, EventArgs e)
        {

        }

        private void btnImportarEstilo_Click(object sender, EventArgs e)
        {
            try
            {
                importarEstilo();
                Cursor.Current = Cursors.Default;

                f.ConsultaPO(cmbPO);
                cmbPO.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


        public void importarEstilo()
        {

            int Contador = 0;
            OpenFileDialog ope = new OpenFileDialog
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            if (ope.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            FileStream stream = new FileStream(ope.FileName, FileMode.Open);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            string estilo1 = "";
            string tipo = "";
            string color = "";
            string descripcion = "";

            Cursor.Current = Cursors.WaitCursor;
            foreach (DataTable table in result.Tables)
            {
                foreach (DataRow dr in table.Rows)
                {
                    Contador = Contador + 1;
                    if (Contador > 1)
                    {

                        try
                        {
                            string upc = dr[2].ToString();
                            if (upc.Length > 5)
                            {
                                tallas clase = new tallas
                                {
                                    size_izquierdo = dr[0].ToString(),
                                    size_derecho = dr[1].ToString(),
                                    upc = dr[2].ToString(),
                                    tipo = dr[3].ToString(),
                                    estilo = dr[4].ToString(),
                                    color = dr[5].ToString(),
                                    descripcion = dr[6].ToString(),
                                };
                                f.GuardaTallaEstilos(clase);



                            }
                            estilo1 = dr[4].ToString();
                            tipo = dr[3].ToString();
                            color = dr[5].ToString();
                            descripcion = dr[6].ToString();
                        }
                        catch (Exception)
                        {
                            // MessageBox.Show(ex.Message);
                        }
                    }
                }
            }
            estilo Nuevoestilo = new estilo();

            Nuevoestilo.estilo1 = estilo1;
            Nuevoestilo.tipo = tipo;
            Nuevoestilo.color = color;
            Nuevoestilo.descripcion = descripcion;

            f.GuardaEstilos(Nuevoestilo);


            MessageBox.Show("Termino con Exito!");
        }
    }
}
