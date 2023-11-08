using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MainMenuSelectLevel : MonoBehaviour
{
    private readonly string SAVE_FOLDER =  Application.dataPath + "/Levels/";
    public event EventHandler OnSelectLevel;
    [SerializeField] GameObject LevelUI;
    [SerializeField] GameObject LevelPreview;

    [SerializeField] GameObject LoadButton;

    private string LevelSelected;

    private Transform currentButton;

    private void Awake()
    {
        LevelSelected = "";
        OnSelectLevel += LevelSelect_OnSelectedChanged;

        RefreshSelectedVisual();

    }

    public void LoadButtonClicked(){
        GameObject.Find("GridHandler").GetComponent<GridHandler>().LoadButton(LevelSelected);
    }

    public void ResetSelectLevel()
    {
        LoadButton.SetActive(false);
        LevelSelected = "";
        RefreshSelectedVisual();
        // LevelPreview.GetComponent<Image>().sprite = null;
        // LevelPreview.SetActive(false);
    }

    private void SetLevelDisplay()
    {
        LevelSelected = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        RefreshSelectedVisual();
        LoadButton.SetActive(true);
        // LevelPreview.SetActive(true);
        // LevelPreview.GetComponent<Image>().sprite = SaveSystem.LoadImage(LevelSelected);
    }

    private void LevelSelect_OnSelectedChanged(object sender, System.EventArgs e)
    {
        RefreshSelectedVisual();
    }

    private void RefreshSelectedVisual()
    {

        DirectoryInfo dirInfo = new DirectoryInfo(SAVE_FOLDER);
        //DirectoryInfo[] levelFolders = dirInfo.GetDirectories();
        Transform[] childList = transform.GetComponentsInChildren<Transform>();
        List<string> childListName = new List<string>();

        foreach (Transform child in childList)
            if (!childListName.Contains(child.name))
                childListName.Add(child.name);

        foreach (var levelFile in dirInfo.GetFiles())
        {
            if(levelFile.Name.Contains(".meta")) continue;
            if (!childListName.Contains(levelFile.Name))
            {
                GameObject levelUI = Instantiate(LevelUI, transform);
                SettingUI(levelUI, levelFile.Name);
                Button btn = levelUI.transform.Find("LevelButton").GetComponent<Button>();
                btn.onClick.AddListener(SetLevelDisplay);
            }
        }
        foreach (Transform child in transform)
        {
            child.Find("LevelButton").Find("Selected").gameObject.SetActive(LevelSelected == child.name);

        }
        // PlayerPrefs.SetString("LevelSystemSave", LevelSelected);
    }

    public void SettingUI(GameObject UI, string levelName, Sprite image = null)
    {
        // //UI.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1);
        // if (image)
        //     UI.transform.Find("Image").GetComponent<Image>().sprite = image;
        UI.transform.Find("LevelButton").Find("LevelName").GetComponent<TMPro.TextMeshProUGUI>().SetText(levelName);
        UI.transform.gameObject.name = levelName;
    }
}
