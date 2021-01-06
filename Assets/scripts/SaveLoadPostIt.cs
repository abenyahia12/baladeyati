using Paroxe.PdfRenderer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadPostIt : MonoBehaviour {


    public static void Save(List<PDFViewerHelper.RectPostIt> list, string fileLocation)
    {
        fileLocation = fileLocation.Replace('/', '-');

        

        if (fileLocation[fileLocation.Length - 4] == '.')
            fileLocation = fileLocation.Substring(0, fileLocation.Length - 4);

        string filePath = Application.persistentDataPath + "/" + fileLocation + ".postit";

        if (
            (list == null) ||
            ((list != null) && (list.Count == 0))
            )
        {
            if (File.Exists(filePath) == true)
            {
                File.Delete(filePath);
            }
        }
        else
        {
            var file = File.Open(filePath, FileMode.Create, FileAccess.Write);

            var writer = new StreamWriter(file);

            writer.WriteLine("items:" + list.Count.ToString());

            for (int i = 0; i < list.Count; i++)
            {
                writer.WriteLine("Pos:" + list[i].pos.x.ToString() + "," + list[i].pos.y.ToString());
                writer.WriteLine("Size:" + list[i].size.x.ToString() + "," + list[i].size.y.ToString());
                writer.WriteLine("page:" + list[i].page.ToString());
                writer.WriteLine("Collapsed:" + list[i].collapsed.ToString());
                writer.WriteLine("Text:" + list[i].text);
                writer.WriteLine("Color:" + list[i].color.r.ToString() + "," + list[i].color.g.ToString() + "," + list[i].color.b.ToString() + "," + list[i].color.a.ToString());
                writer.WriteLine("ColorIndex:"+list[i].indexColor.ToString());
                writer.WriteLine("Id:"+list[i].id.ToString());
                //writer.WriteLine("Zoom:" + list[i].zoomWhenCreated.ToString());
            }

            writer.Close();
        }
    }

    public static List<PDFViewerHelper.RectPostIt> Load(string fileLocation)
    {
        fileLocation = fileLocation.Replace('/', '-');

        if (fileLocation[fileLocation.Length - 4] == '.')
            fileLocation = fileLocation.Substring(0, fileLocation.Length - 4);

        List<PDFViewerHelper.RectPostIt> newList = new List<PDFViewerHelper.RectPostIt>();
        newList.Clear();

        if (File.Exists(Application.persistentDataPath + "/" + fileLocation + ".postit") == true)
        {
            try
            {
                var file = File.Open(Application.persistentDataPath + "/" + fileLocation + ".postit", FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(file);
                string str = reader.ReadLine();
                string[] str2 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                int count = int.Parse(str2[1]);

                string[] str3;
                string[] str4;
                for (int i = 0; i < count; i++)
                {

                    PDFViewerHelper.RectPostIt newItem = new PDFViewerHelper.RectPostIt();

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    str4 = str3[1].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.pos = new Vector2(float.Parse(str4[0]), float.Parse(str4[1]));//pos

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    str4 = str3[1].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.size = new Vector2(float.Parse(str4[0]), float.Parse(str4[1]));//size

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.page = int.Parse(str3[1]);//page

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.collapsed = bool.Parse(str3[1]);//collapsed

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    if (str3.Length == 2)
                        newItem.text = str3[1];//text
                    else
                        newItem.text = "";

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    str4 = str3[1].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.color = new Color(float.Parse(str4[0]), float.Parse(str4[1]), float.Parse(str4[2]), float.Parse(str4[3]));//color

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.indexColor = int.Parse(str3[1]);//index color


                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.id = int.Parse(str3[1]);//id

                    //str = reader.ReadLine();
                    //str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    //newItem.zoomWhenCreated = float.Parse(str3[1]);//id
                    //newItem.backupZoomWhenCreated = newItem.zoomWhenCreated;

                    newList.Add(newItem);

                }

                reader.Close();
            }
            catch (System.Exception)
            {

            }
        }

        return newList;
    }



}
