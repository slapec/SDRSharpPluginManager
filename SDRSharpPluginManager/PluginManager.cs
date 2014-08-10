using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace SDRSharpPluginManager {
    class Plugin {
        #region Properties
        private string displayName;
        private string typeName;
        private string assemblyName;
        private bool enabled = true;
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                enabled = value;
            }
        }

        public string DisplayName {
            get {
                return displayName;
            }
        }
        public string TypeName {
            get {
                return typeName;
            }
        }
        public string AssemblyName {
            get {
                return assemblyName;
            }
        }
        #endregion

        public Plugin(XmlNode node) {
            if (node is XmlElement) {
                ParseNode(node);
            }
            else if (node is XmlComment) {
                enabled = false;
                XmlDocument singleNodeDocument = new XmlDocument();
                singleNodeDocument.LoadXml(node.InnerText);
                ParseNode(singleNodeDocument.DocumentElement);
            }
            else {
                throw new ArgumentException("Argument must be XmlElement or XmlComment subclass of XmlNode");
            }
        }
        public Plugin(string _displayName, string _typeName, string _assemblyName) {
            displayName = _displayName;
            typeName = _typeName;
            assemblyName = _assemblyName;
        }

        public void ParseNode(XmlNode node){
            XmlAttributeCollection attributes = node.Attributes;

            // These nodes always have exactly 2 attributes
            if (attributes.Count == 2) {
                displayName = attributes["key"].Value;
                string[] descriptors = attributes["value"].Value.Split(',');
                typeName = descriptors[0];
                assemblyName = descriptors[1];
            }
            else {
                throw new ArgumentException("The given XmlElement has not enough attributes.");
            }
        }
    }
    class PluginManager {
        private string basePath;

        private XmlDocument pluginFile;
        private XmlNode pluginRootNode;

        private List<Plugin> plugins;
        public Plugin[] Plugins {
            get {
                return plugins.ToArray();
            }
        }

        private HashSet<string> pluginTypes = new HashSet<string>();
        
        public PluginManager(String path) {
            basePath = path;
            string absolutePath = Path.Combine(basePath, Consts.ConfigFileName);

            pluginFile = new XmlDocument();
            pluginFile.Load(absolutePath);

            ParseSharpPlugins();
        }

        private void ParseSharpPlugins() {
            plugins = new List<Plugin>();
            
            XmlNode pluginRoot = pluginFile.SelectSingleNode(Consts.sharpPluginsRootXPath);
            XmlAttributeCollection rootAttributes = pluginRoot.Attributes;

            if (rootAttributes["configSource"] != null) {
                // Continue parsing the external plugins.xml (newer sdr# releases)
                string externalPluginFileName = rootAttributes["configSource"].Value;

                pluginFile = new XmlDocument();
                pluginFile.Load(Path.Combine(basePath, externalPluginFileName));
                ParsePluginNodes(pluginFile.SelectSingleNode(Consts.sharpPluginsRootXPath));
            }
            else {
                // For older sdr# releases
                ParsePluginNodes(pluginRoot);
            }
        }

        private void ParsePluginNodes(XmlNode root) {
            foreach (XmlNode node in root.ChildNodes) {
                Plugin _plugin = new Plugin(node);
                if (!pluginTypes.Contains(_plugin.TypeName)) {
                    pluginTypes.Add(_plugin.TypeName);
                    plugins.Add(_plugin);
                }
            }
            pluginRootNode = root;
        }

        public void Remove(Plugin _plugin) {
            pluginTypes.Remove(_plugin.TypeName);
            plugins.Remove(_plugin);
        }

        public void Add(Plugin _plugin) {
            if (pluginTypes.Contains(_plugin.TypeName)) {
                throw new ArgumentException("Duplicate plugin type in set");
            }
            else {
                pluginTypes.Add(_plugin.TypeName);
                plugins.Add(_plugin);
            }
        }

        public void Save() {
            // Drop every old node
            pluginRootNode.RemoveAll();

            // Create new shiny nodes
            foreach (Plugin plugin in plugins) {
                XmlNode pluginNode = pluginFile.CreateNode("element", "add", "");

                XmlAttribute pluginKey = pluginFile.CreateAttribute("key");
                pluginNode.Attributes.Append(pluginKey);
                pluginKey.Value = plugin.DisplayName;

                XmlAttribute pluginValue = pluginFile.CreateAttribute("value");
                pluginNode.Attributes.Append(pluginValue);
                pluginValue.Value = String.Format("{0},{1}", plugin.TypeName, plugin.AssemblyName);

                if (plugin.Enabled) {
                    pluginRootNode.AppendChild(pluginNode);
                }
                else {
                    XmlComment pluginToComment = pluginFile.CreateComment(pluginNode.OuterXml);
                    pluginRootNode.AppendChild(pluginToComment);
                }
            }

            // Write changes
            Uri path = new Uri(pluginFile.BaseURI);
            pluginFile.Save(path.LocalPath);
        }
    }
}
