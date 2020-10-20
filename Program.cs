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
        static string API_KEY = "869301a4-6978-4286-a523-eaf015c0912e";
        static string URL = "https://geocode-maps.yandex.ru/1.x/?apikey={0}&geocode={1}&format=json";
        static List<MyProxy> goodProxy;

        static void AddProxy(HttpWebRequest request, MyProxy myProxy)
        {
            var proxy = new WebProxy(myProxy.IP, myProxy.Port);
            request.Proxy = proxy;
        }

        static void Main(string[] args)
        {
            house(args);
        }

        static void street(string[] args)
        {
            goodProxy = new List<MyProxy>();
            API_KEY = args[0];

            ProxyList proxyList = new ProxyList(args[1], goodProxy);
            List<MyProxy> proxies = proxyList.GetProxyList();

            using (ApplicationContext db = new ApplicationContext())
            {
                var streets = (from s in db.Streets
                              where s.b_yandex_fail
                               select new
                              {
                                  id = s.id,
                                  c_street_type = s.c_type,
                                  c_street_name = s.c_name
                              }).ToList();
                Console.WriteLine("count: " + streets.Count);
                MyProxy myProxy = proxies.First();
                for (int i = 0; i < streets.Count; i++)
                {
                    if (proxies.Count == 0)
                    {
                        Console.WriteLine("Список прокси кончился");
                        break;
                    }


                    var street = streets[i];

                    string houseName = "чувашия " + street.c_street_name;
                    Console.Write(i + "/" + streets.Count + ": " + houseName + "\r");

                    string url = String.Format(URL, API_KEY, houseName);
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Timeout = int.Parse(args[2]);
                    AddProxy(req, myProxy);

                    try
                    {
                        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                        using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                        {
                            string json = stream.ReadToEnd();
                            myProxy.Count++;
                            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                            int found = response.response.GeoObjectCollection.metaDataProperty.GeocoderResponseMetaData.found;
                            Street houseForUpdate = db.Streets.First(t => t.id == street.id);
                            if (found > 0)
                            {
                                var featureMember = response.response.GeoObjectCollection.featureMember[0];
                                string description = featureMember.GeoObject.description;
                                string name = featureMember.GeoObject.name;
                                string geo = featureMember.GeoObject.Point.pos;

                                double latitude = double.Parse(geo.Split(' ')[1].Replace(".", ",")); // широта
                                double longitude = double.Parse(geo.Split(' ')[0].Replace(".", ",")); // долгота


                                houseForUpdate.b_yandex = true;
                                houseForUpdate.c_yandex_description = description;
                                houseForUpdate.c_yandex_name = name;
                                houseForUpdate.n_latitude = latitude;
                                houseForUpdate.n_longitude = longitude;
                                houseForUpdate.jb_yandex_res = json;

                                Console.Write("\r" + i + "/" + streets.Count + ": " + houseName + " SUCCESS\n");
                            }
                            else
                            {
                                houseForUpdate.b_yandex_fail = true;
                                Console.Write("\r" + i + "/" + streets.Count + ": " + houseName + " FAIL (" + found + ")\n");
                            }

                            db.SaveChanges();
                        }
                    }
                    catch (WebException e)
                    {
                        i--;

                        if (myProxy.Count >= int.Parse(args[3]))
                        {
                            proxyList.GoodProxy(myProxy);
                        }

                        myProxy.Status = false;
                        proxyList.updateFile();
                        proxyList = new ProxyList(args[1], goodProxy);
                        proxies = proxyList.GetProxyList();
                        myProxy = proxies.First();
                        Console.Write("\r" + i + "/" + streets.Count + ": " + houseName + " Fail timeout\n");
                    }
                }
            }

            Console.WriteLine("FINISH");
        }

        static void house(string[] args)
        {
            goodProxy = new List<MyProxy>();
            API_KEY = args[0];

            ProxyList proxyList = new ProxyList(args[1], goodProxy);
            List<MyProxy> proxies = proxyList.GetProxyList();

            using (ApplicationContext db = new ApplicationContext())
            {
                var houses = (from h in db.Houses
                              join s in db.Streets on h.f_street equals s.id
                              where !h.b_yandex
                              select new
                              {
                                  id = h.id,
                                  c_street_type = s.c_type,
                                  c_street_name = s.c_name,
                                  c_house_num = h.c_house_num,
                                  c_house_build = h.c_build_num
                              }).ToList();
                Console.WriteLine("count: " + houses.Count);
                MyProxy myProxy = proxies.First();
                for (int i = 0; i < houses.Count; i++)
                {
                    if (proxies.Count == 0)
                    {
                        Console.WriteLine("Список прокси кончился");
                        break;
                    }


                    var house = houses[i];

                    if (house.c_house_num.ToLower().IndexOf("бн") >= 0)
                    {
                        continue;
                    }

                    string houseName = "чебоксары " + house.c_street_type + " " + house.c_street_name + " " + house.c_house_num + (house.c_house_build == null || house.c_house_build.Trim().Length == 0 ? "" : " корп. " + house.c_house_build);
                    Console.Write(i + "/" + houses.Count + ": " + houseName + "\r");

                    string url = String.Format(URL, API_KEY, houseName);
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Timeout = int.Parse(args[2]);
                    AddProxy(req, myProxy);

                    try
                    {
                        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                        using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                        {
                            string json = stream.ReadToEnd();
                            myProxy.Count++;
                            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
                            int found = response.response.GeoObjectCollection.metaDataProperty.GeocoderResponseMetaData.found;
                            int results = response.response.GeoObjectCollection.metaDataProperty.GeocoderResponseMetaData.results;
                            House houseForUpdate = db.Houses.First(t => t.id == house.id);
                            if (found == 1)
                            {
                                var featureMember = response.response.GeoObjectCollection.featureMember[0];
                                string description = featureMember.GeoObject.description;
                                string name = featureMember.GeoObject.name;
                                string geo = featureMember.GeoObject.Point.pos;

                                double latitude = double.Parse(geo.Split(' ')[1].Replace(".", ",")); // широта
                                double longitude = double.Parse(geo.Split(' ')[0].Replace(".", ",")); // долгота


                                houseForUpdate.b_yandex = true;
                                houseForUpdate.c_yandex_description = description;
                                houseForUpdate.c_yandex_name = name;
                                houseForUpdate.n_latitude = latitude;
                                houseForUpdate.n_longitude = longitude;
                                houseForUpdate.jb_yandex_res = json;

                                Console.Write("\r" + i + "/" + houses.Count + ": " + houseName + " SUCCESS\n");
                            }
                            else
                            {
                                Console.WriteLine("\r");
                                for (int j = 0; j < response.response.GeoObjectCollection.featureMember.Count; j++)
                                {
                                    var _featureMember = response.response.GeoObjectCollection.featureMember[j];
                                    string _description = _featureMember.GeoObject.description;
                                    string _name = _featureMember.GeoObject.name;
                                    Console.WriteLine(j + ": " + _description + " " + _name);
                                }
                                int idx = int.Parse(Console.ReadLine());
                                if (idx >= 0)
                                {
                                    var featureMember = response.response.GeoObjectCollection.featureMember[idx];
                                    string description = featureMember.GeoObject.description;
                                    string name = featureMember.GeoObject.name;
                                    string geo = featureMember.GeoObject.Point.pos;

                                    double latitude = double.Parse(geo.Split(' ')[1].Replace(".", ",")); // широта
                                    double longitude = double.Parse(geo.Split(' ')[0].Replace(".", ",")); // долгота


                                    houseForUpdate.b_yandex = true;
                                    houseForUpdate.c_yandex_description = description;
                                    houseForUpdate.c_yandex_name = name;
                                    houseForUpdate.n_latitude = latitude;
                                    houseForUpdate.n_longitude = longitude;
                                    houseForUpdate.jb_yandex_res = json;

                                    Console.Write("\r" + i + "/" + houses.Count + ": " + houseName + " SUCCESS\n");
                                }
                                else
                                {

                                    houseForUpdate.b_yandex_fail = true;
                                    Console.Write("\r" + i + "/" + houses.Count + ": " + houseName + " FAIL (" + found + ")\n");
                                }
                            }

                            db.SaveChanges();
                        }
                    }
                    catch (WebException e)
                    {
                        i--;

                        if (myProxy.Count >= int.Parse(args[3]))
                        {
                            proxyList.GoodProxy(myProxy);
                        }

                        myProxy.Status = false;
                        proxyList.updateFile();
                        proxyList = new ProxyList(args[1], goodProxy);
                        proxies = proxyList.GetProxyList();
                        myProxy = proxies.First();
                        Console.Write("\r" + i + "/" + houses.Count + ": " + houseName + " Fail timeout\n");
                    }
                }
            }

            Console.WriteLine("FINISH");
        }
    }
}
