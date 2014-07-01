using System;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Linq;

namespace SDRSharpPluginManager {
    public partial class PluginManagerWindow : Form {
        public enum ProcessResult {Success = 0, Missing = 1};
        public static String SDRSharpCommonDllFName = "SDRSharp.Common.dll";
        public static String SDRSharpExe = "SDRSharp.exe.Config";
        public static String[] RequiredFiles = {SDRSharpExe, SDRSharpCommonDllFName};
        public static String PluginInterfaceName = "ISharpPlugin";
        public static String DisplayNameFieldName = "_displayName";

        public static String PluginFileFilter = "SDRSharp Plugins (*.dll)|*.dll";

        private FolderBrowserDialog sdrSharpFolderDialog;
        private OpenFileDialog pluginFileDialog;

        public PluginManagerWindow() {
            InitializeComponent();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
        }

        Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            AssemblyName name = new AssemblyName(args.Name);
            String assemblyNameWithExtension = name.Name + ".dll";

            String assemblyAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), assemblyNameWithExtension);

            return Assembly.LoadFrom(assemblyAbsolutePath);
        }

        #region Parsing config file
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

        private ProcessResult ProcessDirectory(String path) {
            Directory.SetCurrentDirectory(path);

            foreach (String requiredFileName in RequiredFiles) {
                if (!File.Exists(requiredFileName)) {
                    String msg = String.Format("File '{0}' was not found in '{1}'", requiredFileName, path);

                    MessageBox.Show(msg, "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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

            FitColumns();
        }
        #endregion

        #region Parsing plugin dll
        private void btnAdd_Click(object sender, EventArgs e) {
            pluginFileDialog = new OpenFileDialog();
            pluginFileDialog.Filter = PluginFileFilter;

            if (pluginFileDialog.ShowDialog() == DialogResult.OK) {
                String pluginFilePath = pluginFileDialog.InitialDirectory + pluginFileDialog.FileName;
                LoadDll(pluginFilePath);
            }
        }

        private void LoadDll(String path) {
            try {
                Assembly pluginAssembly = Assembly.LoadFrom(path);

                Type pluginEntryType = (from type in pluginAssembly.GetTypes()
                                        where type.GetInterface(PluginInterfaceName) != null
                                        select type).First();
                String name = (String)pluginEntryType.GetField(DisplayNameFieldName, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                String assemblyName = pluginAssembly.GetName().Name;
                String typeName = String.Format("{0}.{1}", assemblyName, pluginEntryType.Name);

                ListViewItem pluginItem = new ListViewItem(name);
                listPlugins.Items.Add(pluginItem);

                pluginItem.Font = new System.Drawing.Font(pluginItem.Font, pluginItem.Font.Style | System.Drawing.FontStyle.Bold);

                pluginItem.SubItems.Add(typeName);
                pluginItem.SubItems.Add(assemblyName);
            }
            catch (InvalidOperationException) {
                MessageBox.Show("The selected DLL does not contain any plugin definition", "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (BadImageFormatException) {
                MessageBox.Show("The selected DLL cannot be inserted in SDR#", "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void lnkProjectHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(lnkProjectHome.Text);
        }

        private void FitColumns() {
            listPlugins.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            listPlugins.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            listPlugins.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}
