using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;

public class MessageBulletSpawner : MonoBehaviour
{
    static readonly Color SignTextColor = new Color(0.12f, 0.07f, 0.03f, 1f);

    [Header("Canvas Prefab Settings")]
    public RectTransform messagePrefab;
    public RectTransform canvasParent;

    [Header("Custom Messages")]
    [TextArea(2, 4)]
    public string[] messages =
    {
        "Assignment deadline updated",
        "New Canvas announcement",
        "Your score has been released",
        "Group member sent 12 messages",
        "Meeting starts in 5 minutes",
        "Unread email from tutor",
        "System notification",
        "Low battery",
        "New discussion reply",
        "Reminder: submit reflection",
        "Course update available",
        "Booking confirmed",
        "Payment required",
        "New feedback received",
        "You have 39 unread messages",
        "Calendar conflict detected",
        "Warning: task overdue",
        "New Teams message",
        "Instagram notification",
        "Amazon delivery update",
        "Grade changed",
        "Lecture recording available",
        "Quiz closes tonight",
        "Urgent: action required"
    };

    [Header("Random Materials")]
    public Material[] prefabMaterials;

    [Header("Canvas Spawn Settings")]
    public float spawnInterval = 0.22f;
    [FormerlySerializedAs("verticalRange")]
    public float horizontalRange = 520f;
    [FormerlySerializedAs("horizontalPadding")]
    public float verticalPadding = 180f;

    [Header("Canvas Movement")]
    public float minSpeed = 210f;
    public float maxSpeed = 360f;
    public float lifeTime = 9f;

    [Header("Auto Size")]
    public float minWidth = 320f;
    public float maxWidth = 820f;
    public float height = 96f;
    public float widthPerCharacter = 18f;
    public float horizontalTextPadding = 58f;

    private float timer;
    TMP_FontAsset signFont;

    void Awake()
    {
        signFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/Electronic Highway Sign SDF");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnMessage();
            timer = 0f;
        }
    }

    void SpawnMessage()
    {
        if (messagePrefab == null || canvasParent == null) return;

        RectTransform obj = Instantiate(messagePrefab, canvasParent);
        obj.gameObject.SetActive(true);

        string randomMessage = "New notification";
        if (messages != null && messages.Length > 0)
        {
            randomMessage = messages[Random.Range(0, messages.Length)];
        }

        TMP_Text text = obj.GetComponentInChildren<TMP_Text>(true);
        if (text != null)
        {
            text.text = randomMessage;
            if (signFont != null)
            {
                text.font = signFont;
            }

            text.textWrappingMode = TextWrappingModes.Normal;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 34f;
            text.fontStyle = FontStyles.Bold;
            text.color = SignTextColor;
            text.enableKerning = false;
            text.characterSpacing = 1.5f;
        }

        ResizeMessageObject(obj, text, randomMessage);
        ApplyRandomMaterial(obj.gameObject);

        float canvasWidth = canvasParent.rect.width;
        float canvasHeight = canvasParent.rect.height;
        float randomX = Random.Range(-Mathf.Min(horizontalRange, canvasWidth * 0.42f), Mathf.Min(horizontalRange, canvasWidth * 0.42f));
        float startY = canvasHeight * 0.5f + verticalPadding;
        float endY = -canvasHeight * 0.5f - verticalPadding;

        obj.anchorMin = new Vector2(0.5f, 0.5f);
        obj.anchorMax = new Vector2(0.5f, 0.5f);
        obj.pivot = new Vector2(0.5f, 0.5f);
        obj.anchoredPosition = new Vector2(randomX, startY);
        obj.localRotation = Quaternion.identity;
        obj.localScale = Vector3.one;

        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        StartCoroutine(MoveMessageDownScreen(obj, randomX, startY, endY, randomSpeed));
    }

    IEnumerator MoveMessageDownScreen(RectTransform obj, float x, float startY, float endY, float speed)
    {
        float timerInside = 0f;
        float distance = Mathf.Abs(endY - startY);
        float duration = distance / Mathf.Max(speed, 1f);

        while (obj != null && timerInside < duration && timerInside < lifeTime)
        {
            timerInside += Time.deltaTime;
            float t = Mathf.Clamp01(timerInside / duration);
            float y = Mathf.Lerp(startY, endY, t);
            obj.anchoredPosition = new Vector2(x, y);
            yield return null;
        }

        if (obj != null)
        {
            Destroy(obj.gameObject);
        }
    }

    void ResizeMessageObject(RectTransform obj, TMP_Text text, string message)
    {
        float targetWidth = minWidth;

        if (!string.IsNullOrEmpty(message))
        {
            targetWidth = Mathf.Clamp(message.Length * widthPerCharacter + horizontalTextPadding * 2f, minWidth, maxWidth);
        }

        obj.sizeDelta = new Vector2(targetWidth, height);

        if (text != null)
        {
            RectTransform textRect = text.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(horizontalTextPadding * 0.5f, 0f);
                textRect.offsetMax = new Vector2(-horizontalTextPadding * 0.5f, 0f);
            }
        }
    }

    void ApplyRandomMaterial(GameObject obj)
    {
        if (prefabMaterials == null || prefabMaterials.Length == 0) return;

        Material randomMaterial = prefabMaterials[Random.Range(0, prefabMaterials.Length)];
        if (randomMaterial == null) return;

        Image[] images = obj.GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            image.material = randomMaterial;
        }
    }
}
