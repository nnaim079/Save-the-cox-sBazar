using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public int carriedBottles = 0;   
    public int depositedBottles = 0; 
    public int totalScore = 0;       

    private TextMeshProUGUI scoreText;

    void Awake()
    {
        CreateDashboardUI();
    }

    void Start()
    {
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Trash"))
        {
            carriedBottles++;
            UpdateUI();
            Destroy(other.gameObject);
        }

        
        if (other.CompareTag("Dustbin"))
        {
            if (carriedBottles > 0)
            {
                depositedBottles += carriedBottles;
                totalScore += carriedBottles * 10;  
                carriedBottles = 0;                
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            
            scoreText.text = $"Carried: {carriedBottles}   |   Deposited: {depositedBottles}   |   Score: {totalScore}";
        }
    }

    void CreateDashboardUI()
    {
        
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("HUD_Canvas");

            canvas = canvasObj.AddComponent<Canvas>();
            
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        
        GameObject panelObj = new GameObject("ScorePanel");

        panelObj.transform.SetParent(canvas.transform, false);

        Image panelImg = panelObj.AddComponent<Image>();

        panelImg.color = new Color(0f, 0f, 0f, 0.7f); 

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();

        panelRect.sizeDelta = new Vector2(480, 45); 

        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -25); 

        
        GameObject textObj = new GameObject("ScoreText");
        textObj.transform.SetParent(panelObj.transform, false);

        scoreText = textObj.AddComponent<TextMeshProUGUI>();

        scoreText.fontSize = 18; 
        scoreText.color = Color.white;
        scoreText.alignment = TextAlignmentOptions.Center;

        RectTransform textRect = textObj.GetComponent<RectTransform>();

        textRect.sizeDelta = panelRect.sizeDelta;
        textRect.anchoredPosition = Vector2.zero;
    }
}