using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using YandexGeo.Models;

namespace YandexGeo
{
    /// <summary>
    /// https://advanced.name/freeproxy/5f20226e3d7c2?type=http
    /// </summary>
    class Program
    {
        static string URL = "https://geocode-maps.yandex.ru/1.x/?apikey={0}&geocode={1}&format=json";

        static void Main(string[] args)
        {
            YandexGeo.Utilits.HouseUtil.Run(args, URL);
        }
    }
}
