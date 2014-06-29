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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
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
            this.listPlugins.Location = new System.Drawing.Point(15, 35);
            this.listPlugins.Name = "listPlugins";
            this.listPlugins.Size = new System.Drawing.Size(564, 297);
            this.listPlugins.TabIndex = 3;
            this.listPlugins.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(424, 338);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Remove";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(505, 338);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Add";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // PluginManagerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 373);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;

    }
}

