using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BACKUP_RESTORE_NGAN
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // hiển thị các database đang hiện có
            String sqlCommand = "SELECT name, database_id" +
                               " FROM sys.databases" +
                               " WHERE(database_id >= 5) AND(NOT(name LIKE N'ReportServer%'))" +
                               " ORDER BY NAME";
            Program.myReader = Program.ExecSqlDataReader(sqlCommand);
            this.listBoxDB.DisplayMember = "name";
            while (Program.myReader.Read())
            {
                String tenDatabase = Program.myReader.GetString(0);
                int idDatabase = Program.myReader.GetInt32(1);
                listBoxDB.Items.Add(new ListDBItem(tenDatabase, idDatabase));
            }
            Program.myReader.Close();
            if (listBoxDB.Items.Count > 0)
            {
                listBoxDB.SelectedIndex = 0;
            }
            //hiển thị thông tin database đang chọn
            this.txtTenCSDL.Text = (listBoxDB.SelectedItem as ListDBItem).Name;
            this.txtId.Text = (listBoxDB.SelectedItem as ListDBItem).Id.ToString();
            //hiển thị các bản backup
            updateTable();
            dateTimePicker1.CustomFormat = "MM/dd/yyyy hh:mm:ss tt";
            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;

        }

        private void btnThoat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Program.form1.Close();
            Program.formDN.Visible = true;
        }

        private void listBoxDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.txtTenCSDL.Text = (listBoxDB.SelectedItem as ListDBItem).Name;
            updateTable();
        }

        private void updateTable()
        {
            this.dataGridViewBanSaoLuu.Rows.Clear();
            //hiển thị các bản backup
            Program.sql = "SELECT position, name, backup_start_date , user_name FROM  msdb.dbo.backupset" +
                               " WHERE database_name = '" + txtTenCSDL.Text + "' AND type = 'D' AND" +
                                " backup_set_id >=" +
                                         "(SELECT MAX(backup_set_id) FROM msdb.dbo.backupset" +
                                             " WHERE media_set_id =" +
                                           " (SELECT  MAX(media_set_id)" +
                                                 " FROM msdb.dbo.backupset" +
                                                       " WHERE database_name = '" + txtTenCSDL.Text + "' AND type = 'D')" +
                                              " AND position = 1" +
                                       " )" +
                                " ORDER BY position DESC";
            Program.sqlCommand = new SqlCommand(Program.sql, Program.conn);
            Program.myReader = Program.sqlCommand.ExecuteReader();

            while (Program.myReader.Read())
            {
                String[] arr = new string[4];
                arr[0] = Program.myReader.GetInt32(0).ToString();
                arr[1] = Program.myReader.IsDBNull(1) ? "" : Program.myReader.GetString(1);
                arr[2] = Program.myReader.GetDateTime(2).ToString();
                arr[3] = Program.myReader.GetString(3);
                this.dataGridViewBanSaoLuu.Rows.Add(arr[0], arr[1], arr[2], arr[3]);
            }

            Program.myReader.Close();
            // select xem DB do co device chưa
            Program.sql = "SELECT name FROM sys.backup_devices where name = 'DEVICE_" + txtTenCSDL.Text + "'";
            Program.sqlCommand = new SqlCommand(Program.sql, Program.conn);
            Program.myReader = Program.sqlCommand.ExecuteReader();
            if (Program.myReader.Read())
            {
                btnTaoDevice.Enabled = false;
                btnSaoLuu.Enabled = true;
                btnPhucHoi.Enabled = true;
                btnPhuchoitheotg.Enabled = true;
            }
            else
            {
                btnTaoDevice.Enabled = true;
                btnSaoLuu.Enabled = false;
                btnPhucHoi.Enabled = false;
                btnPhuchoitheotg.Enabled = false;
            }
            Program.myReader.Close();
            if (dataGridViewBanSaoLuu.Rows.Count > 0)
            {
                txtId.Text = dataGridViewBanSaoLuu.SelectedRows[0].Cells[0].Value.ToString();
                dataGridViewBanSaoLuu.Rows[0].Selected = true;
                btnPhucHoi.Enabled = true;
                //btnSaoLuu.Enabled = true;
                barCheckPhucHoiTheoTG.Enabled = true;
                // btnTaoDevice.Enabled = false;
            }
            else
            {
                txtId.Text = "";
                //btnSaoLuu.Enabled = false;
                btnPhucHoi.Enabled = false;
                barCheckPhucHoiTheoTG.Enabled = false;
            }

        }
        private void dataGridViewBanSaoLuu_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewBanSaoLuu.SelectedRows.Count > 0)
                txtId.Text = dataGridViewBanSaoLuu.SelectedRows[0].Cells[0].Value.ToString();
        }
        private void btnSaoLuu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            String sqlCommand = "";
            if (checkBoxInit.Checked) // backup init
            {
                DialogResult dialogResult = MessageBox.Show("Bạn muốn xóa tất cả các bản sao lưu cũ trong File trước khi sao lưu", "backup INIT", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    this.progressBar1.Value = 0;
                    try
                    {                       
                        this.progressBar1.Visible = true;
                        sqlCommand = "BACKUP DATABASE " + txtTenCSDL.Text + " TO DEVICE_" + txtTenCSDL.Text + " WITH INIT;";
                        Program.myReader = Program.ExecSqlDataReader(sqlCommand);
                        Program.myReader.Close();
                        this.progressBar1.Value = 100;
                        MessageBox.Show("Đã backup thành công!", "THÔNG BÁO");
                        this.progressBar1.Visible = false;
                        updateTable();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Lỗi sao lưu dữ liệu " + ex);
                    }

                }

            }
            else
            {
                DialogResult dialogResult = MessageBox.Show(" Sao lưu full ", "CẢNH BÁO", MessageBoxButtons.OKCancel);
                if (dialogResult == DialogResult.OK)
                {
                    this.progressBar1.Value = 0;
                                                                           
                    try
                    {
                        this.progressBar1.Visible = true;
                        sqlCommand = "BACKUP DATABASE " + txtTenCSDL.Text + " TO " + "DEVICE_" + txtTenCSDL.Text.Trim() + ";";
                        Program.myReader = Program.ExecSqlDataReader(sqlCommand);
                        Program.myReader.Close();
                        this.progressBar1.Value = 100;
                        MessageBox.Show("Đã sao lưu thành công!", "THÔNG BÁO");
                        this.progressBar1.Visible = false;
                        updateTable();
                    }
                    catch(SqlException ex)
                    {
                        MessageBox.Show("Lỗi sao lưu dữ liệu " + ex);
                    }
                }

            }
        }

        private void barCheckPhucHoiTheoTG_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

            if (barCheckPhucHoiTheoTG.Checked == true)
            {
                MessageBox.Show("Vui lòng chọn thời điểm phục hồi bên dưới, sau đó nhấn Phục hồi");
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;
                dateTimePicker1.Focus();
            }
            else
            {
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }
        }

        private void btnPhucHoi_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (barCheckPhucHoiTheoTG.Checked == true)
            {
                //PHUC HOI DEN THOI DIEM CHUA BACKUP
                // MessageBox.Show("PHUC HOI DEN THOI DIEM CHUA BACKUP");
                //checkTime();

                //dataGridViewBanSaoLuu.SelectedRows[0].Selected = false;

               // dataGridViewBanSaoLuu.Rows[0].Selected = true;

                String dbname = txtTenCSDL.Text;
                String backupTime = dataGridViewBanSaoLuu.SelectedRows[0].Cells[2].Value.ToString();
                int file = int.Parse(txtId.Text);
                String TimeStopAt = dateTimePicker1.Value.ToString();
                if (checkTime() == false)
                {
                    MessageBox.Show("Thời điểm đã chọn phải sau thời điểm bản backup đã chọn và trước thời điểm hiện tại ít nhất 3 phút!", "CẢNH BÁO");
                    return;
                }
                DialogResult dialogResult = MessageBox.Show("Bạn muốn phục hồi database " + txtTenCSDL.Text + " từ thời điểm " + backupTime + " đến thời điểm " + TimeStopAt + "?", "CẢNH BÁO", MessageBoxButtons.YesNo);
                String sqlCommand = "";
                if (dialogResult == DialogResult.Yes)
                {
                    this.progressBar1.Value = 10;
                    this.progressBar1.Visible = true;
                    sqlCommand = "ALTER DATABASE " + dbname + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;\n" 
                        + "BACKUP LOG " + dbname + " TO DISK = 'E:\\Backup\\" + dbname + "_LOG.trn' WITH INIT, NORECOVERY;\n"
                     + "USE master \n"
                     + "RESTORE DATABASE " + dbname + " FROM DEVICE_" + dbname + " WITH FILE = " + file + ", NORECOVERY;\n"
                     + "RESTORE DATABASE " + dbname + " FROM DISK = 'E:\\Backup\\" + dbname + "_LOG.trn' WITH STOPAT = '" + TimeStopAt + "';\n"
                     + "ALTER DATABASE " + dbname + " SET MULTI_USER";
                    try
                    {
                        
                        Program.myReader = Program.ExecSqlDataReader(sqlCommand);
                        Program.myReader.Close();
                        this.progressBar1.Value = 100;
                        MessageBox.Show("Đã restore thành công!", "THÔNG BÁO");
                        this.progressBar1.Visible = false;
                    }
                    catch(SqlException ex)
                    {
                        MessageBox.Show("Lỗi không thể restore database " + ex);
                    }
                    

                }
            }
            else
            {
                //PHUC HOI DEN THOI DIEM DA BACKUP
                String ngayGio = dataGridViewBanSaoLuu.SelectedRows[0].Cells[2].Value.ToString();
                int position = int.Parse(dataGridViewBanSaoLuu.SelectedRows[0].Cells[0].Value.ToString());
                DialogResult dialogResult = MessageBox.Show("Bạn muốn phục hồi database " + txtTenCSDL.Text + " tại thời điểm " + ngayGio + "?", "CẢNH BÁO", MessageBoxButtons.YesNo);
                String sqlCommand = "";
                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        this.progressBar1.Value = 0;
                        this.progressBar1.Visible = true;
                        sqlCommand = "ALTER DATABASE " + txtTenCSDL.Text + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE " +
                                    " USE tempdb" +
                                    " RESTORE DATABASE " + txtTenCSDL.Text + " FROM DEVICE_" + txtTenCSDL.Text + " WITH FILE = " + position + ", REPLACE" +
                                    " ALTER DATABASE " + txtTenCSDL.Text + "  SET MULTI_USER";
                        Program.myReader = Program.ExecSqlDataReader(sqlCommand);
                        Program.myReader.Close();
                        this.progressBar1.Value = 100;
                        MessageBox.Show("Đã restore thành công!", "THÔNG BÁO");
                        this.progressBar1.Visible = false;
                    }
                    catch(SqlException ex)
                    {
                        MessageBox.Show("Lỗi không thể restore " + ex);
                    }
                    
                }
            }
        }

        private void btnTaoDevice_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            String sqlCreateDevice = "exec sp_addumpdevice 'disk', 'DEVICE_" + txtTenCSDL.Text + "', 'E:\\Backup\\DEVICE_" + txtTenCSDL.Text + ".bak'";
            DialogResult dialogResult = MessageBox.Show("Bạn có chắc muốn backup database" + txtTenCSDL.Text + " vào đường dẫn: E:\\Backup\\DEVICE_" + txtTenCSDL.Text + ".bak", "CẢNH BÁO", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    this.progressBar1.Value = 0;
                    this.progressBar1.Visible = true;
                    Program.myReader = Program.ExecSqlDataReader(sqlCreateDevice);
                    Program.myReader.Close();
                    this.progressBar1.Value = 100;
                    //Program.myReader = Program.ExecSqlDataReader(sqlCommand);
                    MessageBox.Show("Đã tạo device thành công!", "THÔNG BÁO");
                    this.progressBar1.Visible = false;
                }
                catch (Exception)
                {
                    MessageBox.Show("Tạo DEVICE Backup thất bại!", "THÔNG BÁO");
                }
                Program.myReader.Close();
                //updateBackupListView();
                updateTable();
            }
            
        }

        private bool checkTime()
        {
            // time chon tren luoi
            DateTime backup = DateTime.Parse(dataGridViewBanSaoLuu.SelectedRows[0].Cells[2].Value.ToString());

            // time user nhap
            /* String strTimeStopAt = dateTimePicker1.Value.Day + "/" + dateTimePicker1.Value.Month + "/"
                 + dateTimePicker1.Value.Year + " " + dateTimePicker2.Value.Hour + ":" + (dateTimePicker2.Value.Minute) + ":"
                 + dateTimePicker2.Value.Second;*/

            DateTime TimeStopAt = dateTimePicker1.Value;
            if (backup < TimeStopAt && TimeStopAt.AddMinutes(3) < DateTime.Now)
            {
               // MessageBox.Show("time hợp lệ");
                return true;
            }
            else
            {
               // MessageBox.Show("time không hợp lệ");
                return false;
            }

        }
    }
}
class ListDBItem
{
    private String name;
    private int id;
    public string Name { get; set; }
    public int Id { get; set; }
    public ListDBItem(string name, int id)
    {
        Id = id;
        Name = name;
    }
}