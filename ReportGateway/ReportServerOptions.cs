namespace ReportGateway
{
    public sealed class ReportServerOptions
    {
        public const string SectionName = "ReportServer";

        public string BaseUrl { get; set; } = "";

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string Domain { get; set; } = "";
    }
}
