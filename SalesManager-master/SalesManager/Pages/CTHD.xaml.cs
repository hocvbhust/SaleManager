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
namespace SalesManager
{
    /// <summary>
    /// Interaction logic for TaoHoaDon.xaml
    /// </summary>
    public partial class CTHD : BasePage
    {
        public static string MaNV, CMND, Mahd;
        private int STT = 0;

        public class MatHang
        {
            public int STT { get; set; }
            public string MH { get; set; }
            public string TenHang { get; set; }
            public int SoLuong { get; set; }
            public int DonGia { get; set; }
            public int ThanhTien { get; set; }
            public int MaLo { get; set; }
        }
        public struct ChiTHD
        {
            public string MaHang;
            public int MaLo;
            public int SoLuong;
        }
        public List<MatHang> list = new List<MatHang>();
        public CTHD()
        {
            InitializeComponent();
            comMaHD.Text = Mahd;
            STT = 0;
            comMaHD_DropDownClosed(null, null);
        }

        private void TroVe_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeLaiSuat;
        }


        private void comMaHD_DropDownClosed(object sender, EventArgs e)
        {
            HangMua.Items.Clear();
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT MAHD FROM HOADON", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            bool flag = false;
            while (reader.Read())
            {
                if (reader.GetString(0) == comMaHD.Text)
                {
                    flag = true;
                    break;
                }
            }
            reader.Close();
            if (flag)
            {
                sqlCommand = new MySqlCommand("SELECT MAHANG, MALO, SOLUONG FROM CTHD WHERE MAHD = '" + comMaHD.Text + "'", sqlConn);
                reader = sqlCommand.ExecuteReader();
                int t = 0;
                List<ChiTHD> listCTHD = new List<ChiTHD>();
                while (reader.Read())
                {
                    ChiTHD tmp = new ChiTHD();
                    tmp.MaHang = reader.GetString(0);
                    tmp.MaLo = reader.GetInt32(1);
                    tmp.SoLuong = reader.GetInt32(2);
                    listCTHD.Add(tmp);
                }
                reader.Close();
                while (t < listCTHD.Count)
                {
                    MatHang tmp = new MatHang();
                    tmp.STT = t + 1;
                    string mahang = listCTHD[t].MaHang;
                    int malo = listCTHD[t].MaLo;
                    tmp.MaLo = malo;
                    tmp.MH = mahang;
                    tmp.SoLuong = listCTHD[t].SoLuong;
                    var cmd = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + mahang + "'", sqlConn);
                    var rd = cmd.ExecuteReader();
                    if (rd.Read()) tmp.TenHang = rd.GetString(0);
                    rd.Close();
                    cmd = new MySqlCommand("SELECT DONGIA FROM NHAPHANG WHERE MAHANG = '" + mahang + "' AND MALO = " + malo.ToString(), sqlConn);
                    rd = cmd.ExecuteReader();
                    if (rd.Read())
                    {
                        tmp.DonGia = Convert.ToInt32(Convert.ToInt32(rd.GetDecimal(0)) * 1.05);
                    }
                    tmp.ThanhTien = tmp.SoLuong * tmp.DonGia;
                    ThanhTien.Text = Convert.ToString(Convert.ToInt32(ThanhTien.Text) + tmp.ThanhTien);
                    rd.Close();
                    list.Add(tmp);
                    t++;
                    HangMua.Items.Add(new MatHang() { STT = tmp.STT, DonGia = tmp.DonGia, SoLuong = tmp.SoLuong, TenHang = tmp.TenHang, ThanhTien = tmp.ThanhTien });
                }
                ThanhTien.Text = string.Format("{0:#,##0}" + " VND", ThanhTien.Text);
            }
        }
    }
}