using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBachHoa.FuntionsForms
{
    public partial class ProductChild : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=BuuBuu;Initial Catalog=QLCHBH;Integrated Security=True");
        GraphicsPath gp = new GraphicsPath();
        public ProductChild()
        {
            InitializeComponent();
            SetButtonStyle(btnAddSP);
            SetButtonStyle(btnRem);
            SetButtonStyle(btnSave);
        }
        private void SetButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(244, 247, 252);

            button.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                int borderRadius = 35;
                GraphicsPath path = new GraphicsPath();
                RectangleF rect = new RectangleF(0, 0, button.Width - 1, button.Height - 1);
                path.AddArc(rect.X, rect.Y, borderRadius, borderRadius, 180, 90); // Đường cong bên trái
                path.AddArc(rect.X + rect.Width - borderRadius, rect.Y, borderRadius, borderRadius, 270, 90); // Đường cong bên phải
                path.AddArc(rect.X + rect.Width - borderRadius, rect.Y + rect.Height - borderRadius, borderRadius, borderRadius, 0, 90); // Đường cong phía dưới bên phải
                path.AddArc(rect.X, rect.Y + rect.Height - borderRadius, borderRadius, borderRadius, 90, 90); // Đường cong phía dưới bên trái
                path.CloseAllFigures();
                button.Region = new Region(path);

                using (Pen pen = new Pen(Color.Black, 2))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            };
        }

        private void ProductChild_Load(object sender, EventArgs e)
        { 
            SqlCommand cmd = new SqlCommand("SELECT IDloai, Tenloai FROM LoaiSP", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            cbbCategory.DataSource = dataTable;
            cbbCategory.DisplayMember = "Tenloai";
            cbbCategory.ValueMember = "IDloai";

        }
        

        private void btnAddSP_Click(object sender, EventArgs e)
        {
            string idProduct = txtIDProduct.Text;
            if (string.IsNullOrEmpty(idProduct))
            {
                MessageBox.Show("Vui lòng nhập ID sản phẩm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string nameProduct = txtNameProduct.Text;
            string categoryId = cbbCategory.SelectedValue.ToString();
            decimal price = decimal.Parse(txtPrice.Text);
            int inventory = int.Parse(txtInventory.Text);

            dtGVDSSP.Rows.Add(idProduct, nameProduct, categoryId, price, inventory);

            txtIDProduct.Text = string.Empty;
            txtNameProduct.Text = string.Empty;
            cbbCategory.SelectedIndex = -1;
            txtPrice.Text = string.Empty;
            txtInventory.Text = string.Empty;
        }

        private void btnRem_Click(object sender, EventArgs e)
        {
            if (dtGVDSSP.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Bạn có muốn xóa dòng được chọn?", "Xóa dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    foreach (DataGridViewRow row in dtGVDSSP.SelectedRows)
                    {
                        dtGVDSSP.Rows.Remove(row);
                    }
                }
            }
            else if (dtGVDSSP.CurrentRow != null)
            {
                DialogResult result = MessageBox.Show("Bạn có muốn xóa dòng hiện tại?", "Xóa dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dtGVDSSP.Rows.Remove(dtGVDSSP.CurrentRow);
                }
            }
            else
            {
                DialogResult result = MessageBox.Show("Bạn có muốn xóa tất cả dữ liệu?", "Xóa dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    dtGVDSSP.Rows.Clear();
                }
            }
        }

        private void ClearControls()
        {
            txtIDProduct.Text = string.Empty;
            txtNameProduct.Text = string.Empty;
            cbbCategory.SelectedItem = null;
            txtPrice.Text = string.Empty;
            txtInventory.Text = string.Empty;
        }
        private void dtGVDSSP_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow selectedRow = dtGVDSSP.Rows[e.RowIndex];
                txtIDProduct.Text = selectedRow.Cells["IDSanpham"].Value?.ToString();
                txtNameProduct.Text = selectedRow.Cells["TenSanpham"].Value?.ToString();
                string selectedCategoryID = selectedRow.Cells["IDLoai"].Value?.ToString();
                foreach (DataRowView item in cbbCategory.Items)
                {
                    if (item["IDLoai"].ToString() == selectedCategoryID)
                    {
                        cbbCategory.SelectedItem = item;
                        break;
                    }
                }
                txtPrice.Text = selectedRow.Cells["Gia"].Value?.ToString();
                txtInventory.Text = selectedRow.Cells["SoluongKho"].Value?.ToString();
            }
            else
            {
                ClearControls();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(@"Data Source=BuuBuu;Initial Catalog=QLCHBH;Integrated Security=True"))
                {
                    connection.Open();

                    // Tạo DataTable để chứa dữ liệu từ DataGridView
                    DataTable dataTable = new DataTable();
                    dataTable.Columns.Add("IDSanpham", typeof(string));
                    dataTable.Columns.Add("TenSanpham", typeof(string));
                    dataTable.Columns.Add("IDLoai", typeof(string));
                    dataTable.Columns.Add("Gia", typeof(decimal));
                    dataTable.Columns.Add("SoluongKho", typeof(int));

                    // Thêm dữ liệu từ DataGridView vào DataTable
                    foreach (DataGridViewRow row in dtGVDSSP.Rows)
                    {
                        string idProduct = row.Cells["IDSanpham"].Value.ToString();
                        string nameProduct = row.Cells["TenSanpham"].Value.ToString();
                        string categoryId = row.Cells["IDLoai"].Value.ToString();
                        decimal price = decimal.Parse(row.Cells["Gia"].Value.ToString());
                        int inventory = int.Parse(row.Cells["SoluongKho"].Value.ToString());

                        dataTable.Rows.Add(idProduct, nameProduct, categoryId, price, inventory);
                    }

                    // Sử dụng SqlBulkCopy để lưu DataTable vào cơ sở dữ liệu
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "Sanpham";
                        bulkCopy.ColumnMappings.Add("IDSanpham", "IDSanpham");
                        bulkCopy.ColumnMappings.Add("TenSanpham", "TenSanpham");
                        bulkCopy.ColumnMappings.Add("IDLoai", "IDLoai");
                        bulkCopy.ColumnMappings.Add("Gia", "Gia");
                        bulkCopy.ColumnMappings.Add("SoluongKho", "SoluongKho");

                        bulkCopy.WriteToServer(dataTable);
                    }

                    MessageBox.Show("Lưu dữ liệu thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
 }
