using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DingTalkApi.Startup))]
namespace DingTalkApi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
