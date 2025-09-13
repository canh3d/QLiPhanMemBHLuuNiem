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
    public partial class HangHoa : Form
    {
        public HangHoa()
        {
            InitializeComponent();
        }
        // ket noi csdl
        string Nguon = "Data Source = ASUSCANH; Initial Catalog = QLBHLUUNIEM; Integrated Security = True; TrustServerCertificate=False";
        string Lenh = @"";
        SqlConnection KetNoi;
        SqlCommand ThucHien;
        SqlDataReader Doc;

        // Code load combobox
        private void cboMaChatLieu_SelectedIndexChanged(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT MaChatLieu, TenChatLieu FROM ChatLieu";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaChatLieu", cboMaChatLieu.SelectedValue);
            KetNoi.Open();

        }

        private void HangHoa_Load(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT MaChatLieu, TenChatLieu FROM ChatLieu";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            SqlDataAdapter da = new SqlDataAdapter(ThucHien);
            DataTable dt = new DataTable();
            da.Fill(dt);
            cboMaChatLieu.DataSource = dt;
            cboMaChatLieu.DisplayMember = "TenChatLieu";
            cboMaChatLieu.ValueMember = "MaChatLieu";
            KetNoi.Close();
            HienThi();
        }
        // btn them
        private void btnThem_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            // Kiểm tra mã hàng đã tồn tại chưa
            string checkQuery = "SELECT COUNT(*) FROM HangHoa WHERE MaHang = @MaHang";
            SqlCommand checkCmd = new SqlCommand(checkQuery, KetNoi);
            checkCmd.Parameters.AddWithValue("@MaHang", txtMaHang.Text);
            KetNoi.Open();
            int count = (int)checkCmd.ExecuteScalar();
            KetNoi.Close();

            if (count > 0)
            {
                MessageBox.Show("Mã hàng đã tồn tại, vui lòng nhập mã khác!");
                return;
            }

            Lenh = "INSERT INTO HangHoa (MaHang, TenHang, MaChatLieu, SoLuong, DonGiaNhap, DonGiaBan, GhiChu) VALUES (@MaHang, @TenHang, @MaChatLieu, @SoLuong, @DonGiaNhap, @DonGiaBan, @GhiChu)";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHang", txtMaHang.Text);
            ThucHien.Parameters.AddWithValue("@TenHang", txtTenHang.Text);
            ThucHien.Parameters.AddWithValue("@MaChatLieu", cboMaChatLieu.SelectedValue);
            ThucHien.Parameters.AddWithValue("@SoLuong", txtSoLuong.Text);
            ThucHien.Parameters.AddWithValue("@DonGiaNhap", txtGiaNhap.Text);
            ThucHien.Parameters.AddWithValue("@DonGiaBan", txtGiaBan.Text);
            ThucHien.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            KetNoi.Close();
            MessageBox.Show("Thêm hàng hóa thành công!");
            HienThi();

        }
        void HienThi()
        {
            KetNoi = new SqlConnection(Nguon);
            dataGridView.Rows.Clear();
            Lenh = @"SELECT MaHang, TenHang, MaChatLieu, SoLuong, DonGiaNhap, DonGiaBan, GhiChu FROM HangHoa";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            Doc = ThucHien.ExecuteReader();
            while (Doc.Read())
            {
                dataGridView.Rows.Add(Doc[0].ToString(), Doc[1].ToString(), Doc[2].ToString(), Doc[3].ToString(), Doc[4].ToString(), Doc[5].ToString(), Doc[6].ToString());
            }
            KetNoi.Close();

        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "UPDATE HangHoa SET TenHang = @TenHang, MaChatLieu = @MaChatLieu, SoLuong = @SoLuong, DonGiaNhap = @DonGiaNhap, DonGiaBan = @DonGiaBan, GhiChu = @GhiChu WHERE MaHang = @MaHang";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHang", txtMaHang.Text);
            ThucHien.Parameters.AddWithValue("@TenHang", txtTenHang.Text);
            ThucHien.Parameters.AddWithValue("@MaChatLieu", cboMaChatLieu.SelectedValue);
            ThucHien.Parameters.AddWithValue("@SoLuong", txtSoLuong.Text);
            ThucHien.Parameters.AddWithValue("@DonGiaNhap", txtGiaNhap.Text);
            ThucHien.Parameters.AddWithValue("@DonGiaBan", txtGiaBan.Text);
            ThucHien.Parameters.AddWithValue("@GhiChu", txtGhiChu.Text);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            KetNoi.Close();
            MessageBox.Show("Sửa hàng hóa thành công!");
            HienThi();

        }
       
        
        private void btnXoa_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = "DELETE FROM HangHoa WHERE MaHang = @MaHang";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaHang", txtMaHang.Text);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            KetNoi.Close();
            MessageBox.Show("Xóa hàng hóa thành công!");
            HienThi();

        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMaHang.Text = dataGridView.CurrentRow.Cells[0].Value.ToString();
            txtTenHang.Text = dataGridView.CurrentRow.Cells[1].Value.ToString();
            cboMaChatLieu.SelectedValue = dataGridView.CurrentRow.Cells[2].Value.ToString();
            txtSoLuong.Text = dataGridView.CurrentRow.Cells[3].Value.ToString();
            txtGiaNhap.Text = dataGridView.CurrentRow.Cells[4].Value.ToString();
            txtGiaBan.Text = dataGridView.CurrentRow.Cells[5].Value.ToString();
            txtGhiChu.Text = dataGridView.CurrentRow.Cells[6].Value.ToString();

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "")
            {
                HienThi();
            }
            else
            {
                KetNoi = new SqlConnection(Nguon);
                dataGridView.Rows.Clear();
                Lenh = @"SELECT MaHang, TenHang, MaChatLieu, SoLuong, DonGiaNhap, DonGiaBan, GhiChu FROM HangHoa WHERE MaHang LIKE @TimKiem OR TenHang LIKE @TimKiem";
                ThucHien = new SqlCommand(Lenh, KetNoi);
                ThucHien.Parameters.AddWithValue("@TimKiem", "%" + txtTimKiem.Text + "%");
                KetNoi.Open();
                ThucHien.ExecuteNonQuery();
                Doc = ThucHien.ExecuteReader();
                while (Doc.Read())
                {
                    dataGridView.Rows.Add(Doc[0].ToString(), Doc[1].ToString(), Doc[2].ToString(), Doc[3].ToString(), Doc[4].ToString(), Doc[5].ToString(), Doc[6].ToString());
                }
                KetNoi.Close();
            }
        }

        private void txtSoLuong_TextChanged(object sender, EventArgs e)
        {
            using (var ketNoi = new SqlConnection(Nguon))
            using (var thucHien = new SqlCommand("SELECT SoLuong FROM HangHoa WHERE MaHang = @MaHang", ketNoi))
            {
                thucHien.Parameters.AddWithValue("@MaHang", txtMaHang.Text);
                ketNoi.Open();
                using (var doc = thucHien.ExecuteReader())
                {
                    if (doc.Read())
                    {
                        int soLuongHienTai = Convert.ToInt32(doc["SoLuong"]);
                        int soLuongNhap;
                        if (int.TryParse(txtSoLuong.Text, out soLuongNhap))
                        {
                            int tongSoLuong = soLuongHienTai + soLuongNhap;
                            // Không nên gán lại txtSoLuong.Text ở đây!
                            // txtSoLuong.Text = tongSoLuong.ToString();
                        }
                    }
                }
            }
        }
        // btn xoa hang hoa 
        private void btnXoaHangHoa_Click(object sender, EventArgs e)
        {
            
        }
    }
}
