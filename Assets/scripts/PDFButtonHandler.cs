using System.Collections.Generic;
using Paroxe.PdfRenderer;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class PDFButtonHandler : MonoBehaviour
{
	public GameObject m_PdfViewerContainer;
	public PDFViewer m_PDFViewer;
	public GameObject m_GridContainer;
	public GameObject m_ContentPage;
	public GameObject m_ButtonPrefab;
	public GameObject m_LoadingImageGeneral;
	public ErrorHandler m_ErrorHandler;
	public DownloadModalController m_DownloadHandler;
	public Transform m_VerticalLayoutOfGrids;
	public Dictionary<string, Transform> m_TableOfCourses = new Dictionary<string, Transform>();
	public List<PDFInfo> m_PDFInfos = new List<PDFInfo>();
	public List<Transform> m_Buttons;
	public List<GridLayoutGroup> CourseGrids;
	public RectTransform GridOfGrids;
	private int m_HeightMultiplier;
	public float m_TextHolderSize;
	public float m_OriginalGridofGridHeight;
	public float m_SizeToAdd = 10f;
	public float m_TextSizeToAdd = 4;
	public bool m_Plus = false;
	public bool m_Minus = false;
	public ScrollRect m_PanelScrollRect;
	public List<CourseGridScript> m_CourseGrids;
	public float m_GridTopBarheight;
	public float m_GridBottomBarheight;
	public Canvas m_ContentPageCanvas;
	public MatchCasePortraitToLandscape m_MatchCasePortraitLandscape;
	public float m_TextHolderPlusCellSize = 313;
	public float m_MaxCellSize = 280;
	public float m_MinCellSize = 146;
	public List<GridInfo> m_GridInfoList;

	public List<PDFButtonView> m_PDFButtonViewList;
	public float InitialCellSize = 146f;
	public float InitialFontSize = 0;
	public float CurrentCellSize = 0f;
	public float m_MaxTextSize;
	public void Start()
	{
		m_TextSizeToAdd = (m_SizeToAdd / m_MinCellSize) * m_TextHolderSize;
	}
	public void GenerateContent()
	{
		GenerateButtons();
		CreateGridsAndAssignButtons();
		ResizeGrid();
		AssignButtons();
	}

	private void CreateGridsAndAssignButtons()
	{
		RectTransform layoutOfGridsRectTransform = m_VerticalLayoutOfGrids.GetComponent<RectTransform>();
		Debug.Assert(layoutOfGridsRectTransform != null, "layoutOfGridsRectTransform != null");

		foreach (Transform item in m_Buttons)
		{
			PDFButtonView TemporaryPDFButton = item.GetComponent<PDFButtonView>();
			string TemporaryCourseName = TemporaryPDFButton.m_DM.CourseName;

			//Is that Button Downloaded? Activate the download Icon
			TemporaryPDFButton.m_DownloadStatusIcon.enabled = !TemporaryPDFButton.m_DM.HasDownloaded();

			//does the pdf course exist in our dictionnary? if not create its grids and set it in the dictionnary
			if (!m_TableOfCourses.ContainsKey(TemporaryCourseName))
			{
				m_HeightMultiplier++;

				layoutOfGridsRectTransform.SetHeight(m_HeightMultiplier * m_TextHolderPlusCellSize);
				GameObject temporaryGrid = Instantiate(m_GridContainer, m_VerticalLayoutOfGrids, false);

				GridInfo TemporaryGridInfo = temporaryGrid.GetComponent<GridInfo>();
				TemporaryGridInfo.m_CourseName.text = TemporaryCourseName;
				m_GridInfoList.Add(TemporaryGridInfo);
				CourseGrids.Add(TemporaryGridInfo.m_ButtonsGrid);
				m_TableOfCourses.Add(TemporaryCourseName, temporaryGrid.transform);
			}

			//assign the button to its grid
			Transform TemporaryParent;

			if (m_TableOfCourses.TryGetValue(TemporaryCourseName, out TemporaryParent))
			{
				item.SetParent(TemporaryParent.GetComponent<GridInfo>().m_CourseGrid);
			}
			else
			{
				UnityEngine.Debug.Log("Temporary parent doesn't exist");
			}

			item.localScale = Vector3.one;
		}
	}

	private void ResizeGrid()
	{
		m_OriginalGridofGridHeight = GridOfGrids.GetHeight();

		foreach (GridLayoutGroup item in CourseGrids)
		{
			m_CourseGrids.Add(item.GetComponent<CourseGridScript>());
		}
	}

	public void AddButton(PDFInfo info)
	{
		m_PDFInfos.Add(info);
	}

	private void GenerateButtons()
	{
		foreach (PDFInfo pdfInfo in m_PDFInfos)
		{
			GameObject temporaryButton = Instantiate(m_ButtonPrefab, transform, false);
			PDFButtonView TemporaryPdfButtonScript = temporaryButton.GetComponent<PDFButtonView>();

			PDFModel model = TemporaryPdfButtonScript.m_DM;
			model.Info = pdfInfo;

			PDFController pdfButtonController = TemporaryPdfButtonScript.m_DC;
			pdfButtonController.m_PDFViewerGo = m_PdfViewerContainer;
			pdfButtonController.m_ErrorHandler = m_ErrorHandler;
			pdfButtonController.m_DownloadModalController = m_DownloadHandler;
			pdfButtonController.m_grid = m_ContentPage;
			pdfButtonController.LoadingImage = m_LoadingImageGeneral;
			pdfButtonController.m_buttonHandler = this;
			pdfButtonController.m_PDFviewer = m_PDFViewer;

			TemporaryPdfButtonScript.m_GridOfGrids = GridOfGrids;
			// TODO: The following can be moved to an init
			TemporaryPdfButtonScript.m_ButtonImage.sprite = pdfInfo.Icon;
			temporaryButton.name = pdfInfo.FileName;
			TemporaryPdfButtonScript.m_UIfilename.text = pdfInfo.DisplayName;

			m_Buttons.Add(temporaryButton.transform);
		}
	}

	public void ResizeContentScenePlus(float value)
	{
		float SizetoBeAddedToGrid = 0;
		float percentage = 0;

		foreach (CourseGridScript item in m_CourseGrids)
		{
			if (item.m_GridLayoutGroup.cellSize.x + value >= m_MaxCellSize)
			{

				return;
			}
			else
			{
				//first increase cellsize
				item.m_GridLayoutGroup.cellSize += new Vector2(value, value);
				CurrentCellSize = item.m_GridLayoutGroup.cellSize.x;
				//get percentage of how much the cell size increased
				percentage = item.m_GridLayoutGroup.cellSize.x / (item.m_GridLayoutGroup.cellSize.x - value);
			
				m_TextHolderSize += m_TextSizeToAdd / CourseGrids.Count;
				item.m_GridLayoutGroup.spacing = new Vector2(20, m_TextHolderSize);
				foreach (Transform child in item.transform)
				{
					foreach (Transform childr in child)
					{
						childr.localScale *= percentage;
					}
				}
				if (CurrentCellSize < m_MaxTextSize)
				{
					foreach (PDFButtonView buttonView in m_PDFButtonViewList)
					{

						buttonView.m_UIfilename.transform.parent.localScale = Vector3.one;
						buttonView.m_UIfilename.transform.localScale = Vector3.one;
						RectTransform TextRect = buttonView.m_UIfilename.GetComponent<RectTransform>();
						
						TextRect.SetHeight(TextRect.GetHeight() * percentage);
						TextRect.SetWidth(TextRect.GetWidth() * percentage);
						float TempPercentage = 0;
						TempPercentage = CurrentCellSize / InitialCellSize;
						buttonView.m_UIfilename.fontSize = Mathf.CeilToInt((InitialFontSize) * TempPercentage);

					}
				}
				else
				{
					foreach (PDFButtonView buttonView in m_PDFButtonViewList)
					{

						buttonView.m_UIfilename.transform.parent.localScale = Vector3.one;
						buttonView.m_UIfilename.transform.localScale = Vector3.one;
					}
				}
				float buttonsContainerWidth = item.m_GridTransform.GetWidth() - item.m_GridLayoutGroup.padding.right - item.m_GridLayoutGroup.padding.left;
				bool complete = false;
				int nButtonsPerLine = 0;
				float Containerfiller = 0;
				while (complete == false)
				{

					if ((Containerfiller + item.m_GridLayoutGroup.cellSize.x + item.m_GridLayoutGroup.spacing.x) <= buttonsContainerWidth)
					{
						Containerfiller += item.m_GridLayoutGroup.cellSize.x + item.m_GridLayoutGroup.spacing.x;
						nButtonsPerLine++;

					}
					else if ((Containerfiller + item.m_GridLayoutGroup.cellSize.x) <= buttonsContainerWidth)
					{
						Containerfiller += item.m_GridLayoutGroup.cellSize.x;
						nButtonsPerLine++;

					}
					else
					{
						complete = true;
					}
				}
				float nExtraLines = (float)(item.transform.childCount) / (float)(nButtonsPerLine);

				if (nExtraLines > 1)
				{
					if (Mathf.Floor(nExtraLines) < nExtraLines)
					{
						float newPrefferedHeight = ((item.m_GridLayoutGroup.cellSize.y + item.m_GridLayoutGroup.spacing.y) * (Mathf.Floor(nExtraLines) + 1)) + item.m_GridLayoutGroup.padding.top + item.m_GridLayoutGroup.padding.bottom + m_GridTopBarheight + m_GridBottomBarheight;
						item.m_GridLayoutElement.preferredHeight = newPrefferedHeight;
						SizetoBeAddedToGrid += newPrefferedHeight;

					}
					else if (Mathf.Floor(nExtraLines) == nExtraLines)
					{
						float newPrefferedHeight = ((item.m_GridLayoutGroup.cellSize.y + item.m_GridLayoutGroup.spacing.y) * Mathf.Floor(nExtraLines))  + m_GridTopBarheight + m_GridBottomBarheight;
						item.m_GridLayoutElement.preferredHeight = newPrefferedHeight;
						SizetoBeAddedToGrid += newPrefferedHeight;
					}
				}
				else
				{
					item.m_GridLayoutElement.preferredHeight += value + ((item.m_GridLayoutGroup.spacing.y - 20) * (percentage - 1));
					SizetoBeAddedToGrid += value + item.m_GridLayoutGroup.spacing.y - 20;
				}


			}

		}
		foreach (GridInfo item in m_GridInfoList)
		{
			float TempPercentage = 0;
			TempPercentage = CurrentCellSize / InitialCellSize;

			item.m_CourseName.fontSize = Mathf.CeilToInt((InitialFontSize) * TempPercentage);
		}
		GridOfGrids.SetHeight(m_OriginalGridofGridHeight + SizetoBeAddedToGrid);
		GridOfGrids.gameObject.SetActive(false);
		GridOfGrids.gameObject.SetActive(true);

	}
	public void ResizeContentSceneMinus(float value)
	{
		float percentage = 0;
		float SizetoBeAddedToGrid = 0;
		foreach (CourseGridScript item in m_CourseGrids)
		{
			if (item.m_GridLayoutGroup.cellSize.x <= m_MinCellSize)
			{

				return;
			}
			else
			{
				//first increase cellsize
				item.m_GridLayoutGroup.cellSize -= new Vector2(value, value);
				CurrentCellSize = item.m_GridLayoutGroup.cellSize.x;
				//get percentage of how much the cell size increased
				percentage = item.m_GridLayoutGroup.cellSize.x / (item.m_GridLayoutGroup.cellSize.x + value);
			
				m_TextHolderSize -= m_TextSizeToAdd / CourseGrids.Count;
				item.m_GridLayoutGroup.spacing = new Vector2(20, m_TextHolderSize);
				foreach (Transform child in item.transform)
				{
					foreach (Transform childr in child)
					{
						if (childr.name != "Spacing")
							childr.localScale *= percentage;
					}
				}
				if (CurrentCellSize < m_MaxTextSize-m_SizeToAdd)
				{
					foreach (PDFButtonView buttonView in m_PDFButtonViewList)
					{

						buttonView.m_UIfilename.transform.parent.localScale = Vector3.one;
						buttonView.m_UIfilename.transform.localScale = Vector3.one;
						RectTransform TextRect = buttonView.m_UIfilename.GetComponent<RectTransform>();
						TextRect.SetHeight(TextRect.GetHeight() * percentage);
						TextRect.SetWidth(TextRect.GetWidth() * percentage);
						float TempPercentage = 0;
						TempPercentage = CurrentCellSize / InitialCellSize;
						buttonView.m_UIfilename.fontSize = Mathf.CeilToInt((InitialFontSize) * TempPercentage);

					}
				}
				else
				{
					foreach (PDFButtonView buttonView in m_PDFButtonViewList)
					{

						buttonView.m_UIfilename.transform.parent.localScale = Vector3.one;
						buttonView.m_UIfilename.transform.localScale = Vector3.one;
					}
				}
				float buttonsContainerWidth = item.m_GridTransform.GetWidth() - item.m_GridLayoutGroup.padding.right - item.m_GridLayoutGroup.padding.left;
				bool complete = false;
				int nButtonsPerLine = 0;
				float Containerfiller = 0;
				while (complete == false)
				{

					if ((Containerfiller + item.m_GridLayoutGroup.cellSize.x + item.m_GridLayoutGroup.spacing.x) <= buttonsContainerWidth)
					{
						Containerfiller += item.m_GridLayoutGroup.cellSize.x + item.m_GridLayoutGroup.spacing.x;
						nButtonsPerLine++;

					}
					else if ((Containerfiller + item.m_GridLayoutGroup.cellSize.x) <= buttonsContainerWidth)
					{
						Containerfiller += item.m_GridLayoutGroup.cellSize.x;
						nButtonsPerLine++;

					}
					else
					{
						complete = true;
					}
				}

				float nExtraLines = (float)(item.transform.childCount) / (float)(nButtonsPerLine);
				if (nExtraLines > 1)
				{
					if (Mathf.Floor(nExtraLines) < nExtraLines)
					{
						float newPrefferedHeight = ((item.m_GridLayoutGroup.cellSize.y + item.m_GridLayoutGroup.spacing.y) * (Mathf.Floor(nExtraLines) + 1)) + item.m_GridLayoutGroup.padding.top + item.m_GridLayoutGroup.padding.bottom + m_GridTopBarheight + m_GridBottomBarheight;
						item.m_GridLayoutElement.preferredHeight = newPrefferedHeight;
						SizetoBeAddedToGrid += newPrefferedHeight;

					}
					else if (Mathf.Floor(nExtraLines) == nExtraLines)
					{
						float newPrefferedHeight = ((item.m_GridLayoutGroup.cellSize.y + item.m_GridLayoutGroup.spacing.y) * Mathf.Floor(nExtraLines)) + item.m_GridLayoutGroup.padding.top + item.m_GridLayoutGroup.padding.bottom + m_GridTopBarheight + m_GridBottomBarheight;
						item.m_GridLayoutElement.preferredHeight = newPrefferedHeight;
						SizetoBeAddedToGrid += newPrefferedHeight;
					}

				}
				else
				{

					item.m_GridLayoutElement.preferredHeight = item.m_GridLayoutGroup.cellSize.y + m_TextHolderSize + m_GridTopBarheight + m_GridBottomBarheight;

					SizetoBeAddedToGrid += (-value);
				}
			}
		}
		foreach (GridInfo item in m_GridInfoList)
		{
			float TempPercentage = 0;
			TempPercentage = CurrentCellSize / InitialCellSize;

			item.m_CourseName.fontSize = Mathf.CeilToInt((InitialFontSize) * TempPercentage);
		}
		GridOfGrids.SetHeight(m_OriginalGridofGridHeight + SizetoBeAddedToGrid);
		GridOfGrids.gameObject.SetActive(false);
		GridOfGrids.gameObject.SetActive(true);

	}


	private void HandleTouchInput()
	{
		if ((m_ContentPageCanvas.enabled != true) || (Input.touchCount != 2))
		{
			m_PanelScrollRect.enabled = true;
			return;
		}

		m_PanelScrollRect.enabled = false;

		// Store both touches.
		Touch touchZero = Input.GetTouch(0);
		Touch touchOne = Input.GetTouch(1);

		// Find the position in the previous frame of each touch.
		Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
		Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

		// Find the magnitude of the vector (the distance) between the touches in each frame.
		float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
		float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

		// Find the difference in the distances between each frame.
		float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

		if (deltaMagnitudeDiff < 0)
		{
			ResizeContentScenePlus(m_SizeToAdd);
		}
		else if (deltaMagnitudeDiff > 0)
		{
			ResizeContentSceneMinus(m_SizeToAdd);
		}
	}

	private void HandleDesktopInput()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			m_PanelScrollRect.enabled = false;
		}

		if (m_ContentPageCanvas.enabled && (Input.GetAxis("Mouse ScrollWheel") > 0) && (Input.GetKey(KeyCode.Space)))
		{
			ResizeContentScenePlus(m_SizeToAdd);
		}
		else if (m_ContentPageCanvas.enabled && (Input.GetAxis("Mouse ScrollWheel") < 0) && (Input.GetKey(KeyCode.Space)))
		{
			ResizeContentSceneMinus(m_SizeToAdd);
		}

		if (m_Plus)
		{
			m_Plus = false;
			ResizeContentScenePlus(m_SizeToAdd);
		}

		if (m_Minus)
		{
			m_Minus = false;
			ResizeContentSceneMinus(m_SizeToAdd);
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			m_PanelScrollRect.enabled = true;
		}
	}

	private void Update()
	{
#if UNITY_IOS || UNITY_ANDROID
		HandleTouchInput();
#endif

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
		HandleDesktopInput();
#endif
	}
	public void AssignButtons()
	{
		foreach (Transform item in m_Buttons)
		{

			m_PDFButtonViewList.Add(item.GetComponent<PDFButtonView>());
		}
	}

}
