/*
 * $HeadURL$
 * $LastChangedBy$
 * $Date$
 * $Revision$
 */

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using BlueprintIT.Storage;
using log4net;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// The initial class of a synchronise session.
	/// </summary>
	public class Replicate
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private IList syncstates = new ArrayList();

		/// <summary>
		/// Creates the Synchonise class.
		/// </summary>
		public Replicate()
		{
			DirectoryInfo userdir = new DirectoryInfo(Environment.GetFolderPath(
								Environment.SpecialFolder.LocalApplicationData)+"\\Blueprint IT Ltd\\Replicate");
			DirectoryInfo machinedir = new DirectoryInfo(Environment.GetFolderPath(
								Environment.SpecialFolder.CommonApplicationData)+"\\Blueprint IT Ltd\\Replicate");
			LoadStates(userdir);
			LoadStates(machinedir);
		}

		public void LoadStates(DirectoryInfo dir)
		{
			if (!dir.Exists)
			{
				dir.Create();
			}
			foreach (FileInfo file in dir.GetFiles("*.sync"))
			{
				State.State state = new State.State(file);
				syncstates.Add(state);
			}
		}

		/// <summary>
		/// Displays the GUI for a synchronise session.
		/// </summary>
		public void ShowGUI()
		{
			Application.EnableVisualStyles();
			Application.Run(new SyncSelector(syncstates));
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			Replicate sync = new Replicate();
			
			if (args.Length==0)
			{
				sync.ShowGUI();
			}
		}
	}
}
