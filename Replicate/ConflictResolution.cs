using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BlueprintIT.Storage;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for TypeConflictResolution.
	/// </summary>
	public class ConflictResolution : System.Windows.Forms.Form
	{
		private SyncRecord record;
		private IDictionary records;
		private System.Windows.Forms.GroupBox groupLocal;
		private System.Windows.Forms.GroupBox groupRemote;
		private System.Windows.Forms.Button btnLocalRename;
		private System.Windows.Forms.Button btnLocalDelete;
		private System.Windows.Forms.Button btnRemoteRename;
		private System.Windows.Forms.Button btnRemoteDelete;
		private System.Windows.Forms.Label lblLocalName;
		private System.Windows.Forms.Label lblLocalSize;
		private System.Windows.Forms.Label lblLocalDate;
		private System.Windows.Forms.Label lblRemoteName;
		private System.Windows.Forms.Label lblRemoteSize;
		private System.Windows.Forms.Label lblRemoteDate;
		private System.Windows.Forms.Button btnIgnore;
		private System.Windows.Forms.Label lblDescription;
		private System.Windows.Forms.Button btnLocalView;
		private System.Windows.Forms.Button btnRemoteView;
		private System.Windows.Forms.Label lblLocalStatus;
		private System.Windows.Forms.Label lblRemoteStatus;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConflictResolution(IDictionary records, SyncRecord record)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			string localtype;
			string remotetype;

			this.record=record;
			this.records=records;

			lblLocalName.Text=record.LocalEntry.Uri.LocalPath;
			lblRemoteName.Text=record.RemoteEntry.Uri.ToString();
			lblLocalStatus.Text+=GetStatusDescription(record.LocalStatus);
			lblRemoteStatus.Text+=GetStatusDescription(record.RemoteStatus);
			
			if (record.LocalEntry is IFile)
			{
				localtype="file";
				IFile localFile = (IFile)record.LocalEntry;
				lblLocalDate.Text+=localFile.Date.ToString();
				lblLocalSize.Text+=localFile.Size.ToString();
			}
			else
			{
				localtype="folder";
				lblLocalDate.Visible=false;
				lblLocalSize.Visible=false;
			}
			
			if (record.RemoteEntry is IFile)
			{
				remotetype="file";
				IFile remoteFile = (IFile)record.RemoteEntry;
				lblRemoteDate.Text+=remoteFile.Date.ToString();
				lblRemoteSize.Text+=remoteFile.Size.ToString();
			}
			else
			{
				remotetype="folder";
				lblRemoteDate.Visible=false;
				lblRemoteSize.Visible=false;
			}

			lblDescription.Text="A change to a local ";
			lblDescription.Text+=localtype;
			lblDescription.Text+=" conflicts with a change to a remote ";
			lblDescription.Text+=remotetype;
			lblDescription.Text+=". You must rename or delete one of these to synchronise correctly. ";

			groupLocal.Text+=localtype;
			groupRemote.Text+=remotetype;
		}

		private static string GetStatusDescription(EntryStatus status)
		{
			switch (status)
			{
				case EntryStatus.Added:
					return "Added since last synchronise";
				case EntryStatus.Removed:
					return "Removed since last synchronise";
				case EntryStatus.Unchanged:
					return "Unchanged since last synchronise";
				case EntryStatus.Updated:
					return "Updated since last synchronise";
				case EntryStatus.Replaced:
					return "Replaced since last synchronise";
			}
			return "Unknown";
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
			this.groupLocal = new System.Windows.Forms.GroupBox();
			this.lblLocalStatus = new System.Windows.Forms.Label();
			this.btnLocalView = new System.Windows.Forms.Button();
			this.lblLocalDate = new System.Windows.Forms.Label();
			this.lblLocalSize = new System.Windows.Forms.Label();
			this.lblLocalName = new System.Windows.Forms.Label();
			this.btnLocalDelete = new System.Windows.Forms.Button();
			this.btnLocalRename = new System.Windows.Forms.Button();
			this.groupRemote = new System.Windows.Forms.GroupBox();
			this.lblRemoteStatus = new System.Windows.Forms.Label();
			this.btnRemoteView = new System.Windows.Forms.Button();
			this.lblRemoteDate = new System.Windows.Forms.Label();
			this.lblRemoteSize = new System.Windows.Forms.Label();
			this.lblRemoteName = new System.Windows.Forms.Label();
			this.btnRemoteDelete = new System.Windows.Forms.Button();
			this.btnRemoteRename = new System.Windows.Forms.Button();
			this.lblDescription = new System.Windows.Forms.Label();
			this.btnIgnore = new System.Windows.Forms.Button();
			this.groupLocal.SuspendLayout();
			this.groupRemote.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupLocal
			// 
			this.groupLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.groupLocal.Controls.Add(this.lblLocalStatus);
			this.groupLocal.Controls.Add(this.btnLocalView);
			this.groupLocal.Controls.Add(this.lblLocalDate);
			this.groupLocal.Controls.Add(this.lblLocalSize);
			this.groupLocal.Controls.Add(this.lblLocalName);
			this.groupLocal.Controls.Add(this.btnLocalDelete);
			this.groupLocal.Controls.Add(this.btnLocalRename);
			this.groupLocal.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupLocal.Location = new System.Drawing.Point(8, 64);
			this.groupLocal.Name = "groupLocal";
			this.groupLocal.Size = new System.Drawing.Size(240, 232);
			this.groupLocal.TabIndex = 0;
			this.groupLocal.TabStop = false;
			this.groupLocal.Text = "Local ";
			// 
			// lblLocalStatus
			// 
			this.lblLocalStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblLocalStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblLocalStatus.Location = new System.Drawing.Point(16, 64);
			this.lblLocalStatus.Name = "lblLocalStatus";
			this.lblLocalStatus.Size = new System.Drawing.Size(208, 23);
			this.lblLocalStatus.TabIndex = 6;
			this.lblLocalStatus.Text = "Status: ";
			// 
			// btnLocalView
			// 
			this.btnLocalView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnLocalView.Enabled = false;
			this.btnLocalView.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnLocalView.Location = new System.Drawing.Point(80, 152);
			this.btnLocalView.Name = "btnLocalView";
			this.btnLocalView.Size = new System.Drawing.Size(80, 23);
			this.btnLocalView.TabIndex = 5;
			this.btnLocalView.Text = "View...";
			// 
			// lblLocalDate
			// 
			this.lblLocalDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblLocalDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblLocalDate.Location = new System.Drawing.Point(16, 112);
			this.lblLocalDate.Name = "lblLocalDate";
			this.lblLocalDate.Size = new System.Drawing.Size(208, 23);
			this.lblLocalDate.TabIndex = 4;
			this.lblLocalDate.Text = "Date: ";
			// 
			// lblLocalSize
			// 
			this.lblLocalSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblLocalSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblLocalSize.Location = new System.Drawing.Point(16, 88);
			this.lblLocalSize.Name = "lblLocalSize";
			this.lblLocalSize.Size = new System.Drawing.Size(208, 23);
			this.lblLocalSize.TabIndex = 3;
			this.lblLocalSize.Text = "Size: ";
			// 
			// lblLocalName
			// 
			this.lblLocalName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblLocalName.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblLocalName.Location = new System.Drawing.Point(16, 24);
			this.lblLocalName.Name = "lblLocalName";
			this.lblLocalName.Size = new System.Drawing.Size(208, 32);
			this.lblLocalName.TabIndex = 2;
			this.lblLocalName.Text = "label1";
			// 
			// btnLocalDelete
			// 
			this.btnLocalDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnLocalDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnLocalDelete.Location = new System.Drawing.Point(136, 192);
			this.btnLocalDelete.Name = "btnLocalDelete";
			this.btnLocalDelete.TabIndex = 1;
			this.btnLocalDelete.Text = "Delete...";
			this.btnLocalDelete.Click += new System.EventHandler(this.btnLocalDelete_Click);
			// 
			// btnLocalRename
			// 
			this.btnLocalRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnLocalRename.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnLocalRename.Location = new System.Drawing.Point(32, 192);
			this.btnLocalRename.Name = "btnLocalRename";
			this.btnLocalRename.TabIndex = 0;
			this.btnLocalRename.Text = "Rename...";
			this.btnLocalRename.Click += new System.EventHandler(this.btnLocalRename_Click);
			// 
			// groupRemote
			// 
			this.groupRemote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupRemote.Controls.Add(this.lblRemoteStatus);
			this.groupRemote.Controls.Add(this.btnRemoteView);
			this.groupRemote.Controls.Add(this.lblRemoteDate);
			this.groupRemote.Controls.Add(this.lblRemoteSize);
			this.groupRemote.Controls.Add(this.lblRemoteName);
			this.groupRemote.Controls.Add(this.btnRemoteDelete);
			this.groupRemote.Controls.Add(this.btnRemoteRename);
			this.groupRemote.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupRemote.Location = new System.Drawing.Point(256, 64);
			this.groupRemote.Name = "groupRemote";
			this.groupRemote.Size = new System.Drawing.Size(240, 232);
			this.groupRemote.TabIndex = 1;
			this.groupRemote.TabStop = false;
			this.groupRemote.Text = "Remote ";
			// 
			// lblRemoteStatus
			// 
			this.lblRemoteStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblRemoteStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblRemoteStatus.Location = new System.Drawing.Point(16, 64);
			this.lblRemoteStatus.Name = "lblRemoteStatus";
			this.lblRemoteStatus.Size = new System.Drawing.Size(208, 23);
			this.lblRemoteStatus.TabIndex = 7;
			this.lblRemoteStatus.Text = "Status: ";
			// 
			// btnRemoteView
			// 
			this.btnRemoteView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoteView.Enabled = false;
			this.btnRemoteView.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoteView.Location = new System.Drawing.Point(80, 152);
			this.btnRemoteView.Name = "btnRemoteView";
			this.btnRemoteView.Size = new System.Drawing.Size(80, 23);
			this.btnRemoteView.TabIndex = 5;
			this.btnRemoteView.Text = "View...";
			// 
			// lblRemoteDate
			// 
			this.lblRemoteDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblRemoteDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblRemoteDate.Location = new System.Drawing.Point(16, 112);
			this.lblRemoteDate.Name = "lblRemoteDate";
			this.lblRemoteDate.Size = new System.Drawing.Size(208, 23);
			this.lblRemoteDate.TabIndex = 4;
			this.lblRemoteDate.Text = "Date: ";
			// 
			// lblRemoteSize
			// 
			this.lblRemoteSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblRemoteSize.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblRemoteSize.Location = new System.Drawing.Point(16, 88);
			this.lblRemoteSize.Name = "lblRemoteSize";
			this.lblRemoteSize.Size = new System.Drawing.Size(208, 23);
			this.lblRemoteSize.TabIndex = 3;
			this.lblRemoteSize.Text = "Size: ";
			// 
			// lblRemoteName
			// 
			this.lblRemoteName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblRemoteName.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblRemoteName.Location = new System.Drawing.Point(16, 24);
			this.lblRemoteName.Name = "lblRemoteName";
			this.lblRemoteName.Size = new System.Drawing.Size(208, 32);
			this.lblRemoteName.TabIndex = 2;
			this.lblRemoteName.Text = "label1";
			// 
			// btnRemoteDelete
			// 
			this.btnRemoteDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoteDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoteDelete.Location = new System.Drawing.Point(136, 192);
			this.btnRemoteDelete.Name = "btnRemoteDelete";
			this.btnRemoteDelete.TabIndex = 1;
			this.btnRemoteDelete.Text = "Delete...";
			this.btnRemoteDelete.Click += new System.EventHandler(this.btnRemoteDelete_Click);
			// 
			// btnRemoteRename
			// 
			this.btnRemoteRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRemoteRename.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemoteRename.Location = new System.Drawing.Point(32, 192);
			this.btnRemoteRename.Name = "btnRemoteRename";
			this.btnRemoteRename.TabIndex = 0;
			this.btnRemoteRename.Text = "Rename...";
			this.btnRemoteRename.Click += new System.EventHandler(this.btnRemoteRename_Click);
			// 
			// lblDescription
			// 
			this.lblDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lblDescription.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblDescription.Location = new System.Drawing.Point(16, 16);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(384, 40);
			this.lblDescription.TabIndex = 2;
			this.lblDescription.Text = "label1";
			// 
			// btnIgnore
			// 
			this.btnIgnore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnIgnore.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnIgnore.Location = new System.Drawing.Point(416, 24);
			this.btnIgnore.Name = "btnIgnore";
			this.btnIgnore.TabIndex = 3;
			this.btnIgnore.Text = "Ignore...";
			this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
			// 
			// ConflictResolution
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(506, 309);
			this.Controls.Add(this.btnIgnore);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.groupRemote);
			this.Controls.Add(this.groupLocal);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ConflictResolution";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Conflict";
			this.groupLocal.ResumeLayout(false);
			this.groupRemote.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnIgnore_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("If you ignore this conflict you will be prompted again the next time you synchronise. Are you sure you wish to ignore this conflict?","Ignore?",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
			{
				record.Status=RecordStatus.Ignore;
				this.DialogResult=DialogResult.Cancel;
			}
		}

		private void btnLocalDelete_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this file?","Delete file?",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)
			{
				record.Status=RecordStatus.Download;
				this.DialogResult=DialogResult.OK;
			}
		}

		private void btnRemoteDelete_Click(object sender, System.EventArgs e)
		{
			record.Status=RecordStatus.Upload;
			this.DialogResult=DialogResult.OK;
		}

		private string FindNewName()
		{
			bool foundnewname = false;
			string newname = null;
			while (!foundnewname)
			{
				newname = Question.Ask("Rename file","Enter a new filename:",record.LocalEntry.Name);
				if (newname==null)
				{
					return null;
				}
				foundnewname=true;
				IFolder folder = record.LocalEntry.Folder;
				if (folder[newname]!=null)
				{
					foundnewname=false;
					MessageBox.Show("A file or folder with that name already exists locally.");
				}
				folder = record.RemoteEntry.Folder;
				if (folder[newname]!=null)
				{
					foundnewname=false;
					MessageBox.Show("A file or folder with that name already exists remotely.");
				}
			}
			return newname;
		}

		private void btnLocalRename_Click(object sender, System.EventArgs e)
		{
			string newname = FindNewName();
			if (newname!=null)
			{
				IEntry local = record.LocalEntry;
				local.Name=newname;

				if (record.RemoteEntry is IFile)
				{
					record.LocalEntry = local.Folder.GetFile(record.RemoteEntry.Name);
				}
				else
				{
					record.LocalEntry = local.Folder.GetFolder(record.RemoteEntry.Name);
				}

				// TODO need to put the new SyncRecord somewhere.
				SyncRecord newrecord = new SyncRecord(newname);
				newrecord.LocalEntry=local;
				if (local is IFile)
				{
					newrecord.RemoteEntry=record.RemoteEntry.Folder.GetFile(newname);
				}
				else
				{
					newrecord.RemoteEntry=record.RemoteEntry.Folder.GetFolder(newname);
				}
				newrecord.Status=RecordStatus.Upload;
				records[newname]=newrecord;

				record.Status=RecordStatus.Download;
				DialogResult=DialogResult.OK;
			}
		}

		private void btnRemoteRename_Click(object sender, System.EventArgs e)
		{
			string newname = FindNewName();
			if (newname!=null)
			{
				IEntry remote = record.RemoteEntry;
				remote.Name=newname;

				if (record.LocalEntry is IFile)
				{
					record.RemoteEntry = remote.Folder.GetFile(record.LocalEntry.Name);
				}
				else
				{
					record.RemoteEntry = remote.Folder.GetFolder(record.LocalEntry.Name);
				}

				// TODO need to put the new SyncRecord somewhere.
				SyncRecord newrecord = new SyncRecord(newname);
				newrecord.RemoteEntry=remote;
				if (remote is IFile)
				{
					newrecord.LocalEntry=record.LocalEntry.Folder.GetFile(newname);
				}
				else
				{
					newrecord.LocalEntry=record.LocalEntry.Folder.GetFolder(newname);
				}
				newrecord.Status=RecordStatus.Download;
				records[newname]=newrecord;

				record.Status=RecordStatus.Upload;
				DialogResult=DialogResult.OK;
			}
		}
	}
}
