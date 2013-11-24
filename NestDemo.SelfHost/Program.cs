using System;
using Nowin;

namespace NestDemo.SelfHost
{
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
            var app = new AppBuilder().BuildApp();

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
