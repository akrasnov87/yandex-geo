using System;
using System.Collections.Generic;
using System.Text;

namespace YandexGeo
{
    public class MyProxy
    {
        public MyProxy(string txt)
        {
            String[] data = txt.Trim().Split(":");
            IP = data[0];
            Port = int.Parse(data[1]);
            Status = true;
        }

        public string IP { get; set; }
        public int Port { get; set; }

        public bool Status { get; set; }

        public int Count { get; set; }

        public override string ToString()
        {
            return IP + ":" + Port;
        }
    }
}
