using MySql.Data.MySqlClient;
using SalesManager.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Data.SqlClient;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for BarcodeScanner.xaml
    /// </summary>
    public partial class BarcodeScanner : BasePage
    {
        //List<Goods> items = new List<Goods>();
        ObservableCollection<Goods> items = new ObservableCollection<Goods>();

        public BarcodeScanner()
        {
            InitializeComponent();
        }

        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void txt_barcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            txt_tienthua.Text = "";
            string barcode = txt_barcode.Text;
            if (!String.IsNullOrEmpty(barcode))
            {
                bool has = items.Any(c => c.Code == barcode);
                if (has)
                {
                    items.Single(c => c.Code == barcode).Count++;
                    items.Single(c => c.Code == barcode).setTotalPrice();
                    lvBarcode.Items.Refresh();
                }
                else
                {
                    MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
                    sqlConn.Open();
                    MySqlCommand sqlCommand = new MySqlCommand("SELECT A.MAHANG, TENHANG,B.DONGIA  FROM LOAIHANG A, NHAPHANG B WHERE A.MAHANG = B.MAHANG and B.MAHANG = " + barcode + ";", sqlConn);
                    MySqlDataReader reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        string Code = reader.GetString(0);
                        string Name = reader.GetString(1);
                        Int32 Price = Convert.ToInt32(reader.GetDecimal(2));
                        int Count = 1;
                        Goods g = new Goods(Code, Name, Price, Count);
                        items.Add(g);
                    }
                }
                lvBarcode.ItemsSource = items;

                TongTien_TextBlock.Text = String.Format("{0:0,0}", items.Sum(c => c.TotalPrice));
                txt_barcode.SelectAll();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Goods good = button.DataContext as Goods;
            items.Remove(good);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            Goods good = tb.DataContext as Goods;
            good.TotalPrice = good.Price * good.Count;
            items.Single(c => c.Code == good.Code).TotalPrice = good.TotalPrice;
            items.Single(c => c.Code == good.Code).TotalPriceText = String.Format("{0:0,0}", good.TotalPrice);
            TongTien_TextBlock.Text = String.Format("{0:0,0}", items.Sum(c => c.TotalPrice));
            lvBarcode.Items.Refresh();
        }

        private void btn_thanhtoan_Click(object sender, RoutedEventArgs e)
        {
            Int32 invoiceId = 0;
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT MAX(INVOICE_ID) from INVOICE");
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                invoiceId = reader.GetInt32(0);
            }

            sqlCommand = new MySqlCommand("INSERT INTO INVOICE (INVOICE_ID, CREATED_DATE , CREATED_USER, TOTAL_PRICE) VALUES " + "(@INVOICE_ID,@CREATED_DATE,@CREATED_USER,@TOTAL_PRICE)", sqlConn);
           // sqlCommand.Parameters.Add("@INVOICE_ID", System.Data.SqlDbType.Int);
            sqlCommand.Parameters["@INVOICE_ID"].Value = invoiceId + 1;
           // sqlCommand.Parameters.Add("@CREATED_DATE", System.Data.SqlDbType.DateTime);
            sqlCommand.Parameters["@CREATED_DATE"].Value = DateTime.Now;
          //  sqlCommand.Parameters.Add("@CREATED_USER", System.Data.SqlDbType.VarChar);
            sqlCommand.Parameters["@CREATED_USER"].Value = "abc";
           // sqlCommand.Parameters.Add("@TOTAL_PRICE", System.Data.SqlDbType.VarChar);
            //sqlCommand.Parameters["@TOTAL_PRICE"].Value = NgaySinh.SelectedDate;
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();

        }

        private void txt_tien_khach_dua_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
            decimal value = decimal.Parse(txt_tien_khach_dua.Text, System.Globalization.NumberStyles.AllowThousands);
            txt_tien_khach_dua.Text = String.Format(culture, "{0:N0}", value);
            txt_tien_khach_dua.Select(txt_tien_khach_dua.Text.Length, 0);

            Int32 customerCash = Int32.Parse(txt_tien_khach_dua.Text.Replace(",", ""));
            Int32 totalPrice = items.Sum(c => c.TotalPrice);
            txt_tienthua.Text = String.Format("{0:0,0}", customerCash - totalPrice);
        }

        private void Huy_Don_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "SERVER=127.0.0.1;DATABASE=DOANLTTQ_QUANLYHANGHOA;UID=root;PASSWORD=12345;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand("select * from LOAIHANG", connection);
            connection.Open();
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            connection.Close();
        }

        private void TroVe_Click(object sender, RoutedEventArgs e)
        {
            
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.SideMenuControl;

        }

        private void btn_print_invoice(object sender, RoutedEventArgs e)
        {

        }
    }
}
