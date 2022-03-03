using SalesManager.Pages;
using System;
using System.Diagnostics;
using System.Globalization;

namespace SalesManager
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public class ApplicationPageValueConverter : BaseValueConverter<ApplicationPageValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Find the appropriate page
            switch ((ApplicationPage)value)
            {
                case ApplicationPage.Home:
                    return new Home();
                case ApplicationPage.NhapHangMoi:
                    return new NhapHangMoi();
                case ApplicationPage.NhapLoaiHangMoi:
                    return new NhapLoaiHangMoi();
                case ApplicationPage.DangNhap:
                    return new DangNhap();
                case ApplicationPage.DangKy:
                    return new DangKy();
                case ApplicationPage.QuenMatKhau:
                    return new QuenMatKhau();
                case ApplicationPage.ThongKeDoanhThu:
                    return new ThongKeDoanhThu();
                case ApplicationPage.DanhSachNhanVien:
                    return new DanhSachNhanVien();
                case ApplicationPage.ThemNhanVien:
                    return new ThemNhanVien();
                case ApplicationPage.ThongTinNhanVien:
                    return new ThongTinNhanVien();
                case ApplicationPage.TaoMaNhanVien:
                    return new TaoMaNhanVien();
                case ApplicationPage.ThongTinChinhSuaDanhSach:
                    return new ThongTinChinhSuaDanhSach();
                case ApplicationPage.SideMenuControl:
                    return new SideMenuControl();
                case ApplicationPage.TaoHoaDon:
                    return new TaoHoaDon();
                case ApplicationPage.VisualOff:
                    return new VisualOff();
                case ApplicationPage.XuatHoaDon:
                    return new XuatHoaDon();
                case ApplicationPage.NotificationControl:
                    return new NotificationControl();
                case ApplicationPage.ThongKeDoanhThuThang:
                    return new ThongKeDoanhThuThang();
                case ApplicationPage.ThongKeSoLuongHang:
                    return new ThongKeSoLuongHang();
                case ApplicationPage.SuaNhanVien:
                    return new SuaNhanVien();
                case ApplicationPage.DoanhThuNhanVien:
                    return new DoanhThuNhanVien();
                case ApplicationPage.ThongTinCuaHang:
                    return new ThongTinCuaHang();
                case ApplicationPage.HangDaBan:
                    return new HangDaBan();
                case ApplicationPage.ChonThang:
                    return new ChonThang();
                case ApplicationPage.CaiDatThongBao:
                    return new CaiDatThongBao();
                case ApplicationPage.SuaHoaDon:
                    return new SuaHoaDon();
                case ApplicationPage.ThongKeLaiSuat:
                    return new ThongKeLaiSuat();
                case ApplicationPage.CTHD:
                    return new CTHD();
                case ApplicationPage.BarcodeScanner:
                    return new BarcodeScanner();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}