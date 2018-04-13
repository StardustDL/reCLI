using Newtonsoft.Json;
using reCLI.Core;
using reCLI.Core.Helpers;
using reCLI.Plugin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace reCLI.Infrastructure.Plugins
{
    public class PluginManager : IEnumerable<(IPlugin, PluginMetadata)>
    {
        public string PluginsRoot { get; set; }

        Dictionary<Guid, string> pluginDirectory = new Dictionary<Guid, string>();
        Dictionary<Guid, Tuple<List<IPlugin>, PluginMetadata>> plugins = new Dictionary<Guid, Tuple<List<IPlugin>, PluginMetadata>>();

        public IEnumerable<(T, PluginMetadata)> GetPluginsForFeature<T>() where T : IFeatures
        {
            return from x in plugins.Values from y in x.Item1 where y is T select ((T)y,x.Item2);
        }

        public IEnumerable<(IPlugin,PluginMetadata)> GetValidPlugins(Query query)
        {
            return from x in plugins.Values from y in x.Item1 where y.Keyword == query.Keyword select (y,x.Item2);
        }

        public Query ParseQuery(string raw)
        {
            raw = raw.TrimStart();
            int ind = raw.IndexOf(Query.TermSeperator);
            if (ind == -1)
            {
                return new Query { RawText = raw, Keyword = raw, Terms = new string[] { raw } };
            }
            else
            {
                string key = raw.Substring(0, ind), args = raw.Substring(ind).Trim();
                var terms = from s in raw.Split(Query.TermSeperator) where !String.IsNullOrEmpty(s) select s;
                return new Query
                {
                    RawText = raw,
                    Keyword = key,
                    Arguments = args,
                    Terms = terms.ToArray()
                };
            }
        }
        
        public async Task LoadPlugins()
        {
            await Task.Run(() =>
            {
                List<IPlugin> plugins = new List<IPlugin>();
                DirectoryInfo dir = new DirectoryInfo(PluginsRoot);
                foreach (var v in dir.EnumerateDirectories())
                {
                    plugins.Clear();
                    var meta = v.GetFiles("plugin.json");
                    if (meta.Length == 0) continue;
                    var metadata = JsonConvert.DeserializeObject<PluginMetadata>(FileIO.ReadText(meta[0].FullName));
                    
#if DEBUG
                    var assembly = Assembly.Load(AssemblyName.GetAssemblyName(Path.Combine(v.FullName, metadata.ExecuteFileName)));
                    foreach (var typeName in metadata.PluginTypes)
                    {
                        var type = assembly.GetType(typeName);
                        if (type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(IPlugin)))
                        {
                            var plugin = (IPlugin)Activator.CreateInstance(type);
                            plugins.Add(plugin);
                        }
                    }
#else
                    Assembly assembly;
                    try
                    {
                        assembly = Assembly.Load(AssemblyName.GetAssemblyName(Path.Combine(v.FullName,metadata.ExecuteFileName)));
                    }
                    catch //(Exception e)
                    {
                        //Log.Exception($"|PluginsLoader.CSharpPlugins|Couldn't load assembly for {metadata.Name}", e);
                        break;
                    }
                    foreach (var typeName in metadata.PluginTypes)
                    {
                        Type type;
                        try
                        {
                            type = assembly.GetType(typeName);
                            if (type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(IPlugin))){}
                            else throw new InvalidOperationException();
                        }
                        catch (InvalidOperationException)
                        {
                            //Log.Exception($"|PluginsLoader.CSharpPlugins|Can't find class implement IPlugin for <{metadata.Name}>", e);
                            break;
                        }
                        IPlugin plugin;
                        try
                        {
                            plugin = (IPlugin)Activator.CreateInstance(type);
                        }
                        catch// (Exception e)
                        {
                            //Log.Exception($"|PluginsLoader.CSharpPlugins|Can't create instance for <{metadata.Name}>", e);
                            break;
                        }
                        plugins.Add(plugin);
                    }
#endif
                    foreach (var plugin in plugins)RegisterPlugin(plugin, metadata, v.FullName);
                }
            });
        }

        public void RegisterPlugin(IPlugin plugin,PluginMetadata metadata,string directory=null)
        {
            if(!pluginDirectory.ContainsKey(metadata.ID)) pluginDirectory.Add(metadata.ID, directory);
            if (!plugins.ContainsKey(metadata.ID))
            {
                plugins.Add(metadata.ID, new Tuple<List<IPlugin>, PluginMetadata>(new List<IPlugin> { plugin },metadata));
            }
            else
            {
                plugins[metadata.ID].Item1.Add(plugin);
            }
        }

        public Task InitializePlugins(Func<PluginMetadata,IPublicAPI> api)
        {
            return Task.WhenAll(plugins.Values.SelectMany(
                pair => pair.Item1,
                (pair,plugin)=>plugin.Initialize(new PluginContext
            {
                PluginDirectory = pluginDirectory[pair.Item2.ID],
                ID=Guid.NewGuid(),
                API = api(pair.Item2),
                Metadata = pair.Item2
            })));
        }

        public Task UninitializePlugins()
        {
            return Task.WhenAll(plugins.Values.SelectMany(
                pair => pair.Item1,
                (pair, plugin) => plugin.Uninitialize()));
        }

        public IEnumerator<(IPlugin, PluginMetadata)> GetEnumerator()
        {
            foreach(var x in plugins.Values)
            {
                foreach(var y in x.Item1)
                {
                    yield return (y, x.Item2);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
