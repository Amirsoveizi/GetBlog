﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(GetBlog.Startup))]
namespace GetBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}