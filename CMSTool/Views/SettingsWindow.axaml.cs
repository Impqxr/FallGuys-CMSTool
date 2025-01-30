using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using static FGCMSTool.Managers.SettingsManager;
using static FGCMSTool.Managers.LocalizationManager;
using Avalonia;
namespace FGCMSTool.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
#if RELEASE_LINUX_X64
            MenuTitle.IsVisible = false;
            MainContent.Margin = new Thickness(15, 10, 15, 0);
#endif

            Loaded += (sender, e) =>
            {
                EncryptCombo.SelectedIndex = (int)Settings.SavedSettings.EncryptStrart;
                DecryptCombo.SelectedIndex = (int)Settings.SavedSettings.DecryptStrat;
                XorKey.Text = Settings.SavedSettings.XorKey;
            };
        }

        void SetEncryptStrat(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender;
            Settings.SavedSettings.EncryptStrart = (EncryptStrart)combo.SelectedIndex;

            switch (Settings.SavedSettings.EncryptStrart)
            {
                case EncryptStrart.V1:
                    EncryptCombo_Desc.Text = LocalizedString("settings_encrypt_v1");
                    break;
                case EncryptStrart.V2:
                    EncryptCombo_Desc.Text = LocalizedString("settings_encrypt_v2");
                    break;
            }
        }

        void SetDecryptStrat(object sender, SelectionChangedEventArgs e)
        {
            var combo = (ComboBox)sender;
            Settings.SavedSettings.DecryptStrat = (DecryptStrat)combo.SelectedIndex;

            switch (Settings.SavedSettings.DecryptStrat)
            {
                case DecryptStrat.Default:
                    DecryptCombo_Desc.Text = LocalizedString("settings_decrypt_default");
                    break;
                case DecryptStrat.Formatting:
                    DecryptCombo_Desc.Text = LocalizedString("settings_decrypt_format");
                    break;
                case DecryptStrat.Parts:
                    DecryptCombo_Desc.Text = LocalizedString("settings_decrypt_parts");
                    break;
            }
        }

        void OnXorChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            Settings.SavedSettings.XorKey = box.Text;
        }

        void ResetXor(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(Settings.DefaultSettings.XorKey);
            XorKey.Text = Settings.DefaultSettings.XorKey;
        }

        void SaveSettings(object sender, RoutedEventArgs e)
        {
            Settings.Save();
            this.Close();
        }
    }
}
