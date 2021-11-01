using System;
using System.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System.IO;
using Newtonsoft.Json;
using ExcelApp = Microsoft.Office.Interop.Excel;
using Attachments.API;

namespace Attachments
{
    class Program
    {

        //Set Up Variable we need to store the info
        public static int adoDefectId = 0;
        public static string adoAttachmentUrl = "";
        public static int almDefectId = 0;
        public static int revisionNumber = 0;
        public static string clientName = "ALLENOVERY";

        public static string testDataFilePath { get; private set; }
        public static string userName { get; private set; }
        public static string password { get; private set; }

        static void Main(string[] args)
        {
           
            //readvalues in from excel
            ExcelApp.Application excelApp = new ExcelApp.Application();

            ExcelApp.Workbook excelBook = excelApp.Workbooks.Open(testDataFilePath);
            ExcelApp._Worksheet excelSheet = excelBook.Sheets[1];
            ExcelApp.Range excelRange = excelSheet.UsedRange;
            int rowCount = excelRange.Rows.Count;
            int colCount = excelRange.Columns.Count;

            for (int i = 1; i <= rowCount; i++)
            {
                int almDefectId = (int)excelRange.Cells[i, 1].Value;
                string attachmentName = (string)excelRange.Cells[i, 2].Value.ToString();
                //Step 1. If the SignInto ALM is successful
                if (AlmApi.SignIntoALM(userName, password)) {
                    //THEN - Step 2. DownLoad ALM Attachment to AttachmentStore (location in properties)
                    if (AlmApi.DownloadAttachment(Properties.Settings.Default.almDomain, Properties.Settings.Default.almProjectName, almDefectId, attachmentName )) {
                        //THEN STEP 3. Post Attachment to ADO
                        (string id, string url) = AzureApi.PostFileToADO(attachmentName);
                        if(id !=null){
                            ////THEN STEP 4. Get CORRESPONDING Azure Defect ID
                            adoDefectId = AzureApi.getADODefectId(almDefectId);
                                if (adoDefectId !=0) {
                                //STEP 5 - Get Work Item Revision Number
                                revisionNumber = AzureApi.GetWorkItemRevisionNo(adoDefectId);
                                if (revisionNumber>0) {
                                    //Step 6 - Link The Attachment To the WorkItem (ADO Defect)
                                    if (AzureApi.LinkAttachmentToWorkItem(adoDefectId, almDefectId, revisionNumber, url))
                                    {
                                        Console.WriteLine("Step7 - The Attachment for ALM Defect: " + almDefectId + "was successfully uploaded to ADO Defect :" + adoDefectId);
                                    }
                                    else Console.WriteLine("Step7 - The Attachment for ALM Defect: " + almDefectId + "was NOT uploaded to ADO Defect :" + adoDefectId + "Please Try again Manually");
                                    { }

                                } else {
                                    //end iteration
                                    Console.WriteLine("Step6 - GET REVISION NUMBER FROM AZURE Failed for the current iteration, AlmDefectId: " + almDefectId + "Moving on to next Record ");
                                }
                            }
                            else {
                                //end iteration
                                Console.WriteLine("Step5 - GET DEFECT ID FROM AZURE Failed for the current iteration, AlmDefectId: " + almDefectId + "Moving on to next Record ");
                            }

                        }
                        else {
                            //end iteration
                            Console.WriteLine("Step 4 - POST ATTACHMENT to ADO  Failed for the current iteration, AlmDefectId: " + almDefectId + "Moving on to next Record ");
                        }
                            //Step 4. Get Corresponding BugID from Azure to which this Attachment belongs (using ALM Defect ID)
                            
                            {
                                //end iteration - STep 3
                                Console.WriteLine("Step 3 - POST ATTACHMENT to ADO  Failed for the current iteration, AlmDefectId: " + almDefectId + "Moving on to next Record ");
                        }
                    }
                    else {
                        //end iteration - Step 2
                        Console.WriteLine("Step 2 - DOWNLOAD ATTACHMENT  Failed for the current iteration, AlmDefectId: " + almDefectId + "Moving on to next Record ");
                    }
                }
                else
                { //end iteration - Step 1
                    Console.WriteLine("Step 1 - LOGIN TO ALM  Failed for the current iteration, AlmDefectId: " + almDefectId + "Moving on to next Record ");
                }
            }//end for    

       
                           





            //Release Excel App
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

    }
}

