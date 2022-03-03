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
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Windows.Threading;
using System.IO;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : BasePage
    {
        public static string CMND;
        
        public Home()
        {
            InitializeComponent();
            
            NotificationControl.listNotification.Clear();
            int sl = 10;
            try
            {
                using (var sr = new StreamReader("Temp.txt"))
                {
                    sl = Convert.ToInt32(sr.ReadToEnd());
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();
            TaoMaNhanVien.cmnd = CMND;
            var cmd = new MySqlCommand("SELECT HOTEN FROM NHANVIEN WHERE CMND = "+ CMND , con);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                string HoTen = dr.GetString(0);
                Usr.Text = $"{HoTen}";
            }
            dr.Close();
            con.Close();

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
            var sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT SUM(SOLUONG), TENHANG, A.MAHANG, DVT FROM NHAPHANG A, LOAIHANG B WHERE A.MAHANG=B.MAHANG  GROUP BY A.MAHANG, TENHANG, DVT", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32(0) < sl && reader.GetInt32(0) != 0)
                {
                    var newItem = new Border();
                    newItem.Tag = true;
                    newItem.Margin = new Thickness(0, 1, 0, 1);
                    newItem.Width = 240;
                    var bc = new BrushConverter();

                    newItem.Background = (Brush)bc.ConvertFrom("#005c99");
                    newItem.BorderBrush = (Brush)bc.ConvertFrom("#1f004d");
                    newItem.BorderThickness = new Thickness(1, 1, 1, 1);
                    newItem.CornerRadius = new CornerRadius(5);
                    newItem.Opacity = 0.95;
                    StackPanel stack = new StackPanel();
                    newItem.Child = stack;
                    stack.Orientation = Orientation.Horizontal;
                    TextBlock textBlock = new TextBlock();
                    textBlock.Width = 100;
                    textBlock.Text = $"Mặt hàng {reader.GetString(1).ToLower()} sắp hết hàng!!!";
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Foreground = Brushes.White;
                    textBlock.FontSize = 15;
                    textBlock.FontWeight = FontWeights.Bold;
                    textBlock.Padding = new Thickness(3, 3, 0, 3);
                    textBlock.FontFamily = new FontFamily("Segoe UI");
                    textBlock.TextAlignment = TextAlignment.Center;

                    TextBlock textBlock1 = new TextBlock();
                    textBlock1.Width = 120;
                    textBlock1.Text = $"Mã hàng: {reader.GetString(2)}\nSố lượng hàng còn lại: {reader.GetInt32(0)} {reader.GetString(3).ToLower()}";
                    textBlock1.TextWrapping = TextWrapping.Wrap;
                    textBlock1.Foreground = Brushes.White;
                    textBlock1.FontSize = 15;
                    textBlock1.Padding = new Thickness(2, 3, 3, 3);
                    textBlock1.FontFamily = new FontFamily("Segoe UI");
                    textBlock1.TextAlignment = TextAlignment.Center;
                    stack.Children.Add(textBlock);

                    stack.Children.Add(textBlock1);
                    NotificationControl.listNotification.Add(newItem);
                }
            }
            reader.Close();
            sqlConn.Close();


            sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            sqlCommand = new MySqlCommand("SELECT * FROM NHAPHANG A, LOAIHANG B WHERE A.MAHANG = B.MAHANG", sqlConn);
            reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetDateTime(3).Date == DateTime.Now.AddDays(1).Date && reader.GetInt32(4) != 0)
                {
                    var newItem = new Border();
                    newItem.Width = 240;
                    newItem.Tag = true;
                    newItem.Margin = new Thickness(0, 1, 0, 1);
                    newItem.Opacity = 0.95;
                    var bc = new BrushConverter();
                    newItem.Background = (Brush)bc.ConvertFrom("#1f1f7a");
                    newItem.BorderBrush = (Brush)bc.ConvertFrom("#0a0a29");
                    newItem.BorderThickness = new Thickness(1, 1, 1, 1);
                    newItem.CornerRadius = new CornerRadius(5);
                    StackPanel stack = new StackPanel();
                    newItem.Child = stack;
                    stack.Orientation = Orientation.Horizontal;
                    TextBlock textBlock = new TextBlock();
                    textBlock.Width = 100;
                    textBlock.FontWeight = FontWeights.Bold;
                    textBlock.Text = $"Mặt hàng {reader.GetString(8).ToLower()} sắp hết hạn!!!";
                    textBlock.TextWrapping = TextWrapping.Wrap;
                    textBlock.Foreground = Brushes.White;
                    textBlock.FontSize = 15;
                    textBlock.Padding = new Thickness(3, 3, 0, 3);
                    textBlock.FontFamily = new FontFamily("Segoe UI");
                    textBlock.TextAlignment = TextAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;

                    TextBlock textBlock1 = new TextBlock();
                    textBlock1.Width = 120;
                    textBlock1.Text = $"Mã hàng: {reader.GetString(0)}\nMã lô: {reader.GetInt32(1)}\nHSD: {reader.GetDateTime(3).Day}/{reader.GetDateTime(3).Month}/{reader.GetDateTime(3).Year}\nSố lượng hàng: {reader.GetInt32(4)}";
                    textBlock1.TextWrapping = TextWrapping.Wrap;
                    textBlock1.Foreground = Brushes.White;
                    textBlock1.FontSize = 15;
                    textBlock1.Padding = new Thickness(2, 3, 3, 3);
                    textBlock.FontFamily = new FontFamily("Segoe UI");
                    textBlock1.TextAlignment = TextAlignment.Center;

                    stack.Children.Add(textBlock);

                    stack.Children.Add(textBlock1);
                    NotificationControl.listNotification.Add(newItem);
                }
            }
            reader.Close();
            sqlConn.Close();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
