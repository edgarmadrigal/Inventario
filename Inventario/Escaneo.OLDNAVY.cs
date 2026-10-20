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
    // Marca OLD NAVY (POItem 44)
    public partial class Escaneo
    {
        private void IncompleteCarton_OLDNAVY()
        {
            // btnImprimeOldNavy.Visible = false;
        
        }

        private void SeleccionTalla_OLDNAVY(ref string[] separadas)
        {
            lblImprimirEtiqueta.Visible = false;
            chkImprimirEtiqueta.Visible = false;
            try
            {
                separadas = cmbSizes.Text.Split('x');
                List<ConsultaProductos_VarcharResult> x =
                    f.ConsultaProductos(cmbPO.Text, cmbPOItem.Text, cmbProductCode.Text, separadas[0].ToString(), separadas[1].ToString());
                if (x.Count > 0)
                {
                    contador = 0;
                    dgvEscan.DataSource = x;
                    txtCartonsPacked.Text = x[0].CartonNumber.ToString();
                    //txtCartonNumber.Text = x[0].CartonNumber.ToString();
                    txtCartonSize.Text = x[0].Size.ToString();
                    dgvEscan.Columns["Cantidad"].Visible = false;
                    dgvEscan.Columns["ProductCode1"].Visible = false;
                    dgvEscan.Columns["Size"].Visible = false;
                    dgvEscan.Columns["UPC"].Visible = false;
                    dgvEscan.Columns["vendor"].Visible = false;
                    dgvEscan.Columns["CartonType"].Visible = false;
                    dgvEscan.Columns["CartonNumber"].Visible = false;
                    dgvEscan.Columns["id"].Visible = false;
                    txtSize.Text = x[0].Size.ToString();
                    txtProductCode.Text = x[0].ProductCode.ToString();
                    id = x[0].id;
                    anterior.Add(id);
                    upc = x[0].UPC.ToString();
                    ///po = Convert.ToInt32(cmbPO.Text);
                    cantidad = x[0].cantidad;
                    txtUnitsReq.Text = cantidad.ToString();
                    txtUnitsScan.Text = "0";
                    txtUnitsRemai.Text = cantidad.ToString();
                    txtCartonRq.Text = "1";
                    txtCartonsReamaining.Text = "1";
                    txtUPCScann.Focus();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        
        }

        private void Escaneo_OLDNAVY()
        {
            #region OLDNAVY  44
            try
            {
                if (PrendasExtra == 0 && Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text))
                {
                    MessageBox.Show("Ya se escanearon el total de las prendas favor de guardar!");
                }
                else if (PrendasExtra == 0 && Convert.ToInt64(txtUnitsScan.Text) != Convert.ToInt64(txtUnitsReq.Text))
                {
                    GuardarEnLista();

                    contador = contador + 1;
                    dgvEscan.Rows[(Convert.ToInt32(txtUnitsScan.Text))].Selected = true;
                    dgvEscan.FirstDisplayedScrollingRowIndex = (Convert.ToInt32(txtUnitsScan.Text));
                    txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                    txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    btnImprimeOldNavy.Visible = true;
                }
                else if (PrendasExtra == 1 && Convert.ToInt64(txtUnitsScan.Text) == Convert.ToInt64(txtUnitsReq.Text))
                {
                    GuardarEnLista();

                    contador = contador + 1;
                    txtUnitsScan.Text = (Convert.ToInt64(txtUnitsScan.Text) + 1).ToString();
                    txtUnitsRemai.Text = (Convert.ToInt64(txtUnitsRemai.Text) - 1).ToString();
                    txtUPCScann.Text = string.Empty;
                    txtUPCScann.Focus();
                    btnImprimeOldNavy.Visible = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            #endregion
        
        }

        private void ReimprimirUltima_OLDNAVY()
        {
            //btnImprimeOldNavy.Visible = false;
        
        }
    }
}
