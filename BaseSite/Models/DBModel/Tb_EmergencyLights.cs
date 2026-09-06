using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


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