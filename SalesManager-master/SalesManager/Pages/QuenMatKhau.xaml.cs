using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for QuenMatKhau.xaml
    /// </summary>
    public partial class QuenMatKhau : BasePage
    {
        public QuenMatKhau()
        {
            InitializeComponent();
        }
        string MaXacNhan;
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DangNhap;
        }
        private void GetPass_Click(object sender, RoutedEventArgs e)
        {
            if (CMND.Text == "" || Gmail.Text == "" || MatKhau.Password == "" || XNMatKhau.Password == "" || MXN.Text == "")
            {
                MessageBox.Show("Bạn vui lòng nhập đủ thông tin!!!");
                return;
            }
            if (MatKhau.Password != XNMatKhau.Password)
            {
                MessageBox.Show("Mật khẩu chưa trùng khớp!!!");
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
            sqlConn.Open();
            string MatKhauMaHoa = Encrypt(MatKhau.Password);
            try
            {
                MySqlCommand sqlCommand = new MySqlCommand("UPDATE NHANVIEN SET MATKHAU = @MATKHAU WHERE CMND = @CMND AND GMAIL = @gmail", sqlConn);
                //sqlCommand.Parameters.Add("@MATKHAU", System.Data.SqlDbType.NVarChar);
                sqlCommand.Parameters["@MATKHAU"].Value = MatKhauMaHoa;
                //sqlCommand.Parameters.Add("@CMND", System.Data.SqlDbType.NVarChar);
                sqlCommand.Parameters["@CMND"].Value = CMND.Text;
                //sqlCommand.Parameters.Add("@GMAIL", System.Data.SqlDbType.NVarChar);
                sqlCommand.Parameters["@GMAIL"].Value = Gmail.Text+"@gmail.com";
                var reader = sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                if (Convert.ToInt32(reader) == 1)
                {
                    CMND.Clear();
                    Gmail.Clear();
                    MXN.Clear();
                    MaXacNhan = null;
                    MatKhau.Clear();
                    XNMatKhau.Clear();
                    sqlConn.Close();
                    MessageBox.Show("Thành công.");
                    ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DangNhap;
                }
                else
                {
                    sqlConn.Close();
                    MessageBox.Show("CMND/CCCD hoặc Gmail chưa trùng khớp.");
                    return;
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Lỗi kết nối dữ liệu!!!");
                sqlConn.Close();
            }
        }

        private void Gmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-zA-Z]+");
            e.Handled = regex.IsMatch(e.Text);
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

        private void CMND_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
