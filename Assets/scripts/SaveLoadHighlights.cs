using Paroxe.PdfRenderer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadHighlights : MonoBehaviour {



    public static void Save(List<PDFViewerHelper.RectHightlights> list, string fileLocation)
    {
        fileLocation = fileLocation.Replace('/', '-');

        if (fileLocation[fileLocation.Length-4]=='.')
            fileLocation = fileLocation.Substring(0, fileLocation.Length - 4);

        string filePath = Application.persistentDataPath + "/" + fileLocation + ".highlight";

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
                writer.WriteLine("Rect:" + list[i].rect.xMin.ToString() + "," + list[i].rect.yMin.ToString() + "," + list[i].rect.width.ToString() + "," + list[i].rect.height.ToString());
                writer.WriteLine("Inner Rects:"+list[i].innerRects.Count.ToString());
                for (int j = 0;j < list[i].innerRects.Count;j++)
                {
                    Rect r = list[i].innerRects[j];
                    writer.WriteLine(r.xMin.ToString() + "," + r.yMin.ToString() + "," + r.width.ToString() + "," + r.height.ToString());
                }
                writer.WriteLine("Color:" + list[i].color.r.ToString() + "," + list[i].color.g.ToString() + "," + list[i].color.b.ToString() + "," + list[i].color.a.ToString());
                writer.WriteLine("page:" + list[i].page.ToString());
            }

            writer.Close();
        }
    }

    public static List<PDFViewerHelper.RectHightlights> Load(string fileLocation)
    {
        fileLocation = fileLocation.Replace('/', '-');

        if (fileLocation[fileLocation.Length - 4] == '.')
            fileLocation = fileLocation.Substring(0, fileLocation.Length - 4);

        List<PDFViewerHelper.RectHightlights> newList = new List<PDFViewerHelper.RectHightlights>();
        newList.Clear();

        if (File.Exists(Application.persistentDataPath + "/" + fileLocation + ".highlight") == true)
        { 
            try
            {
                var file = File.Open(Application.persistentDataPath + "/" + fileLocation + ".highlight", FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(file);
                string str = reader.ReadLine();
                string[] str2 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                int count = int.Parse(str2[1]);

                string[] str3;
                string[] str4;
                for (int i = 0; i < count; i++)
                {

                    PDFViewerHelper.RectHightlights newItem = new PDFViewerHelper.RectHightlights();

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    str4 = str3[1].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.rect = new Rect(float.Parse(str4[0]), float.Parse(str4[1]), float.Parse(str4[2]), float.Parse(str4[3]));//rect

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                    List<Rect> innerRects = new List<Rect>();
                    innerRects.Clear();

                    int x = int.Parse(str3[1]);
                    for (int j = 0; j < x; j++)
                    {
                        str = reader.ReadLine();
                        str4 = str.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                        innerRects.Add(new Rect(float.Parse(str4[0]), float.Parse(str4[1]), float.Parse(str4[2]), float.Parse(str4[3])));
                    }
                    newItem.innerRects = innerRects;

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    str4 = str3[1].Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.color = new Color(float.Parse(str4[0]), float.Parse(str4[1]), float.Parse(str4[2]), float.Parse(str4[3]));//color

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.page = int.Parse(str3[1]);//page

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
