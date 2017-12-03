using UnityEngine;

public class Inventory : MonoBehaviour {

	public void Close() {
        Destroy(gameObject);
    }

    public Transform GetContent() {
        return transform.Find("Scroll View/Viewport/Content");
    }
}
