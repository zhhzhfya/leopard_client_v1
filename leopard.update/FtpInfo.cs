
namespace update
{
    /// <summary>
    /// FTP服务器信息
    /// </summary>
    public struct FtpInfo
    {
        private string _remoteHost;
        private int _remotePort;
        private string _remotePath;
        private string _remoteUser;
        private string _remotePass;

        /// <summary>
        /// FTP伺服器IP地址
        /// </summary>
        public string RemoteHost
        {
            get { return _remoteHost; }
            set { _remoteHost = value; }
        }
        /// <summary>
        /// FTP伺服器端口号
        /// </summary>

        public int RemotePort
        {
            get { return _remotePort; }
            set { _remotePort = value; }
        }
        /// <summary>
        /// 当前访问FTP开放目录下的相对路径
        /// </summary>

        public string RemotePath
        {
            get { return _remotePath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _remotePath = "\\";
                else
                {
                    if (value[value.Length - 1] != '\\')
                        _remotePath = value + "\\";
                    else
                        _remotePath = value;
                }
            }
        }
        /// <summary>
        /// FTP帐号
        /// </summary>

        public string RemoteUser
        {
            get { return _remoteUser; }
            set { _remoteUser = value; }
        }
        /// <summary>
        /// FTP用户密码
        /// </summary>

        public string RemotePass
        {
            get { return _remotePass; }
            set { _remotePass = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="remoteHost"></param>
        /// <param name="remotePath"></param>
        /// <param name="remoteUser"></param>
        /// <param name="remotePass"></param>
        /// <param name="remotePort"></param>
        public FtpInfo(string remoteHost, string remotePath, string remoteUser, string remotePass, int remotePort)
        {
            _remoteHost = remoteHost;

            if (string.IsNullOrEmpty(remotePath))
                _remotePath = "\\";
            else
            {
                if (remotePath[remotePath.Length - 1] != '\\')
                    _remotePath = remotePath + "\\";
                else
                    _remotePath = remotePath;
            }

            _remoteUser = remoteUser;
            _remotePass = remotePass;
            _remotePort = remotePort;
        }
    }
}