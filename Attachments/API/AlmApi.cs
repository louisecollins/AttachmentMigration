using System;
using System.Collections.Generic;
using RestSharp;


namespace Attachments.API
{
    public class AlmApi : BaseAPI
    {
                                   
             


        public static bool SignIntoALM(string userName, string passWord)
        {
            RestResponse response = SendRequestAndGetResponsePayload<RestResponse>(Properties.Settings.Default.almUrl + "api/authentication/sign-in", Method.GET, userName, passWord, queryParams:null, requestPayload: null, contentType: null);           
            if (response.IsSuccessful) { return true; } else { return false; }
        }

        public static bool DownloadAttachment(string almDomain, string almProjectName, int almDefectId, string attachmentName)
        {
            string downloadAttachmentEndPoint = "/rest/domains/"+ almDomain + "/projects/" + almProjectName + "/defects/" + almDefectId + "/attachments/" + attachmentName + "?login-form-required=y";
            //arrange - Set Up Query Data for the test           
            Dictionary<String, String> queryParams = new Dictionary<String, String> { };
            queryParams.Add("Accept", "application/octet-stream");

            var fileDownloaded = DownloadData(Properties.Settings.Default.almUrl + downloadAttachmentEndPoint, Method.GET, Properties.Settings.Default.attachmentFilePath, attachmentName);
            
            if (fileDownloaded) { return true; }
            else { return false; }
        }






    }
}


            


