# Unity Scene Guide: Email Distraction Tunnel

## 1. Project Overview

This document describes a standalone Unity experience space called **Email Distraction Tunnel**.

The scene is designed as a **30-second non-interactive experience**. The player enters a dark abstract tunnel where a clear goal appears in the distance. As time passes, email notifications gradually appear, fly across the player's field of view, and eventually form a visual wall that blocks the original goal.

The experience is intended to communicate how constant email notifications interrupt attention and gradually make it harder to stay focused.

The player does not need to move, click, select, or interact with anything. The scene automatically plays from start to finish and exits after 30 seconds.

---

## 2. Core Experience Concept

### Scene Name

```text
EmailDistractionTunnel
```

### Main Theme

The player experiences a transition from:

```text
Clear focus -> Light interruption -> Increasing distraction -> Information overload -> Exit
```

### Main Visual Metaphor

The tunnel represents the player's focus path.  
The glowing text at the end of the tunnel represents the player's original goal.  
The emails represent external interruptions.  
The final email wall represents the loss of focus caused by repeated digital distractions.

---

## 3. Experience Timeline

The whole scene lasts approximately **30 seconds**.

---

### 0-6 seconds: Clear Focus Stage

#### Visuals

- The player stands inside a dark tunnel.
- The camera is fixed.
- At the far end of the tunnel, a glowing text says:

```text
Finish the task
```

- No email notifications appear yet.
- The tunnel is quiet and visually clean.

#### Intended Feeling

The player understands that there is a clear goal ahead. The environment feels focused, calm, and controlled.

---

### 6-12 seconds: Light Interruption Stage

#### Visuals

- A small number of email cards begin to fly across the player's view.
- They enter from the left or right side of the tunnel.
- They do not fully block the goal.
- Each email card contains a short message.

#### Example Email Messages

```text
Quick question
Can you check this?
Just following up
Meeting update
```

#### Audio

- A soft notification sound plays whenever an email card appears.

#### Intended Feeling

The player can still see the goal, but small interruptions have started to break the sense of focus.

---

### 12-21 seconds: Increasing Distraction Stage

#### Visuals

- Email cards appear more frequently.
- They enter from multiple directions: left, right, top, bottom, and front.
- Some emails quickly fly past the player.
- Some emails stay in the view for a short moment.
- The glowing goal text begins to flicker or dim slightly.

#### Example Email Messages

```text
Reply needed
Deadline reminder
New comment added
Please confirm
Can you review this?
Action required
```

#### Audio

- Notification sounds become more frequent.
- The soundscape becomes more stressful.

#### Intended Feeling

The player still remembers the goal, but the surrounding information begins to compete for attention.

---

### 21-28 seconds: Email Wall Stage

#### Visuals

- Email cards spawn very quickly.
- Instead of flying away, many of them stop in front of the camera.
- These email cards gradually form a wall.
- The original goal text is mostly or completely blocked.
- The screen may include a slight vignette or blur effect to increase pressure.

#### Example Email Messages

```text
URGENT
ACTION REQUIRED
REPLY NOW
FOLLOW UP
MEETING CHANGED
FINAL REMINDER
ASAP
PLEASE RESPOND
```

#### Audio

- Notification sounds overlap more densely.
- The sound should feel chaotic but not painfully loud.

#### Intended Feeling

The player can no longer focus on the original goal. The only visible things are interruptions.

---

### 28-30 seconds: Collapse and Exit Stage

#### Visuals

- The email wall moves closer to the camera.
- The screen is gradually filled by email cards.
- The scene fades to black.
- A final message appears:

```text
Too many interruptions.
```

#### Scene Exit

At 30 seconds, the scene automatically loads another scene, such as:

```text
MainScene
```

#### Intended Feeling

The player experiences the final loss of focus, followed by an automatic exit from the distraction space.

---

## 4. Unity Hierarchy Structure

Recommended hierarchy:

```text
EmailDistractionTunnel
|
|-- Main Camera
|-- Directional Light
|
|-- TunnelRoot
|   |-- Floor
|   |-- LeftWall
|   |-- RightWall
|   |-- Ceiling
|   |-- BackWall
|   |-- EndGlow
|
|-- FocusTarget
|   |-- FinishTaskText
|
|-- EmailSystem
|   |-- EmailSpawner
|   |-- EmailMoveTarget
|   |-- EmailWallTargetRoot
|   |-- EmailSpawnPoints
|       |-- Spawn_Left
|       |-- Spawn_Right
|       |-- Spawn_Top
|       |-- Spawn_Bottom
|       |-- Spawn_Front
|
|-- UI
|   |-- FadeCanvas
|       |-- BlackFadePanel
|       |-- EndMessageText
|
|-- Audio
|   |-- AmbientAudioSource
|   |-- NotificationAudioSource
|
|-- SceneAutoExitManager
|-- EmailDistractionDirector
```

---

## 5. Required Scene Objects

### 5.1 Main Camera

The camera should be fixed. The player is only watching the experience.

Recommended transform:

```text
Position: (0, 1.6, -6)
Rotation: (0, 0, 0)
Field of View: 60-70
```

No player controller is required.

---

### 5.2 Tunnel Objects

The tunnel can be built using simple Cube objects.

Recommended object sizes:

```text
Floor
Position: (0, -1, 10)
Scale:    (8, 0.2, 40)

LeftWall
Position: (-4, 1.5, 10)
Scale:    (0.2, 5, 40)

RightWall
Position: (4, 1.5, 10)
Scale:    (0.2, 5, 40)

Ceiling
Position: (0, 4, 10)
Scale:    (8, 0.2, 40)

BackWall
Position: (0, 1.5, 30)
Scale:    (8, 5, 0.2)
```

Recommended material:

```text
Dark gray or black material
Slight emission optional
Low smoothness
```

---

### 5.3 Focus Target

Create an empty object:

```text
FocusTarget
Position: (0, 1.8, 24)
```

Inside it, add a TextMeshPro 3D Text object:

```text
Text: Finish the task
Position: (0, 0, 0)
Rotation: (0, 0, 0)
Font Size: 2
Alignment: Center
Color: White, cyan, or light blue
```

Optional:

- Add a Point Light behind the text.
- Add a glowing plane behind the text.
- Use an emission material to make the goal feel important.

---

### 5.4 Email Spawn Points

Create an empty object:

```text
EmailSpawnPoints
```

Add the following child objects:

```text
Spawn_Left
Position: (-6, 1.8, 8)

Spawn_Right
Position: (6, 1.8, 8)

Spawn_Top
Position: (0, 5, 10)

Spawn_Bottom
Position: (0, -0.5, 10)

Spawn_Front
Position: (0, 2, 24)
```

Create another empty object:

```text
EmailMoveTarget
Position: (0, 1.8, -3)
```

Normal email cards should move toward `EmailMoveTarget`, so they pass through the player's field of view.

---

## 6. Required Prefab

## 6.1 EmailCardPrefab

Create a prefab called:

```text
EmailCardPrefab
```

Recommended structure:

```text
EmailCardPrefab
|
|-- World Space Canvas
    |-- Background
    |-- EmailText
```

### Canvas Settings

```text
Render Mode: World Space
Width: 300
Height: 100
Scale: around (0.01, 0.01, 0.01) depending on scene size
```

### Background

Use a UI Image.

Recommended style:

```text
Color: White or light gray
Alpha: 0.7-0.85
Rounded corners optional
```

### EmailText

Use TextMeshProUGUI.

Recommended settings:

```text
Font Size: 24-36
Alignment: Center
Color: Black or dark gray
```

### Script

Attach this script to the root object:

```text
EmailCardMover.cs
```

---

## 7. Required Scripts

Create the following scripts:

```text
EmailDistractionDirector.cs
EmailSpawner.cs
EmailCardMover.cs
SceneAutoExitManager.cs
```

---

# 8. Script: SceneAutoExitManager.cs

## Purpose

This script handles leaving the distraction scene after the experience ends.

## Code

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutoExitManager : MonoBehaviour
{
    [Header("Scene Exit")]
    public string exitSceneName = "MainScene";

    public void ExitScene()
    {
        SceneManager.LoadScene(exitSceneName);
    }
}
```

## Inspector Setup

Attach this script to:

```text
SceneAutoExitManager
```

Set:

```text
Exit Scene Name: MainScene
```

Make sure `MainScene` is added to Build Settings.

---

# 9. Script: EmailCardMover.cs

## Purpose

This script controls the movement and lifetime of each email card.

It supports two modes:

1. Fly-through mode: the card moves across the player's view and disappears.
2. Wall mode: the card moves to a position in front of the camera and stays there.

## Code

```csharp
using TMPro;
using UnityEngine;

public class EmailCardMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float lifeTime = 6f;
    public Transform moveTarget;

    [Header("Wall Mode")]
    public bool stopAtTarget = false;
    public float stopDistance = 0.05f;

    [Header("Text")]
    public TextMeshProUGUI emailText;

    private float timer;
    private bool hasStopped;

    private void Update()
    {
        FaceCamera();

        if (moveTarget != null && !hasStopped)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                moveTarget.position,
                moveSpeed * Time.deltaTime
            );

            float distance = Vector3.Distance(transform.position, moveTarget.position);

            if (stopAtTarget && distance <= stopDistance)
            {
                hasStopped = true;
            }
        }

        if (!stopAtTarget)
        {
            timer += Time.deltaTime;
            if (timer >= lifeTime)
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetText(string message)
    {
        if (emailText != null)
        {
            emailText.text = message;
        }
    }

    private void FaceCamera()
    {
        if (Camera.main == null) return;

        transform.LookAt(Camera.main.transform);
        transform.Rotate(0f, 180f, 0f);
    }
}
```

## Inspector Setup

Attach this to:

```text
EmailCardPrefab root object
```

Assign:

```text
Email Text: EmailText child object
```

---

# 10. Script: EmailSpawner.cs

## Purpose

This script dynamically creates email cards during the scene.

It controls:

- Spawn frequency
- Message selection
- Notification sound
- Normal flying emails
- Email wall behavior

## Code

```csharp
using UnityEngine;

public class EmailSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject emailCardPrefab;

    [Header("Spawn Setup")]
    public Transform[] spawnPoints;
    public Transform normalMoveTarget;
    public Transform wallTargetRoot;

    [Header("Audio")]
    public AudioSource notificationAudio;

    [Header("Messages")]
    public string[] lowIntensityMessages =
    {
        "Quick question",
        "Can you check this?",
        "Just following up",
        "Meeting update"
    };

    public string[] mediumIntensityMessages =
    {
        "Reply needed",
        "Deadline reminder",
        "New comment added",
        "Please confirm",
        "Can you review this?",
        "Action required"
    };

    public string[] highIntensityMessages =
    {
        "URGENT",
        "ACTION REQUIRED",
        "REPLY NOW",
        "FOLLOW UP",
        "MEETING CHANGED",
        "FINAL REMINDER",
        "ASAP",
        "PLEASE RESPOND"
    };

    [Header("Runtime")]
    [Range(0, 4)]
    public int intensity = 0;

    private float spawnTimer;
    private int wallIndex;

    private void Update()
    {
        if (intensity <= 0) return;

        spawnTimer += Time.deltaTime;

        float interval = GetSpawnInterval();

        if (spawnTimer >= interval)
        {
            spawnTimer = 0f;
            SpawnEmail();
        }
    }

    public void SetIntensity(int newIntensity)
    {
        intensity = Mathf.Clamp(newIntensity, 0, 4);
    }

    private float GetSpawnInterval()
    {
        switch (intensity)
        {
            case 1:
                return 1.6f;
            case 2:
                return 0.8f;
            case 3:
                return 0.35f;
            case 4:
                return 0.15f;
            default:
                return 999f;
        }
    }

    private void SpawnEmail()
    {
        if (emailCardPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject card = Instantiate(emailCardPrefab, spawnPoint.position, spawnPoint.rotation);

        EmailCardMover mover = card.GetComponent<EmailCardMover>();

        if (mover != null)
        {
            mover.SetText(GetRandomMessage());

            if (intensity >= 4)
            {
                mover.stopAtTarget = true;
                mover.moveTarget = CreateWallTarget();
                mover.moveSpeed = 6f;
            }
            else
            {
                mover.stopAtTarget = false;
                mover.moveTarget = normalMoveTarget;
                mover.moveSpeed = Random.Range(3f, 6f);
                mover.lifeTime = Random.Range(4f, 7f);
            }
        }

        if (notificationAudio != null)
        {
            notificationAudio.pitch = Random.Range(0.85f, 1.15f);
            notificationAudio.PlayOneShot(notificationAudio.clip);
        }
    }

    private string GetRandomMessage()
    {
        string[] selectedArray;

        if (intensity <= 1)
        {
            selectedArray = lowIntensityMessages;
        }
        else if (intensity <= 3)
        {
            selectedArray = mediumIntensityMessages;
        }
        else
        {
            selectedArray = highIntensityMessages;
        }

        if (selectedArray == null || selectedArray.Length == 0)
        {
            return "New Email";
        }

        return selectedArray[Random.Range(0, selectedArray.Length)];
    }

    private Transform CreateWallTarget()
    {
        GameObject target = new GameObject("EmailWallTarget_" + wallIndex);

        if (wallTargetRoot != null)
        {
            target.transform.SetParent(wallTargetRoot);
        }

        int columns = 5;
        int rows = 4;

        int col = wallIndex % columns;
        int row = (wallIndex / columns) % rows;

        float x = -3f + col * 1.5f + Random.Range(-0.2f, 0.2f);
        float y = 0.5f + row * 0.75f + Random.Range(-0.1f, 0.1f);
        float z = 2.5f + Random.Range(-0.3f, 0.3f);

        target.transform.position = new Vector3(x, y, z);
        wallIndex++;

        return target.transform;
    }
}
```

## Inspector Setup

Attach this script to:

```text
EmailSpawner
```

Assign:

```text
Email Card Prefab: EmailCardPrefab
Spawn Points: Spawn_Left, Spawn_Right, Spawn_Top, Spawn_Bottom, Spawn_Front
Normal Move Target: EmailMoveTarget
Wall Target Root: EmailWallTargetRoot
Notification Audio: NotificationAudioSource
```

---

# 11. Script: EmailDistractionDirector.cs

## Purpose

This script controls the full 30-second experience.

It changes the intensity of the email spawner, fades the target text, displays the final message, and exits the scene.

## Code

```csharp
using TMPro;
using UnityEngine;

public class EmailDistractionDirector : MonoBehaviour
{
    [Header("Timing")]
    public float totalDuration = 30f;

    [Header("Email System")]
    public EmailSpawner emailSpawner;

    [Header("Focus Target")]
    public TextMeshPro focusTargetText;
    public Light focusLight;

    [Header("Fade UI")]
    public CanvasGroup fadeCanvasGroup;
    public TextMeshProUGUI endMessageText;

    [Header("Exit")]
    public SceneAutoExitManager exitManager;

    private float timer;
    private bool hasExited;

    private void Start()
    {
        timer = 0f;

        if (emailSpawner != null)
        {
            emailSpawner.SetIntensity(0);
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
        }

        if (endMessageText != null)
        {
            endMessageText.text = "";
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        UpdateExperienceStage();
        UpdateFocusTarget();
        UpdateFinalFade();

        if (timer >= totalDuration && !hasExited)
        {
            hasExited = true;

            if (exitManager != null)
            {
                exitManager.ExitScene();
            }
        }
    }

    private void UpdateExperienceStage()
    {
        if (emailSpawner == null) return;

        if (timer < 6f)
        {
            emailSpawner.SetIntensity(0);
        }
        else if (timer < 12f)
        {
            emailSpawner.SetIntensity(1);
        }
        else if (timer < 21f)
        {
            emailSpawner.SetIntensity(2);
        }
        else if (timer < 28f)
        {
            emailSpawner.SetIntensity(4);
        }
        else
        {
            emailSpawner.SetIntensity(4);
        }
    }

    private void UpdateFocusTarget()
    {
        float distractionProgress = Mathf.InverseLerp(6f, 28f, timer);
        float flicker = Mathf.Sin(Time.time * 18f) * 0.08f;
        float alpha = Mathf.Clamp01(1f - distractionProgress * 0.8f + flicker);

        if (focusTargetText != null)
        {
            Color color = focusTargetText.color;
            color.a = alpha;
            focusTargetText.color = color;
        }

        if (focusLight != null)
        {
            focusLight.intensity = Mathf.Lerp(4f, 0.5f, distractionProgress);
        }
    }

    private void UpdateFinalFade()
    {
        if (timer < 28f) return;

        float fadeProgress = Mathf.InverseLerp(28f, 30f, timer);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = fadeProgress;
        }

        if (endMessageText != null)
        {
            endMessageText.text = "Too many interruptions.";
        }
    }
}
```

## Inspector Setup

Attach this script to:

```text
EmailDistractionDirector
```

Assign:

```text
Email Spawner: EmailSpawner
Focus Target Text: FinishTaskText
Focus Light: optional Point Light near the goal
Fade Canvas Group: FadeCanvas CanvasGroup
End Message Text: EndMessageText
Exit Manager: SceneAutoExitManager
```

---

## 12. Fade UI Setup

Create a Canvas:

```text
FadeCanvas
```

Recommended settings:

```text
Render Mode: Screen Space - Overlay
Canvas Scaler: Scale With Screen Size
Reference Resolution: 1920 x 1080
```

Add a CanvasGroup component to `FadeCanvas`.

Inside the canvas, create:

```text
BlackFadePanel
EndMessageText
```

### BlackFadePanel

Use UI Image:

```text
Anchor: Stretch full screen
Color: Black
Alpha controlled by CanvasGroup
```

### EndMessageText

Use TextMeshProUGUI:

```text
Text: empty at start
Alignment: Center
Font Size: 48
Color: White
Position: Center of screen
```

---

## 13. Audio Setup

Create an empty object:

```text
Audio
```

Inside it, create:

```text
AmbientAudioSource
NotificationAudioSource
```

### AmbientAudioSource

Recommended settings:

```text
Loop: true
Play On Awake: true
Volume: 0.2
```

Use a low hum, wind sound, or quiet electronic ambience.

### NotificationAudioSource

Recommended settings:

```text
Loop: false
Play On Awake: false
Volume: 0.4
```

Assign a short email notification sound clip.

---

## 14. Codex Task Prompt

You can copy this prompt into Codex:

```text
Create a Unity scene called EmailDistractionTunnel.

The scene is a 30-second non-interactive experience about being distracted by email notifications.

Requirements:
1. The player does not move and does not interact.
2. Main Camera is fixed at position (0, 1.6, -6), facing forward.
3. Create a dark tunnel using cube objects: floor, left wall, right wall, ceiling, and back wall.
4. Place a glowing TextMeshPro 3D text at the far end of the tunnel saying “Finish the task”.
5. Create an EmailCardPrefab using a World Space Canvas with a semi-transparent background and TextMeshPro text.
6. Email cards should spawn dynamically using Object.Instantiate.
7. From 0-6 seconds, no emails appear.
8. From 6-12 seconds, spawn emails slowly from the sides.
9. From 12-21 seconds, spawn emails faster from multiple directions.
10. From 21-28 seconds, spawn emails very quickly and make them stop in front of the camera to form an email wall.
11. From 28-30 seconds, fade the screen to black and show the message “Too many interruptions.”
12. At 30 seconds, automatically load a scene called MainScene.
13. Create the following scripts:
- EmailDistractionDirector.cs
- EmailSpawner.cs
- EmailCardMover.cs
- SceneAutoExitManager.cs
14. Make sure all public references can be assigned in the Inspector.
15. Use TextMeshPro for all visible text.
16. Add comments in the code explaining what each script does.
```

---

## 15. Final Scene Checklist

Before testing, check the following:

```text
[ ] EmailDistractionTunnel scene exists.
[ ] Main Camera is fixed at (0, 1.6, -6).
[ ] Tunnel cubes are visible and correctly positioned.
[ ] Finish the task text is visible at the end of the tunnel.
[ ] EmailCardPrefab exists and uses World Space Canvas.
[ ] EmailCardMover is attached to EmailCardPrefab.
[ ] EmailSpawner has all spawn points assigned.
[ ] EmailSpawner has EmailCardPrefab assigned.
[ ] NotificationAudioSource is assigned.
[ ] EmailDistractionDirector has all references assigned.
[ ] FadeCanvas has CanvasGroup.
[ ] EndMessageText is assigned.
[ ] SceneAutoExitManager exit scene name is correct.
[ ] MainScene is added to Build Settings.
[ ] The experience lasts about 30 seconds.
[ ] The scene exits automatically after the fade.
```

---

## 16. Expected Final Experience

The player enters a dark tunnel and sees a clear goal ahead:

```text
Finish the task
```

At first, the space is calm. Then emails begin to fly through the view. The interruptions become faster, louder, and more visually dominant. Eventually, the emails block the goal completely and form a wall in front of the player.

The screen fades to black with the final message:

```text
Too many interruptions.
```

Then the scene automatically exits.

This creates a short but clear experience of how constant email notifications can fragment attention and make the original task disappear from view.
