using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections;
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
		private IDictionary records = new Hashtable();
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

		private SyncRecord CreateRecord(IEntry local, IEntry remote, IDictionary list)
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
				if (records.Contains(entry.Path))
				{
					record = (SyncRecord)records[entry.Path];
				}
				else
				{
					record = new SyncRecord(entry.Name);
					records[entry.Path]=record;
					list[entry.Name]=record;
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

			IDictionary list = new Hashtable();
			IList localfolders = new ArrayList();

			// Makes sync records for local folders and files.
			foreach (IEntry entry in local.Folders)
			{
				SyncRecord record = CreateRecord(entry,null,list);
				if (record!=null)
					localfolders.Add(record);
			}
			foreach (IEntry entry in local.Files)
			{
				CreateRecord(entry,null,list);
			}

			// Makes sync records for remote folders and files.
			if (remote!=null)
			{
				foreach (IEntry entry in remote.Folders)
				{
					CreateRecord(null,entry,list);
				}
				foreach (IEntry entry in remote.Files)
				{
					CreateRecord(null,entry,list);
				}
			}

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
			foreach (SyncRecord record in localfolders)
			{
				if (record.RemoteEntry.Exists)
				{
					if ((record.LocalEntry is IFolder)&&(record.RemoteEntry is IFolder))
					{
						Scan((IFolder)record.LocalEntry,(IFolder)record.RemoteEntry);
					}
				}
			}
		}

		/// <summary>
		/// Scans the stores.
		/// </summary>
		private void Scan()
		{
			Scan(local.Root,remote.Root);
			foreach (string path in records.Keys)
			{
				SyncRecord record = (SyncRecord)records[path];
				Debug.WriteLine(path+" "+record.Status);
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
				if ((record.Status==RecordStatus.TypeConflict)||(record.Status==RecordStatus.Conflict))
				{
					(new ConflictResolution(record)).ShowDialog();
				}
			}
		}
	}
}
