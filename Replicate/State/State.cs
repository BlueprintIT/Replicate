/*
 * $HeadURL$
 * $LastChangedBy$
 * $Date$
 * $Revision$
 */

using System;
using System.IO;
using System.Xml;
using log4net;
using System.Collections;

namespace BlueprintIT.Replicate.State
{
	/// <summary>
	/// Summary description for State.
	/// </summary>
	public class State
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		public static string SYNC_NS = "http://www.blueprintit.co.uk/replicate#";

		private XmlDocument xmlstate;
		private XmlElement state;
		private FileInfo file;
		private Random random = new Random();

		private IDictionary foldersById;
		private IDictionary filesById;
		private IDictionary mappings;

		public State(FileInfo file)
		{
			this.file=file;
			foldersById = new Hashtable();
			filesById = new Hashtable();
			mappings = new Hashtable();

			Load();
		}

		public Folder GetFolderById(long id)
		{
			return (Folder)foldersById[id];
		}

		public Folder GetFolderByPath(string mapping, string path)
		{
			return null;
		}

		private long GenerateFolderID()
		{
			long id = 0;
			do
			{
				id = random.Next();
			} while (foldersById.Contains(id));
			return id;
		}

		public ICollection Folders
		{
			get
			{
				return foldersById.Values;
			}
		}

		public Folder AddFolder(long id)
		{
			XmlElement element = xmlstate.CreateElement("sync","folder",SYNC_NS);
			state.AppendChild(element);
			Folder folder = new Folder(element);
			folder.ID=id;
			foldersById[folder.ID]=folder;
			return folder;
		}

		public Folder AddFolder()
		{
			return AddFolder(GenerateFolderID());
		}

		public void RemoveFolder(Folder folder)
		{
			foldersById.Remove(folder.ID);
		}

		public File GetFileById(long id)
		{
			return (File)filesById[id];
		}

		public File GetFileByPath(string mapping, string path)
		{
			return null;
		}

		public ICollection Files
		{
			get
			{
				return filesById.Values;
			}
		}

		private long GenerateFileID()
		{
			long id = 0;
			do
			{
				id = random.Next();
			} while (filesById.Contains(id));
			return id;
		}

		public File AddFile(long id)
		{
			XmlElement element = xmlstate.CreateElement("sync","file",SYNC_NS);
			state.AppendChild(element);
			File file = new File(element);
			file.ID=id;
			filesById[file.ID]=file;
			return file;
		}

		public File AddFile()
		{
			return AddFile(GenerateFileID());
		}

		public void RemoveFile(File file)
		{
			filesById.Remove(file.ID);
		}

		public string Name
		{
			get
			{
				if (state.HasAttribute("name",SYNC_NS))
				{
					return state.GetAttribute("name",SYNC_NS);
				}
				return null;
			}

			set
			{
				state.SetAttribute("name",SYNC_NS,value);
			}
		}

		public ICollection Mappings
		{
			get
			{
				return mappings.Values;
			}
		}

		public StateMapping this[string id]
		{
			get
			{
				return (StateMapping)mappings[id];
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public Uri LocalUri
		{
			get
			{
				return this["local"].Uri;
			}

			set
			{
				this["local"].Uri=value;
			}
		}

		public Uri RemoteUri
		{
			get
			{
				return this["remote"].Uri;
			}

			set
			{
				this["remote"].Uri=value;
			}
		}

		public bool HasMapping(string id)
		{
			return mappings.Contains(id);
		}

		public StateMapping CreateMapping(string id)
		{
			if (HasMapping(id))
			{
				return this[id];
			}
			XmlElement el = xmlstate.CreateElement("sync","mapping",State.SYNC_NS);
			state.AppendChild(el);
			StateMapping mapping = new StateMapping(el);
			mapping.ID=id;
			mappings[mapping.ID]=mapping;
			return mapping;
		}

		private void Load()
		{
			foldersById.Clear();
			filesById.Clear();
			mappings.Clear();

			xmlstate=null;

			if (file.Exists)
			{
				xmlstate = new XmlDocument();
				xmlstate.Load(file.FullName);
				state = xmlstate.DocumentElement;
				if ((state==null)||(state.LocalName!="sync")||(state.NamespaceURI!=SYNC_NS))
				{
					log.Warn("Invalid state xml file");
					xmlstate=null;
				}
				else
				{
					foreach (XmlElement el in state.GetElementsByTagName("mapping",SYNC_NS))
					{
						StateMapping mapping = new StateMapping(el);
						mappings[mapping.ID]=mapping;
					}
					foreach (XmlElement element in state.GetElementsByTagName("folder",SYNC_NS))
					{
						Folder folder = new Folder(element);
						foldersById[folder.ID]=folder;
					}
					foreach (XmlElement el in state.GetElementsByTagName("file",SYNC_NS))
					{
						File f = new File(el);
						filesById[f.ID]=file;
					}
				}
			}
			
			if (xmlstate==null)
			{
				xmlstate = new XmlDocument();
				state = xmlstate.CreateElement("sync","sync",SYNC_NS);
				xmlstate.AppendChild(state);
				CreateMapping("local");
				CreateMapping("remote");
			}
		}

		public void Persist()
		{
			xmlstate.Save(file.FullName);
		}
	}
}
