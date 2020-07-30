using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace YandexGeo
{
    /// <summary>
    /// http://free-proxy.cz/ru/proxylist/country/RU/http/ping/all/5
    /// </summary>
    public class ProxyList
    {
        private List<MyProxy> proxies;
        private string Path;

        private List<MyProxy> goodProxy;

        public ProxyList(String path, List<MyProxy> goodProxy)
        {
            Path = path;
            proxies = new List<MyProxy>();
            string txt = File.ReadAllText(Path);
            string[] lines = txt.Split("\r\n");
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                proxies.Add(new MyProxy(line));
            }

            this.goodProxy = goodProxy;
        }

        public List<MyProxy> GetProxyList()
        {
            return proxies.Where(t=>t.Status).ToList();
        }

        public void GoodProxy(MyProxy myProxy)
        {
            goodProxy.Add(myProxy);

            StringBuilder builder = new StringBuilder();
            foreach (MyProxy proxy in goodProxy)
            {
                builder.Append(proxy.ToString() + "\r\n");
            }
            string txt = builder.ToString();
            File.WriteAllText("good-proxy.txt", txt.Substring(0, txt.Length - 2));
        }

        public void updateFile()
        {
            StringBuilder builder = new StringBuilder();
            foreach (MyProxy myProxy in proxies)
            {
                if (myProxy.Status)
                {
                    builder.Append(myProxy.ToString() + "\r\n");
                }
            }
            string txt = builder.ToString();
            if (txt.Length == 0)
            {
                string proxyTxt = File.ReadAllText("proxy.txt");
                File.WriteAllText(Path, proxyTxt);
            }
            else
            {
                File.WriteAllText(Path, txt.Substring(0, txt.Length - 2));
            }
        }
    }
}
