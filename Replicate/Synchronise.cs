using System;
using System.Collections;
using log4net;
using BlueprintIT.Storage;
using BlueprintIT.Replicate.State;

namespace BlueprintIT.Replicate
{
	/// <summary>
	/// Summary description for Synchronise.
	/// </summary>
	public class Synchronise
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private State.State state;

		private IStore localstore,remotestore;
		private IDictionary localmapping = new Hashtable();
		private IDictionary remotemapping = new Hashtable();

		public Synchronise(State.State state)
		{
			if ((!state.HasMapping("local"))&&(!state.HasMapping("remote")))
			{
				throw new ArgumentException("Invalid state encountered");
			}
			this.state=state;
		}

		private void MapFolderFromId(IDictionary mapping, string type, IFolder folder, IList pending)
		{
			pending.Add(folder);
			foreach (IFolder subfolder in folder.Folders)
			{
				MapFolderFromId(mapping,type,subfolder,pending);
			}
			foreach (IFile file in folder.Files)
			{
				pending.Add(file);
			}
		}

		private void MapPending(IDictionary mapping, string type, IList pending)
		{
			foreach (object obj in pending)
			{
				if (obj is IFolder)
				{
					IFolder folder = (IFolder)obj;
					Folder mapped = state.GetFolderByPath(type,folder.Path);
					if (mapping.Contains(mapped))
					{
						log.Info("Hopefully a renamed folder");
						mapped=null;
					}
					if (mapped!=null)
					{
						mapping[mapped]=folder;
					}
					else
					{
						mapped = state.AddFolder();
						mapped.CreateMapping(type).Path=folder.Path;
						mapping[mapped]=folder;
					}
				}
				else if (obj is IFile)
				{
					IFile file = (IFile)obj;
					File mapped = state.GetFileByPath(type,file.Path);
					if (mapping.Contains(mapped))
					{
						log.Info("Hopefully a renamed file");
						mapped=null;
					}
					if (mapped!=null)
					{
						mapping[mapped]=file;
					}
					else
					{
						mapped = state.AddFile();
						mapped.CreateMapping(type).Path=file.Path;
						mapping[mapped]=file;
					}
				}
				else
				{
					log.Warn("Invalid item found pending");
				}
			}
		}

		private void Map()
		{
			IList localpending = new ArrayList();
			IList remotepending = new ArrayList();

			MapFolderFromId(localmapping,"local",localstore.Root,localpending);
			MapFolderFromId(remotemapping,"remote",remotestore.Root,remotepending);

			MapPending(localmapping,"local",localpending);
			MapPending(remotemapping,"remote",remotepending);
		}

		private void Synch()
		{
			localstore = StorageProviders.OpenStore(state["local"].Uri);
			remotestore = StorageProviders.OpenStore(state["remote"].Uri);
		}
	}
}
