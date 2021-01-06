using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadFreehand : MonoBehaviour
{

    public static List<freeHandControl.freeHandItems> Load(string fileLocation)
    {

        List<freeHandControl.freeHandItems> newList = new List<freeHandControl.freeHandItems>();
        newList.Clear();

        if (File.Exists(Application.persistentDataPath + "/" + fileLocation + ".freehand") == true)
        {
            try
            {
                var file = File.Open(Application.persistentDataPath + "/" + fileLocation + ".freehand", FileMode.Open, FileAccess.Read);
                var reader = new StreamReader(file);
                string str = reader.ReadLine();
                string[] str2 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                int count = int.Parse(str2[1]);

                string[] str3;
                for (int i = 0; i < count; i++)
                {
                    freeHandControl.freeHandItems newItem = new freeHandControl.freeHandItems();

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.page = int.Parse(str3[1]);//page

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.color = int.Parse(str3[1]);//color

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.xMin = float.Parse(str3[1]);//xMin

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.xMax = float.Parse(str3[1]);//xMax

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.yMin = float.Parse(str3[1]);//xMin

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    newItem.yMax = float.Parse(str3[1]);//yMax

                    str = reader.ReadLine();
                    str3 = str.Split(":".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                    int k = int.Parse(str3[1]);

                    newItem.listOfPoints = new List<Vector2>();
                    for (int j=0;j<k;j++)
                    {
                        str = reader.ReadLine();
                        str3 = str.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
                        Vector2 newPoint = new Vector2(float.Parse(str3[0]), float.Parse(str3[1]));//pos
                        newItem.listOfPoints.Add(newPoint);
                    }

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

    public static void Save(List<freeHandControl.freeHandItems> list, string fileLocation)
    {
        fileLocation = fileLocation.Replace('/', '-');

        if (fileLocation[fileLocation.Length - 4] == '.')
            fileLocation = fileLocation.Substring(0, fileLocation.Length - 4);

        string filePath = Application.persistentDataPath + "/" + fileLocation + ".freehand";

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
                writer.WriteLine("page:" + list[i].page.ToString());
                writer.WriteLine("color:" + list[i].color.ToString());
                writer.WriteLine("xMin:" + list[i].xMin.ToString());
                writer.WriteLine("xMax:" + list[i].xMax.ToString());
                writer.WriteLine("yMin:" + list[i].yMin.ToString());
                writer.WriteLine("yMax:" + list[i].yMax.ToString());
                writer.WriteLine("points:" + list[i].listOfPoints.Count.ToString());
                for (int j=0;j< list[i].listOfPoints.Count;j++)
                {
                    writer.WriteLine(list[i].listOfPoints[j].x.ToString() + "," + list[i].listOfPoints[j].y.ToString());
                }
            }
            
            writer.Close();
        }
    }
}
