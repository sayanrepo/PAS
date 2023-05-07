using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BaseSite.Models.DBModel
{
    public partial class Tb_Attachments
    {
        public override string ToString()
        {
            string s = "";
            s += "نام ملحقات: " + this.Name + "\tشرح: " + this.Description + "\n\r";
            s += "قیمت: " + this.Cost.ToString() + "\tضریب: " + this.ProductFactor.ToString() + "\tموجود: " + (this.Available ? "هست" : "نیست");
            return s;
        }
    }

    public partial class Tb_PushButtons
    {
        public override string ToString()
        {
            string s = "";
            s += "نام پوش باتون: " + this.Name + "\tشرح: " + this.Description + "\n\r";
            s += "قیمت: " + this.Cost.ToString() + "\tضریب: " + this.ProductFactor.ToString() + "\tموجود: " + (this.Available.HasValue && this.Available.Value ? "هست" : "نیست");
            return s;
        }
    }

    public partial class Tb_Monitors
    {
        public override string ToString()
        {
            string s = "";
            s += "نام نمایشگر: " + this.Name + "\tشرح: " + this.Description + "\n\r";
            s += "قیمت: " + this.Cost.ToString() + "\tضریب: " + this.ProductFactor.ToString() + "\tموجود: " + (this.Available.HasValue && this.Available.Value ? "هست" : "نیست");
            return s;
        }
    }

    public partial class Tb_SurfaceMetals
    {
        [NotMapped]
        public string FullName
        {
            get { return string.Format("{0}{1}", string.IsNullOrWhiteSpace(this.Name) ? "" : this.Name, string.IsNullOrWhiteSpace(this.Description) ? "" : "(" + this.Description + ")"); }
            set { }
        }

        public override string ToString()
        {
            string s = "";
            s += "نام فلزرویه: " + this.Name + "\tشرح: " + this.Description + "\n\r";
            s += "قیمت: " + this.Cost.ToString() + "\tضریب: " + this.ProductFactor.ToString() + "\tموجود: " + (this.Available.HasValue && this.Available.Value ? "هست" : "نیست");
            return s;
        }
    }

    public partial class Tb_CabinPanels
    {
        public override string ToString()
        {
            string s = "";
            s += "نام پنل داخل کابین: " + this.Name + "\tشرح: " + this.Description + "\n\r";
            s += "قیمت: " + this.Cost.ToString() + "\tضریب: " + this.ProductFactor.ToString() + "\tموجود: " + (this.Available.HasValue && this.Available.Value ? "هست" : "نیست") + "\n\r";
            s += "شروع تولید از: " + Models.Cache.Order_ProductStatus[this.StartFrom] + "\tضریب مصرف فلزرویه: " + this.SurfaceArea.ToString();
            return s;
        }
    }

    public partial class Tb_HallPanels
    {
        public override string ToString()
        {
            string s = "";
            s += "نام پنل طبقات: " + this.Name + "\tشرح: " + this.Description + "\n\r";
            s += "قیمت: " + this.Cost.ToString() + "\tضریب: " + this.ProductFactor.ToString() + "\tموجود: " + (this.Available.HasValue && this.Available.Value ? "هست" : "نیست") + "\n\r";
            s += "شروع تولید از: " + Models.Cache.Order_ProductStatus[this.StartFrom] + "\tضریب مصرف فلزرویه: " + this.SurfaceArea.ToString();
            return s;
        }
    }

    public partial class Tb_DoorTopPanels
    {
        public override string ToString()
        {
            string s = "";
            s += "نام پنل سردرب: " + this.Name + "\tشرح: " + this.Description + "\n\r";
            s += "قیمت: " + this.Cost.ToString() + "\tضریب: " + this.ProductFactor.ToString() + "\tموجود: " + (this.Available.HasValue && this.Available.Value ? "هست" : "نیست") + "\n\r";
            s += "شروع تولید از: " + Models.Cache.Order_ProductStatus[this.StartFrom] + "\tضریب مصرف فلزرویه: " + this.SurfaceArea.ToString();
            return s;
        }
    }
}