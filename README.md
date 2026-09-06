# BaseSite

این راهکار، برنامه قدیمی ASP.NET MVC را به دو برنامه اجرایی مستقل روی .NET 10 تقسیم می‌کند:

- `BaseSite.Api`: وب API، احراز هویت توکنی و دسترسی به داده‌ها و منطق دامنه موجود.
- `BaseSite.Web`: Blazor Web App با حالت Interactive Server، رابط راست‌به‌چپ و MudBlazor.

پروژه‌های `AppHost`، `ServiceDefaults` و `ReportGateway` زیرساخت اجرای Aspire و گزارش‌های موجود هستند. `AppHost` هر دو برنامه جدید را با وابستگی صحیح اجرا می‌کند.

## اجرا

برای اجرای یکپارچه، پروژه `AppHost` را اجرا کنید. برای اجرای جداگانه نیز ابتدا API و سپس Web را اجرا کنید:

```powershell
dotnet run --project BaseSite/BaseSite.Api.csproj
dotnet run --project BaseSite.Web/BaseSite.Web.csproj --launch-profile http
```

آدرس پیش‌فرض API در اجرای مستقل `http://localhost:5184` و آدرس Web برابر `http://localhost:5074` است. مقدار `BaseSiteApi:BaseUrl` را می‌توان در تنظیمات محیط مقصد تغییر داد.

## مرزبندی پروژه‌ها

مدل‌ها، EF6 و مدیرهای دامنه فعلی در API نگه داشته شده‌اند. فایل‌های View و assetهای MVC قدیمی از build و publish پروژه API حذف شده‌اند و فقط برای مراجعه هنگام انتقال جزئیات فرم‌های قدیمی در مخزن باقی مانده‌اند. UI جدید هیچ reference مستقیمی به API یا EF ندارد و صرفاً با قراردادهای HTTP کار می‌کند.
