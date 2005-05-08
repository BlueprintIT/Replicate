/*
 * $HeadURL$
 * $LastChangedBy$
 * $Date$
 * $Revision$
 */

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BlueprintIT.Storage;
using log4net;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for DeleteConflictResolution.
	/// </summary>
	public class DeleteConflictResolution : System.Windows.Forms.Form
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private SyncRecord record;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnRename;
		private System.Windows.Forms.Button btnKeep;
		private System.Windows.Forms.Button btnView;
		private System.Windows.Forms.Label lblDescription;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DeleteConflictResolution(SyncRecord record)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.record=record;
			string deleted = "";
			string changed = "";
			string type = "";
			if (record.LocalEntry.Exists)
			{
				changed="local";
				deleted="remote";
				if (record.LocalEntry is IFile)
				{
					type="file";
				}
				else
				{
					type="folder";
				}
			}
			else if (record.RemoteEntry.Exists)
			{
				changed="remote";
				deleted="local";
				if (record.RemoteEntry is IFile)
				{
					type="file";
				}
				else
				{
					type="folder";
				}
			}
			lblDescription.Text="A "+deleted+" "+type+" was deleted, but the "+changed+" "+type+" was changed. Select "+
				"what you want to do with the "+changed+" "+type+".";
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
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnRename = new System.Windows.Forms.Button();
			this.btnKeep = new System.Windows.Forms.Button();
			this.btnView = new System.Windows.Forms.Button();
			this.lblDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnDelete
			// 
			this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnDelete.Location = new System.Drawing.Point(40, 128);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.TabIndex = 0;
			this.btnDelete.Text = "Delete...";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnRename
			// 
			this.btnRename.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRename.Location = new System.Drawing.Point(136, 128);
			this.btnRename.Name = "btnRename";
			this.btnRename.TabIndex = 1;
			this.btnRename.Text = "Rename...";
			this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
			// 
			// btnKeep
			// 
			this.btnKeep.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnKeep.Location = new System.Drawing.Point(232, 128);
			this.btnKeep.Name = "btnKeep";
			this.btnKeep.TabIndex = 2;
			this.btnKeep.Text = "Keep...";
			this.btnKeep.Click += new System.EventHandler(this.btnKeep_Click);
			// 
			// btnView
			// 
			this.btnView.Enabled = false;
			this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnView.Location = new System.Drawing.Point(136, 80);
			this.btnView.Name = "btnView";
			this.btnView.TabIndex = 3;
			this.btnView.Text = "View...";
			// 
			// lblDescription
			// 
			this.lblDescription.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.lblDescription.Location = new System.Drawing.Point(24, 24);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(296, 40);
			this.lblDescription.TabIndex = 4;
			this.lblDescription.Text = "A local folder was deleted, but the remote folder was changed. Select what you wa" +
				"nt to do with the remote folder.";
			this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// DeleteConflictResolution
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(346, 173);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.btnView);
			this.Controls.Add(this.btnKeep);
			this.Controls.Add(this.btnRename);
			this.Controls.Add(this.btnDelete);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeleteConflictResolution";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Conflict";
			this.ResumeLayout(false);

		}
		#endregion

		private void btnDelete_Click(object sender, System.EventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this file?","Delete file?",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.Yes)
			{
				record.Status=RecordStatus.Delete;
				DialogResult=DialogResult.OK;
			}
		}

		private void btnKeep_Click(object sender, System.EventArgs e)
		{
			if (record.LocalEntry.Exists)
			{
				record.Status=RecordStatus.Upload;
			}
			else
			{
				record.Status=RecordStatus.Download;
			}
			DialogResult=DialogResult.OK;
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
					continue;
				}
				folder = record.RemoteEntry.Folder;
				if (folder[newname]!=null)
				{
					foundnewname=false;
					MessageBox.Show("A file or folder with that name already exists remotely.");
					continue;
				}
			}
			return newname;
		}

		private void btnRename_Click(object sender, System.EventArgs e)
		{
			string newname=FindNewName();
			if (newname!=null)
			{
				record.NewName=newname;
				if (record.LocalEntry.Exists)
				{
					record.Status=RecordStatus.LocalRename;
				}
				else
				{
					record.Status=RecordStatus.RemoteRename;
				}
				DialogResult=DialogResult.OK;
			}
		}
	}
}
