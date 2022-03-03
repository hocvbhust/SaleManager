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
using MySql.Data.MySqlClient;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for ChonThang.xaml
    /// </summary>
    public partial class ChonThang : BasePage
    {
        MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
        public ChonThang()
        {
            InitializeComponent();

            for (int i = 1; i <= 12; i++)
            {
                thang.Items.Add(i);
                if (i == DateTime.Now.Month) thang.SelectedIndex = i - 1;
            }
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT YEAR(NGTHANHLAP) FROM CUAHANG ", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            int year=2000;
            while (reader.Read())
            {
                year = reader.GetInt32(0);
            }
            reader.Close();
            sqlConn.Close();
            for (int i = year ; i <= DateTime.Now.Year; i++)
            {
                nam.Items.Add(i);
                if (i == DateTime.Now.Year) nam.SelectedIndex = i - year;
            }
        }

        private void xem_Click(object sender, RoutedEventArgs e)
        {
            HangDaBan.Nam = nam.SelectedItem.ToString();
            HangDaBan.Thang = thang.SelectedItem.ToString();
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.HangDaBan;
        }

        private void Thoat_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.VisualOff;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.SideMenuControl;
        }
    }
}
