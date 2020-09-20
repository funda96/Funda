using System;
using System.Collections.Generic;
using System.Text;

namespace Funda
{
    // Class to simulate HttpClient / return XML data needed for unit testing
    public class CTestData
    {
        Dictionary<string, string> moData = new Dictionary<string, string>();

        public void AddData(int nPage, int nPageSize, string sXML)
        {
            moData.Add(nPage.ToString() + "_" + nPageSize.ToString(), sXML);
        }

        public string GetString(int nPage, int nPageSize)
        {
            return moData[nPage.ToString() + "_" + nPageSize.ToString()];
        }

    }
}
