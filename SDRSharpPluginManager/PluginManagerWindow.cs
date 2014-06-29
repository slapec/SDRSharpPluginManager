using System;
using System.IO;
using System.Windows.Forms;

namespace SDRSharpPluginManager {
    public partial class PluginManagerWindow : Form {
        public enum ProcessResult {
            Success = 0, 
            Missing = 1
        };
        public static String[] RequiredFiles = { "SDRSharp.Common.dll" };

        private FolderBrowserDialog sdrSharpFolderDialog;

        public PluginManagerWindow() {
            InitializeComponent();
        }

        private ProcessResult ProcessDirectory(String path) {
            Directory.SetCurrentDirectory(path);

            Console.WriteLine(path);

            foreach (String requiredFileName in RequiredFiles) {
                if (!File.Exists(requiredFileName)) {
                    String msg = String.Format("File '{0}' is not found in '{1}'", requiredFileName, path);

                    MessageBox.Show(msg, "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return ProcessResult.Missing;
                }
            }

            tBoxSDRSharpPathValue.Text = path;

            return ProcessResult.Success;
        }

        private void PluginManagerWindow_Load(object sender, EventArgs e) {
            DialogResult dialogResult;
            ProcessResult processResult;

            sdrSharpFolderDialog = new FolderBrowserDialog();
            sdrSharpFolderDialog.Description = "Select the location of SDR#";
            sdrSharpFolderDialog.SelectedPath = Directory.GetCurrentDirectory();
            sdrSharpFolderDialog.ShowNewFolderButton = false;

            processResult = ProcessDirectory(sdrSharpFolderDialog.SelectedPath);
            /*while (processResult != ProcessResult.Success) {
                dialogResult = sdrSharpFolderDialog.ShowDialog();
                if (dialogResult == DialogResult.Cancel) {
                    this.Close();
                    break;
                }
                else {
                    processResult = ProcessDirectory(sdrSharpFolderDialog.SelectedPath);
                }
            }*/
        }

        private void lnkProjectHome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(lnkProjectHome.Text);
        }
    }
}
