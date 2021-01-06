using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IconLookUp", menuName = "PDF/ButtonLookUp")]
public class IconLookup : ScriptableObject
{
    [System.Serializable]
    public struct NamedImage
    {
        public string Iconname;
        public Sprite Icon;
    }
    public List<NamedImage> Pictures;
    public Dictionary<string, Sprite> PicturesDictionnary = new Dictionary<string, Sprite>();
     public void CopyListInDictionnary()
    {
        foreach (NamedImage item in Pictures)
        {

            PicturesDictionnary.Add(item.Iconname, item.Icon);
        }
    }
    public Sprite GetIconFromName(string Iconname)
    {
        if (PicturesDictionnary.Count <=1)
        {
            CopyListInDictionnary();
        }
        Sprite temporarySprite;
        if (PicturesDictionnary.ContainsKey(Iconname))
        {
            PicturesDictionnary.TryGetValue(Iconname, out temporarySprite);
        }
        else
        {
            PicturesDictionnary.TryGetValue("Unknown", out temporarySprite);
        }
        return temporarySprite;
    }
}