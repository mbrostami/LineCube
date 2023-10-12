
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour
{
    public GameObject ParentPanel;
    private LevelManagement levelManagement;

    void Start() {
        levelManagement = SaveGame.LoadLevel();
        Image originalImage = GameObject.Find("Level1").GetComponent<Image>();
        float nextX = originalImage.rectTransform.anchoredPosition.x + 190;
        float nextY = originalImage.rectTransform.anchoredPosition.y;
        int index = 1;
        foreach (KeyValuePair<int, Level> item in levelManagement.GetLevels())
        {
            if (item.Key == 1) {
                continue;
            }
            index = index + 1;
            int levelnumber = item.Key;
            Image img = Instantiate<Image>(originalImage, ParentPanel.transform);
            img.name = "Level" + levelnumber;
            if (item.Value.CanPlay == false) {
                img.name = "Level(BLOCKED:" + levelnumber + ")";
            }
            Vector3 vector3 = new Vector3();
            vector3.x = nextX ;
            vector3.y = nextY;
            img.rectTransform.anchoredPosition = vector3;
            Text levelText = img.GetComponentInChildren<Text>();
            levelText.name = "level" + levelnumber + "text";
            levelText.text = "Level " + levelnumber;
            if (item.Value.CanPlay == false) {
                levelText.text = "Level " + levelnumber + " :L";
            }
            img.gameObject.SetActive(true);
            if ((index % 4) == 0) {
                nextY = nextY - 190;
                nextX = nextX - (190 * 3);
            } else {
                nextX = nextX + 190;
            }
        }
        // GameObject NewObj = new GameObject(); //Create the GameObject
        // Image NewImage = NewObj.AddComponent<Image>(); //Add the Image Component script
        // NewImage.sprite = Resources.Load<Sprite>("empty-button.png"); //Set the Sprite of the Image Component on the new GameObject
        // NewImage.name = "Level3";
        // NewObj.GetComponent<RectTransform>().SetParent(ParentPanel.transform); //Assign the newly created Image GameObject as a Child of the Parent Panel.
        // NewObj.SetActive(true); //Activate the GameObject
    }

    public void ClickOnLevel()
    {
        var selectedLevel = EventSystem.current.currentSelectedGameObject;
        string numericLevel = new String(selectedLevel.name.Where(Char.IsDigit).ToArray());
        Scenes.Load("Game", "level", numericLevel);
    }

    public void Back()
    {
        SceneManager.LoadScene("Menu");
    }
}
