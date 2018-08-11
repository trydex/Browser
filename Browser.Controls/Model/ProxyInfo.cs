namespace Browser.Controls.Model
{
    public class ProxyInfo
    {
        public ProxyInfo(string scheme, string ip, int port, string username = null, string password = null)
        {
            Scheme = scheme;
            Ip = ip;
            Port = port;
            Username = username;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
        public string Ip { get; }
        public int Port { get;}

        /// <summary>
        /// Схема прокси: socks5,http и т.д.
        /// </summary>
        public string Scheme { get; }
    }
}