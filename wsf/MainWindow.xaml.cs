using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wsf.ViewModels;

namespace wsf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel ViewModel => (MainViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (!IsAdministrator())
        {
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            var startInfo = new ProcessStartInfo(exeName)
            {
                Verb = "runas",
                UseShellExecute = true
            };

            try
            {
                var proc = Process.Start(startInfo);
                if (proc != null)
                {
                    Application.Current.Shutdown();
                }
            }
            catch
            {
                MessageBox.Show("Admin privileges are required to fix Windows Search.");
            }
        }
        else
        {
            ViewModel.FixWindowsSearch();
        }
    }

    public static bool IsAdministrator()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}
