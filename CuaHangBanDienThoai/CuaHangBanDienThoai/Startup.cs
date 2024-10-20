
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
[assembly: OwinStartup(typeof(CuaHangBanDienThoai.Startup))]
namespace CuaHangBanDienThoai
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           

            app.MapSignalR();

          
        }
    }
}