using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YandexGeo.Models
{
    [Table("cs_street", Schema = "dbo")]
    public class Street
    {
        [Key]
        public Guid id { get; set; }
        /// <summary>
        /// Район
        /// </summary>
        public string c_type { get; set; }
        public string c_name { get; set; }
        public DateTime? dx_date { get; set; }
        public bool? b_disabled { get; set; }
        public int? f_division { get; set; }
        public string c_short_type { get; set; }
        public int? f_user { get; set; }
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
