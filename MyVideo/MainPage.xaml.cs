using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyVideo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Play(object sender, RoutedEventArgs e) {
            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;
            mediaPlayer.Play();
            EllStoryboard.Begin();
        }

        private void Pause(object sender, RoutedEventArgs e) {
            PlayButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Collapsed;
            mediaPlayer.Pause();
            EllStoryboard.Pause();
        }

        private void Stop(object sender, RoutedEventArgs e) {
            PlayButton.Visibility = Visibility.Visible;
            PauseButton.Visibility = Visibility.Collapsed;
            mediaPlayer.Stop();
            EllStoryboard.Stop();
        }

        private async void ChooseFile(object sender, RoutedEventArgs e) {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();

            if (file != null) {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                mediaPlayer.SetSource(stream, file.ContentType);
                this.Pause(mediaPlayer, null);
                if (file.ContentType.StartsWith("video")) { 
                    this.ellipse.Visibility = Visibility.Collapsed;
                    this.mediaPlayer.Visibility = Visibility.Visible;
                } else {
                    this.ellipse.Visibility = Visibility.Visible;
                    this.mediaPlayer.Visibility = Visibility.Collapsed;
                }
                EllStoryboard.Stop();
            }
        }

        private void FullScreen(object sender, RoutedEventArgs e) {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isInFullScreenMode = view.IsFullScreenMode;
            if (isInFullScreenMode) {
                view.ExitFullScreenMode();
            } else {
                view.TryEnterFullScreenMode();
            }
        }

        private void Volumn_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
            mediaPlayer.Volume = Volumn.Value;
        }


        private void MediaPlayerOpen(object sender, RoutedEventArgs e) {
            slider.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
        }
    }

    class MusicConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ((TimeSpan)value).TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return TimeSpan.FromSeconds((double)value);
        }
    }
}
