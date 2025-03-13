using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utils
{
    internal sealed class AppSettingsSingleton
    {
        private static readonly Lazy<AppSettingsSingleton> _instance = new Lazy<AppSettingsSingleton>(() => new AppSettingsSingleton());
        public static AppSettingsSingleton Instance => _instance.Value;

        public IConfiguration Configuration { get; private set; }

        private AppSettingsSingleton() {
            Configuration = null!;
        }

        public void Initialize(IConfiguration config)
        {
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
        }
    }
}
