using System.Windows.Forms;

namespace SDRSharpPluginManager {
    class PluginListViewItem : ListViewItem {
        // My variable names are not so creative
        private Plugin plugin;
        public Plugin Plugin {
            get {
                return plugin;
            }
            set {
                this.plugin = Plugin;
            }
        }

        public PluginListViewItem(Plugin _plugin) : base(){
            plugin = _plugin;

            SubItems.Add(plugin.DisplayName);
            SubItems.Add(plugin.TypeName);
            SubItems.Add(plugin.AssemblyName);

            Checked = plugin.Enabled;
        }
    }
}
