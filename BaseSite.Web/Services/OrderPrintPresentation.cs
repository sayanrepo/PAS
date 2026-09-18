using BaseSite.Web.Models;

namespace BaseSite.Web.Services;

public static class OrderPrintPresentation
{
    // Separate items shown on their own rows from the panel amount, preserving
    // the legacy invoice/bill rules. Only this request's snapshot is modified.
    public static void Prepare(OrderPrintData order, string kind)
    {
        if (kind == "specification") return;
        double Extras(List<PrintAttachment> attachments, List<PrintAddition> additions) =>
            kind == "bill"
                ? attachments.Where(x => x.Tb_Attachments.Id is 5 or 65).Sum(x => x.Cost)
                : attachments.Where(x => x.Tb_Attachments.Id > 0).Sum(x => x.Cost)
                    + additions.Where(x => x.Tb_Additions.Id > 0).Sum(x => x.Cost);

        // The original templates only split cabin/hall rows when their first panel is active.
        if (order.Order_Cabin.FirstOrDefault()?.Tb_CabinPanels.Id > 0)
            foreach (var panel in order.Order_Cabin.Where(x => x.Tb_CabinPanels.Id > 0))
                panel.Cost -= Extras(panel.Order_Panel_Attachment, panel.Order_Panel_Addition)
                    + (kind == "invoice" && panel.MonitorId > 0 ? panel.CostMonitor * panel.Count : 0);
        if (order.Order_Hall.FirstOrDefault()?.Tb_HallPanels.Id > 0)
            foreach (var panel in order.Order_Hall.Where(x => x.Tb_HallPanels.Id > 0))
                panel.Cost -= Extras(panel.Order_Panel_Attachment, panel.Order_Panel_Addition);
        foreach (var panel in order.Order_DoorTop.Where(x => x.Tb_DoorTopPanels.Id > 0))
            panel.Cost -= Extras(panel.Order_Panel_Attachment, panel.Order_Panel_Addition);
    }
}

public static class PrintFormatting
{
    public static string WithMaxLength(string? value, int length) =>
        value is null ? "" : value[..Math.Min(value.Length, length)];
}
