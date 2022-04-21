namespace Com.Ctrip.Framework.Apollo.OpenApi;

public class OpenApiOptions
{
    public Uri? PortalUrl { get; set; }

    public string? Token { get; set; }

        public int Timeout { get; set; } = 5000;
    }
}
