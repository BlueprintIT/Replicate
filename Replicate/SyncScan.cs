using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;
using BlueprintIT.Storage;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Scans over the local and remote folders to find what has changed since the last synchronisation.
	/// </summary>
	public class SyncScan
	{
		private IStore local,remote;
		private static string SYNCFOLDER = ".Sync";
		private static string SYNCFILE = "synclog.xml";

		// Maps between IFolders and an IDictionary which maps between names and SyncRecords
		private IDictionary folderrecords = new Hashtable();
		// Maps between IFolders and XMLDocuments.
		private IDictionary logs = new Hashtable();

		/// <summary>
		/// Creates the class.
		/// </summary>
		/// <param name="local">The local store.</param>
		/// <param name="remote">The remote store.</param>
		public SyncScan(IStore local, IStore remote)
		{
			this.local=local;
			this.remote=remote;
			Scan();
		}

		private SyncRecord CreateRecord(IDictionary localrecords, IEntry local, IEntry remote)
		{
			SyncRecord record;
			IEntry entry;
			if (local!=null)
			{
				entry=local;
			}
			else if (remote!=null)
			{
				entry=remote;
			}
			else
			{
				throw new ArgumentNullException("At least local or remot must be non-null");
			}
			if (entry.Name!=SYNCFOLDER)
			{
				if (localrecords.Contains(entry.Name))
				{
					record = (SyncRecord)localrecords[entry.Name];
				}
				else
				{
					record = new SyncRecord(entry.Name);
					localrecords[entry.Name]=record;
				}

				if (local!=null)
				{
					record.LocalEntry=local;
				}

				if (remote!=null)
				{
					record.RemoteEntry=remote;
				}
				return record;
			}
			return null;
		}

		private void FillRecord(SyncRecord record, IFolder local, IFolder remote)
		{
			if (record.LocalEntry==null)
			{
				if (record.RemoteEntry is IFile)
				{
					record.LocalEntry=local.GetFile(record.RemoteEntry.Name);
				}
				else
				{
					record.LocalEntry=local.GetFolder(record.RemoteEntry.Name);
				}
			}
			if (record.RemoteEntry==null)
			{
				if (record.LocalEntry is IFile)
				{
					record.RemoteEntry=remote.GetFile(record.LocalEntry.Name);
				}
				else
				{
					record.RemoteEntry=remote.GetFolder(record.LocalEntry.Name);
				}
			}
		}

		/// <summary>
		/// Scans a folder.
		/// </summary>
		/// <param name="local">The local folder.</param>
		/// <param name="remote">The remote equivalent of this folder.</param>
		private void Scan(IFolder local, IFolder remote)
		{
			// Creates the sync log folder if necessary.
			IFolder dfolder = local.GetFolder(SYNCFOLDER);
			if (!dfolder.Exists)
			{
				dfolder.Create();
				dfolder.Hidden=true;
			}

			// Loads the file or creates a blank one.
			IFile xmlfile = dfolder.GetFile(SYNCFILE);
			XmlDocument syncxml = new XmlDocument();
			if (xmlfile.Exists)
			{
				try
				{
					TextReader reader = xmlfile.OpenText();
					try
					{
						syncxml.Load(reader);
					}
					catch (Exception)
					{
						syncxml = new XmlDocument();
					}
					reader.Close();
				}
				catch
				{
				}
			}
			logs[xmlfile]=syncxml;

			// Makes the xml document start right.
			XmlElement root = syncxml.DocumentElement;
			if (root==null)
			{
				root = syncxml.CreateElement("FolderInfo");
				root.SetAttribute("uri",local.Uri.ToString());
				syncxml.AppendChild(root);
			}

			// Wipes the xml if the uri is not there or wrong.
			if ((!root.HasAttribute("uri"))||(root.GetAttribute("uri")!=local.Uri.ToString()))
			{
				root.RemoveAll();
				root.SetAttribute("uri",local.Uri.ToString());
			}

			IDictionary localrecords = new Hashtable();
			folderrecords[local]=localrecords;

			// Makes sync records for local folders and files.
			foreach (IEntry entry in local.Folders)
			{
				CreateRecord(localrecords,entry,null);
			}
			foreach (IEntry entry in local.Files)
			{
				CreateRecord(localrecords,entry,null);
			}

			// Makes sync records for remote folders and files.
			if (remote!=null)
			{
				foreach (IEntry entry in remote.Folders)
				{
					CreateRecord(localrecords,null,entry);
				}
				foreach (IEntry entry in remote.Files)
				{
					CreateRecord(localrecords,null,entry);
				}
			}

			IDictionary list = new Hashtable(localrecords);
			// Applies pre-existing xml descriptions to the sync records.
			foreach (XmlNode node in root.ChildNodes)
			{
				if ((node.NodeType==XmlNodeType.Element)&&((node.Name=="File")||(node.Name=="Folder")))
				{
					XmlElement element = (XmlElement)node;
					string name = element.GetAttribute("name");
					if (list.Contains(name))
					{
						SyncRecord record = (SyncRecord)list[name];
						FillRecord(record,local,remote);
						record.Description=element;
						record.MakeDecision();
						list.Remove(name);
					}
					else
					{
						// No sync record exists, so entry is not local or remote so must have been deleted from both.
						root.RemoveChild(element);
					}
				}
			}
			// Creates xml descriptions for sync records that don't have one yet.
			foreach (SyncRecord rec in list.Values)
			{
				//XmlElement element = syncxml.CreateElement("Entry");
				//root.AppendChild(element);
				FillRecord(rec,local,remote);
				//rec.Description=element;
				rec.MakeDecision();
			}

			// Scans sub folders that exist locally.
			foreach (SyncRecord record in localrecords.Values)
			{
				if ((record.RemoteEntry.Exists)&&(record.LocalEntry.Exists))
				{
					if ((record.LocalEntry is IFolder)&&(record.RemoteEntry is IFolder))
					{
						Scan((IFolder)record.LocalEntry,(IFolder)record.RemoteEntry);
					}
				}
			}
		}

		/// <summary>
		/// Resolves any detected conflicts.
		/// </summary>
		/// <param name="folder">The folder to work from.</param>
		private void ResolveConflicts(IFolder folder)
		{
			IDictionary localrecords = (IDictionary)folderrecords[folder];
			foreach (SyncRecord record in localrecords.Values)
			{
				if ((record.Status==RecordStatus.TypeConflict)||(record.Status==RecordStatus.Conflict))
				{
					if ((new ConflictResolution(localrecords,record)).ShowDialog()!=DialogResult.OK)
					{
						record.Status=RecordStatus.Ignore;
						continue;
					}																												 
				}
				if ((record.LocalEntry is IFolder)&&(record.LocalEntry.Exists))
				{
					ResolveConflicts((IFolder)record.LocalEntry);
				}
			}
		}

		private void SynchroniseFile(SyncRecord record)
		{
		}

		private void SynchroniseFolder(SyncRecord record)
		{
		}

		/// <summary>
		/// Synchronises all the entries in a folder.
		/// </summary>
		/// <param name="folder">The folder.</param>
		private void Synchronise(IFolder folder)
		{
			IDictionary localrecords = (IDictionary)folderrecords[folder];
			foreach (SyncRecord record in localrecords.Values)
			{
				Debug.WriteLine(record.LocalEntry.Path+" "+record.Status);
				if (record.LocalEntry.Exists)
				{
					Debug.WriteLine("  Local: "+record.LocalEntry.Uri+" ("+record.LocalStatus+")");
				}
				else
				{
					Debug.WriteLine("  Local: ("+record.LocalStatus+")");
				}
				if (record.RemoteEntry.Exists)
				{
					Debug.WriteLine("  Remote: "+record.RemoteEntry.Uri+" ("+record.RemoteStatus+")");
				}
				else
				{
					Debug.WriteLine("  Remote: ("+record.RemoteStatus+")");
				}

				if ((record.Status==RecordStatus.Delete)||
					(record.Status==RecordStatus.Download)||
					(record.Status==RecordStatus.Upload))
				{
					if ((record.LocalEntry is IFolder)&&(record.RemoteEntry is IFolder))
					{
						SynchroniseFolder(record);
					}

					if ((record.LocalEntry is IFile)&&(record.RemoteEntry is IFile))
					{
						SynchroniseFile(record);
					}
				}

				if ((record.LocalEntry is IFolder)&&(record.LocalEntry.Exists))
				{
					Synchronise((IFolder)record.LocalEntry);
				}
			}
		}

		/// <summary>
		/// Scans the stores.
		/// </summary>
		private void Scan()
		{
			Scan(local.Root,remote.Root);
			ResolveConflicts(local.Root);
			Synchronise(local.Root);
		}
	}
}
