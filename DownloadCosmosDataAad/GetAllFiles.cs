using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using VcClient;

namespace DownloadCosmosDataAad
{
    class GetFiles
    {
        public static Dictionary<string, string> GetAllFiles(string cosmosPath, string Ext, int recurseDir)
        {

            var dict = new Dictionary<string, string>();
            var baseCosmosPath = cosmosPath;
            foreach (var streamPath in GetStreamsRecurse(baseCosmosPath, new Regex(Ext), recurseDir))
            {


                var uri = new Uri(streamPath);
                var relativeStreamPath = uri.Segments[uri.Segments.Length - 1];
                dict.Add(streamPath, relativeStreamPath);

            }
            return dict;
        }

        private static IEnumerable<string> GetStreamsRecurse(string baseDirectory, Regex regex, int recurseDir)
        {
            List<StreamInfo> streams = null;

            int maxRetryAttempts = 5;
            TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(2);
            RetryHelper.RetryOnException(maxRetryAttempts, pauseBetweenFailures, () =>
            {
                streams = VC.GetDirectoryInfo(baseDirectory, false);
            });
            foreach (var streamInfo in streams)
            {
                var uri = new Uri(streamInfo.StreamName);
                if (recurseDir == 1 && streamInfo.IsDirectory)
                {

                    foreach (var subStream in GetStreamsRecurse(streamInfo.StreamName, regex, recurseDir))
                    {
                        yield return subStream;
                    }

                }
                else if (!streamInfo.IsDirectory && regex.IsMatch(uri.Segments[uri.Segments.Length - 1]))
                {
                    yield return streamInfo.StreamName;
                }
            }
        }
    }
}
