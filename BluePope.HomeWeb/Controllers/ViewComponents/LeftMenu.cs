﻿using BluePope.HomeWeb.Hubs.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BluePope.HomeWeb.Controllers.ViewComponents
{
    public class LeftMenu : ViewComponent
    {
        static IHubContext<ChatHub> ChatHub;

        public LeftMenu(IHubContext<ChatHub> chatHub)
        {
            ChatHub = chatHub;
        }
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var controller = ViewContext.RouteData.Values["Controller"] as string;
            var action = ViewContext.RouteData.Values["Action"] as string;
            
            return View("Default");
        }
    }
}
