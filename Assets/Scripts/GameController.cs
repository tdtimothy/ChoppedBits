using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public BoardLayout boardLayout;
    public float gameTimer = 120.0f;
    public static bool gameActive = true;
    public bool gameStart = false;
    public AudioClip matchSound;
    private AudioSource source {get {return GetComponent<AudioSource>();}}

    [Header("UI Elements")]
    public Sprite[] tiles;
    public RectTransform gameBoard;
    public RectTransform removedBoard;
    public Text Score;
    public Text Combo;
    public Text Multiplier;
    public Text bestScore;
    public Text bestCombo;
    public Text Timer;
    public Slider multiplierTimer;
    public GameObject gameOverMenu;

    [Header("Prefabs")]
    public GameObject tilePiece;
    public GameObject removedPiece;

    int width = 8;
    int height = 7;
    int[] fills;
    Tile[,] board;
    [HideInInspector]
    public static double maxScore;
    [HideInInspector]
    public static int maxCombo;

    List<TilePiece> update;
    List<TilePiece> dead;
    List<RemovedPiece> removed;

    System.Random random;

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if(gameTimer <= 0.0f)
            gameOver();
        if(PauseControl.GameIsPaused || !gameActive)
            return;
        if(StartControl.Endless){
            Timer.text = '\u221E'.ToString();
        }
        else if(gameStart){
            gameTimer -= Time.deltaTime;
            Timer.text = ((int)gameTimer).ToString();
        }
        multiplierTimer.value -= Time.deltaTime;
        if(multiplierTimer.value <= 0.0f){
            Combo.text  = "0";
            Multiplier.text = "1x";
        }
        List<TilePiece> finishedUpdating = new List<TilePiece>();
        for(int i = 0; i < update.Count; i++) {
            TilePiece piece = update[i];
            if (piece == null)
                continue;
            if (!piece.UpdatePiece()) {
                finishedUpdating.Add(piece);
            }
        }
        for (int i = 0; i < finishedUpdating.Count; i++) {
            TilePiece piece = finishedUpdating[i];
            int x = (int)piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);

            List<TileCoord> matches = matchFinder(piece.index, true);
            Debug.Log(matches.Count);
            if (matches.Count > 0) {
                source.PlayOneShot(matchSound, .25f);
                foreach (TileCoord pos in matches) {
                    RemovePiece(pos);
                    Tile tile = getTile(pos);
                    TilePiece tilePiece = tile.getPiece();
                    if (tilePiece != null) {
                        tilePiece.gameObject.SetActive(false);
                        dead.Add(tilePiece);
                    }
                    tile.SetPiece(null);
                }
                multiplierTimer.value += 1.5f + (matches.Count - 3) * .5f;
                int comboVal = int.Parse(Combo.text);
                comboVal += 1 + (matches.Count - 3) / 2;
                if(comboVal > maxCombo)
                    maxCombo = comboVal;
                Combo.text = comboVal.ToString();
                int scoreMult = 1 + comboVal  / 10;
                if(scoreMult > 5)
                    scoreMult = 5;
                Multiplier.text = scoreMult + "x";
                double scoreVal = double.Parse(Score.text);
                scoreVal += (matches.Count - 2) * 100 *  scoreMult;
                if(scoreVal > maxScore)
                    maxScore = scoreVal;
                Score.text = string.Format("{0:000000000}", scoreVal);
            }
            ApplyGravityToBoard();
            update.Remove(piece);
        }
    }

    void gameOver(){
        gameActive = false;
        bestCombo.text = maxCombo.ToString();
        bestScore.text = string.Format("{0:000000000}", maxScore);
        gameOverMenu.SetActive(true);
    }

    public void ApplyGravityToBoard() {
        for (int x = 0; x < width; x++) {
            for (int y = (height - 1); y >= 0; y--) {
                TileCoord pos = new TileCoord(x, y);
                Tile tile = getTile(pos);
                int type = getTileType(pos);
                if (type != 0) 
                    continue;
                for (int ny = (y - 1); ny >= -1; ny--) {
                    TileCoord next = new TileCoord(x, ny);
                    int nextType = getTileType(next);
                    if (nextType == 0)
                        continue;
                    if (nextType != -1) {
                        Tile gotten = getTile(next);
                        TilePiece piece = gotten.getPiece();
                        tile.SetPiece(piece);
                        update.Add(piece);
                        gotten.SetPiece(null);
                    }
                    else {
                        int newType = genTile();
                        TilePiece piece;
                        TileCoord fallPnt = new TileCoord(x, (-1 - fills[x]));
                        if(dead.Count > 0) {
                            TilePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            piece = revived;

                            dead.RemoveAt(0);
                        }
                        else {
                            GameObject obj = Instantiate(tilePiece, gameBoard);
                            TilePiece n = obj.GetComponent<TilePiece>();
                            piece = n;
                        }

                        piece.Initialize(newType, pos, tiles[newType - 1]);
                        piece.rect.anchoredPosition = getPositionFromCoord(fallPnt);
                        Tile hole = getTile(pos);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    break;
                }
            }
        }
    }

    void StartGame() {
        fills = new int[width];
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<TilePiece>();
        dead = new List<TilePiece>();
        removed = new List<RemovedPiece>();

        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    void InitializeBoard() {
        board = new Tile[width, height];
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                board[x, y] = new Tile((boardLayout.rows[y].row[x]) ? - 1 : genTile(), new TileCoord(x, y));
            }
        }
    }

    void VerifyBoard() {
        List<int> remove;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                TileCoord pos = new TileCoord(x, y);
                int type = getTileType(pos);
                if (type <= 0) 
                    continue;
                remove = new List<int>();
                while (matchFinder(pos, true).Count > 0) {
                    type = getTileType(pos);
                    if (!remove.Contains(type))
                        remove.Add(type);
                    setTileType(pos, newType(ref remove));
                }
            }
        }
    }

    void InstantiateBoard() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Tile tile = getTile(new TileCoord(x, y));
                int type = tile.type;
                if (type <= 0) 
                    continue;
                GameObject gameTile = Instantiate(tilePiece, gameBoard);
                TilePiece piece = gameTile.GetComponent<TilePiece>();
                RectTransform rect = gameTile.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(60 + (120 * x), -60 - (120 * (y - 7)));
                piece.Initialize(type, new TileCoord(x, y), tiles[type - 1]);
                tile.SetPiece(piece);
                ResetPiece(piece);
            }
        }
    }
     
    public void ResetPiece(TilePiece piece) {
        piece.ResetPosition();
        update.Add(piece);
    }

    public void RotatePieces(TileCoord topLeft, bool direction) {
        Tile one = getTile(topLeft);
        TilePiece pieceOne = one.getPiece();
        Tile two = getTile(TileCoord.add(topLeft, TileCoord.right));
        TilePiece pieceTwo = two.getPiece();
        Tile three = getTile(TileCoord.add(topLeft, TileCoord.up));
        TilePiece pieceThree = three.getPiece();
        Tile four = getTile(TileCoord.add(topLeft, TileCoord.add(TileCoord.right, TileCoord.up)));
        TilePiece pieceFour = four.getPiece();
        Debug.Log("P00: " + getTileType(one.pos) + " P10: " + getTileType(two.pos) + " P01: " + getTileType(three.pos) + " P11: " + getTileType(four.pos));
        if(direction) {
            one.SetPiece(pieceThree);
            two.SetPiece(pieceOne);
            three.SetPiece(pieceFour);
            four.SetPiece(pieceTwo);
        }
        else {
            one.SetPiece(pieceTwo);
            two.SetPiece(pieceFour);
            three.SetPiece(pieceOne);
            four.SetPiece(pieceThree);
        }
        Debug.Log("P00: " + getTileType(one.pos) + " P10: " + getTileType(two.pos) + " P01: " + getTileType(three.pos) + " P11: " + getTileType(four.pos));
        update.Add(pieceOne);
        update.Add(pieceTwo);
        update.Add(pieceThree);
        update.Add(pieceFour);
    }

    void RemovePiece(TileCoord pos) {
        List<RemovedPiece> available = new List<RemovedPiece>();
        for (int i = 0; i < removed.Count; i++)
            if (!removed[i].falling) 
                available.Add(removed[i]);
        RemovedPiece set = null;
        if (available.Count > 0)
            set = available[0];
        else {
            GameObject remove = GameObject.Instantiate(removedPiece, removedBoard);
            RemovedPiece kPiece = remove.GetComponent<RemovedPiece>();
            set = kPiece;
            removed.Add(kPiece);
        }
        int tile = getTileType(pos) - 1;
        if (set != null && tile >= 0 && tile < tiles.Length)
            set.Initialize(tiles[tile], getPositionFromCoord(pos));
    }

    List<TileCoord> matchFinder(TileCoord pos, bool initial) {
        List<TileCoord> matches = new List<TileCoord>();
        int type = getTileType(pos);
        TileCoord[] directions = {
            TileCoord.up,
            TileCoord.right,
            TileCoord.down,
            TileCoord.left
        };
        
        foreach(TileCoord dir in directions)
        {
            List<TileCoord> line = new List<TileCoord>();

            int match = 0;
            for(int i = 1; i < 3; i++)
            {
                TileCoord check = TileCoord.add(pos, TileCoord.mult(dir, i));
                int typero = getTileType(check);
                if(getTileType(check) == type)
                {
                    line.Add(check);
                    match++;
                }
            }

            if (match >= 2) 
                AddMatches(ref matches, line);
        }

        for(int i = 0; i < 2; i++) {
            List<TileCoord> line = new List<TileCoord>();
            int match = 0;
            TileCoord[] check = { TileCoord.add(pos, directions[i]), TileCoord.add(pos, directions[i + 2]) };
            foreach (TileCoord next in check) {
                if (getTileType(next) == type)
                {
                    line.Add(next);
                    match++;
                }
            }
            if (match > 1)
                AddMatches(ref matches, line);
        }

        if(initial) {
            for (int i = 0; i < matches.Count; i++)
                AddMatches(ref matches, matchFinder(matches[i], false));
        }
        return matches;
    }

    void AddMatches(ref List<TileCoord> points, List<TileCoord> add) {
        foreach(TileCoord p in add) {
            bool doAdd = true;
            for(int i = 0; i < points.Count; i++) {
                if(points[i].Equals(p)) {
                    doAdd = false;
                    break;
                }
            }
            if (doAdd) 
                points.Add(p);
        }
    }

    int genTile() {
        int type = 1;
        type = (random.Next(0, 100) / (100 / tiles.Length));
        if(type == 0)
            type = 1;
        return type;
    }

    int getTileType(TileCoord pos) {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) return -1;
        return board[pos.x, pos.y].type;
    }

    void setTileType(TileCoord pos, int type) {
        board[pos.x, pos.y].type = type;
    }

    Tile getTile(TileCoord pos) {
        return board[pos.x, pos.y];
    }

    int newType(ref List<int> remove) {
        List<int> available = new List<int>();
        for (int i = 0; i < tiles.Length; i++)
            available.Add(i + 1);
        foreach (int i in remove)
            available.Remove(i);
        if (available.Count <= 0) 
            return 0;
        return available[random.Next(0, available.Count)];
    }

    string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdeghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        return seed;
    }

    public Vector2 getPositionFromCoord(TileCoord pos)
    {
        return new Vector2(60 + (120 * pos.x), -60 - (120 * pos.y));
    }
}

[System.Serializable]
public class Tile
{
    public int type;
    public TileCoord pos;
    TilePiece piece;

    public Tile(int type, TileCoord pos)
    {
        this.type = type;
        this.pos = pos;
    }

    public void SetPiece(TilePiece tile)
    {
        piece = tile;
        type = (tile == null) ? 0 : tile.type;
        if (tile == null) 
            return;
        piece.SetIndex(pos);
    }

    public TilePiece getPiece()
    {
        return piece;
    }
}