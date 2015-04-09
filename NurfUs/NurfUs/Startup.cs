using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NurfUs.Startup))]
namespace NurfUs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
