using UnityEngine;
using UnityEngine.UI;

public class BuyerController : MonoBehaviour {

    private const float FireRate = 1;

    public Transform buyer;
    public Transform player;
    public Transform plannedMeetingPanel;
    public Transform meetingPlannedDialog;
    public GameObject bulletPrefab;

    public Message Message { get; private set; }
    public bool IsUndercoverPolice { get; private set; }

    private float fireTimer = 0;

    private void Start() {
        DismissBuyer();
    }

    private void Update() {
        if (Message == null) return;
        if (!IsUndercoverPolice) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer < 0) fireTimer = 0;
        if (fireTimer != 0) return;

        if (Vector3.Distance(buyer.transform.position, player.transform.position) < 5) {
            Vector3 relativePosition = player.transform.position - buyer.transform.position;
            float angle = Mathf.Atan2(relativePosition.x, relativePosition.y);
            float degrees = angle * Mathf.Rad2Deg;
            degrees *= -1;
            GameObject bullet = Instantiate(bulletPrefab, transform, false);
            bullet.tag = "BuyerBullet";
            bullet.transform.Rotate(0, 0, degrees);
            bullet.transform.position = buyer.transform.position;
            fireTimer = 1 / FireRate;
        }
    }

    public void PlanMeeting(Message message) {
        Message = message;
        buyer.gameObject.SetActive(true);
        meetingPlannedDialog.gameObject.SetActive(true);

        plannedMeetingPanel.Find("Drug Type").GetComponent<Text>().text = $"Drug Type: {Message.DrugType.Name}";
        plannedMeetingPanel.Find("Amount").GetComponent<Text>().text = $"Amount: {Message.Amount}";
        plannedMeetingPanel.gameObject.SetActive(true);

        float undercoverPoliceChance = Message.DrugType.PurchasePrice * Message.DrugType.Intensity / 2000f;
        float sellProfitPart = (float)(Message.OfferedPrice / Message.Amount) / Message.DrugType.PurchasePrice - 1f;
        undercoverPoliceChance += sellProfitPart / 5;
        if (undercoverPoliceChance >= 1) {
            undercoverPoliceChance = 0.9f;
        }
        IsUndercoverPolice = Random.Range(0f, 1f) <= undercoverPoliceChance;
        Debug.Log($"Undercover police chance: {undercoverPoliceChance}, is: {IsUndercoverPolice}");
    }

    public void DismissBuyer() {
        Message = null;
        buyer.gameObject.SetActive(false);
        plannedMeetingPanel.gameObject.SetActive(false);
    }
}
