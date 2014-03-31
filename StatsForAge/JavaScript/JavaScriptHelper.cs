using System;
using System.Web;
using System.Web.UI;

[assembly: WebResource("StatsForAge.JavaScript.CentileData.js","application/x-javascript")]
[assembly: WebResource("StatsForAge.JavaScript.UkWeightData.js","application/x-javascript")]

namespace StatsForAge
{
    /// <summary>
    /// Helps include embedded JavaScript files in pages.
    /// </summary>
    public class JavaScript
    {

        #region Constants

        private const string TEMPLATE_SCRIPT = "<script type=\"text/javascript\" src=\"{0}\"></script>\r\n";
        private const string CentileData = "StatsForAge.JavaScript.CentileData.js";
        private const string UkWeightData = "StatsForAge.JavaScript.UkWeightData.js";

        #endregion

        #region Public Methods

        /// <summary>
        /// Includes ShowMessage.js in the page.
        /// </summary>
        /// <param name="manager">Accessible via Page.ClientScript.</param>
        /// <param name="late">Include the JavaScript at the bottom of the HTML?</param>
        public static void Include_CentileData
		(ClientScriptManager manager, bool late = false)
        {
            IncludeJavaScript(manager, CentileData, late);
        }

        /// <summary>
        /// Includes GreetUser.js (and all dependencies) in the page.
        /// </summary>
        /// <param name="manager">Accessible via Page.ClientScript.</param>
        /// <param name="late">Include the JavaScript at the bottom of the HTML?</param>
        public static void Include_UkWeightData
		(ClientScriptManager manager, bool late = false)
        {
            // Dependency (ShowMessage.js).
            Include_CentileData(manager, late);

            // Include GreetUser.js.
            IncludeJavaScript(manager, UkWeightData, late);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Includes the specified embedded JavaScript file in the page.
        /// </summary>
        /// <param name="manager">Accessible via Page.ClientScript.</param>
        /// <param name="resourceName">The name used to identify the 
        /// embedded JavaScript file.
        /// </param>
        /// <param name="late">Include the JavaScript at the bottom of the HTML?</param>
        private static void IncludeJavaScript(ClientScriptManager manager,
			string resourceName, bool late)
        {
            var type = typeof(StatsForAge.JavaScript);
            if (!manager.IsStartupScriptRegistered(type, resourceName))
            {
                if (late)
                {
                    var url = manager.GetWebResourceUrl(type, resourceName);
                    var scriptBlock = string.Format(TEMPLATE_SCRIPT, HttpUtility.HtmlEncode(url));
                    manager.RegisterStartupScript(type, resourceName, scriptBlock);
                }
                else
                {
                    manager.RegisterClientScriptResource(type, resourceName);
                    manager.RegisterStartupScript(type, resourceName, string.Empty);
                }
            }
        }

        #endregion
    }
}
