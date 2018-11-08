using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBoardManager : MonoBehaviour {

    //Singleton, allows it to be called from any script
    public static MyBoardManager instance;

    // A list of sprites can be used as tile pieces to random the board
    public List<Sprite> characters = new List<Sprite>();

    // the prefab will be instantiated when create new board
    public GameObject tile;

    // store the tiles in the board
    private GameObject[,] tiles;

    // the x and y of the dimension of the board
    public int xSize, ySize;

    public bool IsShifting { get; set; }
    // Use this for initialization
    void Start() {
        instance = GetComponent<MyBoardManager>();

        // the size of the tile
        Vector2 offSet = tile.GetComponent<SpriteRenderer>().bounds.size;

        CreateBoard(offSet.x, offSet.y);
    }
    void CreateBoard(float xOffset, float yOffset)
    {
        // create the board
        tiles = new GameObject[xSize, ySize];

        // the current transform position of the board manager when the game started
        float startX = transform.position.x;
        float startY = transform.position.y;

        // these variables hold a reference to adjacent tiles so you can replace their characters
        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        // create 2d array 
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                // instantiate the tile into the 2d array
                GameObject newTile = Instantiate(tile, new Vector2(startX + (xOffset * x), startY + (yOffset * y)), tile.transform.rotation);
                tiles[x, y] = newTile;
                
                // make all the instantiated tiles the children of the Board Manager game object to make the hierarchy clean
                newTile.transform.parent = transform;

                // create a new list of possible characters to prevent the game from having 3 match at the start
                List<Sprite> possibleCharacters = new List<Sprite>();
                possibleCharacters.AddRange(characters);

                // remove the sprite of the characters that are on the left and below the current sprite from the list of possible characters
                possibleCharacters.Remove(previousLeft[y]);
                possibleCharacters.Remove(previousBelow);

                // random the tile sprites
                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

                // assign the new sprite to both the tile left and below the current one for the next iteration of the loop to use
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }
}
