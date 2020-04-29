using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TaskManager.Startup))]

namespace TaskManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}