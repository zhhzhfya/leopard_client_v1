using Excel;
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
                //dataGridView1.Rows.Clear();
                //dataGridView1.Columns.Clear();
                
                string foldPath = dialog.SelectedPath;
                //MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                var files = Directory.GetFiles(foldPath, "*.*", SearchOption.AllDirectories)
                                    .Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx"));
                Console.WriteLine(files.Count());
                DataTable dt = new DataTable();
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

                // 
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadFunc), this.dataGridView1.Rows[i]);
                }
                for (int i = 0; i < this.dataGridView1.Columns.Count; i++)//对于DataGridView的每一个列都调整
                {
                    this.dataGridView1.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
                    //width += this.dataGridView1.Columns[i].Width;//记录整个DataGridView的宽度
                }
                Console.WriteLine(".....");
            }
        }

        public void ThreadFunc(object state)
        {
            DataGridViewRow row = (DataGridViewRow)state;
            Console.WriteLine(row.Cells["文件"]);

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
            for (int i = 0; i < result.Tables[0].Columns.Count; i++)
            {
                DataColumn col = result.Tables[0].Columns[i];
                sb.Append(col.ToString());
                Console.WriteLine(col);
            }
            row.Cells["包含列"].Value = sb.ToString();
            
            excelReader.Close();
        }

        private void labelSelectTable_Click(object sender, EventArgs e)
        {
            selectTableForm form = new selectTableForm();
            form.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String filePath = @"C:\\hotel_2222.xlsx";
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = null;
            if (filePath.EndsWith(".xls"))
            {
                //Choose one of either 1 or 2
                //1. Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            if (filePath.EndsWith(".xlsx"))
            {
                //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            //Choose one of either 3, 4, or 5
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            DataSet result = excelReader.AsDataSet();

            //4. DataSet - Create column names from first row
            //excelReader.IsFirstRowAsColumnNames = true;
            //DataSet result = excelReader.AsDataSet();

            ////5. Data Reader methods
            //while (excelReader.Read())
            //{
            //    //excelReader.GetInt32(0);
            //}

            //6. Free resources (IExcelDataReader is IDisposable)
            excelReader.Close();
            dataGridView1.DataSource = result.Tables[0];
        }
    }
}
