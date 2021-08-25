using System;
using System.Threading.Tasks;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamWebView;
using XamWebView.iOS.Renderers;

[assembly: ExportRenderer(typeof(AutoResizeWebView), typeof(MyWebViewRenderer))]
namespace XamWebView.iOS.Renderers
{
    public class MyWebViewRenderer : WkWebViewRenderer
    {
        private WKNavigationDelegate TempNavDelegate { get; set; }

        private WKNavigationDelegate FormsNavDelegate { get; set; }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.NewElement == null)
                    return;

                FormsNavDelegate = (WKNavigationDelegate)NavigationDelegate;
                TempNavDelegate = new MyWKWebViewNavigationDelegate(this);

                NavigationDelegate = TempNavDelegate;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error at WebViewRenderer OnElementChanged: " + ex.Message);
            }
        }

        class MyWKWebViewNavigationDelegate : WKNavigationDelegate
        {
            readonly MyWebViewRenderer _webViewRenderer;

            public MyWKWebViewNavigationDelegate(MyWebViewRenderer webViewRenderer)
            {
                _webViewRenderer = webViewRenderer ?? new MyWebViewRenderer();
            }

            public override async void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
            {
                try
                {
                    // wait here till content is rendered
                    await Task.Delay(1000);

                    if (_webViewRenderer.Element is WebView formsWebView && webView != null)
                    {
                        if (webView.ScrollView != null && webView.ScrollView.ContentSize != null)
                        {
                            formsWebView.HeightRequest = (double)webView.ScrollView.ContentSize.Height;
                        }
                    }
                    _webViewRenderer.NavigationDelegate = _webViewRenderer.FormsNavDelegate;
                    _webViewRenderer.TempNavDelegate.Dispose();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine($">>> Note: Don't care if the page is navigated away already. Error: {ex}");
#endif
                }
            }
        }
    }
}