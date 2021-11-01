using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Attachments.API
{
    public class BaseAPI
    {

        /// <summary>
        /// This method sends a http request and returns a custom response payload object T. 
        /// </summary>
        /// <param name="requestUri">The Uri for the request</param>
        /// <param name="httpMethod">The http method e.g. HttpMethod.Post</param>
        /// <param name="headerAccept">The payload for the request</param>
        /// <param name="requestPayload">The payload for the request</param>
        /// <param name="cookieToken">The cookie token</param>
        public static T SendRequestAndGetResponsePayload<T>(String requestUri, Method method, String userName = null, String password = null,  Dictionary<String, String> queryParams = null, Object requestPayload = null, String contentType = null)
        {
            var response = SendRequest(requestUri, method, userName, password, requestPayload, contentType, queryParams);
            return JsonConvert.DeserializeObject<T>(response.Content);


        }

        public static bool SendRequestAndGetStatus(String requestUri, Method method, String requestPayload = null, String contentType = null)
        {
            bool statusOK = false;
            var response = SendRequest(requestUri, method, requestPayload, contentType);
            if (response.StatusDescription.Equals("OK"))
            {
                statusOK = true; ;
            }
            return statusOK;
        }
               


        private static RestResponse SendRequest(String requestUri, Method method, String userName, String password, Object requestPayload = null, String contentType = null, Dictionary<String, String> queryParams = null)
        {
            var restClient = new RestClient();

            restClient.CookieContainer = new System.Net.CookieContainer();
            restClient.Authenticator = new HttpBasicAuthenticator(userName, password);            

            RestRequest request = new RestRequest(requestUri, method);
            request.AddHeader("Content-Type", contentType);
           // request.AddHeader("Authorization", "Bearer " + bearerToken);
            if (queryParams != null)
            {

                foreach (var param in queryParams)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }

            // do not add body payload to a Get Request!
            if (method.ToString() != "GET")
            {
                request.AddJsonBody(requestPayload);

            }
            var response = restClient.Execute(request);
            return (RestResponse)response;
        }


        public static bool DownloadData(String requestUri, Method method, string filePath, string fileName, String userName = null, String password=null, Object requestPayload = null, String contentType = null, Dictionary<String, String> queryParams = null)
        {
            var restClient = new RestClient();

            restClient.CookieContainer = new System.Net.CookieContainer();
            restClient.Authenticator = new HttpBasicAuthenticator(userName, password);

            RestRequest request = new RestRequest(requestUri, method);
            request.AddHeader("Content-Type", contentType);
            // request.AddHeader("Authorization", "Bearer " + bearerToken);
            if (queryParams != null)
            {

                foreach (var param in queryParams)
                {
                    request.AddQueryParameter(param.Key, param.Value);
                }
            }

            // do not add body payload to a Get Request!
            if (method.ToString() != "GET")
            {
                request.AddJsonBody(requestPayload);

            }
            var filebites = restClient.DownloadData(request);            
            File.WriteAllBytes(Path.Combine(filePath, fileName), filebites);
            return true;
        }

    }

}











