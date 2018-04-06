using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    // basics
    private Grid levelGrid;
    private Tilemap[] tileMaps;
    private GameObject debugLight; // Light for editing level, dimmed in game

    // player related
    private PlayerMovement playerScript;
    private GameObject player;

    // win conditions
    public List<Vector3Int> gravestoneLocations;
    // Bool to hold if player is on exit
    // Needed because:
    // From CheckWin() you don't know if the player is on an exit, so you call IsOnExit()
    // From IsOnExit() you don't know if this GameController asked for it, so you call CheckWin() just in case
    // Solution: hold data here
    public bool playerOnExit;
    public bool win = false;

    // undo feature
    public GameObject tileObjHolder;
    public Stack<LevelData> levelDataStack; // List of LevelDatas, so you can undo
    [SerializeField]
    public int statesSaved {
        get {
            return levelDataStack.Count;
        }
    }

    private void Awake() {
        levelDataStack = new Stack<LevelData>();
        tileObjHolder = GameObject.FindGameObjectWithTag("TileObjHolder"); // Parent holder of all souls
        Debug.Log(tileObjHolder);
    }

    void Start() {
        levelGrid = GameObject.FindGameObjectWithTag("LevelGrid").GetComponent<Grid>();
        tileMaps = levelGrid.gameObject.transform.GetComponentsInChildren<Tilemap>();

        debugLight = gameObject.transform.Find("Debug Light").gameObject;
        debugLight.GetComponent<Light>().intensity = 0.5f;

        player = GameObject.FindWithTag("Player");
        FindGravestones(); // Search for gravestones
    }

    // Update is called once per frame
    void Update() {
        if (playerScript == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerScript = player.GetComponent<PlayerMovement>();
        }
        if (!win) CheckWin();

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            SaveLevelData();
            Debug.Log("Saved state");
        } else if (levelDataStack.Count > 0 && Input.GetKeyDown(KeyCode.LeftArrow)) {
            LoadLevelData(levelDataStack.Pop());
            Debug.Log("Loaded state");
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

    /// <summary> Load a LevelData to the scene.
    /// The data in <paramref name="levelData"/> will be loaded to the scene
    /// </summary>
    public void LoadLevelData(LevelData levelData) {

        //Reset tiles from given level data
        for (int i = 0; i < 2; i++) {
            tileMaps[i].SetTilesBlock(tileMaps[0].cellBounds, levelData.maps[i]);
        }

        // restore from levelData
        for (int i = 0; i < tileObjHolder.transform.childCount; i++) {
            tileObjHolder.transform.GetChild(i).GetComponent<TileObject>().SetState(levelData.states[i]);
        }

    }

    public void SaveLevelData() {
        // Save data after every player move
        TileObject.TileObjectState[] tileStates = new TileObject.TileObjectState[tileObjHolder.transform.childCount];
        for (int i = 0; i < tileStates.Length; i++) {
            tileStates[i] = tileObjHolder.transform.GetChild(i).GetComponent<TileObject>().GetState();
        }
        LevelData snapshot = new LevelData(tileMaps, tileStates);
        levelDataStack.Push(snapshot); // Add the data from this move into the array
    }
}
