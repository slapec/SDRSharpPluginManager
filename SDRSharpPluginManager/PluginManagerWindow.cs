using System;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.Collections.Specialized;
using System.Xml;
using System.Collections.Generic;

namespace SDRSharpPluginManager {
    public partial class PluginManagerWindow : Form {
        public static string ConfigFileName = "SDRSharp.exe.Config";
        public static string[] RequiredFiles = {ConfigFileName, 
                                                "SDRSharp.Common.dll",
                                                "SDRSharp.Radio.dll"};

        public static string PluginInterfaceName = "ISharpPlugin";
        public static string DisplayNameFieldName = "_displayName";

        public static string PluginFileFilter = "SDRSharp Plugins (*.dll)|*.dll";

        private SDRSharpConfig config;

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
            SetWorkingDirectory();
        }

        //Still not the best method name
        private void SetWorkingDirectory() {
            DialogResult dialogResult;
            FolderBrowserDialog sdrSharpFolderDialog = FolderBrowserFactory();

            //Maybe the CWD is already a valid directory so we can skip showing the dialog
            while(CheckDirectory(sdrSharpFolderDialog.SelectedPath) != true){
                dialogResult = sdrSharpFolderDialog.ShowDialog();
                if (dialogResult == DialogResult.Cancel) {
                    this.Close();
                    break;
                }
            }

            LoadConfig();
        }

        private bool CheckDirectory(string path) {
            foreach (string requiredFileName in RequiredFiles) {
                string absolutePath = Path.Combine(path, requiredFileName);

                if (!File.Exists(absolutePath)) {
                    string msg = string.Format("File '{0}' is missing", absolutePath);
                    MessageBox.Show(msg, "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return false;
                }
            }
            return true;
        }

        private void LoadConfig() {
            config = new SDRSharpConfig(Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName));

            // Fill listPlugins
            foreach (KeyValuePair<string, string> pluginData in config.GetSharpPlugins()) {
                string[] descriptors = pluginData.Value.Split(',');
                string typeName = descriptors[0];
                string assemblyName = descriptors[1];

                ListViewItem pluginItem = new ListViewItem(pluginData.Key);
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
                object pluginObject = Activator.CreateInstance(pluginEntryType);

                string displayName = (string)pluginEntryType.GetProperty("DisplayName").GetValue(pluginObject);
                string assemblyName = pluginAssembly.GetName().Name;

                // I hope generating 'typeName' is this simple
                string typeName = string.Format("{0}.{1}", assemblyName, pluginEntryType.Name);

                // Adding new plugin to the config
                config.AddSharpPlugin(displayName, typeName, assemblyName);

                // Adding new plugin to the list
                ListViewItem pluginItem = new ListViewItem(displayName);
                listPlugins.Items.Add(pluginItem);

                pluginItem.Font = new System.Drawing.Font(pluginItem.Font, pluginItem.Font.Style | System.Drawing.FontStyle.Bold);

                pluginItem.SubItems.Add(typeName);
                pluginItem.SubItems.Add(assemblyName);

                btnSave.Enabled = true;
                FitColumns();
            }
            catch (InvalidOperationException) {
                MessageBox.Show("The selected DLL does not contain any plugin definition", "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (BadImageFormatException) {
                MessageBox.Show("The selected DLL cannot be inserted in SDR#", "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FileLoadException) {
                MessageBox.Show("The selected DLL cannot be inserted in SDR#", "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentException) {
                MessageBox.Show("There is already a plugin in the list with the same description as the one you wanted to add", "Duplicated DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ReflectionTypeLoadException e) {
                Exception[] loaderExceptions = e.LoaderExceptions;
                foreach (Exception loaderException in loaderExceptions) {
                    if (loaderException is TypeLoadException) {
                        string msg = String.Format("The selected DLL cannot be inserted in SDR#.\n\nOriginal error is:\n{0}", ((TypeLoadException)loaderException).Message);
                        MessageBox.Show(msg, "Invalid DLL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else throw loaderException;
                }
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
                    config.RemoveSharpPlugin(selected.Text);
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
            config.Save();

            foreach (ListViewItem pluginItem in listPlugins.Items) {
                pluginItem.Font = new System.Drawing.Font(pluginItem.Font, System.Drawing.FontStyle.Regular);
            }
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

        private FolderBrowserDialog FolderBrowserFactory() {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select the location of SDR#";
            dialog.SelectedPath = Directory.GetCurrentDirectory();
            dialog.ShowNewFolderButton = false;

            return dialog;
        }
        #endregion
    }
}
