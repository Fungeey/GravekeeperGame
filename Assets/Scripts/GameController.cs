using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    private Grid levelGrid;
    private Tilemap[] tileMap;
    public List<Vector3Int> gravestoneLocations;

    private PlayerMovement playerScript;
    public bool win = false;
    private GameObject debugLight; // Light for editing level, turns of in game


    public bool playerOnExit; // Bool to hold if player is on exit
                              // Needed because:
                              // From CheckWin() you don't know if the player is on an exit, so you call IsOnExit()
                              // From IsOnExit() you don't know if this GameController asked for it, so you call CheckWin() just in case
                              // Solution: hold data here


    // Use this for initialization
    void Start() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tileMap = levelGrid.gameObject.transform.GetComponentsInChildren<Tilemap>();

        debugLight = gameObject.transform.Find("Debug Light").gameObject;
        debugLight.GetComponent<Light>().intensity = 0.5f;

        FindGravestones(); // Search for gravestones
    }

    // Update is called once per frame
    void Update() {
        if (playerScript == null) {
            playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        if (!win) CheckWin();
    }

    void FindGravestones() {
        for (int i = tileMap[0].cellBounds.xMin; i < tileMap[0].cellBounds.xMax; i++) {
            for (int j = tileMap[0].cellBounds.yMin; j < tileMap[0].cellBounds.yMax; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = tileMap[0].GetTile(pos);
                if (tile != null) {
                    if (tile.name == "Gravestone") {
                        gravestoneLocations.Add(pos);
                    }
                }
            }
        }
    }

    public bool CheckWin() {
        foreach (Vector3Int pos in gravestoneLocations) {
            // Check if there is a gravestone at that position on ground (not solid)
            if (tileMap[0].GetTile(pos) != null) {
                // If there is a tile, return false
                return false;
            }
        }

        if (!playerOnExit)
            return false;
        Debug.Log("WWWWWIIIIINNNNN");
        win = true;
        return true;
        // Congrats, you won!
    }
}
