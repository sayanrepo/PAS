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

آدرس پیش‌فرض API در اجرای مستقل `https://localhost:58998` یا `http://localhost:58999` و آدرس Web برابر `http://localhost:5074` است. مقدار `BaseSiteApi:BaseUrl` را می‌توان در تنظیمات محیط مقصد تغییر داد.

### نمایش و فراخوانی APIها

در محیط `Development`، مستندات تعاملی Scalar در مسیر `/scalar` و سند OpenAPI در مسیر `/openapi/v1.json` در دسترس هستند. پروفایل اجرای `BaseSite.Api` مرورگر را روی Scalar باز می‌کند:

- اجرای مستقل: `https://localhost:58998/scalar` یا `http://localhost:58999/scalar`.
- اجرای Aspire: مسیر `/scalar` را به آدرس سرویس `basesite-api` در داشبورد اضافه کنید.
- برای بررسی اتصال، درخواست `GET /api/status` را از Scalar اجرا کنید.
- برای APIهای محافظت‌شده، ابتدا `POST /api/auth/login` را اجرا کنید و مقدار `accessToken` پاسخ را بدون پیشوند `Bearer` در بخش Authentication مربوط به `BaseSiteBearer` وارد کنید. سپس درخواست موردنظر را اجرا کنید؛ سطح دسترسی کاربر همچنان اعمال می‌شود.

صفحه Scalar و سند OpenAPI در محیط‌های غیرتوسعه منتشر نمی‌شوند.

## تنظیمات محیط‌ها

در هر دو پروژه `BaseSite.Api` (پوشه `BaseSite`) و `BaseSite.Web`، تنظیمات مشترک در `appsettings.json` و تنظیمات هر محیط در `appsettings.Development.json` و `appsettings.Production.json` قرار دارند. ASP.NET Core فایل محیط جاری را به‌صورت خودکار روی تنظیمات مشترک اعمال می‌کند.

پروفایل‌های اجرای محلی، محیط `Development` را انتخاب می‌کنند. در این محیط، Web در اجرای مستقل به `http://localhost:58999` متصل می‌شود. هنگام اجرای Aspire، آدرس کشف‌شدهٔ سرویس API اولویت دارد.

فایل‌های `Production` با مقادیر قبلی پروژه مقداردهی شده‌اند. پیش از استقرار، اتصال دیتابیس، `Reports:GatewayUrl` و `Cors:Origins` در API و `BaseSiteApi:BaseUrl` در Web را متناسب با سرور مقصد تنظیم کنید. مقادیر محرمانه را از طریق متغیرهای محیطی مانند `ConnectionStrings__PantaEntities` وارد کنید؛ متغیرهای محیطی بر فایل‌های JSON اولویت دارند.

برای اجرای مستقل در محیط Production، پروفایل Development را غیرفعال کنید:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run --project BaseSite/BaseSite.Api.csproj --no-launch-profile
# در ترمینال جداگانه، با همان مقدار ASPNETCORE_ENVIRONMENT:
dotnet run --project BaseSite.Web/BaseSite.Web.csproj --no-launch-profile
```

در سرور نیز `ASPNETCORE_ENVIRONMENT=Production` را برای هر دو برنامه تنظیم کنید. اگر `DOTNET_ENVIRONMENT` هم تعریف شده است، مقدار آن باید با محیط انتخاب‌شده هماهنگ باشد.

## مرزبندی پروژه‌ها

مدل‌ها، EF6 و مدیرهای دامنه فعلی در API نگه داشته شده‌اند. فایل‌های View و assetهای MVC قدیمی از build و publish پروژه API حذف شده‌اند و فقط برای مراجعه هنگام انتقال جزئیات فرم‌های قدیمی در مخزن باقی مانده‌اند. UI جدید هیچ reference مستقیمی به API یا EF ندارد و صرفاً با قراردادهای HTTP کار می‌کند.

## مستندات

ساختار اولیه مستندات در پوشه `docs` قرار دارد و به‌تدریج تکمیل می‌شود:

- [کلیات سیستم](docs/overview.md): هدف سیستم، کاربران و محدوده کار.
- [بخش‌های سیستم](docs/modules/): شرح امکانات و قواعد هر بخش.
- [تصمیم‌ها](docs/decisions/): تصمیم‌های مهم و دلایل آن‌ها.

## چاپ مستقل سفارش

سه خروجی چاپ در `BaseSite.Web/Pages/Print/Orders` به‌صورت Razor Pages قرار دارند:

- `/print/orders/{id}/specification`: کارت مشخصات سفارش؛ مجوز `Order` و `Plan_Print`.
- `/print/orders/{id}/invoice`: فاکتور؛ مجوز `Order` و `Order_Print`.
- `/print/orders/{id}/bill`: صورتحساب فروش؛ همان مجوز فاکتور، با فیلدهای قابل تکمیل پیش از چاپ.

هر صفحه در درخواست HTTP خودش داده را از API دریافت می‌کند و با Layout مستقل HTML تولید می‌کند. لینک‌های چاپ در تب جدید باز می‌شوند و نوسازی صفحه به اتصال یا state صفحهٔ Blazor وابسته نیست. چاپ، آخرین اطلاعات ذخیره‌شدهٔ سفارش را نشان می‌دهد.

نشست فعلی Blazor هنگام ورود و بازیابی، یک کوکی رمزنگاری‌شدهٔ HttpOnly محدود به مسیر `/print` ایجاد می‌کند. این انتقال از مسیر `/print/session` با بررسی antiforgery انجام می‌شود. کوکی با خروج پاک می‌شود و پس از انقضای توکن معتبر نیست؛ API مجوزهای کاربر را دوباره بررسی می‌کند. در HTTPS کوکی Secure است. توکن در URL قرار نمی‌گیرد.

`GET /api/orders/{id}/print/{kind}` قرارداد JSON را با مدل‌های خروجی موجود در `BaseSite/Api/Contracts` برمی‌گرداند؛ مدل‌های متناظر دریافت و نمایش در `BaseSite.Web/Models` قرار دارند و شکل JSON آن‌ها باید همگام نگه داشته شود. تولید HTML و قالب‌های چاپ در Web انجام می‌شود. Viewهای MVC قدیمی برای مراجعه در مخزن باقی مانده‌اند و در API کامپایل نمی‌شوند.
