using System;
using System.Collections.Generic;
using System.Text;

namespace framework.utils
{
    // svn_logs.utils.HessianService
    public interface HessianService
    {
        object[] findResultBySql(string sql, bool withColumn);
        object[] findResultByPage(string sql, int page, int count);
        void doAct(string servId, string act, object[] objects);
    }
}
