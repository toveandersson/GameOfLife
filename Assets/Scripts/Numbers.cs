using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Numbers : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI volumeText = null;
    [SerializeField] private TextMeshProUGUI cellSizeText = null;
    [SerializeField] private TextMeshProUGUI spawnPercentageText = null;

    public float volumeOn = 1;
    public float volume = 50;
    public float cellSize = 20;
    public float spawnPercentage = 20;

    public void MusicOn(float value)
    {
        if (volumeOn == 1)
        {   volumeOn = 0;  }
        else
        {   volumeOn = 1; }
    }
    public void volumeInput(float value)
    {
        volume = value;
        volumeText.text = volume.ToString("0")+"%";
    }
    public void cellSizeInput(float value)
    {
        cellSize = value;
        cellSizeText.text = cellSize.ToString("0") + "%";
    }
    public void spawnPercentageInput(float value)
    {
        spawnPercentage = value;
        spawnPercentageText.text = spawnPercentage.ToString("0") + "%";
    }
    public void NextScene()
    {
        PlayerPrefs.SetFloat("volumeOn", volumeOn);
        PlayerPrefs.SetFloat("volume", volume/100);
        PlayerPrefs.SetFloat("cellsize", cellSize * 0.01f);
        PlayerPrefs.SetFloat("spawnpercentage", spawnPercentage);

        SceneManager.LoadScene(1);
    }
}
