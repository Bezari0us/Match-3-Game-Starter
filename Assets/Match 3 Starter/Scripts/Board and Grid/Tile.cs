/*
 * Copyright (c) 2017 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Tile previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	void Awake() {
		render = GetComponent<SpriteRenderer>();
    }

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Tile>();
		SFXManager.instance.PlaySFX(Clip.Select);
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

    private void OnMouseDown()
    {
        if(render.sprite==null || MyBoardManager.instance.IsShifting)
        {
            return;
        }
        // if already selected and player click somewhere else or click the previousSelected
        if(isSelected)
        {
            Deselect();
        }
        else
        {
            // if it is the first one 
            if (previousSelected == null)
            {
                Select();
            }
            else
            {
                // if the adjacent tiles are those one hit by raycast, get the previousSelected gameobject and swap the sprite
                if(GetAllAdjacentTiles().Contains(previousSelected.gameObject))

                {
                    // swap sprite
                    SwapSprite(previousSelected.render);
                    // deselect previous
                    previousSelected.Deselect();          
                }

                else
                // deselect the previous sprite and select the new ones
                {
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
                
            }
           
        }
    }

    // swap the sprite
    public void SwapSprite(SpriteRenderer render2)
    {
        if(render.sprite==render2.sprite)
        {
            return;
        }
        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        SFXManager.instance.PlaySFX(Clip.Swap);

    }

    // retrieve a single adjacent tile by sending a raycast in the target specified by castDir
    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if(hit.collider!=null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    // GetAdjacent method add the sprite which was being hit by the raycast from the previousSelected sprite, up, down, left, right
    // and add it to the list game object adjacentTiles
    // then GetAllAdjacentTiles will return the adjacentTiles
    private List<GameObject> GetAllAdjacentTiles()
    {
        List <GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;

    }


}