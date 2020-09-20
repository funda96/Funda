using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Funda;

namespace FundaTest
{
    public class Test_CReport
    {
        CReport moReport;

        [SetUp]
        public void Setup()
        {
            moReport = new CReport("http://test.com", "test");
        }

        [Test]
        public void GetNumKoopWoningen_0()
        {
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();

            // 0 Houses
            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(oWoningen.Count, oWoningen));

            Assert.AreEqual(moReport.GetNumKoopWoningen(""), 0, "GetNumKoopWoningen Failed");
        }

        [Test]
        public void GetNumKoopWoningen_123456()
        {
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();

            // 123456 houses
            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(123456, oWoningen));

            Assert.AreEqual(moReport.GetNumKoopWoningen(""), 123456, "GetNumKoopWoningen Failed");
        }

        [Test]
        public void GetKoopWoningen_0()
        {
            Task<List<CKoopWoning>> oTask;
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();
            List<CKoopWoning> oResult;

            // 0 Houses
            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(oWoningen.Count, oWoningen));

            oTask = moReport.GetKoopWoningen("", 1, 25);
            oTask.Wait();
            oResult = oTask.Result;

            Assert.AreEqual(oWoningen.Count, oResult.Count, "GetKoopWoningen '0 Houses' Failed");
        }


        [Test]
        public void GetKoopWoningen_3()
        {
            Task<List<CKoopWoning>> oTask;
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();
            List<CKoopWoning> oResult;

            // 3 Houses
            oWoningen.Add(new CKoopWoning("a", 1, "Makelaar_1"));
            oWoningen.Add(new CKoopWoning("b", 2, "Makelaar_2"));
            oWoningen.Add(new CKoopWoning("c", 1, "Makelaar_1"));

            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(oWoningen.Count, oWoningen));
          
            oTask = moReport.GetKoopWoningen("", 1, 25);
            oTask.Wait();
            oResult = oTask.Result;

            Assert.AreEqual(oWoningen.Count, oResult.Count, "GetKoopWoningen '3 Houses' Failed");
            for (int i = 0; i < oWoningen.Count - 1; i++)
            {
                Assert.AreEqual(oWoningen[i].WoningID, oResult[i].WoningID, "GetKoopWoningen '3 Houses' Failed");
                Assert.AreEqual(oWoningen[i].MakelaarID, oResult[i].MakelaarID, "GetKoopWoningen '3 Houses' Failed");
                Assert.AreEqual(oWoningen[i].MakelaarName, oResult[i].MakelaarName, "GetKoopWoningen '3 Houses' Failed");
            }
        }

        // Test top 1 makelaar with 0 houses
        [Test]
        public void GetTopMakelaars_0()
        {
            List<CMakelaar> oMakelaars;
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();
            
            // 0 Houses
            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(oWoningen.Count, oWoningen));

            oMakelaars = moReport.GetTopMakelaars("", 1);
     
            Assert.AreEqual(oMakelaars.Count, 0, "GetTopMakelaars '0 Houses' Failed");
        }

        // Test top 1 makelaar
        [Test]
        public void GetTopMakelaars_1()
        {
            List<CMakelaar> oMakelaars;
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();

            // 3 Houses
            oWoningen.Add(new CKoopWoning("a", 1, "Makelaar_1"));
            oWoningen.Add(new CKoopWoning("b", 2, "Makelaar_2"));
            oWoningen.Add(new CKoopWoning("c", 1, "Makelaar_1"));

            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(oWoningen.Count, oWoningen));

            oMakelaars = moReport.GetTopMakelaars("", 1);

            Assert.AreEqual(oMakelaars.Count, 1, "GetTopMakelaars 'Top 1' Failed");
            Assert.AreEqual(oMakelaars[0].MakelaarID, 1, "GetTopMakelaars 'Top 1' Failed");
            Assert.AreEqual(oMakelaars[0].MakelaarName, "Makelaar_1", "GetTopMakelaars 'Top 1' Failed");
            Assert.AreEqual(oMakelaars[0].NumWoningen, 2, "GetTopMakelaars 'Top 1' Failed");
        }

        // Test top 2 makelaars
        [Test]
        public void GetTopMakelaars_2()
        {
            List<CMakelaar> oMakelaars;
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();

            // 3 Houses
            oWoningen.Add(new CKoopWoning("a", 1, "Makelaar_1"));
            oWoningen.Add(new CKoopWoning("b", 2, "Makelaar_2"));
            oWoningen.Add(new CKoopWoning("c", 1, "Makelaar_1"));

            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(oWoningen.Count, oWoningen));

            oMakelaars = moReport.GetTopMakelaars("", 2);

            Assert.AreEqual(oMakelaars.Count, 2, "GetTopMakelaars 'Top 2' Failed");
            Assert.AreEqual(oMakelaars[0].MakelaarID, 1, "GetTopMakelaars 'Top 2' Failed");
            Assert.AreEqual(oMakelaars[0].MakelaarName, "Makelaar_1", "GetTopMakelaars 'Top 2' Failed");
            Assert.AreEqual(oMakelaars[0].NumWoningen, 2, "GetTopMakelaars 'Top 2' Failed");
            Assert.AreEqual(oMakelaars[1].MakelaarID, 2, "GetTopMakelaars 'Top 2' Failed");
            Assert.AreEqual(oMakelaars[1].MakelaarName, "Makelaar_2", "GetTopMakelaars 'Top 2' Failed");
            Assert.AreEqual(oMakelaars[1].NumWoningen, 1, "GetTopMakelaars 'Top 2' Failed");
        }

        // Test top 10 makelaars, with only 2 makelaars
        [Test]
        public void GetTopMakelaars_10()
        {
            List<CMakelaar> oMakelaars;
            List<CKoopWoning> oWoningen = new List<CKoopWoning>();

            // 3 Houses
            oWoningen.Add(new CKoopWoning("a", 1, "Makelaar_1"));
            oWoningen.Add(new CKoopWoning("b", 2, "Makelaar_2"));
            oWoningen.Add(new CKoopWoning("c", 1, "Makelaar_1"));

            moReport.moTestData = new CTestData();
            moReport.moTestData.AddData(1, 25, CreateXML(oWoningen.Count, oWoningen));

            oMakelaars = moReport.GetTopMakelaars("", 10);

            Assert.AreEqual(oMakelaars.Count, 2, "GetTopMakelaars 'Top 10' Failed");
            Assert.AreEqual(oMakelaars[0].MakelaarID, 1, "GetTopMakelaars 'Top 10' Failed");
            Assert.AreEqual(oMakelaars[0].MakelaarName, "Makelaar_1", "GetTopMakelaars 'Top 10' Failed");
            Assert.AreEqual(oMakelaars[0].NumWoningen, 2, "GetTopMakelaars 'Top 10' Failed");
            Assert.AreEqual(oMakelaars[1].MakelaarID, 2, "GetTopMakelaars 'Top 10' Failed");
            Assert.AreEqual(oMakelaars[1].MakelaarName, "Makelaar_2", "GetTopMakelaars 'Top 10' Failed");
            Assert.AreEqual(oMakelaars[1].NumWoningen, 1, "GetTopMakelaars 'Top 10' Failed");
        }


        // Create cutdown version of XML used for testing
        private string CreateXML(int nNumWomingen, List<CKoopWoning> oWoningen)
        {
            string sWoningen = "";

            foreach(CKoopWoning oWoning in oWoningen)
            {
                sWoningen += "<Object><Id>" + oWoning.WoningID + "</Id>" +
                                     "<MakelaarId>" + oWoning.MakelaarID.ToString() + "</MakelaarId>" +
                                     "<MakelaarNaam>" + oWoning.MakelaarName + "</MakelaarNaam>" +
                             "</Object>";
            }
            return "<LocatieFeed xmlns =\"http://schemas.datacontract.org/2004/07/FundaAPI.Feeds.Entities\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><AccountStatus>Unknown</AccountStatus><EmailNotConfirmed>false</EmailNotConfirmed><ValidationFailed>false</ValidationFailed><ValidationReport i:nil=\"true\" xmlns:a=\"http://funda.nl/api/2010-05-11/validation\"/><Website>None</Website><Metadata><ObjectType>Koopwoningen</ObjectType><Omschrijving>Koopwoningen &gt; Hengelo (OV)</Omschrijving><Titel>Huizen te koop in Hengelo (OV)</Titel></Metadata><Objects>" +
                   sWoningen +
                   "</Objects><Paging><AantalPaginas>1</AantalPaginas><HuidigePagina>25</HuidigePagina><VolgendeUrl>/~/koop/hengelo-ov/p26/</VolgendeUrl><VorigeUrl>/~/koop/hengelo-ov/p24/</VorigeUrl></Paging><TotaalAantalObjecten>" + nNumWomingen.ToString() + "</TotaalAantalObjecten></LocatieFeed>";
        }
    }
}