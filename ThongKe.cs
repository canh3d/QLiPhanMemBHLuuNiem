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
    public partial class ThongKe : Form
    {
        public ThongKe()
        {
            InitializeComponent();
        }
        // ket noi CSDL
        string Nguon = "Data Source = ASUSCANH; Initial Catalog = QLBHLUUNIEM; Integrated Security = True; TrustServerCertificate=False";
        string Lenh = @"";
        SqlConnection KetNoi;
        SqlCommand ThucHien;
        SqlDataReader Doc;


        private void ThongKe_Load(object sender, EventArgs e)
        {
            KetNoi = new SqlConnection(Nguon);
            dtpNgayBan.Value = DateTime.Now;
            HienThiTheoNgay(dtpNgayBan.Value.Date);
        }
        void HienThiTheoNgay(DateTime ngayChon)
        {
            Lenh = @"SELECT MaHDBan, MaKH, MaNV, NgayBan, ThanhTien FROM HDBan WHERE CAST(NgayBan AS DATE) = @NgayBan";
            KetNoi = new SqlConnection(Nguon);
            ThucHien = new SqlCommand(Lenh, KetNoi);
            ThucHien.Parameters.AddWithValue("@NgayBan", ngayChon);
            try
            {
                KetNoi.Open();
                SqlDataAdapter da = new SqlDataAdapter(ThucHien);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Tính tổng tiền tự động từ cột ThanhTien
                decimal tongTien = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (row["ThanhTien"] != DBNull.Value)
                        tongTien += Convert.ToDecimal(row["ThanhTien"]);
                }
                txtTongTien.Text = tongTien.ToString("N0");
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

        private void dtpNgayBan_ValueChanged(object sender, EventArgs e)
        {
            HienThiTheoNgay(dtpNgayBan.Value.Date);
        }
        void HienThi()
        {
            dataGridView1.Rows.Clear();
            Lenh = @"SELECT MaHDBan, MaKH, MaNV, NgayBan, ThanhTien FROM HDBan";
            ThucHien = new SqlCommand(Lenh, KetNoi);
            KetNoi.Open();
            ThucHien.ExecuteNonQuery();
            Doc = ThucHien.ExecuteReader();
            while (Doc.Read())
            {
                dataGridView1.Rows.Add(Doc[0].ToString(), Doc[1].ToString(), Doc[2].ToString(), Convert.ToDateTime(Doc[3]).ToString("dd/MM/yyyy"), Doc[4].ToString());
            }
            KetNoi.Close();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            dtpNgayBan.Value = DateTime.Now;
            txtTongTien.Clear();
            dataGridView1.DataSource = null;
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Excel.Application excelApp = new Excel.Application();
            excelApp.Visible = true;
            Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];
            worksheet.Name = "Thống Kê Doanh Thu";

            // Tiêu đề
            Excel.Range titleRange = worksheet.Range["A1", "E1"];
            titleRange.Merge();
            titleRange.Value = "THỐNG KÊ DOANH THU NGÀY " + dtpNgayBan.Value.ToString("dd/MM/yyyy");
            titleRange.Font.Size = 16;
            titleRange.Font.Bold = true;
            titleRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // Tiêu đề cột
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                worksheet.Cells[3, i + 1] = dataGridView1.Columns[i].HeaderText;
                worksheet.Cells[3, i + 1].Font.Bold = true;
                worksheet.Cells[3, i + 1].Interior.Color = Color.LightGray;
            }

            // Dữ liệu
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    worksheet.Cells[i + 4, j + 1] = dataGridView1.Rows[i].Cells[j].Value?.ToString();
                }
            }

            // Tổng tiền
            worksheet.Cells[dataGridView1.Rows.Count + 5, 4] = "Tổng doanh thu:";
            worksheet.Cells[dataGridView1.Rows.Count + 5, 5] = txtTongTien.Text + " VND";
            worksheet.Cells[dataGridView1.Rows.Count + 5, 4].Font.Bold = true;
            worksheet.Cells[dataGridView1.Rows.Count + 5, 5].Font.Bold = true;

            worksheet.Columns.AutoFit();

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtTongTien_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
