using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using NurfUsMatchPuller.Classes.Data;

namespace NurfUs.Classes
{
    public class RESTHelpers
    {
        public static async Task<RESTResult<T>> RESTRequest<T>(string baseAddress, string relativePath, string apiKey, string extraGetVariables, byte requestType)
        {
            NurfUsEntities nurfUsEntities = new NurfUsEntities();

            RESTResult<T> restResult = new RESTResult<T>();
            
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);

                string getVariables = "?api_key=" + apiKey;

                if (!string.IsNullOrWhiteSpace(extraGetVariables))
                {
                    if (!extraGetVariables.StartsWith("&"))
                    {
                        getVariables += "&";
                    }

                    getVariables += extraGetVariables;
                }

                HttpResponseMessage result = await client.GetAsync(relativePath + getVariables);

                if (result.IsSuccessStatusCode)
                {
                    restResult.Success = true;

                    string resultContent = result.Content.ReadAsStringAsync().Result;

                    if (requestType == 4)
                    {
                        nurfUsEntities.APIResponses.Add(new APIRespons()
                        {
                            DateStamp = DateTime.UtcNow,
                            JSON = resultContent,
                            RequestType = requestType
                        });
                        nurfUsEntities.SaveChanges();
                    }

                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    T jsonResult = jss.Deserialize<T>(resultContent);

                    restResult.ReturnObject = jsonResult;
                }
                else
                {
                    restResult.Success = false;
                }

                restResult.StatusCode = result.StatusCode;

                return restResult;
            }
        }
    }
}