using ArabicSupport;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FixSearchText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI copiedText;
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        text.text = copiedText.text;
        text.text = ArabicFixer.Fix(text.text, false, false);
    }
}
