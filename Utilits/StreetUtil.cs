using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using YandexGeo.Models;

namespace YandexGeo.Utilits
{
    public static class StreetUtil
    {
        public static void Run(string[] args, string URL)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var streets = (from s in db.Streets
                               where s.n_latitude == null && s.n_longitude == null
                               select new
                               {
                                   id = s.id,
                                   c_street_type = s.c_type,
                                   c_street_name = s.c_name
                               }).ToList();

                Console.WriteLine("count: " + streets.Count);

                for (int i = 0; i < streets.Count; i++)
                {
                    var street = streets[i];

                    string streetName = string.Format("Республика Чувашия, г. Чебоксары, {0} {1}", street.c_street_type, street.c_street_name);
                    Console.Write(i + "/" + streets.Count + ": " + streetName + "\r");

                    string url = string.Format(URL, args[0], streetName);
                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Timeout = int.Parse(args[1]);

                    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                    using (StreamReader stream = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                    {
                        string json = stream.ReadToEnd();
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

                            houseForUpdate.n_latitude = latitude;
                            houseForUpdate.n_longitude = longitude;

                            Console.Write("\r" + i + "/" + streets.Count + ": " + streetName + " SUCCESS\n");
                        }
                        else
                        {
                            Console.Write("\r" + i + "/" + streets.Count + ": " + streetName + " FAIL (" + found + ")\n");
                        }

                        db.SaveChanges();
                    }
                }
            }

            Console.WriteLine("FINISH");
        }
    }
}
