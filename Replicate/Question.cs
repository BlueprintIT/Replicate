using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for Question.
	/// </summary>
	public class Question : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox txtAnswer;
		private System.Windows.Forms.Label lblDescription;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Question()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		public static string Ask(string caption, string description, string defaultanswer)
		{
			Question q = new Question();
			q.Text=caption;
			q.Description=description;
			if (q.ShowDialog()==DialogResult.OK)
			{
				return q.Answer;
			}
			return null;
		}

		public static string Ask(string caption, string description)
		{
			return Ask(caption,description,"");
		}

		public static string Ask(string caption)
		{
			return Ask(caption,"","");
		}

		public string Description
		{
			get
			{
				return lblDescription.Text;
			}

			set
			{
				lblDescription.Text=value;
			}
		}

		public string Answer
		{
			get
			{
				return txtAnswer.Text;
			}

			set
			{
				txtAnswer.Text=value;
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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.txtAnswer = new System.Windows.Forms.TextBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(56, 88);
			this.button1.Name = "button1";
			this.button1.TabIndex = 0;
			this.button1.Text = "OK";
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button2.Location = new System.Drawing.Point(160, 88);
			this.button2.Name = "button2";
			this.button2.TabIndex = 1;
			this.button2.Text = "Cancel";
			// 
			// txtAnswer
			// 
			this.txtAnswer.Location = new System.Drawing.Point(16, 48);
			this.txtAnswer.Name = "txtAnswer";
			this.txtAnswer.Size = new System.Drawing.Size(256, 20);
			this.txtAnswer.TabIndex = 2;
			this.txtAnswer.Text = "";
			// 
			// lblDescription
			// 
			this.lblDescription.Location = new System.Drawing.Point(16, 16);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(256, 23);
			this.lblDescription.TabIndex = 3;
			// 
			// Question
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 131);
			this.ControlBox = false;
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.txtAnswer);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Question";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Question";
			this.ResumeLayout(false);

		}
		#endregion
	}
}
