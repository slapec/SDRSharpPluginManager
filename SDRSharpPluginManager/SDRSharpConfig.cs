using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace SDRSharpPluginManager {
    class PluginManager {
        // Sorry, I'm still not so familiar with C# types so I use the same types I'd use in Python
        public static string sharpPluginsRootXPath = "//sharpPlugins";
        public static string pluginElementName = sharpPluginsRootXPath + "/add";

        private string basePath;
        private XmlDocument externalPluginFile;
        private XmlDocument configFile;
        private Dictionary<string, XmlNode> sharpPluginNodes;
        private HashSet<XmlDocument> modifiedFiles = new HashSet<XmlDocument>();
        
        public PluginManager(String path) {
            basePath = path;
            string absolutePath = Path.Combine(basePath, PluginManagerWindow.ConfigFileName);

            configFile = new XmlDocument();
            configFile.Load(absolutePath);

            ParseSharpPlugins();
        }

        private void LoadPluginNodes(XmlNodeList nodeList) {
            foreach (XmlNode node in nodeList) {
                XmlAttributeCollection nodeAttributes = node.Attributes;
                try {
                    sharpPluginNodes.Add(nodeAttributes["key"].Value, node);
                }
                catch (ArgumentException) {
                    continue;
                }
            }
        }

        private void ParseSharpPlugins() {
            List<XmlNodeList> pluginNodeLists = new List<XmlNodeList>();
            sharpPluginNodes = new Dictionary<string, XmlNode>();
            
            XmlNode sharpPluginsRoot = configFile.SelectSingleNode(sharpPluginsRootXPath);
            XmlAttributeCollection rootAttributes = sharpPluginsRoot.Attributes;

            if (rootAttributes["configSource"] != null) {
                string externalPluginFileName = rootAttributes["configSource"].Value;

                externalPluginFile = new XmlDocument();
                externalPluginFile.Load(Path.Combine(basePath, externalPluginFileName));

                LoadPluginNodes(externalPluginFile.SelectNodes(pluginElementName));
            }
            else {
                LoadPluginNodes(configFile.SelectNodes(pluginElementName));
            }
        }

        public Dictionary<string, string> GetSharpPlugins() {
            Dictionary<string, string> plugins = new Dictionary<string, string>();
            foreach (KeyValuePair<string, XmlNode> plugin in sharpPluginNodes) {
                plugins.Add(plugin.Key, plugin.Value.Attributes["value"].Value);
            }
            return plugins;
        }

        public void RemoveSharpPlugin(string pluginName) {
            //Remove node from XML
            XmlNode pluginNode = sharpPluginNodes[pluginName];
            modifiedFiles.Add(pluginNode.OwnerDocument);

            pluginNode.ParentNode.RemoveChild(pluginNode);
            //Remove reference from dict
            sharpPluginNodes.Remove(pluginName);
        }

        public void AddSharpPlugin(string displayName, string typeName, string assemblyName) {
            XmlDocument workingFile;

            if (externalPluginFile != null) {
                workingFile = externalPluginFile;

            }
            else {
                workingFile = configFile;
            }

            XmlNode newPlugin = workingFile.CreateNode("element", "add", "");
            // Add reference to dict
            sharpPluginNodes.Add(displayName, newPlugin);

            // Add node to dom
            workingFile.SelectSingleNode("//sharpPlugins").AppendChild(newPlugin);

            XmlAttribute newPluginKey = workingFile.CreateAttribute("key");
            newPlugin.Attributes.Append(newPluginKey);
            newPluginKey.Value = displayName;

            XmlAttribute newPluginValue = workingFile.CreateAttribute("value");
            newPlugin.Attributes.Append(newPluginValue);
            newPluginValue.Value = String.Format("{0},{1}", typeName, assemblyName);
            modifiedFiles.Add(workingFile);
        }

        public void Save() {
            foreach (XmlDocument modifiedFile in modifiedFiles) {
                Uri path = new Uri(modifiedFile.BaseURI);
                modifiedFile.Save(path.LocalPath);
            }
            modifiedFiles.Clear();
        }
    }
}
