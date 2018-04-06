using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    private Grid levelGrid;
    private Tilemap[] tileMaps;
    public List<Vector3Int> gravestoneLocations;
    public List<GameObject> soulObjects; // List of soul objects so we can keep track of them for Undo

    public List<LevelData> levelUndo = new List<LevelData>(); // List of LevelDatas, so you can undo
    
    private PlayerMovement playerScript;
    public bool win = false;
    private GameObject debugLight; // Light for editing level, dimmed in game
    private GameObject soulHolder;
    private GameObject player;

    public int undoIndex = 0; // The index that we are in the undo array
    // Increase every time you save a new levelData
    // Decrease every time you go back one


    public bool playerOnExit; // Bool to hold if player is on exit
                              // Needed because:
                              // From CheckWin() you don't know if the player is on an exit, so you call IsOnExit()
                              // From IsOnExit() you don't know if this GameController asked for it, so you call CheckWin() just in case
                              // Solution: hold data here


    // Use this for initialization
    void Start() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tileMaps = levelGrid.gameObject.transform.GetComponentsInChildren<Tilemap>();

        debugLight = gameObject.transform.Find("Debug Light").gameObject;
        debugLight.GetComponent<Light>().intensity = 0.5f;

        soulHolder = transform.Find("SoulHolder").gameObject; // Parent holder of all souls
        player = GameObject.FindWithTag("Player");

        FindGravestones(); // Search for gravestones
        FindSouls(); // Search for souls (cant join b/c gravestones are on ground, souls are on main)

        saveLevelData(tileMaps[0], tileMaps[1], soulObjects, player);
    }

    // Update is called once per frame
    void Update() {
        if (playerScript == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerMovement>();
        }
        if (!win) CheckWin();

        // Check for undo / redo
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            resetLevelData(-1);
        }else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            resetLevelData(1);
        }
    }

    void FindGravestones() { //Find location of gravestones
        for (int i = tileMaps[0].cellBounds.xMin; i < tileMaps[0].cellBounds.xMax; i++) {
            for (int j = tileMaps[0].cellBounds.yMin; j < tileMaps[0].cellBounds.yMax; j++) {
                Vector3Int pos = new Vector3Int(i, j, 0);
                TileBase tile = tileMaps[0].GetTile(pos);
                if (tile != null) {
                    if (tile.name == "Gravestone") {
                        gravestoneLocations.Add(pos);
                    }
                }
            }
        }
    }

    void FindSouls() { // Get every soul [Object], so we can refer to their locations whenever we want
        for(int i = 0; i < soulHolder.transform.childCount; i++) {
            soulObjects.Add(soulHolder.transform.GetChild(i).gameObject);
            // Add every soul parented to SoulHolder
        }
    }

    public bool CheckWin() {
        foreach (Vector3Int pos in gravestoneLocations) {
            // Check if there is a gravestone at that position on ground (not solid)
            if (tileMaps[0].GetTile(pos) != null) {
                // If there is a tile, return false
                return false;
            }
        }

        if (!playerOnExit)
            return false;
        Debug.Log("WWWWWIIIIINNNNN");
        win = true;
        return true; // Congrats, you won!
    }

    /// <summary> Go one step in the direction specified 
    /// The <paramref name="LeftorRight"/> -1 for left, 1 for right
    /// </summary>
    public void resetLevelData(int LeftorRight) {
        int i = undoIndex + LeftorRight; // Index
        if (i < 0 && i >= levelUndo.Capacity) {
            return; // Don't do anything if the index is invalid
        }
        LevelData levelData = levelData = levelUndo[i];

        //Reset tiles from memory
        tileMaps[0] = levelData.main;
        tileMaps[1] = levelData.ground;

        // For souls: destroy all souls in scene, make new ones from memory
        for(int a = 0; i < soulHolder.transform.childCount; i++) {
            Destroy(soulHolder.transform.GetChild(a)); // Destroy each soul in soulholder
        }
        foreach(GameObject soul in levelData.souls) {
            // Now add them back in from memory
            Instantiate(soul); //Does this work???
        }

        //For player
        Destroy(player);
        GameObject playr = Instantiate(levelData.player); // Does this work??
        playr.GetComponentInChildren<Light>().enabled = true;
        playr.GetComponent<PlayerMovement>().enabled = true;


    }

    public void saveLevelData(Tilemap ground, Tilemap main, List<GameObject> soulObj, GameObject Player) {
        // Save data after every player move
        LevelData levelData = new LevelData(tileMaps[0], tileMaps[1], soulObjects, Player);
        levelUndo.Add(levelData); // Add the data from this move into the array
        undoIndex++;
    }
}
