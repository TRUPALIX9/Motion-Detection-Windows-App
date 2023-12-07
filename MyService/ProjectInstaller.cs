using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace MyService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            // Configure the service process installer
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            // Configure the service installer
            this.serviceInstaller.ServiceName = "MyService";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // Add the installers to the collection
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                this.serviceProcessInstaller,
                this.serviceInstaller
            });
        }
        public override void Install( System.Collections.IDictionary stateSaver )
        {
            try
            {
                // Check if the application is running with administrator privileges
                if (!IsAdministrator())
                {
                    RestartAsAdministrator();
                    return;
                }

                base.Install(stateSaver);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during installation: {ex.Message}");
            }
        }

        // Helper method to check if the application is running with administrator privileges
        private static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            Console.WriteLine(principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator));
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        // Helper method to restart the application with elevated privileges
        private static void RestartAsAdministrator()
        {
            var processStartInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location)
            {
                UseShellExecute = true,
                Verb = "runas" 
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restarting as administrator: {ex.Message}");
            }

            Environment.Exit(0);
        }
    }
}
