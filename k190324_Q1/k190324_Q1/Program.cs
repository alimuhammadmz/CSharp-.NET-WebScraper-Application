using System.Net;
using System.Reflection;
using System.Web;
using System;
using System.Configuration;

class WebPage
{
    private string downloadedPage;
        
    public WebPage()
    {
        downloadedPage = "";
    }
    public void DownloadWebpage(string remoteFilename)
    {
        WebClient client = new WebClient();
        string response = client.DownloadString(remoteFilename);

        downloadedPage = response;
    }

    public void CreateFile(string path)
    {
        DateTime now = DateTime.Now;
        
        string tmp = @path +"Summary" + now.ToString("ddMMMMyy") + ".html";
        
        FileStream tmpFile = File.Create(tmp);
        StreamWriter fs = new StreamWriter(tmpFile);

        fs.WriteLine(downloadedPage);
        fs.Close();
    }

    static void Main(string[] args)
    {
        WebPage myWebPage = new WebPage();
        string link = args[0];
        myWebPage.DownloadWebpage(link);
        myWebPage.CreateFile(args[1]);
        
        Console.WriteLine("Operations done successfully!");
    }
}