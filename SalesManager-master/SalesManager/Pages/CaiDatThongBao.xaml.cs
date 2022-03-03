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
using System.IO;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for CaiDatThongBao.xaml
    /// </summary>
    public partial class CaiDatThongBao : BasePage
    {
        public CaiDatThongBao()
        {
            InitializeComponent();
            MainWindow_Loaded();
           
        }
        private async void MainWindow_Loaded()
        {
            try
            {
                using (var sr = new StreamReader("Temp.txt"))
                {
                    textSL.Text = await sr.ReadToEndAsync();
                }
            }
            catch (FileNotFoundException ex)
            {
                textSL.Text = ex.Message;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText("Temp.txt", textSL.Text);
            }
            catch (FileNotFoundException ex)
            {
                textSL.Text = ex.Message;
            }
            MessageBox.Show("Chỉnh sửa thành công");
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.SideMenuControl;
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
        }
    }
}
