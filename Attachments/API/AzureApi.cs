using Attachments.API.Responses;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace Attachments.API
{
    public class AzureApi : BaseAPI
    {
        public static (string id, string url) PostFileToADO(string attachmentName)
        {
            string endpointURL = "/_apis/wit/attachments?fileName=" + attachmentName + "&api-version=5.0";
            Dictionary<string, string> queryParams = new Dictionary<string, string> { };
            queryParams.Add("Content-Type", "application/octet-stream");
           // queryParams.Add("application/binary", File.ReadAllBytes(Properties.Settings.Default.attachmentFilePath + attachmentName), ParameterType.RequestBody);


            AdoAttachment response = SendRequestAndGetResponsePayload<AdoAttachment>(Properties.Settings.Default.azureUrl + endpointURL, Method.POST, Properties.Settings.Default.azureUserName, Properties.Settings.Default.azurePassword, queryParams: queryParams, requestPayload: File.ReadAllBytes(Properties.Settings.Default.attachmentFilePath + attachmentName), contentType: "application/octet-stream");
            if (response.Id != null)
            {
                return (response.Id, response.Url);
            }
            else
            {
                return ("", "");
            }
        }


        public static int getADODefectId(int almDefectId)
        {
            string wiql = "SELECT[ID] FROM WorkItem WHERE[Work Item Type] = 'Bug' AND[ALM Defect ID] = '" + almDefectId + "'";
            string endpoint = "/_apis/wit/wiql?api-version=5.1";
            var body = new { Query = wiql };

            AdoWorkItemList response = SendRequestAndGetResponsePayload<AdoWorkItemList>(Properties.Settings.Default.azureUrl + endpoint, Method.POST, Properties.Settings.Default.azureUserName, Properties.Settings.Default.azurePassword, queryParams: null, requestPayload: body, contentType: "application/json");
            return response.WorkItems[0].Id;

        }


        public static int GetWorkItemRevisionNo(int adoDefectId)
        {
            string endpoint = "/_apis/wit/workitems/" + adoDefectId + "/revisions";

            AdoWorkItemRevisions response = SendRequestAndGetResponsePayload<AdoWorkItemRevisions>(Properties.Settings.Default.azureUrl + endpoint, Method.GET, Properties.Settings.Default.azureUserName, Properties.Settings.Default.azurePassword, queryParams: null, requestPayload: null, contentType: "application/json");
            return response.Count;

        }

        public static bool LinkAttachmentToWorkItem(int adoDefectId, int almDefectId, int revisionNumber, string adoAttachmentUrl)
        {
            string endpoint = "/_apis/wit/workitems/" + adoDefectId + "?api-version=5.1";
            var patchBodyJson = "[{\"op\": \"test\",\"path\": \"/rev\",\"value\": " + revisionNumber + "},{\"op\": \"add\", \"path\": \"/fields/System.History\", \"value\": \"Uploaded the attachment from ALM into ADO\"}, {\"op\": \"add\", \"path\": \"/relations/-\", \"value\": {\"rel\": \"AttachedFile\",\"url\": \"" + adoAttachmentUrl + "\",\"attributes\": {\"comment\": \"This attachment has been migrated from HP ALM from ALM Defect: " + almDefectId + "\"} }}]";

            RestResponse response = SendRequestAndGetResponsePayload<RestResponse>(Properties.Settings.Default.azureUrl + endpoint, Method.PATCH, Properties.Settings.Default.azureUserName, Properties.Settings.Default.azurePassword, queryParams: null, requestPayload: patchBodyJson, contentType: "application/json-patch+json");
            return response.IsSuccessful;
        }


    }
}