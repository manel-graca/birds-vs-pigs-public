using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class G_Ball_TutorialHandler : MonoBehaviour
{
    [SerializeField] GameObject[] tutorials;
    [SerializeField] GameObject startTutorialButton;
    [SerializeField] GameObject nextButton;


    int index;

    G_Ball_GameManager manager;

    private void Start()
    {
        startTutorialButton.GetComponent<Button>().onClick.AddListener(StartTutorial);
        index = 0;
    }

    private void OnEnable()
    {
        manager = FindObjectOfType<G_Ball_GameManager>();

        manager.EnableGlobalInputBlocker(true);
        manager.isInputBlock = true;
        PlayerPrefs.SetString("tutorial", "running");
    }

    public void IterateThroughTutorials()
    {
        index += 1;
        index = Mathf.Clamp(index, 1, 3);
        var buttonText = nextButton.GetComponentInChildren<TextMeshProUGUI>();

        switch (index)
        {
            case 1:
                buttonText.text = "NEXT";
                ShowTutorialTwo();
                break;
            case 2:
                ShowTutorialThree();
                buttonText.text = "PLAY";
                break;
            case 3:
                manager.EnableGlobalInputBlocker(false);
                manager.isInputBlock = false;
                PlayerPrefs.SetString("tutorial", "done");
                Destroy(gameObject);
                break;
        }
    }

    public void StartTutorial()
    {
        startTutorialButton.SetActive(false);
        nextButton.SetActive(true);
        ShowTutorialOne();
    }

    public void ShowTutorialOne()
    {
        var transform = tutorials[0].transform;
        transform.localScale = new Vector2(0f, 0f);

        transform.LeanScale(Vector2.one, 0.6f).setEaseOutBack().delay = 0.01f;

        tutorials[0].SetActive(true);
    }


    public void ShowTutorialTwo()
    {
        var transform = tutorials[1].transform;
        transform.localScale = new Vector2(0f, 0f);

        transform.LeanScale(Vector2.one, 0.6f).setEaseOutBack().delay = 0.01f;

        tutorials[1].SetActive(true);
    }

    public void ShowTutorialThree()
    {
        var transform = tutorials[2].transform;
        transform.localScale = new Vector2(0f, 0f);

        transform.LeanScale(Vector2.one, 0.6f).setEaseOutBack().delay = 0.01f;

        tutorials[2].SetActive(true);
    }
}