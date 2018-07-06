using System;
using System.Net.Http;

namespace AshamaneDependencyManager
{
    class HttpHelper
    {
        public static String GetRawGithubURL(String project, String file)
        {
            Random rand = new Random();
            return "https://raw.githubusercontent.com/" + project + "/master/" + file + "?rand=" + rand.NextDouble();
        }

        public static String GetURLContent(String url)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    return responseContent.ReadAsStringAsync().Result;
                }
            }

            throw new Exception("Invalid module.json for that Github project");
        }
    }
}
