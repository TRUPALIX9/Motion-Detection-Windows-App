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
            this.serviceInstaller.DisplayName = "ZoneWatch Motion Detection Service";
            this.serviceInstaller.Description = "ZoneWatch: watches an RTSP camera for motion and writes events to the Application Event Log.";
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            // Add the installers to the collection
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
                this.serviceProcessInstaller,
                this.serviceInstaller
            });
        }
        public override void Install( System.Collections.IDictionary stateSaver )
        {
            // Installing a service needs an elevated prompt. Fail so InstallUtil reports it,
            // instead of relaunching without arguments and exiting with success.
            if (!IsAdministrator())
            {
                throw new System.Configuration.Install.InstallException("Run InstallUtil from an elevated (Run as administrator) command prompt.");
            }

            base.Install(stateSaver);
        }

        // Helper method to check if the application is running with administrator privileges
        private static bool IsAdministrator()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            Console.WriteLine(principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator));
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}
