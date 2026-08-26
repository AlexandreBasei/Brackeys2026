using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutoManager : PersistentSingleton<TutoManager>
{
    [SerializeField] private TutoData[] tutosData;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private Image[] tutoImages;
    [HideInInspector] public int currentDay;
    private Canvas tutoCanvas;
    void Start()
    {
        currentDay = GameManager.Instance.dayCount;
    }

    public void ShowTuto()
    {
        if (currentDay < 0 || currentDay >= tutosData.Length)
        {
            Debug.LogWarning("Invalid tuto index: " + currentDay);
            return;
        }

        titleText.text = tutosData[currentDay].title;
        bodyText.text = tutosData[currentDay].text;

        for (int i = 0; i < tutoImages.Length; i++)
        {
            if (i < tutosData[currentDay].images.Length)
            {
                tutoImages[i].sprite = tutosData[currentDay].images[i];
                tutoImages[i].gameObject.SetActive(true);
            }
            else
            {
                tutoImages[i].gameObject.SetActive(false);
            }
        }

        GetComponent<Canvas>().enabled = true;
    }

    public void HideTuto()
    {
        GetComponent<Canvas>().enabled = false;
        AudioManager.Instance.PlaySFX(AudioManager.Instance.closeTuto);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameManager.Instance.NextDay();
    }

    public void NextTuto()
    {
        currentDay++;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
