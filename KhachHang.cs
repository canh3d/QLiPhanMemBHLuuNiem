using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLPMBanHangLuuNiem
{
    public partial class KhachHang : Form
    {
        public KhachHang()
        {
            InitializeComponent();
        }
        // ket noi CSDL
        string Nguon = "Data Source = ASUSCANH; Initial Catalog = QLBHLUUNIEM; Integrated Security = True; TrustServerCertificate=False";
        string Lenh = @"";
        SqlConnection KetNoi;
        SqlCommand ThucHien;
        SqlDataReader Doc;

        void HienThi()
        {
            dataGridView1.Rows.Clear();
            Lenh = @"SELECT    MaKH, TenKH, GioiTinh, NgaySinh, DiaChi, DienThoai
FROM         KhachHang";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            Doc = ThucHien.ExecuteReader();
            while (Doc.Read())
            {
                dataGridView1.Rows.Add(Doc[0].ToString(), Doc[1].ToString(), Doc[2].ToString(), Convert.ToDateTime(Doc[3]).ToString("dd/MM/yyyy"), Doc[4].ToString(), Doc[5].ToString());
            }
            KetNoi.Close();
        }

        private void KhachHang_Load(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            HienThi();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            {
                MessageBox.Show("Mã khách hàng không được để trống!");
                return;
            }
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT COUNT(*) FROM KhachHang WHERE MaKH = @MaKH";// kiem tra ma nhan vien da ton tai chua
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaKH", txtMaKH.Text);
            KetNoi.Open();
            int count = (int)ThucHien.ExecuteScalar();
            KetNoi.Close();
            if (count > 0)
            {
                MessageBox.Show("Mã khách hàng đã tồn tại!");
                return;
            }
            string gioiTinh = checkBoxGTNam.Checked ? "Nam" : (checkBoxGTNu.Checked ? "Nữ" : "");
            if (string.IsNullOrEmpty(gioiTinh))
            {
                MessageBox.Show("Vui lòng chọn giới tính!");
                return;
            }
            Lenh = @"INSERT INTO KhachHang (MaKH, TenKH, GioiTinh, NgaySinh, DiaChi, DienThoai) 
         VALUES (@MaKH, @TenKH, @GioiTinh, @NgaySinh, @DiaChi, @DienThoai)";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaKH", txtMaKH.Text);
            ThucHien.Parameters.AddWithValue("@TenKH", txtTenKH.Text);
            ThucHien.Parameters.AddWithValue("@GioiTinh", gioiTinh);
            ThucHien.Parameters.AddWithValue("@NgaySinh", dateTimePickerNgaySinh.Value); // Consider parsing to DateTime
            ThucHien.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
            ThucHien.Parameters.AddWithValue("@DienThoai", txtDienThoai.Text);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            KetNoi.Close();
            HienThi();
        }

        private void btnSua_Click_1(object sender, EventArgs e)
        {
            string gioiTinh = checkBoxGTNam.Checked ? "Nam" : (checkBoxGTNu.Checked ? "Nữ" : "");
            if (string.IsNullOrEmpty(gioiTinh))
            {
                MessageBox.Show("Vui lòng chọn giới tính!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            {
                MessageBox.Show("Vui lòng chọn khách hàng cần sửa!");
                return;
            }

            // Loại bỏ khoảng trắng thừa
            string maKH = txtMaKH.Text.Trim();

            Lenh = @"UPDATE KhachHang
         SET TenKH = @TenKH, GioiTinh = @GioiTinh, NgaySinh = @NgaySinh, DiaChi = @DiaChi, DienThoai = @DienThoai
         WHERE MaKH = @MaKH";
            KetNoi = new SqlConnection(Nguon);
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaKH", maKH);
            ThucHien.Parameters.AddWithValue("@TenKH", txtTenKH.Text);
            ThucHien.Parameters.AddWithValue("@GioiTinh", gioiTinh);
            ThucHien.Parameters.AddWithValue("@NgaySinh", dateTimePickerNgaySinh.Value);
            ThucHien.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
            ThucHien.Parameters.AddWithValue("@DienThoai", txtDienThoai.Text);

            try
            {
                KetNoi.Open();
                int rows = ThucHien.ExecuteNonQuery();
                if (rows > 0)
                    MessageBox.Show("Sửa thành công!");
                else
                    MessageBox.Show("Không tìm thấy khách hàng để sửa!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
            }
            finally
            {
                KetNoi.Close();
            }
            // Cho phép nhập lại mã nhân viên khi thêm mới
            txtMaKH.ReadOnly = false;
            HienThi();
        }

        private void btnXoa_Click_1(object sender, EventArgs e)
        {
            DialogResult D = MessageBox.Show("Bạn có chắc chắn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (D == DialogResult.Yes)
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMaKH.Text)) //nếu chưa chọn bản ghi nào
                {
                    MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Lấy mã khach hang từ dòng đang chọn trên DataGridView
                string maKH = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                KetNoi = new SqlConnection(Nguon);
                Lenh = @"DELETE FROM KhachHang WHERE MaKH = @MaKH";
                ThucHien = new SqlCommand(Lenh, KetNoi);
                ThucHien.Parameters.AddWithValue("@MaKH", maKH);
                try
                {
                    KetNoi.Open();
                    int rows = ThucHien.ExecuteNonQuery();
                    if (rows > 0)
                        MessageBox.Show("Xóa thành công!");
                    else
                        MessageBox.Show("Không tìm thấy khách hàng để xóa!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                }
                finally
                {
                    KetNoi.Close();
                }

                HienThi();
            }
        }

        private void btnThoat_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            txtDiaChi.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            txtMaKH.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtTenKH.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();

            // Khóa không cho sửa mã nhân viên khi sửa
            txtMaKH.ReadOnly = true;

            // Xử lý ngày sinh an toàn
            var dateString = dataGridView1.CurrentRow.Cells[3].Value?.ToString();
            DateTime ngaySinh;
            if (!string.IsNullOrWhiteSpace(dateString) &&
                DateTime.TryParseExact(dateString, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ngaySinh))
            {
                dateTimePickerNgaySinh.Value = ngaySinh;
            }
            else
            {
                dateTimePickerNgaySinh.Value = DateTime.Now;
            }

            txtDienThoai.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            string gioiTinh = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            if (gioiTinh == "Nam")
            {
                checkBoxGTNam.Checked = true;
                checkBoxGTNu.Checked = false;
            }
            else if (gioiTinh == "Nữ")
            {
                checkBoxGTNam.Checked = false;
                checkBoxGTNu.Checked = true;
            }
            else
            {
                checkBoxGTNam.Checked = false;
                checkBoxGTNu.Checked = false;
            }
        }
    }
}
