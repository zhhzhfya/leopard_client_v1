using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace deploy.tools
{
    public partial class DeployMain : Form
    {
        public DeployMain()
        {
            InitializeComponent();
        }

        static string[] selfFiles = { "lib.Update.Caller.dll", "update.exe" };

        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = Application.StartupPath;
            this.DialogResult = folderBrowserDialog1.ShowDialog(this);
            if (DialogResult == DialogResult.OK)
            {
                this.txtDir.Text = folderBrowserDialog1.SelectedPath;
                btnLoad.Enabled = true;
            }
            else
            {
                if (string.IsNullOrEmpty(txtDir.Text))
                    btnLoad.Enabled = btnGenerate.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDir.Text))
            {
                MessageBox.Show("请选择项目文件夹！");
            }
            if (!Directory.Exists(txtDir.Text))
            {
                MessageBox.Show("所选择文件夹不存在或无法访问！");
                return;
            }

            treeDir.Nodes.Clear();
            DirectoryInfo rootDir = new DirectoryInfo(txtDir.Text);
            ShowDirectoryNode(rootDir, null);
            treeDir.ExpandAll();

            btnGenerate.Enabled = true;
        }

        /// <summary>
        /// 展示文件目录对应的树节点
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <param name="pntNode"></param>
        private void ShowDirectoryNode(DirectoryInfo dirInfo, TreeNode pntNode)
        {
            TreeNode node = new TreeNode(dirInfo.Name, 2, 3);
            node.Checked = true;
            node.Tag = "0";     //0表示文件夹， 1表示文件

            if (pntNode == null)
                treeDir.Nodes.Add(node);
            else
                pntNode.Nodes.Add(node);

            DirectoryInfo[] childDirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo childDir in childDirs)
            {
                if (childDir.Name.ToLower() == "updatetemp")
                    continue;

                ShowDirectoryNode(childDir, node);
            }
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (selfFiles.Contains(file.Name.ToLower()))
                    continue;

                TreeNode fileNode = new TreeNode(file.Name, 0, 1);
                fileNode.Checked = true;
                fileNode.Tag = "1";
                fileNode.ToolTipText = file.FullName;
                node.Nodes.Add(fileNode);
            }
        }
    }
}
