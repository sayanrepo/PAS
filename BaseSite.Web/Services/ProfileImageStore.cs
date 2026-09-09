namespace BaseSite.Web.Services;

public sealed class ProfileImageStore(IWebHostEnvironment environment)
{
    public const int MaxBytes = 2 * 1024 * 1024;

    public static string GetExtension(byte[] bytes)
    {
        if (bytes.Length == 0 || bytes.Length > MaxBytes)
            throw new InvalidOperationException("حجم تصویر باید بین یک بایت و ۲ مگابایت باشد.");

        var data = bytes.AsSpan();
        if (data.Length >= 8 && data[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
            return "png";
        if (data.Length >= 3 && data[0] == 255 && data[1] == 216 && data[2] == 255)
            return "jpg";
        if (data.Length >= 12 && data[..4].SequenceEqual("RIFF"u8) && data.Slice(8, 4).SequenceEqual("WEBP"u8))
            return "webp";

        throw new InvalidOperationException("لطفاً تصویر JPG، PNG یا WebP انتخاب کنید.");
    }

    public async Task<string> SaveAsync(int userId, byte[] bytes)
    {
        if (userId <= 0)
            throw new InvalidOperationException("برای تغییر عکس وارد حساب کاربری شوید.");
        var extension = GetExtension(bytes);
        var directory = Path.Combine(environment.WebRootPath, "Images", "System");
        Directory.CreateDirectory(directory);
        var fileName = $"{userId}_{Guid.NewGuid():N}.{extension}";
        await File.WriteAllBytesAsync(Path.Combine(directory, fileName), bytes);
        return fileName;
    }
}
