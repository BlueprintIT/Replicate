/*
 * $HeadURL$
 * $LastChangedBy$
 * $Date$
 * $Revision$
 */

using System;
using Microsoft.Win32;
using log4net;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for SyncSettings.
	/// </summary>
	public class SyncSettings
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private Uri local;
		private Uri remote;
		private string name;
		private string id;

		public SyncSettings()
		{
		}

		public void Test(params object[] args)
		{
		}

		public SyncSettings(string id)
		{
			this.id=id;
			Load();
		}

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

		public Uri LocalUri
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

		public Uri RemoteUri
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

		public override string ToString()
		{
			return name;
		}

		public void Load()
		{
			if (id!=null)
			{
				RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Blueprint IT Ltd\Replicate\Settings\"+id);
				local=new Uri((string)key.GetValue("LocalUri"));
				remote=new Uri((string)key.GetValue("RemoteUri"));
				name=(string)key.GetValue("Name");
			}
		}

		public void Save()
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Blueprint IT Ltd\Replicate\Settings");
			if (id==null)
			{
				int pos = 0;
				while (key.OpenSubKey(""+pos)!=null)
				{
					pos++;
				}
				id=""+pos;
			}
			key=key.CreateSubKey(id);
			key.SetValue("LocalUri",local.ToString());
			key.SetValue("RemoteUri",remote.ToString());
			key.SetValue("Name",name);
		}
	}
}
