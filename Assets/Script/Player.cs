using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    public string name;
    public bool status;
    public bool isPlay;
    public int win;
    public int lose;

    public Player()
    {
    }

    public Player(string name)
    {
        this.name = name;
        this.status = true;
        this.isPlay = false;
        this.win = 0;
        this.lose = 0;
    }

    public Player(string name, bool status, bool isPlay, int win, int lose)
    {
        this.name = name;
        this.status = status;
        this.isPlay = isPlay;
        this.win = win;
        this.lose = lose;
    }
    
}
