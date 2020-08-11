using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.UIElements;

public class StackManager : MonoBehaviour
{
    private const float TILEBOUNDS = 4f;
    private const float ERROR_MARGIN = 0.1f;

    [SerializeField] GameObject[] stackArray;
    private float yTransform = 0;
    private int stackIndex;
    private float tileTransition = 0f;
    private int tileCombo;
    [SerializeField] float tileSpeed;
    private bool isTileMovingOnX;
    private bool isGameOver;
    private float updatedPosition;
    private Vector3 stackPosition;
    private Vector3 previousTilePosition;
    private Vector2 currentTileBounds;

    [SerializeField] Color32[] tileColors = new Color32[4];
    [SerializeField] Material tileMaterial;

    // Start is called before the first frame update
    void Start()
    {
        stackArray = new GameObject[transform.childCount];  //initializes array size to number of children
        for (int i = 0; i < transform.childCount; i++)
        {
            stackArray[i] = transform.GetChild(i).gameObject;
            ColorTile(stackArray[i].GetComponent<MeshFilter>().mesh);
        }

        stackIndex = transform.childCount - 1;  // Since the lowest tile is pushed to the top, stackIndex is set to lowest tile number
        isTileMovingOnX = true;
        tileCombo = 0;
        currentTileBounds = new Vector2(TILEBOUNDS, TILEBOUNDS);
        isGameOver = false;

    }

    // Update is called once per frame
    void Update()
    {
        MoveTile();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (TilePlacedSuccessfully())
            {
                SpawnTileOnTop();           // Scale of tile is 0.25, incrementing value by               
                yTransform += 0.25f;        // 1 creates gaps in stack, so value is increased by 0.25
            }
            else
            {
                EndGame();
            }
        }

        transform.position = Vector3.Lerp(transform.position, stackPosition, 5f * Time.deltaTime);
    }

    private void EndGame()
    {
        isGameOver = true;
        stackArray[stackIndex].AddComponent<Rigidbody>();
    }

    private bool TilePlacedSuccessfully()
    {
        Transform currentTileTransform = stackArray[stackIndex].transform;  
        if (isTileMovingOnX)
        {
            float deltaX = previousTilePosition.x - currentTileTransform.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                // Tile cutting logic
                tileCombo = 0;
                currentTileBounds.x -= Mathf.Abs(deltaX);   // Reduces tile bounds after failed attempt
                if (currentTileBounds.x <= 0)
                    return false;

                float midPoint = previousTilePosition.x + currentTileTransform.localPosition.x / 2;
                currentTileTransform.localScale = new Vector3(currentTileBounds.x, 0.25f, currentTileBounds.y);
                // Spawn tile rubble                    
                SpawnTileRubble(
                    new Vector3((currentTileTransform.position.x > 0)
                                 ? currentTileTransform.position.x + (currentTileTransform.localScale.x / 2)
                                 : currentTileTransform.position.x - (currentTileTransform.localScale.x / 2),
                                 currentTileTransform.position.y, currentTileTransform.position.z),
                    new Vector3(Mathf.Abs(deltaX), 0.25f, currentTileTransform.localScale.z)
                    );
                currentTileTransform.localPosition = new Vector3(midPoint - (previousTilePosition.x / 2),
                                                      yTransform, previousTilePosition.z);
            }
            else    
            {
                // Tile is placed perfectly, without losing any part
                if(tileCombo > 3)
                {
                    currentTileBounds.x += 0.25f;   // Increases tile size for 3 successful placements
                    if (currentTileBounds.x > TILEBOUNDS)
                        currentTileBounds.x = TILEBOUNDS;   // Makes sure tile boundsnever goes past the set limit  
                    float midPoint = previousTilePosition.x + currentTileTransform.localPosition.x / 2;
                    currentTileTransform.localScale = new Vector3(currentTileBounds.x, 0.25f, currentTileBounds.y);
                    currentTileTransform.localPosition = new Vector3(midPoint - (previousTilePosition.x / 2),
                                                          yTransform, previousTilePosition.z);
                }
                tileCombo += 1;
                currentTileTransform.localPosition = new Vector3(previousTilePosition.x, yTransform,
                                                                 previousTilePosition.z);
            }
        }
        else
        {
            float deltaZ = previousTilePosition.z - currentTileTransform.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {
                // Tile cutting logic
                tileCombo = 0;
                currentTileBounds.y -= Mathf.Abs(deltaZ);   // Reduces tile bounds after failed attempt
                if (currentTileBounds.y <= 0)
                    return false;

                float midPoint = previousTilePosition.z + currentTileTransform.localPosition.z / 2;
                currentTileTransform.localScale = new Vector3(currentTileBounds.x, 0.25f, currentTileBounds.y);
                // Spawn tile rubble
                SpawnTileRubble(
                                new Vector3(currentTileTransform.position.x, currentTileTransform.position.y,
                                           (currentTileTransform.position.z > 0)
                                           ? currentTileTransform.position.z + (currentTileTransform.localScale.z / 2)
                                           : currentTileTransform.position.z - (currentTileTransform.localScale.z / 2)),

                                  //new Vector3(Mathf.Abs(deltaZ), 0.25f, currentTileTransform.localScale.z)
                                  new Vector3(currentTileTransform.localScale.x, 0.25f, Mathf.Abs(deltaZ))  
                                );
                currentTileTransform.localPosition = new Vector3(previousTilePosition.x, yTransform,
                                                      midPoint - (previousTilePosition.z / 2));
            }
            else    
            {
                // Tile is placed perfectly, without losing any part
                if (tileCombo > 3)
                {
                    currentTileBounds.y += 0.25f;   // Increases tile size for 3 successful placements
                    if (currentTileBounds.y > TILEBOUNDS)
                        currentTileBounds.y = TILEBOUNDS;   // Makes sure tile boundsnever goes past the set limit  
                    float midPoint = previousTilePosition.z + currentTileTransform.localPosition.z / 2;
                    currentTileTransform.localScale = new Vector3(currentTileBounds.x, 0.25f, currentTileBounds.y);
                    currentTileTransform.localPosition = new Vector3(previousTilePosition.x, yTransform,
                                                          midPoint - (previousTilePosition.z / 2));
                }
                tileCombo += 1;
                currentTileTransform.localPosition = new Vector3(previousTilePosition.x, yTransform,    
                                                                 previousTilePosition.z); 
            }
        }

        updatedPosition = (isTileMovingOnX)
            ? stackArray[stackIndex].transform.localPosition.x     // Checks boolean, then spawns new tile 
            : stackArray[stackIndex].transform.localPosition.z;    // on top of new tile, instead of spawning it at origin
        isTileMovingOnX = !isTileMovingOnX;     // Reverses value of the boolean every time a tile is placed
         
        return true;
    }

    private void MoveTile()
    {
        if (isGameOver)   // Stops moving tile if gameOver boolean is set
            return;

        tileTransition += tileSpeed * Time.deltaTime;   // Used for moving tile forward and back
        if (isTileMovingOnX)
        {
            stackArray[stackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * TILEBOUNDS,
                                                                         yTransform, updatedPosition);
        }
        else
        {
            stackArray[stackIndex].transform.localPosition = new Vector3(updatedPosition, yTransform,
                                                                         Mathf.Sin(tileTransition) * TILEBOUNDS);
        }
    }

    private void SpawnTileOnTop()
    {
        previousTilePosition = stackArray[stackIndex].transform.localPosition;
        stackIndex--;
        if(stackIndex < 0)                          // Resets stackIndex to avoid 
        {                                           // out of bounds error
            stackIndex = transform.childCount - 1;  
        }
        stackPosition = Vector3.down * yTransform;  // Moves the stack down so it always stays in the view of the camera
        stackArray[stackIndex].transform.localPosition = new Vector3(0, yTransform, 0);
        stackArray[stackIndex].transform.localScale = new Vector3(currentTileBounds.x,    // Spawns new tile with new dimensions after cutting          
                                                                  0.25f, currentTileBounds.y);

        ColorTile(stackArray[stackIndex].GetComponent<MeshFilter>().mesh);
    }

    private void SpawnTileRubble(Vector3 position, Vector3 scale)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gameObject.transform.localPosition = position;
        gameObject.transform.localScale = scale;gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<MeshRenderer>().material = tileMaterial;
        ColorTile(gameObject.GetComponent<MeshFilter>().mesh);
    }

    private Color32 SelectTileColor(Color32 color1, Color32 color2, Color32 color3, Color32 color4, float transition)
    {
        if(transition < 0.33f)
        {
            return Color.Lerp(color1, color2, transition / 0.33f);
        }
        else if(transition < 0.66f)
        {
            return Color.Lerp(color2, color3, (transition - 0.33f) / 0.66f);
        }
        else
            return Color.Lerp(color3, color4, (transition - 0.66f) / 0.33f);
    }

    private void ColorTile(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(yTransform * 0.25f);

        for(int i = 0; i < vertices.Length; i++)
        {
            colors[i] = SelectTileColor(tileColors[0], tileColors[1], tileColors[2], tileColors[3], f);
        }
        mesh.colors32 = colors;
    }
}
