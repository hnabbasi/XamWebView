using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamWebView;
using XamWebView.Droid.Renderers;

[assembly: ExportRenderer(typeof(AutoResizeWebView), typeof(MyWebViewRenderer))]
namespace XamWebView.Droid.Renderers
{
    public class MyWebViewRenderer : WebViewRenderer
    {
        private readonly Context _context;

        private AutoHeightWebView _webView;

        public MyWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement is null || Control is null)
                return;

            _webView = new AutoHeightWebView(_context);            
            _webView.SetWebViewClient(Control.WebViewClient);

            SetNativeControl(_webView);

            _webView.SizeChanged += OnWebContentSizeChanged;

            if (e.OldElement != null)
                _webView.SizeChanged -= OnWebContentSizeChanged;
        }

        private void OnWebContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Element.WidthRequest = e.Width;
            Element.HeightRequest = e.Height;
        }

        /// <summary>
        /// Custom WebView to handle size changes
        /// </summary>
        public class AutoHeightWebView : Android.Webkit.WebView
        {
            public EventHandler<SizeChangedEventArgs> SizeChanged;

            public AutoHeightWebView(Context context) : base(context) { }

            protected AutoHeightWebView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }

            int _previousMesuredHeight = 0;

            protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
            {
                base.OnSizeChanged(w, h, oldw, oldh);
                OnSizeChange();
            }

            void OnSizeChange()
            {
                var newHeight = ContentHeight;
                if (newHeight > 0 && _previousMesuredHeight != newHeight)
                {
                    SizeChanged?.Invoke(this, new SizeChangedEventArgs(Width, newHeight, Width, _previousMesuredHeight));
                    _previousMesuredHeight = newHeight;
                }
            }
        }

        /// <summary>
        /// Size changed event args
        /// </summary>
        public class SizeChangedEventArgs : EventArgs
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int OldWidth { get; set; }
            public int OldHeight { get; set; }

            public SizeChangedEventArgs(int w, int h, int oldw, int oldh)
            {
                Width = w;
                Height = h;
                OldWidth = oldw;
                OldHeight = oldh;
            }
        }
    }    
}