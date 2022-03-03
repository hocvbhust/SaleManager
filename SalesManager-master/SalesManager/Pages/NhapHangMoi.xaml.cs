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
    /// Interaction logic for NhapHangMoi.xaml
    /// </summary>
    public partial class NhapHangMoi : BasePage
    {
        public static string MaNV, CMND;
        string valueDonGia;
        public NhapHangMoi()
        {
            InitializeComponent();
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();
            var cmd = new MySqlCommand("SELECT TENHANG FROM LOAIHANG", con);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comTenHang.Items.Add(dr.GetString(0));
            }
            dr.Close();

            var cmd2 = new MySqlCommand("SELECT MANV FROM NHANVIEN WHERE CMND = "+ CMND, con);
            var dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                MaNV = dr2.GetString(0);
            }
            dr2.Close();

            con.Close();
            NgayNhapHang.SelectedDate = DateTime.Now;
        }

        private void Check_SoLuong(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Check_DonGia(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NhapHang_Click(object sender, RoutedEventArgs e)
        {
            if (comTenHang.Text == "" || NgayNhapHang.Text == "" || HSD.Text == "" || textSL.Text == "" || textDonGia.Text == "")
                MessageBox.Show("Vui lòng nhập đủ thông tin", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (HSD.SelectedDate < NgayNhapHang.SelectedDate) MessageBox.Show("Hạn sử dụng phải lớn hơn ngày nhập", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (Convert.ToInt32(textSL.Text) == 0) MessageBox.Show("Số lượng hàng nhập phải lớn hơn 0", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (Convert.ToInt64(valueDonGia) <= 100) MessageBox.Show("Giá mặt hàng phải lớn hơn 100 đồng", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("SELECT MAHANG FROM LOAIHANG", sqlConn);
                var reader = sqlCommand.ExecuteReader();
                bool flag = false;
                while (reader.Read())
                {
                    if (reader.GetString(0) == comTenHang.Text)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag) MessageBox.Show("Mã hàng không tồn tại trong hệ thống!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    reader.Close();
                    sqlCommand.CommandText = "SELECT MAX(MALO) FROM NHAPHANG WHERE MAHANG = " + "'" + comTenHang.Text + "'";
                    reader = sqlCommand.ExecuteReader();
                    int MaLo = 0;
                    if (reader.Read())
                    {
                        if (!(reader.GetValue(0) is DBNull)) MaLo = reader.GetInt32(0);
                    }
                    reader.Close();
                    sqlCommand.CommandText = "INSERT INTO NHAPHANG VALUES (@MAHANG,@MALO,@NGNHAP,@HANSD,@SOLUONG,@DONGIA,@MANV)";
                    //sqlCommand.Parameters.Add("@MAHANG", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MAHANG"].Value = comTenHang.Text;
                   // sqlCommand.Parameters.Add("@MALO", System.Data.SqlDbType.Int);
                    sqlCommand.Parameters["@MALO"].Value = MaLo + 1;
                    //sqlCommand.Parameters.Add("@NGNHAP", System.Data.SqlDbType.SmallDateTime);
                    sqlCommand.Parameters["@NGNHAP"].Value = NgayNhapHang.SelectedDate;
                    //sqlCommand.Parameters.Add("@HANSD", System.Data.SqlDbType.SmallDateTime);
                    sqlCommand.Parameters["@HANSD"].Value = HSD.SelectedDate;
                   // sqlCommand.Parameters.Add("@SOLUONG", System.Data.SqlDbType.Int);
                    sqlCommand.Parameters["@SOLUONG"].Value = Convert.ToInt32(textSL.Text);
                   // sqlCommand.Parameters.Add("@DONGIA", System.Data.SqlDbType.Money);
                    sqlCommand.Parameters["@DONGIA"].Value = Convert.ToInt32(valueDonGia);
                   // sqlCommand.Parameters.Add("@MANV", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MANV"].Value = MaNV;
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    MessageBox.Show("Thêm hàng thành công!");
                    comTenHang.Text = "";
                    NgayNhapHang.SelectedDate = DateTime.Now;
                    HSD.Text = "";
               
                    textDonGia.Text = "";
                    textSL.Text = "";
                }
            }
        }

        private void comTenHang_DropDownClosed(object sender, EventArgs e)
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '"+comTenHang.Text+"'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                //if (!reader.IsDBNull(0)) TextTenHang.Content = reader.GetString(0);
            }    
        }

        private void comTenHang_TextInput(object sender, TextCompositionEventArgs e)
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + comTenHang.Text + "'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                //if (!reader.IsDBNull(0)) TextTenHang.Content = reader.GetString(0);
            }
        }

        private void textDonGia_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                textDonGia.MaxLength = 13;
                valueDonGia = textDonGia.Text;
                textDonGia.Text = string.Format("{0:#,##0}" + " VND", double.Parse(textDonGia.Text));
            }
            catch
            {
                textDonGia.MaxLength = 7;
                valueDonGia = "";
                textDonGia.Text = "";
            }
        }

        private void textDonGia_GotFocus(object sender, RoutedEventArgs e)
        {
            textDonGia.MaxLength = 7;
            textDonGia.Text = valueDonGia;
        }

        private void TroVe_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
        }
    }
}
