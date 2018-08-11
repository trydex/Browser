using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Browser.Controls.Handlers;
using CefSharp;
using CefSharp.Wpf;
using DialogHandler = Browser.Controls.Handlers.DialogHandler;
using LifeSpanHandler = Browser.Controls.Handlers.LifeSpanHandler;
using NewWindowOpeningEventArgs = Browser.Controls.Model.EventArgs.NewWindowOpeningEventArgs;

namespace Browser.Controls.View
{
    public class TabView : ChromiumWebBrowser
    {
       public string CachePath
        {
            get => (string)GetValue(CachePathProperty);
            set => SetValue(CachePathProperty, value);
        }

        public static readonly DependencyProperty CachePathProperty =
            DependencyProperty.Register("CachePath", typeof(string), typeof(TabView));


        public Action<NewWindowOpeningEventArgs> NewWindowOpeningHandler
        {
            get => (Action<NewWindowOpeningEventArgs>)GetValue(NewWindowOpeningHandlerProperty);
            set => SetValue(NewWindowOpeningHandlerProperty, value);
        }

        public static readonly DependencyProperty NewWindowOpeningHandlerProperty =
            DependencyProperty.Register("NewWindowOpeningHandler", typeof(Action<NewWindowOpeningEventArgs>), typeof(TabView));


        public bool IsNeedFileDialogCancel
        {
            get => (bool)GetValue(IsNeedFileDialogCancelProperty);
            set => SetValue(IsNeedFileDialogCancelProperty, value);
        }

        public static readonly DependencyProperty IsNeedFileDialogCancelProperty =
            DependencyProperty.Register("IsNeedFileDialogCancel", typeof(bool), typeof(TabView), new PropertyMetadata(false, DialogHandlerOptionsChangedCallback));

        public List<string> UploadFilePaths
        {
            get => (List<string>)GetValue(UploadFilePathsProperty);
            set => SetValue(UploadFilePathsProperty, value);
        }
    
        public static readonly DependencyProperty UploadFilePathsProperty =
            DependencyProperty.Register("UploadFilePaths", typeof(List<string>), typeof(TabView), new PropertyMetadata(new List<string>(), DialogHandlerOptionsChangedCallback));

        private static void DialogHandlerOptionsChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var tabView = sender as TabView;
            if (tabView is null)
                return;

            tabView.DialogHandler = new DialogHandler(tabView.IsNeedFileDialogCancel, tabView.UploadFilePaths);
        }


        public TabView()
        {
            Initialized += OnInitialized;
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            SetHandlers();
        }

        private void SetHandlers()
        {
            //Для отключения контесктного меню
            //MenuHandler = new ContextMenuHandler(); //TODO: Убрать комментарий

            //Для открытия ссылкок _blank в новом окне
            var lifeSpanHandler = new LifeSpanHandler();
            lifeSpanHandler.NewWindowOpening += NewWindowOpeningH;
            LifeSpanHandler = lifeSpanHandler;

            //Для раздельного кэша
            var requestContextSettings = new RequestContextSettings
            {
                CachePath = CachePath,
                PersistSessionCookies = false,
                PersistUserPreferences = false
            };
            
            var requestContext = new RequestContext(requestContextSettings, new RequestContextHandler());
            RequestContext = requestContext;



        }

        private void NewWindowOpeningH(object sender, NewWindowOpeningEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => NewWindowOpeningHandler(e)));
        }
    }
}