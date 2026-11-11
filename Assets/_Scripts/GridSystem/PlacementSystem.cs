using System;
using UnityEngine;
using System.Collections.Generic;
public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO data;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualisation;

    private GridData floorData, furnitureData;

    [SerializeField]
    private PreviewSystem previewSystem;

    private List<GameObject> placedGameObjects = new();
   
    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private LayerMask wallLayer;          
    [SerializeField] private float wallCheckHalfHeight = 1f; 
    [SerializeField] private float wallCheckPadding = 0.05f; 

    private void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int objectIndex) //called when the player clicks on a button to place an object in the grid. The objectIndex parameter is used to identify which object the player wants to place. This method sets up the necessary variables and enables the grid visualisation and cell indicator, allowing the player to see where they can place the object. It also subscribes to the OnClicked and OnExit events of the inputManager, so that when the player clicks or exits the placement mode, the appropriate methods are called.
    {
        StopPlacement();//stop any previous placement before starting a new one
        selectedObjectIndex = data.objectsData.FindIndex(data => data.ID == objectIndex);//find the index of the object in the database based on its ID. This is used to identify which object the player wants to place in the grid.
        if (selectedObjectIndex < 0) //if the object is not found in the database, log an error and return
        {
            Debug.LogError("No id found");
            return;
        }

        //enable the grid visualisation and cell indicator, allowing the player to see where they can place the object
        gridVisualisation.SetActive(true);
        previewSystem.StartShowingPlacementPreview(data.objectsData[selectedObjectIndex].prefab, data.objectsData[selectedObjectIndex].Size);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void StopPlacement() //called when the player exits the placement mode or clicks on a button to stop placing an object. This method resets the selectedObjectIndex to -1, disables the grid visualisation and cell indicator, and unsubscribes from the OnClicked and OnExit events of the inputManager, so that the appropriate methods are no longer called.
    {
        selectedObjectIndex = -1;
        gridVisualisation.SetActive(false);
        previewSystem.StopShowingPreview();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
    }

    private void PlaceStructure()//called when the player clicks on the grid to place an object. This method checks if the player is currently placing an object and if the mouse pointer is not over a UI element. If both conditions are met, it gets the mouse position and converts it to a grid position using the WorldToCell method of the grid. It then instantiates a new object based on the selectedObjectIndex and sets its position to the grid position using the CellToWorld method of the grid.
    {
       if(inputManager.IsPointerOverUI())
       {
           return;
       }
        //get the mouse position and convert it to a grid position using the WorldToCell method of the grid
        Vector3 mousePos = inputManager.GetSelectMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex); //check if the object can be placed at the specified position in the grid based on its size and the positions that are already occupied by other objects
        if (!placementValidity)
        {
            return;
        }

        GameObject newObj = Instantiate(data.objectsData[selectedObjectIndex].prefab);
        newObj.transform.position = grid.CellToWorld(gridPosition);

        placedGameObjects.Add(newObj);
        GridData selectedData = data.objectsData[selectedObjectIndex].ID == 0 ? furnitureData : furnitureData;
        selectedData.AddObjectAt(gridPosition, data.objectsData[selectedObjectIndex].Size, data.objectsData[selectedObjectIndex].ID, placedGameObjects.Count - 1);

        previewSystem.UpdatePreviewPosition(grid.CellToWorld(gridPosition), false);

    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        Vector2Int size = data.objectsData[selectedObjectIndex].Size;

        if (IsOverlappingWall(gridPosition, size))     
        { 
            return false;
        }

        GridData selectedData = data.objectsData[selectedObjectIndex].ID == 0 ? furnitureData : furnitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, size);
    }
    private bool IsOverlappingWall(Vector3Int gridPosition, Vector2Int size)
    {
        Vector3 cellSize = grid.cellSize;
        Vector3 origin = grid.CellToWorld(gridPosition);

        float halfX = size.x * cellSize.x * 0.5f;
        float halfZ = size.y * cellSize.z * 0.5f;

        Vector3 center = origin + new Vector3(halfX, wallCheckHalfHeight, halfZ);
        Vector3 halfExtents = new Vector3(halfX - wallCheckPadding, wallCheckHalfHeight, halfZ - wallCheckPadding);

        return Physics.CheckBox(center, halfExtents, Quaternion.identity, wallLayer);
    }
    private void Update()
    {
        if(selectedObjectIndex < 0)
        {
            return;
        }
        //get the mouse position and convert it to a grid position using the WorldToCell method of the grid. Then, set the position of the mouse indicator and cell indicator to the mouse position and grid position, respectively.
        Vector3 mousePos = inputManager.GetSelectMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);
        if(lastDetectedPosition != gridPosition)
        {
            bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

            mouseIndicator.transform.position = mousePos;
            previewSystem.UpdatePreviewPosition(grid.CellToWorld(gridPosition), placementValidity);
                    lastDetectedPosition = gridPosition;

        }
       
    }
}
