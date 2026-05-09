using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private PlayerOxygen _playerOxygen;
    [SerializeField] private int _blockCount = 10;

    private static readonly Color TextPrimary = new Color(0.88f, 0.80f, 0.66f, 0.85f);
    private static readonly Color TextDim     = new Color(0.88f, 0.80f, 0.66f, 0.50f);
    private static readonly Color PanelBg     = new Color(0.05f, 0.04f, 0.03f, 0.45f);
    private static readonly Color BlockOn     = new Color(0.88f, 0.80f, 0.66f, 0.90f);
    private static readonly Color BlockOff    = new Color(0.88f, 0.80f, 0.66f, 0.12f);

    private Image[] _healthBlocks;
    private Image[] _oxygenBlocks;

    private void Awake()
    {
        if (_playerHealth == null) _playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (_playerOxygen == null) _playerOxygen = FindFirstObjectByType<PlayerOxygen>();

        var canvas = BuildCanvas();
        BuildTopLeft(canvas.transform);
        BuildBottomLeft(canvas.transform);
        var (healthParent, oxygenParent) = BuildBottomRight(canvas.transform);
        _healthBlocks = SpawnBlocks(healthParent);
        _oxygenBlocks = SpawnBlocks(oxygenParent);
    }

    private void Update()
    {
        if (_healthBlocks == null || _oxygenBlocks == null) return;
        SetBar(_healthBlocks, _playerHealth != null ? _playerHealth.HealthNormalized : 1f);
        SetBar(_oxygenBlocks, _playerOxygen != null ? _playerOxygen.Oxygen : 1f);
    }

    // Canvas + EventSystem ─────────────────────────────────────────────────

    private Canvas BuildCanvas()
    {
        var go = new GameObject("HUDCanvas");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var esGo = new GameObject("EventSystem");
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }

        return canvas;
    }

    // Top-left: CURRENT TASK ───────────────────────────────────────────────

    private void BuildTopLeft(Transform canvasT)
    {
        var panel = MakePanel(canvasT, "TopLeft",
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(20f, -20f), new Vector2(320f, 60f));
        AddBg(panel);

        var title = MakeTMP(panel, "TaskTitle", "CURRENT TASK",
            12f, FontStyles.Bold, TextAlignmentOptions.TopLeft);
        SetRT(title.rectTransform, new Vector2(10f, -8f), new Vector2(300f, 18f));

        var objective = MakeTMP(panel, "TaskObjective", "Find the way out.",
            15f, FontStyles.Italic, TextAlignmentOptions.TopLeft);
        SetRT(objective.rectTransform, new Vector2(10f, -30f), new Vector2(300f, 22f));
    }

    // Bottom-left: torch status ────────────────────────────────────────────

    private void BuildBottomLeft(Transform canvasT)
    {
        var panel = MakePanel(canvasT, "BottomLeft",
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
            new Vector2(20f, 20f), new Vector2(180f, 56f));
        AddBg(panel);

        var iconGo  = new GameObject("TorchIcon");
        var iconRt  = iconGo.AddComponent<RectTransform>();
        iconGo.transform.SetParent(panel, false);
        var iconImg = iconGo.AddComponent<Image>();
        iconImg.color = BlockOn;
        iconImg.raycastTarget = false;
        SetRT(iconRt, new Vector2(8f, -12f), new Vector2(14f, 14f));

        var status = MakeTMP(panel, "TorchStatus", "UNLIT",
            14f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        SetRT(status.rectTransform, new Vector2(28f, -8f), new Vector2(130f, 20f));

        var hint = MakeTMP(panel, "KeybindHint", "[F] torch",
            11f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        hint.color = TextDim;
        SetRT(hint.rectTransform, new Vector2(28f, -30f), new Vector2(130f, 18f));
    }

    // Bottom-right: health + oxygen bars ──────────────────────────────────

    private (RectTransform health, RectTransform oxygen) BuildBottomRight(Transform canvasT)
    {
        var panel = MakePanel(canvasT, "BottomRight",
            new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f),
            new Vector2(-20f, 20f), new Vector2(210f, 96f));
        AddBg(panel);

        var healthLabel = MakeTMP(panel, "HealthLabel", "HEALTH",
            11f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        healthLabel.color = TextDim;
        SetRT(healthLabel.rectTransform, new Vector2(8f, -8f), new Vector2(194f, 16f));

        var healthBlocksGo = new GameObject("HealthBlocks");
        var healthRt = healthBlocksGo.AddComponent<RectTransform>();
        healthBlocksGo.transform.SetParent(panel, false);
        SetRT(healthRt, new Vector2(8f, -28f), new Vector2(194f, 16f));
        AddHLG(healthBlocksGo);

        var oxygenLabel = MakeTMP(panel, "OxygenLabel", "OXYGEN",
            11f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        oxygenLabel.color = TextDim;
        SetRT(oxygenLabel.rectTransform, new Vector2(8f, -54f), new Vector2(194f, 16f));

        var oxygenBlocksGo = new GameObject("OxygenBlocks");
        var oxygenRt = oxygenBlocksGo.AddComponent<RectTransform>();
        oxygenBlocksGo.transform.SetParent(panel, false);
        SetRT(oxygenRt, new Vector2(8f, -74f), new Vector2(194f, 16f));
        AddHLG(oxygenBlocksGo);

        return (healthRt, oxygenRt);
    }

    // Helpers ──────────────────────────────────────────────────────────────

    private RectTransform MakePanel(Transform parent, string goName,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(goName);
        var rt = go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return rt;
    }

    private TextMeshProUGUI MakeTMP(Transform parent, string goName, string text,
        float fontSize, FontStyles style, TextAlignmentOptions alignment)
    {
        var go = new GameObject(goName);
        go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = style;
        tmp.alignment = alignment;
        tmp.color = TextPrimary;
        tmp.raycastTarget = false;
        return tmp;
    }

    private void AddBg(Transform parent)
    {
        var go = new GameObject("Bg");
        var rt = go.AddComponent<RectTransform>();
        go.transform.SetParent(parent, false);
        go.transform.SetAsFirstSibling();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color = PanelBg;
        img.raycastTarget = false;
    }

    private void AddHLG(GameObject go)
    {
        var hlg = go.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 3f;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
    }

    private void SetRT(RectTransform rt, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot     = new Vector2(0f, 1f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    private Image[] SpawnBlocks(RectTransform parent)
    {
        var blocks = new Image[_blockCount];
        for (int i = 0; i < _blockCount; i++)
        {
            var go = new GameObject($"Block_{i}");
            var rt = go.AddComponent<RectTransform>();
            go.transform.SetParent(parent, false);
            rt.sizeDelta = new Vector2(16f, 14f);
            var img = go.AddComponent<Image>();
            img.color = BlockOn;
            img.raycastTarget = false;
            blocks[i] = img;
        }
        return blocks;
    }

    private void SetBar(Image[] blocks, float normalized)
    {
        int active = Mathf.CeilToInt(normalized * blocks.Length);
        for (int i = 0; i < blocks.Length; i++)
            blocks[i].color = i < active ? BlockOn : BlockOff;
    }
}
