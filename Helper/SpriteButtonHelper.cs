using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace MvcApp
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
        TextOnly = 64,
        /// <summary>
        /// Show just icon and title text (normal text won't render)
        /// </summary>
        IconOnly = 128,
    }

    public static class SpriteButtonHelper
    {
        public const string IconDefaultClass = "t4SpriteVar";
        public const string IconDisabledClass = "disabled";

        #region HtmlHelper
        public static MvcHtmlString SPActionLink(this HtmlHelper htmlHelper, string icon, string linkText, string actionName, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, icon, status, linkText, null, actionName, null,
                null, null, iconvariantclass, null, null));
        }

        public static MvcHtmlString SPActionLink(this HtmlHelper htmlHelper, string icon, string linkText, string actionName, string controllerName, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, icon, status, linkText, null, actionName, controllerName,
                null, null, iconvariantclass, null, null));
        }

        public static MvcHtmlString SPActionLink(this HtmlHelper htmlHelper, string icon, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, icon, status, linkText, null, actionName, controllerName,
                new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), iconvariantclass, null, null));
        }

        public static MvcHtmlString SPActionLink(this HtmlHelper htmlHelper, string icon, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection, icon, status, linkText, null, actionName, 
                controllerName, routeValues, htmlAttributes, iconvariantclass, null, null));
        }
        #endregion

        #region AjaxHelper

        public static MvcHtmlString SPActionLink(this AjaxHelper ajaxHelper, string icon, string linkText, string actionName, AjaxOptions ajaxOptions, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(ajaxHelper.ViewContext.RequestContext, ajaxHelper.RouteCollection, icon, status, linkText, null, actionName, null,
                null, null, iconvariantclass, ajaxHelper, ajaxOptions));
        }

        public static MvcHtmlString SPActionLink(this AjaxHelper ajaxHelper, string icon, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(ajaxHelper.ViewContext.RequestContext, ajaxHelper.RouteCollection, icon, status, linkText, null, actionName, controllerName, 
                null, null, iconvariantclass, ajaxHelper, ajaxOptions));
        }

        public static MvcHtmlString SPActionLink(this AjaxHelper ajaxHelper, string icon, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, object routeValues, object htmlAttributes, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(ajaxHelper.ViewContext.RequestContext, ajaxHelper.RouteCollection, icon, status, linkText, null, actionName, controllerName, 
                new RouteValueDictionary(routeValues), HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), iconvariantclass, ajaxHelper, ajaxOptions));
        }

        public static MvcHtmlString SPActionLink(this AjaxHelper ajaxHelper, string icon, string linkText, string actionName, string controllerName, AjaxOptions ajaxOptions, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, ButtonMode status = ButtonMode.Default, string iconvariantclass = null)
        {
            return MvcHtmlString.Create(GenerateLinkInternal(ajaxHelper.ViewContext.RequestContext, ajaxHelper.RouteCollection, icon, status, linkText, null, actionName, controllerName,
                routeValues, htmlAttributes, iconvariantclass, ajaxHelper, ajaxOptions));
        }

        #endregion

        #region common
        private static string GenerateLinkInternal(RequestContext requestContext, RouteCollection routeCollection, string icon, ButtonMode status, string linkText, string routeName, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, string iconvariantclass, AjaxHelper ajaxHelper, AjaxOptions ajaxOptions)
        {
            string targetUrl = UrlHelper.GenerateUrl(routeName, actionName, controllerName, null, null, null, routeValues, routeCollection, requestContext, false);

            if ((status & ButtonMode.IconOnly) != 0 && string.IsNullOrEmpty(icon))
                throw new Exception("Only icon button without icon, how?");
            string caption = HttpUtility.HtmlEncode(linkText);
            bool disabled = (status & ButtonMode.Disabled) != 0;

            TagBuilder captionTag = null;
            if ((status & ButtonMode.IconOnly) == 0)
            {
                captionTag = new TagBuilder("span") { InnerHtml = caption };
                captionTag.AddCssClass("t4button-text");
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

            //in disabled state we rendering a div, for normal state a <a>
            TagBuilder buttonTag = new TagBuilder(disabled ? "div" : "a");

            if (iconTag == null)
                buttonTag.InnerHtml = captionTag.ToString(TagRenderMode.Normal);
            else if (captionTag == null)
                buttonTag.InnerHtml = iconTag.ToString(TagRenderMode.Normal);
            else
                buttonTag.InnerHtml = iconTag.ToString(TagRenderMode.Normal) + captionTag.ToString(TagRenderMode.Normal);

            buttonTag.MergeAttributes(htmlAttributes);
            buttonTag.Attributes.Add("role", "button");


            if (disabled)
            {
                buttonTag.Attributes.Add("aria-disabled", "true");
                buttonTag.AddCssClass(IconDisabledClass);
            }
            else
            {
                buttonTag.MergeAttribute("href", targetUrl);
                buttonTag.Attributes.Add("aria-disabled", "false");

                if (ajaxHelper != null && ajaxHelper.ViewContext.UnobtrusiveJavaScriptEnabled)
                    buttonTag.MergeAttributes(ajaxOptions.ToUnobtrusiveHtmlAttributes());
            }

            if ((status & ButtonMode.IconOnly) != 0)
            {
                buttonTag.AddCssClass("t4button-icon-only");
                buttonTag.Attributes.Add("title", caption);
            }
            else
            {
                buttonTag.AddCssClass(iconTag == null ? "t4button-text-only" : "t4button-text-icon");
                if ((status & ButtonMode.ShowTitle) != 0)
                    buttonTag.Attributes.Add("title", caption);
            }

            buttonTag.AddCssClass("t4button");
            return buttonTag.ToString(TagRenderMode.Normal);
        }
        #endregion
    }
}