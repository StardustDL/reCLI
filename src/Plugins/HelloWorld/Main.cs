﻿using reCLI.Core;
using reCLI.Plugin;
using reCLI.Plugin.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HelloWorld
{
    public class Main : IPlugin
    {
        PluginContext context;

        public Guid ID { get => context.ID; }

        public string Name { get => "Hello World"; }

        public string Description { get => "Hello World"; }

        public string Keyword => "hw";

        public Task<bool> Initialize(PluginContext context)
        {
            return Task.Run(() =>
            {
                this.context = context;
                return true;
            });
        }

        public Task<IEnumerable<Answer>> Query(Query query)
        {
            return Task.Run(() =>
            {
                var result = new Answer
                {
                    Title = "Hello World",
                    SubTitle = $"Query: {query.Arguments}",
                    OriginalQuery = $"hw {query.Arguments}",
                    Execute = _ => { context.API.ShowMessage("Hello World, " + query.Arguments, TimeSpan.FromSeconds(2), MessageIcon.Success); return Task.FromResult(Result.NotAutoHide); }
                };
                return (IEnumerable<Answer>)(new Answer[] { result });
            });
        }

        public Task Uninitialize()
        {
            return Task.CompletedTask;
        }
    }
}
