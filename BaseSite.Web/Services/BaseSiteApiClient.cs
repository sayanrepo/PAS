using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BaseSite.Web.Models;

namespace BaseSite.Web.Services;

public sealed class BaseSiteApiClient(HttpClient httpClient, ApiSession session)
{
    public Task<BasicTable?> GetBasicTableAsync(string key,int? parentId=null,CancellationToken token=default) => GetAsync<BasicTable>("api/basic/"+Uri.EscapeDataString(key)+(parentId.HasValue?$"?parentId={parentId}":""),token);
    public Task SaveBasicAsync(string key,int id,BasicSave value) => BasicWrite(id==0?HttpMethod.Post:HttpMethod.Put,"api/basic/"+Uri.EscapeDataString(key)+(id==0?"":$"/{id}"),value);
    public Task DeleteBasicAsync(string key,int id) => BasicWrite(HttpMethod.Delete,$"api/basic/{Uri.EscapeDataString(key)}/{id}",null);
    public Task ChangeBasicPricesAsync(string key,BasicPriceChange change) => BasicWrite(HttpMethod.Post,$"api/basic/{Uri.EscapeDataString(key)}/prices",change);
    public Task<CompatibilityGrid?> GetCompatibilityAsync(int primary,int secondary) => GetAsync<CompatibilityGrid>($"api/basic/compatibility/{primary}/{secondary}",default);
    public Task SaveCompatibilityAsync(int primary,int secondary,CompatibilityCell cell) => BasicWrite(HttpMethod.Put,$"api/basic/compatibility/{primary}/{secondary}",cell);
    private async Task BasicWrite(HttpMethod method,string url,object? value) {
        using var request=CreateRequest(method,url);
        if(value is not null) request.Content=JsonContent.Create(value);
        using var response=await httpClient.SendAsync(request);
        if(response.StatusCode==HttpStatusCode.Unauthorized) await session.SignOutAsync();
        if(!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response,default));
    }
    public Task<List<ReportInfo>?> GetReportsAsync(CancellationToken token=default) => GetAsync<List<ReportInfo>>("api/reports",token);
    public Task<List<OrderLookup>?> FindReportCustomersAsync(string text,CancellationToken token=default) => GetAsync<List<OrderLookup>>("api/reports/customers?term="+Uri.EscapeDataString(text),token);
    public Task<ReportResult?> RunReportAsync(string key,ReportFilter filter,CancellationToken token=default) {
        var culture=System.Globalization.CultureInfo.InvariantCulture;
        var query=$"dateFrom={filter.DateFrom!.Value.ToString("yyyy-MM-dd",culture)}&dateTo={filter.DateTo!.Value.ToString("yyyy-MM-dd",culture)}&customerId={filter.CustomerId}&part={filter.Part}";
        return GetAsync<ReportResult>("api/reports/"+Uri.EscapeDataString(key)+"?"+query,token);
    }
    public Task<DeliveryPage?> GetDeliveriesAsync(DeliverySearch filter, CancellationToken token = default) {
        var query = new List<string> { $"page={filter.Page}", $"pageSize={filter.PageSize}" };
        if(filter.DocumentNumber.HasValue) query.Add($"documentNumber={filter.DocumentNumber}");
        if(filter.CustomerId.HasValue) query.Add($"customerId={filter.CustomerId}");
        if(filter.StatusId.HasValue) query.Add($"statusId={filter.StatusId}");
        if(!string.IsNullOrWhiteSpace(filter.Customer)) query.Add("customer="+Uri.EscapeDataString(filter.Customer));
        if(filter.DateFrom.HasValue) query.Add("dateFrom="+filter.DateFrom.Value.ToString("yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture));
        if(filter.DateTo.HasValue) query.Add("dateTo="+filter.DateTo.Value.ToString("yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture));
        return GetAsync<DeliveryPage>("api/deliveries?"+string.Join("&",query),token);
    }
    public Task<DeliveryLookups?> GetDeliveryLookupsAsync(CancellationToken token=default) => GetAsync<DeliveryLookups>("api/deliveries/lookups",token);
    public Task<List<OrderLookup>?> FindDeliveryCustomersAsync(string text,CancellationToken token=default) => GetAsync<List<OrderLookup>>("api/deliveries/customers?term="+Uri.EscapeDataString(text),token);
    public Task<DeliveryDetail?> GetDeliveryAsync(int id,CancellationToken token=default) => GetAsync<DeliveryDetail>($"api/deliveries/{id}",token);
    public Task<DeliveryDraft?> GetDeliveryDraftAsync(string kind,int number,CancellationToken token=default) => GetAsync<DeliveryDraft>($"api/deliveries/source/{Uri.EscapeDataString(kind)}/{number}",token);
    public async Task<CreatedDocument> CreateDeliveryAsync(DeliveryForm form,CancellationToken token=default) {
        using var request=CreateRequest(HttpMethod.Post,"api/deliveries");
        request.Content=JsonContent.Create(form);
        using var response=await httpClient.SendAsync(request,token);
        if(response.StatusCode==HttpStatusCode.Unauthorized) await session.SignOutAsync();
        if(!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response,token));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>(token) ?? throw new InvalidOperationException("پاسخ ثبت تحویل معتبر نیست.");
    }
    public Task<DeliveryEditor?> GetDeliveryEditorAsync(int id) => GetAsync<DeliveryEditor>($"api/deliveries/editor/{id}",default);
    public Task<OrderActivity?> GetDeliveryActivityAsync(int id) => GetAsync<OrderActivity>($"api/deliveries/{id}/activity",default);
    public async Task<OrderCommentItem> AddDeliveryCommentAsync(int id,string comment) {
        using var request=CreateRequest(HttpMethod.Post,$"api/deliveries/{id}/activity/comments");
        request.Content=JsonContent.Create(new OrderCommentRequest {Comment=comment});
        using var response=await httpClient.SendAsync(request);
        if(response.StatusCode==HttpStatusCode.Unauthorized) await session.SignOutAsync();
        if(!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response,default));
        return await response.Content.ReadFromJsonAsync<OrderCommentItem>() ?? throw new InvalidOperationException("پاسخ ثبت یادداشت معتبر نیست.");
    }
    public async Task<CreatedDocument> SaveDeliveryAsync(int id,DeliveryForm form) {
        using var request=CreateRequest(HttpMethod.Put,$"api/deliveries/editor/{id}");
        request.Content=JsonContent.Create(form);
        using var response=await httpClient.SendAsync(request);
        if(response.StatusCode==HttpStatusCode.Unauthorized) {await session.SignOutAsync();throw new InvalidOperationException("نشست شما پایان یافته است.");}
        if(!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response,default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ ذخیره تحویل معتبر نیست.");
    }
    public async Task<CreatedDocument> TransitionDeliveryAsync(int id,DeliveryTransitionRequest transition) {
        using var request=CreateRequest(HttpMethod.Post,$"api/deliveries/editor/{id}/transition");
        request.Content=JsonContent.Create(transition);
        using var response=await httpClient.SendAsync(request);
        if(response.StatusCode==HttpStatusCode.Unauthorized) {await session.SignOutAsync();throw new InvalidOperationException("نشست شما پایان یافته است.");}
        if(!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response,default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ عملیات تحویل معتبر نیست.");
    }
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ ورود از سرور معتبر نیست.");
    }

    public Task<DashboardSummary?> GetDashboardAsync(CancellationToken cancellationToken = default) =>
        GetAsync<DashboardSummary>("api/dashboard", cancellationToken);

    public Task<OrderPage?> GetOrdersAsync(OrderSearch filter, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["documentNumber"] = filter.DocumentNumber?.ToString(), ["customer"] = filter.Customer,
            ["customerId"] = filter.CustomerId?.ToString(), ["statusId"] = filter.StatusId?.ToString(),
            ["tradeTypeId"] = filter.TradeTypeId?.ToString(), ["projectName"] = filter.ProjectName,
            ["orderDateFrom"] = filter.OrderDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["orderDateTo"] = filter.OrderDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["factorDateFrom"] = filter.FactorDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["factorDateTo"] = filter.FactorDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["page"] = filter.Page.ToString(), ["pageSize"] = filter.PageSize.ToString()
        };
        return GetAsync<OrderPage>(Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/orders", query), cancellationToken);
    }

    public Task<OrderLookups?> GetOrderLookupsAsync() => GetAsync<OrderLookups>("api/orders/lookups", default);
    public Task<List<OrderLookup>?> FindOrderCustomersAsync(string term, CancellationToken cancellationToken) =>
        GetAsync<List<OrderLookup>>("api/orders/customers?term=" + Uri.EscapeDataString(term), cancellationToken);
    public Task<OrderDetail?> GetOrderAsync(int id) => GetAsync<OrderDetail>($"api/orders/{id}", default);
    public Task<OrderEditor?> GetOrderEditorAsync(int id) => GetAsync<OrderEditor>(id == 0 ? "api/orders/editor/new" : $"api/orders/editor/{id}", default);
    public Task<OrderActivity?> GetOrderActivityAsync(int id) => GetAsync<OrderActivity>($"api/orders/{id}/activity", default);
    public async Task<OrderCommentItem> AddOrderCommentAsync(int id, string comment)
    {
        using var request = CreateRequest(HttpMethod.Post, $"api/orders/{id}/activity/comments");
        request.Content = JsonContent.Create(new OrderCommentRequest { Comment = comment });
        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<OrderCommentItem>() ?? throw new InvalidOperationException("پاسخ ثبت یادداشت معتبر نیست.");
    }
    public Task<List<OrderLookup>?> FindOrderEditorCustomersAsync(string term, CancellationToken token) =>
        GetAsync<List<OrderLookup>>("api/orders/editor/customers?term=" + Uri.EscapeDataString(term), token);
    public async Task<CreatedDocument> SaveOrderAsync(int id, OrderForm form)
    {
        using var request = CreateRequest(id == 0 ? HttpMethod.Post : HttpMethod.Put,
            id == 0 ? "api/orders/editor" : $"api/orders/editor/{id}");
        request.Content = JsonContent.Create(form);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) { await session.SignOutAsync(); throw new InvalidOperationException("نشست شما پایان یافته است."); }
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ ثبت معتبر نیست.");
    }

    public Task<SalePage?> GetSalesAsync(SaleSearch filter, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["documentNumber"] = filter.DocumentNumber?.ToString(), ["customer"] = filter.Customer,
            ["customerId"] = filter.CustomerId?.ToString(), ["statusId"] = filter.StatusId?.ToString(),
            ["tradeTypeId"] = filter.TradeTypeId?.ToString(),
            ["orderDateFrom"] = filter.OrderDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["orderDateTo"] = filter.OrderDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["factorDateFrom"] = filter.FactorDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["factorDateTo"] = filter.FactorDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["page"] = filter.Page.ToString(), ["pageSize"] = filter.PageSize.ToString()
        };
        return GetAsync<SalePage>(Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/sales", query), cancellationToken);
    }

    public Task<OrderLookups?> GetSaleLookupsAsync() => GetAsync<OrderLookups>("api/sales/lookups", default);
    public Task<List<OrderLookup>?> FindSaleCustomersAsync(string term, CancellationToken cancellationToken) =>
        GetAsync<List<OrderLookup>>("api/sales/customers?term=" + Uri.EscapeDataString(term), cancellationToken);
    public Task<SaleDetail?> GetSaleAsync(int id) => GetAsync<SaleDetail>($"api/sales/{id}", default);
    public Task<SaleEditor?> GetSaleEditorAsync(int id) =>
        GetAsync<SaleEditor>(id == 0 ? "api/sales/editor/new" : $"api/sales/editor/{id}", default);
    public Task<OrderActivity?> GetSaleActivityAsync(int id) => GetAsync<OrderActivity>($"api/sales/{id}/activity", default);
    public Task<List<OrderLookup>?> FindSaleEditorCustomersAsync(string term, CancellationToken token) =>
        GetAsync<List<OrderLookup>>("api/sales/editor/customers?term=" + Uri.EscapeDataString(term), token);
    public async Task<OrderCommentItem> AddSaleCommentAsync(int id, string comment)
    {
        using var request = CreateRequest(HttpMethod.Post, $"api/sales/{id}/activity/comments");
        request.Content = JsonContent.Create(new OrderCommentRequest { Comment = comment });
        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<OrderCommentItem>() ?? throw new InvalidOperationException("پاسخ ثبت یادداشت معتبر نیست.");
    }
    public async Task<CreatedDocument> SaveSaleAsync(int id, SaleForm form)
    {
        using var request = CreateRequest(id == 0 ? HttpMethod.Post : HttpMethod.Put,
            id == 0 ? "api/sales/editor" : $"api/sales/editor/{id}");
        request.Content = JsonContent.Create(form);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) { await session.SignOutAsync(); throw new InvalidOperationException("نشست شما پایان یافته است."); }
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ ثبت معتبر نیست.");
    }

    public Task<ServicePage?> GetServicesAsync(ServiceSearch filter, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["documentNumber"] = filter.DocumentNumber?.ToString(), ["customer"] = filter.Customer,
            ["customerId"] = filter.CustomerId?.ToString(), ["statusId"] = filter.StatusId?.ToString(),

            ["orderDateFrom"] = filter.OrderDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["orderDateTo"] = filter.OrderDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["factorDateFrom"] = filter.FactorDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["factorDateTo"] = filter.FactorDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["page"] = filter.Page.ToString(), ["pageSize"] = filter.PageSize.ToString()
        };
        return GetAsync<ServicePage>(Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/services", query), cancellationToken);
    }

    public Task<OrderLookups?> GetServiceLookupsAsync() => GetAsync<OrderLookups>("api/services/lookups", default);
    public Task<List<OrderLookup>?> FindServiceCustomersAsync(string term, CancellationToken cancellationToken) =>
        GetAsync<List<OrderLookup>>("api/services/customers?term=" + Uri.EscapeDataString(term), cancellationToken);
    public Task<ServiceDetail?> GetServiceAsync(int id) => GetAsync<ServiceDetail>($"api/services/{id}", default);
    public Task<ServiceEditor?> GetServiceEditorAsync(int id) =>
        GetAsync<ServiceEditor>(id == 0 ? "api/services/editor/new" : $"api/services/editor/{id}", default);
    public Task<OrderActivity?> GetServiceActivityAsync(int id) => GetAsync<OrderActivity>($"api/services/{id}/activity", default);
    public Task<List<OrderLookup>?> FindServiceEditorCustomersAsync(string term, CancellationToken token) =>
        GetAsync<List<OrderLookup>>("api/services/editor/customers?term=" + Uri.EscapeDataString(term), token);
    public async Task<OrderCommentItem> AddServiceCommentAsync(int id, string comment)
    {
        using var request = CreateRequest(HttpMethod.Post, $"api/services/{id}/activity/comments");
        request.Content = JsonContent.Create(new OrderCommentRequest { Comment = comment });
        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<OrderCommentItem>() ?? throw new InvalidOperationException("پاسخ ثبت یادداشت معتبر نیست.");
    }
    public async Task<CreatedDocument> SaveServiceAsync(int id, ServiceForm form)
    {
        using var request = CreateRequest(id == 0 ? HttpMethod.Post : HttpMethod.Put,
            id == 0 ? "api/services/editor" : $"api/services/editor/{id}");
        request.Content = JsonContent.Create(form);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) { await session.SignOutAsync(); throw new InvalidOperationException("نشست شما پایان یافته است."); }
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ ثبت معتبر نیست.");
    }
    public Task<NewDocumentOptions?> GetNewDocumentOptionsAsync(string kind) => GetAsync<NewDocumentOptions>($"api/new-documents/{kind}/options", default);
    public Task<List<OrderLookup>?> FindNewDocumentCustomersAsync(string kind, string term, CancellationToken token) =>
        GetAsync<List<OrderLookup>>($"api/new-documents/{kind}/customers?term=" + Uri.EscapeDataString(term), token);
    public async Task<CreatedDocument> CreateDocumentAsync(string kind, NewDocumentRequest model)
    {
        using var request = CreateRequest(HttpMethod.Post, $"api/new-documents/{kind}");
        request.Content = JsonContent.Create(model);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) { await session.SignOutAsync(); throw new InvalidOperationException("نشست شما پایان یافته است."); }
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ ثبت معتبر نیست.");
    }
    public Task<PaymentPage?> GetPaymentsAsync(PaymentSearch filter, CancellationToken token = default)
    {
        var query = new Dictionary<string, string?> {
            ["documentNumber"] = filter.DocumentNumber?.ToString(), ["customer"] = filter.Customer,
            ["customerId"] = filter.CustomerId?.ToString(), ["statusId"] = filter.StatusId?.ToString(),
            ["paymentTypeId"] = filter.PaymentTypeId?.ToString(), ["babatId"] = filter.BabatId?.ToString(),
            ["documentDateFrom"] = filter.DocumentDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["documentDateTo"] = filter.DocumentDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["dueDateFrom"] = filter.DueDateFrom?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["dueDateTo"] = filter.DueDateTo?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            ["page"] = filter.Page.ToString(), ["pageSize"] = filter.PageSize.ToString()
        };
        return GetAsync<PaymentPage>(Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/payments", query), token);
    }
    public Task<PaymentLookups?> GetPaymentLookupsAsync() => GetAsync<PaymentLookups>("api/payments/lookups", default);
    public Task<List<OrderLookup>?> FindPaymentCustomersAsync(string term, CancellationToken token) =>
        GetAsync<List<OrderLookup>>("api/payments/customers?term=" + Uri.EscapeDataString(term), token);
    public Task<PaymentDetail?> GetPaymentAsync(int id) => GetAsync<PaymentDetail>($"api/payments/{id}", default);
    public Task<PaymentEditor?> GetPaymentEditorAsync(int id) =>
        GetAsync<PaymentEditor>(id == 0 ? "api/payments/editor/new" : $"api/payments/editor/{id}", default);
    public Task<OrderActivity?> GetPaymentActivityAsync(int id) => GetAsync<OrderActivity>($"api/payments/{id}/activity", default);
    public Task<List<OrderLookup>?> FindPaymentEditorCustomersAsync(string term, CancellationToken token) =>
        GetAsync<List<OrderLookup>>("api/payments/editor/customers?term=" + Uri.EscapeDataString(term), token);
    public async Task<OrderCommentItem> AddPaymentCommentAsync(int id, string comment)
    {
        using var request = CreateRequest(HttpMethod.Post, $"api/payments/{id}/activity/comments");
        request.Content = JsonContent.Create(new OrderCommentRequest { Comment = comment });
        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<OrderCommentItem>() ?? throw new InvalidOperationException("پاسخ ثبت یادداشت معتبر نیست.");
    }
    public async Task<CreatedDocument> SavePaymentAsync(PaymentForm form)
    {
        using var request = CreateRequest(HttpMethod.Post, "api/payments/editor");
        request.Content = JsonContent.Create(form);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) { await session.SignOutAsync(); throw new InvalidOperationException("نشست شما پایان یافته است."); }
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ ثبت معتبر نیست.");
    }
    public async Task<CreatedDocument> TransitionPaymentAsync(int id, PaymentTransitionRequest transition)
    {
        using var request = CreateRequest(HttpMethod.Post, $"api/payments/editor/{id}/transition");
        request.Content = JsonContent.Create(transition);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) { await session.SignOutAsync(); throw new InvalidOperationException("نشست شما پایان یافته است."); }
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ عملیات معتبر نیست.");
    }
    public async Task<CreatedDocument> CreatePaymentAsync(NewPaymentRequest model)
    {
        using var request = CreateRequest(HttpMethod.Post, "api/payments");
        request.Content = JsonContent.Create(model);
        using var response = await httpClient.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.Unauthorized) { await session.SignOutAsync(); throw new InvalidOperationException("نشست شما پایان یافته است."); }
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException(await ReadErrorAsync(response, default));
        return await response.Content.ReadFromJsonAsync<CreatedDocument>() ?? throw new InvalidOperationException("پاسخ ثبت معتبر نیست.");
    }
    public Task<CurrentUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default) =>
        GetAsync<CurrentUser>("api/auth/me", cancellationToken);

    public Task<LoginResponse> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default) =>
        UpdateAccountAsync("api/auth/change-password", request, cancellationToken);

    public Task<LoginResponse> ChangeImageAsync(string imagePath, CancellationToken cancellationToken = default) =>
        UpdateAccountAsync("api/auth/change-image", new { imagePath }, cancellationToken);

    private async Task<LoginResponse> UpdateAccountAsync<T>(string url, T payload, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Post, url);
        request.Content = JsonContent.Create(payload);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await session.SignOutAsync();
            throw new InvalidOperationException("نشست شما پایان یافته است. دوباره وارد شوید.");
        }
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ ذخیره حساب کاربری معتبر نیست.");
    }

    public Task<List<DocumentSummary>?> GetDocumentsAsync(
        string kind,
        int? documentNumber,
        string? customer,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (documentNumber.HasValue) query.Add($"documentNumber={documentNumber.Value}");
        if (!string.IsNullOrWhiteSpace(customer)) query.Add($"customer={Uri.EscapeDataString(customer)}");
        var url = $"api/documents/{Uri.EscapeDataString(kind)}" + (query.Count == 0 ? string.Empty : "?" + string.Join("&", query));
        return GetAsync<List<DocumentSummary>>(url, cancellationToken);
    }

    public Task<PersonOrganizationPage?> GetCustomersAsync(PersonOrganizationSearch filter, CancellationToken cancellationToken = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["term"] = filter.Term,
            ["partnerTypeId"] = filter.PartnerTypeId?.ToString(),
            ["countryId"] = filter.CountryId?.ToString(),
            ["provinceId"] = filter.ProvinceId?.ToString(),
            ["cityId"] = filter.CityId?.ToString(),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        return GetAsync<PersonOrganizationPage>(Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/customers", query), cancellationToken);
    }

    public Task<PersonOrganizationLookups?> GetPersonOrganizationLookupsAsync(CancellationToken token = default) =>
        GetAsync<PersonOrganizationLookups>("api/customers/lookups", token);

    public Task<PersonOrganizationDetails?> GetPersonOrganizationAsync(int id, CancellationToken token = default) =>
        GetAsync<PersonOrganizationDetails>($"api/customers/{id}", token);

    public Task<PersonOrganizationDetails> SavePersonOrganizationAsync(int id, PersonOrganizationSave value, CancellationToken token = default) =>
        SendAsync<PersonOrganizationDetails>(id == 0 ? HttpMethod.Post : HttpMethod.Put,
            id == 0 ? "api/customers" : $"api/customers/{id}", value, token);

    public Task<CrmPersonPage?> GetCrmPeopleAsync(CrmPersonSearch filter, CancellationToken token = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["term"] = filter.Term,
            ["partnerTypeId"] = filter.PartnerTypeId?.ToString(),
            ["countryId"] = filter.CountryId?.ToString(),
            ["provinceId"] = filter.ProvinceId?.ToString(),
            ["cityId"] = filter.CityId?.ToString(),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        var url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/crm/people", query);
        return GetAsync<CrmPersonPage>(url, token);
    }

    public Task<CrmPersonLookups?> GetCrmPersonLookupsAsync(CancellationToken token = default) =>
        GetAsync<CrmPersonLookups>("api/crm/people/lookups", token);

    public Task<CrmPersonProfile?> GetCrmPersonAsync(int id, CancellationToken token = default) =>
        GetAsync<CrmPersonProfile>($"api/crm/people/{id}", token);

    public Task<PersonNote> AddCrmPersonNoteAsync(int id, string comment, CancellationToken token = default) =>
        SendAsync<PersonNote>(HttpMethod.Post, $"api/crm/people/{id}/notes", new { comment }, token);

    public Task<SystemUserPage?> GetSystemUsersAsync(SystemUserSearch filter, CancellationToken token = default)
    {
        var query = new Dictionary<string, string?>
        {
            ["term"] = filter.Term,
            ["roleId"] = filter.RoleId?.ToString(),
            ["statusId"] = filter.StatusId?.ToString(),
            ["departmentId"] = filter.DepartmentId?.ToString(),
            ["page"] = filter.Page.ToString(),
            ["pageSize"] = filter.PageSize.ToString()
        };
        var url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString("api/system-users", query);
        return GetAsync<SystemUserPage>(url, token);
    }

    public Task<SystemUserLookups?> GetSystemUserLookupsAsync(CancellationToken token = default) =>
        GetAsync<SystemUserLookups>("api/system-users/lookups", token);

    public Task<List<NamedLookup>?> FindEligibleSystemUsersAsync(string term, CancellationToken token = default) =>
        GetAsync<List<NamedLookup>>("api/system-users/eligible-people?term=" + Uri.EscapeDataString(term ?? string.Empty), token);

    public Task<SystemUserEditor?> GetSystemUserAsync(int personId, CancellationToken token = default) =>
        GetAsync<SystemUserEditor>($"api/system-users/{personId}", token);

    public Task<SystemUserEditor> SaveSystemUserAsync(int personId, SystemUserSave value, CancellationToken token = default) =>
        SendAsync<SystemUserEditor>(HttpMethod.Put, $"api/system-users/{personId}", value, token);

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        using var request = CreateRequest(HttpMethod.Post, "api/auth/logout");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        await session.SignOutAsync();
    }

    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, url);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await session.SignOutAsync();
            return default;
        }
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string url, object value, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(method, url);
        request.Content = JsonContent.Create(value);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await session.SignOutAsync();
            throw new InvalidOperationException("نشست شما پایان یافته است. دوباره وارد شوید.");
        }
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ سرور معتبر نیست.");
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        if (!string.IsNullOrWhiteSpace(session.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        return request;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == HttpStatusCode.Forbidden) return "شما دسترسی لازم برای این عملیات را ندارید.";
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var json = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (json.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? "خطا در ارتباط با سرور.";
        }
        catch (JsonException)
        {
        }

        return "خطا در ارتباط با سرور.";
    }
}
