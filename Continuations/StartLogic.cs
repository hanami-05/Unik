using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Continuations
{
    internal class StartLogic
    {
        private Task ShowSplashScreenAsync(bool isFaulted)
        {
            return Task.Run(
                () =>
                {
                    if (isFaulted)
                    {
                        throw new Exception("Exception in SplashScreen");
                    }

                    else Console.WriteLine("Splash Screen");
                }
            );
        }

        private void RequestLicense(bool isFaulted)
        {
            Thread.Sleep(1000);
            if (isFaulted)
            {
                throw new Exception("Exception in Request License");
            }
            else Console.WriteLine("Request License");
        }

        private void CheckForUpdates(bool isFaulted) 
        {
            Thread.Sleep(1500);
            if (isFaulted)
            {
                throw new Exception("Exception in Check Updates");
            }
            else Console.WriteLine("Check Updates");
                    
        }

        private void SetupMenus() 
        {
            Thread.Sleep(2000);
            Console.WriteLine("Setup Menu");
        }

        private void DownloadUpdates() 
        {
            Thread.Sleep(3000);
            Console.WriteLine("Download Updates");
                
        }

        private void DisplayWelcomeScreen(bool isFaulted) 
        {
            Thread.Sleep(500);

            if (isFaulted) 
            {
                throw new Exception("Exception in Display Home Screen");
            }

            else Console.WriteLine("Display Welocome Screen");
        }

        private void HideSplashScreen() 
        {
            Thread.Sleep(700);
            Console.WriteLine("Hide Splash Screen");
        }

        private void OnFaulted(Task t) { Console.WriteLine(t.Exception?.Message); }

        public Task SetUpAcync(bool[] flags) 
        {
            Task splashScreenTask = ShowSplashScreenAsync(flags[0]);

            Task onSplashScreenFaultedTask = splashScreenTask.ContinueWith(
                 OnFaulted, TaskContinuationOptions.NotOnRanToCompletion
                );

            Task requestLicenseTask = splashScreenTask.ContinueWith(
                (Task t) => RequestLicense(flags[1]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task checkForUdpatesTask = splashScreenTask.ContinueWith(
                (Task t) => CheckForUpdates(flags[2]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task onRequestLicenseFaultedTask = requestLicenseTask.ContinueWith(
                    OnFaulted, TaskContinuationOptions.NotOnRanToCompletion
                );
            Task onCheckForUpdatesFaultedTask = checkForUdpatesTask.ContinueWith(
                    OnFaulted, TaskContinuationOptions.NotOnRanToCompletion
                );

            Task setupMenuTask = requestLicenseTask.ContinueWith(
                (Task t) => SetupMenus(),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task downloadUpdatesTask = checkForUdpatesTask.ContinueWith(
                (Task t) => DownloadUpdates(),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task displayWelcomeScreenTask = Task.WhenAll(setupMenuTask, downloadUpdatesTask)
                .ContinueWith(
                (Task t) => DisplayWelcomeScreen(flags[3]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task onDisplayWelcomeScreenFaultedTask = displayWelcomeScreenTask.ContinueWith(
                    OnFaulted, TaskContinuationOptions.NotOnRanToCompletion
                );

            Task hideSplashScreenTask = displayWelcomeScreenTask.ContinueWith(
                (Task t) => HideSplashScreen(),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            return hideSplashScreenTask;  
        }
    }
}
