#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models.DBModel;
using System.Globalization;

namespace BaseSite.Api.Queries;

public static class OrderDetails
{
    public static OrderDetail Map(Order_Order order) => new()
    {
        Summary = new OrderSummary
        {
            Id = order.Id, DocumentNumber = order.DocNumber, FactorNumber = order.FactorNumber,
            Receiver = order.Account_Users1?.FullName ?? "", Customer = order.Account_Users?.FullName ?? "",
            ProjectName = order.ProjectName ?? "", OrderDate = order.DateOrder, FactorDate = order.DateFactor,
            Amount = order.Cost, Status = order.Order_Status?.Name ?? "", TradeType = order.Tb_TradeTypes?.Name ?? "", HasTax = order.Tax > 0
        },
        ClienteleName = order.ClienteleName ?? "", DeliveryAddress = order.DeliveryAddress ?? "",
        PackType = order.Tb_PackTypes?.Name ?? "", ElevatorBoard = order.Tb_ElevatorBoards?.Name ?? "",
        Comment = order.Comment ?? "", ProductionRequestDate = order.DateDelivery,
        DeliveryCost = order.DeliveryCost, TaxPercent = order.Tax, DiscountPercent = order.DiscountRate,
        CabinTotal = order.SumCostPanel, HallTotal = order.SumCostHall, DoorTopTotal = order.SumCostDoorTop,
        AttachmentTotal = order.SumCostAttachment, AdditionTotal = order.SumCostAddition,
        DeductionTotal = order.SumCostDeduction, TaxTotal = order.SumCostTax, DiscountTotal = order.SumCostDiscountRate,
        Panels = order.Order_Cabin.OrderBy(x => x.Id).Select(Cabin)
            .Concat(order.Order_Hall.OrderBy(x => x.Id).Select(Hall))
            .Concat(order.Order_DoorTop.OrderBy(x => x.Id).Select(DoorTop)).ToList(),
        Deductions = order.Order_Deduction.Select(x => new OrderExtra { Kind = "کسورات", Name = x.Tb_Deductions?.Name ?? "", Amount = x.Cost }).ToList()
    };

    private static OrderPanel Cabin(Order_Cabin x) => new()
    {
        Kind = "پنل کابین", Name = x.Tb_CabinPanels?.Name ?? "", DocumentNumber = x.DocNumber, Count = x.Count, Amount = x.Cost,
        Specifications = [
            Spec("پوش باتون", x.Tb_PushButtons?.Name), Spec("نمایشگر", x.Tb_Monitors?.Name),
            Spec("فلز رویه", x.Tb_CabinSurfaceMetals?.Name), Spec("فلز رویه دوم", x.Tb_CabinSurfaceMetals1?.Name),
            Spec("نوع نصب", x.Tb_InstallationTypes?.Name), Spec("گوینده", x.Tb_Speakers?.Name), Spec("روشنایی اضطراری", x.EmergencyLigh?.Name),
            Spec("تعداد طبقات", x.FloorCount), Spec("نام طبقات", x.FloorNames), Spec("تعداد زیرزمین", x.UGFloorCount), Spec("نام زیرزمین‌ها", x.UGFloorNames),
            Spec("کلید تلفن", x.PhoneCallButton), Spec("کلید باز کردن درب", x.DO), Spec("کلید بستن درب", x.DC),
            Spec("شماره ورق", x.SheetNumber), Spec("متن برش لیزر", x.LaserCuttingText), Spec("متن حک لیزر", x.LaserEngravingText),
            Spec("وضعیت تولید", x.Order_ProductStatus?.Name), Spec("اولویت تولید", x.ProductPriority), Spec("توضیحات", x.Comment), Spec("توضیحات تحویل", x.DeliveryComment)
        ],
        Extras = Extras(x.Order_Panel_Attachment, x.Order_Panel_Addition)
    };

    private static OrderPanel Hall(Order_Hall x) => new()
    {
        Kind = "پنل طبقات", Name = x.Tb_HallPanels?.Name ?? "", DocumentNumber = x.DocNumber, Count = x.Count, Amount = x.Cost,
        Specifications = [
            Spec("پوش باتون", x.Tb_PushButtons?.Name), Spec("نمایشگر", x.Tb_Monitors?.Name), Spec("فلز رویه", x.Tb_HallSurfaceMetals?.Name),
            Spec("آسانسور", x.Tb_ElevatorCounts?.Name), Spec("تعداد شاسی", x.Tb_HallPushButtonCounts?.Name),
            Spec("تعداد طبقات", x.FloorCount), Spec("نام طبقات", x.FloorNames), Spec("تعداد زیرزمین", x.UGFloorCount), Spec("نام زیرزمین‌ها", x.UGFloorNames),
            Spec("وضعیت تولید", x.Order_ProductStatus?.Name), Spec("اولویت تولید", x.ProductPriority), Spec("توضیحات", x.Comment), Spec("توضیحات تحویل", x.DeliveryComment)
        ],
        Extras = Extras(x.Order_Panel_Attachment, x.Order_Panel_Addition)
    };

    private static OrderPanel DoorTop(Order_DoorTop x) => new()
    {
        Kind = "پنل سردرب", Name = x.Tb_DoorTopPanels?.Name ?? "", DocumentNumber = x.DocNumber, Count = x.Count, Amount = x.Cost,
        Specifications = [
            Spec("نمایشگر", x.Tb_Monitors?.Name), Spec("فلز رویه", x.Tb_SurfaceMetals?.Name), Spec("مصرف فلز رویه", x.SurfaceMetalDosage),
            Spec("وضعیت تولید", x.Order_ProductStatus?.Name), Spec("اولویت تولید", x.ProductPriority), Spec("توضیحات", x.Comment), Spec("توضیحات تحویل", x.DeliveryComment)
        ],
        Extras = Extras(x.Order_Panel_Attachment, x.Order_Panel_Addition)
    };

    private static OrderSpec Spec(string label, object? value) => new()
    {
        Label = label,
        Value = value switch { null => "—", bool b => b ? "بله" : "خیر", IFormattable f => f.ToString(null, CultureInfo.InvariantCulture), _ => value.ToString() ?? "—" }
    };

    private static List<OrderExtra> Extras(IEnumerable<Order_Panel_Attachment> attachments, IEnumerable<Order_Panel_Addition> additions) =>
        attachments.Select(x => new OrderExtra { Kind = "ملحقات", Name = x.Tb_Attachments?.Name ?? "", Count = x.Count, Amount = x.Cost })
            .Concat(additions.Select(x => new OrderExtra { Kind = "اضافات", Name = x.Tb_Additions?.Name ?? "", Amount = x.Cost })).ToList();
}
