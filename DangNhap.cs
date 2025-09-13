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
    public partial class DangNhap : Form
    {
        public DangNhap()
        {
            InitializeComponent();
        }

        // ket noi CSDL
        string Nguon  = "Data Source = ASUSCANH; Initial Catalog = QLBHLUUNIEM; Integrated Security = True; TrustServerCertificate=False";
        string Lenh = @"";

        SqlConnection KetNoi;
        SqlCommand ThucHien;
        SqlDataReader Doc;
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = @"SELECT * FROM TaiKhoan WHERE TaiKhoan = @TaiKhoan AND MatKhau = @MatKhau";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.Add("@TaiKhoan", SqlDbType.NVarChar);
            ThucHien.Parameters.Add("@MatKhau", SqlDbType.NVarChar);
            ThucHien.Parameters["@TaiKhoan"].Value = txt_taikhoan.Text;
            ThucHien.Parameters["@MatKhau"].Value = txtMatKhau.Text;
            KetNoi.Open();
            Doc = ThucHien.ExecuteReader();
            if (Doc.Read() == true)
            {
                MessageBox.Show("Đăng nhập thành công");
                this.Hide();
                fPhanmemQli f = new fPhanmemQli();
                f.ShowDialog();
                
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu");
            }

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DangNhap_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát chương trình ?","Thông báo",MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.Cancel)
            {
            }
            else
            {
                e.Cancel = true;
            }
        }
            

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (txtMatKhau.PasswordChar == '*')
            {
                txtMatKhau.PasswordChar = '\0'; // hien mat khau
            }
            else
                txtMatKhau.PasswordChar = '*'; // an mat khau
        }

        private void btnDangKi_Click(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            Lenh = @"INSERT INTO TaiKhoan (TaiKhoan, MatKhau, Email) VALUES (@TaiKhoan, @MatKhau ,@Email)";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.Add("@TaiKhoan", SqlDbType.NVarChar);
            ThucHien.Parameters.Add("@MatKhau", SqlDbType.NVarChar);
            ThucHien.Parameters.Add("@Email", SqlDbType.NVarChar);
            ThucHien.Parameters["@TaiKhoan"].Value = txt_taikhoan.Text;
            ThucHien.Parameters["@MatKhau"].Value = txtMatKhau.Text;
            ThucHien.Parameters["@Email"].Value = txtEmail.Text;

            KetNoi.Open();
            int kq = ThucHien.ExecuteNonQuery();
            if (kq > 0)
            {
                MessageBox.Show("Đăng ký thành công");
            }
            else
            {
                MessageBox.Show("Đăng ký thất bại");
            }
            // Khong duoc de trong tai khoan va mat khau
            if (string.IsNullOrWhiteSpace(txt_taikhoan.Text) || string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Tài khoản và mật khẩu không được để trống");
                return;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // btn dang ky


    }
}
