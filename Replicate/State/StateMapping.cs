using System;
using System.Xml;
using log4net;

namespace BlueprintIT.Replicate.State
{
	/// <summary>
	/// Summary description for StateMapping.
	/// </summary>
	public class StateMapping
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private XmlElement element;

		public StateMapping(XmlElement element)
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

		public Uri Uri
		{
			get
			{
				if (element.HasAttribute("uri",State.SYNC_NS))
				{
					return new Uri(element.GetAttribute("uri",State.SYNC_NS));
				}
				return null;
			}

			set
			{
				element.SetAttribute("uri",State.SYNC_NS,value.ToString());
			}
		}
	}
}
