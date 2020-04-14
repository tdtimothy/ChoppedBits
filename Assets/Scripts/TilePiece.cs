using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TilePiece : MonoBehaviour
{
    public int type;
    public TileCoord index;

    [HideInInspector]
    public Vector2 pos;
    [HideInInspector]
    public RectTransform rect;

    //bool updating;
    Image img;

    public void Initialize(int type, TileCoord pos, Sprite tile)
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        this.type = type;
        SetIndex(pos);
        img.sprite = tile;
    }

    public void SetIndex(TileCoord pos)
    {
        index = pos;
        ResetPosition();
        UpdateName();
    }

    public void ResetPosition()
    {
        pos = new Vector2(60 + (120 * index.x), -60 - (120 * index.y));
    }

    public void MovePositionTo(Vector2 move)
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, move, Time.deltaTime * 16f);
    }

    public bool UpdatePiece()
    {
        //Debug.Log(Vector3.Distance(rect.anchoredPosition, pos));
        if(Vector3.Distance(rect.anchoredPosition, pos) > 1)
        {
            MovePositionTo(pos);
            //updating = true;
            return true;
        }
        else
        {
            rect.anchoredPosition = pos;
            //updating = false;
            return false;
        }
    }

    void UpdateName()
    {
        transform.name = "Node [" + index.x + ", " + index.y + "]";
    }
}
