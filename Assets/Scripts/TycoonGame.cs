using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TycoonGame : MonoBehaviour
{
    [Serializable] private class SaveData
    {
        public long cash = 10000;
        public int stock = 10, orders = 0, marketing = 1, warehouse = 1, automation = 0, price = 799;
        public long revenue = 0;
    }

    private SaveData data;
    private TextMeshProUGUI stats, message;
    private TMP_InputField priceField;
    private float customerTimer;
    private const int ProductCost = 350, ShippingCost = 70;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Load();
        BuildUI();
        Refresh("Welcome, CEO! Buy stock and ship orders to grow your store.");
    }

    private void Update()
    {
        customerTimer += Time.deltaTime;
        float interval = Mathf.Max(5f, 18f - data.marketing * 1.5f - data.automation * 2f);
        if (customerTimer >= interval)
        {
            customerTimer = 0;
            if (UnityEngine.Random.value < Mathf.Clamp01(.35f + data.marketing * .08f)) ProcessOrder(false);
        }
    }

    private void BuildUI()
    {
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920); scaler.matchWidthOrHeight = .5f;

        GameObject bg = new GameObject("Background", typeof(Image)); bg.transform.SetParent(canvas.transform, false);
        bg.GetComponent<Image>().color = new Color32(10, 16, 32, 255); Stretch(bg.GetComponent<RectTransform>());
        TextMeshProUGUI title = MakeText(canvas.transform, "DROPSHIP\nTYCOON", 64, new Color32(80, 230, 255, 255), TextAlignmentOptions.Center);
        Place(title.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -150), new Vector2(900, 210));
        stats = MakeText(canvas.transform, "", 32, Color.white, TextAlignmentOptions.Left);
        Place(stats.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -375), new Vector2(900, 320));
        message = MakeText(canvas.transform, "", 26, new Color32(255, 205, 100, 255), TextAlignmentOptions.Center);
        Place(message.rectTransform, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(0, -590), new Vector2(900, 130));

        MakeButton(canvas.transform, "BUY 10 PRODUCTS  •  ₹3,500", 30, new Vector2(.5f, 1), new Vector2(0, -745), new Vector2(850, 110), () => BuyStock(10));
        MakeButton(canvas.transform, "FULFILL CUSTOMER ORDER", 30, new Vector2(.5f, 1), new Vector2(0, -875), new Vector2(850, 110), () => ProcessOrder(true));
        MakeButton(canvas.transform, "UPGRADE MARKETING  •  ₹2,500", 27, new Vector2(.5f, 1), new Vector2(0, -1005), new Vector2(850, 100), UpgradeMarketing);
        MakeButton(canvas.transform, "EXPAND WAREHOUSE  •  ₹4,000", 27, new Vector2(.5f, 1), new Vector2(0, -1125), new Vector2(850, 100), UpgradeWarehouse);
        MakeButton(canvas.transform, "AUTOMATION  •  ₹6,000", 27, new Vector2(.5f, 1), new Vector2(0, -1245), new Vector2(850, 100), UpgradeAutomation);

        TextMeshProUGUI label = MakeText(canvas.transform, "SELLING PRICE (₹)", 25, Color.white, TextAlignmentOptions.Center);
        Place(label.rectTransform, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(-160, 300), new Vector2(380, 60));
        GameObject input = new GameObject("Price Input", typeof(Image), typeof(TMP_InputField)); input.transform.SetParent(canvas.transform, false);
        input.GetComponent<Image>().color = new Color32(30, 43, 67, 255);
        priceField = input.GetComponent<TMP_InputField>();
        TextMeshProUGUI inputText = MakeText(input.transform, "Input Text", 32, Color.white, TextAlignmentOptions.Center);
        Stretch(inputText.rectTransform); priceField.textComponent = inputText; priceField.text = data.price.ToString();
        Place(input.GetComponent<RectTransform>(), new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(250, 300), new Vector2(300, 70));
        MakeButton(canvas.transform, "APPLY PRICE", 26, new Vector2(.5f, 0), new Vector2(0, 190), new Vector2(500, 85), ApplyPrice);
        MakeButton(canvas.transform, "SAVE GAME", 24, new Vector2(.5f, 0), new Vector2(0, 85), new Vector2(500, 75), () => { Save(); Refresh("Progress saved on this device."); });
    }

    private TextMeshProUGUI MakeText(Transform parent, string name, float size, Color color, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(name, typeof(TextMeshProUGUI)); go.transform.SetParent(parent, false);
        TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>(); t.fontSize = size; t.color = color; t.alignment = align;
        t.textWrappingMode = TextWrappingModes.Normal; t.text = name; return t;
    }

    private void MakeButton(Transform parent, string label, float size, Vector2 anchor, Vector2 offset, Vector2 dimensions, Action action)
    {
        GameObject go = new GameObject(label, typeof(Image), typeof(Button)); go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = new Color32(20, 105, 145, 255); Place(go.GetComponent<RectTransform>(), anchor, anchor, offset, dimensions);
        go.GetComponent<Button>().onClick.AddListener(() => action());
        TextMeshProUGUI t = MakeText(go.transform, "Label", size, Color.white, TextAlignmentOptions.Center); Stretch(t.rectTransform); t.text = label;
    }

    private static void Place(RectTransform rt, Vector2 min, Vector2 max, Vector2 pos, Vector2 size)
    { rt.anchorMin = min; rt.anchorMax = max; rt.anchoredPosition = pos; rt.sizeDelta = size; }
    private static void Stretch(RectTransform rt)
    { rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero; }

    private void BuyStock(int quantity)
    {
        long cost = (long)quantity * ProductCost;
        int capacity = 10 + (data.warehouse - 1) * 20;
        if (data.cash < cost) { Refresh("Not enough cash to buy stock."); return; }
        if (data.stock + quantity > capacity) { Refresh("Warehouse full! Expand storage first."); return; }
        data.cash -= cost; data.stock += quantity; Save(); Refresh("Stock delivered. Ready to ship orders!");
    }

    private void ProcessOrder(bool manual)
    {
        if (data.stock <= 0) { Refresh("Out of stock! Buy more products."); return; }
        int price = Mathf.Clamp(data.price, 100, 1000000);
        if (price > 1200 + data.marketing * 120 && UnityEngine.Random.value < .65f)
        { if (manual) Refresh("Customer declined the price. Try lowering it."); return; }
        data.stock--; data.orders++; data.cash += price - ShippingCost; data.revenue += price; Save();
        Refresh($"Order #{data.orders} shipped! Revenue ₹{price:N0} • Estimated profit ₹{price - ProductCost - ShippingCost:N0}");
    }

    private void UpgradeMarketing() { Upgrade(2500, () => data.marketing++, "Marketing upgraded! More customers can find your store."); }
    private void UpgradeWarehouse() { Upgrade(4000, () => data.warehouse++, "Warehouse expanded! Storage capacity +20."); }
    private void UpgradeAutomation() { Upgrade(6000, () => data.automation++, "Automation installed! Orders can arrive faster."); }
    private void Upgrade(int cost, Action action, string success)
    { if (data.cash < cost) { Refresh($"You need ₹{cost:N0} for this upgrade."); return; } data.cash -= cost; action(); Save(); Refresh(success); }

    private void ApplyPrice()
    {
        if (!int.TryParse(priceField.text, out int p) || p < 100 || p > 1000000) { Refresh("Enter a price between ₹100 and ₹1,000,000."); return; }
        data.price = p; Save(); Refresh($"Selling price set to ₹{p:N0}.");
    }

    private void Refresh(string msg)
    {
        if (stats != null) stats.text = $"CASH       ₹{data.cash:N0}\nSTOCK      {data.stock}/{10 + (data.warehouse - 1) * 20}\nORDERS     {data.orders:N0}\nREVENUE    ₹{data.revenue:N0}\nMARKETING  Lv.{data.marketing}\nWAREHOUSE  Lv.{data.warehouse}\nAUTOMATION Lv.{data.automation}";
        if (message != null) message.text = msg;
    }
    private void Save() { PlayerPrefs.SetString("dropship_save", JsonUtility.ToJson(data)); PlayerPrefs.Save(); }
    private void Load()
    {
        try { data = PlayerPrefs.HasKey("dropship_save") ? JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("dropship_save")) : new SaveData(); }
        catch { data = new SaveData(); }
        if (data == null) data = new SaveData();
    }
}