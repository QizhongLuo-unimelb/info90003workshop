using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUIController : MonoBehaviour
{
    const float PanelWidth = 1020f;
    const float PanelHeight = 230f;
    const float IconSize = 70f;
    const float LeftPadding = 42f;
    const float RightPadding = 42f;
    const float TextLeft = 140f;
    const float TopPadding = 30f;
    const float TitleHeight = 40f;
    const float MessageTop = 84f;
    const float MessageHeight = 116f;
    const float TimeWidth = 96f;
    const float RestingY = -36f;
    const float HiddenY = 260f;
    const float ScrollDuration = 0.85f;

    static readonly Color BoardColor = new Color(0.55f, 0.33f, 0.16f, 0.94f);
    static readonly Color BoardEdgeColor = new Color(0.24f, 0.13f, 0.06f, 1f);
    static readonly Color TextColor = new Color(0.12f, 0.07f, 0.03f, 1f);
    static readonly Color MutedTextColor = new Color(0.24f, 0.14f, 0.07f, 1f);

    [Header("UI References")]
    public RectTransform notificationPanel;
    public Image iconImage;
    public TMP_Text titleText;
    public TMP_Text messageText;
    public TMP_Text timeText;

    [Header("Icons")]
    public Sprite emailIcon;
    public Sprite gameIcon;
    public Sprite shoppingIcon;
    public Sprite defaultIcon;

    TMP_FontAsset signFont;
    Coroutine scrollRoutine;
    CanvasGroup panelGroup;

    void Awake()
    {
        signFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Electronic Highway Sign SDF");
        ApplyLayout();
    }

    void OnValidate()
    {
        ApplyLayout();
    }

    public void Show(StepNode node)
    {
        if (node == null)
        {
            return;
        }

        NotificationKind kind = ResolveKind(node);
        string title = string.IsNullOrWhiteSpace(node.notificationTitle)
            ? GetDefaultTitle(kind)
            : node.notificationTitle;

        ShowNotification(title, CleanMessage(node.nodeMessage), kind);
    }

    public void ShowSystemMessage(string title, string message)
    {
        ShowNotification(title, message, NotificationKind.System);
    }

    void ShowNotification(string title, string message, NotificationKind kind)
    {
        ApplyLayout();

        if (notificationPanel != null)
        {
            notificationPanel.gameObject.SetActive(true);
            if (scrollRoutine != null)
            {
                StopCoroutine(scrollRoutine);
            }

            scrollRoutine = StartCoroutine(ScrollPanelFromTop());
        }

        if (titleText != null)
        {
            titleText.text = title;
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (timeText != null)
        {
            timeText.text = "now";
        }

        if (iconImage != null)
        {
            iconImage.sprite = GetIcon(kind);
            iconImage.enabled = iconImage.sprite != null;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }
    }

    NotificationKind ResolveKind(StepNode node)
    {
        if (node.notificationKind != NotificationKind.Auto)
        {
            return node.notificationKind;
        }

        string source = (node.name + " " + node.nodeMessage).ToLowerInvariant();

        if (source.Contains("email") || source.Contains("gmail"))
        {
            return NotificationKind.Email;
        }

        if (source.Contains("shopping") || source.Contains("shop"))
        {
            return NotificationKind.Shopping;
        }

        if (source.Contains("game") || source.Contains("login") || source.Contains("checked in"))
        {
            return NotificationKind.Game;
        }

        return NotificationKind.System;
    }

    string GetDefaultTitle(NotificationKind kind)
    {
        switch (kind)
        {
            case NotificationKind.Email:
                return "Outlook";
            case NotificationKind.Game:
                return "Game Center";
            case NotificationKind.Shopping:
                return "Shopping";
            default:
                return "Notification";
        }
    }

    string CleanMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return "";
        }

        string trimmed = message.TrimStart();

        if (!trimmed.StartsWith("["))
        {
            return message;
        }

        int endOfCue = trimmed.IndexOf(']');
        if (endOfCue < 0)
        {
            return message;
        }

        return trimmed.Substring(endOfCue + 1).TrimStart('\r', '\n', ' ');
    }

    Sprite GetIcon(NotificationKind kind)
    {
        switch (kind)
        {
            case NotificationKind.Email:
                return emailIcon != null ? emailIcon : defaultIcon;
            case NotificationKind.Game:
                return gameIcon != null ? gameIcon : defaultIcon;
            case NotificationKind.Shopping:
                return shoppingIcon != null ? shoppingIcon : defaultIcon;
            default:
                return defaultIcon;
        }
    }

    void ApplyLayout()
    {
        if (notificationPanel != null)
        {
            notificationPanel.anchorMin = new Vector2(0.5f, 1f);
            notificationPanel.anchorMax = new Vector2(0.5f, 1f);
            notificationPanel.pivot = new Vector2(0.5f, 1f);
            notificationPanel.anchoredPosition = new Vector2(0f, RestingY);
            notificationPanel.sizeDelta = new Vector2(PanelWidth, PanelHeight);
            ApplyBoardStyle();
        }

        RectTransform iconRect = iconImage != null ? iconImage.rectTransform : null;
        if (iconRect != null)
        {
            iconImage.color = new Color(0.9f, 0.78f, 0.5f, 1f);
            iconImage.preserveAspect = true;
            iconRect.anchorMin = new Vector2(0f, 1f);
            iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.pivot = new Vector2(0f, 1f);
            iconRect.anchoredPosition = new Vector2(LeftPadding, -64f);
            iconRect.sizeDelta = new Vector2(IconSize, IconSize);
        }

        RectTransform timeRect = timeText != null ? timeText.rectTransform : null;
        if (timeRect != null)
        {
            timeRect.anchorMin = new Vector2(1f, 1f);
            timeRect.anchorMax = new Vector2(1f, 1f);
            timeRect.pivot = new Vector2(1f, 1f);
            timeRect.anchoredPosition = new Vector2(-RightPadding, -TopPadding);
            timeRect.sizeDelta = new Vector2(TimeWidth, 28f);
        }

        RectTransform titleRect = titleText != null ? titleText.rectTransform : null;
        if (titleRect != null)
        {
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(0f, 1f);
            titleRect.pivot = new Vector2(0f, 1f);
            titleRect.anchoredPosition = new Vector2(TextLeft, -TopPadding);
            titleRect.sizeDelta = new Vector2(PanelWidth - TextLeft - RightPadding - TimeWidth - 12f, TitleHeight);
        }

        RectTransform messageRect = messageText != null ? messageText.rectTransform : null;
        if (messageRect != null)
        {
            messageRect.anchorMin = new Vector2(0f, 1f);
            messageRect.anchorMax = new Vector2(0f, 1f);
            messageRect.pivot = new Vector2(0f, 1f);
            messageRect.anchoredPosition = new Vector2(TextLeft, -MessageTop);
            messageRect.sizeDelta = new Vector2(PanelWidth - TextLeft - RightPadding, MessageHeight);
        }

        ConfigureText(titleText, TextAlignmentOptions.Left, TextWrappingModes.NoWrap, 28f, 0f, TextColor);
        ConfigureText(messageText, TextAlignmentOptions.TopLeft, TextWrappingModes.Normal, 32f, -4f, TextColor);
        ConfigureText(timeText, TextAlignmentOptions.Right, TextWrappingModes.NoWrap, 20f, 0f, MutedTextColor);
    }

    void ConfigureText(TMP_Text text, TextAlignmentOptions alignment, TextWrappingModes wrapping, float fontSize, float lineSpacing, Color color)
    {
        if (text == null)
        {
            return;
        }

        if (signFont != null)
        {
            text.font = signFont;
        }

        text.alignment = alignment;
        text.textWrappingMode = wrapping;
        text.fontSize = fontSize;
        text.fontStyle = FontStyles.Bold;
        text.lineSpacing = lineSpacing;
        text.color = color;
        text.enableKerning = false;
        text.characterSpacing = 1.5f;
        text.margin = Vector4.zero;
        text.raycastTarget = false;
    }

    void ApplyBoardStyle()
    {
        Image panelImage = notificationPanel.GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = BoardColor;
            panelImage.raycastTarget = false;
        }

        Outline outline = notificationPanel.GetComponent<Outline>();
        if (outline == null)
        {
            outline = notificationPanel.gameObject.AddComponent<Outline>();
        }

        outline.effectColor = BoardEdgeColor;
        outline.effectDistance = new Vector2(10f, -10f);

        Shadow shadow = null;
        Shadow[] shadows = notificationPanel.GetComponents<Shadow>();
        foreach (Shadow candidate in shadows)
        {
            if (candidate is Outline)
            {
                continue;
            }

            shadow = candidate;
            break;
        }

        if (shadow == null)
        {
            shadow = notificationPanel.gameObject.AddComponent<Shadow>();
        }

        shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
        shadow.effectDistance = new Vector2(0f, -8f);

        panelGroup = notificationPanel.GetComponent<CanvasGroup>();
        if (panelGroup == null)
        {
            panelGroup = notificationPanel.gameObject.AddComponent<CanvasGroup>();
        }
    }

    IEnumerator ScrollPanelFromTop()
    {
        Vector2 start = new Vector2(0f, HiddenY);
        Vector2 end = new Vector2(0f, RestingY);
        float elapsed = 0f;

        notificationPanel.anchoredPosition = start;
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
        }

        while (elapsed < ScrollDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / ScrollDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            notificationPanel.anchoredPosition = Vector2.LerpUnclamped(start, end, eased);

            if (panelGroup != null)
            {
                panelGroup.alpha = Mathf.SmoothStep(0f, 1f, t);
            }

            yield return null;
        }

        notificationPanel.anchoredPosition = end;
        if (panelGroup != null)
        {
            panelGroup.alpha = 1f;
        }

        scrollRoutine = null;
    }
}
