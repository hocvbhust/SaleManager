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
using Excel = Microsoft.Office.Interop.Excel;


namespace SalesManager
{
    /// <summary>
    /// Interaction logic for ThongKeLaiSuat.xaml
    /// </summary>
    public partial class ThongKeLaiSuat : BasePage
    {
        public ThongKeLaiSuat()
        {
            InitializeComponent();
            SapXep_ComboBox.SelectedIndex = 0;
            SapXepTheo_ComboBox.SelectedIndex = 0;
            SapXep_ComboBox.SelectionChanged += SapXep_ComboBox_SelectionChanged;
            SapXepTheo_ComboBox.SelectionChanged += SapXep_ComboBox_SelectionChanged;
            TimKiem_TextBox.TextChanged += TimKiem_TextBox_TextChanged;
            LoadList();
        }
        private void SapXep_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
                view.SortDescriptions.Clear();
                if (SapXepTheo_ComboBox.SelectedIndex == 0)
                {
                    switch (SapXep_ComboBox.SelectedIndex)
                    {
                        case 0:
                            view.SortDescriptions.Add(new SortDescription("NGAY", ListSortDirection.Ascending));
                            break;
                        case 1:
                            view.SortDescriptions.Add(new SortDescription("GIAGOC", ListSortDirection.Ascending));
                            break;
                        case 2:
                            view.SortDescriptions.Add(new SortDescription("TIENLAI", ListSortDirection.Ascending));
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    switch (SapXep_ComboBox.SelectedIndex)
                    {
                        case 0:
                            view.SortDescriptions.Add(new SortDescription("NGAY", ListSortDirection.Descending));
                            break;
                        case 1:
                            view.SortDescriptions.Add(new SortDescription("GIAGOC", ListSortDirection.Descending));
                            break;
                        case 2:
                            view.SortDescriptions.Add(new SortDescription("TIENLAI", ListSortDirection.Descending));
                            break;
                        default:
                            return;
                    }
                }

                CollectionViewSource.GetDefaultView(lvUsers.ItemsSource).Refresh();
            }
            catch
            {
                MessageBox.Show("Chưa có thông tin mặt hàng");
            }
        }

        public class LoaiHD
        {
            public string MAHD { get; set; }
            public string NGAY { get; set; }
            public int GIAGOC { get; set; }
            public int GIABAN { get; set; }
            public int TIENLAI { get; set; }
        }
        void LoadList()
        {
            int Sum = 0;
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            MySqlCommand sqlCommand = new MySqlCommand("SELECT MAHD, NGHOADON, TRIGIA FROM HOADON  ", sqlConn);
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            List<LoaiHD> items = new List<LoaiHD>();
            while (reader.Read())
            {
                items.Add(new LoaiHD()
                {
                    MAHD = reader.GetString(0),
                    NGAY = reader.GetDateTime(1).ToString("dd/MM/yyyy"),
                    GIABAN = Convert.ToInt32(reader.GetDecimal(2)),
                    GIAGOC = Convert.ToInt32(Convert.ToInt32(reader.GetDecimal(2)) / 1.05),
                    TIENLAI = Convert.ToInt32(reader.GetDecimal(2)) - Convert.ToInt32(Convert.ToInt32(reader.GetDecimal(2)) / 1.05)
                });
                lvUsers.ItemsSource = items;
            }
            try
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("MAHD", ListSortDirection.Ascending));
                view.Filter = UserFilter;
                foreach (LoaiHD item in items)
                {
                    Sum += item.TIENLAI;
                }
                TongTien_TextBlock.Text = string.Format("{0:#,##0}", double.Parse(Sum.ToString()));
                reader.Close();
                sqlConn.Close();
            }
            catch
            {
                MessageBox.Show("Chưa có thông tin hóa đơn");
            }
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(TimKiem_TextBox.Text))
                return true;
            else
                return ((item as LoaiHD).MAHD.IndexOf(TimKiem_TextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private void TimKiem_TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                CollectionViewSource.GetDefaultView(lvUsers.ItemsSource).Refresh();
            }
            catch
            {
                MessageBox.Show("Chưa có thông tin hóa đơn");
            }
        }
        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CTHD.Mahd = ((LoaiHD)lvUsers.SelectedItem).MAHD;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.CTHD;
        }
    }
}
