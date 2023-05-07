using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseSite.Models.DBModel
{
    public class Sale_Goods
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SaleId { get; set; }
        public byte TypeId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }
        public int Count { get; set; }
        public double Phi { get; set; }

        [MaxLength(255)]
        public string Comment { get; set; }
        public Nullable<int> DeliveryId { get; set; }

        [MaxLength(255)]
        public string DeliveryComment { get; set; }

        public int ProductId { get; set; }


        [ForeignKey("DeliveryId")]
        public virtual Delivery_Delivery Delivery_Delivery { get; set; }
        [ForeignKey("SaleId")]
        public virtual Sale_Sale Sale_Sale { get; set; }
    }
}
