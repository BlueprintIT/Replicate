using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BlueprintIT.Storage;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for DeleteConflictResolution.
	/// </summary>
	public class DeleteConflictResolution : System.Windows.Forms.Form
	{
		private SyncRecord record;
		private System.Windows.Forms.Button btnDelete;
		private System.Windows.Forms.Button btnRename;
		private System.Windows.Forms.Button btnKeep;
		private System.Windows.Forms.Button btnView;
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
			this.SuspendLayout();
			// 
			// btnDelete
			// 
			this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnDelete.Location = new System.Drawing.Point(104, 272);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.TabIndex = 0;
			this.btnDelete.Text = "Delete...";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnRename
			// 
			this.btnRename.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRename.Location = new System.Drawing.Point(208, 272);
			this.btnRename.Name = "btnRename";
			this.btnRename.TabIndex = 1;
			this.btnRename.Text = "Rename...";
			this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
			// 
			// btnKeep
			// 
			this.btnKeep.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnKeep.Location = new System.Drawing.Point(312, 272);
			this.btnKeep.Name = "btnKeep";
			this.btnKeep.TabIndex = 2;
			this.btnKeep.Text = "Keep...";
			this.btnKeep.Click += new System.EventHandler(this.btnKeep_Click);
			// 
			// btnView
			// 
			this.btnView.Enabled = false;
			this.btnView.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnView.Location = new System.Drawing.Point(208, 224);
			this.btnView.Name = "btnView";
			this.btnView.TabIndex = 3;
			this.btnView.Text = "View...";
			// 
			// DeleteConflictResolution
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(490, 317);
			this.Controls.Add(this.btnView);
			this.Controls.Add(this.btnKeep);
			this.Controls.Add(this.btnRename);
			this.Controls.Add(this.btnDelete);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DeleteConflictResolution";
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
