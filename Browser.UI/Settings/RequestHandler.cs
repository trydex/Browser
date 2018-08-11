using System.IO;
using System.Security.Cryptography.X509Certificates;
using Browser.Model;
using CefSharp;

public class ResponseFilter : IResponseFilter
{
    public void Dispose()
    {
    }

    public bool InitFilter()
    {
        return false;
    }

    public FilterStatus Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
    {
        dataInRead = 0;
        dataOutWritten = 0;
        return FilterStatus.Done;
    }
}

public class RequestHandler : IRequestHandler
{
    private readonly ProxyInfo _instanceProxy;

    public RequestHandler(ProxyInfo instanceProxy)
    {
        _instanceProxy = instanceProxy;
    }

    public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
        bool isRedirect)
    {
        return false;
    }

    public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
        WindowOpenDisposition targetDisposition, bool userGesture)
    {
        return false;
    }

    public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode,
        string requestUrl,
        ISslInfo sslInfo, IRequestCallback callback)
    {
        return false;
    }

    public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
    {
    }

    public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame,
        IRequest request,
        IRequestCallback callback)
    {
        return CefReturnValue.Continue;
    }

    public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy,
        string host, int port,
        string realm, string scheme, IAuthCallback callback)
    {
        if (isProxy)
        {
            callback.Continue(_instanceProxy.Username, _instanceProxy.Password);
            return true;
        }

        return false;
    }

    public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host,
        int port,
        X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
    {
        return false;
    }

    public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
    {
    }

    public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize,
        IRequestCallback callback)
    {
        return false;
    }

    public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
        IResponse response, ref string newUrl)
    {
    }

    public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
    {
        return false;
    }

    public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
    {
    }

    public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
        IResponse response)
    {
        return false;
    }

    public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame,
        IRequest request,
        IResponse response)
    {
        return new ResponseFilter();
    }

    public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
        IResponse response, UrlRequestStatus status, long receivedContentLength)
    {
    }
}