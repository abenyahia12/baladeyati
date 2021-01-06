using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GridInfo : MonoBehaviour
{

	public Text m_CourseName;
	public RectTransform m_CourseGrid;
	public GridLayoutGroup m_ButtonsGrid;
	public LayoutElement m_LE;
	public float m_TextHolderSize;
	public float m_GridTopBarheight;
	public float m_GridBottomBarheight;


	void Start()
	{
		StartCoroutine(DelayedAdaptation());

	}
	void OnRectTransformDimensionsChange()
	{
		AdaptSize();
	}

	void AdaptSize()
	{

		float buttonsContainerWidth = m_CourseGrid.GetWidth() - m_ButtonsGrid.padding.right - m_ButtonsGrid.padding.left;

		bool complete = false;
		int nButtonsPerLine = 0;
		float Containerfiller = 0;
		while (complete == false)
		{

			if ((Containerfiller + m_ButtonsGrid.cellSize.x + m_ButtonsGrid.spacing.x) <= buttonsContainerWidth)
			{
				Containerfiller += m_ButtonsGrid.cellSize.x + m_ButtonsGrid.spacing.x;
				nButtonsPerLine++;

			}
			else if ((Containerfiller + m_ButtonsGrid.cellSize.x) <= buttonsContainerWidth)
			{
				Containerfiller += m_ButtonsGrid.cellSize.x;
				nButtonsPerLine++;

			}
			else
			{
				complete = true;
			}
		}

		float nExtraLines = (float)(m_ButtonsGrid.transform.childCount) / (float)(nButtonsPerLine);

		if (nExtraLines > 1)
		{

			if (Mathf.Floor(nExtraLines) < nExtraLines)
			{
				float newPrefferedHeight = ((m_ButtonsGrid.cellSize.y + m_ButtonsGrid.spacing.y) * (Mathf.Floor(nExtraLines) + 1)) + m_ButtonsGrid.padding.top + m_ButtonsGrid.padding.bottom + m_GridTopBarheight + m_GridBottomBarheight;
				m_LE.preferredHeight = newPrefferedHeight;
			}
			else if (Mathf.Floor(nExtraLines) == nExtraLines)
			{
				float newPrefferedHeight = ((m_ButtonsGrid.cellSize.y + m_ButtonsGrid.spacing.y) * Mathf.Floor(nExtraLines)) + m_ButtonsGrid.padding.top + m_ButtonsGrid.padding.bottom + m_GridTopBarheight + m_GridBottomBarheight;
				m_LE.preferredHeight = newPrefferedHeight;
			}
		}
		else
		{

			m_LE.preferredHeight = m_ButtonsGrid.cellSize.y  + m_TextHolderSize + m_GridTopBarheight + m_GridBottomBarheight;

		}

	}




	IEnumerator DelayedAdaptation()
	{

		yield return new WaitForSeconds(0.1f);
		AdaptSize();
	}

}
