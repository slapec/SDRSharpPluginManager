using System;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.Collections.Specialized;

namespace SDRSharpPluginManager {
    public partial class PluginManagerWindow : Form {
        public enum ProcessResult {Success = 0, Missing = 1};

        public static string ConfigFileName = "SDRSharp.exe.Config";
        public static string[] RequiredFiles = {ConfigFileName, 
                                                "SDRSharp.Common.dll",
                                                "SDRSharp.Radio.dll"};

        public static string PluginInterfaceName = "ISharpPlugin";
        public static string DisplayNameFieldName = "_displayName";

        public static string PluginFileFilter = "SDRSharp Plugins (*.dll)|*.dll";

        private FolderBrowserDialog sdrSharpFolderDialog;
        private OpenFileDialog pluginFileDialog;

        public PluginManagerWindow() {
            InitializeComponent();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;
        }

        // This handler is needed to satisfy the dependencies of a parsed plugin
        Assembly currentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            AssemblyName unresolvedAssemblyName = new AssemblyName(args.Name);
            string unresolvedAssemblyNameWithExtension = unresolvedAssemblyName.Name + ".dll";

            string unresolvedAssemblyAbsolutePath = Path.Combine(Directory.GetCurrentDirectory(), unresolvedAssemblyNameWithExtension);

            return Assembly.LoadFrom(unresolvedAssemblyAbsolutePath);
        }

        #region Parsing config file
        private void PluginManagerWindow_Load(object sender, EventArgs e) {
            DialogResult dialogResult;
            ProcessResult processResult;

            // Folder dialog is here so it is shown right on running this program
            sdrSharpFolderDialog = new FolderBrowserDialog();
            sdrSharpFolderDialog.Description = "Select the location of SDR#";
            sdrSharpFolderDialog.SelectedPath = Directory.GetCurrentDirectory();
            sdrSharpFolderDialog.ShowNewFolderButton = false;

            processResult = ProcessDirectory(sdrSharpFolderDialog.SelectedPath);
            // Keep showing folder dialog until a the correct folder is selected or 'Cancel' pressed
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

        // Setting the CWD to the folder of SDR# and parse its config file
        private ProcessResult ProcessDirectory(string path) {
            Directory.SetCurrentDirectory(path);

            // Check existance of some required files in folder
            foreach (string requiredFileName in RequiredFiles) {
                if (!File.Exists(requiredFileName)) {
                    string msg = string.Format("File '{0}' was not found in '{1}'", requiredFileName, path);

                    MessageBox.Show(msg, "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return ProcessResult.Missing;
                }
            }

            tBoxSDRSharpPathValue.Text = path;
            this.LoadConfig();
            return ProcessResult.Success;
        }

        private void LoadConfig() {
            // Switching the config file of 'SDRSharpPluginManager' to SDR#'s
            AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName));

            // Fill listPlugins
            NameValueCollection pluginsInConfig = (NameValueCollection)ConfigurationManager.GetSection("sharpPlugins");
            foreach (string displayName in pluginsInConfig.Keys) {
                string[] descriptors = pluginsInConfig[displayName].Split(',');
                string typeName = descriptors[0];
                string assemblyName = descriptors[1];

                ListViewItem pluginItem = new ListViewItem(displayName);
                listPlugins.Items.Add(pluginItem);

                pluginItem.SubItems.Add(typeName);
                pluginItem.SubItems.Add(assemblyName);
            }
            FitColumns();
        }
        #endregion

        #region Add plugin
        private void btnAdd_Click(object sender, EventArgs e) {
            pluginFileDialog = new OpenFileDialog();
            pluginFileDialog.Filter = PluginFileFilter;

            if (pluginFileDialog.ShowDialog() == DialogResult.OK) {
                string pluginFilePath = pluginFileDialog.InitialDirectory + pluginFileDialog.FileName;
                LoadDLL(pluginFilePath);
            }
        }

        private void LoadDLL(string path) {
            try {
                Assembly pluginAssembly = Assembly.LoadFrom(path);

                // Search for the first class that implements 'ISharpPlugin' interface
                Type pluginEntryType = (from type in pluginAssembly.GetTypes()
                                        where type.GetInterface(PluginInterfaceName) != null
                                        select type).First();
                string name = (string)pluginEntryType.GetField(DisplayNameFieldName, BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
                string assemblyName = pluginAssembly.GetName().Name;

                // I hope generating 'typeName' is this simple
                string typeName = string.Format("{0}.{1}", assemblyName, pluginEntryType.Name);

                ListViewItem pluginItem = new ListViewItem(name);
                listPlugins.Items.Add(pluginItem);

                pluginItem.Font = new System.Drawing.Font(pluginItem.Font, pluginItem.Font.Style | System.Drawing.FontStyle.Bold);

                pluginItem.SubItems.Add(typeName);
                pluginItem.SubItems.Add(assemblyName);

                btnSave.Enabled = true;
            }
            catch (InvalidOperationException) {
                MessageBox.Show("The selected DLL does not contain any plugin definition", "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (BadImageFormatException) {
                MessageBox.Show("The selected DLL cannot be inserted in SDR#", "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Remove plugin
        private void RemoveSelected() {
            ListView.SelectedListViewItemCollection selection = listPlugins.SelectedItems;

            if (selection.Count > 0) {
                ListViewItem selected = selection[0];

                string message = String.Format("Are you sure you want to remove '{0}'?", selected.Text);

                // First argument is the list itself so we don't loose the focus
                DialogResult result = MessageBox.Show(listPlugins, message, "Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) {
                    listPlugins.Items.Remove(selected);
                    btnRemove.Enabled = false;
                    btnSave.Enabled = true;
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e) {
            RemoveSelected();
        }

        private void listPlugins_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Delete) {
                RemoveSelected();
            }
        }
        #endregion

        #region Save changes
        private void btnSave_Click(object sender, EventArgs e) {
            btnSave.Enabled = false;
        }
        #endregion

        #region UI related methods
        private void lnkProjectHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(lnkProjectHome.Text);
        }
        private void FitColumns() {
            listPlugins.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            listPlugins.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            listPlugins.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void listPlugins_SelectedIndexChanged(object sender, EventArgs e) {
            btnRemove.Enabled = true;
        }
        #endregion
    }
}
