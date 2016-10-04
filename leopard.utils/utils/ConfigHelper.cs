using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using framework.utils;

namespace leopard.utils.utils
{
    public class ConfigHelper
    {
        public static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        public static void SaveConfig(String key, String value)
        {
            // 写入参数设置
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            // 重新读取参数
            ConfigurationManager.RefreshSection("appSettings");
            WebServiceFactory.hessianUrl = ConfigurationManager.AppSettings["hessianUrl"];
            //WriteFile.WriteFileName = ConfigurationManager.AppSettings["WriteFileName"];
            //WriteFile.WritePath = ConfigurationManager.AppSettings["WritePath"].Split('|');
            //PostMessage.PostMessageURL = ConfigurationManager.AppSettings["PostMessageURL"];
            // PostMessage.LeasedLineURL = ConfigurationManager.AppSettings["LeasedLineURL"];
        }

    }
}
