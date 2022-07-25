using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FocusTimeAccumulator.Features.Bucket;
using FocusTimeAccumulator.Features.Pool;

namespace FocusTimeAccumulator.Features.Plugins
{
	
	public class Plugin
	{
		//what to do when the program starts
		public virtual void OnStart( )
		{

		}
		//what to do every update
		public virtual void OnTick( )
		{

		}
		public virtual void OnProcessChanged(Process process, string prevName, string prevTitle, string appName, string appTitle ) {
			
		}
		public virtual void OnPoolSpanMerge(List<PoolApp.AppSpan> oldApps, PoolApp.AppSpan main)
		{

		}

		public virtual void OnBucketNameMerge( IEnumerable<KeyValuePair<string, int>> dead, KeyValuePair<string, int> main ) 
		{
			
		}

		public virtual void ModifyBucketPacketCreation( BucketApp.AppSpan packet ) 
		{
		
		}
		public virtual void ModifyPoolAppUpdate( PoolApp.AppSpan packet )
		{

		}
		public virtual void ModifyPoolAppCreation( PoolApp.AppSpan packet )
		{

		}
	}
}
