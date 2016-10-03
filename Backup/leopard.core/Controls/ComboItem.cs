using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace framework.Controls
{
    /// <summary>
    /// ComboItem 的摘要说明。
    /// </summary>
    public class ComboItem
    {
        private string name;
        private int ivalue;
        private long lValue;
        private string strvalue = null;
        private object ovalue = null;

        public ComboItem(string name, int ivalue)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            this.name = name;
            this.ivalue = ivalue;
            this.strvalue = ivalue.ToString();
            this.ovalue = ivalue;
        }
        public ComboItem(String name, string strvalue)
        {
            this.name = name;
            this.strvalue = strvalue;
            this.ovalue = strvalue;
        }
        public ComboItem(String name, long lValue)
        {
            this.name = name;
            this.lValue = lValue;
            this.strvalue = lValue.ToString();
            this.ovalue = lValue;
        }
        public ComboItem(string name, object ovalue)
        {
            this.name = name;
            this.ovalue = ovalue;
            this.strvalue = ovalue != null ? ovalue.ToString() : null;
        }

        public string Name
        {
            get { return name; }
        }

        public int Value
        {
            get { return ivalue; }
        }
        public long longValue
        {
            get { return lValue; }
        }
        public string StrValue
        {
            get { return strvalue; }
        }
        public object ObjectValue
        {
            get { return ovalue; }
        }
        public override string ToString()
        {
            return this.name;
        }


        public static ComboItem[] HashToComboItem(Hashtable hash)
        {
            try
            {
                ComboItem[] comboItems = new ComboItem[hash.Count];
                string[] keys = new string[hash.Count];

                IEnumerator ie = hash.Keys.GetEnumerator();
                int index = 0;
                while (ie.MoveNext())
                {
                    keys[index] = ie.Current.ToString();
                    index++;
                }

                for (int i = 0; i < keys.Length; i++)
                {
                    for (int j = i + 1; j < keys.Length; j++)
                    {
                        if (keys[i].CompareTo(keys[j]) > 0)
                        {
                            string tmp = keys[i];
                            keys[i] = keys[j];
                            keys[j] = tmp;
                        }
                    }
                }

                for (int i = 0; i < keys.Length; i++)
                {
                    int ivalue = int.Parse(keys[i]);
                    string name = hash[keys[i]].ToString();
                    comboItems[i] = new ComboItem(name, ivalue);
                }

                return comboItems;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static void DefaultSelectedItem(ComboBox comboBox, string strValue)
        {
            List<ComboItem> result = (List<ComboItem>)comboBox.DataSource;
            foreach (ComboItem cbItem in result)
            {
                if (cbItem.StrValue == strValue)
                {
                    comboBox.SelectedItem = cbItem;
                }
            }
        }

        public static void DefaultSelectedItem2(ComboBox comboBox, string strValue)
        {
            ArrayList result = (ArrayList)comboBox.DataSource;
            foreach (ComboItem cbItem in result)
            {
                if (cbItem.StrValue.Equals(strValue))
                {
                    comboBox.SelectedItem = cbItem;
                    return;
                }
            }
        }
    }
}
