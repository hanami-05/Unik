using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Continuations
{
    internal class StartLogic
    {
        private Task ShowSplashScreenAsync(int exceptionCode)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    if (exceptionCode == 0)
                    {
                        throw new Exception("Exception in SplashScreen");
                    }

                    else Console.WriteLine("Splash Screen");
                }
            );
        }

        private void RequestLicense(int exceptionCode)
        {
            Thread.Sleep(1000);
            if (exceptionCode == 0)
            {
                throw new Exception("Exception in Request License");
            }
            else Console.WriteLine("Request License");
        }

        private void CheckForUpdates(int exceptionCode) 
        {
            Thread.Sleep(1500);
            if (exceptionCode == 0)
            {
                throw new Exception("Exception in Check Updates");
            }
            else Console.WriteLine("Check Updates");
                    
        }

        private void SetupMenus(int exceptionCode) 
        {
            Thread.Sleep(2000);
            if (exceptionCode == 0)
            {
                throw new Exception("Exception in Setup Menu");
            }
            else Console.WriteLine("Setup Menu");
        }

        private void DownloadUpdates(int exceptionCode) 
        {
            Thread.Sleep(3000);
            if (exceptionCode == 0)
            {
                throw new Exception("Exception in Download Updates");
            }
            else Console.WriteLine("Download Updates");
                
        }

        private void DisplayWelcomeScreen(int exceptionCode) 
        {
            Thread.Sleep(500);
            if (exceptionCode == 0)
            {
                throw new Exception("Exception in Display Welocome Screen");
            }
            else Console.WriteLine("Display Welocome Screen");
        }

        private void HideSplashScreen(int exceptionCode) 
        {
            Thread.Sleep(700);
            if (exceptionCode == 0)
            {
                throw new Exception("Exception in Hide Splash Screen");
            }
            else Console.WriteLine("Hide Splash Screen");
        }

        public Task SetUpAcync(int[] exceptionCodes) 
        {
            Task splashScreenTask = ShowSplashScreenAsync(exceptionCodes[0]);

            Task requestLicenseTask = splashScreenTask.ContinueWith(
                (Task t) => RequestLicense(exceptionCodes[1]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task checkForUdpatesTask = splashScreenTask.ContinueWith(
                (Task t) => CheckForUpdates(exceptionCodes[2]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task setupMenuTask = requestLicenseTask.ContinueWith(
                (Task t) => SetupMenus(exceptionCodes[3]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task downloadUpdatesTask = checkForUdpatesTask.ContinueWith(
                (Task t) => DownloadUpdates(exceptionCodes[4]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task displayWelcomeScreenTask = Task.WhenAll(setupMenuTask, downloadUpdatesTask)
                .ContinueWith(
                (Task t) => DisplayWelcomeScreen(exceptionCodes[5]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            Task hideSplashScreenTask = displayWelcomeScreenTask.ContinueWith(
                (Task t) => HideSplashScreen(exceptionCodes[6]),
                TaskContinuationOptions.OnlyOnRanToCompletion
                );

            return hideSplashScreenTask;  
        }
    }
}
