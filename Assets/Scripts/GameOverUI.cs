using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipesDeliverdText;
    [SerializeField] private TextMeshProUGUI countDownText;
    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenManager_OnStateChanged;
        Hide();
    }

    private void KitchenManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsGameOver())
        {
            recipesDeliverdText.text = DeliveryManager.Instance.GetsuccessfulReciepesAmount().ToString();
            Show();
        }
        else
        {
            Hide();
        }

    }
    private void Update()
    {
 
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }



}
