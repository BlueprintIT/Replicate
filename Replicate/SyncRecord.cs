using System;
using System.Xml;
using BlueprintIT.Storage;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// The state of an entry.
	/// </summary>
	public enum EntryStatus
	{
		Unknown, Nonexistent, Added, Removed, Updated, Unchanged, Replaced
	}

	/// <summary>
	/// The state of the entire record.
	/// </summary>
	public enum RecordStatus
	{
		Unknown, Ignore, Upload, Download, Delete, Conflict, LocalRename, RemoteRename
	}

	/// <summary>
	/// Holds information about what has changed remotely and locally about a particular file.
	/// </summary>
	public class SyncRecord
	{
		private IEntry local = null;
		private IEntry remote = null;
		private string name;
		private string newname;
		private XmlElement description;
		private EntryStatus localStatus = EntryStatus.Unknown;
		private EntryStatus remoteStatus = EntryStatus.Unknown;
		private RecordStatus status = RecordStatus.Unknown;

		private static string DATEFORMAT = "yyyy-MM-dd HH:mm:ss.fff";

		/// <summary>
		/// Creates a record for a given entry name.
		/// </summary>
		/// <param name="name">The entry name.</param>
		public SyncRecord(string name)
		{
			this.name=name;
		}

		/// <summary>
		/// Gets or sets the name for this entry.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				name=value;
			}
		}

		/// <summary>
		/// Gets or sets the new name associated with a rename operation.
		/// </summary>
		public string NewName
		{
			get
			{
				return newname;
			}

			set
			{
				newname=value;
			}
		}

		/// <summary>
		/// Gets or sets the local entry for this record.
		/// </summary>
		public IEntry LocalEntry
		{
			get
			{
				return local;
			}

			set
			{
				local=value;
			}
		}

		/// <summary>
		/// Gets or sets the remote entry for this record.
		/// </summary>
		public IEntry RemoteEntry
		{
			get
			{
				return remote;
			}

			set
			{
				remote=value;
			}
		}

		/// <summary>
		/// Gets the determined status of the local entry.
		/// </summary>
		public EntryStatus LocalStatus
		{
			get
			{
				return localStatus;
			}
		}

		/// <summary>
		/// Gets the determined status of the remote entry.
		/// </summary>
		public EntryStatus RemoteStatus
		{
			get
			{
				return remoteStatus;
			}
		}

		/// <summary>
		/// Gets or sets the determined status of the record.
		/// </summary>
		/// <remarks>
		/// The status is calculated by a call to MakeDecision, but can be forced to something
		/// else.
		/// </remarks>
		public RecordStatus Status
		{
			get
			{
				return status;
			}

			set
			{
				status=value;
			}
		}

		/// <summary>
		/// Gets or sets the XML based description of this record.
		/// </summary>
		public XmlElement Description
		{
			get
			{
				return description;
			}

			set
			{
				description=value;
			}
		}

		/// <summary>
		/// Determines the status of a given entry.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="element">The xml description of this entry.</param>
		/// <returns></returns>
		private EntryStatus MakeEntryDecision(IEntry entry, XmlElement element)
		{
			if ((entry.Exists)&&(entry is IFile))
			{
				if (description.Name=="Folder")
				{
					return EntryStatus.Replaced;
				}
				IFile file = (IFile)entry;
				if (element!=null)
				{
					long lastsize = long.Parse(element.GetAttribute("size"));
					if (lastsize!=file.Size)
					{
						return EntryStatus.Updated;
					}
					string currentdate = file.Date.ToString(DATEFORMAT);
					if (currentdate!=element.GetAttribute("date"))
					{
						return EntryStatus.Updated;
					}
					return EntryStatus.Unchanged;
				}
				else
				{
					return EntryStatus.Added;
				}
			}
			else if ((entry.Exists)&&(entry is IFolder))
			{
				if (description.Name=="File")
				{
					return EntryStatus.Replaced;
				}
				return EntryStatus.Unchanged;
			}
			else
			{
				return EntryStatus.Removed;
			}
		}

		public void Serialise()
		{
			if ((local.Exists)&&(remote.Exists))
			{
				description.SetAttribute("name",name);
				XmlElement details = null;
				foreach (XmlElement element in description.GetElementsByTagName("Details"))
				{
					if (element.GetAttribute("remoteUri")==remote.Uri.ToString())
					{
						details=element;
						break;
					}
				}
				if (details==null)
				{
					details=description.OwnerDocument.CreateElement("Details");
					details.SetAttribute("remoteUri",remote.Uri.ToString());
					description.AppendChild(details);
				}
				if (local is IFile)
				{
					IFile file = (IFile)local;
					XmlElement element = (XmlElement)details.GetElementsByTagName("Local").Item(0);
					if (element==null)
					{
						element = details.OwnerDocument.CreateElement("Local");
						details.AppendChild(element);
					}
					element.SetAttribute("size",file.Size+"");
					element.SetAttribute("date",file.Date.ToString(DATEFORMAT));
				}
				else
				{
					XmlElement element = (XmlElement)details.GetElementsByTagName("Local").Item(0);
					if (element!=null)
					{
						details.RemoveChild(element);
					}
				}
				if (remote is IFile)
				{
					IFile file = (IFile)remote;
					XmlElement element = (XmlElement)details.GetElementsByTagName("Remote").Item(0);
					if (element==null)
					{
						element = details.OwnerDocument.CreateElement("Remote");
						details.AppendChild(element);
					}
					element.SetAttribute("size",file.Size+"");
					element.SetAttribute("date",file.Date.ToString(DATEFORMAT));
				}
				else
				{
					XmlElement element = (XmlElement)details.GetElementsByTagName("Remote").Item(0);
					if (element!=null)
					{
						details.RemoveChild(element);
					}
				}
			}
			else
			{
				description.ParentNode.RemoveChild(description);
			}
		}

		/// <summary>
		/// Makes a decision based on the information in this record.
		/// </summary>
		public void MakeDecision()
		{
			// Makes the local and remote statuses.
			XmlElement details = null;
			if (description!=null)
			{
				foreach (XmlElement element in description.GetElementsByTagName("Details"))
				{
					if (element.GetAttribute("remoteUri")==remote.Uri.ToString())
					{
						details=element;
						break;
					}
				}
			}
			if (details!=null)
			{
				XmlElement localelement = (XmlElement)details.GetElementsByTagName("Local").Item(0);
				XmlElement remoteelement = (XmlElement)details.GetElementsByTagName("Remote").Item(0);
				remoteStatus=MakeEntryDecision(remote,remoteelement);
				localStatus=MakeEntryDecision(local,localelement);
			}
			else
			{
				if (local.Exists)
				{
					localStatus=EntryStatus.Added;
				}
				else
				{
					localStatus=EntryStatus.Nonexistent;
				}
				if (remote.Exists)
				{
					remoteStatus=EntryStatus.Added;
				}
				else
				{
					remoteStatus=EntryStatus.Nonexistent;
				}
			}

			// Makes the final status for the record.
			/*if ((((local is IFile)&&(remote is IFolder))||((local is IFolder)&&(remote is IFile)))&&
				((local.Exists)&&(remote.Exists)))
			{
				status=RecordStatus.TypeConflict;
				return;
			}*/

			if ((localStatus==EntryStatus.Unchanged)&&(remoteStatus==EntryStatus.Unchanged))
			{
				status=RecordStatus.Ignore;
				return;
			}
			if ((localStatus==EntryStatus.Removed)&&(remoteStatus==EntryStatus.Removed))
			{
				status=RecordStatus.Ignore;
				return;
			}
			if ((localStatus==EntryStatus.Nonexistent)&&(remoteStatus==EntryStatus.Nonexistent))
			{
				status=RecordStatus.Ignore;
				return;
			}
			if (((localStatus==EntryStatus.Added)||(localStatus==EntryStatus.Updated)||(localStatus==EntryStatus.Replaced))&&
				((remoteStatus==EntryStatus.Added)||(remoteStatus==EntryStatus.Updated)||(remoteStatus==EntryStatus.Replaced)))
			{
				if ((local is IFolder)&&(remote is IFolder))
				{
					status=RecordStatus.Ignore;
					return;
				}
				else
				{
					status=RecordStatus.Conflict;
					return;
				}
			}
			if ((localStatus==EntryStatus.Removed)&&((remoteStatus==EntryStatus.Added)||(remoteStatus==EntryStatus.Replaced)))
			{
				status=RecordStatus.Download;
				return;
			}
			if ((remoteStatus==EntryStatus.Removed)&&((localStatus==EntryStatus.Added)||(localStatus==EntryStatus.Replaced)))
			{
				status=RecordStatus.Upload;
				return;
			}
			if ((localStatus==EntryStatus.Removed)&&(remoteStatus==EntryStatus.Updated))
			{
				status=RecordStatus.Conflict;
				return;
			}
			if ((remoteStatus==EntryStatus.Removed)&&(localStatus==EntryStatus.Updated))
			{
				status=RecordStatus.Conflict;
				return;
			}
			if ((remoteStatus==EntryStatus.Removed)||(localStatus==EntryStatus.Removed))
			{
				status=RecordStatus.Delete;
				return;
			}
			if ((localStatus==EntryStatus.Added)||(localStatus==EntryStatus.Updated)||(localStatus==EntryStatus.Replaced))
			{
				status=RecordStatus.Upload;
				return;
			}
			if ((remoteStatus==EntryStatus.Added)||(remoteStatus==EntryStatus.Updated)||(remoteStatus==EntryStatus.Replaced))
			{
				status=RecordStatus.Download;
				return;
			}
		}
	}
}
