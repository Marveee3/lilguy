using UnityEngine.SceneManagement;
using UnityEngine;

public class levlManager : MonoBehaviour
{
    [SerializeField]private GameObject fade;
    private Animator anim;
    int currentSceneIndex;
    int levl;
    [SerializeField]private cameraShake cameraShake;
    private AudioSource audio;
    void Awake()
    {
        anim = GetComponent<Animator>();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Camera camera = FindObjectOfType<Camera>();
        cameraShake = camera.GetComponent<cameraShake>();
        audio = GetComponent<AudioSource>();
    }

    void fadeOn()
    {
        fade.SetActive(true);
    }

    public void LvlMng(int lvl)
    {
        levl = lvl;
        anim.SetTrigger("close");
        audio.Play();
        shake();
        
    }
    void shake()
    {
        cameraShake.StartShake(0.2f, 0.5f);
    }

    void LvlLoad()
    {
        if(levl == 0)LoadNextLevel();
        if(levl == 1)RestartLevl();
    }

    void LoadNextLevel()
    {
        if (currentSceneIndex + 1 >= SceneManager.sceneCountInBuildSettings) return;

        PlayerPrefs.SetInt("UnlockedLevels", Mathf.Max(PlayerPrefs.GetInt("UnlockedLevels", 0), currentSceneIndex - 1));
        PlayerPrefs.Save();
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    void RestartLevl()
    {   
        anim.SetTrigger("close");
        SceneManager.LoadScene(currentSceneIndex); 
    }
}
