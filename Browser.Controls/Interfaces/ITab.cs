using System.Collections.Generic;
using System.Threading.Tasks;
using Browser.Controls.Model;

namespace Browser.Controls.Interfaces
{
    public interface ITab
    {
        string Address { get; }
        bool IsBusy { get; }

        void SetActive();
        void Navigate(string url, string referrer = "");
        void Close();
        void WaitDownloading();
        void SendText(string text, Range latency);

        Task<IWebElement> FindElementByXPathAsync(string xpath);
        Task<List<IWebElement>> FindElementsByXPathAsync(string xpath);
    }
}   