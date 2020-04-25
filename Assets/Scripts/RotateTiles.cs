using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTiles : MonoBehaviour
{
    GameController game;
    int x;
    int y;
    int gridWidth = 120;
    int gridHeight = 120;
    public RectTransform rect;
    public RectTransform canvas;

    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseControl.GameIsPaused || !game.gameActive)
            return;
        UpdatePosition();
        if (Input.GetMouseButtonDown(0)) {
            if(!game.gameStart)
                game.gameStart = true;
            game.RotatePieces(new TileCoord((x / 120) - 1, (y / -120) - 1), false);
        }

        if (Input.GetMouseButtonDown(1)) {
            if(!game.gameStart)
                game.gameStart = true;
            game.RotatePieces(new TileCoord((x / 120) - 1, (y / -120) - 1), true);
        }
    }

    void UpdatePosition() {
        Vector2 RelativePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out RelativePos);
        x = Mathf.RoundToInt(((RelativePos.x + 480) / gridWidth) ) * 120;
        y = Mathf.RoundToInt(((RelativePos.y - 420) / gridHeight) ) * 120;
        if(x < 120)
            x = 120;
        else if(x > 840)
            x = 840;
        if(y > -120)
            y = -120;
        else if(y < -720)
            y = -720;
        rect.anchoredPosition = new Vector2(x, y);
    }
}
