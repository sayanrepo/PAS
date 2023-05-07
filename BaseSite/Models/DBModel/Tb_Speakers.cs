using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Speakers
    {
        public Tb_Speakers()
        {
        }
    
        public short Id { get; set; }
        public string Name { get; set; }
    }
}
