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

namespace SalesManager.Pages
{
    /// <summary>
    /// Interaction logic for ThongKeDoanhThuThang.xaml
    /// </summary>
    public partial class ThongKeDoanhThuThang : BasePage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LiveCharts.SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public ThongKeDoanhThuThang()
        {
            InitializeComponent();
            DataContext = this;
            loadDTThang();
            tb_Title.Text = "THỐNG KÊ DOANH THU THÁNG " + thang + " NĂM " + nam;
        }
        double S = 0, tam = 0;
        private void loadDTThang()
        {
            double S1 = 0, S2 = 0, S3 = 0, S4 = 0, S5 = 0, S6 = 0, S7 = 0, S8 = 0, S9 = 0, S10 = 0, S11 = 0, S12 = 0, S13 = 0, S14 = 0, S15 = 0, S16 = 0, S17 = 0, S18 = 0, S19 = 0, S20 = 0, S21 = 0, S22 = 0, S23 = 0, S24 = 0, S25 = 0, S26 = 0, S27 = 0, S28 = 0, S29 = 0, S30 = 0, S31 = 0;
            string ngay = "";
            S = TongDT(S);
            tb_tongDT.Text = string.Format("{0:#,##0}" + " VND", S);
            tb_tongLai.Text = string.Format("{0:#,##0}" + " VND", S - S/1.05); 
            S1 = TongDTngay(S1, ngay = "1");
            S2 = TongDTngay(S2, ngay = "2");
            S3 = TongDTngay(S3, ngay = "3");
            S4 = TongDTngay(S4, ngay = "4");
            S5 = TongDTngay(S5, ngay = "5");
            S6 = TongDTngay(S6, ngay = "6");
            S7 = TongDTngay(S7, ngay = "7");
            S8 = TongDTngay(S8, ngay = "8");
            S9 = TongDTngay(S9, ngay = "9");
            S10 = TongDTngay(S10, ngay = "10");
            S11 = TongDTngay(S11, ngay = "11");
            S12 = TongDTngay(S12, ngay = "12");
            S13 = TongDTngay(S13, ngay = "13");
            S14 = TongDTngay(S14, ngay = "14");
            S15 = TongDTngay(S15, ngay = "15");
            S16 = TongDTngay(S16, ngay = "16");
            S17 = TongDTngay(S17, ngay = "17");
            S18 = TongDTngay(S18, ngay = "18");
            S19 = TongDTngay(S19, ngay = "19");
            S20 = TongDTngay(S20, ngay = "20");
            S21 = TongDTngay(S21, ngay = "21");
            S22 = TongDTngay(S22, ngay = "22");
            S23 = TongDTngay(S23, ngay = "23");
            S24 = TongDTngay(S24, ngay = "24");
            S25 = TongDTngay(S25, ngay = "25");
            S26 = TongDTngay(S26, ngay = "26");
            S27 = TongDTngay(S27, ngay = "27");
            S28 = TongDTngay(S28, ngay = "28");
            S29 = TongDTngay(S29, ngay = "29");
            S30 = TongDTngay(S30, ngay = "30");
            S31 = TongDTngay(S31, ngay = "31");
            SeriesCollection = new LiveCharts.SeriesCollection()
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<double> {  S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12,S13,S14,S15,S16,S17,S18,S19,S20,S21,S22,S23,S24,S25,S26,S27,S28,S29,S30,S31 }
                }
            };
            Labels = new[] { "Ngày 1", "Ngày 2", "Ngày 3", "Ngày 4", "Ngày 5", "Ngày 6", "Ngày 7", "Ngày 8", "Ngày 9", "Ngày 10", "Ngày 11", "Ngày 12", "Ngày 13", "Ngày 14", "Ngày 15", "Ngày 16", "Ngày 17", "Ngày 18", "Ngày 19", "Ngày20", "Ngày 21", "Ngày 22", "Ngày 23", "Ngày 24", "Ngày 25", "Ngày 26", "Ngày 27", "Ngày 28", "Ngày 29", "Ngày 30", "Ngày 31" };
            //Formatter = value => value.ToString("N");
        }

        private double TongDTngay(double S, string ngay)
        {
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TRIGIA FROM HOADON WHERE YEAR(NGHOADON)='" + nam + "' AND MONTH(NGHOADON)='" + thang + "' AND DAY(NGHOADON)='" + ngay+"'", sqlConn);
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
            var sqlCommand = new MySqlCommand("SELECT TRIGIA FROM HOADON WHERE YEAR(NGHOADON)=2021 AND MONTH(NGHOADON)='"+thang+"'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                S += double.Parse(reader[0].ToString());
            }
            reader.Close();
            sqlConn.Close();
            return S;
        }

        private void thoat_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)System.Windows.Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
        }

        private void thongkenam_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)System.Windows.Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeDoanhThu;
        }

        private void XuatExcel_click(object sender, RoutedEventArgs e)
        {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            Range myRange1 = (Range)sheet1.Cells[3, 1];
            //sheet1.Cells[3, 1].Font.Bold = true;
           // sheet1.Columns[1].ColumnWidth = 40;
            myRange1.Value2 = "Ngày";

            Range myRange2 = (Range)sheet1.Cells[3, 2];
            //sheet1.Cells[3, 2].Font.Bold = true;
            //sheet1.Columns[2].ColumnWidth = 40;
            myRange2.Value2 = "Tổng doanh thu (VNĐ)";

            sheet1.Range[sheet1.Cells[1, 1], sheet1.Cells[1, 2]].Merge();
            Range TitleExcel = (Range)sheet1.Cells[1, 1];
            TitleExcel.Value2 = "THỐNG KÊ DOANH THU THÁNG "+thang+" NĂM "+nam;
            //sheet1.Cells[1, 1].Font.Bold = true;
            //sheet1.Cells[1, 1].Font.Size = 18;
            sheet1.get_Range("A1", "B3").Cells.HorizontalAlignment =
                 Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            int r = 3;
            for (int i = 1; i <= 31; i++)
            {
                string x = "";
                Range myRange = (Range)sheet1.Cells[r + 1, 1];
                myRange.Value2 = i;
                myRange = (Range)sheet1.Cells[r + 1, 2];
                myRange.Value2 = TongDTngay(tam, x = i.ToString());
                r++;
            }
            Range myRange3 = (Range)sheet1.Cells[r + 1, 1];
            myRange3.Value2 = "                                                                    ------------";
            myRange3 = (Range)sheet1.Cells[r + 1, 2];
            myRange3.Value2 = "                                                                    ------------";
            r++;
            Range myRange4 = (Range)sheet1.Cells[r + 1, 1];
            myRange4.Value2 = "                                                            Tổng doanh thu: ";
            myRange4 = (Range)sheet1.Cells[r + 1, 2];
            myRange4.Value2 = S;
        }
    }
}