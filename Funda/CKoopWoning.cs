using System;
using System.Collections.Generic;
using System.Text;

namespace Funda
{
    // Purpose:     Class to store information about the KoopWoning (WoningId, MakelaarID, MakelaarName).  
    public class CKoopWoning
    {
        public string WoningID = "";        
        public int MakelaarID = 0;
        public string MakelaarName = "";

        // Constructor
        public CKoopWoning(string sWoningID, int nMakelaarID, string sMakelaarName)
        {
            WoningID = sWoningID;
            MakelaarID = nMakelaarID;
            MakelaarName = sMakelaarName;
        }
    }
}
