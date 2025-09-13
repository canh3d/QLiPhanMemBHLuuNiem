using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;

namespace QLPMBanHangLuuNiem
{
    public partial class HDBan : Form
    {
        public HDBan()
        {
            InitializeComponent();
        }
        // ket noi csdl
        string Nguon = "Data Source = ASUSCANH; Initial Catalog = QLBHLUUNIEM; Integrated Security = True; TrustServerCertificate=False";
        string Lenh = @"";
        SqlConnection KetNoi;
        SqlCommand ThucHien;
        SqlDataReader Doc;

        private void HDBan_Load(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT MaNV, TenNV FROM NhanVien";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            SqlDataAdapter da = new SqlDataAdapter(ThucHien);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cboMaNV.DataSource = dt;
            cboMaNV.DisplayMember = "TenNV";
            cboMaNV.ValueMember = "MaNV";
            KetNoi.Close();
            // load combobox khach hang
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT MaKH, TenKH FROM KhachHang";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            SqlDataAdapter da1 = new SqlDataAdapter(ThucHien);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            cboMaKH.DataSource = dt1;
            cboMaKH.DisplayMember = "TenKH";
            cboMaKH.ValueMember = "MaKH";
            KetNoi.Close();
            // load combobox hang hoa
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT MaHang, TenHang FROM HangHoa";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            SqlDataAdapter da2 = new SqlDataAdapter(ThucHien);
            DataTable dt2 = new DataTable();
            da2.Fill(dt2);
            cboMaHang.DataSource = dt2;
            cboMaHang.DisplayMember = "TenHang";
            cboMaHang.ValueMember = "MaHang";
            KetNoi.Close();

            // Hiển thị dữ liệu lên DataGridView khi load form
            HienThi();
        }

        private void cboMaKH_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            var selectedValue = cboMaKH.SelectedValue;
            if (selectedValue == null)
            {
                txtTenKH.Text = "";
                txtDiaChi.Text = "";
                txtDienThoai.Text = "";
                return;
            }
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT TenKH, DiaChi, DienThoai FROM KhachHang WHERE MaKH = @MaKH";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            if (selectedValue is DataRowView drv)
                selectedValue = drv["MaKH"];
            ThucHien.Parameters.AddWithValue("@MaKH", selectedValue);
            KetNoi.Open();
            Doc = ThucHien.ExecuteReader();
            if (Doc.Read())
            {
                txtTenKH.Text = Doc["TenKH"].ToString();
                txtDiaChi.Text = Doc["DiaChi"].ToString();
                txtDienThoai.Text = Doc["DienThoai"].ToString();
            }
            KetNoi.Close();
        }

        private void cboMaHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = cboMaHang.SelectedValue;
            if (selectedValue == null)
            {
                txtTenHang.Text = "";
                txtDonGia.Text = "";
                txtSoLuong.Text = "";
                return;
            }
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT TenHang, DonGiaBan, SoLuong FROM HangHoa WHERE MaHang = @MaHang";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            if (selectedValue is DataRowView drv)
                selectedValue = drv["MaHang"];
            ThucHien.Parameters.AddWithValue("@MaHang", selectedValue);
            KetNoi.Open();
            Doc = ThucHien.ExecuteReader();
            if (Doc.Read())
            {
                txtTenHang.Text = Doc["TenHang"].ToString();
                txtDonGia.Text = Doc["DonGiaBan"].ToString();
                txtSoLuong.Text = Doc["SoLuong"].ToString();
            }
            KetNoi.Close();
        }

        private void cboMaNV_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            var selectedValue = cboMaNV.SelectedValue;
            if (selectedValue == null)
            {
                txtTenNV.Text = "";
                return;
            }
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT TenNV FROM NhanVien WHERE MaNV = @MaNV";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            if (selectedValue is DataRowView drv)
                selectedValue = drv["MaNV"];
            ThucHien.Parameters.AddWithValue("@MaNV", selectedValue);
            KetNoi.Open();
            Doc = ThucHien.ExecuteReader();
            if (Doc.Read())
            {
                txtTenNV.Text = Doc["TenNV"].ToString();
            }
            KetNoi.Close();
        }

        private void txtGiamGia_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal donGia = decimal.Parse(txtDonGia.Text);
                int soLuong = int.Parse(txtSoLuong.Text);
                decimal giamGia = decimal.Parse(txtGiamGia.Text);

                if (giamGia < 0 || giamGia > 100)
                {
                    MessageBox.Show("Giảm giá phải nằm trong khoảng từ 0 đến 100%", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtGiamGia.Focus();
                    return;
                }

                decimal thanhTien = donGia * soLuong * (1 - giamGia / 100);
                txtThanhTien.Text = thanhTien.ToString("N0"); // Hiển thị không có số lẻ
            }
            catch (FormatException)
            {
                MessageBox.Show("Vui lòng nhập số hợp lệ cho Đơn Giá, Số Lượng và Giảm Giá.", "Lỗi nhập liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtGiamGia.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvHDBan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT * FROM HDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            SqlDataAdapter da = new SqlDataAdapter(ThucHien);// 
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (e.RowIndex >= 0 && e.RowIndex < dt.Rows.Count)
            {
                DataRow row = dt.Rows[e.RowIndex];
                txtMaHDBan.Text = row["MaHDBan"].ToString();
                dtpNgayBan.Value = Convert.ToDateTime(row["NgayBan"]);
                cboMaNV.SelectedValue = row["MaNV"].ToString();
                cboMaKH.SelectedValue = row["MaKH"].ToString();
                txtTenKH.Text = row["TenKH"].ToString();
                txtDiaChi.Text = row["DiaChi"].ToString();
                txtDienThoai.Text = row["DienThoai"].ToString();
                cboMaHang.SelectedValue = row["MaHang"].ToString();
                txtTenHang.Text = row["TenHang"].ToString();
                txtDonGia.Text = row["DonGia"].ToString();
                txtSoLuong.Text = row["SoLuong"].ToString();
                txtGiamGia.Text = row["GiamGia"].ToString();
                txtThanhTien.Text = row["ThanhTien"].ToString();
            }
            KetNoi.Close();


        }
        // hàm hiển thị dữ liệu lên datagridview
        void HienThi()
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT * FROM HDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            SqlDataAdapter da = new SqlDataAdapter(ThucHien);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvHDBan.DataSource = dt;
            dgvHDBan.Columns["MaHDBan"].HeaderText = "Mã HĐ";
            dgvHDBan.Columns["NgayBan"].HeaderText = "Ngày Bán";
            dgvHDBan.Columns["MaNV"].HeaderText = "Mã NV";
            dgvHDBan.Columns["MaKH"].HeaderText = "Mã KH";
            dgvHDBan.Columns["TenKH"].HeaderText = "Tên KH";
            dgvHDBan.Columns["DiaChi"].HeaderText = "Địa Chỉ";
            dgvHDBan.Columns["DienThoai"].HeaderText = "SDT";
            dgvHDBan.Columns["MaHang"].HeaderText = "Mã Hàng";
            dgvHDBan.Columns["TenHang"].HeaderText = "Tên Hàng";
            dgvHDBan.Columns["DonGia"].HeaderText = "Giá";
            dgvHDBan.Columns["SoLuong"].HeaderText = "SL";
            dgvHDBan.Columns["GiamGia"].HeaderText = "Giảm Giá(%)";
            dgvHDBan.Columns["ThanhTien"].HeaderText = "Thành Tiền";
            // Định dạng cột số tiền
            dgvHDBan.Columns["DonGia"].DefaultCellStyle.Format = "N0"; // Định dạng không có số lẻ
            dgvHDBan.Columns["ThanhTien"].DefaultCellStyle.Format = "N0"; 
            // Căn chỉnh lại kích thước cột
            dgvHDBan.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }


        private void btnThemHD_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "INSERT INTO HDBan (MaHDBan, NgayBan, MaNV, MaKH, TenKH, DiaChi, DienThoai, MaHang, TenHang, DonGia, SoLuong, GiamGia, ThanhTien) VALUES (@MaHDBan, @NgayBan, @MaNV, @MaKH, @TenKH, @DiaChi, @DienThoai, @MaHang, @TenHang, @DonGia, @SoLuong, @GiamGia, @ThanhTien)";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHDBan", txtMaHDBan.Text);
            ThucHien.Parameters.AddWithValue("@NgayBan", dtpNgayBan.Value);
            ThucHien.Parameters.AddWithValue("@MaNV", cboMaNV.SelectedValue);
            ThucHien.Parameters.AddWithValue("@MaKH", cboMaKH.SelectedValue);
            ThucHien.Parameters.AddWithValue("@TenKH", txtTenKH.Text);
            ThucHien.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
            ThucHien.Parameters.AddWithValue("@DienThoai", txtDienThoai.Text);
            ThucHien.Parameters.AddWithValue("@MaHang", cboMaHang.SelectedValue);
            ThucHien.Parameters.AddWithValue("@TenHang", txtTenHang.Text);
            ThucHien.Parameters.AddWithValue("@DonGia", decimal.Parse(txtDonGia.Text));
            ThucHien.Parameters.AddWithValue("@SoLuong", int.Parse(txtSoLuong.Text));
            ThucHien.Parameters.AddWithValue("@GiamGia", decimal.Parse(txtGiamGia.Text));
            ThucHien.Parameters.AddWithValue("@ThanhTien", decimal.Parse(txtThanhTien.Text));
            KetNoi.Open();
            try
            {
                ThucHien.ExecuteNonQuery();
                MessageBox.Show("Thêm hóa đơn bán thành công!");
                HienThi();
                dgvHDBan.ClearSelection(); // Bo ̉ chọn tất cả các dòng
                if (dgvHDBan.Rows.Count > 0)
                {
                    dgvHDBan.Rows[dgvHDBan.Rows.Count - 1].Selected = true; // Chọn dòng vừa thêm
                    dgvHDBan.FirstDisplayedScrollingRowIndex = dgvHDBan.Rows.Count - 1; // Cuộn tới dòng vừa thêm
                }
            }
            catch (SqlException ex) when (ex.Number == 2627) // Mã lỗi SQL Server cho vi phạm khóa chính
            {
                MessageBox.Show("Mã hóa đơn bán đã tồn tại. Vui lòng sử dụng mã khác.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
            finally
            {
                KetNoi.Close();
            }

        }
        // thuc hien cnang sau khi an btnLuu thi thuc hien duoc cnang tong tien


        private void btnLuu_Click_1(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT SUM(ThanhTien) FROM HDBan WHERE MaHDBan = @MaHDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHDBan", txtMaHDBan.Text);
            KetNoi.Open();
            var result = ThucHien.ExecuteScalar();
            if (result != DBNull.Value)
            {
                decimal tongTien = Convert.ToDecimal(result);
                MessageBox.Show("Tổng tiền của hóa đơn " + txtMaHDBan.Text + ": " + tongTien.ToString("N0") + " VND", "Tổng Tiền", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không có hóa đơn nào với mã này.", "Tổng Tiền", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            KetNoi.Close();
            // Hiển thị tổng tiền lên txtTongTien
            txtTongTien.Text = result != DBNull.Value ? Convert.ToDecimal(result).ToString("N0") : "0";
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "DELETE FROM HDBan WHERE MaHDBan = @MaHDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHDBan", txtMaHDBan.Text);
            KetNoi.Open();
            try
            {
                int rowsAffected = ThucHien.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Xóa hóa đơn thành công!");
                    HienThi(); // Cập nhật lại dữ liệu trên DataGridView
                }
                else
                {
                    MessageBox.Show("Không tìm thấy hóa đơn để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
            finally
            {
                KetNoi.Close();
            }
        }
        // btn huy hoa don thuc hien xoa toan bo thong tin tren form


        private void btnHuy_Click_1(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(
        "Bạn có chắc chắn muốn hủy hóa đơn này không?",
        "Xác nhận hủy hóa đơn",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (confirmResult != DialogResult.Yes)
                return;

            KetNoi = new SqlConnection(Nguon);
            Lenh = "DELETE FROM HDBan WHERE MaHDBan = @MaHDBan"; // Xóa theo mã hóa đơn
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHDBan", txtMaHDBan.Text);
            KetNoi.Open();
            try
            {
                int rowsAffected = ThucHien.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Đã hủy hóa đơn có mã " + txtMaHDBan.Text + "!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy hóa đơn với mã này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                HienThi();
                txtMaHDBan.Clear();
                dtpNgayBan.Value = DateTime.Now;
                cboMaNV.SelectedIndex = -1;
                cboMaKH.SelectedIndex = -1;
                txtTenKH.Clear();
                txtDiaChi.Clear();
                txtDienThoai.Clear();
                cboMaHang.SelectedIndex = -1;
                txtTenHang.Clear();
                txtDonGia.Clear();
                txtSoLuong.Clear();
                txtGiamGia.Clear();
                txtThanhTien.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
            finally
            {
                KetNoi.Close();
            }
        }
        // btn thoat
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT * FROM HDBan WHERE MaHDBan LIKE @MaHDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHDBan", "%" + txtTimKiem.Text + "%");
            SqlDataAdapter da = new SqlDataAdapter(ThucHien);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvHDBan.DataSource = dt;
            KetNoi.Close();
        }




        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
           KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT * FROM HDBan WHERE MaHDBan = @MaHDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHDBan", txtMaHDBan.Text);
            SqlDataAdapter da = new SqlDataAdapter(ThucHien);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count == 0) // Không tìm thấy hóa đơn
            {
                MessageBox.Show("Không tìm thấy hóa đơn với mã này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Excel.Application excelApp = new Excel.Application();// Tạo một ứng dụng Excel mới
            excelApp.Visible = true;// Hiển thị ứng dụng Excel
            Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);// Tạo một workbook mới
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];// Lấy sheet đầu tiên
            worksheet.Name = "Hóa Đơn Bán";// Đặt tên cho sheet
            // Tiêu đề
            Excel.Range titleRange = worksheet.Range["A1", "M1"];// Kéo dài từ cột A đến cột M
            titleRange.Merge();// Gộp các ô
            titleRange.Value = "HÓA ĐƠN BÁN HÀNG";// Thiết lập tiêu đề
            titleRange.Font.Size = 16;
            titleRange.Font.Bold = true;
            titleRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;// Căn giữa
            // Thông tin hóa đơn
            worksheet.Cells[3, 1] = "Mã HĐ:";
            worksheet.Cells[3, 2] = dt.Rows[0]["MaHDBan"].ToString();
            worksheet.Cells[4, 1] = "Ngày Bán:";
            worksheet.Cells[4, 2] = Convert.ToDateTime(dt.Rows[0]["NgayBan"]).ToString("dd/MM/yyyy");
            worksheet.Cells[5, 1] = "Nhân Viên:";
            worksheet.Cells[5, 2] = cboMaNV.Text;
            worksheet.Cells[6, 1] = "Khách Hàng:";
            worksheet.Cells[6, 2] = dt.Rows[0]["TenKH"].ToString();
            worksheet.Cells[7, 1] = "Địa Chỉ:";
            worksheet.Cells[7, 2] = dt.Rows[0]["DiaChi"].ToString();
            worksheet.Cells[8, 1] = "Điện Thoại:";
            worksheet.Cells[8, 2] = dt.Rows[0]["DienThoai"].ToString();
            // Tiêu đề cột
            worksheet.Cells[10, 1] = "Mã Hàng";
            worksheet.Cells[10, 2] = "Tên Hàng";
            worksheet.Cells[10, 3] = "Đơn Giá";
            worksheet.Cells[10, 4] = "Số Lượng";
            worksheet.Cells[10, 5] = "Giảm Giá(%)";
            worksheet.Cells[10, 6] = "Thành Tiền";
            Excel.Range headerRange = worksheet.Range["A10", "F10"];
            headerRange.Font.Bold = true;
            headerRange.Interior.Color = Color.LightGray;
            // Dữ liệu hóa đơn
            int rowIndex = 11;
            foreach (DataRow row in dt.Rows)
            {
                worksheet.Cells[rowIndex, 1] = row["MaHang"].ToString();
                worksheet.Cells[rowIndex, 2] = row["TenHang"].ToString();
                worksheet.Cells[rowIndex, 3] = Convert.ToDecimal(row["DonGia"]).ToString("N0");
                worksheet.Cells[rowIndex, 4] = Convert.ToInt32(row["SoLuong"]);
                worksheet.Cells[rowIndex, 5] = Convert.ToDecimal(row["GiamGia"]).ToString("N0");
                worksheet.Cells[rowIndex, 6] = Convert.ToDecimal(row["ThanhTien"]).ToString("N0");
                rowIndex++;
            }
            // Tổng tiền
            worksheet.Cells[rowIndex, 5] = "Tổng Tiền:";
            worksheet.Cells[rowIndex, 6] = txtTongTien.Text + " VND";
            Excel.Range totalRange = worksheet.Range[worksheet.Cells[rowIndex, 5], worksheet.Cells[rowIndex, 6]];
            totalRange.Font.Bold = true;
            // Căn chỉnh lại kích thước cột
            worksheet.Columns.AutoFit();


        }
        

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT * FROM HDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            SqlDataAdapter da = new SqlDataAdapter(ThucHien);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvHDBan.DataSource = dt;
            txtTimKiem.Clear();
            KetNoi.Close();
        }
        
    }
}