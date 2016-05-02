using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EshopMVC.Startup))]
namespace EshopMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
