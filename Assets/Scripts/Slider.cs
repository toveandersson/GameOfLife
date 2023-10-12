using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Slider : MonoBehaviour
{
    public Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    //[SerializeField] private Slider cellSizeSlider;
    //[SerializeField] private Slider spawnPercentageSlider;
    public float volume;
    public float cellSize;
    public float spawnPercentage;
    public bool musicOn;
    public float volumeSaver;

    public void ChangeCellSize(float cellSizeInput)
    {
        cellSize = cellSizeInput;
    }
    public void ChangeSpawnPercentage(float spawnPercentageInput)
    {
        spawnPercentage = spawnPercentageInput;
    }
 
}
