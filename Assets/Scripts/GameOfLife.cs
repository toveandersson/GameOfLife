using System;
using System.Runtime.CompilerServices;
using TMPro.Examples;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOfLife : MonoBehaviour
{
    public GameObject cellPrefab;
    public GameObject fadedCellPrefab;
    Cell[,] cells;
    FadedCells[,] fadedCells;
    int[,] conditionForLifeQuestionMark; // Matrix that calculates the next generation
    int[,] conditionForFadedLifeQuestionMark;
    int conditionForAcornLife= 0;
    int conditionForGosperLife = 0;
    int conditionForAchimspLife = 0;
    int conditionForAchimsp2Life = 0;
    int conditionForAchimsp3Life = 0;
    int conditionForAchimsp4Life = 0;
    int conditionForToadLife = 0;
    int conditionForWipe = 0;
    int[] acornCoordinates;

    public float cellSize = 0.1f;
    public float spawnChancePercentage = 5f;
    public float musicVolume = 1f;
    public AudioSource source;

    int numberOfColums, numberOfRows;

    Vector2 gridOrigin = Vector2.zero;

    float generationDelay = 0.38f; //0.42

    bool rPressed = false;
    bool frogFrame = true;
    bool nPressed = false;
    bool mute = false;
    public TextMesh musicText;
    private MeshRenderer meshRenderer;
    bool acorn = false;
    bool toad = false;
    bool achimsp = false;
    bool achimsp2 = false;
    bool achimsp3 = false;
    bool achimsp4 = false;
    bool gosper = false;
    int gosperTurn90 = 0;
    bool gosperSwitchAngle = false;

    //frog offsets
    int matrixOffset = 0;
    int spawnOffset = 1;
    int spawnEnd = 0;
    int activeAreaStart = 2;
    int activeAreaEnd = -1;

    public int X;
    public int Y;

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {   generationDelay -= 0.005f;}

        if (Input.GetKey(KeyCode.DownArrow))
        {   generationDelay += 0.005f;}

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (frogFrame)
            {
                frogFrame = false;
                FrogFrame();
            }
            else if (!frogFrame)    
            {
                frogFrame= true;
                FrogFrame();
            }
        }

        if (Input.GetKeyDown(KeyCode.N) && !nPressed)                   //no halffrogs for better visualization
        { nPressed = true;   }
        else if (Input.GetKeyDown(KeyCode.N) && nPressed)
        {  nPressed = false;}

        if (Input.GetKeyDown(KeyCode.R))                            //regenerate
        { rPressed = true; }
        if (Input.GetKeyDown(KeyCode.W))                                //wipe
        { conditionForWipe = 1; }

        if (Input.GetKeyDown(KeyCode.C))                                //clear
        {
            acorn = false;
            toad = false;
            achimsp = false;
            achimsp2 = false;
            achimsp3 = false;
            achimsp4 = false;
            gosper = false;
            gosperTurn90 = 0;
            gosperSwitchAngle = false;
        }

        if (Input.GetKeyDown(KeyCode.M) && !mute)
        {
            mute = true;
            source.volume = 0f;
        }
        else if ((Input.GetKeyDown(KeyCode.M) && mute))
        {
            mute = false;
            source.volume = musicVolume;
        }

        if (Input.GetKeyDown(KeyCode.A))                                //acorn
        {  acorn = true;}
        if ((Input.GetMouseButtonDown(0)) && (acorn == true))       
        { conditionForAcornLife = 1;  }

        if (Input.GetKeyDown(KeyCode.T))                                //toad
        { toad = true; }
        if ((Input.GetMouseButtonDown(0)) && (toad == true))
        { conditionForToadLife = 1; }

        if (Input.GetKeyDown(KeyCode.S))                                //achismp
        {achimsp = true; }
        if ((Input.GetMouseButtonDown(0)) && (achimsp == true))
        {conditionForAchimspLife = 1;}

        if (Input.GetKeyDown(KeyCode.D))                                //achismp2
        { achimsp2 = true; }
        if ((Input.GetMouseButtonDown(0)) && (achimsp2 == true))
        { conditionForAchimsp2Life = 1; }

        if (Input.GetKeyDown(KeyCode.E))                                //achismp3
        { achimsp3 = true; }
        if ((Input.GetMouseButtonDown(0)) && (achimsp3 == true))
        { conditionForAchimsp3Life = 1; }

        if (Input.GetKeyDown(KeyCode.Z))                                //achismp4
        { achimsp4 = true; }
        if ((Input.GetMouseButtonDown(0)) && (achimsp4 == true))
        { conditionForAchimsp4Life = 1; }

        if (Input.GetKeyDown(KeyCode.G))                                //gosper
        {
            gosper = true;
            gosperTurn90 = 0;
            gosperSwitchAngle = false;
        }

        if ((gosper == true) && (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetMouseButtonDown(2)))
        { gosperTurn90++; }

        if ((gosper == true) && Input.GetKeyDown(KeyCode.LeftArrow))
        { gosperTurn90 += 3; }

        //if (gosperSwitchAngle == false && Input.GetMouseButtonDown(1))
        //{ 
        //    gosperSwitchAngle = true;
        //    Debug.Log(gosperSwitchAngle);
        //}

        //else if (gosperSwitchAngle = true && Input.GetMouseButtonDown(1))
        //{ 
        //    gosperSwitchAngle = false;
        //    Debug.Log("har redan körts");
        //}

        if ((Input.GetMouseButtonDown(0)) && (gosper == true))
        { conditionForGosperLife = 1; }

    }

    void Start()
    {
        musicVolume = PlayerPrefs.GetFloat("volume", 0.2f);
        //musicText.text = "♫frosti - björk";
        if (PlayerPrefs.GetFloat("volumeOn") == 0f)
        {   
            musicVolume = 0f;
            //musicText.text = " ";
        }
        source.volume = musicVolume;

        cellSize = PlayerPrefs.GetFloat("cellsize", 0.5f);
        spawnChancePercentage = PlayerPrefs.GetFloat("spawnpercentage", 15f);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        //Calculate our grid depending on size and cellSize
        numberOfColums = (int)Mathf.Floor((Camera.main.orthographicSize *
            Camera.main.aspect * 2) / cellSize);
        numberOfRows = (int)Mathf.Floor(Camera.main.orthographicSize * 2 / cellSize);

        //Initiate our matrix array
        cells = new Cell[numberOfColums+matrixOffset+1, numberOfRows+matrixOffset+1];
        fadedCells = new FadedCells[numberOfColums + matrixOffset + 1, numberOfRows + matrixOffset + 1];
        conditionForLifeQuestionMark = new int[numberOfColums + matrixOffset + 1, numberOfRows + matrixOffset + 1];
        conditionForFadedLifeQuestionMark = new int[numberOfColums + matrixOffset + 1, numberOfRows + matrixOffset + 1];

        //Create all objects
        for (int x = spawnOffset; x < cells.GetLength(0)+ spawnEnd; x++)  //all
        {
            for (int y = spawnOffset; y < cells.GetLength(1)+ spawnEnd; y++)
            {
                //Create our game cell objects, multiply by cellSize for correct world placement
                Vector2 newPos = new Vector2(x * cellSize - Camera.main.orthographicSize *
                    Camera.main.aspect ,
                    y * cellSize - Camera.main.orthographicSize);

                var newCell = Instantiate(cellPrefab, newPos, Quaternion.identity);
                newCell.transform.localScale = Vector2.one * cellSize;
                cells[x, y] = newCell.GetComponent<Cell>();


                var newFadedCell = Instantiate(fadedCellPrefab, newPos, Quaternion.identity);
                newFadedCell.transform.localScale = Vector2.one * cellSize;
                fadedCells[x, y] = newFadedCell.GetComponent<FadedCells>();

                cells[x, y].alive = false;
                fadedCells[x, y].alive = false;
                cells[x, y].UpdateStatus();
                fadedCells[x, y].UpdateStatus();

                cells[x, y].XIndex = x;
                cells[x, y].YIndex = y;
            }
        }
        AllFrogsHalfwayUp();
    }
    private void AllFrogsHalfwayUp()
    {
        for (int y = spawnOffset; y < cells.GetLength(1) + spawnEnd; y++) 
        {
            for (int x = 1; x < cells.GetLength(0)+ spawnEnd; x++)    
            {
                fadedCells[x, y].alive = true;
                fadedCells[x, y].UpdateStatus();
            }
        }
        Invoke(nameof(AllFrogsDown), 0.5f);
    }
    private void AllFrogsDown()     //all frogs except a frame of frogs
    {
        for (int y = activeAreaStart; y < cells.GetLength(1)+ activeAreaEnd; y++)
        {
            for (int x = activeAreaStart; x < cells.GetLength(0)+ activeAreaEnd; x++)
            {
                conditionForFadedLifeQuestionMark[x, y] = 0;
                conditionForLifeQuestionMark[x, y] = 0;
                cells[x, y].alive = false;
                cells[x, y].UpdateStatus();
                fadedCells[x, y].alive = false;
                fadedCells[x, y].UpdateStatus();
            }
        }
        Invoke(nameof(RandomizedFrogsHalfwayUp), 0.4f);
    }
    private void RandomizedFrogsHalfwayUp() //randomized frogs are halfway up from the water
    {
        for (int y = activeAreaStart; y < cells.GetLength(1)+ activeAreaEnd; y++)
        {
            for (int x = activeAreaStart; x < cells.GetLength(0)+ activeAreaEnd; x++)
            {
                //Random check to see if it should be alive
                if (UnityEngine.Random.Range(0, 100) < spawnChancePercentage)
                {
                    fadedCells[x, y].alive = true;
                }
                else
                {
                    fadedCells[x, y].alive = false;                 
                }
                fadedCells[x, y].UpdateStatus();
            }
        }
        
        Invoke(nameof(RandomizedFrogsUp), 0.4f);
    }
    private void RandomizedFrogsUp() //frogs are up 
    {
        for (int y = activeAreaStart; y < cells.GetLength(1) + activeAreaEnd; y++)
        {
            for (int x = activeAreaStart; x < cells.GetLength(0) +activeAreaEnd; x++)
            {
                if (fadedCells[x, y].alive == true)
                {
                    fadedCells[x, y].alive = false;
                    cells[x, y].alive = true;
                    cells[x, y].UpdateStatus();
                }
            }
        }
        Invoke(nameof(CalculateNextGeneration), 0.5f);
        rPressed = false;
    }
    private void FrogFrame()
    {
        if (frogFrame)
        {
            for (int x = spawnOffset; x < cells.GetLength(0) + spawnEnd; x++)
            {
                fadedCells[x, cells.GetLength(1) + spawnEnd-1].alive = true;
                fadedCells[x, cells.GetLength(1) + spawnEnd-1].UpdateStatus();
            }
            for (int x = spawnOffset; x < cells.GetLength(0) + spawnEnd; x++)
            {
                fadedCells[x, spawnOffset].alive = true;
                fadedCells[x, spawnOffset].UpdateStatus();
            }
            for (int y = spawnOffset; y < cells.GetLength(1) + spawnEnd; y++)
            {
                fadedCells[cells.GetLength(0) + spawnEnd-1, y].alive = true;
                fadedCells[cells.GetLength(0) + spawnEnd - 1, y].UpdateStatus();
            }
            for (int y = spawnOffset; y < cells.GetLength(1) + spawnEnd; y++)
            {
                fadedCells[spawnOffset, y].alive = true;
                fadedCells[spawnOffset, y].UpdateStatus();
            }
        }
        else if (!frogFrame)
        {
            for (int x = spawnOffset; x < cells.GetLength(0) + spawnEnd; x++)
            {
                fadedCells[x, cells.GetLength(1) + spawnEnd - 1].alive = false;
                fadedCells[x, cells.GetLength(1) + spawnEnd - 1].UpdateStatus();
            }
            for (int x = spawnOffset; x < cells.GetLength(0) + spawnEnd; x++)
            {
                fadedCells[x, spawnOffset].alive = false;
                fadedCells[x, spawnOffset].UpdateStatus();
            }

            for (int y = spawnOffset; y < cells.GetLength(1) + spawnEnd; y++)
            {
                fadedCells[cells.GetLength(0) + spawnEnd-1, y].alive = false;
                fadedCells[cells.GetLength(0) + spawnEnd-1, y].UpdateStatus();
            }
            for (int y = spawnOffset; y < cells.GetLength(1) + spawnEnd; y++)
            {
                fadedCells[spawnOffset, y].alive = false;
                fadedCells[spawnOffset, y].UpdateStatus();
            }
        }
    }
    private void CalculateNextGeneration()
    {
        //update conditionForLife? matrix to get current state of all cells
        //Array.Clear(acornCoordinates, 0, acornCoordinates.Length);
        int acornArrayCount = 0;
        for (int y = activeAreaStart; y < cells.GetLength(1) +activeAreaEnd; y++)
        {
            if (rPressed)
            {
                break;
            }
            for (int x = activeAreaStart; x < cells.GetLength(0)+ activeAreaEnd; x++)
            {
                int neighborSum = 0;
                for (int neighborY = -1; neighborY < 2; neighborY++) //for each step on the y-axis..
                {
                    for (int neighborX = -1; neighborX < 2; neighborX++) //look through tree naighbor cells
                    {
                        if (neighborY == 0 && neighborX == 0)   //don't count the cell that is the starting point 
                        { continue; }

                        if (cells[x + neighborX, y + neighborY].alive)
                        { neighborSum++; }
                    }
                }
                ConditionForLife(x, y, neighborSum, acornArrayCount);
            }
        }
        InstantiateNextGeneration();
    }
    private void ConditionForLife(int x, int y, int neighborSum, int acornArrayCount)
    {
        if (neighborSum < 2)
        {
            conditionForLifeQuestionMark[x, y] = 0;  //conditionforlife applies to the next generation
            if (cells[x, y].alive)                      //does the cell live now but not the next generation there is condition for "fadedlife"
            {
                if (!nPressed)
                {
                    conditionForFadedLifeQuestionMark[x, y] = 1;
                }
                else
                {
                    conditionForFadedLifeQuestionMark[x, y] = 0;
                }
            }           
        }
        else if ((neighborSum == 2 || neighborSum == 3) && cells[x, y].alive)
        {
            conditionForLifeQuestionMark[x, y] = 1;
            conditionForFadedLifeQuestionMark[x, y] = 0;
        }
        else if (neighborSum > 3)
        {
            conditionForLifeQuestionMark[x, y] = 0;
            if (cells[x, y].alive)
            {
                if (!nPressed)
                {
                    conditionForFadedLifeQuestionMark[x, y] = 1;
                }
                else
                {
                    conditionForFadedLifeQuestionMark[x, y] = 0;
                }
            }
        }
        else if (neighborSum == 3)                 
        {
            conditionForLifeQuestionMark[x, y] = 1;         //if not alive now but but condition is right there is condition for life
            conditionForFadedLifeQuestionMark[x, y] = 0;
        }
        if (!cells[x, y].alive)
        {
            conditionForFadedLifeQuestionMark[x, y] = 0;
        }
        //if (neighborSum == 0)
        //{
        //    conditionForAcornLife[x, y] = 1;
        //}
        //if (neighborSum == 0 && conditionForAcornLife[x - 3, y] == 1)
        //{
        //    acornCoordinates[acornArrayCount] = conditionForAcornLife[x - 2, y];
        //    acornArrayCount++;
        //}
    }
    public void SpawnAcorn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            Cell clickedCell = clickedObject.GetComponent<Cell>();

            if (clickedCell != null)
            {
                int x = clickedCell.XIndex;
                int y = clickedCell.YIndex;
                int[] acornOffsetsY = { 1, 0,-1,-1,-1,-1,-1};
                int[] acornOffsetsX = { -2, 0, -3, -2, 1, 2, 3 };
                for (int i = 0; i < acornOffsetsX.Length; i++)
                {
                    int acornX = x + acornOffsetsX[i];
                    int acornY = y + acornOffsetsY[i];

                    if (acornX >= 0 && acornX < cells.GetLength(0) && acornY >= 0 && acornY < cells.GetLength(1))
                    {
                        conditionForLifeQuestionMark[acornX, acornY] = 1;
                    }
                }
            }      
        }
    }
    //int[,] hej = acornCoordinates[UnityEngine.Random.Range(0, acornCoordinates.Length)];
    public void SpawnToad()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            Cell clickedCell = clickedObject.GetComponent<Cell>();

            if (clickedCell != null)
            {
                int x = clickedCell.XIndex;
                int y = clickedCell.YIndex;
                int[] toadOffsetsY = { 1,1,1,0,0,0};
                int[] toadOffsetsX = { -1,0,1,-2,-1,0};
                for (int i = 0; i < toadOffsetsX.Length; i++)
                {
                    int toadX = x + toadOffsetsX[i];
                    int toadY = y + toadOffsetsY[i];

                    if (toadX >= 0 && toadX < cells.GetLength(0) && toadY >= 0 && toadY < cells.GetLength(1))
                    {
                        conditionForLifeQuestionMark[toadX, toadY] = 1;
                    }
                }
            }
        }
    }
    public void SpawnAchimsp1()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            Cell clickedCell = clickedObject.GetComponent<Cell>();

            if (clickedCell != null)
            {
                int x = clickedCell.XIndex;
                int y = clickedCell.YIndex;
                int[] achismpOffsetsY = { 6,6,5,5,4,4,4,4,3,3,3,2,2,1,1,1};
                int[] achismpOffsetsX = { 1,2,1,3,-4,1,3,4,-5,-4,2,-6,-3,-6,-5,-4};
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x + achismpOffsetsX[i];
                    int achimspY = y + achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    {
                        conditionForLifeQuestionMark[achimspX, achimspY] = 1;
                    }
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x - achismpOffsetsX[i];
                    int achimspY = y - achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    {
                        conditionForLifeQuestionMark[achimspX, achimspY] = 1;
                    }
                }
            }
        }
    }
    public void SpawnAchimsp2()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            Cell clickedCell = clickedObject.GetComponent<Cell>();

            if (clickedCell != null)
            {
                int x = clickedCell.XIndex;
                int y = clickedCell.YIndex;
                int[] achismpOffsetsY = { 1, 1, 2, 2, 2, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 9, 10, 11 };
                int[] achismpOffsetsX = { -11, -10, -7, -6, -5, -8, -2, -9, -7, -2, -8, -6, -2, -7, -5, -6, -1, -1 };
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x +1 + achismpOffsetsX[i];
                    int achimspY = y - 1 + achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1;}
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x - achismpOffsetsX[i];
                    int achimspY = y -1  + achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1; }
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x  - achismpOffsetsX[i];
                    int achimspY = y  - achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1; }
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x +1 + achismpOffsetsX[i];
                    int achimspY = y  - achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1; }
                }

            }
        }
    }
    public void SpawnAchimsp3()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            Cell clickedCell = clickedObject.GetComponent<Cell>();

            if (clickedCell != null)
            {
                int x = clickedCell.XIndex;
                int y = clickedCell.YIndex;
                int[] achismpOffsetsY = { 2,2, 3,3,3, 4,4,4, 5,5,5, 6,6,6,6, 7,7,7,7,7,7, 8,8,8, 9,9,9,9,9, 13,13, 17,17, 18,18, 19,19, 20, 22,22, 23,23};
                int[] achismpOffsetsX = { -8,-7, -13,-9,-7, -13,-9,-7, -18,-9,-8, -23,-22,-19,-17, -23,-22,-20,-4,-3,-2, -19,-5,-2, -18,-17,-5,-4,-3, -4,-3, -9,-6, -9,-5, -8,-6, -7, -7,-6, -7,-6};
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x + achismpOffsetsX[i];
                    int achimspY = y + achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1; }
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x - achismpOffsetsX[i];
                    int achimspY = y + achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1; }
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x - achismpOffsetsX[i];
                    int achimspY = y - achismpOffsetsY[i];
                     
                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1; }
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x + achismpOffsetsX[i];
                    int achimspY = y - achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    { conditionForLifeQuestionMark[achimspX, achimspY] = 1; }
                }
            }
        }
    }
    public void SpawnAchimsp4()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            Cell clickedCell = clickedObject.GetComponent<Cell>();

            if (clickedCell != null)
            {
                int x = clickedCell.XIndex;
                int y = clickedCell.YIndex;
                int[] achismpOffsetsY = {9,9,9,9, 8,8,8,8, 7,7, 6,6, 5,5, 4, 3,3, 2,2, 1,1,};
                int[] achismpOffsetsX = {-14,-13,12,13, -14,-13,12,13, 4,5, 3,6, 4,5, 0, -1,1, -2,2, -2,1};
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x + achismpOffsetsX[i];
                    int achimspY = y + achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    {
                        conditionForLifeQuestionMark[achimspX, achimspY] = 1;
                    }
                }
                for (int i = 0; i < achismpOffsetsX.Length; i++)
                {
                    int achimspX = x -1 - achismpOffsetsX[i];
                    int achimspY = y  - achismpOffsetsY[i];

                    if (achimspX >= 0 && achimspX < cells.GetLength(0) && achimspY >= 0 && achimspY < cells.GetLength(1))
                    {
                        conditionForLifeQuestionMark[achimspX, achimspY] = 1;
                    }
                }
            }
        }
    }
    public void SpawnGosper()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            Cell clickedCell = clickedObject.GetComponent<Cell>();

            if (clickedCell != null)
            {
                int x = clickedCell.XIndex;
                int y = clickedCell.YIndex;
                int gosperX;
                int gosperY;
                int[] gliderGunOffsetsY = { 4, 3, 3, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2, -2, -3, -3, -4, -4 };
                int[] gliderGunOffsetsX = { 6, 4, 6, -6, -5, 2, 3, 16, 17, -7, -3, 2, 3, 16, 17, -18, -17, -8, -2, 2, 3, -18, -17, -8, -4, -2, -1, 4, 6, -8, -2, 6, -7, -3, -6, -5 };
                for (int i = 0; i < gliderGunOffsetsX.Length; i++)
                { 
                    if (gosperTurn90 % 4 == 0) 
                    {
                        gosperX = x + gliderGunOffsetsX[i];
                        gosperY = y + gliderGunOffsetsY[i];
                        if (gosperX >= 0 && gosperX < cells.GetLength(0) && gosperY >= 0 && gosperY < cells.GetLength(1))
                        {
                            conditionForLifeQuestionMark[gosperX, gosperY] = 1;
                        }
                    }
                    else if (gosperTurn90 % 4 == 0)
                    {
                        gosperX = x - gliderGunOffsetsX[i];
                        gosperY = y - gliderGunOffsetsY[i];
                        if (gosperX >= 0 && gosperX < cells.GetLength(0) && gosperY >= 0 && gosperY < cells.GetLength(1))
                        {
                            conditionForLifeQuestionMark[gosperX, gosperY] = 1;
                        }
                    }
                    else if (gosperTurn90 % 4 == 1)
                    {
                        gosperX = x + gliderGunOffsetsY[i];
                        gosperY = y + gliderGunOffsetsX[i];
                        if (gosperX >= 0 && gosperX < cells.GetLength(0) && gosperY >= 0 && gosperY < cells.GetLength(1))
                        {
                            conditionForLifeQuestionMark[gosperX, gosperY] = 1;
                        }
                    }
                    else if (gosperTurn90 % 4 == 2)
                    {
                        gosperX = x - gliderGunOffsetsX[i];
                        gosperY = y - gliderGunOffsetsY[i];
                        if (gosperX >= 0 && gosperX < cells.GetLength(0) && gosperY >= 0 && gosperY < cells.GetLength(1))
                        {
                            conditionForLifeQuestionMark[gosperX, gosperY] = 1;
                        }
                    }
                    else if (gosperTurn90 % 4 == 3)
                    {
                        gosperX = x - gliderGunOffsetsY[i];
                        gosperY = y - gliderGunOffsetsX[i];
                        if (gosperX >= 0 && gosperX < cells.GetLength(0) && gosperY >= 0 && gosperY < cells.GetLength(1))
                        {
                            conditionForLifeQuestionMark[gosperX, gosperY] = 1;
                        }
                    }
                }
                //if ((clickedCell.XIndex-18 > activeAreaStart && clickedCell.XIndex+18 < cells.Length +activeAreaEnd) && (clickedCell.YIndex-4 > activeAreaStart && clickedCell.YIndex+4 < cells.Length + activeAreaEnd))
                //{

                //}
            }
        }
    }
    private void Wipe()
    {
        for (int y = activeAreaStart; y < cells.GetLength(1) + activeAreaEnd; y++)
        {
            for (int x = activeAreaStart; x < cells.GetLength(0) + activeAreaEnd; x++)
            {
                conditionForFadedLifeQuestionMark[x, y] = 0;
                conditionForLifeQuestionMark[x, y] = 0;
                fadedCells[x, y].alive = false;
                fadedCells[x,y].UpdateStatus();
                cells[x, y].alive = false;
                cells[x, y].UpdateStatus();
            }
        }
    }
    private void InstantiateNextGeneration()
    {
        if (conditionForAcornLife == 1)
        {
            SpawnAcorn();
            acorn = false;
            conditionForAcornLife = 0;
        }
        if (conditionForToadLife == 1)
        {
            SpawnToad();
            toad = false;
            conditionForToadLife = 0;
        }
        if (conditionForGosperLife == 1)
        {
            SpawnGosper();
            gosper = false;
            conditionForGosperLife = 0;
        }
        if (conditionForAchimspLife == 1)
        {
            SpawnAchimsp1();
            achimsp = false;
            conditionForAchimspLife = 0;
        }
        if (conditionForAchimsp2Life == 1)
        {
            SpawnAchimsp2();
            achimsp2 = false;
            conditionForAchimsp2Life = 0;
        }
        if (conditionForAchimsp3Life == 1)
        {
            SpawnAchimsp3();
            achimsp3 = false;
            conditionForAchimsp3Life = 0;
        }
        if (conditionForAchimsp4Life == 1)
        {
            SpawnAchimsp4();
            achimsp4 = false;
            conditionForAchimsp4Life = 0;
        }
        if (conditionForWipe == 1)
        {
            Wipe();
            conditionForWipe = 0;
        }
        for (int y = activeAreaStart; y < cells.GetLength(1) +activeAreaEnd; y++)
        {
            for (int x = activeAreaStart; x < cells.GetLength(0)+activeAreaEnd; x++)
            {
                if (conditionForLifeQuestionMark[x, y] == 0)
                {
                    if ((conditionForFadedLifeQuestionMark[x, y] == 1) && !nPressed) //faded cells becomes visible if there is condition for faded life, a cell is alive at the 
                    {                                                                                    //spot right now and has not condition to stay alive next generation and n is not pressed
                        if (!nPressed)
                        {
                            fadedCells[x, y].alive = true;
                        }  
                    }
                    cells[x, y].alive = false;                      //has to change the state for next generation after checking the state in recent generation in previous if statement                   
                }
                if (conditionForFadedLifeQuestionMark[x, y] == 0)   //faded cells will not be visible when the condition for faded life is 0
                {
                    fadedCells[x, y].alive = false;
                }
                if (conditionForLifeQuestionMark[x, y] == 1)
                {
                    cells[x, y].alive = true;
                    fadedCells[x,y].alive = false;
                }
                cells[x, y].UpdateStatus();
                fadedCells[x, y].UpdateStatus();
            }
        }
        if (!rPressed)
        {
            Invoke(nameof(CalculateNextGeneration), generationDelay); //-Time.deltaTime
        }
        else if (rPressed)
        {
            AllFrogsDown();
        }
    }
    
}

