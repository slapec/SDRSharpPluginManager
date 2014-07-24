using System;
using System.Collections.Generic;
using System.Xml;

namespace SDRSharpPluginManager {
    class SDRSharpConfig {
        public string sharpPluginsXPath = "//sharpPlugins/add";

        private XmlDocument externalPluginFile;
        private XmlDocument configFile;
        private Dictionary<string, XmlNode> sharpPluginNodes;
        private HashSet<XmlDocument> modifiedFiles = new HashSet<XmlDocument>();
        
        public SDRSharpConfig(String path) {
            configFile = new XmlDocument();
            configFile.Load(path);

            ParseSharpPlugins();
        }

        private void LoadPluginNodes(XmlNodeList nodeList) {
            foreach (XmlNode node in nodeList) {
                XmlAttributeCollection nodeAttributes = node.Attributes;
                sharpPluginNodes.Add(nodeAttributes["key"].Value, node);
            }
        }

        private void ParseSharpPlugins() {
            List<XmlNodeList> pluginNodeLists = new List<XmlNodeList>();
            sharpPluginNodes = new Dictionary<string, XmlNode>();
            
            XmlNode sharpPluginsRoot = configFile.SelectSingleNode("//sharpPlugins");
            XmlAttributeCollection rootAttributes = sharpPluginsRoot.Attributes;

            if (rootAttributes["configSource"] != null) {
                string externalPluginFilePath = rootAttributes["configSource"].Value;

                externalPluginFile = new XmlDocument();
                externalPluginFile.Load(externalPluginFilePath);

                LoadPluginNodes(externalPluginFile.SelectNodes(sharpPluginsXPath));
            }
            else {
                LoadPluginNodes(configFile.SelectNodes(sharpPluginsXPath));
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
            workingFile.SelectSingleNode("//sharpPlugins").AppendChild(newPlugin);

            XmlAttribute newPluginKey = workingFile.CreateAttribute("key");
            newPlugin.Attributes.Append(newPluginKey);
            newPluginKey.Value = displayName;

            XmlAttribute newPluginValue = workingFile.CreateAttribute("value");
            newPlugin.Attributes.Append(newPluginValue);
            newPluginValue.Value = String.Format("{0},{1}", typeName, assemblyName);
            modifiedFiles.Add(externalPluginFile);
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
