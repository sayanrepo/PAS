using MudBlazor;

namespace BaseSite.Web;

public static class Theme
{
    public static MudTheme AppTheme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#0f766e",
            Secondary = "#d97706",
            Background = "#f4f7f6",
            Surface = "#ffffff",
            AppbarBackground = "#ffffff",
            AppbarText = "#12332f",
            DrawerBackground = "#103c37",
            DrawerText = "#e7f7f4",
            DrawerIcon = "#9dd9d1"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthLeft = "272px"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Vazirmatn", "Tahoma", "Arial", "sans-serif"]
            }
        }
    };
}
