using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    private void Start() {
        if (SceneManager.GetActiveScene().name == "GameOverScene") {
            if (GameController.totalEarnedMoney >= GameController.MoneyNeededToWin) {
                transform.Find("Title").GetComponent<Text>().text = "You Won!";
                string text = $"Time taken to earn {GameController.MoneyNeededToWin} €: {(int)GameController.totalTimer} seconds!";
                transform.Find("Text").GetComponent<Text>().text = text;
            }
        }
    }

    public void LoadGame() {
        SceneManager.LoadScene("GameScene");
    }
}
