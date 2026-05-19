# Email Distraction Space Unity Guide

## 1. Concept Overview

This scene is designed as a short **email distraction experience**.  
The player does not need to interact with anything. The purpose is to let the player experience how constant email notifications interrupt attention.

The visual style is based on a blurred email inbox background. On top of the blurred background, clear Gmail-style email notification cards keep appearing from a specific email area. Each notification only shows:

- Sender
- Email title
- Short content preview

The scene should feel like the player is looking at an inbox, but their attention is repeatedly pulled away by fresh messages.

---

## 2. Final Scene Effect

The final effect should be:

```text
Background: blurred email / Gmail inbox screenshot
Foreground: clear email notification cards
Motion: cards fade in, slightly move upward, stay briefly, then fade out
Rhythm: notifications become more frequent over time
Experience length: around 30 seconds
Interaction: no player interaction required
```

Recommended experience name:

```text
Inbox Flood
```

Alternative name:

```text
Notification Overload Space
```

---

## 3. Unity Scene Structure

Create the following structure under one Canvas:

```text
Canvas
├── BlurredEmailBackground
├── EmailSpawnArea
├── EmailNotificationSpawner
└── EventSystem
```

The `EmailNotificationPrefab` should be saved in the project folder, not left directly in the scene.

Recommended project folders:

```text
Assets
├── Images
│   └── blurred_email_background.png
├── Prefabs
│   └── EmailNotificationPrefab.prefab
└── Scripts
    ├── EmailNotificationCard.cs
    └── EmailNotificationSpawner.cs
```

---

## 4. Background Setup

### 4.1 Prepare the background image

Use a Gmail or email inbox screenshot as the scene background.

Before importing it into Unity, apply Gaussian Blur in an external tool such as Figma, Photoshop, Canva, or any image editor.

Recommended image settings:

```text
Resolution: 1920 x 1080
Blur strength: 8–16
Brightness: slightly reduced
Saturation: slightly reduced
```

The blur is important because the background should feel like visual noise, not readable content. The clear notification cards should become the main attention-grabbing element.

---

### 4.2 Import into Unity

1. Put the blurred image into:

```text
Assets/Images
```

2. Select the image in Unity.
3. In the Inspector, set:

```text
Texture Type: Sprite (2D and UI)
```

4. Click **Apply**.

---

### 4.3 Create the background UI Image

Inside the Canvas:

```text
Right Click Canvas > UI > Image
```

Rename it:

```text
BlurredEmailBackground
```

Set the Source Image to the blurred email screenshot.

Set its RectTransform to full screen:

```text
Anchor Presets: Stretch Full Screen
Left: 0
Right: 0
Top: 0
Bottom: 0
```

Make sure this object is at the top of the Canvas hierarchy so it renders behind everything else.

---

## 5. Email Spawn Area Setup

Create an empty UI object under Canvas:

```text
Right Click Canvas > Create Empty
```

Rename it:

```text
EmailSpawnArea
```

This object defines the area where new email cards appear.

Recommended RectTransform values:

```text
Anchor: Middle Center
Pos X: 120
Pos Y: 0
Width: 900
Height: 500
```

You can move this area to match the email list area in your blurred background image.

For example, if your blurred Gmail inbox list is on the right side of the image, move `EmailSpawnArea` to the same region.

---

## 6. Email Notification Prefab Setup

### 6.1 Create the card

Inside Canvas, create:

```text
Right Click Canvas > UI > Panel
```

Rename it:

```text
EmailNotificationPrefab
```

Recommended RectTransform:

```text
Width: 760
Height: 86
```

Recommended panel color:

```text
Color: White
Alpha: 230 / 255
```

This makes the card clear but still slightly blended with the background.

---

### 6.2 Add text elements

Inside `EmailNotificationPrefab`, create three TextMeshPro text objects:

```text
EmailNotificationPrefab
├── SenderText
├── TitleText
└── SummaryText
```

Recommended styles:

#### SenderText

```text
Font Size: 20–24
Font Style: Bold
Color: Near black
Position: upper left
```

#### TitleText

```text
Font Size: 20
Font Style: Normal or SemiBold
Color: Near black
Position: middle left
```

#### SummaryText

```text
Font Size: 16–18
Font Style: Normal
Color: Grey
Position: lower left
```

Example text layout:

```text
Canvas LMS
New Assignment Update
Your INFO90003 assignment has received a new comment...
```

---

### 6.3 Save as prefab

Drag `EmailNotificationPrefab` from the Hierarchy into:

```text
Assets/Prefabs
```

After saving it as a prefab, delete the original one from the scene.

The scene should only keep:

```text
Canvas
├── BlurredEmailBackground
├── EmailSpawnArea
├── EmailNotificationSpawner
└── EventSystem
```

---

## 7. Script 1: EmailNotificationCard.cs

Create a script named:

```text
EmailNotificationCard.cs
```

Put it in:

```text
Assets/Scripts
```

Attach this script to the root object of `EmailNotificationPrefab`.

```csharp
using UnityEngine;
using TMPro;
using System.Collections;

public class EmailNotificationCard : MonoBehaviour
{
    [Header("Text References")]
    public TMP_Text senderText;
    public TMP_Text titleText;
    public TMP_Text summaryText;

    [Header("Animation Settings")]
    public float fadeInTime = 0.25f;
    public float stayTime = 2.2f;
    public float fadeOutTime = 0.6f;
    public float moveDistance = 30f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 startPos;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    public void Setup(string sender, string title, string summary)
    {
        senderText.text = sender;
        titleText.text = title;
        summaryText.text = summary;

        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        canvasGroup.alpha = 0f;

        Vector2 hiddenPos = startPos + new Vector2(0, -moveDistance);
        Vector2 visiblePos = startPos;

        rectTransform.anchoredPosition = hiddenPos;

        float t = 0f;

        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            float p = t / fadeInTime;

            canvasGroup.alpha = p;
            rectTransform.anchoredPosition = Vector2.Lerp(hiddenPos, visiblePos, p);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = visiblePos;

        yield return new WaitForSeconds(stayTime);

        t = 0f;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            float p = t / fadeOutTime;

            canvasGroup.alpha = 1f - p;
            rectTransform.anchoredPosition = Vector2.Lerp(
                visiblePos,
                visiblePos + new Vector2(0, moveDistance),
                p
            );

            yield return null;
        }

        Destroy(gameObject);
    }
}
```

---

## 8. Script 2: EmailNotificationSpawner.cs

Create a script named:

```text
EmailNotificationSpawner.cs
```

Put it in:

```text
Assets/Scripts
```

Attach it to the `EmailNotificationSpawner` object in the Canvas.

This version creates a 30-second escalation. Notifications appear slowly at first, then become more frequent.

```csharp
using UnityEngine;

public class EmailNotificationSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public EmailNotificationCard emailPrefab;

    [Header("Spawn Area")]
    public RectTransform spawnArea;

    [Header("Experience Timing")]
    public float experienceDuration = 30f;

    [Header("Spawn Limit")]
    public int maxActiveEmailsEarly = 4;
    public int maxActiveEmailsMiddle = 8;
    public int maxActiveEmailsLate = 14;

    private float elapsedTime;
    private float timer;
    private float nextSpawnTime;

    private string[] senders =
    {
        "Canvas LMS",
        "Gmail",
        "INFO90003 Teaching Team",
        "University of Melbourne",
        "Assignment Bot",
        "Tutor Feedback",
        "Group Member",
        "System Notification",
        "Submission Portal"
    };

    private string[] titles =
    {
        "New Assignment Update",
        "Reminder: Submission Due Soon",
        "Your Grade Has Been Updated",
        "New Comment on Your Work",
        "Team Member Sent You a Message",
        "Feedback Released",
        "Unread Course Announcement",
        "Action Required",
        "Meeting Reminder"
    };

    private string[] summaries =
    {
        "Your INFO90003 project has received a new update.",
        "Please check the latest feedback before your next submission.",
        "A new announcement has been posted by the teaching team.",
        "Your group member mentioned you in a comment.",
        "The deadline is approaching. Please review your progress.",
        "New marks and comments are now available.",
        "You have multiple unread notifications from Canvas.",
        "Please respond to this message as soon as possible.",
        "Your submission status has changed."
    };

    private void Start()
    {
        SetNextSpawnTime();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            timer = 0f;

            if (GetActiveEmailCount() < GetCurrentMaxEmails())
            {
                SpawnEmail();
            }

            SetNextSpawnTime();
        }
    }

    private void SpawnEmail()
    {
        EmailNotificationCard newEmail = Instantiate(emailPrefab, spawnArea);

        RectTransform emailRect = newEmail.GetComponent<RectTransform>();

        float randomX = Random.Range(
            -spawnArea.rect.width / 2f + emailRect.rect.width / 2f,
            spawnArea.rect.width / 2f - emailRect.rect.width / 2f
        );

        float randomY = Random.Range(
            -spawnArea.rect.height / 2f + emailRect.rect.height / 2f,
            spawnArea.rect.height / 2f - emailRect.rect.height / 2f
        );

        emailRect.anchoredPosition = new Vector2(randomX, randomY);

        string sender = senders[Random.Range(0, senders.Length)];
        string title = titles[Random.Range(0, titles.Length)];
        string summary = summaries[Random.Range(0, summaries.Length)];

        newEmail.Setup(sender, title, summary);
    }

    private void SetNextSpawnTime()
    {
        if (elapsedTime < 10f)
        {
            nextSpawnTime = Random.Range(0.9f, 1.7f);
        }
        else if (elapsedTime < 20f)
        {
            nextSpawnTime = Random.Range(0.45f, 0.9f);
        }
        else
        {
            nextSpawnTime = Random.Range(0.15f, 0.45f);
        }
    }

    private int GetCurrentMaxEmails()
    {
        if (elapsedTime < 10f)
        {
            return maxActiveEmailsEarly;
        }
        else if (elapsedTime < 20f)
        {
            return maxActiveEmailsMiddle;
        }
        else
        {
            return maxActiveEmailsLate;
        }
    }

    private int GetActiveEmailCount()
    {
        return spawnArea.childCount;
    }
}
```

---

## 9. Inspector Connection Guide

### 9.1 EmailNotificationPrefab

Open the prefab and select its root object.

Make sure it has this script:

```text
EmailNotificationCard.cs
```

Drag the child text objects into the script fields:

```text
Sender Text  -> SenderText
Title Text   -> TitleText
Summary Text -> SummaryText
```

Recommended animation values:

```text
Fade In Time: 0.25
Stay Time: 2.2
Fade Out Time: 0.6
Move Distance: 30
```

---

### 9.2 EmailNotificationSpawner

Select the scene object:

```text
EmailNotificationSpawner
```

Make sure it has:

```text
EmailNotificationSpawner.cs
```

Drag references into the Inspector:

```text
Email Prefab -> EmailNotificationPrefab from Assets/Prefabs
Spawn Area   -> EmailSpawnArea from the Canvas hierarchy
```

Recommended values:

```text
Experience Duration: 30
Max Active Emails Early: 4
Max Active Emails Middle: 8
Max Active Emails Late: 14
```

---

## 10. Recommended Visual Style

The card should look close to a real Gmail inbox preview.

Recommended design:

```text
Card background: white or very light grey
Card opacity: 90%–95%
Corner radius: optional, if using custom UI image
Text color: black / dark grey
Summary color: medium grey
Card size: wide and short
Motion: subtle, not game-like
```

Avoid making the card fly randomly across the screen. The effect should feel like real emails appearing in the inbox area, not arcade-style floating text.

---

## 11. Optional: Add Sound Feedback

To make the space more stressful, add a short notification sound when each card appears.

Possible sound style:

```text
Short email ding
Soft notification pop
Low-volume repeated alert
```

Do not make the sound too loud at the beginning. It should become stressful through repetition, not through one loud sound.

Possible extension:

```text
0–10 seconds: occasional sound
10–20 seconds: more frequent sound
20–30 seconds: dense sound feedback
```

---

## 12. Optional: Auto Exit After 30 Seconds

If this scene is part of a larger Unity experience, add an auto-exit script that returns the player to the main scene after 30 seconds.

Example script:

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoExitAfterDelay : MonoBehaviour
{
    public float delay = 30f;
    public string targetSceneName = "MainScene";

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= delay)
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
```

Attach it to an empty object in the scene:

```text
AutoExitController
```

Inspector settings:

```text
Delay: 30
Target Scene Name: your main scene name
```

Make sure the target scene has been added to:

```text
File > Build Settings > Scenes In Build
```

---

## 13. Design Rationale

This design works because it creates a clear contrast between background and interruption.

The blurred email background represents the general work or study environment. It is present, but not fully readable. The clear email cards represent sudden interruptions that demand attention.

This makes the distraction experience more realistic than random flying text. The user understands immediately that the problem is not just visual noise, but repeated task interruption from emails and notifications.

The escalating spawn speed also helps communicate a gradual loss of control:

```text
0–10 seconds: manageable distraction
10–20 seconds: uncomfortable interruption
20–30 seconds: overload and loss of focus
```

---

## 14. Common Problems and Fixes

### Problem 1: The email cards do not appear

Check:

```text
Email Prefab is assigned in EmailNotificationSpawner
Spawn Area is assigned
EmailNotificationPrefab has EmailNotificationCard.cs
The prefab text references are assigned
```

---

### Problem 2: Cards appear outside the target area

Check the size and position of:

```text
EmailSpawnArea
```

Reduce its width and height, or move it to better match the email list area in the background.

---

### Problem 3: Text is invisible

Check:

```text
Text color is not white on white
TextMeshPro objects are inside the prefab
Text references are assigned in EmailNotificationCard.cs
Canvas sorting is correct
```

---

### Problem 4: Background covers the email cards

Move `BlurredEmailBackground` to the top of the Canvas hierarchy.

Unity UI renders lower objects later, so objects lower in the hierarchy appear in front.

The correct order should be:

```text
Canvas
├── BlurredEmailBackground
├── EmailSpawnArea
├── EmailNotificationSpawner
└── EventSystem
```

---

### Problem 5: Too many cards make the scene unreadable too early

Reduce these values:

```text
Max Active Emails Early
Max Active Emails Middle
Max Active Emails Late
```

Or increase the spawn intervals in `SetNextSpawnTime()`.

---

## 15. Final Checklist

Before testing, confirm:

```text
[ ] Blurred background image is full screen
[ ] EmailSpawnArea is positioned over the email list area
[ ] EmailNotificationPrefab is saved in Assets/Prefabs
[ ] EmailNotificationCard.cs is attached to the prefab
[ ] SenderText, TitleText, and SummaryText are assigned
[ ] EmailNotificationSpawner.cs is attached to a scene object
[ ] Email Prefab and Spawn Area references are assigned
[ ] Play mode creates clear email cards over the blurred inbox
[ ] Notification frequency increases over time
[ ] Scene exits after 30 seconds if AutoExitAfterDelay is used
```

---

## 16. Suggested Tuning Values

For a calm but noticeable version:

```text
Early max emails: 3
Middle max emails: 6
Late max emails: 10
```

For a stronger overload version:

```text
Early max emails: 4
Middle max emails: 8
Late max emails: 14
```

For an extreme stress version:

```text
Early max emails: 6
Middle max emails: 12
Late max emails: 20
```

Recommended default:

```text
Early max emails: 4
Middle max emails: 8
Late max emails: 14
```
