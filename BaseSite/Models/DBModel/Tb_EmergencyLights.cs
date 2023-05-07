using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_EmergencyLights
    {
        public Tb_EmergencyLights()
        {
        }

        public short Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
    }
}