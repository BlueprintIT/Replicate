/*
 * $HeadURL$
 * $LastChangedBy$
 * $Date$
 * $Revision$
 */

using System;
using System.Xml;
using System.Collections;
using log4net;

namespace BlueprintIT.Replicate.State
{
	/// <summary>
	/// Summary description for Folder.
	/// </summary>
	public class Folder
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private State store;
		private XmlElement element;
		private IDictionary mappings;

		public Folder(XmlElement element)
		{
			this.store=store;
			this.element=element;
			mappings = new Hashtable();
			foreach (XmlElement el in element.GetElementsByTagName("mapping",State.SYNC_NS))
			{
				FolderMapping mapping = new FolderMapping(el);
				mappings[mapping.ID]=mapping;
			}
		}

		public FolderMapping this[string id]
		{
			get
			{
				return (FolderMapping)mappings[id];
			}
		}

		public long ID
		{
			get
			{
				return long.Parse(element.GetAttribute("id",State.SYNC_NS));
			}

			set
			{
				element.SetAttribute("id",State.SYNC_NS,value.ToString());
			}
		}

		public bool HasMapping(string id)
		{
			return mappings.Contains(id);
		}

		public FolderMapping CreateMapping(string id)
		{
			if (HasMapping(id))
			{
				return this[id];
			}
			XmlElement el = element.OwnerDocument.CreateElement("sync","mapping",State.SYNC_NS);
			element.AppendChild(el);
			FolderMapping mapping = new FolderMapping(el);
			mapping.ID=id;
			mappings[mapping.ID]=mapping;
			return mapping;
		}
	}
}
