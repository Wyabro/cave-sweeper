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

    private TorchController _torchController;
    private TextMeshProUGUI _torchStatusText;

    private static readonly Color TextPrimary = new Color(0.88f, 0.80f, 0.66f, 0.75f);
    private static readonly Color TextDim     = new Color(0.88f, 0.80f, 0.66f, 0.38f);
    private static readonly Color BlockOn     = new Color(0.88f, 0.80f, 0.66f, 0.82f);
    private static readonly Color BlockOff    = new Color(0.88f, 0.80f, 0.66f, 0.10f);

    private Image[] _healthBlocks;
    private Image[] _oxygenBlocks;

    private void Awake()
    {
        if (_playerHealth == null) _playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (_playerOxygen == null) _playerOxygen = FindAnyObjectByType<PlayerOxygen>();
        if (_torchController == null) _torchController = FindAnyObjectByType<TorchController>();

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
        if (_torchStatusText != null && _torchController != null)
            _torchStatusText.text = _torchController.IsOn ? "LIT" : "UNLIT";
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

        if (FindAnyObjectByType<EventSystem>() == null)
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
            new Vector2(40f, -40f), new Vector2(525f, 105f));

        var title = MakeTMP(panel, "TaskTitle", "CURRENT TASK",
            22f, FontStyles.Bold, TextAlignmentOptions.TopLeft);
        title.characterSpacing = 7f;
        SetRT(title.rectTransform, new Vector2(0f, 0f), new Vector2(525f, 32f));

        var objective = MakeTMP(panel, "TaskObjective", "Find the way out.",
            28f, FontStyles.Italic, TextAlignmentOptions.TopLeft);
        SetRT(objective.rectTransform, new Vector2(0f, -40f), new Vector2(525f, 50f));
    }

    // Bottom-left: torch status ────────────────────────────────────────────

    private void BuildBottomLeft(Transform canvasT)
    {
        var panel = MakePanel(canvasT, "BottomLeft",
            new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
            new Vector2(40f, 40f), new Vector2(180f, 80f));

        // Torch head
        var headGo = new GameObject("TorchHead");
        var headRt = headGo.AddComponent<RectTransform>();
        headGo.transform.SetParent(panel, false);
        var headImg = headGo.AddComponent<Image>();
        headImg.color = BlockOn;
        headImg.raycastTarget = false;
        SetRT(headRt, new Vector2(0f, -8f), new Vector2(18f, 18f));

        // Torch shaft
        var shaftGo = new GameObject("TorchShaft");
        var shaftRt = shaftGo.AddComponent<RectTransform>();
        shaftGo.transform.SetParent(panel, false);
        var shaftImg = shaftGo.AddComponent<Image>();
        shaftImg.color = BlockOn;
        shaftImg.raycastTarget = false;
        SetRT(shaftRt, new Vector2(6f, -26f), new Vector2(5f, 45f));

        _torchStatusText = MakeTMP(panel, "TorchStatus", "UNLIT",
            25f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        SetRT(_torchStatusText.rectTransform, new Vector2(32f, -8f), new Vector2(140f, 35f));

        var hint = MakeTMP(panel, "KeybindHint", "[F] TORCH",
            20f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        hint.color = TextDim;
        SetRT(hint.rectTransform, new Vector2(32f, -45f), new Vector2(140f, 30f));
    }

    // Bottom-right: health + oxygen bars ──────────────────────────────────

    private (RectTransform health, RectTransform oxygen) BuildBottomRight(Transform canvasT)
    {
        var panel = MakePanel(canvasT, "BottomRight",
            new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f),
            new Vector2(-40f, 40f), new Vector2(350f, 160f));

        var healthLabel = MakeTMP(panel, "HealthLabel", "HEALTH",
            21f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        healthLabel.color = TextDim;
        healthLabel.characterSpacing = 5f;
        SetRT(healthLabel.rectTransform, new Vector2(15f, -15f), new Vector2(320f, 30f));

        var healthBlocksGo = new GameObject("HealthBlocks");
        var healthRt = healthBlocksGo.AddComponent<RectTransform>();
        healthBlocksGo.transform.SetParent(panel, false);
        SetRT(healthRt, new Vector2(15f, -52f), new Vector2(320f, 20f));
        AddHLG(healthBlocksGo);

        var oxygenLabel = MakeTMP(panel, "OxygenLabel", "OXYGEN",
            21f, FontStyles.Normal, TextAlignmentOptions.TopLeft);
        oxygenLabel.color = TextDim;
        oxygenLabel.characterSpacing = 5f;
        SetRT(oxygenLabel.rectTransform, new Vector2(15f, -90f), new Vector2(320f, 30f));

        var oxygenBlocksGo = new GameObject("OxygenBlocks");
        var oxygenRt = oxygenBlocksGo.AddComponent<RectTransform>();
        oxygenBlocksGo.transform.SetParent(panel, false);
        SetRT(oxygenRt, new Vector2(15f, -127f), new Vector2(320f, 20f));
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
            rt.sizeDelta = new Vector2(20f, 15f);
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
