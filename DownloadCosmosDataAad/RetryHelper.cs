using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadCosmosDataAad
{
    public static class RetryHelper
    {
        // private static ILog logger = LogManager.GetLogger(); //use a logger or trace of your choice

        public static void RetryOnException(int times, TimeSpan delay, Action operation)
        {
            var attempts = 0;
            do
            {
                try
                {
                    attempts++;
                    operation();
                    break; // Sucess! Lets exit the loop!
                }
                catch (Exception ex)
                {
                    if (attempts == times)
                        throw;
                    Program.LogError("Retry Error " + ex.ToString());
                    // logger.Error($"Exception caught on attempt {attempts} - will retry after delay {delay}", ex);

                    Task.Delay(delay).Wait();
                }
            } while (true);
        }
    }
}
