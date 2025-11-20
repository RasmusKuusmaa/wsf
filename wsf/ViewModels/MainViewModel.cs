using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public MainViewModel()
        {
            _isBingEnabled = GetBingState();
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
    }
}
