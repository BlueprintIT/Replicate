/*
 * $HeadURL$
 * $LastChangedBy$
 * $Date$
 * $Revision$
 */

using System;
using System.Windows.Forms;
using BlueprintIT.Storage;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// The initial class of a synchronise session.
	/// </summary>
	public class Synchronise
	{
		private StorageProviders providers = new StorageProviders();

		/// <summary>
		/// Creates the Synchonise class.
		/// </summary>
		public Synchronise()
		{
		}

		/// <summary>
		/// Displays the GUI for a synchronise session.
		/// </summary>
		public void ShowGUI()
		{
			Application.EnableVisualStyles();
			Application.Run(new SyncSelector(providers));
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			Synchronise sync = new Synchronise();
			
			if (args.Length==0)
			{
				sync.ShowGUI();
			}
		}
	}
}
