using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BlueprintIT.Storage;
using Microsoft.Win32;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for SyncSelector.
	/// </summary>
	public class SyncSelector : System.Windows.Forms.Form
	{
		private StorageProviders providers;
		private ArrayList settings = new ArrayList();
		private System.Windows.Forms.Button btnNew;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.ListBox listSettings;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SyncSelector(StorageProviders providers)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.providers=providers;
			LoadSettings();
		}

		private void LoadSettings()
		{
			settings.Clear();
			listSettings.Items.Clear();
			RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Blueprint IT Ltd\Replicate\Settings");
			foreach (string id in key.GetSubKeyNames())
			{
				SyncSettings sets = new SyncSettings(id);
				settings.Add(sets);
				listSettings.Items.Add(sets);
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listSettings = new System.Windows.Forms.ListBox();
			this.btnNew = new System.Windows.Forms.Button();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnSelect = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listSettings
			// 
			this.listSettings.Location = new System.Drawing.Point(16, 16);
			this.listSettings.Name = "listSettings";
			this.listSettings.Size = new System.Drawing.Size(232, 147);
			this.listSettings.TabIndex = 0;
			this.listSettings.DoubleClick += new System.EventHandler(this.listSettings_DoubleClick);
			this.listSettings.SelectedIndexChanged += new System.EventHandler(this.listSettings_SelectedIndexChanged);
			// 
			// btnNew
			// 
			this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnNew.Location = new System.Drawing.Point(264, 16);
			this.btnNew.Name = "btnNew";
			this.btnNew.TabIndex = 1;
			this.btnNew.Text = "New...";
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnDelete.Location = new System.Drawing.Point(264, 96);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.TabIndex = 2;
			this.btnDelete.Text = "Delete...";
			// 
			// btnSelect
			// 
			this.btnSelect.Enabled = false;
			this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSelect.Location = new System.Drawing.Point(264, 56);
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.TabIndex = 3;
			this.btnSelect.Text = "Select...";
			this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
			// 
			// btnExit
			// 
			this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnExit.Location = new System.Drawing.Point(264, 136);
			this.btnExit.Name = "btnExit";
			this.btnExit.TabIndex = 4;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// SyncSelector
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(354, 181);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.btnSelect);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnNew);
			this.Controls.Add(this.listSettings);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SyncSelector";
			this.Text = "Select Synchronisation";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnNew_Click(object sender, System.EventArgs e)
		{
			SyncOptions options = new SyncOptions(providers);
			options.ShowDialog();
			LoadSettings();
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnSelect_Click(object sender, System.EventArgs e)
		{
			SyncSettings setting = (SyncSettings)settings[listSettings.SelectedIndex];
			SyncOptions options = new SyncOptions(providers,setting);
			options.ShowDialog();
		}

		private void listSettings_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			btnSelect.Enabled=(listSettings.SelectedIndex>=0);
		}

		private void listSettings_DoubleClick(object sender, System.EventArgs e)
		{
			if (listSettings.SelectedIndex>=0)
			{
				btnSelect_Click(sender,e);
			}
		}
	}
}
