using System.Collections.Generic;

namespace Browser.Interfaces
{
    public interface IHtmlElement
    {
        bool IsNull { get; }
        bool IsHidden { get; }
        bool IsNullOrHidden { get; }
        string Value { get; }
        
        string GetAttribute(string attrName);
        void SetAttribute(string attrName, string attrValue);
        string GetValue();
        void SetValue(string value);
        void RiseEvent(string eventName);
        void Click();
        void ScrollIntoView();
        string DrawToBitmap(bool isImage);
        List<IHtmlElement> FindChildrenByXPath(string xpath);
    }
}