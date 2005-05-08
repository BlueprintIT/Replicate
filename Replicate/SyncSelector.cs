/*
 * $HeadURL$
 * $LastChangedBy$
 * $Date$
 * $Revision$
 */

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BlueprintIT.Storage;
using Microsoft.Win32;
using log4net;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for SyncSelector.
	/// </summary>
	public class SyncSelector : System.Windows.Forms.Form
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private ArrayList settings = new ArrayList();
		private System.Windows.Forms.Button btnNew;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.ListBox listSettings;
		private System.Windows.Forms.Button button1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SyncSelector(IList syncstates)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			LoadSettings(syncstates);
		}

		private void LoadSettings(IList syncstates)
		{
			settings.Clear();
			listSettings.Items.Clear();
			foreach (State.State state in syncstates)
			{
				settings.Add(state);
				listSettings.Items.Add(state.Name);
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
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listSettings
			// 
			this.listSettings.Location = new System.Drawing.Point(16, 16);
			this.listSettings.Name = "listSettings";
			this.listSettings.Size = new System.Drawing.Size(232, 186);
			this.listSettings.TabIndex = 0;
			this.listSettings.DoubleClick += new System.EventHandler(this.listSettings_DoubleClick);
			this.listSettings.SelectedIndexChanged += new System.EventHandler(this.listSettings_SelectedIndexChanged);
			// 
			// btnNew
			// 
			this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnNew.Location = new System.Drawing.Point(264, 16);
			this.btnNew.Name = "btnNew";
			this.btnNew.Size = new System.Drawing.Size(80, 23);
			this.btnNew.TabIndex = 1;
			this.btnNew.Text = "New...";
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			// 
			// btnDelete
			// 
			this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnDelete.Location = new System.Drawing.Point(264, 96);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(80, 23);
			this.btnDelete.TabIndex = 2;
			this.btnDelete.Text = "Delete...";
			// 
			// btnSelect
			// 
			this.btnSelect.Enabled = false;
			this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSelect.Location = new System.Drawing.Point(264, 56);
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.Size = new System.Drawing.Size(80, 23);
			this.btnSelect.TabIndex = 3;
			this.btnSelect.Text = "Edit...";
			this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
			// 
			// btnExit
			// 
			this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnExit.Location = new System.Drawing.Point(264, 136);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(80, 23);
			this.btnExit.TabIndex = 4;
			this.btnExit.Text = "Exit";
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(264, 176);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 24);
			this.button1.TabIndex = 5;
			this.button1.Text = "Synchronise";
			// 
			// SyncSelector
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(362, 223);
			this.Controls.Add(this.button1);
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
			SyncOptions options = new SyncOptions(new DirectoryInfo(Environment.GetFolderPath(
				Environment.SpecialFolder.LocalApplicationData)+"\\Blueprint IT Ltd\\Replicate"));
			options.ShowDialog();
		}

		private void btnExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void btnSelect_Click(object sender, System.EventArgs e)
		{
			State.State setting = (State.State)settings[listSettings.SelectedIndex];
			SyncOptions options = new SyncOptions(setting);
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
