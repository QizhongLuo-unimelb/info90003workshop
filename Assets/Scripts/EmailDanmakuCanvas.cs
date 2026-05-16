using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EmailDanmakuCanvas : MonoBehaviour
{
    static readonly Color BoardColor = new Color(0.55f, 0.33f, 0.16f, 0.94f);
    static readonly Color BoardEdgeColor = new Color(0.24f, 0.13f, 0.06f, 1f);
    static readonly Color BoardShadowColor = new Color(0f, 0f, 0f, 0.45f);
    static readonly Color GrassColor = new Color(0.22f, 0.56f, 0.16f, 0.58f);
    static readonly Color DirtColor = new Color(0.36f, 0.22f, 0.11f, 0.58f);
    static readonly Color StoneColor = new Color(0.32f, 0.34f, 0.34f, 0.44f);

    [Header("Icon")]
    public Sprite emailIcon;

    [Header("Scene Background Object")]
    public Transform emailRoomSphere;
    public Renderer emailRoomSphereRenderer;

    [Header("Editable Gmail Background")]
    public RawImage existingBackgroundImage;
    public Texture2D gmailBackground;
    public bool stretchBackgroundToScreen = true;
    public Vector2 backgroundAnchoredPosition = Vector2.zero;
    public Vector2 backgroundSize = new Vector2(1600f, 900f);
    public Color backgroundTint = new Color(1f, 1f, 1f, 0.72f);

    [Header("Spawn")]
    public float initialSpawnInterval = 2.2f;
    public float finalSpawnInterval = 0.12f;
    public int laneCount = 8;
    public int warmStartCount = 3;
    public int initialMaxMessagesOnScreen = 8;
    public int finalMaxMessagesOnScreen = 95;
    public float horizontalPadding = 420f;

    [Header("Movement")]
    public float minSpeed = 35f;
    public float maxSpeed = 75f;
    public float speedRampMultiplier = 6.2f;

    [Header("Card Size")]
    public float minWidth = 460f;
    public float maxWidth = 920f;
    public float cardHeight = 88f;
    public float widthPerCharacter = 15f;

    [Header("Colors")]
    public Color backgroundColor = BoardColor;
    public Color urgentBackgroundColor = new Color(0.45f, 0.18f, 0.12f, 0.96f);
    public Color textColor = new Color(0.12f, 0.07f, 0.03f, 1f);
    public Color mutedTextColor = new Color(0.25f, 0.15f, 0.08f, 1f);

    [Header("Auto Exit")]
    public float autoExitSeconds = 30f;
    public string fallbackReturnSceneName = "Main game";

    readonly List<RectTransform> activeMessages = new List<RectTransform>();
    RectTransform canvasRect;
    RectTransform messageLayer;
    RectTransform cardTemplate;
    TMP_FontAsset signFont;
    TextMeshProUGUI missedCountText;
    TextMeshProUGUI activeCountText;
    TextMeshProUGUI pressureText;
    TextMeshProUGUI countdownText;
    float timer;
    float elapsedTime;
    int nextLane;
    int missedEmails = 127;
    int spawnedMessages;
    bool hasExited;

    readonly string[] senders =
    {
        "Canvas", "Tutor", "Subject Admin", "Library", "Group Project",
        "Student Portal", "Unread Email", "Calendar", "LMS", "Assessment"
    };

    readonly string[] subjects =
    {
        "Assignment deadline updated",
        "New feedback has been released",
        "Meeting starts in 5 minutes",
        "Your draft needs revision",
        "Unread reply from your tutor",
        "Submission reminder: due tonight",
        "Group member mentioned you",
        "Course announcement posted",
        "Extension request received",
        "Quiz closes at midnight",
        "Action required before 5pm",
        "New rubric comment available",
        "Inbox storage almost full",
        "Booking confirmation attached",
        "Important update for tomorrow",
        "Reading list changed",
        "You have 39 unread messages",
        "Calendar conflict detected"
    };

    void Start()
    {
        signFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Electronic Highway Sign SDF");
        BuildCanvas();
        BuildTemplate();
        StartCoroutine(WarmStart());
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= GetCurrentSpawnInterval())
        {
            timer = 0f;
            SpawnMessage(false);
        }

        UpdateStatsBoard();

        if (!hasExited && autoExitSeconds > 0f && elapsedTime >= autoExitSeconds)
        {
            hasExited = true;
            ExitEmailScene();
        }
    }

    void BuildCanvas()
    {
        GameObject canvasObject = new GameObject("Email Danmaku Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasRect = canvasObject.GetComponent<RectTransform>();

        BuildMinecraftBackdrop(canvasRect);

        GameObject layerObject = new GameObject("Email Message Layer", typeof(RectTransform));
        messageLayer = layerObject.GetComponent<RectTransform>();
        messageLayer.SetParent(canvasRect, false);
        messageLayer.anchorMin = Vector2.zero;
        messageLayer.anchorMax = Vector2.one;
        messageLayer.offsetMin = Vector2.zero;
        messageLayer.offsetMax = Vector2.zero;

        BuildStatsBoard(canvasRect);
    }

    void BuildMinecraftBackdrop(RectTransform parent)
    {
        RawImage gmailImage = existingBackgroundImage;
        if (gmailImage == null)
        {
            GameObject gmailObject = new GameObject("Editable Gmail Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
            gmailImage = gmailObject.GetComponent<RawImage>();
        }

        RectTransform gmailRect = gmailImage.GetComponent<RectTransform>();
        if (gmailRect.parent != parent)
        {
            gmailRect.SetParent(parent, false);
        }

        gmailRect.SetAsFirstSibling();

        if (stretchBackgroundToScreen)
        {
            gmailRect.anchorMin = Vector2.zero;
            gmailRect.anchorMax = Vector2.one;
            gmailRect.offsetMin = Vector2.zero;
            gmailRect.offsetMax = Vector2.zero;
        }
        else
        {
            gmailRect.anchorMin = new Vector2(0.5f, 0.5f);
            gmailRect.anchorMax = new Vector2(0.5f, 0.5f);
            gmailRect.pivot = new Vector2(0.5f, 0.5f);
            gmailRect.anchoredPosition = backgroundAnchoredPosition;
            gmailRect.sizeDelta = backgroundSize;
        }

        gmailImage.texture = gmailBackground;
        gmailImage.color = backgroundTint;
        gmailImage.raycastTarget = false;

        GameObject sky = new GameObject("MC Pixel Sky Backdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform skyRect = sky.GetComponent<RectTransform>();
        skyRect.SetParent(parent, false);
        skyRect.anchorMin = Vector2.zero;
        skyRect.anchorMax = Vector2.one;
        skyRect.offsetMin = Vector2.zero;
        skyRect.offsetMax = Vector2.zero;
        Image skyImage = sky.GetComponent<Image>();
        skyImage.color = new Color(0.38f, 0.62f, 0.86f, 0.08f);
        skyImage.raycastTarget = false;

        BuildBlockBand(parent, "Top Dirt Blocks", new Vector2(0f, 1f), new Vector2(1f, 1f), -40f, 9, 92f, DirtColor);
        BuildBlockBand(parent, "Top Grass Blocks", new Vector2(0f, 1f), new Vector2(1f, 1f), -8f, 9, 32f, GrassColor);
        BuildBlockBand(parent, "Bottom Stone Blocks", Vector2.zero, new Vector2(1f, 0f), 42f, 10, 96f, StoneColor);
        BuildBlockBand(parent, "Bottom Grass Blocks", Vector2.zero, new Vector2(1f, 0f), 126f, 10, 34f, GrassColor);
    }

    void BuildBlockBand(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, float y, int count, float blockHeight, Color color)
    {
        GameObject band = new GameObject(name, typeof(RectTransform));
        RectTransform bandRect = band.GetComponent<RectTransform>();
        bandRect.SetParent(parent, false);
        bandRect.anchorMin = anchorMin;
        bandRect.anchorMax = anchorMax;
        bandRect.pivot = new Vector2(0.5f, anchorMin.y);
        bandRect.anchoredPosition = new Vector2(0f, y);
        bandRect.sizeDelta = new Vector2(0f, blockHeight);

        for (int i = 0; i < count; i++)
        {
            GameObject block = new GameObject("Block", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            RectTransform blockRect = block.GetComponent<RectTransform>();
            blockRect.SetParent(bandRect, false);
            blockRect.anchorMin = new Vector2(i / (float)count, 0f);
            blockRect.anchorMax = new Vector2((i + 1f) / count, 1f);
            blockRect.offsetMin = new Vector2(1f, 1f);
            blockRect.offsetMax = new Vector2(-1f, -1f);
            Image blockImage = block.GetComponent<Image>();
            float shade = i % 2 == 0 ? 1f : 0.82f;
            blockImage.color = new Color(color.r * shade, color.g * shade, color.b * shade, color.a);
            blockImage.raycastTarget = false;
        }
    }

    void BuildStatsBoard(RectTransform parent)
    {
        GameObject statusObject = new GameObject("MC Email Stats Board", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform statusRect = statusObject.GetComponent<RectTransform>();
        statusRect.SetParent(parent, false);
        statusRect.anchorMin = new Vector2(1f, 1f);
        statusRect.anchorMax = new Vector2(1f, 1f);
        statusRect.pivot = new Vector2(1f, 1f);
        statusRect.anchoredPosition = new Vector2(-32f, -32f);
        statusRect.sizeDelta = new Vector2(430f, 220f);

        Image statusImage = statusObject.GetComponent<Image>();
        statusImage.color = BoardColor;
        statusImage.raycastTarget = false;

        Outline outline = statusObject.AddComponent<Outline>();
        outline.effectColor = BoardEdgeColor;
        outline.effectDistance = new Vector2(8f, -8f);

        Shadow shadow = statusObject.AddComponent<Shadow>();
        shadow.effectColor = BoardShadowColor;
        shadow.effectDistance = new Vector2(0f, -8f);

        GameObject titleObject = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.SetParent(statusRect, false);
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -18f);
        titleRect.sizeDelta = new Vector2(-36f, 50f);
        TextMeshProUGUI titleText = titleObject.GetComponent<TextMeshProUGUI>();
        ConfigureText(titleText, 30f, FontStyles.Bold, textColor, TextAlignmentOptions.Center, TextWrappingModes.NoWrap);
        titleText.text = "The Email You Missed.";

        GameObject countRowObject = new GameObject("Unread Count Row", typeof(RectTransform));
        RectTransform countRowRect = countRowObject.GetComponent<RectTransform>();
        countRowRect.SetParent(statusRect, false);
        countRowRect.anchorMin = new Vector2(0f, 1f);
        countRowRect.anchorMax = new Vector2(1f, 1f);
        countRowRect.pivot = new Vector2(0.5f, 1f);
        countRowRect.anchoredPosition = new Vector2(0f, -76f);
        countRowRect.sizeDelta = new Vector2(-44f, 42f);

        GameObject countIconObject = new GameObject("Unread Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform countIconRect = countIconObject.GetComponent<RectTransform>();
        countIconRect.SetParent(countRowRect, false);
        countIconRect.anchorMin = new Vector2(0f, 0.5f);
        countIconRect.anchorMax = new Vector2(0f, 0.5f);
        countIconRect.pivot = new Vector2(0f, 0.5f);
        countIconRect.anchoredPosition = new Vector2(0f, 0f);
        countIconRect.sizeDelta = new Vector2(34f, 34f);
        Image countIcon = countIconObject.GetComponent<Image>();
        countIcon.sprite = emailIcon;
        countIcon.color = new Color(0.92f, 0.76f, 0.45f, 1f);
        countIcon.preserveAspect = true;
        countIcon.raycastTarget = false;

        GameObject countObject = new GameObject("Unread Count", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform countRect = countObject.GetComponent<RectTransform>();
        countRect.SetParent(countRowRect, false);
        countRect.anchorMin = new Vector2(0f, 0f);
        countRect.anchorMax = new Vector2(1f, 1f);
        countRect.offsetMin = new Vector2(46f, 0f);
        countRect.offsetMax = Vector2.zero;
        missedCountText = countObject.GetComponent<TextMeshProUGUI>();
        ConfigureText(missedCountText, 26f, FontStyles.Bold, textColor, TextAlignmentOptions.Left, TextWrappingModes.NoWrap);

        activeCountText = BuildBoardLine(statusRect, "Active Count", -122f, 22f);
        pressureText = BuildBoardLine(statusRect, "Pressure", -158f, 20f);
        countdownText = BuildBoardLine(statusRect, "Countdown", -188f, 20f);
        UpdateStatsBoard();
    }

    TextMeshProUGUI BuildBoardLine(RectTransform parent, string name, float y, float fontSize)
    {
        GameObject lineObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform lineRect = lineObject.GetComponent<RectTransform>();
        lineRect.SetParent(parent, false);
        lineRect.anchorMin = new Vector2(0f, 1f);
        lineRect.anchorMax = new Vector2(1f, 1f);
        lineRect.pivot = new Vector2(0.5f, 1f);
        lineRect.anchoredPosition = new Vector2(0f, y);
        lineRect.sizeDelta = new Vector2(-44f, 30f);
        TextMeshProUGUI lineText = lineObject.GetComponent<TextMeshProUGUI>();
        ConfigureText(lineText, fontSize, FontStyles.Bold, mutedTextColor, TextAlignmentOptions.Left, TextWrappingModes.NoWrap);
        return lineText;
    }

    void BuildTemplate()
    {
        GameObject cardObject = new GameObject("Email Message Template", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        cardTemplate = cardObject.GetComponent<RectTransform>();
        cardTemplate.SetParent(messageLayer, false);
        cardTemplate.anchorMin = new Vector2(0.5f, 0.5f);
        cardTemplate.anchorMax = new Vector2(0.5f, 0.5f);
        cardTemplate.pivot = new Vector2(0.5f, 0.5f);
        cardTemplate.sizeDelta = new Vector2(minWidth, cardHeight);

        Image background = cardObject.GetComponent<Image>();
        background.color = backgroundColor;
        background.raycastTarget = false;
        Outline cardOutline = cardObject.AddComponent<Outline>();
        cardOutline.effectColor = BoardEdgeColor;
        cardOutline.effectDistance = new Vector2(5f, -5f);
        Shadow cardShadow = cardObject.AddComponent<Shadow>();
        cardShadow.effectColor = BoardShadowColor;
        cardShadow.effectDistance = new Vector2(0f, -5f);

        GameObject iconObject = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.SetParent(cardTemplate, false);
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.anchoredPosition = new Vector2(22f, 0f);
        iconRect.sizeDelta = new Vector2(42f, 42f);
        Image iconImage = iconObject.GetComponent<Image>();
        iconImage.sprite = emailIcon;
        iconImage.color = new Color(0.92f, 0.76f, 0.45f, 1f);
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;

        GameObject senderObject = new GameObject("Sender", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform senderRect = senderObject.GetComponent<RectTransform>();
        senderRect.SetParent(cardTemplate, false);
        senderRect.anchorMin = new Vector2(0f, 1f);
        senderRect.anchorMax = new Vector2(1f, 1f);
        senderRect.pivot = new Vector2(0f, 1f);
        senderRect.offsetMin = new Vector2(82f, -38f);
        senderRect.offsetMax = new Vector2(-124f, -10f);
        TextMeshProUGUI senderText = senderObject.GetComponent<TextMeshProUGUI>();
        ConfigureText(senderText, 21f, FontStyles.Bold, textColor, TextAlignmentOptions.Left, TextWrappingModes.NoWrap);

        GameObject timeObject = new GameObject("Time", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform timeRect = timeObject.GetComponent<RectTransform>();
        timeRect.SetParent(cardTemplate, false);
        timeRect.anchorMin = new Vector2(1f, 1f);
        timeRect.anchorMax = new Vector2(1f, 1f);
        timeRect.pivot = new Vector2(1f, 1f);
        timeRect.anchoredPosition = new Vector2(-20f, -12f);
        timeRect.sizeDelta = new Vector2(92f, 24f);
        TextMeshProUGUI timeText = timeObject.GetComponent<TextMeshProUGUI>();
        ConfigureText(timeText, 17f, FontStyles.Bold, mutedTextColor, TextAlignmentOptions.Right, TextWrappingModes.NoWrap);

        GameObject subjectObject = new GameObject("Subject", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        RectTransform subjectRect = subjectObject.GetComponent<RectTransform>();
        subjectRect.SetParent(cardTemplate, false);
        subjectRect.anchorMin = new Vector2(0f, 0f);
        subjectRect.anchorMax = new Vector2(1f, 1f);
        subjectRect.offsetMin = new Vector2(82f, 10f);
        subjectRect.offsetMax = new Vector2(-26f, -40f);
        TextMeshProUGUI subjectText = subjectObject.GetComponent<TextMeshProUGUI>();
        ConfigureText(subjectText, 27f, FontStyles.Bold, textColor, TextAlignmentOptions.Left, TextWrappingModes.NoWrap);

        cardObject.SetActive(false);
    }

    IEnumerator WarmStart()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();

        for (int i = 0; i < warmStartCount; i++)
        {
            SpawnMessage(true);
        }
    }

    void SpawnMessage(bool warmStart)
    {
        if (cardTemplate == null || messageLayer == null || canvasRect == null)
        {
            return;
        }

        CleanupMissingMessages();
        if (activeMessages.Count >= GetCurrentMaxMessages())
        {
            return;
        }

        string sender = senders[Random.Range(0, senders.Length)];
        string subject = subjects[Random.Range(0, subjects.Length)];
        bool urgent = subject.Contains("required") || subject.Contains("deadline") || subject.Contains("tonight") || subject.Contains("midnight");
        spawnedMessages++;
        missedEmails++;

        RectTransform card = Instantiate(cardTemplate, messageLayer);
        card.gameObject.SetActive(true);
        activeMessages.Add(card);

        float targetWidth = Mathf.Clamp(subject.Length * widthPerCharacter + 210f, minWidth, maxWidth);
        card.sizeDelta = new Vector2(targetWidth, cardHeight);

        Image background = card.GetComponent<Image>();
        if (background != null)
        {
            background.color = urgent ? urgentBackgroundColor : backgroundColor;
        }

        TextMeshProUGUI[] texts = card.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI text in texts)
        {
            if (text.name == "Sender")
            {
                text.text = sender;
            }
            else if (text.name == "Time")
            {
                text.text = Random.Range(0, 4) == 0 ? "now" : Random.Range(1, 59) + "m ago";
            }
            else if (text.name == "Subject")
            {
                text.text = subject;
            }
        }

        float width = canvasRect.rect.width;
        float height = canvasRect.rect.height;
        float laneHeight = Mathf.Max(cardHeight + 10f, (height - 120f) / Mathf.Max(1, laneCount));
        int lane = warmStart ? Random.Range(0, laneCount) : nextLane;
        nextLane = (nextLane + 1) % Mathf.Max(1, laneCount);

        float top = height * 0.5f - 70f;
        float y = top - lane * laneHeight - Random.Range(0f, Mathf.Max(0f, laneHeight - cardHeight));
        float startX = width * 0.5f + horizontalPadding;
        float endX = -width * 0.5f - horizontalPadding;

        card.anchoredPosition = new Vector2(warmStart ? Random.Range(endX, startX) : startX, y);
        card.localRotation = Quaternion.identity;
        card.localScale = Vector3.one * Random.Range(0.92f, 1.08f);

        float baseSpeed = Random.Range(minSpeed, maxSpeed);
        StartCoroutine(MoveAcross(card, endX, baseSpeed));
    }

    IEnumerator MoveAcross(RectTransform card, float endX, float baseSpeed)
    {
        while (card != null && card.anchoredPosition.x > endX)
        {
            card.anchoredPosition += Vector2.left * (baseSpeed * GetSpeedRamp() * Time.deltaTime);
            yield return null;
        }

        if (card != null)
        {
            activeMessages.Remove(card);
            Destroy(card.gameObject);
        }
    }

    void CleanupMissingMessages()
    {
        for (int i = activeMessages.Count - 1; i >= 0; i--)
        {
            if (activeMessages[i] == null)
            {
                activeMessages.RemoveAt(i);
            }
        }
    }

    void ConfigureText(TMP_Text text, float fontSize, FontStyles style, Color color, TextAlignmentOptions alignment, TextWrappingModes wrapping)
    {
        if (signFont != null)
        {
            text.font = signFont;
        }

        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = wrapping;
        text.enableKerning = false;
        text.characterSpacing = 1.5f;
        text.margin = Vector4.zero;
        text.raycastTarget = false;
    }

    void UpdateStatsBoard()
    {
        if (missedCountText != null)
        {
            missedCountText.text = "MISSED: " + missedEmails;
        }

        if (activeCountText != null)
        {
            activeCountText.text = "ON SCREEN: " + activeMessages.Count;
        }

        if (pressureText != null)
        {
            pressureText.text = "DISTRACTIONS: " + spawnedMessages;
        }

        if (countdownText != null)
        {
            int secondsLeft = Mathf.Max(0, Mathf.CeilToInt(autoExitSeconds - elapsedTime));
            countdownText.text = "EXIT IN: " + secondsLeft + "s";
        }
    }

    float GetSpeedRamp()
    {
        if (autoExitSeconds <= 0f)
        {
            return 1f;
        }

        float t = Mathf.Clamp01(elapsedTime / autoExitSeconds);
        return Mathf.Lerp(1f, speedRampMultiplier, t * t);
    }

    float GetCurrentSpawnInterval()
    {
        if (autoExitSeconds <= 0f)
        {
            return initialSpawnInterval;
        }

        float t = Mathf.Clamp01(elapsedTime / autoExitSeconds);
        return Mathf.Lerp(initialSpawnInterval, finalSpawnInterval, t * t);
    }

    int GetCurrentMaxMessages()
    {
        if (autoExitSeconds <= 0f)
        {
            return initialMaxMessagesOnScreen;
        }

        float t = Mathf.Clamp01(elapsedTime / autoExitSeconds);
        return Mathf.RoundToInt(Mathf.Lerp(initialMaxMessagesOnScreen, finalMaxMessagesOnScreen, t * t));
    }

    void ExitEmailScene()
    {
        string returnSceneName = PlayerPrefs.GetString("ReturnSceneName", "");
        if (string.IsNullOrEmpty(returnSceneName))
        {
            returnSceneName = fallbackReturnSceneName;
        }

        if (!string.IsNullOrEmpty(returnSceneName))
        {
            SceneManager.LoadScene(returnSceneName);
        }
    }
}
