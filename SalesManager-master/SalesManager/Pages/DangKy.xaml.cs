using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Data;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for DangKy.xaml
    /// </summary>
    public partial class DangKy : BasePage
    {
        public DangKy()
        {
            InitializeComponent();
        }
        public static string Encrypt(string toEncrypt)
        {
            string key = "";
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private void SDT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void CMND_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void Gmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-zA-Z]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DangNhap;
        }
        string MaXacNhan;
        private void HoanTat_Click(object sender, RoutedEventArgs e)
        {
            // Nhập chưa đủ thông tin
            if (TenCuaHang.Text == "" || NgayThanhLap.Text == "" || TenQuanLi.Text == "" || NgaySinh.Text == "" || SDT.Text == "" || DiaChi.Text == "" || CMND.Text == "" || MatKhau.Password == "" || XNMatKhau.Password == "" || Gmail.Text == "" || MXN.Text == "")
            {
                MessageBox.Show("Bạn vui lòng nhập đủ thông tin!!!");
                return;
            }
            // Mã xác nhận không khớp
            /*if (MXN.Text != MaXacNhan)
            {
                MessageBox.Show("Mã xác nhận chưa chính xác. Vui lòng lấy lại mã xác nhận khác!!!");
                MXN.Clear();
                MXN.Focus();
                MaXacNhan = null;
                return;
            }*/
            //Mật khẩu không khớp
            if (MatKhau.Password != XNMatKhau.Password)
            {
                MessageBox.Show("Mật khẩu chưa trùng khớp!!!");
                return;
            }
            else
            {
                MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                try
                {
                    string MatKhauMaHoa = Encrypt(MatKhau.Password);
                    sqlConn.Open();
                    var sqlCommand = new MySqlCommand("INSERT INTO NHANVIEN (MANV,HOTEN,NGSINH,CMND,DIACHI,NGVAOLAM,MATKHAU,GMAIL) VALUES " + "(@MANV,@HOTEN,@NGSINH,@CMND,@DIACHI,@NGVAOLAM,@MATKHAU,@GMAIL)", sqlConn);
                    //sqlCommand.Parameters.Add("@MANV", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MANV"].Value = "NVQL";
                    //sqlCommand.Parameters.Add("@HOTEN", System.Data.SqlDbType.NVarChar);
                    sqlCommand.Parameters["@HOTEN"].Value = TenQuanLi.Text;
                   // sqlCommand.Parameters.Add("@NGSINH", System.Data.SqlDbType.SmallDateTime);
                    sqlCommand.Parameters["@NGSINH"].Value = NgaySinh.SelectedDate;
                   // sqlCommand.Parameters.Add("@CMND", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@CMND"].Value = CMND.Text;
                   // sqlCommand.Parameters.Add("@DIACHI", System.Data.SqlDbType.NVarChar);
                    sqlCommand.Parameters["@DIACHI"].Value = DiaChi.Text;
                   // sqlCommand.Parameters.Add("@NGVAOLAM", System.Data.SqlDbType.SmallDateTime);
                    sqlCommand.Parameters["@NGVAOLAM"].Value = NgayThanhLap.SelectedDate;
                   // sqlCommand.Parameters.Add("@MATKHAU", System.Data.SqlDbType.NVarChar);
                    sqlCommand.Parameters["@MATKHAU"].Value = MatKhauMaHoa;
                   // sqlCommand.Parameters.Add("@GMAIL", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@GMAIL"].Value = Gmail.Text + "@gmail.com";
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();
                    sqlConn.Open();
                    sqlCommand.CommandText = "INSERT INTO CUAHANG (MACH,TENCH,MACHUCH,NGTHANHLAP) VALUES " + "(@MACH,@TENCH,@MACHUCH,@NGTHANHLAP)";
                   // sqlCommand.Parameters.Add("@MACH", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MACH"].Value = "CH";
                   // sqlCommand.Parameters.Add("@TENCH", System.Data.SqlDbType.NVarChar);
                    sqlCommand.Parameters["@TENCH"].Value = TenCuaHang.Text;
                    //sqlCommand.Parameters.Add("@MACHUCH", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MACHUCH"].Value = "NVQL";
                   // sqlCommand.Parameters.Add("@NGTHANHLAP", System.Data.SqlDbType.SmallDateTime);
                    sqlCommand.Parameters["@NGTHANHLAP"].Value = NgayThanhLap.SelectedDate;
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    sqlConn.Close();
                    MessageBox.Show("Tạo tài khoản thành công.");
                    ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DangNhap;
                }
                catch (Exception)
                {
                    MessageBox.Show("Lỗi kết nối dữ liệu!!!");
                    sqlConn.Close();
                    return;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Gmail.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập gmail.");
                return;
            }
            Random Ran = new Random();
            MaXacNhan = Ran.Next(100000, 999999).ToString();
            MailMessage Mess = new MailMessage("quanlibanhangnhom5@gmail.com", Gmail.Text + "@gmail.com", "[QUẢN LÍ BÁN HÀNG]", "Mã xác nhận: " + MaXacNhan);
            Mess.BodyEncoding = System.Text.Encoding.UTF8;
            Mess.SubjectEncoding = System.Text.Encoding.UTF8;
            Mess.IsBodyHtml = true;
            Mess.Sender = new MailAddress("quanlibanhangnhom5@gmail.com", "QUANLIBANHANG");
            SmtpClient Client = new SmtpClient("smtp.gmail.com", 587);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential("quanlibanhangnhom5@gmail.com", "Quanlibanhangnhom5*");
            try
            {
                Client.Send(Mess);
                MessageBox.Show("Đã gửi mã xác nhận. Bạn vui lòng kiểm tra trong hộp thư gmail của bạn.");
            }
            catch (Exception)
            {
                MessageBox.Show("Gửi mã xác nhận thất bại.");
            }

        }

        private void Gmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            MXN.Clear();
            MaXacNhan = null;
        }
    }
}
