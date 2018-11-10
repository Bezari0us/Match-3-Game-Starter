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

    public IEnumerator FindNullTiles()
    {
        // loop through to find empty tiles
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }
        // when shitf tiles down, there a chance that player get combos so we need to clear all the combo matches as well
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<MyTile>().ClearAllMatches();
            }
        }
    }
    private IEnumerator ShiftTilesDown(int x, int yStart, float delay=0.03f)
    {
        IsShifting = true;    
        // create a list game object to count empty spaces
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {
            // loop through and count the empty spaces
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if(render.sprite==null)
            {
                nullCount++;
            }
            renders.Add(render);
        }

        // loop the through the empty space space again
        for (int i = 0; i < nullCount; i++)
        {
            // wait 
            yield return new WaitForSeconds(delay);
            // loop through every sprite renderers in the list of renders
            for (int k=0; k<renders.Count-1; k++)
            {
                // swap each sprite with the one above it, until the end is reached and the last sprite is set to null 
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }       
        }
        IsShifting = false;
    }

    // get new sprite for the tiles shifted down
    private Sprite GetNewSprite(int x,int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);
        // need comment
        if(x>0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        // need comment
        if(x>xSize-1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        // need comment
        if(y>0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }
        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }
}
