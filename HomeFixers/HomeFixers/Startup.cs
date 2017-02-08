using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HomeFixers.Startup))]
namespace HomeFixers
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
