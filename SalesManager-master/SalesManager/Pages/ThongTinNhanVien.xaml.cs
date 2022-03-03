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
using System.Data;
using System.ComponentModel;
using System.Security.Cryptography;
namespace SalesManager
{
    /// <summary>
    /// Interaction logic for ThongTinNhanVien.xaml
    /// </summary>
    public partial class ThongTinNhanVien : BasePage
    {
        public ThongTinNhanVien()
        {
            InitializeComponent();
            Load();
            LoadListHD();
            LoadListNam();
        }

        public class HOADON
        {
            public string MAHD { get; set; }
            public string NGHOADON { get; set; }
            public int TRIGIA { get; set; }
        }

        void LoadListHD()
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT MAHD,NGHOADON,TRIGIA FROM HOADON WHERE MANV='" + manv + "'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            List<HOADON> items = new List<HOADON>();
            while (reader.Read())
            {
                items.Add(new HOADON() { MAHD = reader[0].ToString(), NGHOADON = reader.GetDateTime(1).ToString("MM/dd/yyyy"), TRIGIA = Convert.ToInt32(reader.GetDecimal(2)) });
                lvHOADON.ItemsSource = items;
            }
            reader.Close();
            sqlConn.Close();
        }

        void Load()
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT * FROM NHANVIEN WHERE MANV= '" + manv + "'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                lb_manv.Text = reader[0].ToString();
                lb_ten.Text = reader[1].ToString();
                lb_ngsinh.Text = $"{reader.GetDateTime(2).Day}/{reader.GetDateTime(2).Month}/{reader.GetDateTime(2).Year}";
                lb_cmnd.Text = reader[3].ToString();
                lb_diachi.Text = reader[4].ToString();
                lb_ngvl.Text = $"{reader.GetDateTime(5).Day}/{reader.GetDateTime(5).Month}/{reader.GetDateTime(5).Year}";
                lb_gmail.Text = reader[7].ToString();
            }
            reader.Close();
            sqlConn.Close();
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

        private void suaNV_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.SuaNhanVien;
        }

        private void XoaNV_Click(object sender, RoutedEventArgs e)
        {
            if (manv == "NVQL")
            { MessageBox.Show("Không được xóa nhân viên quản lý!", "THÔNG BÁO", MessageBoxButton.OK, MessageBoxImage.Error); }
            else
            if (MessageBox.Show("Bạn có chắc muốn xóa nhân viên này???", "CÂU HỎI", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("DELETE FROM HOADON WHERE MANV='" + manv + "'", sqlConn);
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();

                sqlCommand.CommandText = "DELETE FROM NHANVIEN WHERE MANV='" + manv + "'";
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
                sqlConn.Close();
                ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DanhSachNhanVien;
            }
        }

        void LoadListNam()
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT DISTINCT YEAR(NGHOADON) FROM HOADON ORDER BY YEAR(NGHOADON) ASC", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                cbNam.Items.Add(reader[0].ToString());
            }
            reader.Close();
            sqlConn.Close();
        }
        private void Thongke_Click(object sender, RoutedEventArgs e)
        {
            if (cbThang.Text == "" || cbNam.Text == "")
            {
                MessageBox.Show("Vui lòng chọn đủ tháng, năm!");
            }
            else
            {
                thang = cbThang.Text;
                nam = cbNam.Text;
                ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DoanhThuNhanVien;
            }
        }
    }
}
