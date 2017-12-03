using UnityEngine;
public class ShopItem {

    private string[] names;
    private int[] prices;
    
    public string Type { get; private set; }
    public int PurchasedTier { get; private set; }
    public string Name {
        get {
            if (PurchasedTier >= prices.Length) return $"{Type}: Max Tier";
            return names[PurchasedTier];
        }
    }
    public int Price {
        get {
            if (PurchasedTier >= prices.Length) return int.MaxValue;
            return prices[PurchasedTier];
        }
    }

    public ShopItem(string type, string[] names, int[] prices) {
        Type = type;
        this.names = names;
        this.prices = prices;
        if (names.Length != prices.Length) {
            Debug.LogError($"Names and prices arrays are different sizes for shop item: {Type}!");
        }
        PurchasedTier = 0;
    }

    public void Purchase() {
        if (PurchasedTier == names.Length) {
            Debug.LogError($"Tried to purchase a higher tier than there are names for shop item: {Type}!");
            return;
        }
        PurchasedTier++;
    }
}
