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


namespace QLPMBanHangLuuNiem
{
    public partial class NhanVien : Form
    {
        public NhanVien()
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
            Lenh = @"SELECT    MaNV, TenNV, GioiTinh, NgaySinh, DiaChi, DienThoai
FROM         NhanVien";
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

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Mã nhân viên không được để trống!");
                return;
            }
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT COUNT(*) FROM NhanVien WHERE MaNV = @MaNV";// kiem tra ma nhan vien da ton tai chua
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
            KetNoi.Open();
            int count = (int)ThucHien.ExecuteScalar();
            KetNoi.Close();
            if (count > 0)
            {
                MessageBox.Show("Mã nhân viên đã tồn tại!");
                return;
            }
            string gioiTinh = checkBoxGTNam.Checked ? "Nam" : (checkBoxGTNu.Checked ? "Nữ" : "");
            if (string.IsNullOrEmpty(gioiTinh))
            {
                MessageBox.Show("Vui lòng chọn giới tính!");
                return;
            }

         

            Lenh = @"INSERT INTO NhanVien (MaNV, TenNV, GioiTinh, NgaySinh, DiaChi, DienThoai) 
         VALUES (@MaNV, @TenNV, @GioiTinh, @NgaySinh, @DiaChi, @DienThoai)";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaNV", txtMaNV.Text);
            ThucHien.Parameters.AddWithValue("@TenNV", txtTenNV.Text);
            ThucHien.Parameters.AddWithValue("@GioiTinh", gioiTinh);
            ThucHien.Parameters.AddWithValue("@NgaySinh", dateTimePickerNgaySinh.Value); // Consider parsing to DateTime
            ThucHien.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
            ThucHien.Parameters.AddWithValue("@DienThoai", txtDienThoai.Text);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            KetNoi.Close();
            HienThi();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtDiaChi.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            txtMaNV.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtTenNV.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();

            // Khóa không cho sửa mã nhân viên khi sửa
            txtMaNV.ReadOnly = true;

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

        private void btnSua_Click(object sender, EventArgs e)
        {
            string gioiTinh = checkBoxGTNam.Checked ? "Nam" : (checkBoxGTNu.Checked ? "Nữ" : "");
            if (string.IsNullOrEmpty(gioiTinh))
            {
                MessageBox.Show("Vui lòng chọn giới tính!");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMaNV.Text))
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần sửa!");
                return;
            }

            // Loại bỏ khoảng trắng thừa
            string maNV = txtMaNV.Text.Trim();

            Lenh = @"UPDATE NhanVien 
         SET TenNV = @TenNV, GioiTinh = @GioiTinh, NgaySinh = @NgaySinh, DiaChi = @DiaChi, DienThoai = @DienThoai
         WHERE MaNV = @MaNV";
            KetNoi = new SqlConnection(Nguon);
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaNV", maNV);
            ThucHien.Parameters.AddWithValue("@TenNV", txtTenNV.Text);
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
                    MessageBox.Show("Không tìm thấy nhân viên để sửa!");
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
            txtMaNV.ReadOnly = false;
            HienThi();
        }

        private void NhanVien_Load_1(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            HienThi();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            DialogResult D = MessageBox.Show("Bạn có chắc chắn xóa không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (D == DialogResult.Yes)
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtMaNV.Text)) //nếu chưa chọn bản ghi nào
                {
                    MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Lấy mã nhân viên từ dòng đang chọn trên DataGridView
                string maNV = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                KetNoi = new SqlConnection(Nguon);
                Lenh = @"DELETE FROM NhanVien WHERE MaNV = @MaNV";
                ThucHien = new SqlCommand(Lenh, KetNoi);
                ThucHien.Parameters.AddWithValue("@MaNV", maNV);
                try
                {
                    KetNoi.Open();
                    int rows = ThucHien.ExecuteNonQuery();
                    if (rows > 0)
                        MessageBox.Show("Xóa thành công!");
                    else
                        MessageBox.Show("Không tìm thấy nhân viên để xóa!");
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

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
