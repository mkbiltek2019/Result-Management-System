using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PRMS.Models
{
    public class Messasges
    {
        //Success Messages  
        public static string DataInsertedSuccessfully = "Data Insertion Successfully!";
        public static string DataUploadedSuccessfully = "Data uploaded successfully!";
        public static string DataUpdatedSuccessfully = "Data updated successfully!";
        public static string DataDeletedSuccessfully = "Data deleted successfuly!";
        public static string Valid = "Valid!";
        public static string MailRegistrationInfo = "PRMS Registration Information";
        public static string MailPasswordRecovery = "PRMS Password Recovery";

        //Error Messages
        public static string InsertionFailed = "Insertion failed!";
        public static string DeletionFailed = "Failed to delete!";
        public static string DataUploadFailed = "Data upload failed!";
        public static string DataUpdateFailed = "Data update failed!";
        public static string InvalidExcelData = "Invalid excel data! May be wrong file choosen.";
        public static string InvalidExcelFileFormate = "File formate not supported. Only .xls or .xlsx is required.";
        public static string RequiredFiledsMissing = "Required field(s) missing.";
        public static string RecordNotFound = "Record not found!";
        public static string Invalid = "Invalid!";
        public static string InvalidRequest = "Invalid Request!";
        public static string InvalidEmail = "Invalid Email!";
        public static string InvalidUsername = "Invalid Username!";
      

    }
}