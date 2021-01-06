using UnityEngine;
using System.Collections;
using ArabicSupport;
using TMPro;
using UnityEngine.UI;
public class FixGUITextCS : MonoBehaviour {
	TextMeshProUGUI text;
	void Start () {
		text = gameObject.GetComponent<TextMeshProUGUI>();
		text.text= ArabicFixer.Fix(text.text, false, false);
	}
}
