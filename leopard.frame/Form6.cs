using Excel;
using framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace frame
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }


        private void Form6_Load(object sender, EventArgs e)
        {
            TabPage tab = new TabPage();
            tab.Name = "dataToolsTab";
            tab.Text = "收集工具";

            DataToolsForm f2 = new DataToolsForm();
            f2.TopLevel = false;
            f2.Dock = DockStyle.Fill;
            f2.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            tab.Controls.Add(f2);

            f2.Parent = tab;
            tabControl1.TabPages.Add(tab);
            tabControl1.SelectedTab = tab;
            f2.Show();
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            selectTableForm form = new selectTableForm();
            form.Show();
        }

        private void toolStripLabel3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 重新登录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoginForm f = new LoginForm();
            f.Show();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles("C:\\work", "*.*", SearchOption.AllDirectories)
                                .Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx"));
            
            foreach (var f in files)
            {
                Console.WriteLine(f);
            }
            Console.WriteLine(".....");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MysqlDBForm mf = new MysqlDBForm();
            mf.Show();
        }

    }
}
