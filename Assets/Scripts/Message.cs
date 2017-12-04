using UnityEngine;

public class Message {
    
    public string SenderName { get; private set; }
    public string Text { get; private set; }
    public DrugType DrugType { get; private set; }
    public int Amount { get; private set; }
    public int OfferedPrice { get; private set; }

    public Message(string senderName, DrugType drugType, int amount, int offeredPrice) {
        SenderName = senderName;
        DrugType = drugType;
        Amount = amount;
        OfferedPrice = offeredPrice;
        Text = GenerateText();
    }

    private string GenerateText() {
        int index = Random.Range(0, Phrases.Length);
        string result = Phrases[index];
        result = result.Replace("[AMOUNT]", Amount.ToString());
        result = result.Replace("[TYPE]", DrugType.Name);
        result = result.Replace("[PRICE]", OfferedPrice.ToString());
        return result;
    }

    static Message() {
        // Check so all phrases are correct at startup
        for (int i = 0; i < Phrases.Length; i++) {
            string result = Phrases[i];
            string old = result;
            result = result.Replace("[AMOUNT]", "");
            if (old == result) {
                Debug.LogError($"Message phrase at index {i} does not contain [AMOUNT]!");
            }
            old = result;
            result = result.Replace("[TYPE]", "");
            if (old == result) {
                Debug.LogError($"Message phrase at index {i} does not contain [TYPE]!");
            }
            old = result;
            result = result.Replace("[PRICE]", "");
            if (old == result) {
                Debug.LogError($"Message phrase at index {i} does not contain [PRICE]!");
            }
        }
    }

    private static readonly string[] Phrases = {
        "Hey man, I'm interested in [AMOUNT] grams [TYPE] for [PRICE] €. Do you have in stock?",
        "[AMOUNT]g [TYPE] for [PRICE], u got any?",
        "sup m8, u havin any [TYPE], want [AMOUNT] for [PRICE]",
        "hey whatup, [AMOUNT] of [TYPE] for [PRICE], where can we meet?",
    };
}
