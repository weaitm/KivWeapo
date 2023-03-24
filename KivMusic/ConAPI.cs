using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;

namespace KivMusic
{
    public class ConAPI
    {
        ObservableCollection<ConnectionAPI> connections = new ObservableCollection<ConnectionAPI>();
        string path;
        public class ConnectionAPI
        {
            public string ApiAddress { get; set; }
        }

        public void GetDocPath(string path)
        {
            this.path = path;
        }

        public void CreateConJson(TextBox txtApiAddress)
        {
            connections.Add(new ConnectionAPI { ApiAddress = txtApiAddress.Text});
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            using (StreamWriter file = File.CreateText(path + "\\ConnectionAPI.json"))
            using (JsonWriter writer = new JsonTextWriter(file))
            {
                serializer.Serialize(writer, connections);
            }
        }

        public string docPath()
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(docPath, "KivMusic");
            return path;
        }

        public string ReadConJson()
        {
            var apiCon = JsonConvert.DeserializeObject<ConnectionAPI>(File.ReadAllText(docPath() + "\\ConnectionAPI.json").ToString().Replace('[', '\0').Replace(']', '\0'));
            return $@"{apiCon.ApiAddress}";
        }

    }
}
