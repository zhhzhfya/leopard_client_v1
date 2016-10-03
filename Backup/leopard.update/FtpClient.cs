using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;

namespace update
{
    /// <summary>
    /// FTPClient 的摘要說明。
    /// </summary>
    internal class FTPClient
    {
        private FtpInfo _ftp;
        private Boolean bConnected;

        #region 构造函数与属性

        /// <summary>
        /// 是否处于登录状态
        /// </summary>
        public bool Connected
        {
            get { return bConnected; }
        }

        public FTPClient(FtpInfo ftp)
        {
            _ftp = ftp;
            //Connect();
        }

        #endregion

        #region 连接
        /// <summary>
        /// 确保所设定的HOST为IP地址，如果传入的是域名，则获取解析的第一个IP
        /// </summary>
        /// <returns></returns>
        public bool EnsureHostIsIP()
        {
            IPAddress address = null;

            if (IPAddress.TryParse(_ftp.RemoteHost, out address))
                return true;
            else
            {
                IPHostEntry dnstoip = Dns.GetHostEntry(_ftp.RemoteHost);
                if (dnstoip.AddressList.Length == 0)
                    return false;
                else
                {
                    _ftp.RemoteHost = dnstoip.AddressList[0].ToString();
                    return true;
                }
            }
        }
        /// <summary>
        /// 建立连接 
        /// </summary>
        public void Connect()
        {
            if (!EnsureHostIsIP())
            {
                bConnected = false;
                return;
            }
            socketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(_ftp.RemoteHost), _ftp.RemotePort);
            // 鏈結
            try
            {
                socketControl.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("Couldn't connect to remote server");
            }
            // 获取应答码
            ReadReply();
            if (iReplyCode != 220)
            {
                DisConnect();
                throw new IOException(strReply.Substring(4));
            }
            // 登陸
            SendCommand("USER " + _ftp.RemoteUser);
            if (!(iReplyCode == 331 || iReplyCode == 230))
            {
                CloseSocketConnect();//关闭连接
                throw new IOException(strReply.Substring(4));
            }
            if (iReplyCode != 230)
            {
                SendCommand("PASS " + _ftp.RemotePass);
                if (!(iReplyCode == 230 || iReplyCode == 202))
                {
                    CloseSocketConnect();//关闭连接
                    throw new IOException(strReply.Substring(4));
                }
            }
            bConnected = true;
            // 切換到目录
            ChangeDir(_ftp.RemotePath);
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void DisConnect()
        {
            if (socketControl != null)
            {
                SendCommand("QUIT");
            }
            CloseSocketConnect();
        }
        #endregion

        #region 传输模式
        /// <summary>
        /// 传输模式:二进位類型、ASCII類型
        /// </summary>
        public enum TransferType { Binary, ASCII };
        /// <summary>
        /// 設置传输模式
        /// </summary>
        /// <param name="ttType">传输模式</param>
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I");//binary類型传输
            }
            else
            {
                SendCommand("TYPE A");//ASCII類型传输
            }
            if (iReplyCode != 200)
            {
                throw new IOException(strReply.Substring(4));
            }
            else
            {
                trType = ttType;
            }
        }

        /// <summary>
        /// 获得传输模式
        /// </summary>
        /// <returns>传输模式</returns>
        public TransferType GetTransferType()
        {
            return trType;
        }

        #endregion

        #region 档操作
        /// <summary>
        /// 获得文件列表
        /// </summary>
        /// <param name="strMask">档案名的匹配字串</param>
        /// <returns></returns>
        public string[] GetFileList(string strMask)
        {
            // 建立鏈結
            if (!bConnected)
            {
                Connect();
            }
            //建立进行资料连接的socket
            Socket socketData = CreateDataSocket();

            //传送命令
            SendCommand("NLST " + strMask);
            //分析应答代码
            if (!(iReplyCode == 150 || iReplyCode == 125 || iReplyCode == 226))
            {
                throw new IOException(strReply.Substring(4));
            }
            //获得結果
            strMsg = "";
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }
            char[] seperator = { '\n' };
            strMsg = strMsg.Replace("\r", string.Empty);
            string[] strsFileList = strMsg.Split(seperator);
            socketData.Close();//资料socket关闭時也会有返回码
            if (iReplyCode != 226)
            {
                ReadReply();
                if (iReplyCode != 226)
                {
                    throw new IOException(strReply.Substring(4));
                }
            }
            return strsFileList;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="strFileName">档案名</param>
        /// <returns>文件大小</returns>
        private long GetFileSize(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("SIZE " + Path.GetFileName(strFileName));
            long lSize = 0;
            if (iReplyCode == 213)
            {
                lSize = Int64.Parse(strReply.Substring(4));
            }
            else
            {
                throw new IOException(strReply.Substring(4));
            }
            return lSize;
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="strFileName">待刪除档案名</param>
        public void DeleteFile(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("DELE " + strFileName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }

        /// <summary>
        /// 重命名(如果新档案名與已有档重名,將覆蓋已有文件)
        /// </summary>
        /// <param name="strOldFileName">舊档案名</param>
        /// <param name="strNewFileName">新档案名</param>
        public void Rename(string strOldFileName, string strNewFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RNFR " + strOldFileName);
            if (iReplyCode != 350)
            {
                throw new IOException(strReply.Substring(4));
            }
            //  如果新档案名與原有档重名,將覆蓋原有档
            SendCommand("RNTO " + strNewFileName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }
        #endregion

        #region 上传和下載
        /// <summary>
        /// 下載一批档
        /// </summary>
        /// <param name="strFileNameMask">档案名的匹配字串</param>
        /// <param name="strFolder">本地目录(不得以\結束)</param>
        public void Download(string strFileNameMask, string strFolder)
        {
            if (!bConnected)
            {
                Connect();
            }
            string[] strFiles = GetFileList(strFileNameMask);
            foreach (string strFile in strFiles)
            {
                if (!strFile.Equals(""))//一般來說strFiles的最后一個元素可能是空字串
                {
                    Download(strFile, strFolder, strFile);
                }
            }
        }

        /// <summary>
        /// 下載一個档
        /// </summary>
        /// <param name="strRemoteFileName">要下載的档案名</param>
        /// <param name="strFolder">本地目录(不得以\結束)</param>
        /// <param name="strLocalFileName">保存在本地時的档案名</param>
        public void Download(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SetTransferType(TransferType.Binary);

            if (strLocalFileName.Equals(""))
                strLocalFileName = strRemoteFileName;

            if (strFolder.Length != 0 && strFolder.LastIndexOf('\\') == strFolder.Length - 1)
                strFolder = strFolder.Substring(0, strFolder.Length - 1);

            if (strFolder.Length != 0 && !Directory.Exists(strFolder))
                Directory.CreateDirectory(strFolder);

            string fullName = strFolder.Length == 0 ? strLocalFileName : strFolder + "\\" + strLocalFileName;

            //if (!File.Exists(fullName))
            //{
            //    Stream st = File.Create(fullName);
            //    st.Close();
            //}
            FileStream output = new FileStream(fullName, FileMode.Create);

            Socket socketData = CreateDataSocket();
            SendCommand("RETR " + strRemoteFileName);
            if (!(iReplyCode == 150 || iReplyCode == 125
             || iReplyCode == 226 || iReplyCode == 250))
            {
                throw new IOException(strReply.Substring(4));
            }
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                output.Write(buffer, 0, iBytes);
                if (iBytes <= 0)
                {
                    break;
                }
            }
            output.Flush();
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    throw new IOException(strReply.Substring(4));
                }
            }
        }

        /// <summary>
        /// 上传一批档
        /// </summary>
        /// <param name="strFolder">本地目录(不得以\結束)</param>
        /// <param name="strFileNameMask">档案名匹配字元(可以包含*和?)</param>
        public void Upload(string strFolder, string strFileNameMask)
        {
            string[] strFiles = Directory.GetFiles(strFolder, strFileNameMask);
            foreach (string strFile in strFiles)
            {
                //strFile是完整的档案名(包含路径)
                Upload(strFile);
            }
        }

        /// <summary>
        /// 上传一個档
        /// </summary>
        /// <param name="strFileName">本地档案名</param>
        public void Upload(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            Socket socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strFileName));
            if (!(iReplyCode == 125 || iReplyCode == 150))
            {
                throw new IOException(strReply.Substring(4));
            }
            FileStream input = new
             FileStream(strFileName, FileMode.Open);
            int iBytes = 0;
            while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                socketData.Send(buffer, iBytes, 0);
            }
            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                if (!(iReplyCode == 226 || iReplyCode == 250))
                {
                    throw new IOException(strReply.Substring(4));
                }
            }
        }

        #endregion

        #region 目录操作
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="strDirName">目录名</param>
        public void CreateDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("MKD " + strDirName);
            if (iReplyCode != 257)
            {
                throw new IOException(strReply.Substring(4));
            }
        }


        /// <summary>
        /// 刪除目录
        /// </summary>
        /// <param name="strDirName">目录名</param>
        public void RemoveDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RMD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
        }


        /// <summary>
        /// 改变当前目录
        /// </summary>
        /// <param name="strDirName">新的工作目录名</param>
        public void ChangeDir(string strDirName)
        {
            if (strDirName.Equals(".") || strDirName.Equals(""))
            {
                return;
            }
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("CWD " + strDirName);
            if (iReplyCode != 250)
            {
                throw new IOException(strReply.Substring(4));
            }
            this._ftp.RemotePath = strDirName;
        }

        #endregion

        #region 內部变量
        /// <summary>
        /// 伺服器返回的应答信息(包含应答码)
        /// </summary>
        private string strMsg;
        /// <summary>
        /// 伺服器返回的应答信息(包含应答码)
        /// </summary>
        private string strReply;
        /// <summary>
        /// 伺服器返回的应答码
        /// </summary>
        private int iReplyCode;
        /// <summary>
        /// 进行控制连接的socket
        /// </summary>
        private Socket socketControl;
        /// <summary>
        /// 传输模式
        /// </summary>
        private TransferType trType;
        /// <summary>
        /// 接收和发送资料的缓冲區
        /// </summary>
        private static int BLOCK_SIZE = 512;
        Byte[] buffer = new Byte[BLOCK_SIZE];
        /// <summary>
        /// 編码方式
        /// </summary>
        Encoding ASCII = Encoding.GetEncoding("gb2312");
        #endregion

        #region 內部函數
        /// <summary>
        /// 將一行应答字串记录在strReply和strMsg
        /// 应答码记录在iReplyCode
        /// </summary>
        private void ReadReply()
        {
            strMsg = "";
            strReply = ReadLine();
            iReplyCode = Int32.Parse(strReply.Substring(0, 3));
        }
        /// <summary>
        /// 建立进行资料连接的socket
        /// </summary>
        /// <returns>资料连接socket</returns>
        private Socket CreateDataSocket()
        {
            SendCommand("PASV");
            if (iReplyCode != 227)
            {
                throw new IOException(strReply.Substring(4));
            }
            int index1 = strReply.IndexOf('(');
            int index2 = strReply.IndexOf(')');
            string ipData =
             strReply.Substring(index1 + 1, index2 - index1 - 1);
            int[] parts = new int[6];
            int len = ipData.Length;
            int partCount = 0;
            string buf = "";
            for (int i = 0; i < len && partCount <= 6; i++)
            {
                char ch = Char.Parse(ipData.Substring(i, 1));
                if (Char.IsDigit(ch))
                    buf += ch;
                else if (ch != ',')
                {
                    throw new IOException("Malformed PASV strReply: " +
                     strReply);
                }
                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = "";
                    }
                    catch (Exception)
                    {
                        throw new IOException("Malformed PASV strReply: " +
                         strReply);
                    }
                }
            }
            string ipAddress = parts[0] + "." + parts[1] + "." +
             parts[2] + "." + parts[3];
            int port = (parts[4] << 8) + parts[5];
            Socket s = new
             Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new
             IPEndPoint(IPAddress.Parse(ipAddress), port);
            try
            {
                s.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("Can't connect to remote server");
            }
            return s;
        }

        /// <summary>
        /// 关闭socket连接(用于登录以前)
        /// </summary>
        private void CloseSocketConnect()
        {
            if (socketControl != null)
            {
                socketControl.Close();
                socketControl = null;
            }
            bConnected = false;
        }

        /// <summary>
        /// 读取Socket返回的所有字串
        /// </summary>
        /// <returns>包含应答码的字串行</returns>
        private string ReadLine()
        {
            while (true)
            {
                int iBytes = socketControl.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }
            char[] seperator = { '\n' };
            string[] mess = strMsg.Split(seperator);
            if (strMsg.Length > 2)
            {
                strMsg = mess[mess.Length - 2];
                //seperator[0]是10,換行符是由13和0組成的,分隔后10后面雖沒有字串,
                //但也会分配為空字串給后面(也是最后一個)字串陣列,
                //所以最后一個mess是沒用的空字串
                //但為什麼不直接取mess[0],因為只有最后一行字串应答码與信息之間有空格
            }
            else
            {
                strMsg = mess[0];
            }
            if (!strMsg.Substring(3, 1).Equals(" "))//返回字串正確的是以应答码(如220開頭,后面接一空格,再接問候字串)
            {
                return ReadLine();
            }
            return strMsg;
        }

        /// <summary>
        /// 发送命令并获取应答码和最后一行应答字串
        /// </summary>
        /// <param name="strCommand">命令</param>
        private void SendCommand(String strCommand)
        {
            Byte[] cmdBytes =
            ASCII.GetBytes((strCommand + "\r\n").ToCharArray());
            socketControl.Send(cmdBytes, cmdBytes.Length, 0);
            ReadReply();
        }
        #endregion
    }
}
