using System.IO;
using System.Reflection;

namespace Browser.Controls.Resources.Helpers
{   
    public static class JavaScriptSnippets
    {
        private static string GetResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public static string GetJQuery()
        {
            var resourceName = "Browser.Controls.Resources.JavaScript.jquery-3.2.1.min.js";
            return GetResource(resourceName);
        }

        public static string GetHtml2Canvas()
        {
            var resourceName = "Browser.Controls.Resources.JavaScript.html2canvas.min.js";
            return GetResource(resourceName);
        }
            
        public static string GetHtml2CanvasPluginForJQuery()
        {
            var resourceName = "Browser.Controls.Resources.JavaScript.jquery.plugin.html2canvas.js";
            return GetResource(resourceName);
        }

        public static string GetFindScript(string xpath) => $"$(document).xpathEvaluate(\"{xpath}\")";

        public static string GetHtmlElementCapture(string xpath) => "(function(){" +
                                                                    "document.htmlElementBase64 = null; " +
                                                                    "var obj = " + GetFindScript(xpath) + "[0]; " +
                                                                    "capture(obj); " +
                                                                    "function capture(e, c) {$(e).html2canvas({useCORS: true, taintTest: false, onrendered: function (canvas) {var data = canvas.toDataURL(\"image/png\"); document.htmlElementBase64 = data.slice(22);}})}})();";
    }
}
