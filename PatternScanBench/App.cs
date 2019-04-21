using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PatternScanBench
{
    class App
    {
        [STAThreadAttribute]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var embeddedResources = new List<string>(assembly.GetManifestResourceNames());
                string assemblyName = new AssemblyName(args.Name).Name;
                string fileName = string.Format("{0}.dll", assemblyName);
                string resourceName = embeddedResources.FirstOrDefault(ern => ern.EndsWith(fileName));
                if (!string.IsNullOrWhiteSpace(resourceName))
                {
                    using (var stream = assembly.GetManifestResourceStream(resourceName))
                    {
                        Byte[] assemblyData = new Byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }
                }

                return null;
            };

            Program.Init(null);
        }
    }
}
