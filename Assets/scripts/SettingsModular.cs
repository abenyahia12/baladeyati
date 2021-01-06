using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsModular : MonoBehaviour {
	public GridLayoutGroup m_GridLayout;
	public LayoutElement m_LayoutElement;

	// Use this for initialization
	void Start () {
		m_LayoutElement.minHeight = m_GridLayout.cellSize.y * m_GridLayout.transform.childCount;
	}

}
