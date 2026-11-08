using UnityEngine;

public class TEPLINKGameManager : MonoBehaviour
{
    [Header("Prototype economy")]
    public float cash = 2500f;
    public int computers = 3;
    public int workingComputers = 3;
    public float internetBill = 180f;
    public float electricityBill = 120f;

    [Header("Needs (0-100)")]
    [Range(0, 100)] public float hunger = 85f;
    [Range(0, 100)] public float thirst = 85f;
    [Range(0, 100)] public float energy = 90f;

    [Header("World")]
    public Light sun;
    public TEPLINKPlayerController player;
    public TEPLINKUI ui;
    public float gameMinutesPerRealSecond = 2f;
    public int hour = 8;
    public int minute = 30;
    public bool shopOpen;
    public int customersToday;
    private float clockAccumulator;
    private float needsAccumulator;

    public void Initialize()
    {
        UpdateSun();
        ui?.Refresh();
    }

    private void Update()
    {
        clockAccumulator += Time.deltaTime * gameMinutesPerRealSecond;
        if (clockAccumulator >= 1f)
        {
            int elapsed = Mathf.FloorToInt(clockAccumulator);
            clockAccumulator -= elapsed;
            minute += elapsed;
            while (minute >= 60) { minute -= 60; hour++; }
            if (hour >= 24) hour -= 24;
            UpdateSun();
        }

        needsAccumulator += Time.deltaTime;
        if (needsAccumulator >= 10f)
        {
            needsAccumulator = 0f;
            hunger = Mathf.Max(0, hunger - 1.2f);
            thirst = Mathf.Max(0, thirst - 1.8f);
            energy = Mathf.Max(0, energy - 0.7f);
        }

        bool shouldBeOpen = (hour >= 9 && hour < 24);
        if (shopOpen != shouldBeOpen)
        {
            shopOpen = shouldBeOpen;
            ui?.ShowNotice(shopOpen ? "السايبر فتح — يوم شغل جديد!" : "السايبر قفل. راجع حساباتك.");
        }
        ui?.RefreshClockAndNeeds();
    }

    private void UpdateSun()
    {
        if (sun == null) return;
        float normalized = (hour + minute / 60f) / 24f;
        sun.transform.rotation = Quaternion.Euler((normalized * 360f) - 90f, -25f, 0);
        float daylight = Mathf.Clamp01(Mathf.Sin(normalized * Mathf.PI * 2f - Mathf.PI / 2f) * 0.8f + 0.35f);
        sun.intensity = Mathf.Lerp(0.08f, 1.15f, daylight);
        RenderSettings.ambientLight = Color.Lerp(new Color(0.08f, 0.1f, 0.2f), new Color(0.65f, 0.62f, 0.55f), daylight);
    }

    public string GetTimeString() => $"{hour:00}:{minute:00}";

    public void BuyComputer()
    {
        const float price = 900f;
        if (cash < price) { ui?.ShowNotice("فلوسك مش كفاية لشراء كمبيوتر جديد."); return; }
        cash -= price;
        computers++;
        workingComputers++;
        ui?.ShowNotice("اشتريت كمبيوتر جديد! إجمالي الأجهزة: " + computers);
        ui?.Refresh();
    }

    public void BuyFood()
    {
        if (cash < 35f) { ui?.ShowNotice("معاكش فلوس كفاية للأكل."); return; }
        cash -= 35f;
        hunger = Mathf.Min(100, hunger + 35);
        ui?.ShowNotice("أكلت سندوتش فول وطعمية بـ ٣٥ جنيه.");
        ui?.Refresh();
    }

    public void BuyDrink()
    {
        if (cash < 15f) { ui?.ShowNotice("معاكش فلوس كفاية للشرب."); return; }
        cash -= 15f;
        thirst = Mathf.Min(100, thirst + 40);
        ui?.ShowNotice("اشتريت مية ساقعة بـ ١٥ جنيه.");
        ui?.Refresh();
    }

    public void PayInternet()
    {
        if (cash < internetBill) { ui?.ShowNotice("الرصيد مش كفاية لدفع الإنترنت."); return; }
        cash -= internetBill;
        ui?.ShowNotice("تم دفع اشتراك الإنترنت: " + internetBill + " جنيه.");
        ui?.Refresh();
    }

    public void CloseDay()
    {
        float revenue = workingComputers * Random.Range(70f, 125f);
        cash += revenue;
        customersToday += Random.Range(4, 13);
        ui?.ShowNotice($"قفلنا اليوم. دخل تقديري {revenue:0} جنيه.");
        ui?.Refresh();
    }
}
