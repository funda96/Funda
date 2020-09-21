# Funda

The solution contains two projects:
- Funda: a console app which executes the two queries /amsterdam/ and /amsterdam/tuin/, and 
  displays the top 10 makelaars.
- FundaTest: NUnit test app, to run automated tests

Notes about the API
- Used https for API
- API seems to have some oddities. 
  1) Sometimes it returns more Objects on a specific page than asked for. E.g. it returns 27 Objects instead of the requested 25.
  2) The total number of returned Objects across all pages is slightly higher than reported in TotaalAantalObjecten.
  3) Invalid queries returns full Server stacktrace to the caller, which from a security perspective is not recommended.
     URL to reproduce:
     https://partnerapi.funda.nl/feeds/Aanbod.svc/ac1b0b1572524640a0ecc54de453ea9f/?type=makelaar&zo=&page=1&pagesize=1

Design Choices:
- Used asynch download of the API, so API calls and XML parsing can run in parallel
- A very simple throttle is implemented by waiting 600ms between each request (100 requests/min).
- Implemented CTestData to simulate data received from the API for the NUnit tests.

Project Files:
- Program.cs Console class, which outputs results in textual format to the console
- Report.cs Implements all main funtionality: calling the API, and collating the information
- CKoopWoning.cs Class to store woning ID, makelaar ID and makelaarname
- CMakelaar.cs Class to store and return aggregate stats for the makelaar: ID, name, and number of Woningen.
- CTestData.cs Class to store test data - simulated results to be returned from the API while running NUnit tests
- App.Config Stores URL and Key for the API
