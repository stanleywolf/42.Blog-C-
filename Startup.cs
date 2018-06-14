using System.Data.Entity;
using Microsoft.Owin;
using Owin;
using _42.Blog.Migrations;
using _42.Blog.Models;

[assembly: OwinStartupAttribute(typeof(_42.Blog.Startup))]
namespace _42.Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<BlogDbContext,Configuration>());

            ConfigureAuth(app);
        }
    }
}
