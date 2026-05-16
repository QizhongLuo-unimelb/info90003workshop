using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;

public class MessageBullet : MonoBehaviour
{
    static readonly Color SignTextColor = new Color(0.12f, 0.07f, 0.03f, 1f);

    [Header("UI References")]
    public RectTransform rectTransform;
    [FormerlySerializedAs("text")]
    public TMP_Text messageText;
    public Image backgroundImage;

    [Header("Auto Size")]
    public float minWidth = 320f;
    public float maxWidth = 820f;
    public float height = 96f;
    public float widthPerCharacter = 18f;
    public float horizontalTextPadding = 58f;

    TMP_FontAsset signFont;

    void Reset()
    {
        rectTransform = GetComponent<RectTransform>();
        messageText = GetComponentInChildren<TMP_Text>(true);
        backgroundImage = GetComponent<Image>();
    }

    void Awake()
    {
        signFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Electronic Highway Sign SDF");

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        if (messageText == null)
        {
            messageText = GetComponentInChildren<TMP_Text>(true);
        }

        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
    }

    public void Setup(string message, Material material)
    {
        if (messageText != null)
        {
            messageText.text = message;
            ApplyTextStyle(messageText);
            messageText.textWrappingMode = TextWrappingModes.Normal;
            messageText.alignment = TextAlignmentOptions.Center;
        }

        if (backgroundImage != null && material != null)
        {
            backgroundImage.material = material;
        }

        ResizeToMessage(message);
    }

    public void ResizeToMessage(string message)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        float targetWidth = minWidth;

        if (!string.IsNullOrEmpty(message))
        {
            targetWidth = Mathf.Clamp(message.Length * widthPerCharacter + horizontalTextPadding * 2f, minWidth, maxWidth);
        }

        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(targetWidth, height);
        }

        if (messageText != null)
        {
            ApplyTextStyle(messageText);

            RectTransform textRect = messageText.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(horizontalTextPadding * 0.5f, 0f);
                textRect.offsetMax = new Vector2(-horizontalTextPadding * 0.5f, 0f);
            }
        }
    }

    void ApplyTextStyle(TMP_Text text)
    {
        if (signFont != null)
        {
            text.font = signFont;
        }

        text.fontSize = 34f;
        text.fontStyle = FontStyles.Bold;
        text.color = SignTextColor;
        text.enableKerning = false;
        text.characterSpacing = 1.5f;
        text.raycastTarget = false;
    }
}
