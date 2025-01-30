using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static FGCMSTool.Managers.LocalizationManager;

namespace FGCMSTool.Views
{
    public partial class DlcWindow : Window
    {
        class DlcItem
        {
            [JsonProperty("path")]
            public string? Path { get; set; }

            [JsonProperty("base")]
            public string? Base { get; set; }
        }

        class DlcImage
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("dlc_item")]
            public DlcItem? DlcItem { get; set; }
        }

        readonly ObservableCollection<string> log = [];
        string? SaveDir;
        public bool isSuceed = false;

        public DlcWindow()
        {
            InitializeComponent();
#if RELEASE_LINUX_X64
            MenuTitleTextBlock.IsVisible = false;
            MenuTitleSeparator.IsVisible = false;
            MainContent.Margin = new Thickness(0, 25, 0, 25);
#endif
        }

        public DlcWindow(JArray dlcImages, string savePath)
        {
            InitializeComponent();
#if RELEASE_LINUX_X64
            MenuTitleTextBlock.IsVisible = false;
            MenuTitleSeparator.IsVisible = false;
            MainContent.Margin = new Thickness(0, 25, 0, 25);
#endif

            LogsControl.ItemsSource = log;

            Loaded += (sender, e) =>
            {
                SaveDir = savePath;

                var images = dlcImages.ToObject<HashSet<DlcImage>>();

                if (images != null)
                    Begin(images);
            };
        }

        async void Begin(HashSet<DlcImage> images)
        {
            int processed = 0;
            int failed = 0;

            if (Directory.Exists(SaveDir))
                Directory.Delete(SaveDir, true);

            Directory.CreateDirectory(SaveDir);

            foreach (var img in images)
            {
                if (img?.DlcItem?.Base == null || img.DlcItem?.Path == null)
                    continue;

                log.Add(LocalizedString("dlc_cms_downloading", [processed + 1, img.Id]));

                try
                {
                    await GetImage($"{img.DlcItem.Base}{img.DlcItem.Path}", img.Id);
                    log[^1] += $" [{LocalizedString("dlc_cms_saved")}]";
                }
                catch
                {
                    log[^1] += $" [{LocalizedString("dlc_cms_failed")}]";
                    failed++;
                }

                processed++;

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    DownloadProgress.Value = (int)((double)processed / images.Count * 100);
                    DownloadProgress.ProgressTextFormat = string.Format(LocalizedString("dlc_cms_progress"), processed, images.Count, failed, DownloadProgress.Percentage);
                    DownloadProgressLog.Offset = new Vector(0, DownloadProgressLog.Extent.Height);
                });
            }

            log.Add(LocalizedString("dlc_cms_done"));
            isSuceed = true;
#if RELEASE_WIN_X64 || DEBUG
            SystemSounds.Exclamation.Play();
#endif
        }

        async Task GetImage(string url, string dlcName)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            byte[] data = await response.Content.ReadAsByteArrayAsync();

            await File.WriteAllBytesAsync(Path.Combine(SaveDir, $"{dlcName}.png"), data);
        }
    }
}
