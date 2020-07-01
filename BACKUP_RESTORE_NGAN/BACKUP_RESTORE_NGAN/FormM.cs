using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BACKUP_RESTORE_NGAN
{
    public partial class FormM : Form
    {
        public FormM()
        {
            InitializeComponent();
        }

        private void FormM_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dS.databases' table. You can move, or remove it, as needed.
            this.sP_TT_BACKUPTableAdapter.Connection = Program.conn;
            this.sP_TT_BACKUPTableAdapter.Fill(this.dS.SP_TT_BACKUP, txtTenCSDL.Text);
          this.databasesTableAdapter.Connection = Program.conn;
            this.databasesTableAdapter.Fill(this.dS.databases1);
           
            if (sP_TT_BACKUPBindingSource.Count > 0)
            {
                txtId.Text = ((DataRowView)sP_TT_BACKUPBindingSource[0])["position"].ToString();

               // ((DataRowView)sP_TT_BACKUPBindingSource[0])["position"] = true;
                btnPhucHoi.Enabled = true;
                btnSaoLuu.Enabled = true;
                btnPhuchoitheotg.Enabled = true;
                btnTaoDevice.Enabled = false;
            }
            else
            {
                txtId.Text = "";
                btnSaoLuu.Enabled = false;
                btnPhucHoi.Enabled = false;
                btnPhuchoitheotg.Enabled = false;
            }
        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Program.formMain.Close();
            Program.formDN.Visible = true;
        }

        private void btnSaoLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String sqlCommand = "";
            if (checkBoxInit.Checked) // backup init
            {
                DialogResult dialogResult = MessageBox.Show("Nhấn thực sự muốn xóa các bản sao lưu cũ", "CẢNH BÁO", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    /* this.progressBar1.Value = 0;
                     this.progressBar1.Visible = true;
                     sqlCommand = "BACKUP DATABASE " + txtTenCSDL.Text + " TO " + txtTenCSDL.Text + "_DEVICE WITH INIT;";
                     Program.myReader = Program.ExecSqlDataReader(sqlCommand);
                     Program.myReader.Close();
                     this.progressBar1.Value = 100;
                     MessageBox.Show("Đã backup thành công!", "THÔNG BÁO");
                     this.progressBar1.Visible = false;
                     updateTable();*/
                }

            }
            //if (dialogResult == DialogResult.OK)
            // backup full
            else
            {
                DialogResult dialogResult = MessageBox.Show("Nhấn bạn muốn backup thường ", "CẢNH BÁO", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    this.progressBar1.Value = 0;
                    this.progressBar1.Visible = true;
                    sqlCommand = "BACKUP DATABASE " + txtTenCSDL.Text + " TO " + "DEVICE_" + txtTenCSDL.Text.Trim() + ";";
                    Program.myReader = Program.ExecSqlDataReader(sqlCommand);
                    Program.myReader.Close();
                    this.progressBar1.Value = 100;
                    MessageBox.Show("Đã backup thành công!", "THÔNG BÁO");
                    this.progressBar1.Visible = false;
                    // updateTable();
                    sP_TT_BACKUPTableAdapter.Connection = Program.conn;
                    this.sP_TT_BACKUPTableAdapter.Fill(this.dS.SP_TT_BACKUP, txtTenCSDL.Text);
                }

            }
        }
    }
}
