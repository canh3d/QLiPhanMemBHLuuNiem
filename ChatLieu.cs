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
    public partial class ChatLieu : Form
    {
        public ChatLieu()
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
            Lenh = @"SELECT    MaChatLieu, TenChatLieu
                    FROM         ChatLieu";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            Doc = ThucHien.ExecuteReader();
            while (Doc.Read())
            {
                dataGridView1.Rows.Add(Doc[0].ToString(), Doc[1].ToString());
            }
            KetNoi.Close();
        }

        private void ChatLieu_Load(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            HienThi();
        }
        private void btnThoat_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnThem_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaChatLieu.Text))
            {
                MessageBox.Show("Mã chất liệu không được để trống!");
                return;
            }

            KetNoi = new SqlConnection(Nguon);
            Lenh = "SELECT COUNT(*) FROM ChatLieu WHERE MaChatLieu = @MaChatLieu AND MaChatLieu <> @OldMaChatLieu";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaChatLieu", txtMaChatLieu.Text);
            ThucHien.Parameters.AddWithValue("@OldMaChatLieu", ""); // Giả sử không có mã chất liệu cũ khi thêm mới

            KetNoi.Open();
            int count = (int)ThucHien.ExecuteScalar();
            KetNoi.Close();

            if (count > 0)
            {
                MessageBox.Show("Mã chất liệu đã tồn tại ở bản ghi khác!");
                return;
            }

            Lenh = @"INSERT INTO ChatLieu (MaChatLieu, TenChatLieu) VALUES (@MaChatLieu, @TenChatLieu)";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaChatLieu", txtMaChatLieu.Text);
            ThucHien.Parameters.AddWithValue("@TenChatLieu", txtTenChatLieu.Text);

            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            KetNoi.Close();
            HienThi();
        }

        private void btnSua_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtMaChatLieu.Text == "") //nếu chưa chọn bản ghi nào
            {
                MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (txtTenChatLieu.Text.Trim().Length == 0) //nếu chưa nhập tên chất liệu
            {
                MessageBox.Show("Bạn chưa nhập tên chất liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Lấy mã chất liệu cũ từ dòng đang chọn trên DataGridView
            string oldMaChatLieu = dataGridView1.CurrentRow.Cells[0].Value.ToString();

            KetNoi = new SqlConnection(Nguon);
            Lenh = @"UPDATE ChatLieu SET MaChatLieu = @MaChatLieu, TenChatLieu = @TenChatLieu WHERE MaChatLieu = @OldMaChatLieu";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@MaChatLieu", txtMaChatLieu.Text);
            ThucHien.Parameters.AddWithValue("@TenChatLieu", txtTenChatLieu.Text);
            ThucHien.Parameters.AddWithValue("@OldMaChatLieu", oldMaChatLieu);

            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            KetNoi.Close();

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
                if (txtMaChatLieu.Text == "") //nếu chưa chọn bản ghi nào
                {
                    MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                KetNoi = new SqlConnection(Nguon);
                Lenh = @"DELETE FROM ChatLieu WHERE MaChatLieu = @MaChatLieu";
                ThucHien = new SqlCommand(Lenh, KetNoi);
                ThucHien.Parameters.AddWithValue("@MaChatLieu", txtMaChatLieu.Text);
                KetNoi.Open();
                ThucHien.ExecuteNonQuery();
                KetNoi.Close();
                HienThi();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMaChatLieu.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            txtTenChatLieu.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
