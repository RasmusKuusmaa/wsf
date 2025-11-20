using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace wsf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private const string RegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Search";
        private const string BingKey = "BingSearchEnabled";
        private const string CortanaKey = "CortanaConsent";

        private bool _isBingEnabled;

        public bool IsBingEnabled
        {
            get => _isBingEnabled;
            set
            {
                if (_isBingEnabled != value)
                {
                    _isBingEnabled = value;
                    OnPropertyChanged(nameof(IsBingEnabled));
                    SetBingState(value);
                }
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged(nameof(StatusMessage));
                }
            }
        }

        public MainViewModel()
        {
            _isBingEnabled = GetBingState();
            _statusMessage = "Ready.";

        }

        private bool GetBingState()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null) return true; 
                object bingValue = key.GetValue(BingKey, 1);
                return bingValue != null && (int)bingValue != 0;
            }
        }

        private void SetBingState(bool enabled)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPath, true))
            {
                if (key == null) return;
                key.SetValue(BingKey, enabled ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue(CortanaKey, enabled ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        public void FixWindowsSearch()
        {
            try
            {
                StatusMessage = "Checking Windows Search service...";

                using (ServiceController sc = new ServiceController("WSearch"))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                        StatusMessage = "Windows Search service started.";
                    }
                    else
                    {
                        StatusMessage = "Windows Search service already running.";
                    }
                }

                StatusMessage = "Rebuilding search index";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Search", true))
                {
                    if (key != null)
                    {
                        key.SetValue("SetupCompletedSuccessfully", 0, RegistryValueKind.DWord);
                    }
                }

                StatusMessage = "Windows Search fix applied.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error fixing Windows Search: {ex.Message}";
            }
        }
    }
}
