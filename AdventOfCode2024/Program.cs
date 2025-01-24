
using AdventOfCode2024.Exceptions;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;

namespace AdventOfCode2024
{
    internal class Program
    {
        const string NAMESPACE = "AdventOfCode2024";
        const string DEFAULT_ENV = "Development";

        public static void Main()
        {
            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? DEFAULT_ENV;

            string currentDirectory = Directory.GetCurrentDirectory();
            string rootDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent?.FullName ?? throw new NullReferenceException();

            var config = new ConfigurationBuilder()
                    .SetBasePath(rootDirectory)
                    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                    .Build();


            int startingDay = config.GetValue<int>("AppSettings:StartingDay");
            int endDay = config.GetValue<int>("AppSettings:LastDay");

            Log.Logger = new LoggerConfiguration()
                           .WriteTo.Console(
                                theme: Serilog.Sinks.SystemConsole.Themes.SystemConsoleTheme.Literate,
                                outputTemplate: "{Message:lj}{NewLine}{Exception}"
                           )
                           //.WriteTo.File($"{nameSpace}.log")
                           .MinimumLevel.Debug()
                           .CreateLogger();


            for (int i = endDay; i >= startingDay; i--) {

                LaunchDay(i);
            }


        }

        private static  void LaunchDay(int i) {

            string className = $"Day{i}";
            try
            {
                string subDirName = className;
                Type dayType = Type.GetType($"{NAMESPACE}.{subDirName}.{className}") ?? throw new DayNotFoundException($"{className} class not found");
                object instance = Activator.CreateInstance(dayType) ?? throw new NullReferenceException($"{className} instance not found");
                MethodInfo method = dayType.GetMethod("Run") ?? throw new NullReferenceException($"{className} Run method not found");
                method.Invoke(instance, null);

            }
            catch (DayNotFoundException)
            {
                return;
            }
            catch (NullReferenceException ex)
            {
                Log.Error(ex.ToString());
            }
        }

    }
}
