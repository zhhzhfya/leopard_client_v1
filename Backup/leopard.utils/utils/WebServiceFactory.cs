using System;
using System.Collections.Generic;
using System.Text;
using hessiancsharp.client;

namespace framework.utils
{
    public class WebServiceFactory
    {
        private static HessianService hService = null;

        public static HessianService HService
        {
            get
            {
                if (hService == null)
                {
                    CHessianProxyFactory factory = new CHessianProxyFactory();
                    string url = "http://192.168.6.179:8080/hessian";
                    hService = (HessianService)factory.Create(Type.GetType("svn_logs.utils.HessianService"), url);
                }
                return hService;
            }
        }

        public static object[] findResultBySql(string sql)
        {
            return HService.findResultBySql(sql);
        }

        public static object[] findResultByPage(string sql, int page, int count)
        {
            return HService.findResultByPage(sql, page, count);
        }

        public static void sqlToGrids(System.Windows.Forms.DataGridView dgv, string sql)
        {

            object[] result = findResultBySql(sql);
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

        public static void sqlToGridWithHeader(System.Windows.Forms.DataGridView dgv, string sql)
        {
            //同步调用
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            object[] result = findResultBySql(sql);
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
                                dgv.Columns.Add("resultcolumn" + i, o[j].ToString());
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
    }
}
