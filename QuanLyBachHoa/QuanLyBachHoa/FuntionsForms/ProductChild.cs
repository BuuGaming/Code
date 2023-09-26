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
            bool hasSelectedRows = dtGVDSSP.SelectedRows.Count > 0;

            if (hasSelectedRows)
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
            else
            {
                DialogResult result = MessageBox.Show("Bạn có muốn xóa tất cả dữ liệu?", "Xóa dữ liệu", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Xóa tất cả dữ liệu từ dtGVDSSP
                    dtGVDSSP.Rows.Clear();
                }
            }
        }
    }
 }
