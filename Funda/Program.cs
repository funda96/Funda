using System;
using System.Collections.Generic;

namespace Funda
{
    class Program
    {
        // Get API URL and Key from config file
        static readonly string APIURL = System.Configuration.ConfigurationManager.AppSettings.Get("APIURL");
        static readonly string APIKEY = System.Configuration.ConfigurationManager.AppSettings.Get("APIKEY");

        static void Main(string[] args)
        {
            try
            {
                CReport oReport = new CReport(APIURL, APIKEY);
                List<CMakelaar> oMakelaars;

                Console.WriteLine("Retrieving information. Please wait...\n");

                // Top 10 Makelaars for all houses in Amsterdam
                oMakelaars = oReport.GetTopMakelaars("/amsterdam/", 10);
                Console.WriteLine("Top 10 Makelaars in Amsterdam:");
                foreach (CMakelaar oMakelaar in oMakelaars)
                {
                    Console.WriteLine(oMakelaar.NumWoningen.ToString() + " huizen: " + oMakelaar.MakelaarID + " (" + oMakelaar.MakelaarName + ")");
                }
                Console.WriteLine("");

                // Top 10 Makelaars for all houses with a yard in Amsterdam
                oMakelaars = oReport.GetTopMakelaars("/amsterdam/tuin/", 10);
                Console.WriteLine("Top 10 Makelaars in Amsterdam with tuin:");
                foreach (CMakelaar oMakelaar in oMakelaars)
                {
                    Console.WriteLine(oMakelaar.NumWoningen.ToString() + " huizen: " + oMakelaar.MakelaarID + " (" + oMakelaar.MakelaarName + ")");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Fatal exception. Cannot recover: " + ex.ToString());
            }
        }
    }
}
