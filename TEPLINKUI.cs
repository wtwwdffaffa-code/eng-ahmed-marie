using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TEPLINKUI : MonoBehaviour
{
    public TEPLINKGameManager manager;
    public static bool IsMenuOpen;
    private Canvas canvas;
    private Text statusText, noticeText, cyberText, crosshair;
    private GameObject cyberPanel, pausePanel;
    private float noticeUntil;
    private TEPLINKComputer selectedComputer;
    private Font font;

    private void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        var eventObj = new GameObject("EventSystem");
        eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<StandaloneInputModule>();

        var canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObj.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1280, 720);
        canvasObj.AddComponent<GraphicRaycaster>();

        statusText = MakeText("Status", new Vector2(0, 1), new Vector2(18, -18), new Vector2(360, 120), 20, TextAnchor.UpperLeft);
        noticeText = MakeText("Notice", new Vector2(.5f, 1), new Vector2(0, -35), new Vector2(760, 48), 22, TextAnchor.MiddleCenter);
        crosshair = MakeText("Crosshair", new Vector2(.5f, .5f), Vector2.zero, new Vector2(40, 40), 28, TextAnchor.MiddleCenter);
        crosshair.text = "+";
        BuildCyberPanel();
        BuildPausePanel();
        BuildMobileControls();
        Refresh();
    }

    private Text MakeText(string name, Vector2 anchor, Vector2 anchoredPosition, Vector2 size, int fontSize, TextAnchor align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(canvas.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        var text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = align;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private Button MakeButton(string label, Vector2 anchor, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject("Button " + label);
        go.transform.SetParent(canvas.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;
        var image = go.AddComponent<Image>();
        image.color = new Color(.08f, .19f, .31f, .94f);
        var button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        var text = MakeText("Label", new Vector2(.5f, .5f), Vector2.zero, size, 17, TextAnchor.MiddleCenter);
        text.transform.SetParent(go.transform, false);
        var tr = text.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
        text.text = label;
        return button;
    }

    private void BuildCyberPanel()
    {
        cyberPanel = new GameObject("Cyber Management Panel");
        cyberPanel.transform.SetParent(canvas.transform, false);
        var rect = cyberPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(.5f, .5f); rect.anchorMax = new Vector2(.5f, .5f);
        rect.anchoredPosition = Vector2.zero; rect.sizeDelta = new Vector2(600, 430);
        var bg = cyberPanel.AddComponent<Image>(); bg.color = new Color(.025f, .05f, .09f, .97f);
        cyberText = MakeText("Cyber Info", new Vector2(.5f, 1), new Vector2(0, -70), new Vector2(540, 270), 22, TextAnchor.UpperCenter);
        cyberText.transform.SetParent(cyberPanel.transform, false);
        var rt = cyberText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(.5f, 1); rt.anchorMax = new Vector2(.5f, 1); rt.anchoredPosition = new Vector2(0, -65);
        MakePanelButton(cyberPanel.transform, "شراء كمبيوتر (900 جنيه)", new Vector2(0, -145), new Vector2(300, 42), () => manager.BuyComputer());
        MakePanelButton(cyberPanel.transform, "دفع الإنترنت", new Vector2(0, -195), new Vector2(300, 42), () => manager.PayInternet());
        MakePanelButton(cyberPanel.transform, "قفل السيستم", new Vector2(0, -245), new Vector2(300, 42), CloseCyberPanel);
        cyberPanel.SetActive(false);
    }

    private Button MakePanelButton(Transform parent, string label, Vector2 pos, Vector2 size, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject(label);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, 1);
        rect.anchoredPosition = pos; rect.sizeDelta = size;
        var img = go.AddComponent<Image>(); img.color = new Color(.12f, .3f, .45f, 1);
        var btn = go.AddComponent<Button>(); btn.targetGraphic = img; btn.onClick.AddListener(action);
        var t = MakeText("Label", new Vector2(.5f, .5f), Vector2.zero, size, 18, TextAnchor.MiddleCenter);
        t.transform.SetParent(go.transform, false);
        var tr = t.GetComponent<RectTransform>(); tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = tr.offsetMax = Vector2.zero;
        t.text = label;
        return btn;
    }

    private void BuildPausePanel()
    {
        pausePanel = new GameObject("Pause hint");
        pausePanel.transform.SetParent(canvas.transform, false);
        var r = pausePanel.AddComponent<RectTransform>();
        r.anchorMin = r.anchorMax = new Vector2(.5f, .5f); r.anchoredPosition = Vector2.zero; r.sizeDelta = new Vector2(430, 130);
        var image = pausePanel.AddComponent<Image>(); image.color = new Color(0, 0, 0, .8f);
        var t = MakeText("Pause instructions", new Vector2(.5f, .5f), Vector2.zero, new Vector2(410, 110), 20, TextAnchor.MiddleCenter);
        t.transform.SetParent(pausePanel.transform, false);
        var tr = t.GetComponent<RectTransform>(); tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = tr.offsetMax = Vector2.zero;
        t.text = "اللعبة متوقفة مؤقتًا\nاضغط بالماوس للعودة";
        pausePanel.SetActive(false);
    }

    private void BuildMobileControls()
    {
        // Basic on-screen movement buttons are visible in touch-oriented builds.
        MakeHoldButton("▲", new Vector2(0, 0), new Vector2(72, 150), "forward");
        MakeHoldButton("▼", new Vector2(0, 0), new Vector2(72, 35), "back");
        MakeHoldButton("◀", new Vector2(0, 0), new Vector2(15, 92), "left");
        MakeHoldButton("▶", new Vector2(0, 0), new Vector2(129, 92), "right");
        MakeButton("تفاعل", new Vector2(1, 0), new Vector2(-85, 80), new Vector2(120, 58), () => manager.player.MobileInteract());
    }

    private void MakeHoldButton(string label, Vector2 anchor, Vector2 pos, string direction)
    {
        var go = new GameObject("Hold " + label);
        go.transform.SetParent(canvas.transform, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = anchor;
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(58, 58);
        var image = go.AddComponent<Image>();
        image.color = new Color(.08f, .19f, .31f, .78f);
        var button = go.AddComponent<Button>();
        button.targetGraphic = image;
        var labelText = MakeText("Label", new Vector2(.5f, .5f), Vector2.zero, new Vector2(58, 58), 22, TextAnchor.MiddleCenter);
        labelText.transform.SetParent(go.transform, false);
        var textRect = labelText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero; textRect.offsetMax = Vector2.zero;
        labelText.text = label;

        var trigger = go.AddComponent<EventTrigger>();
        var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener(_ => manager.player.SetMobileMove(direction, true));
        trigger.triggers.Add(down);
        var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener(_ => manager.player.SetMobileMove(direction, false));
        trigger.triggers.Add(up);
        var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => manager.player.SetMobileMove(direction, false));
        trigger.triggers.Add(exit);
    }

    public void Refresh()
    {
        if (statusText == null || manager == null) return;
        statusText.text = $"TEPLINK | صاحب السايبر\nالوقت: {manager.GetTimeString()}\nالرصيد: {manager.cash:0} جنيه\nأجهزة الكمبيوتر: {manager.workingComputers}/{manager.computers}\nالجوع: {manager.hunger:0}%  العطش: {manager.thirst:0}%\nالطاقة: {manager.energy:0}%\nالسايبر: {(manager.shopOpen ? "مفتوح" : "مغلق")}";
    }

    public void RefreshClockAndNeeds() { Refresh(); }

    public void ShowNotice(string message)
    {
        noticeText.text = message;
        noticeUntil = Time.time + 4f;
        Refresh();
    }

    private void Update()
    {
        if (Time.time > noticeUntil && noticeText != null) noticeText.text = "";
    }

    public void OpenCyberPanel(TEPLINKComputer computer)
    {
        selectedComputer = computer;
        cyberPanel.SetActive(true);
        IsMenuOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cyberText.text = $"نظام إدارة سايبر TEPLINK\n\nالجهاز المحدد: PC-{computer.computerNumber:00}\nحالة الأجهزة: {manager.workingComputers}/{manager.computers} شغال\nإيرادات اليوم: تقديرية حسب عدد الأجهزة\nاشتراك الإنترنت: {manager.internetBill:0} جنيه\nكهرباء: {manager.electricityBill:0} جنيه\n\nاختار إجراء من الأزرار بالأسفل.";
    }

    private void CloseCyberPanel()
    {
        cyberPanel.SetActive(false);
        IsMenuOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowPauseHint() { pausePanel.SetActive(true); }
    public void HidePauseHint() { pausePanel.SetActive(false); }

    private void OnGUI()
    {
        // Keep the prototype usable without opening the cyber panel first.
        if (manager != null && !IsMenuOpen)
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width / 2 - 180, Screen.height - 42, 360, 30),
                "WASD للحركة | الماوس للنظر | E للتفاعل مع الكمبيوتر أو الطعام");
        }
    }
}
