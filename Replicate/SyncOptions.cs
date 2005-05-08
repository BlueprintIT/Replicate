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
using log4net;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for SyncOptions.
	/// </summary>
	public class SyncOptions : System.Windows.Forms.Form
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private StorageProviders providers;
		private ArrayList protocols = new ArrayList();
		private State.State settings;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnLocalBrowse;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cmbProtocol;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnRemoteBrowse;
		private System.Windows.Forms.FolderBrowserDialog folderBrowser;
		private System.Windows.Forms.TextBox txtLocal;
		private System.Windows.Forms.TextBox txtRemote;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOk;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private SyncOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.providers=providers;
			foreach (IStoreProvider provider in StorageProviders.Providers)
			{
				protocols.Add(provider);
				cmbProtocol.Items.Add(provider.DisplayName);
			}
			cmbProtocol.SelectedIndex=0;
		}

		public SyncOptions(DirectoryInfo dir): this()
		{
			settings = new State.State(GetUniqueFile(dir));
		}

		private FileInfo GetUniqueFile(DirectoryInfo dir)
		{
			int id=0;
			while (File.Exists(dir.FullName+"\\sync"+id+".sync"))
			{
				id++;
			}
			return new FileInfo(dir.FullName+"\\sync"+id+".sync");
		}

		public SyncOptions(State.State settings): this()
		{
			this.settings=settings;
			txtLocal.Text=settings.LocalUri.LocalPath;
			int pos=0;
			foreach (IStoreProvider provider in protocols)
			{
				if (provider.Protocol==settings.RemoteUri.Scheme)
				{
					cmbProtocol.SelectedIndex=pos;
					break;
				}
				pos++;
			}
			txtRemote.Text=settings.RemoteUri.ToString();
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnLocalBrowse = new System.Windows.Forms.Button();
			this.txtLocal = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnRemoteBrowse = new System.Windows.Forms.Button();
			this.txtRemote = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cmbProtocol = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnLocalBrowse);
			this.groupBox1.Controls.Add(this.txtLocal);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(392, 80);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Local Files";
			// 
			// btnLocalBrowse
			// 
			this.btnLocalBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnLocalBrowse.Location = new System.Drawing.Point(296, 32);
			this.btnLocalBrowse.Name = "btnLocalBrowse";
			this.btnLocalBrowse.TabIndex = 1;
			this.btnLocalBrowse.Text = "Browse...";
			this.btnLocalBrowse.Click += new System.EventHandler(this.btnLocalBrowse_Click);
			// 
			// txtLocal
			// 
			this.txtLocal.Location = new System.Drawing.Point(16, 32);
			this.txtLocal.Name = "txtLocal";
			this.txtLocal.Size = new System.Drawing.Size(272, 20);
			this.txtLocal.TabIndex = 0;
			this.txtLocal.Text = "";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnRemoteBrowse);
			this.groupBox2.Controls.Add(this.txtRemote);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.cmbProtocol);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(8, 104);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(392, 120);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Remote Files";
			// 
			// btnRemoteBrowse
			// 
			this.btnRemoteBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoteBrowse.Location = new System.Drawing.Point(296, 72);
			this.btnRemoteBrowse.Name = "btnRemoteBrowse";
			this.btnRemoteBrowse.TabIndex = 4;
			this.btnRemoteBrowse.Text = "Browse...";
			this.btnRemoteBrowse.Click += new System.EventHandler(this.btnRemoteBrowse_Click);
			// 
			// txtRemote
			// 
			this.txtRemote.Location = new System.Drawing.Point(40, 72);
			this.txtRemote.Name = "txtRemote";
			this.txtRemote.Size = new System.Drawing.Size(248, 20);
			this.txtRemote.TabIndex = 3;
			this.txtRemote.Text = "";
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(16, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(24, 24);
			this.label2.TabIndex = 2;
			this.label2.Text = "Url:";
			// 
			// cmbProtocol
			// 
			this.cmbProtocol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbProtocol.Location = new System.Drawing.Point(112, 32);
			this.cmbProtocol.Name = "cmbProtocol";
			this.cmbProtocol.Size = new System.Drawing.Size(176, 21);
			this.cmbProtocol.TabIndex = 1;
			this.cmbProtocol.SelectedIndexChanged += new System.EventHandler(this.cmbProtocol_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Files are held on:";
			// 
			// folderBrowser
			// 
			this.folderBrowser.Description = "Select the local files:";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(248, 240);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			// 
			// btnOk
			// 
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOk.Location = new System.Drawing.Point(88, 240);
			this.btnOk.Name = "btnOk";
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "OK";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			// 
			// SyncOptions
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(408, 283);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "SyncOptions";
			this.Text = "Synchronise Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnLocalBrowse_Click(object sender, System.EventArgs e)
		{
			folderBrowser.SelectedPath=txtLocal.Text;
			if (folderBrowser.ShowDialog()==DialogResult.OK)
			{
				txtLocal.Text=folderBrowser.SelectedPath;
			}
		}

		private void cmbProtocol_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			IStoreProvider provider = (IStoreProvider)protocols[cmbProtocol.SelectedIndex];
			txtRemote.Text=provider.Protocol+"://";
			btnRemoteBrowse.Visible=provider.SupportsBrowse;
		}

		private void btnRemoteBrowse_Click(object sender, System.EventArgs e)
		{
			Uri uri = null;
			try
			{
				uri = new Uri(txtRemote.Text);
			}
			catch
			{
			}
			IStoreProvider provider = (IStoreProvider)protocols[cmbProtocol.SelectedIndex];
			uri=provider.Browse(uri);
			if (uri!=null)
			{
				txtRemote.Text = uri.ToString();
			}
		}

		private void btnSync_Click(object sender, System.EventArgs e)
		{
			//IStore local = providers.OpenStore(new Uri("file:///"+txtLocal.Text.Replace('\\','/')));
			//IStore remote = providers.OpenStore(new Uri(txtRemote.Text));
			//SyncScan scanner = new SyncScan(local,remote);
		}

		private void btnOk_Click(object sender, System.EventArgs e)
		{
			if (settings.Name==null)
			{
				settings.Name=Question.Ask("Settings Name","Enter a name for these settings:");
			}
			if (settings.Name!=null)
			{
				settings.RemoteUri = new Uri(txtRemote.Text);
				settings.LocalUri = new Uri("file:///"+txtLocal.Text.Replace('\\','/'));
				settings.Persist();
			}
		}
	}
}
