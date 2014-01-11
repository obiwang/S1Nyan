using System.Reflection;
using System.Xml;

namespace S1Nyan.Utils
{
    public class VersionHelper
    {
        public static bool IsBeta { get; private set; }
        public static string CopyRight { get; private set; }
        public static string Version { get; private set; }

        static VersionHelper()
        {
            IsBeta = GetManifestAttributeValue("ProductID") == "1ba57ae3-e568-43a4-b907-f4d89c539de2";

            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
                CopyRight = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;

            var version = new AssemblyName(Assembly.GetExecutingAssembly().FullName).Version;
            Version = string.Format("v {0}.{1}.{2}", version.Major, version.Minor, version.Build);
            if (IsBeta)
            {
                Version = string.Format("{0}.{1}", version.Revision);
            }
        }

        public static string GetManifestAttributeValue(string attributeName)
        {
            var xmlReaderSettings = new XmlReaderSettings
            {
                XmlResolver = new XmlXapResolver()
            };

            using (var xmlReader = XmlReader.Create("WMAppManifest.xml", xmlReaderSettings))
            {
                xmlReader.ReadToDescendant("App");

                return xmlReader.GetAttribute(attributeName);
            }
        }
    }


}