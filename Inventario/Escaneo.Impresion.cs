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
    // Utilerias de impresion: impresoras, imagenes y codigos UPC
    public partial class Escaneo
    {
        private Bitmap RotarImagen90(Bitmap original)
        {
            Bitmap rotada = new Bitmap(original);
            rotada.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return rotada;
        }
        private void CargarImpresorasDisponibles()
        {
            cmbImpresora.Items.Clear();
            foreach (string impresora in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbImpresora.Items.Add(impresora);
            }

            // selecciona la predeterminada de Windows al abrir la pantalla
            string predeterminada = new System.Drawing.Printing.PrinterSettings().PrinterName;
            if (cmbImpresora.Items.Contains(predeterminada))
                cmbImpresora.SelectedItem = predeterminada;
        }

        public static UPCResult GeneraCodigoUPC(string upc)
        {
            // Limpia caracteres no numéricos
            string limpio = upc;
            //Regex.Replace(upc ?? "", @"\D", "");

            if (string.IsNullOrEmpty(limpio))
                throw new ArgumentException("UPC vacío");

            // Si vienen 11 dígitos, calculamos el dígito de control
            if (limpio.Length == 11)
            {
                int checkDigit = CalculaCheckDigit(limpio);
                limpio += checkDigit.ToString();
            }

            // Si vienen 12 dígitos, validamos el dígito de control
            if (limpio.Length == 12 && !RevisaUPCA(limpio))
                throw new ArgumentException("UPC inválido: dígito de control incorrecto");

            // BarcodeLib espera 11 dígitos
            string onceDigitos = limpio.Substring(0, 11);

            BarcodeLib.Barcode Codigo = new BarcodeLib.Barcode
            {
                IncludeLabel = true,
                LabelFont = new Font("Arial", 12, FontStyle.Bold),
                Alignment = BarcodeLib.AlignmentPositions.CENTER,
                LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER
            };

            // Ajusta el ancho y alto
            Codigo.BarWidth = 3;
            Codigo.Height = 120;
            Codigo.Width = 250;



            Image codigoBarras = Codigo.Encode(BarcodeLib.TYPE.UPCA, onceDigitos);

            return new UPCResult
            {
                UPCCompleto = limpio,
                CodigoBarras = codigoBarras
            };
        }

        // Calcula el dígito de control UPC-A
        public static int CalculaCheckDigit(string upc11)
        {
            int sumOdd = 0, sumEven = 0;

            for (int i = 0; i < 11; i++)
            {
                int digit = upc11[i] - '0';
                if ((i % 2) == 0) sumOdd += digit;
                else sumEven += digit;
            }

            int total = (sumOdd * 3) + sumEven;
            return (10 - (total % 10)) % 10;
        }


        public static bool RevisaUPCA(string upc)
        {
            if (upc.Length != 12 || !long.TryParse(upc, out _))
                return false;

            int sumOdd = 0;
            int sumEven = 0;

            // Recorremos los primeros 11 dígitos
            for (int i = 0; i < 11; i++)
            {
                int digit = upc[i] - '0';
                if ((i % 2) == 0) // posiciones impares (0,2,4...) en índice
                    sumOdd += digit;
                else
                    sumEven += digit;
            }

            int total = (sumOdd * 3) + sumEven;
            int checkDigit = (10 - (total % 10)) % 10;

            // Comparar con el último dígito
            return checkDigit == (upc[11] - '0');
        }
    }
}
