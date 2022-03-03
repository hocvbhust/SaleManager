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
using Microsoft.Office.Interop.Excel;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for ThayDoiThongTinMatHang.xaml
    /// </summary>
    public partial class ThongKeSoLuongHang : BasePage
    {
        public ThongKeSoLuongHang()
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
                            view.SortDescriptions.Add(new SortDescription("TENHANG", ListSortDirection.Ascending));
                            break;
                        case 1:
                            view.SortDescriptions.Add(new SortDescription("GIA", ListSortDirection.Ascending));
                            break;
                        case 2:
                            view.SortDescriptions.Add(new SortDescription("HSD", ListSortDirection.Ascending));
                            break;
                        case 3:
                            view.SortDescriptions.Add(new SortDescription("SOLUONG", ListSortDirection.Ascending));
                            break;
                        case 4:
                            view.SortDescriptions.Add(new SortDescription("TONGGIA", ListSortDirection.Ascending));
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
                            view.SortDescriptions.Add(new SortDescription("TENHANG", ListSortDirection.Descending));
                            break;
                        case 1:
                            view.SortDescriptions.Add(new SortDescription("GIA", ListSortDirection.Descending));
                            break;
                        case 2:
                            view.SortDescriptions.Add(new SortDescription("HSD", ListSortDirection.Descending));
                            break;
                        case 3:
                            view.SortDescriptions.Add(new SortDescription("SOLUONG", ListSortDirection.Descending));
                            break;
                        case 4:
                            view.SortDescriptions.Add(new SortDescription("TONGGIA", ListSortDirection.Descending));
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

        public class LoaiHangHoa
        {
            public string MAHANG { get; set; }
            public int MALO { get; set; }
            public string TENHANG { get; set; }
            public int SOLUONG { get; set; }
            public string HSD { get; set; }
            public int GIA { get; set; }
            public int TONGGIA { get; set; }
        }
        void LoadList()
        {
            int Sum = 0;
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            MySqlCommand sqlCommand = new MySqlCommand("SELECT * FROM LOAIHANG A, NHAPHANG B WHERE A.MAHANG = B.MAHANG AND SOLUONG != 0 ", sqlConn);
            MySqlDataReader reader = sqlCommand.ExecuteReader();
            List<LoaiHangHoa> items = new List<LoaiHangHoa>();
            while (reader.Read())
            {
                items.Add(new LoaiHangHoa()
                {
                    MAHANG = reader.GetString(0),
                    TENHANG = reader.GetString(1),
                    MALO = reader.GetInt32(4),
                    SOLUONG = reader.GetInt32(7),
                    GIA = Convert.ToInt32(reader.GetDecimal(8)),
                    HSD = $"{reader.GetDateTime(6).Day}/{reader.GetDateTime(6).Month}/{reader.GetDateTime(6).Year}",
                    TONGGIA = Convert.ToInt32(reader.GetDecimal(8)) * reader.GetInt32(7)
                });
                lvUsers.ItemsSource = items;
            }
            try
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvUsers.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("TENHANG", ListSortDirection.Ascending));
                view.Filter = UserFilter;
                foreach (LoaiHangHoa item in items)
                {
                    Sum += item.TONGGIA;
                }
                TongTien_TextBlock.Text = string.Format("{0:#,##0}", double.Parse(Sum.ToString())); 
                reader.Close();
                sqlConn.Close();
            }
            catch
            {
                MessageBox.Show("Chưa có thông tin mặt hàng");
            }
        }

        private bool UserFilter(object item)
        {
            if (String.IsNullOrEmpty(TimKiem_TextBox.Text))
                return true;
            else
                return ((item as LoaiHangHoa).TENHANG.IndexOf(TimKiem_TextBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        private void TimKiem_TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                CollectionViewSource.GetDefaultView(lvUsers.ItemsSource).Refresh();
            }
            catch
            {
                MessageBox.Show("Chưa có thông tin mặt hàng");
            }
        }
        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ThongTinChinhSuaDanhSach.mahang = ((LoaiHangHoa)lvUsers.SelectedItem).MAHANG;
            ThongTinChinhSuaDanhSach.malo = ((LoaiHangHoa)lvUsers.SelectedItem).MALO;
        }
        private void CHINHSUA(object sender, RoutedEventArgs e)
        {
            if (ThongTinChinhSuaDanhSach.mahang == "") MessageBox.Show("Nháy đúp chọn hàng trước khi sửa thông tin!");
            else
            {
                ((WindowViewModel)((MainWindow)System.Windows.Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongTinChinhSuaDanhSach;
            }

        }
        private void XuatExcel(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int i = 0; i < 7; i++)
            {
                Range myRange = (Range)sheet1.Cells[3, i + 1];
               //sheet1.Cells[3, i + 1].Font.Bold = true;
                //sheet1.Columns[i + 1].ColumnWidth = 30;
                myRange.Value2 = ((GridView)lvUsers.View).Columns[i].Header;
            }
            sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[1, 7]].Merge();
            Range TitleExcel = (Range)sheet1.Cells[1, 1];
            TitleExcel.Value2 = "Quản lý bán hàng";
            //sheet1.Cells[1, 1].Font.Bold = true;
            //sheet1.Cells[1, 1].Font.Size = 18;
            sheet1.get_Range("A1", "G3").Cells.HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            int r = 3;
            List<LoaiHangHoa> items = (List<LoaiHangHoa>)lvUsers.ItemsSource;
            foreach (LoaiHangHoa item in items)
            {
                Range myRange = (Range)sheet1.Cells[r + 1, 1];
                myRange.Value2 = item.MAHANG;
                myRange = (Range)sheet1.Cells[r + 1, 2];
                myRange.Value2 = item.MALO;
                myRange = (Range)sheet1.Cells[r + 1, 3];
                myRange.Value2 = item.TENHANG;
                myRange = (Range)sheet1.Cells[r + 1, 4];
                myRange.Value2 = item.SOLUONG;
                myRange = (Range)sheet1.Cells[r + 1, 5];
                myRange.Value2 = item.HSD;
                myRange = (Range)sheet1.Cells[r + 1, 6];
                myRange.Value2 = item.GIA;
                myRange = (Range)sheet1.Cells[r + 1, 7];
                myRange.Value2 = item.TONGGIA;
                r++;
            }

        }
    }
}
