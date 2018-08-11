using System.Collections.Generic;
using System.Threading.Tasks;

namespace Browser.Controls.Interfaces
{
    public interface IWebElement
    {
        bool IsNull { get; }

        Task ClickAsync();
        Task SetAttributeAsync(string attrName, string attrValue);
        Task<string> GetAttributeAsync(string attrName);
        Task SetValueAsync(string value);
        Task<string> GetValueAsync();

        Task RiseEventAsync(string eventName);
        Task ScrollIntoViewAsync();
        Task<string> DrawToBitmapAsync();
        Task<List<IWebElement>> FindChildrenByXPathAsync(string xpath);
    }   
}