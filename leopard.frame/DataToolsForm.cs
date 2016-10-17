using Excel;
using leopard.utils.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace frame
{
    public partial class DataToolsForm : Form
    {
        DataTable dt = new DataTable();
        public DataToolsForm()
        {
            InitializeComponent();
        }

        private void labelFolderSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dt.Rows.Clear();
                dt.Columns.Clear();
                //dataGridView1.Rows.Clear();
                //dataGridView1.Columns.Clear();
                
                string foldPath = dialog.SelectedPath;
                //MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var files = Directory.GetFiles(foldPath, "*.*", SearchOption.AllDirectories)
                                    .Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx"));
                Console.WriteLine(files.Count());

                dt.Columns.Add("序号", Type.GetType("System.Int32"));
                dt.Columns[0].AutoIncrement = true;
                dt.Columns[0].AutoIncrementSeed = 1;
                dt.Columns[0].AutoIncrementStep = 1;

                dt.Columns.Add("文件", Type.GetType("System.String"));
                dt.Columns.Add("包含列", Type.GetType("System.String"));
                dt.Columns.Add("数据行数", Type.GetType("System.String"));
                dt.Columns.Add("校验信息", Type.GetType("System.String"));
                dt.Columns.Add("上传", Type.GetType("System.String"));
                foreach (var f in files)
                {
                    DataRow row = dt.NewRow();
                    row["文件"] = f;
                    dt.Rows.Add(row);
                }
                dataGridView1.DataSource = dt;

                // 线程处理计算每个文件的列、行数
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadFunc), this.dataGridView1.Rows[i]);
                }
                for (int i = 0; i < this.dataGridView1.Columns.Count; i++)//对于DataGridView的每一个列都调整
                {
                    this.dataGridView1.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
                }
            }
        }

        public void ThreadFunc(object state)
        {
            DataGridViewRow row = (DataGridViewRow)state;

            String filePath = row.Cells["文件"].Value.ToString();
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader = null;
            if (filePath.EndsWith(".xls"))
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            if (filePath.EndsWith(".xlsx"))
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            DataSet result = excelReader.AsDataSet();
            StringBuilder sb = new StringBuilder();
            if (this.checkBox1.Checked)
            {
                DataRow dataRow = result.Tables[0].Rows[0];
                foreach (var item in dataRow.ItemArray)
                {
                    sb.Append(item).Append(",");
                }
            }
            
            sb.Length = sb.Length - 1;
            //row.Cells["包含列"].Value = sb.ToString();
            UpdateGV(row.Cells["包含列"], sb.ToString());
            DataGridViewCell cell = row.Cells["数据行数"];
            
            //InvokeHelper.Set(row.Cells["数据行数"], "Value", row.DataGridView.RowCount);
            //row.Cells["数据行数"].Value = row.DataGridView.RowCount;
            UpdateGV(row.Cells["数据行数"], result.Tables[0].Rows.Count - 1);
            excelReader.Close();
        }

        delegate void UpdateDataGridView(DataGridViewCell dell, object value);
        private void UpdateGV(DataGridViewCell cell, object value) {
            if (dataGridView1.InvokeRequired)
            {
                this.BeginInvoke(new UpdateDataGridView(UpdateGV), new object[] { cell, value });
            } else {
                cell.Value = value;
            }
        }

        private void labelSelectTable_Click(object sender, EventArgs e)
        {
            selectTableForm form = new selectTableForm();
            form.Show();
        }

    }
}
