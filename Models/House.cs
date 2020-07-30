using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YandexGeo.Models
{
    [Table("cs_house", Schema = "dbo")]
    public class House
    {
        [Key]
        public Guid id { get; set; }
        public Guid f_street { get; set; }
        public string c_house_num { get; set; }
        public string c_build_num { get; set; }
        public DateTime? dx_date { get; set; }
        public int? n_uik { get; set; }
        public int? f_subdivision { get; set; }
        public int? f_user { get; set; }
        public bool b_correct_uik { get; set; }
        public int? n_uik_correct { get; set; }
        public double? n_latitude { get; set; }
        public double? n_longitude { get; set; }
        public string c_yandex_description { get; set; }
        public string c_yandex_name { get; set; }
        public bool b_yandex { get; set; }
        public bool b_yandex_fail { get; set; }
        [Column(TypeName = "jsonb")]
        public string jb_yandex_res { get; set; }
    }
}
