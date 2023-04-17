using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // private bool columnThree = false;
    private bool rowThree = false;

    private int gridXY;

    void Awake()
    {
        //since we want to reload the game, and we don't want GameManager to not reload its Awake and Start
        //especially when we set up the grid in Start
        //set instance to this
        instance = this;
    }

    //init and define the size of the grid: 4 by 4
    public int width = 4, height = 4;
    public int[,] grid;
  
    
    public Transform object1;
    public Transform object2;

    public GameObject eggPrefab0, eggPrefab1, eggPrefab2;
    public GameObject prefabs;      //parent obj of instantiated eggs
    
    //go crazy and make both a dictionary and a list for the instantiated eggs
    private Dictionary<string, GameObject> spawnedPieces = new Dictionary<string, GameObject>();
    private List<GameObject> spawnedPiecesList = new List<GameObject>();
    
    int randomNum;      //init the int for randomization

    //the cracked eggs prefabs
    public GameObject whiteEgg;
    public GameObject brownEgg;
    public GameObject rainbowEgg;
    
    private GameObject newEgg;      //holder of instantiated eggs
    

    void Start()
    {
        //define the grid
        grid = new int[width, height];

        //the array to pull eggs from
        //GameObject[] threePrefabs = { eggPrefab0, eggPrefab1, eggPrefab2 };
        
        checkInitialization();
        
        InstantiatePrefab();
    }

    void checkInitialization()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //initialize the grid, and assign a random int --0, 1, 2--to each grid
                randomNum = Random.Range(0, 3);
                //Debug.Log(randomNum);
                grid[x, y] = randomNum;
            }
        }
        
        int counter = 0;        //a legacy counter to debug infinite loop, which we used to have a lot
        
        //THE FOLLLOWING: CHECK IF THERE ARE MATCH-3'S ON THE BOARD WHEN WE ARE GENERATING THE GRID, BEFORE THE GAME STARTS
        //Just define each grid's value and check if the starting board has matches
        //if there is matches, replace
        while (ConnectThree(false)) //shouldReplace is false because we don't need to change anything to cracked eggs yet
        {
            counter++;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    randomNum = Random.Range(0, 3);
                    //Debug.Log(randomNum);
                    grid[x, y] = randomNum;     //assign a new value to those matched grids
                    //Debug.Log("ConnectThree: " + ConnectThree(false));
                }
            }
        }
        Debug.Log("couldnt find a valid grid, counter = "+counter + "ConnectThree: " + ConnectThree(false));
    }

    void Update()
    {
        //reload scene when pressing space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }

    public bool Swapped(Transform object1, Transform object2)
    {
        //if both object 1 and object 2 are true, return true
        return object1 && object2;
    }

    void InstantiatePrefab()
    {
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                switch (grid[x,y])
                    {
                        case 0:     //if grid value is 0, generate eggPrefab0 (brown)
                            GameObject obj0 = Instantiate(eggPrefab0,new Vector3(x, y, 0), Quaternion.identity);
                            obj0.transform.parent = prefabs.transform;  //add to the parent object
                            spawnedPiecesList.Add(obj0);    //add obj0 to the list for spawned eggs
                            break;
                        case 1:     //if grid value is 1, generate eggPrefab0 (white)
                            GameObject obj1 = Instantiate(eggPrefab1,new Vector3(x, y, 0), Quaternion.identity);
                            obj1.transform.parent = prefabs.transform;
                            spawnedPiecesList.Add(obj1);    //add obj1 to the list
                            break;
                        case 2:     //if grid value is 2, generate eggPrefab0 (rainbow)
                            GameObject obj2 = Instantiate(eggPrefab2,new Vector3(x, y, 0), Quaternion.identity);
                            obj2.transform.parent = prefabs.transform;
                            spawnedPiecesList.Add(obj2);    //add obj2 to the list
                            break;
                        case 3:     //if grid value is 3, don't do anything
                            break;
                    }
            }
        }
    }

    public bool ConnectThree(bool shouldReplace)
    {
        //shouldReplace is true when we are playing the game
        //shouldReplace is false when we are just generating grids and their values
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y < 2)      //check only 2 grids per column
                {
                    //if the grid has the same value as the grid to the right
                    //and also the same as the grid further right
                    //but the grid's value isn't 3, which is for eggs that have already matched in the game
                    if (grid[x, y] == grid[x, y + 1] && grid[x, y] == grid[x, y + 2] && grid[x,y] != 3)
                    {
                        //Debug.Log("column" + grid[x,y]);
                        
                        //assign each grid's value to gridXY
                        gridXY = grid[x, y];
                        
                        if (shouldReplace)      //if we have a matching 3
                        {
                            //we replace all three grids with cracked eggs
                            ReplaceThreeVer2(x,y);
                            ReplaceThreeVer2(x,y + 1);
                            ReplaceThreeVer2(x,y + 2);
                            ConnectThree(shouldReplace);     //run this function again to check the board once over
                                                             //until shouldReplace is false
                        }
                        else
                        {
                            //if we don't have a matching 3
                            //ConnectThree returns true
                            return true;
                        }
                    }
                }

                if (x < 2)
                {
                    if (grid[x, y] == grid[x + 1, y] && grid[x, y] == grid[x + 2, y] && grid[x,y] != 3)
                    {
                        //Debug.Log("row" + grid [x,y]);
                        gridXY = grid[x, y];
                        rowThree = true;
                        if (shouldReplace)
                        {
                            ReplaceThreeVer2(x,y);
                            ReplaceThreeVer2(x+1,y);
                            ReplaceThreeVer2(x+2,y);
                            ConnectThree(shouldReplace);
                        }
                        return true;
                    }
                }

                
            }
        }
        //after evaluating the board through the loops, we can't find any matches and return false
        return false;
    }

    void ReplaceThreeVer2(int x, int y)
    {
        //just to make sure we have 16 spawned pieces
        //Debug.Log(spawnedPiecesList.Count);
        
        //for each item in the 
        foreach (GameObject item in spawnedPiecesList)
        {
            //Debug.Log(item.transform.position);

            //we're too tired to make a dictionary holding the eggs' position in the 2D array
            //but we realized their position in world was the same as as 2D array position
            //so we decided to cheat lol
            if (item.transform.position == new Vector3(x, y, 0))
            {
                //give the grid a new value: 3
                grid[x, y] = 3;
                
                //if the original value of the grid is...
                switch (gridXY)
                {
                    case 0:
                        //change the egg sprite to cracked brown
                        newEgg = Instantiate(brownEgg, new Vector3(x,y,0), Quaternion.identity);
                        break;
                    case 1:
                        //change the egg sprite to cracked white
                        newEgg = Instantiate(whiteEgg, new Vector3(x,y,0), Quaternion.identity);
                        break;
                    case 2:
                        //change the egg sprite to cracked rainbow
                        newEgg = Instantiate(rainbowEgg, new Vector3(x,y,0), Quaternion.identity);
                        break;
                    default:
                        newEgg = null;
                        break;
                }
                
                //insert the egg to the original egg's position
                spawnedPiecesList.Insert(y * 4 + x, newEgg);
                //and then remove and destroy the original egg so the list index is not messed up
                spawnedPiecesList.Remove(item);
                Destroy(item);
                break;
            }
        }
    }
    //
    // void ReInstantiate(int a, int b)
    // {
    //     Debug.Log("destroyed" + "" +(a * 10 + b));
    //     Destroy(spawnedPieces["" +(a * 10 + b)]);
    //
    //     for (var y = 0; y < grid.GetLength(1); y++)
    //     {
    //         for (var x = 0; x < grid.GetLength(0); x++)
    //         {
    //             if (grid[x, y] == -2)
    //             {
    //                 grid[x, y] = Random.Range(0, 3);
    //             }
    //             InstantiatePrefab();
    //         }
    //     }
    // }
}
