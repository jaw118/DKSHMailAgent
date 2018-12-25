using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;


namespace DKSH.MailAgent.Constants
{

    public class DKSHMailFolder
    {
        public const string POP3Log =   "C://jaws//ascdev//Pop3.log";
        public const string IMAP4Log =  "C://jaws//ascdev//Imap4.log";

        public const string FilePOP3 =  "C://jaws//ascdev//Pop3.log";
        public const string FileIMAP4 = "C://jaws//ascdev//Imap4.log";



        public const string ApiTypeLimited = "2";
        public const string ApiTypePublic = "3";
    }

    public class DKSHConfig
    {
        public const string MailConfig = "MailConfig";
        public const string LogConfig = "LogConfig";


       
    }


    public class ApiType
        {
            public const string ApiTypeProducer = "1";
            public const string ApiTypeLimited = "2";
            public const string ApiTypePublic = "3";
        }



        public class EntityType
        {
            public const string POP3EntryDate = "MB51EntryDate";
            public const string Supplier = "Supplier";
            public const string MPDL = "MPDL";
            public const string HsCode = "HsCode";
        }



        public class ImportStatusCode
        {
            public const string Canceled = "CCL";
            public const string Finished = "F";
            public const string Failed = "FLD";
            public const string New = "N";
            public const string InProgress = "P";
        }


        public class Month
        {
            public const string MonthJanuary = "1";
            public const string MonthOctober = "10";
            public const string MonthNovember = "11";
            public const string MonthDecember = "12";
            public const string MonthFebruary = "2";
            public const string MonthMarch = "3";
            public const string MonthApril = "4";
            public const string MonthMay = "5";
            public const string MonthJune = "6";
            public const string MonthJuly = "7";
            public const string MonthAugust = "8";
            public const string MonthSeptember = "9";
        }

        public class OrganizationUnit
        {
            public const string OUDepartment = "1";
            public const string OUSection = "2";
            public const string OUGroup = "3";

        }


        public class ResetEvery
        {
            public const string Daily = "Daily";
            public const string Monthly = "Monthly";
            public const string Never = "Never";
            public const string Yearly = "Yearly";

            public static (int Day, int Month, int Year) GetResetDetail(string resetEvery)
            {
                int day = 0;
                int month = 0;
                int year = 0;
                if (resetEvery == Yearly)
                {
                    year = DateTime.Today.Year;
                }
                else if (resetEvery == Monthly)
                {
                    year = DateTime.Today.Year;
                    month = DateTime.Today.Month;
                }
                else if (resetEvery == Daily)
                {
                    year = DateTime.Today.Year;
                    month = DateTime.Today.Month;
                    day = DateTime.Today.Day;
                }
                return (day, month, year);
            }
        }

        public class StatusCode
        {
            public const string Active = "A";
            public const string Approved = "APP";
            public const string Canceled = "C";
            public const string Closed = "CL";
            public const string Draft = "D";
            public const string Inactive = "I";
            public const string Index = "IN";
            public const string Progress = "P";
            public const string Rejected = "R";
            public const string WaitApproved = "W";
        }
   


}


 