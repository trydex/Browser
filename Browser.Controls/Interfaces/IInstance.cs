using System.Collections.Generic;
using System.Collections.ObjectModel;
using ProxyInfo = Browser.Controls.Model.ProxyInfo;

namespace Browser.Controls.Interfaces
{
    public interface IInstance
    {
        ITab ActiveTab { get; }
        ObservableCollection<ITab> Tabs { get; }
        ITab NewTab(string name = "New Tab");
        void SetProxy(ProxyInfo proxy);
        void ClearProxy();
        void SetFileUploadPolicy(bool cancelFileDialog);
        void SetFilesForUpload(List<string> filePaths);
        void SetUserAgent(string userAgentString);
    }   
}
