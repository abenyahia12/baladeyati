using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public static void SaveList(List<string> yourList,string filename)
    {
      
        FileStream fs = new FileStream(filename, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, yourList);
        fs.Close();
    }
    public static List<string> LoadList(string filename)
    {
        using (Stream stream = File.Open(filename, FileMode.Open))
        {
            var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            List<string> items = (List<string>)bformatter.Deserialize(stream);
            return items;
        }
    }

}
