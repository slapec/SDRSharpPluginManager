using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDRSharpPluginManager {
    class Consts {
        public static string ConfigFileName = "SDRSharp.exe.Config";
        public static string[] RequiredFiles = {ConfigFileName, 
                                                "SDRSharp.Common.dll",
                                                "SDRSharp.Radio.dll"};

        public static string PluginInterfaceName = "ISharpPlugin";
        public static string DisplayNameFieldName = "DisplayName";

        public static string PluginFileFilter = "SDRSharp Plugins (*.dll)|*.dll";
        public static string sharpPluginsRootXPath = "//sharpPlugins";
        public static string pluginElementName = sharpPluginsRootXPath + "/add";
    }
}
