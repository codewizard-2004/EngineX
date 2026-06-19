using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    private const float PanelWidth = 260f;
    private const float PanelHeight = 118f;

    private Lander lander;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI fuelText;
    private RectTransform fuelFillRectTransform;
    private Image fuelFillImage;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (FindFirstObjectByType<GameHUD>() != null) return;
        if (FindFirstObjectByType<Lander>() == null) return;

        GameObject hudObject = new GameObject("Game HUD", typeof(RectTransform));
        hudObject.AddComponent<GameHUD>();
    }

    private void Awake()
    {
        lander = FindFirstObjectByType<Lander>();
        BuildHUD();
    }

    private void OnEnable()
    {
        if (lander == null) return;

        lander.OnFuelChanged += Lander_OnFuelChanged;
        lander.OnScoreChanged += Lander_OnScoreChanged;
    }

    private void Start()
    {
        UpdateFuel();
        UpdateScore();
    }

    private void OnDisable()
    {
        if (lander == null) return;

        lander.OnFuelChanged -= Lander_OnFuelChanged;
        lander.OnScoreChanged -= Lander_OnScoreChanged;
    }

    private void Lander_OnFuelChanged(object sender, EventArgs e)
    {
        UpdateFuel();
    }

    private void Lander_OnScoreChanged(object sender, EventArgs e)
    {
        UpdateScore();
    }

    private void BuildHUD()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();

        RectTransform canvasRectTransform = gameObject.GetComponent<RectTransform>();
        GameObject panel = CreateUIObject("Scoreboard", canvasRectTransform);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.55f);

        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
        panelRectTransform.anchorMin = new Vector2(0f, 1f);
        panelRectTransform.anchorMax = new Vector2(0f, 1f);
        panelRectTransform.pivot = new Vector2(0f, 1f);
        panelRectTransform.anchoredPosition = new Vector2(16f, -16f);
        panelRectTransform.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        scoreText = CreateText("Score Text", panelRectTransform, new Vector2(14f, -12f), new Vector2(PanelWidth - 28f, 36f), 24f);
        fuelText = CreateText("Fuel Text", panelRectTransform, new Vector2(14f, -50f), new Vector2(PanelWidth - 28f, 28f), 20f);

        GameObject fuelBar = CreateUIObject("Fuel Bar", panelRectTransform);
        Image fuelBarImage = fuelBar.AddComponent<Image>();
        fuelBarImage.color = new Color(1f, 1f, 1f, 0.25f);
        RectTransform fuelBarRectTransform = fuelBar.GetComponent<RectTransform>();
        fuelBarRectTransform.anchorMin = new Vector2(0f, 1f);
        fuelBarRectTransform.anchorMax = new Vector2(0f, 1f);
        fuelBarRectTransform.pivot = new Vector2(0f, 1f);
        fuelBarRectTransform.anchoredPosition = new Vector2(14f, -82f);
        fuelBarRectTransform.sizeDelta = new Vector2(PanelWidth - 28f, 18f);

        GameObject fuelFill = CreateUIObject("Fuel Fill", fuelBarRectTransform);
        fuelFillImage = fuelFill.AddComponent<Image>();
        fuelFillImage.color = new Color(0.15f, 0.9f, 0.35f, 0.95f);
        fuelFillRectTransform = fuelFill.GetComponent<RectTransform>();
        fuelFillRectTransform.anchorMin = new Vector2(0f, 0f);
        fuelFillRectTransform.anchorMax = new Vector2(0f, 1f);
        fuelFillRectTransform.pivot = new Vector2(0f, 0.5f);
        fuelFillRectTransform.anchoredPosition = Vector2.zero;
        fuelFillRectTransform.sizeDelta = new Vector2(PanelWidth - 28f, 0f);
    }

    private TextMeshProUGUI CreateText(string objectName, Transform parent, Vector2 anchoredPosition, Vector2 size, float fontSize)
    {
        GameObject textObject = CreateUIObject(objectName, parent);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.color = Color.white;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Left;
        text.enableWordWrapping = false;

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        return text;
    }

    private GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject uiObject = new GameObject(objectName, typeof(RectTransform));
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    private void UpdateFuel()
    {
        if (lander == null || fuelText == null || fuelFillRectTransform == null) return;

        float fuelNormalized = Mathf.Clamp01(lander.GetFuelNormalized());
        float maxFillWidth = PanelWidth - 28f;
        fuelFillRectTransform.sizeDelta = new Vector2(maxFillWidth * fuelNormalized, 0f);
        fuelText.text = $"Fuel {Mathf.CeilToInt(lander.GetFuelAmount())}/{Mathf.CeilToInt(lander.GetMaxFuelAmount())}";

        if (fuelFillImage != null)
        {
            fuelFillImage.color = fuelNormalized > 0.25f
                ? new Color(0.15f, 0.9f, 0.35f, 0.95f)
                : new Color(1f, 0.25f, 0.15f, 0.95f);
        }
    }

    private void UpdateScore()
    {
        if (lander == null || scoreText == null) return;

        scoreText.text = $"Coins {lander.GetCoinScore()}   Score {lander.GetTotalScore()}";
    }
}
