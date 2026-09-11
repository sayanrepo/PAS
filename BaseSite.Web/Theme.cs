using MudBlazor;

namespace BaseSite.Web;

public static class Theme
{
    public static MudTheme AppTheme { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#3b5ccc",
            Secondary = "#d97706",
            Background = "#f4f6fa",
            Surface = "#ffffff",
            AppbarBackground = "#2d4468",
            AppbarText = "#f5f7fc",
            DrawerBackground = "#18243b",
            DrawerText = "#edf2fb",
            DrawerIcon = "#b4c7e7"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
            DrawerWidthRight = "296px"
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
