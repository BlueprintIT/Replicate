using System;
using System.Xml;
using log4net;
using BlueprintIT.Storage;

namespace BlueprintIT.Replicate.State
{
	/// <summary>
	/// Summary description for FolderMapping.
	/// </summary>
	public class FolderMapping
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private XmlElement element;

		public FolderMapping(XmlElement element)
		{
			this.element=element;
		}

		public string ID
		{
			get
			{
				if (element.HasAttribute("id",State.SYNC_NS))
				{
					return element.GetAttribute("id",State.SYNC_NS);
				}
				return null;
			}

			set
			{
				element.SetAttribute("id",State.SYNC_NS,value);
			}
		}

		public string Path
		{
			get
			{
				if (element.HasAttribute("path",State.SYNC_NS))
				{
					return element.GetAttribute("path",State.SYNC_NS);
				}
				return null;
			}

			set
			{
				element.SetAttribute("path",State.SYNC_NS,value);
			}
		}

		public bool Hidden
		{
			get
			{
				if (element.HasAttribute("hidden",State.SYNC_NS))
				{
					return Boolean.Parse(element.GetAttribute("hidden",State.SYNC_NS));
				}
				return false;
			}

			set
			{
				element.SetAttribute("hidden",State.SYNC_NS,value.ToString());
			}
		}

		public bool Exists
		{
			get
			{
				if (element.HasAttribute("exists",State.SYNC_NS))
				{
					return Boolean.Parse(element.GetAttribute("exists",State.SYNC_NS));
				}
				return false;
			}

			set
			{
				element.SetAttribute("exists",State.SYNC_NS,value.ToString());
			}
		}
	}
}
