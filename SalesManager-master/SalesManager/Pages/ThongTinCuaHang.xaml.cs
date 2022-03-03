using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for ThongTinCuaHang.xaml
    /// </summary>
    public partial class ThongTinCuaHang : BasePage
    {
        public ThongTinCuaHang()
        {
            InitializeComponent();
        }
        string MaXacNhan;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Gmail;
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            try
            {
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("SELECT GMAIL FROM NHANVIEN WHERE MANV = 'NVQL'", sqlConn);
                var read = sqlCommand.ExecuteScalar();
                Gmail = read.ToString();
                sqlConn.Close();
                Random Ran = new Random();
                MaXacNhan = Ran.Next(100000, 999999).ToString();
                MailMessage Mess = new MailMessage("quanlibanhangnhom5@gmail.com", Gmail, "[QUẢN LÍ BÁN HÀNG]", "Mã xác nhận: " + MaXacNhan);
                Mess.BodyEncoding = System.Text.Encoding.UTF8;
                Mess.SubjectEncoding = System.Text.Encoding.UTF8;
                Mess.IsBodyHtml = true;
                Mess.Sender = new MailAddress("quanlibanhangnhom5@gmail.com", "QUANLIBANHANG");
                SmtpClient Client = new SmtpClient("smtp.gmail.com", 587);
                Client.EnableSsl = true;
                Client.Credentials = new NetworkCredential("quanlibanhangnhom5@gmail.com", "Quanlibanhangnhom5*");
                Client.Send(Mess);
                MessageBox.Show("Đã gửi mã xác nhận đến gmail của nhân viên quản lí. Bạn vui lòng kiểm tra trong hộp thư.");
            }
            catch (Exception)
            {
                MessageBox.Show("Gửi mã xác nhận thất bại.");
            }
        }
        private void BtnSua_Click(object sender, RoutedEventArgs e)
        {
            TenCuaHang.IsEnabled = true;
            NgayThanhLap.IsEnabled = true;
            NVQL.IsEnabled = true;
            MQL.Visibility = Visibility.Visible;
            Txmxn.Visibility = Visibility.Visible;
            MXN.Visibility = Visibility.Visible;
            btnMXN.Visibility = Visibility.Visible;

        }
        private void BtnLuu_Click(object sender, RoutedEventArgs e)
        {
            if (TenCuaHang.Text == "" || NgayThanhLap.Text == "" || MXN.Text == "")
            {
                MessageBox.Show("Bạn vui lòng điền đầy đủ thông tin");
                return;
            }
            if (MXN.Text != MaXacNhan)
            {
                MessageBox.Show("Mã xác nhận chưa chính xác. Vui lòng lấy lại mã xác nhận khác!!!");
                MXN.Clear();
                MXN.Focus();
                MaXacNhan = null;
                return;
            }
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            try
            {
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("UPDATE CUAHANG SET TENCH=@TENCH, NGTHANHLAP=@NGTHANHLAP", sqlConn);
                //sqlCommand.Parameters.Add("@TENCH", System.Data.SqlDbType.NVarChar);
                sqlCommand.Parameters["@TENCH"].Value = TenCuaHang.Text;
                //sqlCommand.Parameters.Add("@NGTHANHLAP", System.Data.SqlDbType.SmallDateTime);
                sqlCommand.Parameters["@NGTHANHLAP"].Value = NgayThanhLap.SelectedDate;
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                sqlConn.Close();
                if (MQL.Text != "NVQL")
                {
                    sqlConn.Open();
                    sqlCommand.CommandText = "UPDATE NHANVIEN SET MANV = @TEMP WHERE MANV = @MANV";
                    //sqlCommand.Parameters.Add("@TEMP", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@TEMP"].Value = "hahahaha";
                    //sqlCommand.Parameters.Add("@MANV", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MANV"].Value = MQL.Text;
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCommand.CommandText = "UPDATE CUAHANG SET MACHUCH = @MACHUCH ";
                    //sqlCommand.Parameters.Add("@MACHUCH", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MACHUCH"].Value = "hahahaha";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCommand.CommandText = "UPDATE NHANVIEN SET MANV = @TEMP1 WHERE MANV = @MANV1";
                   // sqlCommand.Parameters.Add("@TEMP1", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@TEMP1"].Value = MQL.Text;
                   // sqlCommand.Parameters.Add("@MANV1", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MANV1"].Value = "NVQL";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCommand.CommandText = "UPDATE CUAHANG SET MACHUCH = @MACHUCH1 ";
                   // sqlCommand.Parameters.Add("@MACHUCH1", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MACHUCH1"].Value = MQL.Text;
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCommand.CommandText = "UPDATE NHANVIEN SET MANV = @TEMP2 WHERE MANV = @MANV2";
                    //sqlCommand.Parameters.Add("@TEMP2", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@TEMP2"].Value = "NVQL";
                    //sqlCommand.Parameters.Add("@MANV2", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MANV2"].Value = "hahahaha";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();

                    sqlConn.Open();
                    sqlCommand.CommandText = "UPDATE CUAHANG SET MACHUCH = @MACHUCH2 ";
                    //sqlCommand.Parameters.Add("@MACHUCH2", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MACHUCH2"].Value = "NVQL";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();
                }


                TenCuaHang.IsEnabled = false;
                NgayThanhLap.IsEnabled = false;
                NVQL.IsEnabled = false;
                MQL.Visibility = Visibility.Hidden;
                Txmxn.Visibility = Visibility.Hidden;
                MXN.Visibility = Visibility.Hidden;
                btnMXN.Visibility = Visibility.Hidden;
                MaXacNhan = null;
                MQL.Text = "NVQL";
                MXN.Clear();
                MessageBox.Show("Cập nhật thông tin cửa hàng thành công");
            }
            catch
            {
                MessageBox.Show("Kết nối dữ liệu không thành công.");
            }
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            MaXacNhan = null;
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            try
            {
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("SELECT * FROM CUAHANG WHERE MACH = 'CH'", sqlConn);
                var read = sqlCommand.ExecuteReader();
                while (read.Read())
                {
                    TenCuaHang.Text = read[1].ToString();
                    NgayThanhLap.SelectedDate= (DateTime)read[3];
                }
                read.Close();
                sqlConn.Close();
                sqlConn.Open();
                sqlCommand.CommandText = "SELECT HOTEN,MANV FROM NHANVIEN";
                read = sqlCommand.ExecuteReader();
                while (read.Read())
                {
                    NVQL.Items.Add(read[0].ToString());
                    if (read[1].ToString() == "NVQL")
                    {
                        NVQL.Text = read[0].ToString();
                    }
                }
                //  MQL.Text = "NVQL";
            }
            catch
            {
                MessageBox.Show("Kết nối dữ liệu không thành công.");
            }
        }

        private void NVQL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            try
            {
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("SELECT MANV FROM NHANVIEN WHERE HOTEN = N'" + NVQL.SelectedItem.ToString() + "'", sqlConn);
                var read = sqlCommand.ExecuteScalar();
                MQL.Text = read.ToString();
                sqlConn.Close();
            }
            catch
            {
                MessageBox.Show("Kết nối dữ liệu không thành công.");
            }
        }
    }
}
