using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string NUMBER_POPUP = "NumberPopUp";
    [SerializeField] private TextMeshProUGUI countDownText;
    private Animator animator;
    private int previousCountDownNumber;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        KitchenGameManager.Instance.OnStateChanged += KitchenManager_OnStateChanged;
        Hide();
    }

    private void KitchenManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (KitchenGameManager.Instance.IsCountdownToStartActive())
        {
            Show();
        }
        else
        {
            Hide();
        }
        
    }
    private void Update()
    {
        int countDownNumber = Mathf.CeilToInt(KitchenGameManager.Instance.GetCountdownToStartTimer());
        countDownText.text = countDownNumber.ToString();
        if (previousCountDownNumber == countDownNumber)
        {
            previousCountDownNumber = countDownNumber;
            animator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayCountDownSound();
        }
        // the reason why we use ToString here is because the countdown text is a string but timer is a float so we use ToString to convert it into the string
        //Mathf.Ceil this is used so that rather then decimal countdown we have normal whole no countdown (3-2-1)
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
