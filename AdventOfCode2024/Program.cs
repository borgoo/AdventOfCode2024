
using AdventOfCode2024.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace AdventOfCode2024
{
    internal class Program
    {
        const string NAMESPACE = "AdventOfCode2024";
        const string DEFAULT_ENV = "Dev";

        public static void Main()
        {
            string? environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? DEFAULT_ENV;

            string currentDirectory = Directory.GetCurrentDirectory();
            string rootDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent?.FullName ?? throw new NullReferenceException();

            var config = new ConfigurationBuilder()
                    .SetBasePath(rootDirectory)
                    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                    .Build();


            bool lastDayAvailableOnly = config.GetValue<bool>("AppSettings:LastDayAvailableOnly");
            int startingDay = config.GetValue<int>("AppSettings:StartingDay");
            int endDay = config.GetValue<int>("AppSettings:LastDay");

           
            for (int i = endDay; i >= startingDay; i--) {

                try { 
                    LaunchDay(i);
                    if (lastDayAvailableOnly) return;
                }
                catch (DayNotFoundException)
                {
                    continue;
                }
                catch (NullReferenceException )
                {
                    throw;
                }
            }


        }

        private static  void LaunchDay(int i) {

            string className = $"Day{i}";
            string subDirName = className;
            Type dayType = Type.GetType($"{NAMESPACE}.{subDirName}.{className}") ?? throw new DayNotFoundException($"{className} class not found");
            object instance = Activator.CreateInstance(dayType) ?? throw new NullReferenceException($"{className} instance not found");
            MethodInfo method = dayType.GetMethod("Run") ?? throw new NullReferenceException($"{className} Run method not found");
            method.Invoke(instance, null);

        }

    }
}
