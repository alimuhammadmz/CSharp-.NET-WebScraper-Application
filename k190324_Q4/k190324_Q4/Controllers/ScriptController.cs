using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using k190324_Q4.Models;
using Nancy.Json;
using System.IO;
using System.Collections;
using System.Data;
using static System.Net.WebRequestMethods;
using System.Xml;
using Microsoft.Extensions.Options;

namespace k190324_Q4.Controllers
{
    [ApiController]
    [Route("api/ScriptController")]
    public class ScriptController : Controller
    {
        string path;
        public ScriptController(IOptions<MyConfigModel> pth)
        {
            path = pth.Value.path;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            string[] dirs = Directory.GetDirectories(@path, "*", SearchOption.TopDirectoryOnly);
            List<CategoryModel> tmp = new List<CategoryModel>();
                
            for(int i = 0; i < dirs.Length; i++)
            {
                CategoryModel cat = new CategoryModel();
                cat.num = i + 1;
                cat.name = dirs[i];
                cat.name = cat.name.Replace(path, "").First().ToString().ToUpper() + cat.name.Replace(path, "").Substring(1).ToString().ToLower();
                tmp.Add(cat);
            }

            ScriptModel script = new ScriptModel();
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return Ok(serializer.Serialize(tmp));
        }
        

        [HttpPost]
        [Route("specific")]
        public async Task<IActionResult> GetSpecificScript([FromBody] CategoryModel category)
        {
            string Category = category.name;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<ScriptModel> dt = new List<ScriptModel>();

            if (Category != null && Category != "All" && Category != "all" && Category != "")
            {
                int fileCount = (from file in Directory.EnumerateFiles(@path + Category.ToString().ToUpper(), "*.xml", SearchOption.AllDirectories)
                                 select file).Count() - 1;

                XmlReader reader;
                try
                {
                    reader = XmlReader.Create(@path + "/" + Category.ToUpper() + "/" + Category.ToLower() + " " + fileCount.ToString() + ".xml");
                }
                catch (Exception err)
                {
                    return Ok(serializer.Serialize(dt));
                }
                
                XmlDocument xml = new XmlDocument();
                xml.Load(@path + "/" + Category.ToString().ToUpper() + "/" + Category.ToString().ToLower() + " " + fileCount.ToString() + ".xml");
                XmlNodeList xnList = xml.SelectNodes("/xml/Scripts");
                int count = 0;
                foreach (XmlNode xn in xnList)
                    count = count + 1;

                int i = 0;
                while (i++ != count)
                {
                    ScriptModel model = new ScriptModel();
                    reader.ReadToFollowing("Script");
                    model.Script = reader.ReadElementContentAsString();

                    reader.ReadToFollowing("Price");
                    model.Price = reader.ReadElementContentAsString();

                    dt.Add(model);
                }
            }
            else
            {
                //parsing all the directories' .xml files (error)
                string[] dirs = Directory.GetDirectories(@path, "*", SearchOption.TopDirectoryOnly);
            
                for (int cnt = 0; cnt < dirs.Length; cnt++)
                {
                    String subDir = dirs[cnt].Replace(path, "");
                    int fileCount = (from file in Directory.EnumerateFiles(@path + subDir.ToUpper(), "*.xml", SearchOption.AllDirectories)
                                     select file).Count() - 1;

                    XmlReader reader;
                    XmlDocument xml = new XmlDocument();

                    try
                    {
                        reader = XmlReader.Create(@path + "/" + subDir.ToUpper() + "/" + subDir.ToLower() + " " + fileCount.ToString() + ".xml");
                        xml.Load(@path + "/" + subDir.ToUpper() + "/" + subDir.ToLower() + " " + fileCount.ToString() + ".xml");
                    }
                    catch (Exception err)
                    {
                        return Ok(serializer.Serialize(dt));
                    }

                    XmlNodeList xnList = xml.SelectNodes("/xml/Scripts");
                    int count = 0;
                    foreach (XmlNode xn in xnList)
                        count = count + 1;

                    int i = 0;
                    while (i++ != count)
                    {
                        ScriptModel model = new ScriptModel();
                        reader.ReadToFollowing("Script");
                        model.Script = reader.ReadElementContentAsString();

                        reader.ReadToFollowing("Price");
                        model.Price = reader.ReadElementContentAsString();

                        dt.Add(model);
                    }
                }
            }
            return Ok(serializer.Serialize(dt));

        }
    }
}
