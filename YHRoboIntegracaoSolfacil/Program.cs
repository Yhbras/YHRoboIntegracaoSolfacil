using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using YHRoboIntegracaoSolfacil.Models;
using YHRoboIntegracaoSolfacil.Services;

namespace YHRoboIntegracaoSolfacil
{
    internal class Program
    {
        private static Connections? _connection;
        private static Settings? _settings;

        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile($"appsettings.json");
            var configuration = builder.Build();

            _connection = new Connections();
            new ConfigureFromConfigurationOptions<Connections>(
                configuration.GetSection("ConnectionSettings"))
                    .Configure(_connection);

            _settings = new Settings();
            new ConfigureFromConfigurationOptions<Settings>(
                configuration.GetSection("Settings"))
                    .Configure(_settings);

            _ = YHRoboIntegracaoSolfacilServices.BuscarTabulacoes(_connection.YHBconn);
        }


    }
}
