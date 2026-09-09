#nullable enable
using BaseSite.Api.Contracts;
using BaseSite.Models;
using System.Security.Claims;
namespace BaseSite.Api.Queries;
public sealed record ReportDefinition(ReportInfo Info,string Role,string? Path=null);
public static class ReportCatalog {
 public static readonly IReadOnlyList<ReportDefinition> All = [
 new(new(){Key="RCustomerBill",Title="صورتحساب مشتری خاص",Category="واحد فروش",Mode="table",CustomerFilter=true,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_CustomerBill)),
 new(new(){Key="RSaleControlling",Title="گزارش کنترلی (فروش روزانه)",Category="واحد فروش",Mode="table",CustomerFilter=false,PartFilter=false,Daily=true},nameof(OPERATIONS.Report_SaleControlling)),
 new(new(){Key="RCustomersBill",Title="صورتحساب کلی",Category="واحد فروش",Mode="table",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_CustomersBill)),
 new(new(){Key="RLending",Title="کالای امانی ما نزد دیگران",Category="واحد فروش",Mode="table",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_Lending)),
 new(new(){Key="RStatistic",Title="گزارشات آماری",Category="واحد فروش",Mode="table",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_Statistic)),
 new(new(){Key="RStatistic2",Title="گزارشات آماری - ریز مصرف",Category="واحد فروش",Mode="table",CustomerFilter=false,PartFilter=true,Daily=false},nameof(OPERATIONS.Report_Statistic2)),
 new(new(){Key="ROrders_Monthly_OrderDate",Title="گزارش ماهانه سفارشات درحال تولید یا تحویل شده",Category="واحد فروش",Mode="powerbi",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_Orders_Monthly_OrderDate),"فروش/گزارش ماهانه سفارشات درحال تولید یا تحویل شده"),
 new(new(){Key="RSales_Payments_Monthly",Title="نمودار وصول / فروش ماهانه",Category="واحد فروش",Mode="table",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_Sales_Payments_Monthly)),
 new(new(){Key="RNewCustomers",Title="نمودار تعداد مشتریان جدید در ماه",Category="شاخص‌های عملکرد",Mode="powerbi",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_KPI),"KPI/نمودار تعداد مشتریان جدید در ماه"),
 new(new(){Key="RSalesToNewCustomers",Title="نمودار نسبت فروش به مشتریان جدید و مشتریان قبلی در هر ماه",Category="شاخص‌های عملکرد",Mode="powerbi",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_KPI),"KPI/نمودار نسبت فروش به مشتریان جدید و مشتریان قبلی در هر ماه"),
 new(new(){Key="ROrder_PreparationDays",Title="مدت زمان تحویل سفارشات",Category="شاخص‌های عملکرد",Mode="powerbi",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_KPI),"KPI/مدت زمان تحویل سفارشات"),
 new(new(){Key="RSalesByUsers",Title="نمودار رتبه بندی فروش کارشناسان فروش",Category="شاخص‌های عملکرد",Mode="powerbi",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_KPI),"KPI/نمودار رتبه بندی فروش کارشناسان فروش"),
 new(new(){Key="RBuyersRank",Title="جدول رتبه بندی خرید مشتریان",Category="شاخص‌های عملکرد",Mode="powerbi",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_KPI),"KPI/جدول رتبه بندی خرید مشتریان"),
 new(new(){Key="Logs_Logs",Title="تاریخچه فعالیت‌ها",Category="واحد فروش",Mode="logs",CustomerFilter=false,PartFilter=false,Daily=true},nameof(OPERATIONS.Logs_Logs)),
 new(new(){Key="ProductFactors",Title="ضرایب کارکرد",Category="تولید",Mode="production",CustomerFilter=false,PartFilter=false,Daily=true},nameof(OPERATIONS.Report_ProductFactor)),
 new(new(){Key="RCustomersInfo",Title="جدول اطلاعات مشتریان",Category="عمومی",Mode="powerbi",CustomerFilter=false,PartFilter=false,Daily=false},nameof(OPERATIONS.Report_CustomersInfo),"عمومی/جدول اطلاعات مشتریان"),
 ];
 public static bool CanRead(ClaimsPrincipal user,ReportDefinition report) => user.IsInRole(nameof(OPERATIONS.Report)) && user.IsInRole(report.Role);
}

