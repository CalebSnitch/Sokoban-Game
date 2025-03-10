using System;
//using Mono.Cecil.Cil;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
   
    public GameObject Player;
    private GridObject PlayerBlock;
    public GameObject[,] GRID = new GameObject[12,7]; //if the size is changed change the clear function to (bottom of update)

    private void Start()
    {
        PlayerBlock = Player.GetComponent<GridObject>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) == true)
        {
            PlayerMove(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.S) == true)
        {
            PlayerMove(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.A) == true)
        {
            PlayerMove(-1, 0);
        }
        if (Input.GetKeyDown(KeyCode.D) == true)
        {
            PlayerMove(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q) == true)
        {
            //Debug.Log(GRID[PlayerBlock.gridPosition.x, PlayerBlock.gridPosition.y - 1]);
        }

        //end of update: 
        for(int i = 0; i < 12; i++)
        {
            for(int ii = 0; i < 7; i++)
            {
                GRID[i, ii] = null;
            }
        }
    }

    private bool PlayerCheckMovement(int _dx, int _dy)
    {
       int _new_x = PlayerBlock.gridPosition.x + _dx;
       int _new_y = PlayerBlock.gridPosition.y + _dy;

       if (CheckGridSpaceExists(_new_x, _new_y) == true)
       {
            GameObject _target_location = GRID[_new_x, _new_y];


            if (_target_location == null)
            {
                return true;
            }
            else 
            {
                Block _target_block = _target_location.GetComponent<Block>();
                
                if (_target_block.blockType == "wall")
                {
                    return false;
                }
                else if (_target_block.blockType == "smooth")
                {
                    return SmoothCheckMovement(_dx, _dy, _target_location);
                }
                else if (_target_block.blockType == "sticky")
                {
                    return StickyCheckMovement( _dx, _dy, _target_location);
                }
                else if (_target_block.blockType == "clingy")
                {
                    return false;
                }
                else //to prevent "not all code paths return a value"
                {
                    return false;
                }
            }
       }
       else
       {
            return false;
       }
    }

    private bool CheckGridSpaceExists(int _new_x, int _new_y)
    {
        if (_new_x <= 10 && _new_x > 0 && _new_y <= 5 && _new_y > 0)
       {
            return true;
       }
       else 
       {
            return false;
       }
    }

    private bool SmoothCheckMovement(int _dx, int _dy, GameObject block)
    {
        Block blockBlock = block.GetComponent<Block>();
        GridObject blockGridObject = block.GetComponent<GridObject>();
        int _new_x = blockGridObject.gridPosition.x + _dx;
        int _new_y = blockGridObject.gridPosition.y + _dy;

        if (_dx == 0 && _dy == 0)
        {
            return false;
        }
         
        
        if (CheckGridSpaceExists(_new_x, _new_y))
        {
             GameObject _target_location = GRID[_new_x, _new_y];

            if (_target_location == null)
            {
                return true;
            }
            else 
            {
                Block _target_block = _target_location.GetComponent<Block>();
                
                if (_target_block.blockType == "wall")
                {
                    return false;
                }
                else if (_target_block.blockType == "smooth")
                {
                    return SmoothCheckMovement(_dx, _dy, _target_location);
                }
                else if (_target_block.blockType == "sticky")
                {
                    return StickyCheckMovement(_dx, _dy, _target_location);
                }
                else if (_target_block.blockType == "clingy")
                {
                    return false;
                }
                else //to prevent "not all code paths return a value"
                {
                    return false;
                }
            }
        }
        else 
        {
            return false;
        }
    }

    private bool StickyCheckMovement(int _dx, int _dy, GameObject block)
    {
       Block blockBlock = block.GetComponent<Block>();
        GridObject blockGridObject = block.GetComponent<GridObject>();
        int _new_x = blockGridObject.gridPosition.x + _dx;
        int _new_y = blockGridObject.gridPosition.y + _dy;
         
        
        if (CheckGridSpaceExists(_new_x, _new_y))
        {
            GameObject _target_location = GRID[_new_x, _new_y];

            if (_target_location == null)
            {
                return true;
            }
            else 
            {
                Block _target_block = _target_location.GetComponent<Block>();
                
                if (_target_block.blockType == "wall")
                {
                    return false;
                }
                else if (_target_block.blockType == "smooth")
                {
                    if (_target_block.has_moved == true)
                    {
                        return false;
                        
                    }
                    else
                    {
                        return SmoothCheckMovement(_dx, _dy, _target_location);
                    }
                }
                else if (_target_block.blockType == "sticky")
                {
                    return StickyCheckMovement(_dx, _dy, _target_location);
                }
                else if (_target_block.blockType == "clingy")
                {
                    return false;
                }
                else  //to prevent "not all paths return a value"
                {
                    return false;
                }
            }
        }
        else
        {
            return false;
        }
    }


    //moves the objects:

    private void PlayerMove( int _dx, int _dy)
    {
        int _x = PlayerBlock.gridPosition.x;
        int _y = PlayerBlock.gridPosition.y;

        if (PlayerCheckMovement(_dx, _dy))
        {
            //moves the block where you're going:
            GameObject _check_location = GRID[_x + _dx, _y + _dy];

            if (_check_location != null)
            {
                Block _test_block = _check_location.GetComponent<Block>();

                if (_test_block.blockType == "smooth")
                {
                    SmoothMove(_dx, _dy, _check_location);
                }
                else if (_test_block.blockType == "sticky")
                {
                    StickyMove(_dx, _dy, _check_location);
                }
            }

            //moves the block to the first side of you:
            _check_location = GRID[_x + _dy, _y + _dx];

            if (_check_location != null)
            {
                Block _test_block = _check_location.GetComponent<Block>();

                if (_test_block.blockType == "sticky")
                {
                    StickyMove(_dx, _dy, _check_location);
                }
            }

            //moves the block to the other side of you:
            _check_location = GRID[_x - _dy, _y - _dx];

            if (_check_location != null)
            {
                Block _test_block = _check_location.GetComponent<Block>();

                if (_test_block.blockType == "sticky")
                {
                    StickyMove(_dx, _dy, _check_location);
                }
            }

           //moves you:

            Player.GetComponent<Block>().has_moved = true;
            PlayerBlock.gridPosition.x += _dx;
            PlayerBlock.gridPosition.y += _dy;
            GRID[PlayerBlock.gridPosition.x, PlayerBlock.gridPosition.y] = Player;
            if (GRID[_x, _y] == Player)
            {
                GRID[_x, _y] = null;
            }

            //moves the block behind you:

              _check_location = GRID[_x - _dx, _y - _dy];

            if (_check_location != null)
            {
                Block _test_block = _check_location.GetComponent<Block>();

                if (_test_block.blockType == "clingy")
                    {
                        if (_test_block.has_moved == false)
                        {
                            ClingyMove(_dx, _dy, _check_location);
                        }
                    }
                    else if (_test_block.blockType == "sticky")
                    {
                        if (_test_block.has_moved == false)
                        {
                            StickyMove(_dx, _dy, _check_location);
                        }
                    }
            }
        }
    }

//SMOOTH MOVE:
    private void SmoothMove(int _dx, int _dy, GameObject block)
    {
       if(block.GetComponent<Block>().has_moved == false)
       {
            block.GetComponent<Block>().has_moved = true;
            GridObject blockGridObject = block.GetComponent<GridObject>();
            int _x = blockGridObject.gridPosition.x;
            int _y = blockGridObject.gridPosition.y;

            GameObject _check_location = GRID[_x + _dx, _y + _dy];

                if (_check_location != null)
                {
                    Block _test_block = _check_location.GetComponent<Block>();

                    if (_test_block.blockType == "smooth")
                    {
                        SmoothMove(_dx, _dy, _check_location);
                    }
                    else if (_test_block.blockType == "sticky")
                    {
                        StickyMove(_dx, _dy, _check_location);
                    }
                }

                //moves the block to the first side of you:
                _check_location = GRID[_x + _dy, _y + _dx];

                if (_check_location != null)
                {
                    Block _test_block = _check_location.GetComponent<Block>();

                    if (_test_block.blockType == "sticky")
                    {
                        StickyMove(_dx, _dy, _check_location);
                    }
                }

                //moves the block to the other side of you:
                _check_location = GRID[_x - _dy, _y - _dx];

                if (_check_location != null)
                {
                    Block _test_block = _check_location.GetComponent<Block>();

                    if (_test_block.blockType == "sticky")
                    {
                        StickyMove(_dx, _dy, _check_location);
                    }
                }

            //moves you:

                block.GetComponent<Block>().has_moved = true;
                blockGridObject.gridPosition.x += _dx;
                blockGridObject.gridPosition.y += _dy;
                GRID[blockGridObject.gridPosition.x, blockGridObject.gridPosition.y] = block;
                if (GRID[_x, _y] == block)
                {    
                    GRID[_x, _y] = null;
                }

                //moves the block behind you:

                _check_location = GRID[_x - _dx, _y - _dy];

                if (_check_location != null)
                {
                    Block _test_block = _check_location.GetComponent<Block>();

                    if (_test_block.blockType == "clingy")
                    {
                        if (_test_block.has_moved == false)
                        {
                            ClingyMove(_dx, _dy, _check_location);
                        }
                    }
                    else if (_test_block.blockType == "sticky")
                    {
                        if (_test_block.has_moved == false)
                        {
                            StickyMove(_dx, _dy, _check_location);
                        }
                    }
                }
              }
        }

    private void StickyMove(int _dx, int _dy, GameObject block)
    {
        if (block.GetComponent<Block>().has_moved == false )
        {
            block.GetComponent<Block>().has_moved = true;
            if(StickyCheckMovement(_dx, _dy, block))
                {
                    GridObject blockGridObject = block.GetComponent<GridObject>();
                    int _x = blockGridObject.gridPosition.x;
                    int _y = blockGridObject.gridPosition.y;

                    //moves the block ahead of you:
                    GameObject _check_location = GRID[_x + _dx, _y + _dy];

                    if (_check_location != null)
                        {
                           Block _test_block = _check_location.GetComponent<Block>();

                           if (_test_block.blockType == "smooth")
                            {
                                SmoothMove(_dx, _dy, _check_location);
                            }
                            else if (_test_block.blockType == "sticky")
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                        }

                    //moves the block to the first side of you:
                    _check_location = GRID[_x + _dy, _y + _dx];

                    if (_check_location != null)
                        {
                            Block _test_block = _check_location.GetComponent<Block>();

                            if (_test_block.blockType == "sticky")
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                        }

                        //moves the block to the other side of you:
                        _check_location = GRID[_x - _dy, _y - _dx];

                        if (_check_location != null)
                        {
                            Block _test_block = _check_location.GetComponent<Block>();

                            if (_test_block.blockType == "sticky")
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                        }

                        //moves you:

                        blockGridObject.gridPosition.x += _dx;
                        blockGridObject.gridPosition.y += _dy;
                        GRID[blockGridObject.gridPosition.x, blockGridObject.gridPosition.y] = block;
                        if (GRID[_x, _y] == block)
                        {
                            GRID[_x, _y] = null;
                        }
                        //moves the block behind you:

                        _check_location = GRID[_x - _dx, _y - _dy];

                        if (_check_location != null)
                        {
                            Block _test_block = _check_location.GetComponent<Block>();

                           if (_test_block.blockType == "clingy")
                            {
                                if (_test_block.has_moved == false)
                                {
                                ClingyMove(_dx, _dy, _check_location);
                                }
                                }
                            else if (_test_block.blockType == "sticky")
                            {
                            if (_test_block.has_moved == false)
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                            }
                        }
                }
        }
    }

    private void ClingyMove(int _dx, int _dy, GameObject block)
    {
         if (block.GetComponent<Block>().has_moved == false )
        {
            block.GetComponent<Block>().has_moved = true;
            if(StickyCheckMovement(_dx, _dy, block))
                {
                    GridObject blockGridObject = block.GetComponent<GridObject>();
                    int _x = blockGridObject.gridPosition.x;
                    int _y = blockGridObject.gridPosition.y;

                    GameObject _check_location = GRID[_x + _dx, _y + _dy];

                    if (_check_location != null)
                        {
                           Block _test_block = _check_location.GetComponent<Block>();

                           if (_test_block.blockType == "smooth")
                            {
                                SmoothMove(_dx, _dy, _check_location);
                            }
                            else if (_test_block.blockType == "sticky")
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                        }

                    //moves the block to the first side of you:
                    _check_location = GRID[_x + _dy, _y + _dx];

                    if (_check_location != null)
                        {
                            Block _test_block = _check_location.GetComponent<Block>();

                            if (_test_block.blockType == "sticky")
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                        }

                        //moves the block to the other side of you:
                        _check_location = GRID[_x - _dy, _y - _dx];

                        if (_check_location != null)
                        {
                            Block _test_block = _check_location.GetComponent<Block>();

                            if (_test_block.blockType == "sticky")
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                        }

                        //moves you:

                        blockGridObject.gridPosition.x += _dx;
                        blockGridObject.gridPosition.y += _dy;
                        GRID[blockGridObject.gridPosition.x, blockGridObject.gridPosition.y] = block;
                        if (GRID[_x, _y] == block)
                        {
                            GRID[_x, _y] = null;
                        }
                        //moves the block behind you:

                        _check_location = GRID[_x - _dx, _y - _dy];

                        if (_check_location != null)
                        {
                            Block _test_block = _check_location.GetComponent<Block>();

                           if (_test_block.blockType == "clingy")
                            {
                                if (_test_block.has_moved == false)
                                {
                                ClingyMove(_dx, _dy, _check_location);
                                }
                                }
                            else if (_test_block.blockType == "sticky")
                            {
                            if (_test_block.has_moved == false)
                            {
                                StickyMove(_dx, _dy, _check_location);
                            }
                            }
                        }
                }
        }
    }


}