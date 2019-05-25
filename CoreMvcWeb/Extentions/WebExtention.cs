﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace CoreMvcWeb.Extentions
{
    public static class WebExtention
    {
        public static int IsStringNotNullCount(this HtmlHelper htmlHelper, params string[] strList)
        {
            int r = 0;

            foreach (var s in strList)
            {
                if (string.IsNullOrWhiteSpace(s) == false)
                    r++;
            }

            return r;
        }

        public static bool IsStringNotNull(this HtmlHelper htmlHelper, params string[] strList)
        {
            foreach (var s in strList)
            {
                if (string.IsNullOrWhiteSpace(s) == false)
                    return true;
            }

            return false;
        }

        public static bool IsInRoleOrAdmin(this IPrincipal principal, params string[] roles)
        {
            if (principal.IsInRole("ADMIN"))
                return true;

            foreach(string role in roles)
            {
                if (principal.IsInRole(role))
                    return true;
            }

            return false;
        }

        public static HtmlString DrawSelectValue(this HtmlHelper htmlHelper, string value, string text, string selectedValue)
        {
            string selected = string.Empty;

            if (value == selectedValue)
            {
                selected = "selected=\"selected\"";
            }

            value = HttpUtility.HtmlEncode(value);
            text = HttpUtility.HtmlEncode(text);

            return new HtmlString(string.Format("<option value=\"{0}\" {1}>{2}</option>", value, selected, text));
        }

        public static bool IsLocal(this ConnectionInfo connection)
        {
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }
    }
}