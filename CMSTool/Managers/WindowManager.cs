using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;

namespace FGCMSTool.Managers
{
    public class WindowManager
    {
        static WindowManager? _instance;

        public static WindowManager Instance
        {
            get => _instance ??= new WindowManager();
        }

        List<Window>? ActiveWindows;

        public void Setup()
        {
            Application.Current.PlatformSettings.ColorValuesChanged += (_, _) =>
            {
                ProcessTheme(Application.Current.PlatformSettings.GetColorValues().ThemeVariant);
            };
        }

        void ProcessTheme(PlatformThemeVariant theme)
        {
            for (int i = 0; i < ActiveWindows.Count; i++)
            {
                Window? window = ActiveWindows[i];
                switch (theme)
                {
                    case PlatformThemeVariant.Light:
                        window.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255), 60);
                        break;
                    case PlatformThemeVariant.Dark:
                        window.Background = new SolidColorBrush(Color.FromRgb(0, 0, 0), 120);
                        break;
                }
            }
        }

        public void SetupWindow(Window window)
        {
            ActiveWindows ??= [];
            ActiveWindows.Add(window);
            ProcessTheme(Application.Current.PlatformSettings.GetColorValues().ThemeVariant);
            window.Closed += (_, _) =>
            {
                RemoveWindow(window);
            };
        }

        public void RemoveWindow(Window window) => ActiveWindows?.Remove(window);
    }
}
