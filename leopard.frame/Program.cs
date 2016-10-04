﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using frame;

namespace framework
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form6());
        }
    }
}