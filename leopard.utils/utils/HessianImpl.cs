using System;
using System.Collections.Generic;
using System.Text;

namespace framework.utils
{
    // svn_logs.utils.HessianService
    public interface HessianService
    {
        object[] findResultBySql(string sql);
        object[] findResultByPage(string sql, int page, int count);
    }
}
