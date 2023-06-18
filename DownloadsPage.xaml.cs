using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace WinUiApp
{
    /// <summary>
    /// Page where we list current and past downloads
    /// </summary>
    public sealed partial class DownloadsPage : Page
    {

        public DownloadsPage()
        {
            this.InitializeComponent();
            InstallManager.InstallationStatusChanged += HandleInstallationStatusChanged;
        }

        // Handing Installtion State Change
        // This function is never run on UI Thread
        // So always make sure to use Dispatcher Queue to update UI thread
        private void HandleInstallationStatusChanged(InstallItem installItem)
        {
            try
            {
                if (installItem == null)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        ActiveDownloadSection.Visibility = Visibility.Collapsed;
                    });
                    return;
                }
                DispatcherQueue.TryEnqueue(() =>
                {
                    ActiveDownloadSection.Visibility = Visibility.Visible;
                    CurrentDownloadTitle.Text = installItem.AppName;
                    DownloadProgressBar.IsIndeterminate = true;
                });

                switch (installItem.Status)
                {
                    case ActionStatus.Processing:
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            DownloadProgressBar.IsIndeterminate = false;
                            DownloadProgressBar.Value = Convert.ToDouble(installItem.ProgressPercentage);
                            CurrentDownloadAction.Text = $@"{installItem.Action}ing";
                            CurrentDownloadedSize.Text = $@"{installItem.DownloadedSize} MiB of  {installItem.TotalDownloadSizeMb} MiB";
                            CurrentDownloadSpeed.Text = $@"{installItem.DownloadSpeedRaw} MiB/s";
                        });
                        return;
                    case ActionStatus.Success:
                        ActiveDownloadSection.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}