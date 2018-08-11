using System.Collections.Generic;
using CefSharp;

namespace Browser.Controls.Handlers
{
    public class DialogHandler : IDialogHandler
    {
        private readonly bool _cancelDialog;
        private readonly List<string> _filePaths;

        public DialogHandler(bool cancelDialog, List<string> filePaths)
        {
            _cancelDialog = cancelDialog;
            _filePaths = filePaths;
        }

        public bool OnFileDialog(IWebBrowser browserControl, IBrowser browser, CefFileDialogMode mode, string title,
            string defaultFilePath, List<string> acceptFilters, int selectedAcceptFilter, IFileDialogCallback callback)
        {
            if (_cancelDialog)
            {
                callback.Cancel();
            }
            else
            {
                callback.Continue(selectedAcceptFilter, _filePaths);
            }
            
            return true;
        }
    }
}