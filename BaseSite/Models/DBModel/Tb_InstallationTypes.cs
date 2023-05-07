using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_InstallationTypes
    {
        public Tb_InstallationTypes()
        {
            this.Order_Cabin = new HashSet<Order_Cabin>();
        }
    
        public short Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("InstallationTypeId")]
        public virtual ICollection<Order_Cabin> Order_Cabin { get; set; }
    }
}
