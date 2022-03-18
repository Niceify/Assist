﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using Assist.MVVM.View.InitPage;
using Assist.MVVM.View.Extra;
using Assist.MVVM.ViewModel;
using Assist.Settings;
using System.Text.Json;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Assist.Modules.XMPP;
using Assist.MVVM.Model;
using ValNet;
using ValNet.Objects.Authentication;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Assist
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            AssistLog.Normal("Program Started");
            //Startup Code here.
            base.OnStartup(e);
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assist"));


            try
            {
                AssistSettings.Current = JsonSerializer.Deserialize<AssistSettings>(File.ReadAllText(AssistSettings.SettingsFilePath));
            }
            catch
            {
                AssistLog.Error("Settings File was not found or tampered with.");
                AssistSettings.Current = new AssistSettings();
            }

            ChangeLanguage();


            AssistLog.Normal("Starting InitPage");

            AssistApplication.AppInstance.AssistApiController.CheckForAssistUpdates();

            Current.MainWindow = new InitPage();

            Screen targetScreen = Screen.PrimaryScreen;

            Rectangle viewport = targetScreen.WorkingArea;
            Current.MainWindow.Top = (viewport.Height - Current.MainWindow.Height) / 2
                                     + viewport.Top;
            Current.MainWindow.Left = (viewport.Width - Current.MainWindow.Width) / 2
                                      + viewport.Left; ;
            Current.MainWindow.Show();
        }



        private void AppExit(object sender, ExitEventArgs e)
        {
            AssistSettings.Save();
            Environment.Exit(0);
        }
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            AssistLog.Error("Unhandled Ex Source: " + e.Exception.Source);
            AssistLog.Error("Unhandled Ex StackTrace: " + e.Exception.StackTrace);
            AssistLog.Error("Unhandled Ex Message: " + e.Exception.Message);
            MessageBox.Show(e.Exception.Message, "Assist Hit an Error", MessageBoxButton.OK, MessageBoxImage.Warning);

        }
        public static async Task<BitmapImage> LoadImageUrl(string url)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();
            

            return image;
        }
        public static async Task<BitmapImage> LoadImageUrl(string url, int imageWidth, int imageHeight)
        {
            // Allows the image to be loaded with the resolution it is intended to be used for.
            // Because the program is a solo resolution that doesnt change res, this is fine.

            var image = new BitmapImage();
            image.BeginInit();
            image.DecodePixelHeight = (int)(imageHeight * AssistApplication.GlobalScaleRate);
            image.DecodePixelWidth = (int)(imageWidth * AssistApplication.GlobalScaleRate);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();
            

            return image;
        }

        public static void ChangeLanguage()
        {
            var curr = AssistSettings.Current.Language;

            switch (curr)
            {
                case Enums.ELanguage.en_us:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", true);
                    break;
                case Enums.ELanguage.ja_jp:
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP", true);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja-JP", true);
                    break;
            }
        }
    }
}
