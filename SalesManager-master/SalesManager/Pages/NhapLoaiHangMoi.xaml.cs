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
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.ObjectModel;
using SalesManager.DataModels;
using System.Globalization;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for NhapLoaiHangMoi.xaml
    /// </summary>
    public partial class NhapLoaiHangMoi : BasePage
    {
        MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
        ObservableCollection<Goods> listGoods = new ObservableCollection<Goods>();
        Goods updatedGoods = new Goods();
        public NhapLoaiHangMoi()
        {
            InitializeComponent();
            grid_update.Visibility = Visibility.Hidden;
        }

        private void Nhap_Click(object sender, RoutedEventArgs e)
        {
            if (textTenHang.Text == "") MessageBox.Show("Vui lòng nhập tên hàng", "", MessageBoxButton.OK, MessageBoxImage.Error);
            if (cbb_goodsType.Text == "") MessageBox.Show("Vui lòng nhập tên hàng", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                sqlConn.Open();
                var sqlCommand = new MySqlCommand("SELECT COUNT(*) FROM LOAIHANG where TENHANG = @TENHANG ", sqlConn);
                sqlCommand.Parameters.AddWithValue("@TENHANG", textTenHang.Text);
                int count = int.Parse(sqlCommand.ExecuteScalar().ToString());
                if (count > 0)
                {
                    MessageBox.Show("Loại hàng này đã tồn tại !!!");
                }
                else
                {
                    sqlCommand.Parameters.Clear();
                    sqlCommand.CommandText = "select max(MAHANG) from LOAIHANG";
                    int mahang_max = 0;
                    if (!String.IsNullOrEmpty(sqlCommand.ExecuteScalar().ToString()))
                    {
                        mahang_max = int.Parse(sqlCommand.ExecuteScalar().ToString());
                    }
                    sqlCommand.CommandText = "select type_id from Goods_Type where TRIM( Type_Name) = '" + cbb_goodsType.Text + "'";
                    int type_id = int.Parse(sqlCommand.ExecuteScalar().ToString());
                    sqlCommand.Parameters.Clear();
                    sqlCommand.CommandText = "INSERT INTO LOAIHANG (MAHANG, TENHANG, DVT, Type_id, Type_Name) VALUES " + "(@MAHANG,@TENHANG,@DVT,@TYPEID, @TYPENAME)";
                    sqlCommand.Parameters.AddWithValue("@MAHANG", mahang_max + 1);
                    sqlCommand.Parameters.AddWithValue("@TENHANG", textTenHang.Text);
                    sqlCommand.Parameters.AddWithValue("@DVT", comBoxDVT.Text);
                    sqlCommand.Parameters.AddWithValue("@TYPEID", type_id);
                    sqlCommand.Parameters.AddWithValue("@TYPENAME", cbb_goodsType.Text);
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    MessageBox.Show("Thêm dữ liệu thành công");

                    textTenHang.SelectAll();
                    comBoxDVT.Text = "Cái";
                    listGoods.Add(new Goods()
                    {
                        Code = mahang_max + 1 + "",
                        Name = textTenHang.Text,
                        DVT = comBoxDVT.Text,
                        GoodsType = cbb_goodsType.Text
                    });
                    lvGoods.ItemsSource = listGoods;
                    lvGoods.Items.Refresh();
                }
                sqlConn.Close();
            }
        }

        private void Trove_click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
        }

        private void cbb_goodsType_Loaded(object sender, RoutedEventArgs e)
        {
            //MySqlConnection sqlConn1 = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            List<string> listGoodsType = new List<string>();
            var sqlCommand = new MySqlCommand("SELECT type_name FROM Goods_Type", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                listGoodsType.Add(reader.GetString(0));
            }
            reader.Close();
            sqlConn.Close();
            cbb_goodsType.ItemsSource = listGoodsType;
        }
        private void cbb_goodsType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT A.MAHANG,A.TENHANG, A.DVT, B.Type_Name FROM LOAIHANG A, Goods_Type B where  A.Type_id = B.Type_id ", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                listGoods.Add(new Goods()
                {
                    Code = reader.GetString(0),
                    Name = reader.GetString(1),
                    DVT = reader.GetString(2),
                    GoodsType = reader.GetString(3)
                });
            }
            reader.Close();
            sqlConn.Close();
            lvGoods.ItemsSource = listGoods;
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("MouseDoubleClick");
            ListViewItem button = sender as ListViewItem;
            Goods good = button.DataContext as Goods;
            updatedGoods = good;
            //MessageBox.Show("MouseDoubleClick");
            textTenHang.Text = good.Name;
            cbb_goodsType.Text = good.GoodsType;
            comBoxDVT.Text = good.DVT;
            grid_update.Visibility = Visibility.Visible;
        }
        private void ListViewItem_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            MessageBox.Show("PreviewGotKeyboardFocus");
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Goods good = button.DataContext as Goods;
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa " + good.Name + " ?", "My App", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        //delete records in the database
                        sqlConn.Open();
                        var sqlCommand = new MySqlCommand("delete from LOAIHANG where MAHANG = @MAHANG", sqlConn);
                        sqlCommand.Parameters.AddWithValue("@MAHANG", good.Code);
                        sqlCommand.ExecuteNonQuery();
                        sqlCommand.Dispose();
                        //update the listview
                        listGoods.Remove(good);
                        lvGoods.Items.Refresh();
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
            sqlConn.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

            grid_update.Visibility = Visibility.Hidden;
            textTenHang.Text = "";
            comBoxDVT.Text = "Cái";
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("select type_id from Goods_Type where TRIM( Type_Name) = @TypeName", sqlConn);
            sqlCommand.Parameters.AddWithValue("@TypeName", updatedGoods.GoodsType);
            int type_id = int.Parse(sqlCommand.ExecuteScalar().ToString());
            sqlCommand.Parameters.Clear();

            sqlCommand = new MySqlCommand("update LOAIHANG set  TENHANG = @TENHANG, DVT = @DVT, Type_id = @Type_id where MAHANG = @MAHANG", sqlConn);
            sqlCommand.Parameters.AddWithValue("@MAHANG", updatedGoods.Code);
            sqlCommand.Parameters.AddWithValue("@TENHANG", textTenHang.Text);
            sqlCommand.Parameters.AddWithValue("@DVT", comBoxDVT.Text);
            sqlCommand.Parameters.AddWithValue("@Type_id", type_id);
            sqlCommand.ExecuteNonQuery();
            sqlCommand.Dispose();

            listGoods.Single(c => c.Code == updatedGoods.Code).Name = textTenHang.Text;
            listGoods.Single(c => c.Code == updatedGoods.Code).DVT = comBoxDVT.Text;
            listGoods.Single(c => c.Code == updatedGoods.Code).GoodsType = cbb_goodsType.Text;
            lvGoods.ItemsSource = listGoods;
            lvGoods.Items.Refresh();
            MessageBox.Show("Cập nhật loại hàng thành công !!!");
            sqlConn.Close();
            textTenHang.Text = "";
            comBoxDVT.Text = "Cái";
            grid_update.Visibility = Visibility.Hidden;
        }

        private void tb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var listGoodsSearch = new ObservableCollection<Goods>(listGoods.Where(x => x.Name.ToUpper().Contains(tb_search.Text.ToUpper())).ToList());
            lvGoods.ItemsSource = listGoodsSearch;
            lvGoods.Items.Refresh();
        }
    }
}
