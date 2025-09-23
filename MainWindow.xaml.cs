using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Text.Json;
using System.IO;


namespace WPFUI
{

    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {

        List<DNS_Profile>? PR1 = new List<DNS_Profile>();
        public void DNS_JSON()
        {
            string filePath = "Data/Default.json";
            string JsonString = File.ReadAllText(filePath);

            PR1 = JsonSerializer.Deserialize<List<DNS_Profile>>(JsonString);
            foreach(var item in PR1)
            {
                ListCombo.Items.Add(new ComboBoxItem { Content = item.Name });
            }

        }
        public void DNSListRefresh(object sender , RoutedEventArgs e)
        {
            var Boxes = new[] { IN1, IN2, IN3, IN4, SEC1, SEC2, SEC3, SEC4 };
            if ((ListCombo.SelectedItem as  ComboBoxItem)?.Content.ToString() == "Custom") 
            {
                if (IN1 != null)
                {
                    foreach (var item in Boxes)
                    {
                        item.IsEnabled = true;
                    }
                }
            } else
            {
                if (IN1 != null)
                {
                    foreach (var item in Boxes)
                    {
                        item.IsEnabled = false;
                    }

                    foreach (var item in PR1)
                    {
                        if(((ListCombo.SelectedItem as ComboBoxItem)?.Content.ToString() == item.Name))
                        {
                            IN1.Text = item.IN1.ToString();
                            IN2.Text = item.IN2.ToString();
                            IN3.Text = item.IN3.ToString();
                            IN4.Text = item.IN4.ToString();

                            SEC1.Text = item.SEC1.ToString();
                            SEC2.Text = item.SEC2.ToString();
                            SEC3.Text = item.SEC3.ToString();
                            SEC4.Text = item.SEC4.ToString();

                        }
                    }
                }
            }
        }
        public void TextBlock_KeyDown(object sender , KeyEventArgs e)
        {
            if(sender is Wpf.Ui.Controls.TextBox currentTextbox)
            {
                if(e.Key == Key.Right)
                {
                    e.Handled = true;
                    if (currentTextbox == IN1)
                    {
                        IN2.Focus();
                    }
                    else if (currentTextbox == IN2)
                    {
                        IN3.Focus();
                    }
                    else if (currentTextbox == IN3)
                    {
                        IN4.Focus();
                    }



                    else if (currentTextbox == SEC1)
                    {
                        SEC2.Focus();
                    }
                    else if (currentTextbox == SEC2)
                    {
                        SEC3.Focus();
                    }
                    else if (currentTextbox == SEC3)
                    {
                        SEC4.Focus();
                    }


                }
                else if (e.Key == Key.Left)
                {
                    e.Handled = true;
                    if (currentTextbox == IN4)
                    {
                        IN3.Focus();
                    }
                    else if (currentTextbox == IN3)
                    {
                        IN2.Focus();
                    }
                    else if (currentTextbox == IN2)
                    {
                        IN1.Focus();
                    }


                    else if (currentTextbox == SEC4)
                    {
                        SEC3.Focus();
                    }
                    else if (currentTextbox == SEC3)
                    {
                        SEC2.Focus();
                    }
                    else if (currentTextbox == SEC2)
                    {
                        SEC1.Focus();
                    }
                }
            }
        }
        private bool Dark = true;
        public MainWindow()
        {
            InitializeComponent();
            this.Icon = new BitmapImage(new Uri("icon/main.ico" , UriKind.Relative));
            this.WindowBackdropType = WindowBackdropType.Acrylic;
            DNS_JSON();
            this.ExtendsContentIntoTitleBar = true;
            //this.WindowBackdropType = WindowBackdropType.None;
            var Adaptors = NetworkInterface.GetAllNetworkInterfaces();
            AdaptorCombo.Items.Add(new ComboBoxItem { Content = "Select an adaptor",IsEnabled=false,IsSelected=true });
            foreach(var item in Adaptors)
            {
                var ITEM = new ComboBoxItem
                {
                    Content = item.Name,
                    
                };
                AdaptorCombo.Items.Add(ITEM);
            }
        }
        public async void BTNSwitch(object sender , RoutedEventArgs e)
        {
            Wpf.Ui.Controls.MessageBox A = new Wpf.Ui.Controls.MessageBox()
            {
                Content = "Please select an adaptor",
                CloseButtonText = "OK",
                Width = 800,
                Height = 500
            };
            var Adaptors = NetworkInterface.GetAllNetworkInterfaces();

            if(AdaptorCombo.SelectedItem is null || (AdaptorCombo.SelectedItem as ComboBoxItem).Content.ToString() == "Select an adaptor")
            {
                await A.ShowDialogAsync();
                
            }else
            {
                string DNS1 = $"{IN1.Text}.{IN2.Text}.{IN3.Text}.{IN4.Text}";
                string DNS2 = $"{SEC1.Text}.{SEC2.Text}.{SEC3.Text}.{SEC4.Text}";

                if (AdaptorCombo.SelectedItem is ComboBoxItem selectedItem)
                {
                    RunCommand($"netsh interface ip set dns name=\"{selectedItem.Content.ToString()}\" static {DNS1}");

                    // Add secondary DNS
                    RunCommand($"netsh interface ip add dns name=\"{selectedItem.Content.ToString()}\" {DNS2} index=2");
                }
                A.Content = "DNS set successfuly";
                await A.ShowDialogAsync();
            }


        }


        private static void ExecuteCommand(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", $"/c {command}") { RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true };

            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.Start(); string result = process.StandardOutput.ReadToEnd(); process.WaitForExit();
            }
        }
        static void RunCommand(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C " + command)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    process?.WaitForExit();



                    if (process?.ExitCode != 0)
                    {
                        string error = process?.StandardError.ReadToEnd();
                        throw new InvalidOperationException(error);
                    }
                }
            }
            catch
            {
                
            }
        }

        public void BTN(object sender , RoutedEventArgs e)
        {
            
            if (Dark)
            {
                ApplicationThemeManager.Apply(
                    ApplicationTheme.Light,
                    Wpf.Ui.Controls.WindowBackdropType.Acrylic,
                    true
                );
                Dark = false;
            }
            else
            {
                ApplicationThemeManager.Apply(
                    ApplicationTheme.Dark,
                    Wpf.Ui.Controls.WindowBackdropType.Acrylic,
                    true
                );
                Dark = true;
            }
            
        }
    }
}