using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ItemUser : RecyclableListItem<int>
{
    [SerializeField] private Image background = null;
    [SerializeField] private Text txtStt = null;
    [SerializeField] private Text txtName = null;
    [SerializeField] private Text txtScore = null;

    public override void Initialize(int dataObject)
    {
    }

    public override void Removed()
    {
    }

    public void SetUp(Color color, string stt, string name, string score)
    {
        background.color = color;
        txtStt.text = stt;
        txtName.text = name;
        txtScore.text = score;
    }

    public override void Setup(int index)
    {
        if (index >= RealtimeDatabase.Instance.ListUser.Count) return;
        var conditionCol = RealtimeDatabase.Instance.ConditionCol;
        User user = RealtimeDatabase.Instance.ListUser[index];
        txtStt.text = (index + 1).ToString();
        txtName.text = user.DisplayName;

        if (conditionCol.Equals("Score")) txtScore.text = user.Score.ToString();
        else if (conditionCol.Equals("TotalTime"))
        {
            txtScore.text = GetTimeString(user.TotalTime);
        }
    }
    private string GetTimeString(int TotalTime)
    {
        TimeSpan time = TimeSpan.FromSeconds(TotalTime);
        return time.ToString(@"hh\:mm\:ss");
    }
}
