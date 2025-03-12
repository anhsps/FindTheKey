using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButtons14 : MonoBehaviour
{
    [SerializeField] private Sprite lockedSprite, unlockedSprite;
    private Button[] levelButtons;

    void Start()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        levelButtons = GetComponentsInChildren<Button>(true);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            Button button = levelButtons[i];
            Image buttonImage = button.GetComponent<Image>();
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (levelIndex <= unlockedLevel)
            {
                // Level da mo khoa
                //buttonText.gameObject.SetActive(true);
                buttonImage.sprite = unlockedSprite;

                button.onClick.AddListener(() => LoadLevel(levelIndex));
            }
            else
            {
                // Level chua mo khoa
                //buttonText.gameObject.SetActive(false);
                buttonImage.sprite = lockedSprite;
            }

            if (buttonText)
                buttonText.text = levelIndex < 10 ? "0" + levelIndex : levelIndex.ToString();
        }
    }

    private void LoadLevel(int levelIndex) => GameManager14.Instance.SetCurrentLV(levelIndex);
}
