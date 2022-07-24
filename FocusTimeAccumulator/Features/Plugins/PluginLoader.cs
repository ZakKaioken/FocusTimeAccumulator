using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FocusTimeAccumulator.Features.Plugins
{
	internal class PluginLoader
	{
		List<Plugin> plugins = new List<Plugin>( );
		internal static List<Plugin?>? LoadPlugins<T>( string path )
		{
			SaveData.CreateMissingPath( path );
			var files = Directory.GetFiles( path, "*.dll", SearchOption.AllDirectories );

			List<Plugin?> plugins = new List<Plugin?>( );

			foreach ( var file in files )
			{
				Assembly plugin = Assembly.LoadFrom( file );
				var type = typeof( T );
				List<Plugin?>? plugsInAssemby = AppDomain.CurrentDomain.GetAssemblies( )
				   .SelectMany( s => s.GetTypes( ) )
				   .Where( t =>
					   t != type &&
					   type.IsAssignableFrom( t )
					   ).Select( tt => (Plugin)Activator.CreateInstance( tt ) ).ToList( );
				plugins.AddRange( plugsInAssemby );
			}
			return plugins;
		}
	}
}
