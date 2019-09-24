using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.ConfigAdapter;
using Com.Ctrip.Framework.Apollo.Enums;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Apollo.AspNetCore.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            YamlConfigAdapter.Register();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
               //.ConfigureAppConfiguration((context, builder) => builder //Using environment variables, command lines, etc., it is recommended that Docker run in this way
               //     .AddApollo(context.Configuration.GetSection("apollo"))
               .ConfigureAppConfiguration(builder => builder //Ordinary, generally configured in appsettings. JSON
                   .AddApollo(builder.Build().GetSection("apollo"))
                    .AddDefault(ConfigFileFormat.Xml)
                    .AddDefault(ConfigFileFormat.Json)
                    .AddDefault(ConfigFileFormat.Yml)
                    .AddDefault(ConfigFileFormat.Yaml)
                    .AddDefault())
                .UseStartup<Startup>();
    }
}
