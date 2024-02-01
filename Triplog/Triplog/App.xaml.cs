using System;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Triplog.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Triplog
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            Distribute.DisableAutomaticCheckForUpdate();
            // In this example OnReleaseAvailable is a method name in same class
            Distribute.NoReleaseAvailable = OnNoReleaseAvailable;
            Distribute.ReleaseAvailable = OnReleaseAvailable;
            AppCenter.Start("android=c2d4e7d4-bca8-4eb6-8912-aaf366ee53a0;",
            typeof(Analytics), typeof(Crashes));
        }
        void OnNoReleaseAvailable()
        {
            AppCenterLog.Info("AppCenterDemo", "No release available callback invoked");
        }
        
        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            // Look at releaseDetails public properties to get version information, release notes text or release notes URL
            string versionName = releaseDetails.ShortVersion;
            string versionCodeOrBuildNumber = releaseDetails.Version;
            string releaseNotes = releaseDetails.ReleaseNotes;
            Uri releaseNotesUrl = releaseDetails.ReleaseNotesUrl;

            // custom dialog
            var title = "Version " + versionName + " available!";
            Task answer;

            // On mandatory update, user can't postpone
            if (releaseDetails.MandatoryUpdate)
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install");
            }
            else
            {
                answer = Current.MainPage.DisplayAlert(title, releaseNotes, "Download and Install", "Maybe tomorrow...");
            }
            answer.ContinueWith((task) =>
            {
                // If mandatory or if answer was positive
                if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                {
                    // Notify SDK that user selected update
                    Distribute.NotifyUpdateAction(UpdateAction.Update);
                }
                else
                {
                    // Notify SDK that user selected postpone (for 1 day)
                    // This method call is ignored by the SDK if the update is mandatory
                    Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                }
            });

            // Return true if you're using your own dialog, false otherwise
            return true;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}