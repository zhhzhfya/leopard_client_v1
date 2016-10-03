using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using update.utils;
using System.IO;

namespace upate
{
    public partial class UpdateForm : Form
    {
        public UpdateForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 判断注册表
            RegistryCommon rc = new RegistryCommon("Software\\");
            RegistryKey hdSoftWare = rc.Key.OpenSubKey("HaydenSoftWare");
            if (null == hdSoftWare)
            {
                // 如果为空创建一个节点
                RegistryKey rk = rc.Key.CreateSubKey("HaydenSoftWare");
                rk.Flush();

                RegistryKey apps = rk.CreateSubKey("apps");
            }
            // 验证URL正确否？
            ComboItem item = (ComboItem)this.comboBox1.SelectedItem;
            // 进行安装
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK || this.folderBrowserDialog1.ShowDialog() == DialogResult.Yes)
            {
                string path = this.folderBrowserDialog1.SelectedPath;
                if (!this.folderBrowserDialog1.SelectedPath.EndsWith("HaydenSoftWare"))
                {
                    if (!path.EndsWith("\\") && !path.EndsWith("/"))
                    {
                        path += "\\";
                    }
                    path += "HaydenSoftWare";
                }
                this.label1.Text = "安装路径："+path;
            }
                        
            //this.timer1.Start();
            // 运行

        }

        private void Form5_Load(object sender, EventArgs e)
        {
            // 判断注册表
            RegistryCommon rc = new RegistryCommon("Software\\");
            RegistryKey hdSoftWare = rc.Key.OpenSubKey("HaydenSoftWare");
            if (null == hdSoftWare)
            {
                // 如果为空创建一个节点
                RegistryKey rk = rc.Key.CreateSubKey("HaydenSoftWare");
                rk.Flush();
            }
            else
            {
                RegistryKey apps_rk = hdSoftWare.OpenSubKey("apps");
                if (apps_rk != null)
                {
                    string[] apps = apps_rk.GetValueNames();
                    this.comboBox1.Items.Clear();
                    foreach (string app in apps)
                    {
                        this.comboBox1.Items.Add(new ComboItem(apps_rk.GetValue(app) + "   " + app, apps_rk.GetValue(app)));
                    }
                    this.comboBox1.SelectedIndex = 0;
                }
            }
        }

        private void comboBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.comboBox1.DroppedDown = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.progressBar1.Value < this.progressBar1.Maximum)
            {
                this.progressBar1.Value = this.progressBar1.Value + 1;    
            }
            else
            {
                this.timer1.Stop();
            }
        }
    }
}
