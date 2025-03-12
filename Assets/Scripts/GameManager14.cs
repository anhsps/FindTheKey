using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Linq;

public class GameManager14 : Singleton<GameManager14>
{
    public static int level;
    [SerializeField] private TextMeshProUGUI starText;
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private Button retryBtn;
    [SerializeField] private GameObject nextBtn_win;
    [SerializeField] private GameObject winMenu, lvMenu;
    [SerializeField] private RectTransform winPanel, lvPanel;
    [SerializeField] private float topPosY = 250f, middlePosY, tweenDuration = 0.3f;
    private int maxLV = 21;

    [HideInInspector] public int star, maxStar;

    protected override void Awake()
    {
        base.Awake();
        level = PlayerPrefs.GetInt("CurrentLevel", 1);
        LoadLevel(level);

        maxStar = GameObject.FindGameObjectsWithTag("Star").Length;
        if (starText) starText.text = star + "/" + maxStar;
    }

    async void Start()
    {
        if (retryBtn)
        {
            retryBtn.interactable = false;
            Invoke(nameof(ActiveButton), 0.5f);
        }

        await HidePanel(winMenu, winPanel);
        await HidePanel(lvMenu, lvPanel);
    }

    private void LoadLevel(int levelIndex)
    {
        if (levelIndex < 1 || levelIndex > maxLV) levelIndex = 1;

        if (levelIndex == maxLV && nextBtn_win) nextBtn_win.SetActive(false);

        PlayerPrefs.SetInt("CurrentLevel", levelIndex);

        if (lvText) lvText.text = "LEVEL " + (levelIndex < 10 ? "0" + levelIndex : levelIndex);
    }

    private void ActiveButton() => retryBtn.interactable = true;

    public void Home() => SceneManager.LoadScene("Home");
    public void StartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void NextLV()
    {
        PlayerPrefs.SetInt("CurrentLevel", level + 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameWin()
    {
        UnlockNextLevel();
        OpenMenu(winMenu, winPanel, 2);
    }

    private void OpenMenu(GameObject menu, RectTransform panel, int soundIndex)
    {
        SoundManager14.Instance.PlaySound(soundIndex);
        ShowPanel(menu, panel);
    }

    public void UnlockNextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (level >= unlockedLevel && level < maxLV)
            PlayerPrefs.SetInt("UnlockedLevel", level + 1);
    }

    public void SetCurrentLV(int levelIndex)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene(levelIndex.ToString());
    }

    private void ShowPanel(GameObject menu, RectTransform panel)
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
        menu.GetComponent<CanvasGroup>().DOFade(1, tweenDuration).SetUpdate(true);
        panel.DOAnchorPosY(middlePosY, tweenDuration).SetUpdate(true);
    }

    private async Task HidePanel(GameObject menu, RectTransform panel)
    {
        if (menu == null || panel == null) return;

        menu.GetComponent<CanvasGroup>().DOFade(0, tweenDuration).SetUpdate(true);
        await panel.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
        if (menu) menu.SetActive(false);
    }

    public void UpdateStar()
    {
        star++;
        starText.text = star + "/" + maxStar;
    }
}
