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
	/// Summary description for File.
	/// </summary>
	public class File
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private XmlElement element;
		private IDictionary mappings;

		public File(XmlElement element)
		{
			this.element=element;
			mappings = new Hashtable();
			foreach (XmlElement el in element.GetElementsByTagName("mapping",State.SYNC_NS))
			{
				FileMapping mapping = new FileMapping(el);
				mappings[mapping.ID]=mapping;
			}
		}

		public FileMapping this[string id]
		{
			get
			{
				return (FileMapping)mappings[id];
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

		public FileMapping CreateMapping(string id)
		{
			if (HasMapping(id))
			{
				return this[id];
			}
			XmlElement el = element.OwnerDocument.CreateElement("sync","mapping",State.SYNC_NS);
			element.AppendChild(el);
			FileMapping mapping = new FileMapping(el);
			mapping.ID=id;
			mappings[mapping.ID]=mapping;
			return mapping;
		}
	}
}
