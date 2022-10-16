using System;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Configuration;

namespace k190324_Q3
{
    public partial class Form1 : Form
    {
        String path;

        public Form1()
        {
            InitializeComponent();
            var appSettings = ConfigurationManager.AppSettings;
            path = appSettings["path"];
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public string[] GetCategories()
        {
            string[] dirs = Directory.GetDirectories(@path, "*", SearchOption.TopDirectoryOnly);
            return dirs;
        }
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Script", typeof(string));
            dt.Columns.Add("Price", typeof(string));

            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem != "All")
            {
                int fileCount = (from file in Directory.EnumerateFiles(@path + comboBox1.SelectedItem.ToString().ToUpper(), "*.xml", SearchOption.AllDirectories)
                                 select file).Count() - 1;

                this.ParseXML(dt, path + "/" + comboBox1.SelectedItem.ToString().ToUpper() + "/" + comboBox1.SelectedItem.ToString().ToLower() + " " + fileCount.ToString() + ".xml");
            }else{
                //parsing all the directories' .xml files 
                string[] subDirs = GetCategories();

                for (int cnt = 0; cnt < subDirs.Length; cnt++)
                {   
                    String subDir = subDirs[cnt].Replace(path, "");
                    this.ParseXML(dt, path + "/" + subDir +"/" + subDir.ToLower() +" 0" + ".xml");
                }
            }
        }

        public int GetCounter(String file){
            XmlDocument xml = new XmlDocument();
            xml.Load(@file);
            XmlNodeList xnList = xml.SelectNodes("/xml/Scripts");
            int count = 0;
            foreach (XmlNode xn in xnList)
                count = count + 1;
            
            return count;
        }

        public void ParseXML(DataTable dt, string file)
        {
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(@file);
            }
            catch (Exception err)
            {
                return;
            }

            int count = GetCounter(@file);
            
            int i = 0;
            while (i++!=count)
            {
                reader.ReadToFollowing("Script");
                String script = reader.ReadElementContentAsString();

                reader.ReadToFollowing("Price");
                String price = reader.ReadElementContentAsString();

                dt.Rows.Add(script, price);
            }

            dataGridView1.DataSource = dt;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] subDirs = GetCategories();
            comboBox1.Items.Add("All");

            for (int i = 0; i < subDirs.Length; i++)
            {
                String subDir = subDirs[i].Replace(path, "").ToLower();
                comboBox1.Items.Add(subDir.First().ToString().ToUpper()+subDir.Substring(1));
            }
        }
    }
}