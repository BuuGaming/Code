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

namespace QuanLyBachHoa
{
    public partial class LoginForm : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=BuuBuu;Initial Catalog=QLCHBH;Integrated Security=True");
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            String username, password;
            username = txtUser.Text;
            password = txtPass.Text;

            try
            {
                String querry = "select * from Nhanvien where Username ='" + txtUser.Text + "'and Password ='" + txtPass.Text + "'";
                SqlDataAdapter sda = new SqlDataAdapter(querry, conn);

                DataTable dt = new DataTable();
                sda.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    username = txtUser.Text;
                    password = txtPass.Text;

                    MainForm formmain = new MainForm();
                    formmain.Show();
                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại!\nHãy kiểm tra lại tài khoản và mật khẩu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUser.Clear();
                    txtPass.Clear();

                    txtUser.Focus();
                }
            }
            catch
            {
                MessageBox.Show("Error!");
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnHidePass_Click(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = false;
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Timer timer = (Timer)sender;
            timer.Stop();
            txtPass.UseSystemPasswordChar = true;
        }
    }
}
