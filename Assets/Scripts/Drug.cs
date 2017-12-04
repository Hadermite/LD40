public class Drug {

    public DrugType Type { get; private set; }

    public Drug(DrugType type) {
        Type = type;
    }
}
