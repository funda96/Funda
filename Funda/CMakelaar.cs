using System;
using System.Collections.Generic;
using System.Text;

namespace Funda
{
    // Class to store information about the Makelaar: MakelaarID, Name, and the number of KoopWoningen
    public class CMakelaar
    {
        public int MakelaarID = 0;
        public string MakelaarName = "";
        public int NumWoningen = 0;

        // Constructor
        public CMakelaar(int nMakelaarID, string sMakelaarName, int nNumWoningen)
        {
            MakelaarID = nMakelaarID;
            MakelaarName = sMakelaarName;
            NumWoningen = nNumWoningen;
        }
    }
}
