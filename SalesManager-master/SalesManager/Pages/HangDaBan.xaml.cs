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

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for HangDaBan.xaml
    /// </summary>
    public partial class HangDaBan : BasePage
    {
        public static string Thang;
         public static string Nam;
        MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
        public class Soluong
        {
            public string MAHANG { get; set; }
            public string TENHANG { get; set; }
            public int SLBAN { get; set; }
        }
        public HangDaBan()
        {
            InitializeComponent();
            LoadListBANHANG();
        }
        void LoadListBANHANG()
        {
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT CTHD.MAHANG , TENHANG,  SUM(SOLUONG) AS SL" +
            " FROM HOADON, CTHD, LOAIHANG WHERE HOADON.MAHD = CTHD.MAHD AND LOAIHANG.MAHANG = CTHD.MAHANG " +
            "AND MONTH(NGHOADON) = '" + Thang + "' AND YEAR(NGHOADON) = '" + Nam + "'" +
            "GROUP BY CTHD.MAHANG, TENHANG ORDER BY SL DESC", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            List<Soluong> items = new List<Soluong>();
            while (reader.Read())
            {
                items.Add(new Soluong()
                {
                    MAHANG = reader.GetString(0),
                    TENHANG = reader.GetString(1),
                    SLBAN = reader.GetInt32(2)
                });
                LISTBANHANG.ItemsSource = items;
            }
            reader.Close();
            sqlConn.Close();
        }
        private void Thoat_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ChonThang;
        }

    }
}
