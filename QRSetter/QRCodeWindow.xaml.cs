using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace QRSetter
{
    /// <summary>
    /// QRCodeWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class QRCodeWindow : Window
    {
        public QRCodeWindow(string content)
        {
            InitializeComponent();
            GenerateQrCode(content);
        }
        private void GenerateQrCode(string content)
        {
            var qrCode = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    ErrorCorrection = ErrorCorrectionLevel.M,
                    CharacterSet = "UTF-8",
                    Width = 500,
                    Height = 500,
                    Margin = 5,
                },
            };

            using (var bmp = qrCode.Write(content))
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                var source = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                QrImage.Source = source;
            }
        }
    }
}
