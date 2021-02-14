using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using YandexGeo.Models;

namespace YandexGeo.Utilits
{
    public static class HouseUtil
    {
        public static void Run(string[] args, string URL)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var houses = (from h in db.Houses
                              join s in db.Streets on h.f_street equals s.id
                              where h.n_latitude == null
                              select new
                              {
                                  id = h.id,
                                  c_street_type = s.c_type,
                                  c_street_name = s.c_name,
                                  c_house_num = h.c_full_number
                              }).ToList();

                Console.WriteLine("count: " + houses.Count);
                for (int i = 0; i < houses.Count; i++)
                {
                    var house = houses[i];

                    if (house.c_house_num.ToLower().IndexOf("бн") >= 0)
                    {
                        continue;
                    }

                    string houseName = string.Format("Республика Чувашия, г. Чебоксары, {0} {1} {2}", house.c_street_type,  house.c_street_name, house.c_house_num);

                    Console.Write(i + "/" + houses.Count + ": " + houseName + "\r");

                    string url = string.Format(URL, args[0], houseName);
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Timeout = int.Parse(args[1]);

                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        string json = stream.ReadToEnd();

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

                            houseForUpdate.n_latitude = latitude;
                            houseForUpdate.n_longitude = longitude;

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

                                houseForUpdate.n_latitude = latitude;
                                houseForUpdate.n_longitude = longitude;

                                Console.Write("\r" + i + "/" + houses.Count + ": " + houseName + " SUCCESS\n");
                            }
                            else
                            {
                                Console.Write("\r" + i + "/" + houses.Count + ": " + houseName + " FAIL (" + found + ")\n");
                            }
                        }

                        db.SaveChanges();
                    }
                }
            }

            Console.WriteLine("FINISH");
        }
    }
}
