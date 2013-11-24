using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fix;
using Simple.Web;

namespace NestDemo
{
    public class AppBuilder
    {
        public static Type[] EnforceReferencesFor =
                {
                    typeof (Simple.Web.Razor.RazorHtmlMediaTypeHandler),
                    typeof (Simple.Web.JsonNet.JsonMediaTypeHandler)
                };

        public Func<IDictionary<string, object>, Task> BuildApp()
        {
            var staticBuilder = Simple.Owin.Static.Statics.AddFolder("/assets").AddFolder("/app");

            var app = new Fixer()
                .Use(staticBuilder.Build())
                .Use(Application.Run)
                .Build();

            return app;
        }
    }
}