using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
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


namespace SalesManager
{
    /// <summary>
    /// Interaction logic for SideMenuControl.xaml
    /// </summary>
    public partial class SideMenuControl : BaseControl
    {
        #region Public Variables
        bool ShowThemHangContent = false;
        static public bool NotificationCheck = false;
        static public int Count = 0;
        #endregion

        #region Constructor
        public SideMenuControl()
        {
            InitializeComponent();
            string Manv = "";
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand($"SELECT MANV FROM NHANVIEN WHERE CMND = " + Home.CMND, sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                Manv = reader.GetString(0);
            }
            if (Manv != "NVQL")
            {
                Staff_Click(null, null);
            }
            NotificationControl.listNotification.CollectionChanged += ListNotification_CollectionChanged;
            if (NotificationCheck == true)
            {
                this.CoThongBao.Visibility = Visibility.Hidden;
            }
            if (NotificationControl.listNotification.Count == 0 || NotificationControl.listNotification.Count == Count)
            {
                this.CoThongBao.Visibility = Visibility.Hidden;
            }
            Count = NotificationControl.listNotification.Count;
            //sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            //sqlConn.Open();
            //sqlCommand = new MySqlCommand($"DELETE FROM NHAPHANG ", sqlConn);
            //reader = sqlCommand.ExecuteReader();
        }
        #endregion

        #region UI for SideMenu

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
        }

        private void ResetColor()
        {
            BrushConverter bc = new BrushConverter();
            ThemHang_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            DanhSachNV_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            ThongKe_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            TaoHoaDon_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            //ThongKeSoLuongHang_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            ThongKeSoLuongHang_Button1.Background = (Brush)bc.ConvertFrom("#00BCD4");
            HangDaBan_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            SuaMatKhau_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            ThongTinCuaHang_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            ThongKeLaiSuat_Button.Background = (Brush)bc.ConvertFrom("#00BCD4");
            ThemLoaiHangMoi_Button.Visibility = Visibility.Collapsed;
            Nhaphangvaokho_Button.Visibility = Visibility.Collapsed;
            ShowThemHangContent = false;
        }
        #endregion

        #region Notification Component
        private void ListNotification_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (NotificationControl.listNotification.Count == 0 || NotificationControl.listNotification.Count == Count)
            {
                this.CoThongBao.Visibility = Visibility.Hidden;
            }
            else
            {
                this.CoThongBao.Visibility = Visibility.Visible;
                NotificationCheck = false;
            }
        }

        private void Notification_Click(object sender, RoutedEventArgs e)
        {
            NotificationCheck = true;
            ResetColor();
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.NotificationControl;
        }

        #endregion

        #region Staff Component
        private void Staff_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            Staff_Button.Foreground = Brushes.DarkSlateBlue;
            Staff_Button.FontSize = 50;
            Manager_Button.Foreground = Brushes.White;
            Manager_Button.FontSize = 30;
            BrushConverter brush = new BrushConverter();
            Setting_Button.Foreground = (Brush)brush.ConvertFrom("#0c6991");
            Setting_Button.FontSize = 30;
            this.Staff.Visibility = Visibility.Visible;
            this.Manager.Visibility = Visibility.Hidden;
            this.Setting.Visibility = Visibility.Hidden;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
        }
        private void ThemHang_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            if (ShowThemHangContent == false)
            {
                ThemHang_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
                ThemLoaiHangMoi_Button.Visibility = Visibility.Visible;
                Nhaphangvaokho_Button.Visibility = Visibility.Visible;
                ShowThemHangContent = true;
            }
            else
            {
                ThemLoaiHangMoi_Button.Visibility = Visibility.Collapsed;
                Nhaphangvaokho_Button.Visibility = Visibility.Collapsed;
                ShowThemHangContent = false;
            }
        }
        private void ThemLoaiHangDaCo_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            ResetColor();
            ThemLoaiHangMoi_Button.Visibility = Visibility.Visible;
            Nhaphangvaokho_Button.Visibility = Visibility.Visible;
            ShowThemHangContent = true;
            ThemHang_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.NhapHangMoi;
        }
        private void ThemLoaiHangMoi_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter bc = new BrushConverter();
            ResetColor();
            ThemLoaiHangMoi_Button.Visibility = Visibility.Visible;
            Nhaphangvaokho_Button.Visibility = Visibility.Visible;
            ShowThemHangContent = true;
            ThemHang_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.NhapLoaiHangMoi;
        }
        private void ThongKeSoLuongHang_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            Nhaphangvaokho_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ThongKeSoLuongHang_Button1.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeSoLuongHang;
        }
        private void SuaHoaDon_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            TaoHoaDon_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.SuaHoaDon;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.VisualOff;
        }

        private void TaoHoaDon_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            TaoHoaDon_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.TaoHoaDon;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.VisualOff;
        }
        private void HangDaBan_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            HangDaBan_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ChonThang;
        }
        #endregion

        #region Manager Component
        private void Manager_Click(object sender, RoutedEventArgs e)
        {
            string Manv = "";
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand($"SELECT MANV FROM NHANVIEN WHERE CMND = " + Home.CMND, sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                Manv = reader.GetString(0);
            }
            if (Manv != "NVQL")
            {
                MessageBox.Show("Chỉ có chủ cửa hàng mới có thể sử dụng những chức năng này");
            }
            else
            {
                Manager_Button.Foreground = Brushes.DarkSlateBlue;
                Manager_Button.FontSize = 50;
                Staff_Button.Foreground = Brushes.White;
                Staff_Button.FontSize = 30;
                BrushConverter brush = new BrushConverter();
                Setting_Button.Foreground = (Brush)brush.ConvertFrom("#0c6991");
                Setting_Button.FontSize = 30;
                ResetColor();
                this.Manager.Visibility = Visibility.Visible;
                this.Staff.Visibility = Visibility.Hidden;
                this.Setting.Visibility = Visibility.Hidden;
                ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
            }
        }
        private void DSNV_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            DanhSachNV_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DanhSachNhanVien;
        }
        private void ThongKeDT_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            ThongKe_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeDoanhThu;
        }
        #endregion

        #region Setting Component
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            Manager_Button.Foreground = Brushes.White;
            Manager_Button.FontSize = 30;
            Staff_Button.Foreground = Brushes.White;
            Staff_Button.FontSize = 30;
            BrushConverter brush = new BrushConverter();
            Setting_Button.Foreground = (Brush)brush.ConvertFrom("#024a69");
            Setting_Button.FontSize = 75;
            this.Staff.Visibility = Visibility.Hidden;
            this.Manager.Visibility = Visibility.Hidden;
            this.Setting.Visibility = Visibility.Visible;

        }
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.DangNhap;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.VisualOff;
        }
        private void SuaMatKhau_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            SuaMatKhau_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.TaoMaNhanVien;
        }
        private void ThongTinCuaHang_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            ThongTinCuaHang_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongTinCuaHang;
        }
        #endregion

        private void ThongKeLaiSuat_Button_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            ThongKeLaiSuat_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.ThongKeLaiSuat;
        }

        private void BarcodeScanner_Click(object sender, RoutedEventArgs e)
        {
            ResetColor();
            BrushConverter bc = new BrushConverter();
            TaoHoaDon_Button.Background = (Brush)bc.ConvertFrom("#0A5E5A");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.BarcodeScanner;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.VisualOff;
        }
    }
}

