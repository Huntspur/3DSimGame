using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectsDatabaseSO data;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualisation;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int objectIndex)
    {
        StopPlacement();
        selectedObjectIndex = data.objectsData.FindIndex(data => data.ID == objectIndex);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError("No id found");
            return;
        }
        gridVisualisation.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualisation.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    private void PlaceStructure()
    {
       if(inputManager.IsPointerOverUI())
       {
           return;
       }
       Vector3 mousePos = inputManager.GetSelecMapPosition();
       Vector3Int gridPosition = grid.WorldToCell(mousePos);
       GameObject newObj = Instantiate(data.objectsData[selectedObjectIndex].prefab);
       newObj.transform.position = grid.CellToWorld(gridPosition);
    }

    private void Update()
    {
        if(selectedObjectIndex < 0)
        {
            return;
        }
        Vector3 mousePos = inputManager.GetSelecMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePos);
        mouseIndicator.transform.position = mousePos;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
}
