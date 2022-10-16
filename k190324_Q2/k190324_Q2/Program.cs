using System.Collections;
using System.Text.Encodings.Web;
using HtmlAgilityPack;
using Spire.Doc;
using Spire.Doc.Documents;
using System;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using System.Configuration;

class HtmlFileManipulation
{
    private String path;

    public HtmlFileManipulation()
    {
        var appSettings = ConfigurationManager.AppSettings;
        this.path = appSettings["path"];
    }

    public void CreateDirectories(HtmlAgilityPack.HtmlNodeCollection tables, ArrayList categoriesList)
    {
        for (int i = 0; i < tables.Count; i++)
        {
            HtmlAgilityPack.HtmlNodeCollection categories = tables[i].SelectNodes("//th[@colspan='8']");
            categoriesList.Add(categories[i].InnerText.Trim());

            string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
            for (int cnt = 0; cnt < chars.Length; cnt++)
            {
                if (categoriesList[i].ToString().Contains(chars[cnt]))
                    categoriesList[i] = categoriesList[i].ToString().Replace(chars[cnt], "");
            }

            string dirPath = @path + categoriesList[i].ToString();
            if (!Directory.Exists(@dirPath))
                Directory.CreateDirectory(@dirPath);
        }
    }

    public void ManipulateContent()
    {
        HtmlDocument doc = new HtmlDocument();
        doc.OptionFixNestedTags = true;

        // path is a path given by the user
        DateTime now = DateTime.Now;
        string ph = @path + "Summary" + now.ToString("ddMMMMyy") + ".html";

        doc.Load(ph);
        if (doc.DocumentNode != null)
        {
            //extracting the tables in HTMLCollection
            var xmlDoc = new XmlDocument();
            HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode("//body");
            HtmlAgilityPack.HtmlNodeCollection tables = node.SelectNodes("//div[@class='table-responsive']");

            ArrayList categoriesList = new ArrayList();
            ArrayList headingsList = new ArrayList();
            ArrayList pricesList = new ArrayList();

            this.CreateDirectories(tables, categoriesList);
;
            for (int i = 0; i < tables.Count; i++)
            {
                HtmlAgilityPack.HtmlNodeCollection tmp = tables[i].SelectNodes("//td[@class='dataportal']");
                HtmlAgilityPack.HtmlNodeCollection price = tables[i].SelectNodes("//td");

                for (int k = 0; k < tmp.Count; k++)
                {
                    headingsList.Add(tmp[k].InnerText);
                    if (price[23 + (k * 8)].InnerText != "CURRENT")
                    {
                        pricesList.Add(price[23 + (k * 8)].InnerText);
                    }
                }
                int lim = tmp.Count-2;
                for (int k = 0; k < lim; k++)
                {
                    if (price[23 + (k * 8)].InnerText != "CURRENT"){
                        pricesList.Add(price[23 + (k * 8)].InnerText);
                    }else{
                        lim++;
                    }
                }
            }

            int j = 0;
            for (int i = 0; i < categoriesList.Count-2; i++)
            {
                XmlDocument tmpXml = new XmlDocument();
                XmlElement root = tmpXml.CreateElement("xml");

                //counter
                var cnt = tables[i].SelectNodes(".//tr").Count-2;
                //counter

                for (int k = 0; k < cnt; k++)
                {
                    XmlElement elem0 = tmpXml.CreateElement("Scripts");
                    XmlElement elem1 = tmpXml.CreateElement("Script");
                    XmlElement elem2 = tmpXml.CreateElement("Price");
                    
                    elem1.InnerText = headingsList[j].ToString();
                    elem2.InnerText = pricesList[j].ToString();

                    elem0.AppendChild(elem1);
                    elem0.AppendChild(elem2);
 
                    root.AppendChild(elem0);
                    j++;
                }
                tmpXml.AppendChild(root);
                
                int fileCount = (from file in Directory.EnumerateFiles(@path + categoriesList[i].ToString(), "*.xml", SearchOption.AllDirectories)
                               select file).Count();
                tmpXml.Save( @path + categoriesList[i].ToString() + "/" + categoriesList[i].ToString().ToLower() + " " + fileCount.ToString() + ".xml" );
            }
        }
        Console.WriteLine("Directories and files created successfully!");
    }

    static public void Main(String[] args)
    {
        HtmlFileManipulation tmp = new HtmlFileManipulation();
        tmp.ManipulateContent();
    }
}