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
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for ThongTinChinhSuaDanhSach.xaml
    /// </summary>
    public partial class ThongTinChinhSuaDanhSach : BasePage
    {       
        public static string mahang = "";
        public static int malo;
        string value;
        public ThongTinChinhSuaDanhSach()
        {
            
            InitializeComponent();
            load();         
            
        }
        void load()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();
            string dvt = "" ;
            MAHG.Text = mahang;
            MALOO.Text = malo.ToString();
            var cmd = new MySqlCommand("SELECT  NGNHAP, HANSD, SOLUONG, DONGIA, DVT FROM NHAPHANG, LOAIHANG WHERE NHAPHANG.MAHANG='" + mahang + "' AND MALO= '" + malo + "' AND NHAPHANG.MAHANG = LOAIHANG.MAHANG", con);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                NGAY.Text = dr.GetDateTime(0).ToString("dd/MM/yyyy");
                HAN.Text = dr.GetDateTime(1).ToString("dd/MM/yyyy");
                SL.Text = dr.GetInt32(2).ToString();
                GIA.Text = Convert.ToInt32(dr.GetDecimal(3)).ToString();
                dvt = dr.GetString(4);
            }
            dr.Close();
            cmd = new MySqlCommand("SELECT TENHANG,DVT FROM LOAIHANG WHERE MAHANG='" + mahang + "'", con);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                TENHG.Text = dr.GetString(0);
                DONVI.Text= dr.GetString(1);
            }
            dr.Close();
            
            value = GIA.Text;
            GIA.Text = string.Format("{0:#,##0}" + " VND", double.Parse(GIA.Text));
            DONVI.SelectedItem = DONVI.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content.ToString() == dvt);
        }
        private void trolai(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeSoLuongHang;
        }

        private void xoathongtin(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();

            if (MessageBox.Show("Bạn có chắc muốn xóa thông tin hàng này???", "CÂU HỎI", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var cmd = new MySqlCommand("DELETE FROM NHAPHANG WHERE MAHANG='" + mahang + "' and MALO ='" + int.Parse(MALOO.Text) + "'", con);
                var dr= cmd.ExecuteReader();
                dr.Close();
                MessageBox.Show("Bạn đã xóa thành công thông tin này, Vui lòng kiểm tra trong danh sách!!!");
                MAHG.Text = "";
                MALOO.Text = "";
                NGAY.Text = "";
                TENHG.Text = "";
                DONVI.Text = "";
                HAN.Text = "";
                SL.Text = "";
                GIA.Text = "";
            }
            con.Close();
        }

        private void suathongtin(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();
            if (MAHG.Text == "" || MALOO.Text == "" || TENHG.Text == "" || NGAY.Text == "" || HAN.Text == "" || SL.Text == "" || DONVI.Text == "" || GIA.Text == "")
                MessageBox.Show("Vui lòng nhập đủ thông tin", "", MessageBoxButton.OK, MessageBoxImage.Error);
            
            else
            {
                var cmd = new MySqlCommand("UPDATE NHAPHANG SET HANSD = @HANSD WHERE MAHANG='" + mahang + "' AND MALO= '" + malo + "'", con);
               // cmd.Parameters.Add("@HANSD", System.Data.SqlDbType.SmallDateTime);
                cmd.Parameters["@HANSD"].Value = HAN.SelectedDate;
                var dr = cmd.ExecuteReader();
                dr.Close();

                cmd = new MySqlCommand("UPDATE LOAIHANG SET TENHANG = @TENHANG  WHERE MAHANG='" + mahang + "'", con);
                //cmd.Parameters.Add("@TENHANG", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@TENHANG"].Value = TENHG.Text;
                dr = cmd.ExecuteReader();
                dr.Close();

                cmd = new MySqlCommand("UPDATE LOAIHANG SET DVT = @DVT WHERE MAHANG='" + mahang + "'", con);
                //cmd.Parameters.Add("@DVT", System.Data.SqlDbType.NVarChar);
                cmd.Parameters["@DVT"].Value = DONVI.Text;
                dr = cmd.ExecuteReader();
                dr.Close();


                cmd = new MySqlCommand("UPDATE NHAPHANG SET DONGIA = @DONGIA WHERE MAHANG='" + mahang + "' AND MALO= '" + malo + "'", con);
                //cmd.Parameters.Add("@DONGIA", System.Data.SqlDbType.Money);
                cmd.Parameters["@DONGIA"].Value = Convert.ToInt32(value);
                dr = cmd.ExecuteReader();
                dr.Close();
                

                con.Close();
                MessageBox.Show("Sửa thông tin thành công!");
                ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeSoLuongHang;
            }
        }

        private void GIA_GotFocus(object sender, RoutedEventArgs e)
        {
            GIA.MaxLength = 7;
            GIA.Text = value;
        }

        private void GIA_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                GIA.MaxLength = 13;
                value = GIA.Text;
                GIA.Text = string.Format("{0:#,##0}" + " VND", double.Parse(GIA.Text));
            }
            catch
            {
                GIA.MaxLength = 7;
                value = "";
                GIA.Text = "";
            }
        }
    }
}
