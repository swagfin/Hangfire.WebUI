using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hangfire.WebUI.Startup))]
namespace Hangfire.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
