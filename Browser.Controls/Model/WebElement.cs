using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Browser.Controls.Interfaces;
using Browser.Controls.Model.Exceptions;
using Browser.Controls.Resources.Helpers;
using CefSharp;
using CefSharp.Wpf;

namespace Browser.Controls.Model
{
    internal class WebElement : IWebElement
    {
        private readonly IWpfWebBrowser _browser;
        private string _xpath;

        public WebElement(IWpfWebBrowser browser, string xpath)
        {
            _browser = browser;
            _xpath = xpath;
        }

        public bool IsNull { get; private set; } = true;

       

        public async Task FindAsync()
        {
            await IncludeJQueryToPageAsync().ConfigureAwait(false);

            var script = JavaScriptSnippets.GetFindScript(_xpath) + ".length;";
            await ExecuteScriptAsync(script, Continuation).ConfigureAwait(false);

            void Continuation(Task<JavascriptResponse> t)
            {
                if (t.IsFaulted) return;
                var response = t.Result;
                if (response.Success && int.TryParse(response.Result?.ToString(), out int count) && count > 0)
                {
                    IsNull = false;
                }
                else
                {
                    IsNull = true;
                }
            }
        }

        private Task IncludeJQueryToPageAsync()
        {
            var script = JavaScriptSnippets.GetJQuery();
            return ExecuteScriptAsync(script);
        }

        private async Task ExecuteScriptAsync(string script, Action<Task<JavascriptResponse>> continuation = null)
        {
            Task<JavascriptResponse> task;
            try
            {
                task = _browser.GetMainFrame().EvaluateScriptAsync(script, null, 1, TimeSpan.FromSeconds(60));
                await task.ConfigureAwait(false);

                if (continuation != null)
                {
                    var continueTask = task.ContinueWith(continuation);
                    await continueTask.ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
                throw new JavaScriptException("Не удалось выполнить скрипт за отведенное время.", script);
            }
           
        }

        public Task ClickAsync()
        {
            
            var script = "var obj = " + JavaScriptSnippets.GetFindScript(_xpath) + "; obj.click()";
            return ExecuteScriptAsync(script);
        }


        public Task SetAttributeAsync(string attrName, string attrValue)
        {
            if (attrName.ToLower() == "value")
            {
                return SetValueAsync(attrValue);
            }

            var script = $"var obj = {JavaScriptSnippets.GetFindScript(_xpath)}[0];" +
                         $"obj.setAttribute('{attrName}','{attrValue}');";

            return ExecuteScriptAsync(script);
        }

        public async Task<string> GetAttributeAsync(string attrName)
        {
            var script = $"var obj = {JavaScriptSnippets.GetFindScript(_xpath)}[0];" +
                         $"obj.getAttribute('{attrName}');";

            string result = null;

            await ExecuteScriptAsync(script, Continuation).ConfigureAwait(false);

            return result;

            void Continuation(Task<JavascriptResponse> t)
            {
                if (t.IsFaulted) return;

                var response = t.Result;
                result = response.Result?.ToString();
            }
        }

        public Task SetValueAsync(string value)
        {
            var script = $"var obj = {JavaScriptSnippets.GetFindScript(_xpath)}[0];" +
                         $"obj.value = '{value}';";

            return ExecuteScriptAsync(script);
        }

        public async Task<string> GetValueAsync()
        {
            var script = $"{JavaScriptSnippets.GetFindScript(_xpath)}[0].value;";
            string result = null;

            await ExecuteScriptAsync(script, Continuation).ConfigureAwait(false);

            return result;

            void Continuation(Task<JavascriptResponse> t)
            {
                if (t.IsFaulted) return;

                var response = t.Result;
                result = response.Result?.ToString();
            }
        }

        public Task RiseEventAsync(string eventName)
        {
            var script = $"var obj = {JavaScriptSnippets.GetFindScript(_xpath)}[0]; $(obj).trigger('{eventName}');";
            return ExecuteScriptAsync(script);
        }

        public Task ScrollIntoViewAsync()
        {
            var script = $"{JavaScriptSnippets.GetFindScript(_xpath)}[0].scrollIntoView();";
            return ExecuteScriptAsync(script);
        }

        public async Task<string> DrawToBitmapAsync()
        {
            var html2Canvas = JavaScriptSnippets.GetHtml2Canvas();
            await ExecuteScriptAsync(html2Canvas).ConfigureAwait(false);

            var html2CanvasPlugin = JavaScriptSnippets.GetHtml2CanvasPluginForJQuery();
            await ExecuteScriptAsync(html2CanvasPlugin).ConfigureAwait(false);

            var script = JavaScriptSnippets.GetHtmlElementCapture(_xpath);

            string base64 = null;
            await ExecuteScriptAsync(script).ConfigureAwait(false);

            await Task.Delay(1000).ConfigureAwait(false); //Wait canvas rendering

            string getBase64Script = "document.htmlElementBase64;";
            await ExecuteScriptAsync(getBase64Script, Continuation).ConfigureAwait(false);

            return base64;

            void Continuation(Task<JavascriptResponse> task)
            {
                if (!task.IsFaulted)
                {
                    var response = task.Result;
                    base64 = response.Result?.ToString();
                }
            }
        }

        public async Task<List<IWebElement>> FindChildrenByXPathAsync(string xpath)
        {
            var parentXpath = _xpath;
            var childXpath = parentXpath + xpath;

            await IncludeJQueryToPageAsync().ConfigureAwait(false);

            var script = JavaScriptSnippets.GetFindScript(childXpath) + ".length";

            int childrenCount = 0;
            await ExecuteScriptAsync(script, GetChildrenCountCallback).ConfigureAwait(false);

            var children = new List<IWebElement>();
            var tasks = new List<Task>();
            for (int i = 1; i <= childrenCount; i++)
            {
                var child = new WebElement(_browser, $"({childXpath})[{i}]");
                tasks.Add(child.FindAsync());
                children.Add(child);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return children;

            void GetChildrenCountCallback(Task<JavascriptResponse> task)
            {
                if (!task.IsFaulted)
                {
                    var response = task.Result;
                    int.TryParse(response.Result?.ToString(), out int count);
                    childrenCount = count;
                }
            }

        }
    }
}