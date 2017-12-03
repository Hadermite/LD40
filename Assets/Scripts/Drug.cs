public class Drug {

    public DrugType Type { get; private set; }

    public Drug(DrugType type) {
        Type = type;
    }

    // TODO: Check this again, maybe change system.
    public static int GetDrugSentence(DrugType type, int count) {
        return type.Intensity * type.PurchasePrice;
    }
}
