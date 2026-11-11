using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new(); //C# 9 allows this syntax for initializing a new dictionary instead of rewriting the type name.

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int iD, int placedObjectIndex) 
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData placementData = new PlacementData(positionsToOccupy, iD, placedObjectIndex); //create a new instance of the PlacementData class with the occupied positions, ID, and placed object index
                                                                                                   //which are passed in as parameters. This instance is then added to the placedObjects dictionary with the grid position as the key.
       
        foreach (var pos in positionsToOccupy) //check if the position is already occupied before adding the new object to the grid
        {
            if(placedObjects.ContainsKey(pos))
            {
                // Handle the case where the position is already occupied
                throw new Exception($"Position {pos} is already occupied by another object.");
            }
            placedObjects[pos] = placementData;//add the new object to the grid at the specified position
        }


    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize) //calculate the positions that the object will occupy in the grid based on its size and the grid position where it is being placed
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)  
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize) //check if the object can be placed at the specified position in the grid based on its size and the positions that are already occupied by other objects
    {
        List<Vector3Int> positionsToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionsToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false; //if any of the positions that the object will occupy are already occupied by another object, return false
            }
        }
        return true; //if all of the positions that the object will occupy are unoccupied, return true
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions; //list is reference type, so we can modify it without needing to create a new instance of the class
    public int ID { get; private set; } //ID of the object placed in the grid, useful for saving/loading the game state. This is set when the object is placed in the grid and cannot be modified afterwards.
    public int PlacedObjectIndex { get; private set; }//When we want to remove or create object system, we need to know which object in the database was placed in the grid. This is set when the object is placed in the grid and cannot be modified afterwards.

    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex) //constructor for the PlacementData class, which takes in a list of occupied positions, an ID, and a placed object index. The ID and placed object index are set as read-only properties, while the occupied positions can be modified after the object is placed in the grid.
    {
        ID = id;
        this.PlacedObjectIndex = placedObjectIndex;
        this.occupiedPositions = occupiedPositions;
    }
}