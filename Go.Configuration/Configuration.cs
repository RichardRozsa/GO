using System;
using System.Globalization;
using System.IO;
using MetaDapper.Engine;

namespace Go.Configuration
{
    public class Configuration
    {
        const string MetaDapperConfigXml = @"MetaDapper.Configuration.xml";
//        const string MetaDapperConfigXml = @"D:\Projects\GO\bin\Debug\MetaDapper.Configuration.xml";
        static public Root GetConfiguration(string configPath)
        {
            var logPath = AppDomain.CurrentDomain.BaseDirectory + @"\Go.log";
            using (var logStream = new FileStream(logPath, FileMode.Create, FileAccess.Write))
            {
                var directoryName = System.Environment.CurrentDirectory;
                var mdPath = Path.Combine(directoryName, MetaDapperConfigXml);

                var log = new Log(logStream);
                var ci = new CultureInfo("en-US");

                var md = new MetaDapper.Engine.MetaDapper(log, ci);
                var config = md.GetConfiguration(mdPath);

                try
                {
                    using (var src = new PersistentXmlReader(log, ci, configPath))
                    using (var dst = new PersistentPublicPropertiesWriter<Root>(log, ci))
                    {
                        string errors;
                        var errorCount = md.MapData(ref config, src, dst, true, out errors);
                        if (errorCount > 0)
                            throw new Exception(string.Format("Error: Mapping error encountered while reading configuration: {0}", errors));
                        if (dst.Data == null || dst.Data.Count == 0)
                            throw new Exception("Error: No data returned when reading configuration");
                        if (dst.RawData.Count > 1)
                            throw new Exception("Error: Unexpectedly read more than one root when reading configuration");
                        
                        return dst.Data[0];
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Error: Unexpected error while reading configuration: {0}", ex.Message), ex);
                }
            }
        }
    }
}
