using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum Scene
{
    Home,
    MatchRoom,
    Setting,
    Game
}

public class SceneDirector : MonoBehaviour
{
    //[SerializeField] private BoolEventChannel 
    public static SceneDirector Instance { get; private set; }
    public List<string> scenes;
    private Scene currentScene = Scene.Home;
    private int index = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void HandleSceneChangeEvent(string sceneName)
    {
        switch (currentScene)
        {
            case Scene.Home:
                break;
            case Scene.MatchRoom:
                break;
            case Scene.Setting:
                break;
            case Scene.Game:
                break;
        }
    }


}