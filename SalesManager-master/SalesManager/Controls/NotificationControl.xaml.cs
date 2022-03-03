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
using System.IO;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : BaseControl
    {
        public NotificationControl()
        {
            InitializeComponent();

            foreach (Border bor in listNotification)
            {
                var parent = VisualTreeHelper.GetParent(bor) as StackPanel;
                if (parent != null)
                    parent.Children.Remove(bor);
                mainPanel.Children.Add(bor);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainPanel.Children.Clear();
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).SideMenu = ApplicationPage.SideMenuControl;
        }
        public static ObservableCollection<Border> listNotification = new ObservableCollection<Border>();
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.CaiDatThongBao;
        }

    }
}
