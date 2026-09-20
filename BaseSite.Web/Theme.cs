using MudBlazor;

namespace BaseSite.Web;

public static class Theme
{
    private static readonly string[] AppFontFamily = ["IRANSans", "Tahoma", "Arial", "sans-serif"];

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
            Default = new DefaultTypography { FontFamily = AppFontFamily },
            H1 = new H1Typography { FontFamily = AppFontFamily },
            H2 = new H2Typography { FontFamily = AppFontFamily },
            H3 = new H3Typography { FontFamily = AppFontFamily },
            H4 = new H4Typography { FontFamily = AppFontFamily },
            H5 = new H5Typography { FontFamily = AppFontFamily },
            H6 = new H6Typography { FontFamily = AppFontFamily },
            Subtitle1 = new Subtitle1Typography { FontFamily = AppFontFamily },
            Subtitle2 = new Subtitle2Typography { FontFamily = AppFontFamily },
            Body1 = new Body1Typography { FontFamily = AppFontFamily },
            Body2 = new Body2Typography { FontFamily = AppFontFamily },
            Button = new ButtonTypography { FontFamily = AppFontFamily },
            Caption = new CaptionTypography { FontFamily = AppFontFamily },
            Overline = new OverlineTypography { FontFamily = AppFontFamily }
        }
    };
}
