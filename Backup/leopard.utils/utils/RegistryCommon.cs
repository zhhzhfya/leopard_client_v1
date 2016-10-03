using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace framework.utils
{
    /// <summary>注册表的值的类型</summary>
    public enum RegistryValueType { String, Binary, DWord, QWord };

    /// <summary>
    /// 简化了注册表的基本操作，如果要进行复杂的操作，可以使用它的属性Key
    /// </summary>
    public class RegistryCommon
    {
        private string _strKeyPath = "";
        private RegistryKey _regRootKey = Registry.CurrentUser;
        RegistryKey _myKey = null;

        /// <summary>
        /// 键路径 默认根键是:Registry.CurrentUser,格式："Software\\Cjf.Sxtrpig";
        /// </summary>
        /// <param name="aSubKeyPath"></param>
        public RegistryCommon(string aSubKeyPath)
        {
            _strKeyPath = aSubKeyPath;
            Open();
        }

        /// <summary>
        /// 根键,键路径 格式："Software\\Cjf.Sxtrpig";
        /// </summary>
        /// <param name="aRootKey">根键</param>
        /// <param name="aSubKeyPath"></param>
        public RegistryCommon(RegistryKey aRootKey, string aSubKeyPath)
        {
            _regRootKey = aRootKey;
            _strKeyPath = aSubKeyPath;
            Open();
        }

        /// <summary>
        /// 返回打开键的根键
        /// </summary>
        public RegistryKey RootKey { get { return _regRootKey; } }

        /// <summary>
        /// 返回键路径
        /// </summary>
        public string KeyPath { get { return _strKeyPath; } }

        /// <summary>
        /// 返回打开的键
        /// </summary>
        public RegistryKey Key { get { return _myKey; } }

        /// <summary>
        /// 打开键
        /// </summary>
        public bool Open()
        {
            try
            {
                if (_myKey != null)
                    return true;
                _myKey = _regRootKey.OpenSubKey(_strKeyPath, true);
                if (_myKey == null)
                {
                    _myKey = _regRootKey.CreateSubKey(_strKeyPath);
                    _myKey.Flush();
                }
                return true;
            }
            catch { return false; }

        }

        /// <summary>
        /// 设置项的值
        /// </summary>
        /// <param name="aItemName">项名</param>
        /// <param name="aItemValue">项值</param>
        /// <returns></returns>
        public bool SetValue(string aItemName, byte[] aItemValue, RegistryValueType aType)
        {
            try
            {
                if (_myKey == null)
                    return false;
                _myKey.SetValue(aItemName, aItemValue);
                _myKey.Flush();
                return true;
            }
            catch { return false; }

        }

        /// <summary>
        /// 设置项的值
        /// </summary>
        /// <param name="aItemName">项名</param>
        /// <param name="aItemValue">项值</param>
        /// <returns></returns>
        public bool SetValue(string aItemName, string aItemValue, RegistryValueType aType)
        {
            try
            {
                if (_myKey == null)
                    return false;
                switch (aType)
                {
                    case RegistryValueType.String:
                        _myKey.SetValue(aItemName, aItemValue);
                        break;
                    case RegistryValueType.Binary:
                        break;
                }
                _myKey.SetValue(aItemName, aItemValue);
                _myKey.Flush();
                return true;
            }
            catch { return false; }

        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="aItemName"><项名/param>
        public void DeleteValue(string aItemName)
        {
            try
            {
                _myKey.DeleteValue(aItemName);
            }
            catch { }
        }

        /// <summary>
        /// 获得项的值
        /// </summary>
        /// <param name="aItemName">项名</param>
        /// <returns></returns>
        public string GetValue(string aItemName)
        {
            try
            {
                object v = _myKey.GetValue(aItemName);
                if (v == null)
                    return null;
                return v.ToString();
            }
            catch { return null; }
        }

        /// <summary>
        /// 获取注册表的节点名称数组
        /// </summary>
        /// <returns></returns>
        public string[] getSubKeyNames()
        {
            if (_regRootKey.SubKeyCount > 0)
            {
                return _regRootKey.GetSubKeyNames();
            }
            return null;
        }

        /// <summary>
        /// 删除自身键
        /// </summary>
        public void DeleteKey()
        {
            try
            {
                Close();
                _regRootKey.DeleteSubKeyTree(_strKeyPath);
            }
            catch { }

        }

        /// <summary>
        /// 关闭键
        /// </summary>
        public void Close()
        {
            if (_myKey != null)
                _myKey.Close();
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~RegistryCommon()
        {
            Close();
        }
    }
}
