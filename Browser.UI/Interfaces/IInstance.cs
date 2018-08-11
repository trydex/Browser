using System.Collections.Generic;
using System.Collections.ObjectModel;
using Browser.Model;

namespace Browser.Interfaces
{
    public interface IInstance
    {
        ITab ActiveTab { get; }
        ObservableCollection<ITab> Tabs { get; }
        ITab NewTab(string name);
            
        void SetProxy(ProxyInfo proxy);
        void ClearProxy();
        void SendText(string text, int latency);
        void SetFileUploadPolicy(bool cancelFileDialog);
        void SetFilesForUpload(List<string> filePaths);
    }   
}
