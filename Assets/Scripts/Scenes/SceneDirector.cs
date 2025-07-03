using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneHandle = UnityEngine.SceneManagement.Scene;


public enum Scene
{
    Start,
    Home,
    Match,
    Setting,
    Game
}

public class SceneDirector : MonoBehaviour
{
    public static SceneDirector Instance { get; private set; }

    [Header("场景列表")]
    public List<string> scenes;

    [Header("用户信息")]
    public string userId;
    public string userpwd;

    [Header("场景切换动画")]
    public GameObject transitionCanvasPrefab; // 用于淡入淡出
    private GameObject transitionCanvas;

    private string currentSceneName;

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

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;

        if (transitionCanvasPrefab != null)
        {
            transitionCanvas = Instantiate(transitionCanvasPrefab);
            DontDestroyOnLoad(transitionCanvas);
            transitionCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// 场景切换接口，带动画并保留上一个场景（禁用）
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionAndLoadAdditive(sceneName));
        RemoveExtraEventSystems();

    }

    private IEnumerator TransitionAndLoadAdditive(string sceneName)
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(true);
            Animator animator = transitionCanvas.GetComponent<Animator>();
            animator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(1f);
        }

        SceneHandle oldScene = SceneManager.GetActiveScene();


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return asyncLoad;

        SceneHandle newScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(newScene);

        currentSceneName = sceneName;

        // 禁用旧场景的根对象
        foreach (GameObject obj in oldScene.GetRootGameObjects())
        {
            obj.SetActive(false);
        }

        if (transitionCanvas != null)
        {
            yield return null; // 等待一帧确保 UI 激活完成
            Animator animator = transitionCanvas.GetComponent<Animator>();

            animator.SetTrigger("FadeOut");    // 再触发淡入
        }

    }

    /// <summary>
    /// 卸载之前的场景（可选）
    /// </summary>
    public void UnloadPreviousScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
        }
    }
    void RemoveExtraEventSystems()
    {
        var all = GameObject.FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        if (all.Length > 1)
        {
            for (int i = 1; i < all.Length; i++)
            {
                Destroy(all[i].gameObject);
            }
        }
    }

    /// <summary>
    /// 获取当前场景名称
    /// </summary>
    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
}
