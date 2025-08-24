using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Handlers;
using CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET.MAUI.Wizard.Extensions
{
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Initializes Maui Wizard
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static MauiAppBuilder UseMauiWizard(this MauiAppBuilder builder)
        {
            builder.UseMauiCommunityToolkit();
            return builder;
        }
    }
}
