using MySql.Data.MySqlClient;
using SalesManager.DataModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
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
    /// Interaction logic for DangNhap.xaml
    /// </summary>
    public partial class DangNhap : BasePage
    {
        private MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
        public DangNhap()
        {
            InitializeComponent();
            Check();

        }
        private void Check()
        {
            //MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            if (sqlConn.State == ConnectionState.Closed) { sqlConn.Open(); }
            MySqlCommand sqlCommandCheck;
            try
            {
                sqlCommandCheck = new MySqlCommand("SELECT COUNT(MACH) FROM CUAHANG", sqlConn);
                var reader = sqlCommandCheck.ExecuteScalar();
                if (Convert.ToInt32(reader) >= 1)
                {
                    DangKy.IsEnabled = false;
                }
                sqlConn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                sqlConn.Close();
                return;
            }

        }


        private void DangKy_Click(object sender, RoutedEventArgs e)
        {

            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DangKy;
        }
        private void DangNhap_Click(object sender, RoutedEventArgs e)
        {
            if (!(CMND.Text == "" || MatKhau.Password == ""))
            {
                //MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                sqlConn.Open();
                string MatKhauMaHoa =Tools.Encrypt(MatKhau.Password);
                try
                {
                    var sqlCommand = new MySqlCommand("SELECT CMND,MATKHAU FROM NHANVIEN WHERE CMND='" + CMND.Text + "'and MATKHAU ='" + MatKhauMaHoa + "'", sqlConn);
                    var reader = sqlCommand.ExecuteReader();
                    if (reader.Read() == true)
                    {
                        sqlConn.Close();
                        reader.Close();
                        NhapHangMoi.CMND = CMND.Text;
                        Home.CMND = CMND.Text;
                        TaoHoaDon.CMND = CMND.Text;
                        SuaHoaDon.CMND = CMND.Text;
                        ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
                        ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.SideMenuControl;
                    }
                    else
                    {
                        CMND.Clear();
                        MatKhau.Clear();
                        MessageBox.Show("CMND/CCCD hoặc mật khẩu của bạn chưa chính xác.");
                    }
                    reader.Close();
                }
                catch (Exception E1)
                {
                    MessageBox.Show(E1.Message.ToString());
                    sqlConn.Close();
                }

            }
            else
                MessageBox.Show("Bạn vui lòng nhập đầy đủ thông tin!!!");

        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.QuenMatKhau;
        }

        private void CMND_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
