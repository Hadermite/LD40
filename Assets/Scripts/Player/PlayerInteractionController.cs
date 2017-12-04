using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteractionController : MonoBehaviour {

    public BuyerController buyer;
    public Transform canvas;
    public Text moneyText;
    public Transform sellDrugsDialog;
    public Transform messageDialog;
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
    private float addMessageTimer = 0;
    private Inventory openedInventory;
    private Inventory messagesInventory;

    private void Start() {
        Money = 100;
        GameController.totalTimer = 0;
        GameController.totalEarnedMoney = 0;

        weaponShopItem = new ShopItem(
            "Weapon",
            new string[] { "Handgun", "Submachine Gun", "Rifle" },
            new int[] { 100, 300, 1000 });
    }

    private void Update() {
        GameController.totalTimer += Time.deltaTime;
        fireTimer -= Time.deltaTime;
        addMessageTimer -= Time.deltaTime;
        if (addMessageTimer <= 0) {
            addMessageTimer = 10f;
            string name = names[Random.Range(0, names.Length - 1)];
            DrugType type = DrugType.drugTypes[Random.Range(0, DrugType.drugTypes.Count)];
            int amount = Random.Range(2, 10);
            int offeredPrice = (int)(type.PurchasePrice * amount * Random.Range(1f, 2f));
            Message message = new Message(name, type, amount, offeredPrice);
            messages.Add(message);
            if (messagesInventory != null) {
                messagesInventory.Close();
                OpenMessages();
            }
        }
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
                bullet.tag = "PlayerBullet";
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
                case "Buyer":
                    if (buyer.Message == null) break;
                    if (buyer.IsUndercoverPolice) break;
                    sellDrugsDialog.gameObject.SetActive(true);
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (insideGameObjects.Contains(collider.gameObject)) {
            Debug.LogError("Entered Game Object that is already added!");
            return;
        }
        if (collider.transform.tag == "PlayerBullet") return;
        if (collider.transform.tag == "BuyerBullet") {
            SceneManager.LoadScene("GameOverScene");
            return;
        }
        insideGameObjects.Add(collider.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if (collider.transform.tag == "PlayerBullet" || collider.transform.tag == "BuyerBullet") return;
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
        messagesInventory = CreateInventory("Messages");

        for (int i = 0; i < messages.Count; i++) {
            Message message = messages[i];
            GameObject messageObject = Instantiate(messagePrefab, messagesInventory.GetContent(), false);
            Text sender = messageObject.transform.Find("Sender").GetComponent<Text>();
            Text text = messageObject.transform.Find("Text").GetComponent<Text>();
            Button discardOffer = messageObject.transform.Find("Discard Offer").GetComponent<Button>();
            Button planMeeting = messageObject.transform.Find("Plan Meeting").GetComponent<Button>();

            sender.text = message.SenderName;
            text.text = message.Text;
            discardOffer.onClick.AddListener(() => {
                messages.Remove(message);
                messagesInventory.Close();
                OpenMessages();
            });
            planMeeting.onClick.AddListener(() => {
                messagesInventory.Close();
                if (buyer.Message != null) {
                    ShowMessageDialog("Meeting Already Planned", "You have already planned a meeting with another person.");
                    return;
                }
                buyer.PlanMeeting(message);
                messages.Remove(message);
            });
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

    private Inventory CreateInventory(string title) {
        if (openedInventory != null) openedInventory.Close();
        GameObject inventoryObject = Instantiate(inventoryPrefab, canvas, false);
        Text inventoryTitle = inventoryObject.transform.Find("Title").GetComponent<Text>();
        inventoryTitle.text = title;
        Inventory inventory = inventoryObject.GetComponent<Inventory>();
        openedInventory = inventory;
        return inventory;
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
                Money -= shopItem.Price;
                inventory.Close();
                shopItem.Purchase();
            }
        });
    }

    public void SellDrugs() {
        if (buyer.Message == null) {
            Debug.LogError("The buyer's message is null!");
            return;
        }

        List<DrugType> drugTypes = new List<DrugType>();
        List<int> drugTypeCounts = new List<int>();
        foreach (Drug drug in drugs) {
            if (drugTypes.Contains(drug.Type)) {
                drugTypeCounts[drugTypes.IndexOf(drug.Type)]++;
            }
            else {
                drugTypes.Add(drug.Type);
                drugTypeCounts.Add(1);
            }
        }

        DrugType requestedDrugType = buyer.Message.DrugType;
        if (!drugTypes.Contains(requestedDrugType)) {
            ShowMessageDialog("Not Enough Stock", "You don't have the requested drug!");
            return;
        }
        int index = drugTypes.IndexOf(requestedDrugType);
        if (drugTypeCounts[index] < buyer.Message.Amount) {
            ShowMessageDialog("Not Enough Stock", "You don't have enough of the requested drug!");
            return;
        }

        int numDrugsToRemove = buyer.Message.Amount;
        Drug[] drugsToRemove = new Drug[numDrugsToRemove];
        for (int i = 0; i < drugs.Count; i++) {
            Drug drug = drugs[i];
            if (drug.Type == requestedDrugType) {
                drugsToRemove[drugsToRemove.Length - numDrugsToRemove] = drug;
                numDrugsToRemove--;
                if (numDrugsToRemove == 0) break;
            }
        }

        foreach (Drug drug in drugsToRemove) {
            drugs.Remove(drug);
        }

        int earnedMoney = buyer.Message.OfferedPrice;
        GameController.totalEarnedMoney += earnedMoney;
        if (GameController.totalEarnedMoney >= GameController.MoneyNeededToWin) {
            SceneManager.LoadScene("GameOverScene");
            return;
        }
        Money += earnedMoney;
        buyer.DismissBuyer();
    }

    private void ShowMessageDialog(string title, string message) {
        messageDialog.Find("Title").GetComponent<Text>().text = title;
        messageDialog.Find("Text").GetComponent<Text>().text = message;
        messageDialog.gameObject.SetActive(true);
    }

    private string[] names = {
        "Arnoldo",
        "Ellena",
        "Devora",
        "Vern",
        "Tricia",
        "Vergie",
        "Janella",
        "Elsie",
        "Ilona",
        "Edgar",
        "Alejand",
        "Patria",
        "Estelle",
        "Cherily",
        "Cory",
        "Lenard",
        "Nguyet",
        "Gina",
        "Genaro",
        "Dan",
        "Britt",
        "Nikole",
        "Marquer",
        "Rosana",
        "Garry",
        "Jeanice",
        "Pat",
        "Issac",
        "Farrah",
        "Elise",
        "Jen",
        "Selena",
        "Wilson",
        "Magdale",
        "Romeo",
        "Kaycee",
        "Aldo",
        "Mendy",
        "Twana",
        "Terisa",
        "Krysta",
        "Marcie",
        "Man",
        "Arianna",
        "Dulcie",
        "Jesica",
        "Michael",
        "Giselle",
        "Vanessa",
        "Emilia"
    };
}
