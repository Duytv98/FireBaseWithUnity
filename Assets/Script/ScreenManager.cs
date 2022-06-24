using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{

    public static ScreenManager Instance;
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject authScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        Show("auth");
    }


    public void DisableAllScreen()
    {
        gameScreen.SetActive(false);
        authScreen.SetActive(false);
    }
    public void Show(string id)
    {
        DisableAllScreen();
        switch (id)
        {
            case "game":
                gameScreen.SetActive(true);
                break;
            case "auth":
                authScreen.SetActive(true);
                break;
            default: break;
        }
    }

}
