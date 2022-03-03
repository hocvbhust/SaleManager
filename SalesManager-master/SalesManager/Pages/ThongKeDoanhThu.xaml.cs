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
using System.Collections.ObjectModel;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for ThongKeDoanhThu.xaml
    /// </summary>
    public partial class ThongKeDoanhThu : BasePage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LiveCharts.SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public ThongKeDoanhThu()
        {
            InitializeComponent();
            DataContext = this;
            loadDTNam();
            LoadListNam();
        }
        double S = 0, tam = 0;
        private void loadDTNam()
        {
            double  S1 = 0, S2 = 0, S3 = 0, S4 = 0, S5 = 0, S6 = 0, S7 = 0, S8 = 0, S9 = 0, S10 = 0, S11 = 0, S12 = 0;
            string thang = "";
            S = TongDT(S);
            tb_tongDT.Text = string.Format("{0:#,##0}" + " VND", S);
            tb_Lai.Text = string.Format("{0:#,##0}" + " VND", S-S/1.05);
            S1 = TongDTthang(S1, thang = "1");
            S2 = TongDTthang(S2, thang = "2");
            S3 = TongDTthang(S3, thang = "3");
            S4 = TongDTthang(S4, thang = "4");
            S5 = TongDTthang(S5, thang = "5");
            S6 = TongDTthang(S6, thang = "6");
            S7 = TongDTthang(S7, thang = "7");
            S8 = TongDTthang(S8, thang = "8");
            S9 = TongDTthang(S9, thang = "9");
            S10 = TongDTthang(S10, thang = "10");
            S11 = TongDTthang(S11, thang = "11");
            S12 = TongDTthang(S12, thang = "12");
            SeriesCollection = new LiveCharts.SeriesCollection()
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<double> {  S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12 }
                }
            };
            //SeriesCollection.Add(new ColumnSeries() { Title = "2016", Values = new ChartValues<double> { 10, 50, 39, 50 } });

            Labels = new[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" };

            //Formatter = value => value.ToString("N");
        }

        private double TongDTthang(double S, string thang)
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TRIGIA FROM HOADON WHERE YEAR(NGHOADON)=2021 AND MONTH(NGHOADON)=" + thang, sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                S += double.Parse(reader[0].ToString());
            }
            reader.Close();
            sqlConn.Close();
            return S;
        }
        double TongDT(double S)
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TRIGIA FROM HOADON WHERE YEAR(NGHOADON)=2021", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                S += double.Parse(reader[0].ToString());
            }
            reader.Close();
            sqlConn.Close();
            return S;
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
        private void thoat_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)System.Windows.Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
            ((WindowViewModel)((MainWindow)System.Windows.Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.SideMenuControl;
        }

        private void ThongKeThang_Click(object sender, RoutedEventArgs e)
        {
            if (cbThang.Text == "" || cbNam.Text == "")
            {
                MessageBox.Show("Vui lòng chọn đủ tháng, năm!");
            }
            else
            {
                thang = cbThang.Text;
                nam = cbNam.Text;
                ((WindowViewModel)((MainWindow)System.Windows.Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeDoanhThuThang;
            }

        }

        private void XuatEX_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            Range myRange1 = (Range)sheet1.Cells[3, 1];
           // sheet1.Cells[3,1].Font.Bold = true;
            //sheet1.Columns[1].ColumnWidth = 30;
            myRange1.Value2 = "Tháng";

            Range myRange2 = (Range)sheet1.Cells[3, 2];
            //sheet1.Cells[3,2].Font.Bold = true;
            //sheet1.Columns[2].ColumnWidth = 30;
            myRange2.Value2 = "Tổng doanh thu (VNĐ)";

            sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[1, 2]].Merge();
            Range TitleExcel = (Range)sheet1.Cells[1, 1];
            TitleExcel.Value2 = "THỐNG KÊ DOANH THU NĂM 2021";
            //sheet1.Cells[1, 1].Font.Bold = true;
            //sheet1.Cells[1, 1].Font.Size = 18;
            sheet1.get_Range("A1", "B3").Cells.HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            int r = 3;
            for(int i=1; i<=12;i++)
            {
                string x = "";
                Range myRange = (Range)sheet1.Cells[r + 1, 1];
                myRange.Value2 = i;
                myRange = (Range)sheet1.Cells[r + 1, 2];
                myRange.Value2 = TongDTthang(tam, x = i.ToString());
                r++;
            }
            Range myRange3 = (Range)sheet1.Cells[r + 1, 1];
            myRange3.Value2 = "                                                ------------";
            myRange3 = (Range)sheet1.Cells[r + 1, 2];
            myRange3.Value2 = "                                                ------------";
            r++;
            Range myRange4 = (Range)sheet1.Cells[r + 1, 1];
            myRange4.Value2 = "                                     Tổng doanh thu: ";
            myRange4 = (Range)sheet1.Cells[r + 1, 2];
            myRange4.Value2 = S;
        }
    }
}
