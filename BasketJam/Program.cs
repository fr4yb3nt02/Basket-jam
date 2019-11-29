/*using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;*/
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BasketJam
{
    public class Program
    {




       

    public static void Main(string[] args)
           {
               CreateWebHostBuilder(args).Build().Run();
           }

           public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
               WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>();




        /*
               public static void Main(string[] args)
                     {
                         BuildWebHost(args).Run();
                     }

                     public static IWebHost BuildWebHost(string[] args) =>
                         WebHost.CreateDefaultBuilder(args)
                             .UseStartup<Startup>()
                             .UseUrls("http://localhost:5001")
                             .Build();

            */
    }
}
