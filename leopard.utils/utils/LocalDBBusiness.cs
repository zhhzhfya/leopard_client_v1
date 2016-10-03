using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Data.SQLite;

namespace framework.utils
{
    public class LocalDBBusiness
    {
        /// <summary>
        /// LocalDBBusiness实例
        /// </summary>
        private static LocalDBBusiness m_Instance = null;

        /// <summary>
        /// 线程同步变量
        /// </summary>
        static readonly object lockObject = new object();
        /// <summary>
        /// 私有构造方法
        /// </summary>
        private LocalDBBusiness()
        {
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static LocalDBBusiness GetInstance()
        {
            if (m_Instance == null)
            {
                lock (lockObject)
                {
                    if (m_Instance == null)
                    {
                        m_Instance = new LocalDBBusiness();
                    }
                }
            }
            return m_Instance;
        }

        /// <summary>
        /// 检查一个表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns>true 表存在，false 表不存在</returns>
        public bool CheckTableExist(string tableName)
        {
            DataSet dataSet = LocalDBBusiness.GetSQLiteMasterByTableName(tableName);
            if (dataSet == null || dataSet.Tables[0].Rows.Count == 0)
            {
                //数据集为空，返回false
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检查本地存储文件是否存在
        /// </summary>
        /// <returns></returns>
        public bool HasLocalStorage()
        {
            if (File.Exists(LocalDBSQLiteHelper.FILE_NAME))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 创建SQLite数据库
        /// </summary>
        public void CreateLocalStorageFile()
        {
            System.Data.SQLite.SQLiteConnection.CreateFile(LocalDBSQLiteHelper.FILE_NAME);
        }

        /// <summary>
        /// 删除数据库
        /// </summary>
        public void DeleteLocalStorageFile()
        {
            if (File.Exists(LocalDBSQLiteHelper.FILE_NAME))
            {
                File.Delete(LocalDBSQLiteHelper.FILE_NAME);
            }
        }

        /// <summary>
        /// 访问SQLiteMaster表
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static DataSet GetSQLiteMaster()
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("Select tbl_name From sqlite_master where Type='table'");
                DataSet dataSet = LocalDBSQLiteHelper.GetInstance().ExecuteDataSet(command);
                return dataSet;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 访问SQLiteMaster表按照表名称查询
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static DataSet GetSQLiteMasterByTableName(string tableName)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand("Select tbl_name From sqlite_master where Type='table' and tbl_name = @TableName");
                command.Parameters.Add(new SQLiteParameter("@TableName", tableName));
                DataSet dataSet = LocalDBSQLiteHelper.GetInstance().ExecuteDataSet(command);
                return dataSet;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
