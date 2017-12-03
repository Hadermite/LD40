using System.Collections.Generic;

public class DrugType {

    public static readonly List<DrugType> drugTypes = new List<DrugType>();

    public static readonly DrugType Shrooms = new DrugType("Shrooms", 5, 2);
    public static readonly DrugType Weed = new DrugType("Weed", 10, 3);
    public static readonly DrugType Ecstasy = new DrugType("Ecstasy", 10, 4);
    public static readonly DrugType LSD = new DrugType("LSD", 20, 5);
    public static readonly DrugType Cocaine = new DrugType("Cocaine", 100, 10);
    public static readonly DrugType Meth = new DrugType("Meth", 100, 10);

    static DrugType() {
        drugTypes.Add(Shrooms);
        drugTypes.Add(Weed);
        drugTypes.Add(Ecstasy);
        drugTypes.Add(LSD);
        drugTypes.Add(Cocaine);
        drugTypes.Add(Meth);
    }


    public string Name { get; private set; }

    // The price the player has to pay for the drug per gram.
    public int PurchasePrice { get; private set; }

    // How "dangerous" the drug is. This is affects the sentence you will get.
    public int Intensity { get; private set; }

    private DrugType(string name, int purchasePrice, int intensity) {
        Name = name;
        PurchasePrice = purchasePrice;
        Intensity = intensity;
    }
}