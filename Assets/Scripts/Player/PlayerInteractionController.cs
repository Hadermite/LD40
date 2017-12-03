using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractionController : MonoBehaviour {

    public new Camera camera;
    public Transform canvas;
    public Text moneyText;
    public GameObject bulletPrefab;
    public GameObject inventoryPrefab;
    public GameObject inventoryItemPrefab;
    public GameObject shopItemPrefab;
    public GameObject messagePrefab;
    public GameObject drugMarketItemPrefab;

    private int _money;
    private int Money {
        get {
            return _money;
        }
        set {
            moneyText.text = $"Money: {value} €";
            _money = value;
        }
    }

    private List<GameObject> insideGameObjects = new List<GameObject>();
    private List<Drug> drugs = new List<Drug>();
    private List<Message> messages = new List<Message>();
    private ShopItem weaponShopItem;
    private float FireRate {
        get {
            return weaponShopItem.PurchasedTier;
        }
    }
    private float fireTimer = 0;

    private void Start() {
        Money = 100000;

        // FIXME: Temporary code, remove when system is implemented.
        messages.Add(new Message("Alex", DrugType.Cocaine, 5, 500));
        messages.Add(new Message("Mike", DrugType.Meth, 40, 50000));

        weaponShopItem = new ShopItem(
            "Weapon",
            new string[] { "Handgun", "Submachine Gun", "Rifle" },
            new int[] { 100, 300, 1000 });
    }

    private void Update() {
        fireTimer -= Time.deltaTime;
        if (fireTimer < 0) fireTimer = 0;
        if (Input.GetMouseButton(0) && weaponShopItem.PurchasedTier > 0) {
            if (fireTimer == 0 && !EventSystem.current.IsPointerOverGameObject()) {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 centerScreen = new Vector3(Screen.width / 2, Screen.height / 2);
                Vector3 relativeMousePosition = mousePosition - centerScreen;
                float angle = Mathf.Atan2(relativeMousePosition.x, relativeMousePosition.y);
                float degrees = angle * Mathf.Rad2Deg;
                degrees *= -1;
                GameObject bullet = Instantiate(bulletPrefab, transform.parent, false);
                bullet.transform.Rotate(0, 0, degrees);
                bullet.transform.position = transform.position;
                fireTimer = 1 / FireRate;
            }
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            OpenPlayerInventory();
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            OpenBuyMenu();
        }

        if (Input.GetKeyDown(KeyCode.M)) {
            OpenMessages();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            if (insideGameObjects.Count == 0) return;

            GameObject insideGameObject = insideGameObjects[insideGameObjects.Count - 1];

            switch (insideGameObject.tag) {
                case "DrugMarket":
                    OpenMarketInventory();
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (insideGameObjects.Contains(collider.gameObject)) {
            Debug.LogError("Entered Game Object that is already added!");
            return;
        }
        insideGameObjects.Add(collider.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (!insideGameObjects.Contains(collider.gameObject)) {
            Debug.LogError("Exited Game Object that was never added!");
            return;
        }
        insideGameObjects.Remove(collider.gameObject);
    }

    private void OpenPlayerInventory() {
        Inventory inventory = CreateInventory("Inventory");
        List<DrugType> drugTypes = new List<DrugType>();
        List<int> drugTypeCounts = new List<int>();
        foreach (Drug drug in drugs) {
            if (drugTypes.Contains(drug.Type)) {
                drugTypeCounts[drugTypes.IndexOf(drug.Type)]++;
            } else {
                drugTypes.Add(drug.Type);
                drugTypeCounts.Add(1);
            }
        }

        for (int i = 0; i < drugTypes.Count; i++) {
            DrugType type = drugTypes[i];
            int count = drugTypeCounts[i];

            GameObject inventoryItem = Instantiate(inventoryItemPrefab, inventory.GetContent(), false);
            inventoryItem.transform.name = type.Name;
            inventoryItem.transform.Find("Drug Type").GetComponent<Text>().text = type.Name;
            inventoryItem.transform.Find("Count").GetComponent<Text>().text = count.ToString();
        }
    }

    private void OpenBuyMenu() {
        Inventory inventory = CreateInventory("Shop");
        AddShopItemToInventory(weaponShopItem, inventory);
    }

    private void OpenMessages() {
        Inventory inventory = CreateInventory("Messages");

        foreach (Message message in messages) {
            GameObject messageObject = Instantiate(messagePrefab, inventory.GetContent(), false);
            Text sender = messageObject.transform.Find("Sender").GetComponent<Text>();
            Text text = messageObject.transform.Find("Text").GetComponent<Text>();
            Button planMeeting = messageObject.transform.Find("Plan Meeting").GetComponent<Button>();

            sender.text = message.SenderName;
            text.text = message.Text;
        }
    }

    private void OpenMarketInventory() {
        Inventory inventory = CreateInventory("Buy Drugs");
        List<DrugType> drugTypesSorted = GetDrugTypesSortedByPurchasePrice();
        for (int i = 0; i < drugTypesSorted.Count; i++) {
            DrugType type = drugTypesSorted[i];
            GameObject drugMarketItem = Instantiate(drugMarketItemPrefab, inventory.GetContent(), false);

            drugMarketItem.transform.Find("Drug Type").GetComponent<Text>().text = type.Name;
            drugMarketItem.transform.Find("Purchase Price").GetComponent<Text>().text = type.PurchasePrice + " € / gram";

            Button buy = drugMarketItem.transform.Find("Buy").GetComponent<Button>();
            buy.onClick.AddListener(() => {
                if (Money >= type.PurchasePrice) {
                    Money -= type.PurchasePrice;
                    drugs.Add(new Drug(type));
                }
            });
        }
    }

    // TODO: Make sure only one of every inventory type can be opened.
    // Or maybe only one inventory of any kind.
    private Inventory CreateInventory(string title) {
        GameObject inventory = Instantiate(inventoryPrefab, canvas, false);
        Text inventoryTitle = inventory.transform.Find("Title").GetComponent<Text>();
        inventoryTitle.text = title;
        return inventory.GetComponent<Inventory>();
    }

    private List<DrugType> GetDrugTypesSortedByPurchasePrice() {
        List<DrugType> drugTypes = new List<DrugType>(DrugType.drugTypes);
        List<DrugType> drugTypesSorted = new List<DrugType>();
        while (drugTypes.Count > 0) {
            int lowestNumber = int.MaxValue;
            int lowestIndex = 0;
            for (int i = 0; i < drugTypes.Count; i++) {
                DrugType type = drugTypes[i];
                if (type.PurchasePrice < lowestNumber) {
                    lowestNumber = type.PurchasePrice;
                    lowestIndex = i;
                }
            }
            drugTypesSorted.Add(drugTypes[lowestIndex]);
            drugTypes.RemoveAt(lowestIndex);
        }

        return drugTypesSorted;
    }

    private void AddShopItemToInventory(ShopItem shopItem, Inventory inventory) {
        GameObject shopItemObject = Instantiate(shopItemPrefab, inventory.GetContent(), false);
        shopItemObject.transform.Find("Name").GetComponent<Text>().text = shopItem.Name;
        string price = "";
        if (shopItem.Price != int.MaxValue) {
            price = shopItem.Price + " €";
        }
        shopItemObject.transform.Find("Price").GetComponent<Text>().text = price;
        Button buy = shopItemObject.transform.Find("Buy").GetComponent<Button>();
        if (price == "") {
            buy.enabled = false;
            return;
        }
        buy.onClick.AddListener(() => {
            if (Money >= shopItem.Price) {
                inventory.Close();
                shopItem.Purchase();
            }
        });
    }
}
