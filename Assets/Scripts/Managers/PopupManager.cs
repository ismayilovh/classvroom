using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [SerializeField] private GameObject popup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Slider timeBarSlider;
    [SerializeField] private float popupDuration = 2f;

    private bool _isEnabled = false;
    private float time;

    private void Awake()
    {
        if (Instance) { Destroy(gameObject); return; } Instance = this;
        popup.SetActive(false);
    }

    public void ShowPopup(string message)
    {
        _isEnabled = true;
        popup.SetActive(true);
        messageText.text = message;
        time = popupDuration;
    }

    public void HidePopup()
    {
        _isEnabled = false;
        popup.SetActive(false);
    }

    private void Update()
    {
        if(_isEnabled)
        {
            if(time > 0)
            {
                time -= Time.deltaTime;

                timeBarSlider.value = popupDuration - time;
            }
            else
            {
                time = 0;
                HidePopup();
            }
        }
    }
}
