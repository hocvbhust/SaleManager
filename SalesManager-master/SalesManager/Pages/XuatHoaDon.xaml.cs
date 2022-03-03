using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using Microsoft.Win32;
namespace SalesManager
{
    /// <summary>
    /// Interaction logic for XuatHoaDon.xaml
    /// </summary>
    public partial class XuatHoaDon : BasePage
    {
        public static string MaHoaDon;
        string valueKhachDua;
        long sum = 0;
        public XuatHoaDon()
        {
            InitializeComponent();
            SoHD.Text = MaHoaDon;
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();
            string MaNV = "";
            var cmd = new MySqlCommand("SELECT NGHOADON, MANV FROM HOADON WHERE MAHD = '" + MaHoaDon + "'", con);
            var dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                NgHD.Text = dr.GetValue(0).ToString();
                MaNV = dr.GetString(1);
            }
            dr.Close();
            cmd.CommandText = "SELECT HOTEN FROM NHANVIEN WHERE MANV = '" + MaNV + "'";
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                TenNV.Text = dr.GetString(0);
            }
            dr.Close();
            cmd.CommandText = "SELECT MAHANG, SOLUONG, MALO FROM CTHD WHERE MAHD = '" + MaHoaDon + "'";
            dr = cmd.ExecuteReader();
            List<string> MaHang = new List<string>();
            List<int> SoLuong = new List<int>();
            List<long> TongTien = new List<long>();
            List<int> MaLo = new List<int>();
            while (dr.Read())
            {
                MaHang.Add(dr.GetString(0));
                SoLuong.Add(dr.GetInt32(1));
                MaLo.Add(dr.GetInt32(2));
            }
            dr.Close();
            for (int i = 0; i < MaHang.Count; i++)
            {
                string _TenHang = "";
                int _DonGia = 0;
                cmd.CommandText = "SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + MaHang[i] + "'";
                dr = cmd.ExecuteReader();
                if (dr.Read()) _TenHang = dr.GetString(0);
                dr.Close();
                cmd.CommandText = "SELECT DONGIA FROM NHAPHANG WHERE MAHANG = '" + MaHang[i] + "' AND MALO = '" + Convert.ToString(MaLo[i]) + "'";
                dr = cmd.ExecuteReader();
                if (dr.Read()) _DonGia = Convert.ToInt32(Convert.ToInt32(dr.GetValue(0))*1.05);
                TongTien.Add(SoLuong[i] * _DonGia);
                HangMua.Items.Add(new TaoHoaDon.MatHang() { TenHang = _TenHang, DonGia = _DonGia, STT = HangMua.Items.Count + 1, SoLuong = SoLuong[i], ThanhTien = SoLuong[i] * _DonGia });
                dr.Close();
            }
            for (int i = 0; i < TongTien.Count; i++) sum += TongTien[i];
            ThanhTien.Text = string.Format("{0:#,##0}" + " VND", double.Parse(sum.ToString()));
        }

        private void KhachDua_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void KhachDua_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (KhachDua.Text.Contains("VND") == false)
            {
                valueKhachDua = KhachDua.Text;
                if (KhachDua.Text == "") ThoiLai.Text = string.Format("{0:#,##0}" + " VND", double.Parse(Convert.ToString(0 - sum)));
                else ThoiLai.Text = string.Format("{0:#,##0}" + " VND", double.Parse(Convert.ToString(Convert.ToInt32(valueKhachDua) - sum)));
            }
        }

        private void TroVe_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.TaoHoaDon;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (KhachDua.Text == "" || ThoiLai.Text.Contains("-") == true )
                MessageBox.Show("Vui lòng kiểm tra lại giá trị tiền khách trả", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                InHoaDon_Button.Visibility = Visibility.Hidden;
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(this);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = MaHoaDon + ".png";
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filename = saveFileDialog.FileName;
                    FileStream fs = new FileStream(filename, FileMode.Create);
                    encoder.Save(fs);
                    fs.Close();
                }
                MessageBox.Show("In hóa đơn thành công");
                ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.TaoHoaDon;
            }
        }

        private void KhachDua_GotFocus(object sender, RoutedEventArgs e)
        {
            KhachDua.MaxLength = 8;
            KhachDua.Text = valueKhachDua;
        }

        private void KhachDua_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                KhachDua.MaxLength = 14;
                valueKhachDua = KhachDua.Text;
                KhachDua.Text = string.Format("{0:#,##0}" + " VND", double.Parse(KhachDua.Text));
            }
            catch
            {
                KhachDua.MaxLength = 9;
                valueKhachDua = "";
                KhachDua.Text = "";
            }
        }
    }
}