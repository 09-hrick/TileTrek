using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It Handles the spawning of the 10*10 floor cube Grid in the 3d Game Space 
/// </summary>
public class FloorSpawnManager : MonoBehaviour
{
    //Cube Prefab reference
    [SerializeField] GameObject Cube;

    //Empty Grid parent game object for cubes
    [SerializeField] GameObject Grid;

    ///<Summary>
    ///Spawn the 10X10 Grid of Cubes in the game with Grid as their parent
    /// </Summary>
    void SpawnCubes()
    {
        for (int i = 0; i < 10; i++) { 
            for(int j = 0; j < 10; j++)
            {
                GameObject cubeInstance = Instantiate(Cube);
                cubeInstance.transform.SetParent(Grid.transform, false);
                cubeInstance.transform.position = new Vector3(i,0,j); //offset the x and z position by 1
            }
        }
    }

    void Start()
    {
        SpawnCubes();
    }
}
