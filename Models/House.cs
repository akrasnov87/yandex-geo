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
        public DateTime? dx_date { get; set; }
        public bool b_disabled { get; set; }
        public int? n_uik { get; set; }
        public int? f_subdivision { get; set; }
        public double? n_latitude { get; set; }
        public double? n_longitude { get; set; }
        public string c_full_number { get; set; }
    }
}
