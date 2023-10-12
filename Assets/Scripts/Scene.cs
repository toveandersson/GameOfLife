using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    private AudioSource source;
    public void MenuScene()
    {
        SceneManager.LoadScene(0);
    }

}
