using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class Block: MonoBehaviour
{

    private GridObject self_gridobject; 

    private GameObject roomGrid;
    private Manager manager;

    public bool has_moved = false;

    public string blockType;

    void Start()
    {


        //places the object in the grid:
        roomGrid = GameObject.Find("Grid");
        manager = roomGrid.GetComponent<Manager>();
        self_gridobject = this.GetComponent<GridObject>();
        manager.GRID[self_gridobject.gridPosition.x, self_gridobject.gridPosition.y] = gameObject;

    
        
    }

    
    void Update()
    {
        has_moved = false;

        
    }

    void LateUpdate()
    {
        manager.GRID[self_gridobject.gridPosition.x, self_gridobject.gridPosition.y] = gameObject;
    }
}