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

		/// <summary>
		/// Stores the synchronisation log for the given folder.
		/// </summary>
		/// <param name="folder">The folder to persist information about.</param>
		private void PersistSyncLog(IFolder folder)
		{
			XmlDocument synclog = GetSyncLog(folder);

			IFolder dfolder = folder.GetFolder(SYNCFOLDER);
			if (!dfolder.Exists)
			{
				dfolder.Create();
				dfolder.Hidden=true;
			}

			IFile xmlfile = dfolder.GetFile(SYNCFILE);
			TextWriter writer = xmlfile.OverwriteText();
			synclog.Save(writer);
			writer.Close();
		}

		/// <summary>
		/// Gets the synchronisation log for a folder.
		/// </summary>
		/// <remarks>
		/// Will return either a new log or the cached log if one exists.
		/// </remarks>
		/// <param name="folder">The folder.</param>
		/// <returns>The synchronisation log. Guaranteed to be the same instance for the same folder.</returns>
		private XmlDocument GetSyncLog(IFolder folder)
		{
			if (logs.Contains(folder))
			{
				return (XmlDocument)logs[folder];
			}

			XmlDocument syncxml = new XmlDocument();

			// Creates the sync log folder if necessary.
			IFolder dfolder = folder.GetFolder(SYNCFOLDER);
			if (dfolder.Exists)
			{
				// Loads the file or creates a blank one.
				IFile xmlfile = dfolder.GetFile(SYNCFILE);
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
			}
			logs[folder]=syncxml;

			// Makes the xml document start right.
			XmlElement root = syncxml.DocumentElement;
			if (root==null)
			{
				root = syncxml.CreateElement("FolderInfo");
				root.SetAttribute("uri",folder.Uri.ToString());
				syncxml.AppendChild(root);
			}

			// Wipes the xml if the uri is not there or wrong.
			if ((!root.HasAttribute("uri"))||(root.GetAttribute("uri")!=folder.Uri.ToString()))
			{
				root.RemoveAll();
				root.SetAttribute("uri",folder.Uri.ToString());
			}

			return syncxml;
		}

		/// <summary>
		/// Creates a new record during the synchronisation scan.
		/// </summary>
		/// <param name="localrecords">Local records to the folder to hold the new record.</param>
		/// <param name="local">The local entry.</param>
		/// <param name="remote">The remote entry.</param>
		/// <returns>The new synchronisation record.</returns>
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
				throw new ArgumentNullException("At least local or remote must be non-null");
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

		/// <summary>
		/// Creates missing local and remote entries.
		/// </summary>
		/// <param name="record">The record ro fill in</param>
		/// <param name="local">The local folder to the record.</param>
		/// <param name="remote">The remote folder to the record.</param>
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
			XmlDocument syncxml = GetSyncLog(local);
			XmlElement root = syncxml.DocumentElement;

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
			if (localrecords!=null)
			{
				foreach (SyncRecord record in localrecords.Values)
				{
					if (record.Status==RecordStatus.Conflict)
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
		}

		private void DataTransfer(long size, Stream source, Stream target)
		{
			byte[] buffer = new byte[1024];
			int pos=0;
			int count;
			do
			{
				count = source.Read(buffer,0,buffer.Length);
				target.Write(buffer,0,count);
				pos+=count;
			} while (count>0);
			if (pos!=size)
			{
				throw new IOException("Invalid number of bytes read from stream");
			}
		}

		private void TransferFile(SyncRecord record, IFile source, IFile target)
		{
			int pos = 0;
			IFile tempfile;
			do
			{
				tempfile = target.Folder.GetFile(target.Name+"_repltemp"+pos);
				pos++;
			} while (tempfile.Exists);

			// Do the transfer
			Stream input = null;
			Stream output = null;
			try
			{
				tempfile.Create();
				output = tempfile.Overwrite();
				input = source.Open();
				DataTransfer(source.Size,input,output);
				output.Close();
				input.Close();
				target.Delete();
				tempfile.Name=source.Name;
				if (record.LocalEntry==target)
				{
					record.LocalEntry=tempfile;
				}
				else if (record.RemoteEntry==target)
				{
					record.RemoteEntry=tempfile;
				}
			}
			catch (IOException)
			{
				output.Close();
				tempfile.Delete();
				input.Close();
			}
		}

		private void SynchroniseFile(SyncRecord record)
		{
			if (record.Status==RecordStatus.Upload)
			{
				TransferFile(record,(IFile)record.LocalEntry,(IFile)record.RemoteEntry);
			}
			else if (record.Status==RecordStatus.Download)
			{
				TransferFile(record,(IFile)record.RemoteEntry,(IFile)record.LocalEntry);
			}
		}

		private void TransferFolder(SyncRecord record)
		{
			IFolder source;
			if (record.RemoteEntry.Exists)
			{
				record.LocalEntry.Create();
				source=(IFolder)record.RemoteEntry;
			}
			else if (record.LocalEntry.Exists)
			{
				record.RemoteEntry.Create();
				source=(IFolder)record.LocalEntry;
			}
			else
			{
				return;
			}

			XmlDocument syncxml = GetSyncLog((IFolder)record.LocalEntry);

			foreach (IFolder folder in source.Folders)
			{
				if (folder.Name!=SYNCFOLDER)
				{
					SyncRecord subrecord = new SyncRecord(folder.Name);
					subrecord.LocalEntry=((IFolder)record.LocalEntry).GetFolder(folder.Name);
					subrecord.RemoteEntry=((IFolder)record.RemoteEntry).GetFolder(folder.Name);
					subrecord.Description=syncxml.CreateElement("Folder");
					syncxml.DocumentElement.AppendChild(subrecord.Description);
					TransferFolder(subrecord);
					PersistSyncLog((IFolder)record.LocalEntry);
				}
			}

			foreach (IFile file in source.Files)
			{
				SyncRecord subrecord = new SyncRecord(file.Name);
				subrecord.LocalEntry=((IFolder)record.LocalEntry).GetFile(file.Name);
				subrecord.RemoteEntry=((IFolder)record.RemoteEntry).GetFile(file.Name);
				subrecord.Description=syncxml.CreateElement("File");
				syncxml.DocumentElement.AppendChild(subrecord.Description);
				if (source==record.LocalEntry)
				{
					TransferFile(subrecord,(IFile)subrecord.LocalEntry,(IFile)subrecord.RemoteEntry);
				}
				else
				{
					TransferFile(subrecord,(IFile)subrecord.RemoteEntry,(IFile)subrecord.LocalEntry);
				}
				PersistSyncLog((IFolder)record.LocalEntry);
			}
		}

		private void SynchroniseFolder(SyncRecord record)
		{
			TransferFolder(record);
		}

		/// <summary>
		/// Synchronises a particular entry.
		/// </summary>
		/// <param name="record"></param>
		private void Synchronise(SyncRecord record)
		{
			if (record.Status==RecordStatus.RemoteRename)
			{
				SyncRecord newrecord = new SyncRecord(record.NewName);
				newrecord.RemoteEntry=record.RemoteEntry;
				if (record.LocalEntry is IFolder)
				{
					record.RemoteEntry=record.RemoteEntry.Folder.GetFolder(record.Name);
				}
				else
				{
					record.RemoteEntry=record.RemoteEntry.Folder.GetFile(record.Name);
				}
				if (newrecord.RemoteEntry is IFolder)
				{
					newrecord.LocalEntry=record.LocalEntry.Folder.GetFolder(newrecord.Name);
				}
				else
				{
					newrecord.LocalEntry=record.LocalEntry.Folder.GetFile(newrecord.Name);
				}
				newrecord.Status=RecordStatus.Download;
				record.Status=RecordStatus.Upload;
				newrecord.RemoteEntry.Name=newrecord.Name;
				Synchronise(newrecord);
			}
			else if (record.Status==RecordStatus.LocalRename)
			{
				SyncRecord newrecord = new SyncRecord(record.NewName);
				newrecord.LocalEntry=record.LocalEntry;
				if (record.RemoteEntry is IFolder)
				{
					record.LocalEntry=record.LocalEntry.Folder.GetFolder(record.Name);
				}
				else
				{
					record.LocalEntry=record.LocalEntry.Folder.GetFile(record.Name);
				}
				if (newrecord.LocalEntry is IFolder)
				{
					newrecord.RemoteEntry=record.RemoteEntry.Folder.GetFolder(newrecord.Name);
				}
				else
				{
					newrecord.RemoteEntry=record.RemoteEntry.Folder.GetFile(newrecord.Name);
				}
				newrecord.Status=RecordStatus.Upload;
				record.Status=RecordStatus.Download;
				newrecord.LocalEntry.Name=newrecord.Name;
				Synchronise(newrecord);
			}

			if (record.Description==null)
			{
				XmlDocument synclog = GetSyncLog(record.LocalEntry.Folder);
				if (record.LocalEntry is IFile)
				{
					record.Description=synclog.CreateElement("File");
				}
				else
				{
					record.Description=synclog.CreateElement("Folder");
				}
				synclog.DocumentElement.AppendChild(record.Description);
			}

			if (record.Status==RecordStatus.Delete)
			{
				record.LocalEntry.Delete();
				record.RemoteEntry.Delete();
			}
			else if ((record.Status==RecordStatus.Download)||
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
			else if ((record.LocalEntry is IFolder)&&(record.LocalEntry.Exists))
			{
				IDictionary localrecords = (IDictionary)folderrecords[record.LocalEntry];
				foreach (SyncRecord subrecord in localrecords.Values)
				{
					Synchronise(subrecord);
				}
			}
			if (record.Status!=RecordStatus.Ignore)
			{
				record.Serialise();
				PersistSyncLog(record.LocalEntry.Folder);
			}
		}

		/// <summary>
		/// Synchronises all the entries in a folder.
		/// </summary>
		private void Synchronise()
		{
			IDictionary localrecords = (IDictionary)folderrecords[local.Root];
			foreach (SyncRecord record in localrecords.Values)
			{
				Synchronise(record);
			}
		}

		/// <summary>
		/// Scans the stores.
		/// </summary>
		private void Scan()
		{
			Scan(local.Root,remote.Root);
			ResolveConflicts(local.Root);
			Synchronise();
		}
	}
}
