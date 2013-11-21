using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NestDemo
{
    using Fix;
    using Nowin;
    using Simple.Web;

    class Program
    {
        const int Port = 1337;

        public static Type[] EnforceReferencesFor =
        {
            typeof (Simple.Web.Razor.RazorHtmlMediaTypeHandler),
            typeof (Simple.Web.JsonNet.JsonMediaTypeHandler)
        };

        static void Main(string[] args)
        {
            var app = new Fixer()
                .Use(Application.Run)
                .Build();

            var serverBuilder = ServerBuilder.New()
                .SetPort(Port)
                .SetOwinApp(app);

            using (serverBuilder.Start())
            {
                Console.WriteLine("Listening on port {0}, press enter to exit", Port);
                Console.ReadLine();
            }
        }
    }
}
