namespace Browser.Interfaces
{
    public interface ITab
    {
        string Address { get; }
        bool IsBusy { get; }

        void SetActive();
        void Navigate(string url, string referrer = "");
        void Close();
        void WaitDownloading();
        
        //IHtmlElement FindElementByXPath(string xpath, int number = 0);
        //List<IHtmlElement> FindElementsByXPath(string xpath);
    }
}