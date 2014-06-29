using System;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Xml;

namespace SDRSharpPluginManager {
    public partial class PluginManagerWindow : Form {
        public enum ProcessResult {
            Success = 0, 
            Missing = 1
        };
        public static String SDRSharpCommonDllFName = "SDRSharp.Common.dll";
        public static String SDRSharpExe = "SDRSharp.exe.Config";
        public static String[] RequiredFiles = {SDRSharpCommonDllFName, SDRSharpExe};

        private FolderBrowserDialog sdrSharpFolderDialog;

        public PluginManagerWindow() {
            InitializeComponent();
        }

        private ProcessResult ProcessDirectory(String path) {
            Directory.SetCurrentDirectory(path);

            foreach (String requiredFileName in RequiredFiles) {
                if (!File.Exists(requiredFileName)) {
                    String msg = String.Format("File '{0}' is not found in '{1}'", requiredFileName, path);

                    MessageBox.Show(msg, "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ProcessResult.Missing;
                }
            }

            tBoxSDRSharpPathValue.Text = path;

            this.LoadConfig();

            return ProcessResult.Success;
        }

        private void LoadConfig() {
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = SDRSharpExe;
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            // Sorry, I've had no better idea
            String sharpPluginsString = config.GetSection("sharpPlugins").SectionInformation.GetRawXml();
            XmlDocument sharpPluginsXML = new XmlDocument();
            sharpPluginsXML.LoadXml(sharpPluginsString);

            foreach (XmlNode node in sharpPluginsXML.GetElementsByTagName("add")) {
                ListViewItem pluginItem = new ListViewItem(node.Attributes["key"].Value);
                listPlugins.Items.Add(pluginItem);

                String[] descriptors = node.Attributes["value"].Value.Split(',');
                String typeName = descriptors[0];
                String assemblyName = descriptors[1];

                pluginItem.SubItems.Add(typeName);
                pluginItem.SubItems.Add(assemblyName);
            }
        }

        private void PluginManagerWindow_Load(object sender, EventArgs e) {
            DialogResult dialogResult;
            ProcessResult processResult;

            sdrSharpFolderDialog = new FolderBrowserDialog();
            sdrSharpFolderDialog.Description = "Select the location of SDR#";
            sdrSharpFolderDialog.SelectedPath = Directory.GetCurrentDirectory();
            sdrSharpFolderDialog.ShowNewFolderButton = false;

            processResult = ProcessDirectory(sdrSharpFolderDialog.SelectedPath);
            while (processResult != ProcessResult.Success) {
                dialogResult = sdrSharpFolderDialog.ShowDialog();
                if (dialogResult == DialogResult.Cancel) {
                    this.Close();
                    break;
                }
                else {
                    processResult = ProcessDirectory(sdrSharpFolderDialog.SelectedPath);
                }
            }
        }

        private void lnkProjectHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(lnkProjectHome.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e) {
            
        }
    }
}
