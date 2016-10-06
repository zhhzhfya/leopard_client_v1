using System;
using System.Collections.Generic;
using System.Text;
using hessiancsharp.client;
using leopard.utils.utils;
using System.Data;

namespace framework.utils
{
    public class WebServiceFactory
    {
        private static HessianService hService = null;

        public static string hessianUrl = ConfigHelper.Get("hessianUrl");// "http://192.168.6.179:8080/hessian";

        public static HessianService HService
        {
            get
            {
                if (hService == null)
                {
                    CHessianProxyFactory factory = new CHessianProxyFactory();
                    hService = (HessianService)factory.Create(Type.GetType("framework.utils.HessianService"), hessianUrl);
                }
                return hService;
            }
        }

        public static object[] findResultBySql(string sql, bool withColumn)
        {
            return HService.findResultBySql(sql, withColumn);
        }

        public static object[] findResultByPage(string sql, int page, int count)
        {
            return HService.findResultByPage(sql, page, count);
        }

        public static void sqlToGrids(System.Windows.Forms.DataGridView dgv, string sql)
        {
            object[] result = findResultBySql(sql, false);
            if (result != null)
            {
                int columnSize = Int32.Parse(result[0].ToString());
                if (columnSize > 0)
                {
                    for (int i = 0; i < (result.Length - 1) / columnSize; i++)
                    {
                        object[] o = new object[columnSize];
                        Array.Copy(result, i * columnSize + 1, o, 0, columnSize);
                        dgv.Rows.Add(o);
                    }
                }
            }
        }

        public static DataTable sqlToDataTable(string sql)
        {
            DataTable dt = new DataTable();
            object[] result = findResultBySql(sql, true);
            if (result != null)
            {
                int columnSize = Int32.Parse(result[0].ToString());
                if (columnSize > 0)
                {
                    for (int i = 0; i < (result.Length - 1) / columnSize; i++)
                    {
                        object[] o = new object[columnSize];
                        Array.Copy(result, i * columnSize + 1, o, 0, columnSize);
                        if (i == 0)
                        {
                            for (int j = 0; j < o.Length; j++)
                            {
                                dt.Columns.Add(new DataColumn(o[j].ToString()));
                            }
                        }
                        else
                        {
                            dt.Rows.Add(o);
                        }
                    }
                }
            }
            return dt;
        }

        public static void sqlToGridWithHeader(System.Windows.Forms.DataGridView dgv, string sql)
        {
            //同步调用
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            object[] result = findResultBySql(sql, true);
            if (result != null)
            {
                // 列的数量
                int columnSize = Int32.Parse(result[0].ToString());
                if (columnSize > 0)
                {
                    for (int i = 0; i < (result.Length - 1) / columnSize; i++)
                    {
                        object[] o = new object[columnSize];
                        Array.Copy(result, i * columnSize + 1, o, 0, columnSize);
                        if (i == 0)
                        {
                            for (int j = 0; j < o.Length; j++)
                            {
                                dgv.Columns.Add("resultcolumn" + j, o[j].ToString());
                            }
                        }
                        else
                        {
                            dgv.Rows.Add(o);
                        }
                    }
                }
            }
        }

        public static void doAct()
        {
            
        }
    }
}
