namespace SDRSharpPluginManager {
    partial class PluginManagerWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.lnkProjectHome = new System.Windows.Forms.LinkLabel();
            this.lblSDRSharpPath = new System.Windows.Forms.Label();
            this.tBoxSDRSharpPathValue = new System.Windows.Forms.TextBox();
            this.listPlugins = new System.Windows.Forms.ListView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAssembly = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lnkProjectHome
            // 
            this.lnkProjectHome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkProjectHome.AutoSize = true;
            this.lnkProjectHome.Location = new System.Drawing.Point(12, 343);
            this.lnkProjectHome.Name = "lnkProjectHome";
            this.lnkProjectHome.Size = new System.Drawing.Size(258, 13);
            this.lnkProjectHome.TabIndex = 0;
            this.lnkProjectHome.TabStop = true;
            this.lnkProjectHome.Text = "https://github.com/slapec/SDRSharpPluginManager";
            this.lnkProjectHome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkProjectHome_LinkClicked);
            // 
            // lblSDRSharpPath
            // 
            this.lblSDRSharpPath.AutoSize = true;
            this.lblSDRSharpPath.Location = new System.Drawing.Point(12, 13);
            this.lblSDRSharpPath.Name = "lblSDRSharpPath";
            this.lblSDRSharpPath.Size = new System.Drawing.Size(65, 13);
            this.lblSDRSharpPath.TabIndex = 1;
            this.lblSDRSharpPath.Text = "SDR# Path:";
            // 
            // tBoxSDRSharpPathValue
            // 
            this.tBoxSDRSharpPathValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tBoxSDRSharpPathValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tBoxSDRSharpPathValue.Location = new System.Drawing.Point(83, 13);
            this.tBoxSDRSharpPathValue.Name = "tBoxSDRSharpPathValue";
            this.tBoxSDRSharpPathValue.ReadOnly = true;
            this.tBoxSDRSharpPathValue.Size = new System.Drawing.Size(496, 13);
            this.tBoxSDRSharpPathValue.TabIndex = 2;
            // 
            // listPlugins
            // 
            this.listPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colType,
            this.colAssembly});
            this.listPlugins.FullRowSelect = true;
            this.listPlugins.GridLines = true;
            this.listPlugins.Location = new System.Drawing.Point(15, 35);
            this.listPlugins.MultiSelect = false;
            this.listPlugins.Name = "listPlugins";
            this.listPlugins.ShowGroups = false;
            this.listPlugins.Size = new System.Drawing.Size(564, 297);
            this.listPlugins.TabIndex = 3;
            this.listPlugins.UseCompatibleStateImageBehavior = false;
            this.listPlugins.View = System.Windows.Forms.View.Details;
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(424, 338);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 4;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(505, 338);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // colName
            // 
            this.colName.Tag = "";
            this.colName.Text = "Name";
            this.colName.Width = 40;
            // 
            // colType
            // 
            this.colType.Tag = "";
            this.colType.Text = "Type";
            this.colType.Width = 36;
            // 
            // colAssembly
            // 
            this.colAssembly.Tag = "";
            this.colAssembly.Text = "Assembly";
            this.colAssembly.Width = 484;
            // 
            // PluginManagerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 373);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.listPlugins);
            this.Controls.Add(this.tBoxSDRSharpPathValue);
            this.Controls.Add(this.lblSDRSharpPath);
            this.Controls.Add(this.lnkProjectHome);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "PluginManagerWindow";
            this.Text = "SDRSharp Plugin Manager";
            this.Load += new System.EventHandler(this.PluginManagerWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkProjectHome;
        private System.Windows.Forms.Label lblSDRSharpPath;
        private System.Windows.Forms.TextBox tBoxSDRSharpPathValue;
        private System.Windows.Forms.ListView listPlugins;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colAssembly;

    }
}

