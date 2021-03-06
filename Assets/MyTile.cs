﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTile : MonoBehaviour
{
    private bool isSelected = false;
    private static MyTile previousSelected = null;
    private SpriteRenderer render;

    // the color will be applied to the sprite was clicked
    private static Color selectedColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    // direction to cast the ray for adjacent tiles
    private Vector2[] adjacentTilesDir = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool matchFound = false;
    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    // select
    void Select()
    {
        isSelected = true;
        previousSelected = gameObject.GetComponent<MyTile>();
        render.color = selectedColor;
        SFXManager.instance.PlaySFX(Clip.Select);
    }

    // deselect
    void Deselect()
    {
        isSelected = false;
        previousSelected = null;
        render.color = Color.white;
    }

    // mouse click

    private void OnMouseDown()
    {
        // game over or at the load screen prevent player from fore-playing
        if (render.sprite == null || MyBoardManager.instance.IsShifting)
        {
            return;
        }

        // if the tile is already selected and you click it again
        if (isSelected)
        {
            Deselect();
        }
        else
        {
            // nothing has been selecting then 
            if (previousSelected == null)
            {
                Select();
            }

            else

            {
                // if you swap the tile which was clicked to up,down,right or left then swap sprite
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                {
                    SwapSprite(previousSelected.render);
                    previousSelected.ClearAllMatches();
                    previousSelected.Deselect();
                    ClearAllMatches();

                }
                else
                // if the tile is already selected but you click somewhere else then deselect
                {
                    previousSelected.GetComponent<MyTile>().Deselect();
                    Select();
                }
            }
        }
    }

    // SWAP TILES 
    void SwapSprite(SpriteRenderer render2)
    {
        // if the tile is already selected then you click on the same tile then do nothing
        if (render.sprite == render2.sprite)
        {
            return;
        }
        // store the new sprite you have clicked to a temporary variables
        Sprite tempSprite = render2.sprite;
        // then swap 
        render2.sprite = render.sprite;
        // then add the new sprite to tempSprite
        render.sprite = tempSprite;

        SFXManager.instance.PlaySFX(Clip.Swap);
    }

    // GET ADJACENT TILES TO SWAP ONLY
    private GameObject GetAdjacentTiles(Vector2 castDir)
    {
        // cast rays to 4 directions up,down,right,left to detect adjacent tiles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentTilesDir.Length; i++)
        {
            adjacentTiles.Add(GetAdjacentTiles(adjacentTilesDir[i]));
        }
        return adjacentTiles;
    }

    // get the similar adjacent game objects to destroy
    private List<GameObject> FindMatch(Vector2 castDir)
    {
        // creat a list to add similar adjacent gameobject
        List<GameObject> matchedSprites = new List<GameObject>();
        // cast ray to detect
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        // if detect a collider and it has same sprite as the previousSelected game object
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
        {
            matchedSprites.Add(hit.collider.gameObject);
            // the collider was being hit keep casting the ray to other adjacent tiles
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        // add the matchedSprite to list GetAdjacent
        return matchedSprites;
    }

    // clear the similar adjacent game objects
    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchedSprites = new List<GameObject>();
        // add adjacent game objects to this new list, loop through GetAdjacent
        for (int i = 0; i < paths.Length; i++)
        {
            matchedSprites.AddRange(FindMatch(paths[i]));
        }

        // if matched sprites eual or greater than 2 then clear 
        if (matchedSprites.Count >= 2) // clear the sprites and add bool matchFound
        {
            for (int i = 0; i < matchedSprites.Count; i++)
            {
                matchedSprites[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }
    // clear all matches, including the previousSelected game object
    public void ClearAllMatches()
    {
        // if the game over or something then
        if (render.sprite == null)
        {
            return;
        }
        // clear all horizontal and vertical matches
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });

        // and also clear the previousSelected game object, clear sound
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;

            StopCoroutine(MyBoardManager.instance.FindNullTiles());
            StartCoroutine(MyBoardManager.instance.FindNullTiles());

            SFXManager.instance.PlaySFX(Clip.Clear);
        }
    }
}
