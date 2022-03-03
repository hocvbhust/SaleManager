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

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for TaoMaNhanVien.xaml
    /// </summary>
    public partial class TaoMaNhanVien : BasePage
    {
        public static string cmnd;
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
        public TaoMaNhanVien()
        {
            InitializeComponent();
            con.Open();
            var cmd = new MySqlCommand("SELECT HOTEN FROM NHANVIEN WHERE CMND = " + cmnd, con);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string HoTen = dr.GetString(0);
                tennv.Text = $"{HoTen}";
            }
            dr.Close();
            con.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
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
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MK.Password == "" || pass.Password == "" || repass.Password == "")
                MessageBox.Show("Vui lòng nhập đủ thông tin", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                string mk = Encrypt(MK.Password);
                string Pass = Encrypt(pass.Password);
                string RePass = Encrypt(repass.Password);
                con.Open();
                var sqlCommand = new MySqlCommand("SELECT MANV,HOTEN FROM NHANVIEN WHERE CMND ='" + cmnd + "'and MATKHAU ='" + mk + "'", con);
                var reader = sqlCommand.ExecuteReader();
                if (reader.Read() == true && Pass == RePass)
                {
                    reader.Close();
                    var cmd = new MySqlCommand("UPDATE NHANVIEN SET MATKHAU = @MATKHAU WHERE CMND ='" + cmnd + "'", con);
                    //cmd.Parameters.Add("@MATKHAU", System.Data.SqlDbType.NVarChar);
                    cmd.Parameters["@MATKHAU"].Value = Pass;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();

                    MK.Password = "";
                    pass.Password = "";
                    repass.Password = "";
                    MessageBox.Show("Sửa mật khẩu thành công!");
                }
                else
                {
                    reader.Close();
                    tennv.Text = "";
                    MK.Password = "";
                    pass.Password = "";
                    repass.Password = "";
                    MessageBox.Show("Sai thông tin! ");
                }

            }
        }
    }
}
