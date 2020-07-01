using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace BACKUP_RESTORE_NGAN
{
    public partial class FormDN : DevExpress.XtraEditors.XtraForm
    {
        private bool kiemtra()
        {
            tbServer.Text = tbTenDangNhap.Text = ltbMatKhau.Text = "";
            if (String.IsNullOrWhiteSpace(txtTenServer.Text))
            {
                tbServer.Visible = true;
                tbServer.Text = "Bạn chưa nhập tên server!";
            }
            if (String.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                tbTenDangNhap.Visible = true;
                tbTenDangNhap.Text = "Bạn chưa nhập tên đăng nhập!";
            }
            if (String.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                ltbMatKhau.Visible = true;
                ltbMatKhau.Text = "Bạn chưa nhập mật khẩu!";
            }
            if (!String.IsNullOrWhiteSpace(txtTenServer.Text) && !String.IsNullOrWhiteSpace(txtTenDangNhap.Text) && !String.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                return true;
            }
            else return false;
        }
    
        public FormDN()
        {
            InitializeComponent();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Program.formDN.Close();
        }

        private void btnDN_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("vao form DN");
            if (kiemtra() == true)
            {
                Program.mlogin = txtTenDangNhap.Text;
                Program.password = txtMatKhau.Text;
                Program.servername = txtTenServer.Text;
                if (Program.KetNoi() == 1)
                {
                    Program.form1 = new Form1();
                    Program.form1.Activate();
                    Program.form1.Show();                   
                    this.Visible = false;
                }
            }
        }
     
    }
}