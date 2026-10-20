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
    // Dividir una caja en dos
    public partial class Escaneo
    {
        private void txtCajaIDDividir_KeyPress(object sender, KeyPressEventArgs e)
        {

            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    ConsultaDivision();
                    txtPiezas.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ConsultaDivision()
        {
            try
            {
                if (txtCajaIDDividir.Text != string.Empty)
                {
                    txtRestante.Text = string.Empty; txtPiezas.Text = string.Empty; CantidadDividir = 0;
                    int idCaja = Convert.ToInt32(txtCajaIDDividir.Text);
                    ubicacion_Dividir_ConsultaCajaIDResult r = f.ConsultaCajaID(idCaja);
                    if (r.cantidad > 0)
                    {
                        txtRestante.Text = r.cantidad.ToString();
                        txtPiezas.Text = string.Empty;
                        CantidadDividir = r.cantidad;
                    }
                    else { txtRestante.Text = string.Empty; txtPiezas.Text = string.Empty; CantidadDividir = 0; }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnGuardarDivision_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRestante.Text != "0" && txtRestante.Text != "")
                {
                    DialogResult dialogResult = MessageBox.Show("Seguro que deseas Dividir esta caja " + txtCajaIDDividir.Text, "Cerrar", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        if (txtPiezas.Text != string.Empty && txtRestante.Text != string.Empty && txtCajaIDDividir.Text != string.Empty)
                        {
                            int idCaja = Convert.ToInt32(txtCajaIDDividir.Text);
                            int piezas = Convert.ToInt32(txtPiezas.Text);
                            int restante = Convert.ToInt32(txtRestante.Text);
                            ubicacion_Dividir_CajaIDResult r = f.DividirCaja(idCaja, usu[0].id, piezas, restante);
                            if (r.Caja1 != null && r.Caja2 != null)
                            {
                                ImprimirEtiquetasDivididas(r.Caja1, r.Caja2);
                                txtCajaIDDividir.Text = "";
                                txtPiezas.Text = "";
                                txtRestante.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                            }
                        }
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                    }
                }
                else
                {
                    MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ImprimirEtiquetasDivididas(int? caja1, int? caja2)
        {
            try
            {
                txtUPCScann.Text = string.Empty;
                txtUPCScann.Focus();
                int cajaid1 = Convert.ToInt32(caja1);
                int cajaid2 = Convert.ToInt32(caja2);
                /* List<ConsultaInventarioIDResult> consulta = f.ConsultaInventarioID(cajaid1);
                 if (consulta.Count > 0)
                 {
                     QRCodeGenerator qrGenerator = new QRCodeGenerator();

                     List<EtiquetaCajaModificada> listClase = new List<EtiquetaCajaModificada>();
                     EtiquetaCajaModificada clase = new EtiquetaCajaModificada();

                     clase.poInCompleto = consulta[0].poInCompleto;
                     clase.po = consulta[0].po;
                     clase.poItem = consulta[0].poItem;
                     clase.ProductCode = consulta[0].ProductCode;
                     clase.Size = consulta[0].Size;
                     clase.size_derecho = consulta[0].size_derecho;
                     clase.size_izquierdo = consulta[0].size_izquierdo;
                     clase.TipoCarton = consulta[0].TipoCarton;
                     clase.upc = consulta[0].upc;
                     clase.Fecha = consulta[0].create_dtm;
                     clase.CartonLeft = consulta[0].CartonLeft;
                     clase.CartonRight = consulta[0].CartonRight;
                     clase.Cantidad = consulta[0].Cantidad;
                     clase.Carton = consulta[0].Carton;
                     clase.usuario = consulta[0].usuario;
                     clase.id_inventario = consulta[0].id;
                     clase.id_cliente = Convert.ToInt32(consulta[0].id_cliente);
                     clase.id_factura = Convert.ToInt32(consulta[0].id_factura);
                     clase.id_terminado = Convert.ToInt32(consulta[0].id_terminado);
                     clase.cliente = consulta[0].cliente == string.Empty ? "NA" : consulta[0].cliente;
                     clase.factura = consulta[0].factura == string.Empty ? "NA" : consulta[0].factura;
                     clase.terminado = consulta[0].terminado == string.Empty ? "NA" : consulta[0].terminado;
                     QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase.id_inventario +
                                                                       "&po=" + clase.po +
                                                                       "&cl=" + clase.cliente +
                                                                       "&fa=" + clase.factura +
                                                                       "&te=" + clase.terminado +
                                                                       "&u=" + clase.usuario +
                                                                       "&pc=" + clase.ProductCode +
                                                                       "&c=" + clase.Cantidad +
                                                                       "&sz=" + clase.Size +
                                                                       "&fe=" + clase.Fecha, QRCodeGenerator.ECCLevel.Q);
                     QRCode qrCode = new QRCode(qrCodeData);
                     BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode();
                     Codigo.IncludeLabel = true;
                     Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                        , consulta[0].id.ToString()
                                                        , Color.Black
                                                        , Color.White, 200, 100);

                     clase.qr = qrCode.GetGraphic(20);
                     clase.codigoBarras = codigoBarras;


                     listClase.Add(clase);
                     ReporteCaja report = new ReporteCaja();
                     report.DataSource = listClase;
                     // Disable margins warning. 
                     report.PrintingSystem.ShowMarginsWarning = false;
                     ReportPrintTool tool = new ReportPrintTool(report);
                     tool.ShowPreview();
                     //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                     //tool.PrintDialog(); //muestra a que impresora se va a mandar
                     tool.Print(); //imprime de golpe
                 }
                 */
                List<ConsultaInventarioIDResult> consulta2 = f.ConsultaInventarioID(cajaid2);
                if (consulta2.Count > 0)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();

                    List<EtiquetaCajaModificada> listclase2 = new List<EtiquetaCajaModificada>();
                    EtiquetaCajaModificada clase2 = new EtiquetaCajaModificada
                    {
                        poInCompleto = Convert.ToDecimal(consulta2[0].poInCompleto),
                        po = consulta2[0].po,
                        poItem = consulta2[0].poItem,
                        ProductCode = consulta2[0].ProductCode,
                        Size = consulta2[0].Size,
                        size_derecho = consulta2[0].size_derecho,
                        size_izquierdo = consulta2[0].size_izquierdo,
                        TipoCarton = consulta2[0].TipoCarton,
                        upc = consulta2[0].upc,
                        Fecha = consulta2[0].create_dtm,
                        CartonLeft = consulta2[0].CartonLeft,
                        CartonRight = consulta2[0].CartonRight,
                        Cantidad = consulta2[0].Cantidad,
                        Carton = consulta2[0].Carton,
                        usuario = consulta2[0].usuario,
                        id_inventario = consulta2[0].id,
                        id_cliente = Convert.ToInt32(consulta2[0].id_cliente),
                        id_factura = Convert.ToInt32(consulta2[0].id_factura),
                        id_terminado = Convert.ToInt32(consulta2[0].id_terminado),
                        cliente = consulta2[0].cliente == string.Empty ? "NA" : consulta2[0].cliente,
                        factura = consulta2[0].factura == string.Empty ? "NA" : consulta2[0].factura,
                        terminado = consulta2[0].terminado == string.Empty ? "NA" : consulta2[0].terminado
                    };
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode("?id=" + clase2.id_inventario +
                                                                      "&po=" + clase2.po +
                                                                      "&cl=" + clase2.cliente +
                                                                      "&fa=" + clase2.factura +
                                                                      "&te=" + clase2.terminado +
                                                                      "&u=" + clase2.usuario +
                                                                      "&pc=" + clase2.ProductCode +
                                                                      "&c=" + clase2.Cantidad +
                                                                      "&sz=" + clase2.Size +
                                                                      "&fe=" + clase2.Fecha, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
                    {
                        IncludeLabel = true
                    };
                    Image codigoBarras2 = Codigo.Encode(BarcodeLib.TYPE.CODE39
                                                       , consulta2[0].id.ToString()
                                                       , Color.Black
                                                       , Color.White, 200, 100);

                    clase2.qr = qrCode.GetGraphic(20);
                    clase2.codigoBarras = codigoBarras2;


                    listclase2.Add(clase2);
                    ReporteCaja report2 = new ReporteCaja
                    {
                        DataSource = listclase2
                    };
                    // Disable margins warning. 
                    report2.PrintingSystem.ShowMarginsWarning = false;
                    ReportPrintTool tool2 = new ReportPrintTool(report2);
                    tool2.ShowPreview();
                    //tool.ShowRibbonPreviewDialog(); // muestra el disenio 
                    //tool.PrintDialog(); //muestra a que impresora se va a mandar
                    tool2.Print(); //imprime de golpe
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCajaIDDividir_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ConsultaDivision();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPiezas_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtPiezas.Text != string.Empty)
                {
                    int piezas = Convert.ToInt32(txtPiezas.Text);
                    if (piezas < CantidadDividir && piezas > -1)
                    {
                        txtRestante.Text = (CantidadDividir - piezas).ToString();
                    }
                    else if (piezas < 0)
                    {
                        txtPiezas.Text = string.Empty;
                        MessageBox.Show("Favor de ingresar una caja correcta");
                    }
                    else
                    {
                        MessageBox.Show("Favor de ingresar un numero de piezas menor de " + CantidadDividir);
                        txtRestante.Text = CantidadDividir.ToString();
                        txtPiezas.Text = string.Empty;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPiezas_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (int)Keys.Enter)
                {
                    if (txtRestante.Text != "0" && txtRestante.Text != "")
                    {
                        DialogResult dialogResult = MessageBox.Show("Seguro que deseas Dividir esta caja " + txtCajaIDDividir.Text, "Cerrar", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (txtPiezas.Text != string.Empty && txtRestante.Text != string.Empty && txtCajaIDDividir.Text != string.Empty)
                            {
                                int idCaja = Convert.ToInt32(txtCajaIDDividir.Text);
                                int piezas = Convert.ToInt32(txtPiezas.Text);
                                int restante = Convert.ToInt32(txtRestante.Text);
                                ubicacion_Dividir_CajaIDResult r = f.DividirCaja(idCaja, usu[0].id, piezas, restante);
                                if (r.Caja1 != null && r.Caja2 != null)
                                {
                                    ImprimirEtiquetasDivididas(r.Caja1, r.Caja2);
                                    txtCajaIDDividir.Text = "";
                                    txtPiezas.Text = "";
                                    txtRestante.Text = "";
                                }
                                else
                                {
                                    MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                                }
                            }
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                        }
                    }
                    else
                    {
                        MessageBox.Show("Esta Caja no se a dado entrada en Almacen");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void xtpDividirCaja_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
