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
    // Pestana AltaPO: alta de PO por marca (Zumies, IFR, Bens, TYNDALE, etc.)
    public partial class Escaneo
    {
        public int validarAltaPO()
        {
            try
            {
                if (string.IsNullOrEmpty(txtzPOZumies.Text) && dgvZumies.RowCount < 1)
                {
                    MessageBox.Show("Favor de llenar todos los campos.");

                    return 0;

                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }


        private void btnzGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                btnzGuardar.Enabled = false;
                if (dgvZumies.RowCount > 0)
                {
                    foreach (DataGridViewRow renglon in dgvZumies.Rows)
                    {
                        EtiquetaCajaModificada pd = new EtiquetaCajaModificada();
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
                                    pd.estilo = txtzEstilo.Text;
                                    pd.color = txtzColor.Text;

                                    TallaRepetida = f.GuardaAltaPO(pd);
                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                    //f.ConsultaPO(cmbPO);
                                }
                                break;
                            case "WRANGLER":
                                if (validarAltaPO() == 1)
                                {
                                    pd.po = txtzPOZumies.Text;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.estilo = txtzEstilo.Text;
                                    pd.color = txtzColor.Text;
                                    pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[6].Value.ToString());

                                    TallaRepetida = f.GuardaWrangler(pd);
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
                                    TallaRepetida = f.GuardarTarget(pd);///**/
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
                            case "IFR":
                                if (validarIFR() == 1)
                                {
                                    string poCompleto = txtzPOZumies.Text;
                                    string poSoloNumeros = String.Empty;
                                    pd.po = txtzPOZumies.Text;
                                    pd.po_varchar = poCompleto.Trim();
                                    pd.estilo = cmbEstilo.Text;
                                    pd.Vendor = poCompleto;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[6].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.Size = renglon.Cells[0].Value.ToString();
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.color = renglon.Cells[5].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    pd.marca = "IFR";
                                    TallaRepetida = f.GuardaIFR(pd);


                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                }
                                break;
                            case "TYNDALE":
                                if (validarIFR() == 1) // misma validación que IFR: PO + al menos 1 renglón
                                {
                                    string poCompleto = txtzPOZumies.Text;
                                    pd.po = txtzPOZumies.Text;
                                    pd.po_varchar = poCompleto.Trim();
                                    pd.estilo = cmbEstilo.Text;
                                    pd.Vendor = poCompleto;
                                    pd.Cantidad = Convert.ToDecimal(renglon.Cells[3].Value.ToString());
                                    pd.CantidadCajas = Convert.ToDecimal(renglon.Cells[6].Value.ToString());
                                    pd.upc = renglon.Cells[1].Value.ToString();
                                    pd.idSize = Convert.ToInt32(renglon.Cells[2].Value.ToString());
                                    pd.Size = renglon.Cells[0].Value.ToString();
                                    pd.itemDescription = renglon.Cells[4].Value.ToString();
                                    pd.color = renglon.Cells[5].Value.ToString();
                                    pd.idusuario = usu[0].id;
                                    pd.marca = "TYNDALE";
                                    TallaRepetida = f.GuardaTYNDALE(pd);

                                    if (TallaRepetida > 0)
                                    {
                                        MessageBox.Show("La Talla " + renglon.Cells[0].Value.ToString() + " ya estaba registrada , !Favor de dar de Baja la Talla!");
                                        TallaRepetida = 0;
                                    }
                                }
                                break;
                        }
                        alta = 0;
                    }
                    f.ConsultaPO(cmbPO);
                    LimpiarCamposZumies();
                    cmbPO.SelectedIndex = -1;
                    cmbMarca.SelectedIndex = 0;
                    cmbMarca.SelectedIndex = 4;
                    ActualizarPO(txtzPOZumies.Text);
                    f.ConsultaPO(cmbPOB);
                    MessageBox.Show("Termino con Exito!");
                    cmbMarca.SelectedItem = "LEVIS";
                }
                btnzGuardar.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnzGuardar.Enabled = true;
            }
        }

        public void LimpiarCamposZumies()
        {
            try
            {
                for (int i = dgvZumies.Rows.Count - 1; i >= 0; i--)
                {
                    dgvZumies.Rows.RemoveAt(i);
                }

                foreach (DataGridViewRow item in dgvZumies.SelectedRows)
                {
                    dgvZumies.Rows.RemoveAt(item.Index);

                }

                tablaZumies = new DataTable();
                tablaZumies.Columns.Add("Talla", typeof(string));
                tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                tablaZumies.Columns.Add("idSize", typeof(int));
                tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                tablaZumies.Columns.Add("itemDescription", typeof(string));
                tablaZumies.Columns.Add("color", typeof(string));

                tablaAltaPO = new DataTable();
                tablaAltaPO.Columns.Add("Talla", typeof(string));
                tablaAltaPO.Columns.Add("Cantidad", typeof(long));
                tablaAltaPO.Columns.Add("Codigo UPC", typeof(string));
                tablaAltaPO.Columns.Add("idSize", typeof(int));


                txtzPOZumies.Text = "";
                txtzEstilo.Text = "";
                //cmbzTallas.SelectedIndex = 0;
                txtzCantidadPrendas.Text = "";
                txtzCantidadCajas.Text = "";
                txtzColor.Text = "";
                txtzItemDescripcion.Text = "";
                txtzUPC.Text = "";
                altaZ = 0;

                cmbTallaMod.Text = "";
                txtCantPrendCaja.Text = "";
                txtCodigoUPC.Text = "";
                txtColorMod.Text = "";
                txtDescription.Text = "";
                txtEstiloPOMod.Text = "";
                cmbPOMod.Text = "";

                dgvZumies.Columns["itemDescription"].Visible = true;
                dgvZumies.Columns["color"].Visible = true;

                LimpiarCamposModificacionPO();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public int validarZumies()
        {
            try
            {
                if (string.IsNullOrEmpty(txtzPOZumies.Text) || string.IsNullOrEmpty(txtzEstilo.Text) || dgvZumies.RowCount < 1)
                {
                    MessageBox.Show("Favor de llenar todos los campos.");

                    return 0;

                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        public int validarIFR()
        {
            try
            {
                if (string.IsNullOrEmpty(txtzPOZumies.Text) || dgvZumies.RowCount < 1)
                {
                    MessageBox.Show("Favor de llenar todos los campos.");

                    return 0;

                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        private void btnzAgrega_Click(object sender, EventArgs e)
        {

            AgregarTallas();
        }

        public void AgregarTallas()
        {


            try
            {
                switch (cmbMarca.SelectedItem)
                {
                    case "TARGET":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaTarget = new DataTable();
                                tablaTarget.Columns.Add("Talla", typeof(string));
                                tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                                tablaTarget.Columns.Add("idSize", typeof(int));
                                tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                                tablaTarget.Columns.Add("itemDescription", typeof(string));
                                tablaTarget.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaTarget.Rows.Add(cmbzTallas.Text
                                                    , txtzUPC.Text
                                                    , cmbzTallas.SelectedValue.ToString()
                                                    , txtzCantidadPrendas.Text
                                                    , txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }

                        break;
                    case "ZUMIES":

                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != "" && txtzCantidadCajas.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaZumies = new DataTable();
                                tablaZumies.Columns.Add("Talla", typeof(string));
                                tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                                tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                                tablaZumies.Columns.Add("idSize", typeof(int));
                                tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                                tablaZumies.Columns.Add("itemDescription", typeof(string));
                                tablaZumies.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;

                                tablaZumies.Rows.Add(cmbzTallas.Text
                                                    , txtzCantidadCajas.Text
                                                    , txtzUPC.Text
                                                    , cmbzTallas.SelectedValue.ToString()
                                                    , txtzCantidadPrendas.Text
                                                    , txtzItemDescripcion.Text
                                                    , txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;


                    case "ROCKY":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaZumies = new DataTable();
                                tablaZumies.Columns.Add("Talla", typeof(string));
                                tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                                tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                                tablaZumies.Columns.Add("idSize", typeof(int));
                                tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                                tablaZumies.Columns.Add("itemDescription", typeof(string));
                                tablaZumies.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                //dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaZumies;
                                //dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;
                    case "OLDNAVY":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaBens = new DataTable();
                                tablaBens.Columns.Add("Talla", typeof(string));
                                tablaBens.Columns.Add("Codigo UPC", typeof(string));
                                tablaBens.Columns.Add("idSize", typeof(int));
                                tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                                tablaBens.Columns.Add("itemDescription", typeof(string));
                                tablaBens.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                dgvZumies.Columns["itemDescription"].Visible = false;
                                dgvZumies.Columns["color"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                dgvZumies.Columns["itemDescription"].Visible = false;
                                dgvZumies.Columns["color"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;

                    case "BENS":

                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaBens = new DataTable();
                                tablaBens.Columns.Add("Talla", typeof(string));
                                tablaBens.Columns.Add("Codigo UPC", typeof(string));
                                tablaBens.Columns.Add("idSize", typeof(int));
                                tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                                tablaBens.Columns.Add("itemDescription", typeof(string));
                                tablaBens.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;

                    case "SAFTECH":

                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaBens = new DataTable();
                                tablaBens.Columns.Add("Talla", typeof(string));
                                tablaBens.Columns.Add("Codigo UPC", typeof(string));
                                tablaBens.Columns.Add("idSize", typeof(int));
                                tablaBens.Columns.Add("CantidadPrendas", typeof(long));
                                tablaBens.Columns.Add("itemDescription", typeof(string));
                                tablaBens.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaBens.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaBens;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                //txtzUPC.Text = "";
                            }
                        }
                        else
                        {

                        }
                        break;

                    case "WRANGLER":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaTarget = new DataTable();
                                tablaTarget.Columns.Add("Talla", typeof(string));
                                tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                                tablaTarget.Columns.Add("idSize", typeof(int));
                                tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                                tablaTarget.Columns.Add("itemDescription", typeof(string));
                                tablaTarget.Columns.Add("color", typeof(string));
                                tablaTarget.Columns.Add("CantidadPrendasTotal", typeof(string));
                                altaZ = altaZ + 1;
                                tablaTarget.Rows.Add(cmbzTallas.Text
                                                    , txtzUPC.Text
                                                    , cmbzTallas.SelectedValue.ToString()
                                                    , txtzCantidadPrendas.Text
                                                    , txtzItemDescripcion.Text
                                                    , txtzColor.Text
                                                    , txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaTarget.Rows.Add(cmbzTallas.Text
                                                    , txtzUPC.Text
                                                    , cmbzTallas.SelectedValue.ToString()
                                                    , txtzCantidadPrendas.Text
                                                    , txtzItemDescripcion.Text
                                                    , txtzColor.Text
                                                    , txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                        }
                        else
                        {

                        }
                        break;

                    case "TYNDALE":
                        if ((txtzCantidadPrendas.Text != "" && txtzCantidadCajas.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaIFR = new DataTable();
                                tablaIFR.Columns.Add("Talla", typeof(string));
                                tablaIFR.Columns.Add("Codigo UPC", typeof(string));
                                tablaIFR.Columns.Add("idSize", typeof(int));
                                tablaIFR.Columns.Add("CantidadPrendas", typeof(long));
                                tablaIFR.Columns.Add("itemDescription", typeof(string));
                                tablaIFR.Columns.Add("color", typeof(string));
                                tablaIFR.Columns.Add("CantidadPrendasTotal", typeof(string));
                                altaZ = altaZ + 1;
                                tablaIFR.Rows.Add(cmbzTallas.Text
                                                  , txtzUPC.Text
                                                  , cmbzTallas.SelectedValue.ToString()
                                                  , txtzCantidadPrendas.Text
                                                  , txtzItemDescripcion.Text
                                                  , cmbColor.Text
                                                  , txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaIFR;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                            }
                            else
                            {
                                tablaIFR.Rows.Add(cmbzTallas.Text
                                                  , txtzUPC.Text
                                                  , cmbzTallas.SelectedValue.ToString()
                                                  , txtzCantidadPrendas.Text
                                                  , txtzItemDescripcion.Text
                                                  , cmbColor.Text
                                                  , txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaIFR;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                            }
                        }
                        else
                        {

                        }
                        break;

                    case "IFR":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaIFR = new DataTable();
                                tablaIFR.Columns.Add("Talla", typeof(string));
                                tablaIFR.Columns.Add("Codigo UPC", typeof(string));
                                tablaIFR.Columns.Add("idSize", typeof(int));
                                tablaIFR.Columns.Add("CantidadPrendas", typeof(long));
                                tablaIFR.Columns.Add("itemDescription", typeof(string));
                                tablaIFR.Columns.Add("color", typeof(string));
                                tablaIFR.Columns.Add("CantidadPrendasTotal", typeof(string));
                                altaZ = altaZ + 1;
                                tablaIFR.Rows.Add(cmbzTallas.Text
                                                  , txtzUPC.Text
                                                  , cmbzTallas.SelectedValue.ToString()
                                                  , txtzCantidadPrendas.Text
                                                  , txtzItemDescripcion.Text
                                                  , txtzColor.Text
                                                  , txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaIFR;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                            }
                            else
                            {
                                tablaIFR.Rows.Add(cmbzTallas.Text
                                                  , txtzUPC.Text
                                                  , cmbzTallas.SelectedValue.ToString()
                                                  , txtzCantidadPrendas.Text
                                                  , txtzItemDescripcion.Text
                                                  , txtzColor.Text
                                                  , txtzCantidadCajas.Text);

                                dgvZumies.DataSource = tablaIFR;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                            }
                        }
                        else
                        {

                        }
                        break;
                    case "LEVIS":
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaTarget = new DataTable();
                                tablaTarget.Columns.Add("Talla", typeof(string));
                                tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                                tablaTarget.Columns.Add("idSize", typeof(int));
                                tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                                tablaTarget.Columns.Add("itemDescription", typeof(string));
                                tablaTarget.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                        }
                        else
                        {

                        }
                        break;
                    default:
                        if ((txtzCantidadPrendas.Text != "" && txtzUPC.Text != ""))
                        {
                            if (altaZ == 0)
                            {
                                tablaTarget = new DataTable();
                                tablaTarget.Columns.Add("Talla", typeof(string));
                                tablaTarget.Columns.Add("Codigo UPC", typeof(string));
                                tablaTarget.Columns.Add("idSize", typeof(int));
                                tablaTarget.Columns.Add("CantidadPrendas", typeof(long));
                                tablaTarget.Columns.Add("itemDescription", typeof(string));
                                tablaTarget.Columns.Add("color", typeof(string));
                                altaZ = altaZ + 1;
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                            else
                            {
                                tablaTarget.Rows.Add(cmbzTallas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                                dgvZumies.DataSource = tablaTarget;
                                dgvZumies.Columns["idSize"].Visible = false;
                                cmbzTallas.SelectedIndex = 0;
                                txtzUPC.Text = "";

                            }
                        }
                        else
                        {

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void btnzActualiza_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvZumies.RowCount > 0)
                {
                    DataGridViewRow newDatarow = dgvZumies.Rows[RengloSelecionado];
                    switch (cmbMarca.SelectedItem)
                    {
                        case "TARGET":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;

                            break;
                        case "ZUMIES":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzCantidadCajas.Text;
                            newDatarow.Cells[2].Value = txtzUPC.Text;
                            newDatarow.Cells[3].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[4].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[5].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[6].Value = txtzColor.Text;
                            break;

                        case "ROCKY":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzCantidadCajas.Text;
                            newDatarow.Cells[2].Value = txtzUPC.Text;
                            newDatarow.Cells[3].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[4].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[5].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[6].Value = txtzColor.Text;
                            break;
                        case "OLDNAVY":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzCantidadCajas.Text;
                            newDatarow.Cells[2].Value = txtzUPC.Text;
                            newDatarow.Cells[3].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[4].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[5].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[6].Value = txtzColor.Text;
                            break;


                        case "BENS":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;


                        case "SAFTECH":

                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;


                        case "LEVIS":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            break;

                        case "WRANGLER":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            newDatarow.Cells[6].Value = txtzCantidadCajas.Text;
                            break;


                        case "IFR":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = txtzColor.Text;
                            newDatarow.Cells[6].Value = txtzCantidadCajas.Text;


                            /* REVISAR!!!!**/
                            break;
                        case "TYNDALE":
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = cmbColor.Text;
                            newDatarow.Cells[6].Value = txtzCantidadCajas.Text;
                            break;
                        default:
                            newDatarow.Cells[0].Value = cmbzTallas.Text;
                            newDatarow.Cells[1].Value = txtzUPC.Text;
                            newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                            newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                            newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                            newDatarow.Cells[5].Value = cmbColor.Text;



                            //cmbzTallas.Text = row.Cells[0].Value.ToString();
                            //txtzUPC.Text = row.Cells[1].Value.ToString();
                            //txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                            //txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                            //txtzColor.Text = row.Cells[5].Value.ToString();
                            //txtzCantidadCajas.Text = row.Cells[1].Value.ToString();




                            break;
                    }
                    //RengloSelecionado = dgvPrePack2.CurrentCell.RowIndex;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtzUPC_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    AgregarTallas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnzQuitar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvZumies.RowCount > 0)
                {
                    RengloSelecionado = dgvZumies.CurrentCell.RowIndex;
                    dgvZumies.Rows.RemoveAt(RengloSelecionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void txtzCantidadPrendas_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    txtzUPC.Focus();
                }
                //Para obligar a que sólo se introduzcan números
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                    if (char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        //el resto de teclas pulsadas se desactivan
                        e.Handled = true;
                    }
                if (e.KeyChar == (int)Keys.Enter)
                {
                    txtzUPC.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtzCantidadCajas_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                //Para obligar a que sólo se introduzcan números
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = false;
                }
                else
                    if (char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        //el resto de teclas pulsadas se desactivan
                        e.Handled = true;
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtzItemDescripcion_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvZumies_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                RengloSelecionado = e.RowIndex;
                DataGridViewRow row = dgvZumies.Rows[RengloSelecionado];

                int target = 0;

                switch (cmbMarca.Text)
                {
                    case "TARGET":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;

                        break;
                    case "ZUMIES":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[2].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        txtzColor.Text = row.Cells[6].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[4].Value.ToString();
                        break;
                    case "ROCKY":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        break;

                    case "OLDNAVY":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[1].Value.ToString();
                        break;
                    case "BENS":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        //txtzCantidadCajas.Text = row.Cells[6].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;
                        break;
                    case "SAFTECH":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[6].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;
                        break;
                    case "WRANGLER":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[6].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;
                        break;

                    case "TYNDALE":
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[6].Value.ToString();
                        break;

                    default:
                        cmbzTallas.Text = row.Cells[0].Value.ToString();
                        txtzUPC.Text = row.Cells[1].Value.ToString();
                        txtzCantidadPrendas.Text = row.Cells[3].Value.ToString();
                        txtzItemDescripcion.Text = row.Cells[4].Value.ToString();
                        txtzColor.Text = row.Cells[5].Value.ToString();
                        txtzCantidadCajas.Text = row.Cells[6].Value.ToString();
                        //newDatarow.Cells[0].Value = cmbzTallas.Text;
                        //newDatarow.Cells[1].Value = txtzUPC.Text;
                        //newDatarow.Cells[2].Value = cmbzTallas.SelectedValue.ToString();
                        //newDatarow.Cells[3].Value = txtzCantidadPrendas.Text;
                        //newDatarow.Cells[4].Value = txtzItemDescripcion.Text;
                        //newDatarow.Cells[5].Value = txtzColor.Text;
                        break;
                }
                //if cmbMarca.Text="ZUMIES"

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void cmbzTallas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMarca.Text == "BENS" || cmbMarca.Text == "ROCKY" || cmbMarca.Text == "OLDNAVY" || cmbMarca.Text == "SAFTECH")
            {
                if (iniciando)
                { }
                else
                {
                    txtzUPC.Text = f.ConsultaUPCBENS(Convert.ToInt32(cmbzTallas.SelectedValue));
                    txtzCantidadPrendas.Focus();
                }
                // Convert.ToInt32(cmbCliente.SelectedValue)

            }
            else if (cmbMarca.Text == "IFR")
            {
                txtzUPC.Text = f.ConsultaUPCIFR(Convert.ToInt32(cmbzTallas.SelectedValue));
                txtzCantidadPrendas.Focus();

            }
            else if (cmbMarca.Text == "TYNDALE")
            {
                txtzUPC.Text = f.ConsultaUPCTYNDALE(Convert.ToInt32(cmbzTallas.SelectedValue));
                txtzCantidadPrendas.Focus();

            }
            else
            {
                txtzUPC.Focus();
            }


        }

        private void txtAltaPO_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Para obligar a que sólo se introduzcan números
            if (char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                if (char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                {
                    e.Handled = false;
                }
                else
                {
                    //el resto de teclas pulsadas se desactivan
                    e.Handled = true;
                }
        }

        private void txtzPOZumies_KeyPress(object sender, KeyPressEventArgs e)
        {


        }

        private void cmbMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                switch (cmbMarca.SelectedItem.ToString())
                {
                    case "LEVIS":
                        txtzColor.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzItemDescripcion.Text = "";
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        txtzUPC.Enabled = true;
                        break;
                    case "ZUMIES":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        iniciando = true;
                        f.ConsultaTallasZumies(cmbzTallas);
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        break;
                    case "ROCKY":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        /* f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                         cmbzTallas.SelectedIndex = 0;*/
                        iniciando = true;
                        f.ConsultaEstiloRocky(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        txtzUPC.Enabled = false;
                        break;
                    case "OLDNAVY":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        iniciando = true;
                        f.ConsultaEstiloOldNavy(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        txtzUPC.Enabled = false;
                        txtzCantidadPrendas.Text = "18";
                        break;
                    case "CINTAS":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "TARGET":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzUPC.Enabled = true;
                        txtzCantidadPrendas.Enabled = true;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzCantidadPrendas.Text = "12";
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        break;

                    case "BENS":
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzEstilo.Enabled = true;
                        txtzColor.Enabled = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        /* f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                         cmbzTallas.SelectedIndex = 0;*/
                        iniciando = true;
                        f.ConsultaEstiloBens(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        txtzUPC.Enabled = true;
                        break;
                    case "SAFTECH":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        /* f.ConsultaTallasXMarca(cmbzTallas, cmbMarca.SelectedItem.ToString());
                         cmbzTallas.SelectedIndex = 0;*/
                        iniciando = true;
                        f.ConsultaEstiloSaftech(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        break;
                    case "WRANGLER":
                        txtzColor.Enabled = false;
                        txtzItemDescripcion.Enabled = false;
                        txtzEstilo.Enabled = false;
                        txtzCantidadCajas.Visible = false;
                        lblCantidadCajas.Visible = false;
                        cmbEstilo.Visible = false;
                        cmbEstilo.Enabled = false;
                        f.ConsultaTallasXMarca(cmbzTallas, "WRANGLER");
                        cmbzTallas.SelectedIndex = 0;
                        txtzUPC.Enabled = true;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        lblCantidadCajas.Text = "Cantidad Total\nprendas x Talla:";
                        break;
                    case "IFR":
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        f.ConsultaEstiloIFR(cmbEstilo);
                        cmbEstilo.SelectedIndex = 0;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        cmbzTallas.Visible = true;
                        cmbzTallas.Enabled = true;
                        f.ConsultaTallasIFR(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        txtzColor.Enabled = true;
                        txtzColor.Visible = true;
                        txtzItemDescripcion.Enabled = true;
                        txtzItemDescripcion.Visible = true;
                        txtzCantidadPrendas.Enabled = true;
                        txtzUPC.Enabled = false;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        lblCantidadCajas.Text = "Cantidad Total\nprendas x Talla:";
                        //lblCantidadCajasModif
                        txtzEstilo.Enabled = false;
                        iniciando = true;
                        iniciando = false;
                        break;
                    case "TYNDALE":
                        f.ConsultaEstiloTYNDALE(cmbEstilo);
                        iniciando = true;
                        cmbEstilo.SelectedIndex = 0;
                        cmbEstilo.SelectedIndex = 1;
                        cmbEstilo.SelectedIndex = 0;
                        iniciando = false;
                        f.ConsultaColorTYNDALE(cmbColor, cmbEstilo.Text.ToString());
                        f.ConsultaColor(cmbEstilo.Text.ToString(), cmbMarca.SelectedItem.ToString());

                        iniciando = true;
                        cmbColor.SelectedIndex = 0;
                        cmbColor.SelectedIndex = 1;
                        cmbColor.SelectedIndex = 0;
                        iniciando = false;
                        f.ConsultaTallasTYNDALE(cmbzTallas, cmbEstilo.Text.ToString(), cmbColor.Text.ToString());
                        iniciando = true;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        f.ConsultaUPCTYNDALE(Convert.ToInt32(cmbzTallas.SelectedValue));
                        cmbEstilo.Visible = true;
                        cmbEstilo.Enabled = true;
                        cmbzTallas.Visible = true;
                        cmbzTallas.Enabled = true;
                        cmbColor.Visible = true;

                        txtzItemDescripcion.Enabled = true;
                        txtzItemDescripcion.Visible = true;
                        txtzCantidadPrendas.Enabled = true;
                        txtzUPC.Enabled = false;
                        txtzCantidadCajas.Visible = true;
                        lblCantidadCajas.Visible = true;
                        lblCantidadCajas.Text = "Cantidad Total\nprendas x Talla:";
                        //lblCantidadCajasModif
                        txtzEstilo.Enabled = false;
                        iniciando = false;
                        break;
                }
            }
        }

        private void btnzCancelar_Click(object sender, EventArgs e)
        {
            LimpiarCamposZumies();
        }


        private void cmbEstilo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (iniciando)
            {

            }
            else
            {
                /*
                 *  LEVIS
                    ZUMIES
                    TARGET
                    CINTAS
                    BENS
                    ROCKY
                    OLDNAVY
                 */
                ConsultaTallasColorResult x = new ConsultaTallasColorResult();
                ConsultaTallasColorIFRResult ColorIFR = new ConsultaTallasColorIFRResult();
                switch (cmbMarca.SelectedItem.ToString())
                {
                    case "LEVIS":
                        iniciando = true;
                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        if (cmbzTallas.Items.Count > 1) cmbzTallas.SelectedIndex = 1;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        break;
                    case "ZUMIES":
                        iniciando = true;
                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        if (cmbzTallas.Items.Count > 1) cmbzTallas.SelectedIndex = 1;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        break;
                    case "TARGET":
                        iniciando = true;
                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        if (cmbzTallas.Items.Count > 1) cmbzTallas.SelectedIndex = 1;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        break;
                    case "BENS":
                        iniciando = true;

                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        if (cmbzTallas.Items.Count > 1) cmbzTallas.SelectedIndex = 1;
                        if (cmbzTallas.Items.Count > 0) cmbzTallas.SelectedIndex = 0;
                        break;
                    case "SAFTECH":
                        iniciando = true;
                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasBENS(cmbzTallas, cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "ROCKY":
                        iniciando = true;
                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        x = f.ConsultaTallasColor(cmbEstilo.Text.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasxMarcaxEstilo(cmbzTallas, "ROCKY", cmbEstilo.Text.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;
                    case "OLDNAVY":
                        iniciando = true;

                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        x = f.ConsultaTallasColor(cmbEstilo.SelectedValue.ToString());
                        txtzColor.Text = x.color;
                        txtzItemDescripcion.Text = x.descripcion;
                        f.ConsultaTallasxMarcaxEstilo(cmbzTallas, "OLDNAVY", cmbEstilo.SelectedValue.ToString());
                        cmbzTallas.SelectedIndex = 0;
                        iniciando = false;
                        cmbzTallas.SelectedIndex = 0;
                        cmbzTallas.SelectedIndex = 1;
                        cmbzTallas.SelectedIndex = 0;
                        break;

                    // C#
                    case "IFR":
                        iniciando = true;
                        cmbColor.Visible = false;
                        txtzColor.Visible = true;
                        ColorIFR = f.ConsultaTallasColorIFR(cmbEstilo.Text?.ToString());

                        if (ColorIFR == null)
                        {
                            iniciando = false;
                            // Valores por defecto / mensaje claro en la UI
                            txtzColor.Text = "";
                            txtzItemDescripcion.Text = "No se encontró información IFR para este estilo";
                            txtzCantidadPrendas.Text = "";
                            // Registro para depuración
                            System.Diagnostics.Debug.WriteLine($"ConsultaTallasColorIFR devolvió null para estilo='{cmbEstilo.Text}'");
                            break;
                        }
                        else
                        {
                            // Asignar valores a los controles
                            txtzColor.Text = ColorIFR.color;
                            txtzItemDescripcion.Text = ColorIFR.descripcion + "," + ColorIFR.descripcion2;
                            txtzCantidadPrendas.Text = ColorIFR.cantidadPrendas;
                        }

                        f.ConsultaTallasIFR(cmbzTallas, cmbEstilo.Text.ToString());
                        iniciando = false;
                        // (resto de la lógica si procede)
                        break;

                    case "TYNDALE":
                        // C#
                        cmbColor.Visible = true;
                        txtzColor.Visible = false;
                        //ConsultaTallasColorTYNDALEResult ColorTYNDALE = f.ConsultaTallasColorTYNDALE(cmbEstilo.Text?.ToString());



                        //f.ConsultaColor(cmbEstilo.Text.ToString(), cmbMarca.SelectedItem.ToString());
                        f.ConsultaColorTYNDALE(cmbColor, cmbEstilo.Text.ToString());
                        cmbColor.SelectedIndex = 0;
                        //f.ConsultaTallasTYNDALE(cmbzTallas, cmbEstilo.Text.ToString(), cmbColor.Text.ToString());
                        //iniciando = true;
                        //cmbzTallas.SelectedIndex = 0;
                        //cmbzTallas.SelectedIndex = 1;
                        //cmbzTallas.SelectedIndex = 0;
                        //iniciando = false;
                        //f.ConsultaUPCTYNDALE(Convert.ToInt32(cmbzTallas.SelectedValue));
                        //cmbEstilo.Visible = true;
                        //cmbEstilo.Enabled = true;


                        /*
                        if (ColorTYNDALE == null)
                        {
                            iniciando = false;
                            // Registrar o mostrar mensaje de error y salir del case
                            txtzColor.Text = "";
                            txtzItemDescripcion.Text = "No se encontró información para este estilo (TYNDALE)";
                            return; // o break según lógica de la función
                        }
                        else
                        {
                            // Asignar valores a los controles
                            cmbColor.Text = ColorTYNDALE.color;
                            txtzItemDescripcion.Text = ColorTYNDALE.descripcion + "," + ColorTYNDALE.descripcion2;
                            txtzCantidadPrendas.Text = ColorTYNDALE.cantidadPrendas;
                        }
                        */
                        // f.ConsultaTallasTYNDALE(cmbzTallas, cmbEstilo.Text.ToString(), cmbColor.Text.ToString());


                        break;
                }

            }

        }

        private void btnTodaslasTallas_Click(object sender, EventArgs e)
        {
            if (altaZ == 0)
            {
                tablaZumies = new DataTable();
                tablaZumies.Columns.Add("Talla", typeof(string));
                tablaZumies.Columns.Add("CantidadCajas", typeof(long));
                tablaZumies.Columns.Add("Codigo UPC", typeof(string));
                tablaZumies.Columns.Add("idSize", typeof(int));
                tablaZumies.Columns.Add("CantidadPrendas", typeof(long));
                tablaZumies.Columns.Add("itemDescription", typeof(string));
                tablaZumies.Columns.Add("color", typeof(string));
                altaZ = altaZ + 1;

                //foreach de todas las tallas

                int tallasCombo = cmbzTallas.Items.Count;
                for (int i = 0; i == tallasCombo; i++)
                {
                    cmbzTallas.SelectedIndex = i;
                    txtzUPC.Text = f.ConsultaUPCBENS(Convert.ToInt32(cmbzTallas.SelectedValue));
                    tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                }

                dgvZumies.DataSource = tablaZumies;
                //dgvZumies.Columns["idSize"].Visible = false;
                cmbzTallas.SelectedIndex = 0;
                txtzUPC.Text = "";

            }
            else
            {
                tablaZumies.Rows.Add(cmbzTallas.Text, txtzCantidadCajas.Text, txtzUPC.Text, cmbzTallas.SelectedValue.ToString(), txtzCantidadPrendas.Text, txtzItemDescripcion.Text, txtzColor.Text);

                dgvZumies.DataSource = tablaZumies;
                //dgvZumies.Columns["idSize"].Visible = false;
                cmbzTallas.SelectedIndex = 0;
                txtzUPC.Text = "";
            }
        }

        private void cmbMarca_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cmbColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!iniciando)
            {
                iniciando = true;
                f.ConsultaTallasTYNDALE(cmbzTallas, cmbEstilo.Text.ToString(), cmbColor.Text.ToString());
                iniciando = false;

            }
            if (cmbzTallas.Items.Count > 0)
                cmbzTallas.SelectedIndex = 0;
        }
    }
}
