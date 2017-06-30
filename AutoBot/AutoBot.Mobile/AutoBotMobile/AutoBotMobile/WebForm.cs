using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace AutoBotMobile
{
    public class WebForm : ContentPage
    {
        public WebForm()
        {
            var browser = new WebView();

            browser.Source = "https://webchat.botframework.com/embed/AutoBotNS?s=qGsYvx8oViA.cwA.olc.QBXiri0Arhgw2ZOPbDx603Mpoet8Yxjm-qcZzpKdGSg";

            this.Content = browser;
        }
    }
}
