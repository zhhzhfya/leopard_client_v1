using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using leopard.utils.utils;

namespace framework
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            string value = ConfigHelper.Get("appUrl");
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add(value);
            this.comboBox1.SelectedText = value;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
