using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YandexGeo.Models
{
    [Table("cv_tmp_house", Schema = "dbo")]
    public class TmpHouse
    {
        public Guid id { get; set; }
        public Guid f_street { get; set; }
        public string c_house_num { get; set; }
        public string c_build_num { get; set; }
    }
}
