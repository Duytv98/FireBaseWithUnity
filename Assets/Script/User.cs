using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class User
{
    public string UserId;
    public string Email;
    public string DisplayName;
    public string PhotoUrl;
    public string ProviderId;
    public int Score;
    public int TotalTime;

    public User()
    {
    }
}
