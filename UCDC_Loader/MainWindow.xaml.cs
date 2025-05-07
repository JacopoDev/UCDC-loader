using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Shell;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace UCDC_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext= this;
            StartDownload();
        }

        private async void StartDownload()
        {
            // 0. Create Target directory
            if (!Directory.Exists("KoboldCpp"))
            {
                Directory.CreateDirectory("KoboldCpp");
            }
            
            // 1. Download KoboldCpp
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "Resources/KoboldCppDownload.bat",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process { StartInfo = psi };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        var match = Regex.Match(e.Data, @"^[ ]*(\d*)");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int percent))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ProgressBar.Value = percent / 2f;
                                try
                                {
                                    TaskbarManager.Instance.SetProgressValue((int)ProgressBar.Value, 100);
                                }
                                catch
                                {
                                    ;
                                }
                            });
                        }
                    }
                };

                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();
            });
            
            //2. Download gguf Model 
            Title = "Downloading AI model...";
            await Task.Run(() =>
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "Resources/GGUFModelDownload.bat",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = new Process { StartInfo = psi };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        var match = Regex.Match(e.Data, @"^[ ]*(\d*)");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int percent))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ProgressBar.Value = percent / 2f + 50;
                                try
                                {
                                    TaskbarManager.Instance.SetProgressValue((int)ProgressBar.Value, 100);
                                }
                                catch
                                {
                                    ;
                                }
                            });
                        }
                    }
                };

                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();
            });
            

            FinishText.Visibility = Visibility.Visible;
            ChanParty.Visibility = Visibility.Visible;
            Height = 490;
            ShutdownOnFinish();
        }
        
        

        private void ShutdownOnFinish()
        {
            Task.Delay(new TimeSpan(0, 0, 2)).ContinueWith(o => { Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown()) ;});
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) 
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}