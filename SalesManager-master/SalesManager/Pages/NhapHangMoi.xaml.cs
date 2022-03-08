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
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using SalesManager.DataModels;

namespace SalesManager
{
    /// <summary>
    /// Interaction logic for NhapHangMoi.xaml
    /// </summary>
    public partial class NhapHangMoi : BasePage
    {
        public static string MaNV, CMND;
        string valueDonGia;
        ObservableCollection<Goods> listGoods = new ObservableCollection<Goods>();
        Goods updatedGoods = new Goods();
        public NhapHangMoi()
        {
            InitializeComponent();

            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            con.Open();
            var cmd = new MySqlCommand("SELECT TENHANG FROM LOAIHANG", con);
            var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comTenHang.Items.Add(dr.GetString(0));
            }
            dr.Close();

            var cmd2 = new MySqlCommand("SELECT MANV FROM NHANVIEN WHERE CMND = " + CMND, con);
            var dr2 = cmd2.ExecuteReader();
            while (dr2.Read())
            {
                MaNV = dr2.GetString(0);
            }
            dr2.Close();

            con.Close();
            NgayNhapHang.SelectedDate = DateTime.Now;
        }

        private void Check_SoLuong(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Check_DonGia(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void NhapHang_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            string goodsType = "";
            var sqlCommand = new MySqlCommand("select max(ID) from NHAPHANG", sqlConn); ;
            int id_max = 0;
            if (!String.IsNullOrEmpty(sqlCommand.ExecuteScalar().ToString()))
            {
                id_max = int.Parse(sqlCommand.ExecuteScalar().ToString());
            }
            
            string goodsCode = "";
            if (comTenHang.Text == "" || NgayNhapHang.Text == "" || textSL.Text == "" || textDonGia.Text == "")
                MessageBox.Show("Vui lòng nhập đủ thông tin", "", MessageBoxButton.OK, MessageBoxImage.Error);

            else if (Convert.ToInt32(textSL.Text) == 0) MessageBox.Show("Số lượng hàng nhập phải lớn hơn 0", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (Convert.ToInt64(valueDonGia) <= 1000) MessageBox.Show("Giá mặt hàng phải lớn hơn 1000 đồng", "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                    sqlCommand.Parameters.Clear();
                    sqlCommand = new MySqlCommand("SELECT MAHANG,Type_Name  FROM LOAIHANG WHERE TENHANG = @TENHANG", sqlConn);
                    sqlCommand.Parameters.AddWithValue("@TENHANG", comTenHang.Text);
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        goodsCode = reader.GetString(0).Trim();
                        goodsType = reader.GetString(1).Trim();
                        break;
                    }
                    if (goodsCode == "") MessageBox.Show("Mã hàng không tồn tại trong hệ thống!", "", MessageBoxButton.OK, MessageBoxImage.Error);
                    reader.Close();

                bool has = listGoods.Any(g => g.Code == goodsCode);
                
                if (has)
                {
                    var eGoods = listGoods.First(g1 => g1.Code == goodsCode);
                    sqlCommand.Parameters.Clear();
                    sqlCommand = new MySqlCommand("Update NHAPHANG set SOLUONG = (@SOLUONG) where MAHANG = @MAHANG", sqlConn);
                    sqlCommand.Parameters.AddWithValue("@SOLUONG", eGoods.Count + Convert.ToInt32(textSL.Text));
                    sqlCommand.Parameters.AddWithValue("@MAHANG", goodsCode);
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                    eGoods.Count = eGoods.Count + Convert.ToInt32(textSL.Text);
                    lvGoods.Items.Refresh();
                }
                else
                {
                    var sqlCommand1 = new MySqlCommand("INSERT INTO NHAPHANG (MAHANG,NGNHAP, SOLUONG,DONGIA,MANV) VALUES (@MAHANG,@NGNHAP,@SOLUONG,@DONGIA,@MANV)", sqlConn);

                    sqlCommand1.Parameters.AddWithValue("@MAHANG", goodsCode);
                    sqlCommand1.Parameters.AddWithValue("@NGNHAP", NgayNhapHang.SelectedDate);
                    sqlCommand1.Parameters.AddWithValue("@SOLUONG", Convert.ToInt32(textSL.Text));
                    sqlCommand1.Parameters.AddWithValue("@DONGIA", Convert.ToInt32(valueDonGia));
                    sqlCommand1.Parameters.AddWithValue("@MANV", MaNV);
                    sqlCommand1.ExecuteNonQuery();
                    sqlCommand1.Dispose();
                    var good = new Goods()
                    {
                        ID = id_max + "",
                        Code = goodsCode,
                        Name = comTenHang.Text,
                        InputDate = NgayNhapHang.SelectedDate ?? DateTime.Now,
                        GoodsType = goodsType,
                        Count = Convert.ToInt32(textSL.Text),
                        Price = Convert.ToInt32(valueDonGia)
                    };
                    good.setPriceText();
                    listGoods.Add(good);
                }  

                comTenHang.Text = "";
                NgayNhapHang.SelectedDate = DateTime.Now;
                textDonGia.Text = "";
                textSL.Text = "";
                text_GoodsCode.SelectAll();
                text_GoodsCode.Focus();
                textDonGia.IsEnabled = true;
                MessageBox.Show("Thêm hàng thành công!");
                lvGoods.Items.Refresh();
        }
            sqlConn.Close();
        }

        private void comTenHang_DropDownClosed(object sender, EventArgs e)
        {
            //MessageBox.Show(comTenHang.Text);
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + comTenHang.Text + "'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                //if (!reader.IsDBNull(0)) TextTenHang.Content = reader.GetString(0);
            }
            reader.Close();
            sqlConn.Close();
        }

        private void comTenHang_TextInput(object sender, TextCompositionEventArgs e)
        {
            
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = '" + comTenHang.Text + "'", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                //if (!reader.IsDBNull(0)) TextTenHang.Content = reader.GetString(0);
            }
            reader.Close();
            sqlConn.Open();

        }

        private void textDonGia_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                textDonGia.MaxLength = 13;
                valueDonGia = textDonGia.Text;
                textDonGia.Text = string.Format("{0:#,##0}" + " VND", double.Parse(textDonGia.Text));
            }
            catch
            {
                textDonGia.MaxLength = 7;
                valueDonGia = "";
                textDonGia.Text = "";
            }
        }


        private void text_GoodsCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            textDonGia.IsEnabled = true;
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT TENHANG FROM LOAIHANG WHERE MAHANG = @MAHANG", sqlConn);
            sqlCommand.Parameters.AddWithValue("@MAHANG", text_GoodsCode.Text);
            var reader = sqlCommand.ExecuteReader();
            bool flag = false;
            while (reader.Read())
            {
                comTenHang.Text = reader.GetString(0).Trim();
                flag = true;
                break;
            }
            if (!flag)
            {
                MessageBox.Show("Mã hàng không tồn tại trong hệ thống!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            var g = listGoods.First(g1 => g1.Code.ToUpper() == text_GoodsCode.Text.ToUpper().Trim());
            if(g!= null)
            {
                textDonGia.Text=g.Price + "";
                textDonGia.IsEnabled = false;
            }
            text_GoodsCode.SelectAll();
            reader.Close();
            sqlConn.Close();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            sqlConn.Open();
            var sqlCommand = new MySqlCommand("SELECT A.ID, A.MAHANG, B.TENHANG, A.NGNHAP, C.Type_Name, A.SOLUONG, A.DONGIA " +
                "FROM NHAPHANG A, LOAIHANG B, Goods_Type C " +
                "where  A.MAHANG = B.MAHANG and B.TYPE_ID = C.TYPE_ID ", sqlConn);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                Goods good = new Goods()
                {
                    ID = reader.GetString(0),
                    Code = reader.GetString(1),
                    Name = reader.GetString(2),
                    InputDate = reader.GetDateTime(3).Date,
                    GoodsType = reader.GetString(4),
                    Count = reader.GetInt32(5),
                    Price = reader.GetInt32(6),
                   
            };
                good.setPriceText();
                listGoods.Add(good);
            }
            reader.Close();
            sqlConn.Close();
            lvGoods.ItemsSource = listGoods;
        }

        private void TroVe_Click(object sender, RoutedEventArgs e)
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Home;
        }
        private void tb_Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var listGoodsSearch = new ObservableCollection<Goods>(listGoods.Where(x => x.Name.ToUpper().Contains(tb_search.Text.ToUpper())).ToList());
            lvGoods.ItemsSource = listGoodsSearch;
            lvGoods.Items.Refresh();
        }
        private void ListViewItem_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            MessageBox.Show("PreviewGotKeyboardFocus");
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("MouseDoubleClick");
            ListViewItem button = sender as ListViewItem;
            Goods good = button.DataContext as Goods;
            updatedGoods = good;
            //MessageBox.Show("MouseDoubleClick");
            //textTenHang.Text = good.Name;
           // cbb_goodsType.Text = good.GoodsType;
            //comBoxDVT.Text = good.DVT;
            //grid_update.Visibility = Visibility.Visible;
        }

        private void comTenHang_LostFocus(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(comTenHang.Text);
            var goods = listGoods.FirstOrDefault(g => g.Name.ToUpper() == comTenHang.Text.Trim().ToUpper());
            if (goods != null)
            {
                textDonGia.Text = goods.Price.ToString();
                textDonGia.IsEnabled = false;
            }
            else
            {
                textDonGia.IsEnabled = true;
                textDonGia.Text = "";
            }
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection sqlConn = new MySqlConnection(ConfigurationManager.ConnectionStrings["connectionStringMySQL"].ConnectionString);
            Button button = sender as Button;
            Goods good = button.DataContext as Goods;
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa " + good.Name + " ?", "My App", MessageBoxButton.YesNo);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    {
                        //delete records in the database
                        sqlConn.Open();
                        var sqlCommand = new MySqlCommand("delete from NHAPHANG where ID = @ID", sqlConn);
                        sqlCommand.Parameters.AddWithValue("@ID", good.ID);
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
    }
}
