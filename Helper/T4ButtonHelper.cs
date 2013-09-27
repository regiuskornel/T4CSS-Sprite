using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApp
{
    public static class T4ButtonHelper
    {
        [Flags]
        public enum ButtonMode
        {
            /// <summary>
            /// Normal button
            /// </summary>
            Default = 0,
            /// <summary>
            /// Disabled button, not clickable and UI theme
            /// </summary>
            Disabled = 1,
            /// <summary>
            /// Show title always
            /// </summary>
            ShowTitle = 16,
            /// <summary>
            /// Show just icon and title text (normal text won't render)
            /// </summary>
            IconOnly = 128,
        }

        public const string IconDefaultClass = "t4SpriteVar";
        public const string IconDisabledClass = "disabled";

        public static MvcHtmlString UIActionLink(this HtmlHelper htmlHelper, string icon, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, ButtonMode status)
        {
            return UIActionLink(htmlHelper, icon, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), status);
        }

        public static MvcHtmlString UIActionLink(this HtmlHelper htmlHelper, string icon, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, ButtonMode status, string iconvariantclass=null)
        {
            return UIActionLink(htmlHelper, icon, linkText, actionName, controllerName, new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), status,iconvariantclass);
        }

        public static MvcHtmlString UIActionLink(this HtmlHelper htmlHelper, string icon, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, ButtonMode status, string iconvariantclass=null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, icon, status, linkText, (string)null, actionName, controllerName, null /* protocol */, null /* hostName */, null /* fragment */, routeValues, htmlAttributes, iconvariantclass));
        }

        private static string GenerateLinkInternal(RequestContext requestContext, RouteCollection routeCollection, string icon, ButtonMode status, string linkText, string routeName, string actionName, string controllerName, string protocol, string hostName, string fragment, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, string iconvariantclass)
        {
            string targetUrl = UrlHelper.GenerateUrl(routeName, actionName, controllerName, protocol, hostName, fragment, routeValues, routeCollection, requestContext, false);

            if ((status & ButtonMode.IconOnly) != 0 && string.IsNullOrEmpty(icon))
                throw new Exception("Only icon button without icon, how?");
            string caption = HttpUtility.HtmlEncode(linkText);
            bool disabled = (status & ButtonMode.Disabled) != 0;

            TagBuilder innerTag = null;
            if ((status & ButtonMode.IconOnly) == 0)
            {
                innerTag = new TagBuilder("span") { InnerHtml = caption };
                innerTag.AddCssClass("t4button-text");
            }
            TagBuilder iconTag = null;
            if (!string.IsNullOrEmpty(icon))
            {
                iconTag = new TagBuilder("span");
                iconTag.AddCssClass("t4icon " + IconDefaultClass + " " + icon);
                if (disabled)
                    iconTag.AddCssClass(IconDisabledClass);
                if (!string.IsNullOrEmpty(iconvariantclass))
                    iconTag.AddCssClass(iconvariantclass);
            }

            TagBuilder tag = new TagBuilder(disabled ? "div" : "a");

            if (iconTag == null)
                tag.InnerHtml = innerTag.ToString(TagRenderMode.Normal);
            else if (innerTag == null)
                tag.InnerHtml = iconTag.ToString(TagRenderMode.Normal);
            else
                tag.InnerHtml = iconTag.ToString(TagRenderMode.Normal) + innerTag.ToString(TagRenderMode.Normal);

            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("role", "button");

            if (disabled)
            {
                tag.Attributes.Add("aria-disabled", "true");
                tag.AddCssClass(IconDisabledClass);
            }
            else
            {
                tag.MergeAttribute("href", targetUrl);
                tag.Attributes.Add("aria-disabled", "false");
            }

            if ((status & ButtonMode.IconOnly) != 0)
            {
                tag.AddCssClass("t4button-icon-only");
                tag.Attributes.Add("title", caption);
            }
            else
            {
                tag.AddCssClass(iconTag == null ? "t4button-text-only" : "t4button-text-icon");
                if ((status & ButtonMode.ShowTitle) != 0)
                    tag.Attributes.Add("title", caption);
            }

            tag.AddCssClass("t4button");
            return tag.ToString(TagRenderMode.Normal);
        }
    }
}