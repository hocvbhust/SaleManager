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
    public partial class TaoHoaDon : BasePage
    {
        public static string MaNV, CMND;
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
        public List<MatHang> list = new List<MatHang>();
        public TaoHoaDon()
        {
            InitializeComponent();
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();
            var cmd = new MySqlCommand("SELECT MAHANG FROM LOAIHANG", con);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comMaHang.Items.Add(dr.GetString(0));
            }
            dr.Close();
            var cmd2 = new MySqlCommand("SELECT MANV FROM NHANVIEN WHERE CMND = " + CMND, con);
            var dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                MaNV = dr2.GetString(0);
            }
            dr2.Close();
            con.Close();
            STT = 0;
        }
        private void Check_SoLuong(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void comMaHang_DropDownClosed(object sender, EventArgs e)
        {
            comMaLo.Items.Clear();
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT MAHANG FROM LOAIHANG", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            bool flag = false;
            while (reader.Read())
            {
                if (reader.GetString(0) == comMaHang.Text)
                {
                    flag = true;
                    break;
                }
            }
            reader.Close();
            if (flag)
            {
                MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                con.Open();
                var cmd = new MySqlCommand("SELECT MALO FROM NHAPHANG WHERE MAHANG = " + "'" + comMaHang.Text + "' AND SOLUONG>0", con);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    comMaLo.Items.Add(Convert.ToString(dr.GetInt32(0)));
                }
            }
            reader.Close();
            sqlCommand = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + comMaHang.Text + "'", sqlConn);
            reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                if (!reader.IsDBNull(0)) TextTenHang.Content = reader.GetString(0);
            }
        }

        private void Xoa_Click(object sender, RoutedEventArgs e)
        {
            if (HangMua.SelectedIndex == -1) return;
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            int i = HangMua.SelectedIndex;
            string SL = Convert.ToString(list[i].SoLuong);
            string MH = list[i].MH;
            string ML = Convert.ToString(list[i].MaLo);
            var sqlCommand = new MySqlCommand("UPDATE NHAPHANG SET SOLUONG = SOLUONG  + " + SL + " WHERE MALO = " + ML + " AND MAHANG = '" + MH + "'", sqlConn);
            sqlCommand.ExecuteNonQuery();
            string a = ThanhTien.Text;
            string b = string.Empty;
            for (int j = 0; j < a.Length; j++)
            {
                if (Char.IsDigit(a[j]))
                    b += a[j];
            }
            ThanhTien.Text = string.Format("{0:#,##0}" + " VND", double.Parse(Convert.ToString(Convert.ToInt32(b) - list[i].ThanhTien)));
            list.RemoveAt(i);
            for (i = 0; i < list.Count; i++) 
            {
                list[i].STT = i + 1;
            }
            HangMua.Items.Clear();
            for (i=0; i<list.Count; i++)
            {
                MatHang tmp = list[i];
                HangMua.Items.Add(new MatHang() { STT = tmp.STT, DonGia = tmp.DonGia, SoLuong = tmp.SoLuong, TenHang = tmp.TenHang, ThanhTien = tmp.ThanhTien });
            }    
            STT--;
        }

        private void TroVe_Click(object sender, RoutedEventArgs e)
        {
            for (int i=0; i<list.Count; i++)
            {
                var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                sqlConn.Open();
                string SL = Convert.ToString(list[i].SoLuong);
                string MH = list[i].MH;
                string ML = Convert.ToString(list[i].MaLo);
                var sqlCommand = new MySqlCommand("UPDATE NHAPHANG SET SOLUONG = SOLUONG  + " + SL + " WHERE MALO = " + ML + " AND MAHANG = '" + MH + "'", sqlConn);
                sqlCommand.ExecuteNonQuery();
            }
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.SideMenuControl;

        }

        private void TaoHoaDon_Click(object sender, RoutedEventArgs e)
        {
            if (ThanhTien.Text == "0") MessageBox.Show("Không thể tạo hóa đơn rỗng", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string a = ThanhTien.Text;
                string b = string.Empty;
                for (int i = 0; i < a.Length; i++)
                {
                    if (Char.IsDigit(a[i]))
                        b += a[i];
                }
                string MaHD = "HD";
                MaHD += DateTime.Now.Day.ToString();
                MaHD += DateTime.Now.Month.ToString();
                MaHD += DateTime.Now.Year.ToString();
                DateTime NgayHD = DateTime.Now;
                var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                sqlConn.Open();
                int SoHD = 0;
                var sqlCommand = new MySqlCommand("SELECT COUNT(*) FROM HOADON WHERE " +
                                                "YEAR(NGHOADON) = "+DateTime.Now.Year.ToString() +
                                                " AND MONTH(NGHOADON) = "+DateTime.Now.Month.ToString() +
                                                " AND DAY(NGHOADON) = "+DateTime.Now.Day.ToString(), sqlConn);
                var reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    SoHD = reader.GetInt32(0);
                }
                MaHD += (SoHD + 1).ToString();
                reader.Close();
                sqlCommand.CommandText = "INSERT INTO HOADON VALUES ('" +
                                          MaHD + "',@NGHOADON,'" +
                                          MaNV + "'," +
                                          b + ")";
                //sqlCommand.Parameters.Add("@NGHOADON", System.Data.SqlDbType.SmallDateTime);
                sqlCommand.Parameters["@NGHOADON"].Value = NgayHD;
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    sqlCommand.CommandText = "INSERT INTO CTHD VALUES ('" +
                                              MaHD + "','" +
                                              list[i].MH + "'," +
                                              Convert.ToString(list[i].SoLuong) + "," +
                                              Convert.ToString(list[i].MaLo) + ")";
                    sqlCommand.ExecuteNonQuery();
                }
                MessageBox.Show("Tạo thành công hóa đơn\nMã hóa đơn " + MaHD);
                XuatHoaDon.MaHoaDon = MaHD;
                HangMua.Items.Clear();
                list = new List<MatHang>();
                ThanhTien.Text = "0";
                ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.XuatHoaDon;
            }
        }

        private void comMaHang_TextInput(object sender, TextCompositionEventArgs e)
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + comMaHang.Text + "'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                if (!reader.IsDBNull(0)) TextTenHang.Content = reader.GetString(0);
            }
        }

        private void NhapHang_Click(object sender, RoutedEventArgs e)
        {
            if (comMaHang.Text == "" || textSL.Text == "" || comMaLo.SelectedIndex == -1) 
                MessageBox.Show("Vui lòng nhập đủ thông tin", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (Convert.ToInt32(textSL.Text) == 0) MessageBox.Show("Số lượng hàng nhập phải lớn hơn 0", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("SELECT MAHANG FROM LOAIHANG", sqlConn);
                var reader = sqlCommand.ExecuteReader();
                bool flag = false;
                while (reader.Read())
                {
                    if (reader.GetString(0) == comMaHang.Text)
                    {
                        flag = true;
                        break;
                    }
                }
                reader.Close();
                if (!flag) MessageBox.Show("Mã hàng không tồn tại trong hệ thống!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    sqlCommand = new MySqlCommand("SELECT SOLUONG FROM NHAPHANG WHERE MALO = " + comMaLo.Text + " AND MAHANG = '" +comMaHang.Text +"'", sqlConn);
                    reader = sqlCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        if (reader.GetInt32(0) < Convert.ToInt32(textSL.Text))
                        {
                            MessageBox.Show("Số lượng hàng còn lại không đủ", "", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    reader.Close();
                    sqlCommand.CommandText = "UPDATE NHAPHANG SET SOLUONG = SOLUONG - " + textSL.Text + " WHERE MALO = " + comMaLo.Text + "AND MAHANG = '" + comMaHang.Text + "'";
                    sqlCommand.ExecuteNonQuery();
                    MatHang tmp = new MatHang();
                    sqlCommand.CommandText = "SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + comMaHang.Text + "'";
                    reader = sqlCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        tmp.TenHang = reader.GetString(0);
                    }
                    reader.Close();
                    tmp.SoLuong = Convert.ToInt32(textSL.Text);
                    sqlCommand.CommandText = "SELECT DONGIA FROM NHAPHANG WHERE MALO = " + comMaLo.Text + "AND MAHANG = '" + comMaHang.Text + "'";
                    reader = sqlCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        tmp.DonGia = Convert.ToInt32(Convert.ToInt32(reader.GetDecimal(0))*1.05);
                    }
                    tmp.ThanhTien = tmp.SoLuong * tmp.DonGia;
                    STT++;
                    tmp.STT = STT;
                    tmp.MaLo = Convert.ToInt32(comMaLo.Text);
                    tmp.MH = comMaHang.Text;
                    reader.Close();
                    list.Add(tmp);
                    HangMua.Items.Add(new MatHang() { STT = tmp.STT, DonGia = tmp.DonGia, SoLuong = tmp.SoLuong, TenHang = tmp.TenHang, ThanhTien = tmp.ThanhTien });
                    textSL.Text = "";
                    comMaHang.Text = "";
                    comMaLo.Text = "";
                    TextTenHang.Content = "";
                    comMaLo.Items.Clear();
                    string a = ThanhTien.Text;
                    string b = string.Empty;
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (Char.IsDigit(a[i]))
                            b += a[i];
                    }
                    ThanhTien.Text = string.Format("{0:#,##0}" + " VND", double.Parse(Convert.ToString(Convert.ToInt32(b) + tmp.ThanhTien)));
                }
            }
        }
    }
}
