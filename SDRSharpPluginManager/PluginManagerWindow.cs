using System;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;

namespace SDRSharpPluginManager {
    public partial class PluginManagerWindow : Form {
        private OpenFileDialog pluginFileDialog;
        private PluginManager plugins;

        public PluginManagerWindow() {
            InitializeComponent();
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
            LoadConfig(sdrSharpFolderDialog.SelectedPath);
        }

        private bool CheckDirectory(string path) {
            foreach (string requiredFileName in Consts.RequiredFiles) {
                string absolutePath = Path.Combine(path, requiredFileName);

                if (!File.Exists(absolutePath)) {
                    string msg = string.Format("File '{0}' is missing", absolutePath);
                    MessageBox.Show(msg, "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    return false;
                }
            }
            return true;
        }

        private void LoadConfig(string path) {
            plugins = new PluginManager(path);

            // Fill listPlugins
            foreach (Plugin plugin in plugins.Plugins) {
                ListViewItem pluginItem = new PluginListViewItem(plugin);
                listPlugins.Items.Add(pluginItem);
            }
            FitColumns();
            tBoxSDRSharpPathValue.Text = path;

            SetupHandlers();
            Directory.SetCurrentDirectory(path);
        }
        #endregion

        #region Add plugin
        private void btnAdd_Click(object sender, EventArgs e) {
            pluginFileDialog = new OpenFileDialog();
            pluginFileDialog.Filter = Consts.PluginFileFilter;

            if (pluginFileDialog.ShowDialog() == DialogResult.OK) {
                string pluginFilePath = Path.Combine(pluginFileDialog.InitialDirectory, pluginFileDialog.FileName);
                LoadDLL(pluginFilePath);
            }
        }

        private void LoadDLL(string path) {
            try {
                Assembly pluginAssembly = Assembly.LoadFrom(path);

                // Search for the first class that implements 'ISharpPlugin' interface
                Type pluginEntryType = (from type in pluginAssembly.GetTypes()
                                        where type.GetInterface(Consts.PluginInterfaceName) != null
                                        select type).First();
                object pluginObject = Activator.CreateInstance(pluginEntryType);

                string displayName = (string)pluginEntryType.GetProperty(Consts.DisplayNameFieldName).GetValue(pluginObject);
                string assemblyName = pluginAssembly.GetName().Name;

                // I hope generating 'typeName' is this simple
                string typeName = string.Format("{0}.{1}", assemblyName, pluginEntryType.Name);

                // Adding new plugin to the config
                Plugin addedPlugin = new Plugin(displayName, typeName, assemblyName);
                plugins.Add(addedPlugin);

                // Adding new plugin to the list
                ListViewItem pluginItem = new PluginListViewItem(addedPlugin);
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
                    plugins.Remove(((PluginListViewItem)selected).Plugin);
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

        #region Enable/disable
        void listPlugins_ItemChecked(object sender, ItemCheckedEventArgs e) {
            PluginListViewItem checkedItem = (PluginListViewItem)e.Item;

            checkedItem.Plugin.Enabled = checkedItem.Checked;
            btnSave.Enabled = true;
        }
        #endregion

        #region Save changes
        private void btnSave_Click(object sender, EventArgs e) {
            btnSave.Enabled = false;
            plugins.Save();

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
            listPlugins.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.ColumnContent);
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

        private void SetupHandlers() {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += currentDomain_AssemblyResolve;

            listPlugins.ItemChecked += listPlugins_ItemChecked;
        }
    }
}
