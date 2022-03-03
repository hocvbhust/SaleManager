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
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SalesManager.Pages
{
    /// <summary>
    /// Interaction logic for SuaNhanVien.xaml
    /// </summary>
    public partial class SuaNhanVien : BasePage
    {
        public SuaNhanVien()
        {
            InitializeComponent();
            if (manv != "") load_ttsuaNV();
        }
        bool flag1 = false;
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
        public static string Decrypt(string toDecrypt)
        {
            string key = "";
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

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

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        void load_ttsuaNV()
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT * FROM NHANVIEN WHERE MANV= '" + manv + "'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                //tbMANV.Text = reader[0].ToString();
                tbHOTEN.Text = reader[1].ToString();
                ngSinh.Text = reader[2].ToString();
                tbCMND.Text = reader[3].ToString();
                tbDIACHI.Text = reader[4].ToString();
                ngVL.Text = reader[5].ToString();
                tbMK.Password = reader[6].ToString();
                tbGmail.Text = reader[7].ToString();
            }
            reader.Close();
            sqlConn.Close();
            tbMK.Password = Decrypt(tbMK.Password.ToString());
            tbRMK.Password = tbMK.Password;
        }
        private void Them_Click(object sender, RoutedEventArgs e)
        {

            if (tbHOTEN.Text == "" || ngSinh.Text == "" || tbCMND.Text == "" || tbDIACHI.Text == "" || ngVL.Text == "" || tbMK.Password == "" || tbRMK.Password == "" || tbGmail.Text == "") MessageBox.Show("Vui lòng nhập đủ thông tin", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (ngVL.SelectedDate < ngSinh.SelectedDate) MessageBox.Show("Ngày sinh phải nhỏ hơn ngày vào làm", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (tbMK.Password != tbRMK.Password) MessageBox.Show("Mật khẩu nhập lại không chính xác. Vui lòng thử lại!", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (flag1 == false) MessageBox.Show("Vui lòng kiểm tra lại CMND!", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string pass = Encrypt(tbMK.Password);
                var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("UPDATE NHANVIEN SET HOTEN = @HOTEN, NGSINH = @NGSINH, CMND = @CMND, DIACHI = @DIACHI, NGVAOLAM = @NGVAOLAM, MATKHAU = @MATKHAU, GMAIL = @GMAIL WHERE MANV = '" + manv + "'", sqlConn);
                MessageBoxResult reSult = MessageBox.Show("Nhân viên đã tồn tại trong hệ thống, bạn có muốn ghi đè dữ liệu đang có không?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (reSult == MessageBoxResult.Yes)
                {
                    //sqlCommand.Parameters.Add("@MANV", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@MANV"].Value = manv;
                    //sqlCommand.Parameters.Add("@HOTEN", System.Data.SqlDbType.NVarChar);
                    sqlCommand.Parameters["@HOTEN"].Value = tbHOTEN.Text;
                    //sqlCommand.Parameters.Add("@NGSINH", System.Data.SqlDbType.SmallDateTime);
                    sqlCommand.Parameters["@NGSINH"].Value = ngSinh.SelectedDate;
                    //sqlCommand.Parameters.Add("@CMND", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@CMND"].Value = tbCMND.Text;
                    //sqlCommand.Parameters.Add("@DIACHI", System.Data.SqlDbType.NVarChar);
                    sqlCommand.Parameters["@DIACHI"].Value = tbDIACHI.Text;
                    //sqlCommand.Parameters.Add("@NGVAOLAM", System.Data.SqlDbType.SmallDateTime);
                    sqlCommand.Parameters["@NGVAOLAM"].Value = ngVL.SelectedDate;
                    //sqlCommand.Parameters.Add("@MATKHAU", System.Data.SqlDbType.NVarChar);
                    sqlCommand.Parameters["@MATKHAU"].Value = pass;
                    //sqlCommand.Parameters.Add("@GMAIL", System.Data.SqlDbType.VarChar);
                    sqlCommand.Parameters["@GMAIL"].Value = tbGmail.Text;
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    MessageBox.Show("Cập nhật dữ liệu thành công");
                    tbHOTEN.Text = "";
                    ngSinh.Text = "";
                    tbCMND.Text = "";
                    tbDIACHI.Text = "";
                    ngVL.Text = "";
                    tbMK.Password = "";
                    tbGmail.Text = "";
                    ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DanhSachNhanVien;
                }

            }
        }

        private void Huy_Click(object sender, RoutedEventArgs e)
        {
            manv = "";
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DanhSachNhanVien;
        }

        private void kiemtra1_Click(object sender, RoutedEventArgs e)
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT MANV,CMND FROM NHANVIEN", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            bool flag = true;
            while (reader.Read())
            {
                if (reader[0].ToString() != manv && reader.GetString(1) == tbCMND.Text)
                {
                    MessageBox.Show("Chứng minh nhân dân đã tồn tại, vui lòng nhập CMND khác!");
                    flag = false;
                    break;
                }
            }
            reader.Close();
            sqlConn.Close();
            if (flag)
            {
                MessageBox.Show("Chứng minh nhân dân hợp lệ!");
                flag1 = true;
            }
        }

        private void tbCMND_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
