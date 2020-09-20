using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace Funda
{
    public class CReport
    {
        // API URL and Key
        private string APIURL = "";
        private string APIKEY = "";

        // HttpClient is designed for re-use. For better performance use one instance only
        private HttpClient moAPIClient;
        public CTestData moTestData;

        // Constructor: API Key and URL to connect to. Instantiate HttpClient
        public CReport(string sAPIURL, string sAPIKEY )
        {
            APIURL = sAPIURL;
            APIKEY = sAPIKEY;
            moAPIClient = new HttpClient();
            moAPIClient.BaseAddress = new Uri(APIURL);
        }

        // Get the top-N makelaars for a given query. Returns a list of makelaars with the top makelaars first.
        public List<CMakelaar> GetTopMakelaars(string sQuery, int nTopN)
        {
            const int nPageSize = 25;
            List<Task<List<CKoopWoning>>> oTasks = new List<Task<List<CKoopWoning>>>();
            int nNumKoopWoningen;
            int nNumRequested = 0;
            int nPage = 0;

            // Get number of koopwoningen
            nNumKoopWoningen = GetNumKoopWoningen(sQuery);

            // Download all koopwoningen, asynchronous
            do
            {
                nPage += 1;
                oTasks.Add(GetKoopWoningen(sQuery, nPage, nPageSize));
                nNumRequested += nPageSize;

                System.Threading.Thread.Sleep(600);         // Rate limit to 100 requests per 60 seconds
            } while (nNumRequested < nNumKoopWoningen);

            // Wait for all tasks to complete
            Task.WaitAll(oTasks.ToArray());

            // Collate the information, and get the top N makelaars
            return CollateInformation(oTasks, nTopN);
        }


        // Read number of available huizen. Read beyond the last page (page 10000), as it is not
        // necessary to get any actual house information.
        public int GetNumKoopWoningen(string sQuery)
        {
            string sURL = "feeds/Aanbod.svc/" + APIKEY + "/?type=koop&zo=" + sQuery + "&page=10000&pagesize=1000";
            Task<string> oTask;
            string sXML = "";
            int nNumTries = 0;

            do
            {
                try
                {
                    if (APIKEY == "test")           // Unit testing?
                        sXML = moTestData.GetString(1, 25);
                    else                            // Regular usage, call API
                    {
                        oTask = moAPIClient.GetStringAsync(sURL);
                        oTask.Wait();
                        sXML = oTask.Result;
                    }
                }
                catch
                {
                    // Try again in 1 sec. Try 60 times. If still failure, then throw exception
                    System.Threading.Thread.Sleep(1000);
                    nNumTries += 1;
                    if(nNumTries>60)
                        throw;
                }
            }
            while (sXML.Length == 0);

            // Retrieve the number of huizen from the XML
            XmlDocument oXML = new XmlDocument();
            oXML.LoadXml(sXML);

            return int.Parse(oXML.GetElementsByTagName("TotaalAantalObjecten")[0].InnerText); 
        }


        // Read the houses on the specified page.
        public async Task<List<CKoopWoning>> GetKoopWoningen(string sQuery, int nPage, int nPageSize)
        {
            string sURL = "feeds/Aanbod.svc/" + APIKEY + "/?type=koop&zo=" + sQuery + "&page=" + nPage.ToString() + "&pagesize=" + nPageSize.ToString();
            XmlNamespaceManager oXMLNameSpace;
            XmlDocument oXML = new XmlDocument();
            XmlNodeList oNodes;
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();
            CKoopWoning oWoning;
            string sXML = "";
            string sMakelaarName;
            string sWoningID;
            int nMakelaarID;
            int nNumTries = 0;

            do
            {
                try {
                    if (APIKEY == "test")           // Unit testing?
                        sXML = moTestData.GetString(nPage, nPageSize);
                    else                            // Regular usage, call API
                    {
                        sXML = await moAPIClient.GetStringAsync(sURL);
                    }
                }
                catch
                {
                    // Try again in 1 sec. Try 60 times. If still failure, then throw exception
                    System.Threading.Thread.Sleep(1000);
                    nNumTries += 1;
                    if (nNumTries > 60)
                        throw;
                }
            } while (sXML.Length == 0);

            if (sXML.Length>0)
            {
                // Load XML document, and XML-namespace
                oXML.LoadXml(sXML);
                oXMLNameSpace = new XmlNamespaceManager(oXML.NameTable);
                oXMLNameSpace.AddNamespace("Funda", "http://schemas.datacontract.org/2004/07/FundaAPI.Feeds.Entities");

                oNodes = oXML.GetElementsByTagName("Object");

                // Loop through all Woningen and get the relevant information (Id, MakerlaarId, MakelaarNaam)
                foreach(XmlNode oNode in oNodes)
                {
                    sWoningID = oNode.SelectSingleNode("Funda:Id", oXMLNameSpace).InnerText;
                    nMakelaarID = int.Parse(oNode.SelectSingleNode("Funda:MakelaarId", oXMLNameSpace).InnerText);
                    sMakelaarName = oNode.SelectSingleNode("Funda:MakelaarNaam", oXMLNameSpace).InnerText;
                   
                    oWoning = new CKoopWoning(sWoningID, nMakelaarID, sMakelaarName);
                    oWoningen.Add(oWoning);
                }
            }

            // Return the list of all Woningen
            return oWoningen;
        }


        // Process the list of houses produced by each individual call to the APi.
        // Collate them together by makelaar ID, and then find the top nTopN makelaars.
        // It is possible that multiple makelaars have the same number of houses available. 
        List<CMakelaar> CollateInformation(List<Task<List<CKoopWoning>>> oTasks, int nTopN)
        {
            List<CMakelaar> oMakelaars = new List<CMakelaar>();
            Dictionary<int, Dictionary<string, CKoopWoning>> oWoningenByMakelaar = new Dictionary<int, Dictionary<string, CKoopWoning>>();
            Dictionary<int, string> oMakelaarNames = new Dictionary<int, string>();
            SortedList<int, List<int>> oMost = new SortedList<int, List<int>>();

            // Collate all information. Makelaar -> List of Woningen 
            foreach (Task<List<CKoopWoning>> oTask in oTasks)
            {
                foreach (CKoopWoning oWoning in oTask.Result)
                {
                    if (!oWoningenByMakelaar.ContainsKey(oWoning.MakelaarID))
                    {
                        oWoningenByMakelaar.Add(oWoning.MakelaarID, new Dictionary<string, CKoopWoning>());
                        oMakelaarNames.Add(oWoning.MakelaarID, oWoning.MakelaarName);
                    }
                    oWoningenByMakelaar[oWoning.MakelaarID][oWoning.WoningID] = oWoning;
                }
            }

            // Sort, but keep the top 10 only. Sorting 10 elements repeatedly is still O(1)
            // with a total runtime of O(N)
            // Because multiple makelaars can have the same number of houses we use:
            //    oMost     Number of houses -> List of makelaars
            foreach (int nMakelaarID in oWoningenByMakelaar.Keys)
            {
                int nNumWoningen = oWoningenByMakelaar[nMakelaarID].Count;

                // Add to the list of the top makelaars, if better than the bottom makelaar we have stored
                if (oMost.Count < nTopN || nNumWoningen > oMost.Keys[0])
                {
                    // Add new entry if not already existing. Remove the worst one from the list 
                    if (!oMost.ContainsKey(nNumWoningen)) {
                        if (oMost.Count >= nTopN)
                            oMost.Remove(oMost.Keys[0]);

                        oMost.Add(nNumWoningen, new List<int>());    
                    }

                    oMost[nNumWoningen].Add(nMakelaarID);
                }
            }

            // Multiple makelaars might have the same number of houses available. 
            // Limit to nTopN makelaars.
            for (int i = oMost.Count - 1; i >= 0; i--)
            {
                foreach (int nMakelaarID in oMost[oMost.Keys[i]])
                {
                    oMakelaars.Add(new CMakelaar(nMakelaarID, oMakelaarNames[nMakelaarID], oWoningenByMakelaar[nMakelaarID].Count));
                    if (oMakelaars.Count == nTopN)
                        return oMakelaars;                  // We're done
                }
            }

            // Return all the makelaars, if there are fewer than the requested number
            return oMakelaars;
        }

    }
}
