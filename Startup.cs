using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lojinha.Startup))]
namespace Lojinha
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
