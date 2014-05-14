using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bauhaus.Helpers
{
    public static class ViewsHelper
    {
        
        /// <summary>
        /// Returns progressbar tag with selected state and Date if provided
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="state">1 = Success, 2 = Warning, 3 = Danger, Default = Info</param>
        /// <param name="date">Date to appear inside Bar</param>
        /// <returns>Set of Div tags that conform a progress bar</returns>
        public static MvcHtmlString ProgressBarState(this HtmlHelper helper, int state,String date=null)
        {
            //Declares Divs
            TagBuilder divPro = new TagBuilder("div");
            TagBuilder divBarFill = new TagBuilder("div");
            TagBuilder spanDate = new TagBuilder("span");

            string type = "info";

            switch (state)
            {
                case 1:
                    type = "success";
                    break;
                case 2:
                    type = "warning";
                    break;
                case 3:
                    type = "danger";
                    break;
                default:
                    type = "info";
                    break;
            }


            //Format Divs
            if (date!=null)
            {
                date.Trim();
                spanDate.InnerHtml = date;
                divBarFill.InnerHtml = spanDate.ToString();
            }
            
            divPro.Attributes.Add("class", "progress progress-striped active");

            divBarFill.Attributes.Add("class", "progress-bar progress-bar-" +type);
            divBarFill.Attributes.Add("style", "width: 100%");

            //Nesting
            if(state != 0)
                divPro.InnerHtml = divBarFill.ToString();

            return new MvcHtmlString(divPro.ToString());

        }
        
    }
}