using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;
using System.Data.SQLite;
using System.Collections;

namespace framework.utils
{
    /// <summary>
    ///  LocalDB SQLite助手类
    /// </summary>
    public class LocalDBSQLiteHelper : SQLiteHelperBase
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public static readonly string FILE_NAME = AppDomain.CurrentDomain.BaseDirectory + @"data.db";
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public override string ConnectionString
        {
            get
            {
                return base.ConnectionString;
            }
            set
            {
                base.ConnectionString = value;
            }
        }

        /// <summary>
        /// LocalDBSQLiteHelper实例
        /// </summary>
        private static LocalDBSQLiteHelper m_Instance = null;
        /// <summary>
        /// 线程同步变量
        /// </summary>
        static readonly object lockObject = new object();

        /// <summary>
        /// 私有构造方法
        /// </summary>
        private LocalDBSQLiteHelper()
        {
            ConnectionString = "Data Source=" + FILE_NAME + ";";
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static LocalDBSQLiteHelper GetInstance()
        {
            if (m_Instance == null)
            {
                lock (lockObject)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new LocalDBSQLiteHelper();
                    }
                }
            }
            return m_Instance;
        }


        

    }
}