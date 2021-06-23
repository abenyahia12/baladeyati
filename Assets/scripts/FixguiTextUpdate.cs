using UnityEngine;
using System.Collections;
using ArabicSupport;
using TMPro;
public class FixguiTextUpdate : MonoBehaviour
{
    public TMP_InputField tMP_InputField;

    public void OnChangeUpdate()
    {
        Debug.Log(tMP_InputField.text);
        FixArabicc(tMP_InputField);
    }
    public void FixArabicc(TMP_InputField tMP_InputField)
    {
        tMP_InputField.text = ArabicFixer.Fix(tMP_InputField.text, false, false);
        tMP_InputField.textComponent.text = ArabicFixer.Fix(tMP_InputField.textComponent.text, false, false);
    }
}
