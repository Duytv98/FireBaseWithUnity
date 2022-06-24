using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{

    [SerializeField] private ItemUser itemUserPrefab = null;
    [SerializeField] private RectTransform listUserContainer = null;
    [SerializeField] private ScrollRect scrollRect = null;

    [SerializeField] private Text conditionCol = null;
    [SerializeField] private string txtcondition = "";
    // [SerializeField] private TestScriptCategory itemCategory = null;


    private RecyclableListHandler<int> levelListHandler;

    public string TxtCondition
    {
        get => txtcondition; set => txtcondition = value;
    }
    private void Start()
    {
        conditionCol.text = TxtCondition;
        RealtimeDatabase.Instance.GetData(0, false);
    }

    public void Show(int totalUser, bool status)
    {
        List<int> sttIndicies = new List<int>();

        for (int i = 0; i < totalUser; i++)
        {
            sttIndicies.Add(i);
        }
        if (status) levelListHandler.NewRefresh(sttIndicies);
        else if (levelListHandler == null)
        {
            levelListHandler = new RecyclableListHandler<int>(sttIndicies, itemUserPrefab, listUserContainer, scrollRect);
            // levelListHandler.OnListItemClicked = OnLevelListItemClicked;
            levelListHandler.Setup();
        }
        else
        {
            levelListHandler.UpdateDataObjects(sttIndicies);
        }
    }
}
