using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MainMenuSlotsManager : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform viewport;
    public Image tilePrefab;
    public List<Sprite> spritePool = new List<Sprite>();

    [Header("Grid")]
    [Min(1)] public int columns = 4;
    [Min(1)] public int rows = 4;
    public float seamOverlap = 1f;

    [Header("Motion")]
    public Vector2 columnSpeedRange = new Vector2(80f, 240f);

    private class Column
    {
        public List<Image> tiles = new List<Image>();
        public float speed;
        public float scrollY;
    }

    private readonly List<Column> _columns = new List<Column>();
    private RectTransform _rt;
    private System.Random _rng;

    private float _cellW, _cellH, _halfW, _halfH;
    private int _tilesPerColumn;

    private void Awake()
    {
        if (!viewport) viewport = transform as RectTransform;
        _rt = viewport;
        _rng = new System.Random(System.DateTime.Now.Millisecond + System.DateTime.Now.Second * 1000);
    }

    private void OnEnable()
    {
        Rebuild();
    }

    private void OnRectTransformDimensionsChange()
    {
        if (isActiveAndEnabled) Rebuild();
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        float topY = _halfH - _cellH * 0.5f;
        float leftX = -_halfW;

        for (int c = 0; c < _columns.Count; c++)
        {
            Column col = _columns[c];
            col.scrollY += col.speed * dt;

            if (col.scrollY >= _cellH)
            {
                col.scrollY -= _cellH;

                // Move TOP tile to the BOTTOM
                Image top = col.tiles[0];
                col.tiles.RemoveAt(0);
                col.tiles.Add(top);

                // Assign new sprite (appears at the bottom)
                Sprite newSprite = GetRandomSprite();
                top.sprite = newSprite;
                top.enabled = newSprite != null;
                top.color = newSprite ? Color.white : Color.clear;
            }

            float centerX = leftX + c * _cellW + _cellW * 0.5f;

            for (int i = 0; i < col.tiles.Count; i++)
            {
                float y = topY - (i * _cellH - col.scrollY);
                RectTransform tr = col.tiles[i].rectTransform;
                tr.anchoredPosition = new Vector2(centerX, y);
            }
        }
    }

    public void Rebuild()
    {
        if (!_rt || tilePrefab == null) return;

        // Cleanup
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        _columns.Clear();

        Vector2 size = _rt.rect.size;
        _halfW = size.x * 0.5f;
        _halfH = size.y * 0.5f;

        _cellW = size.x / Mathf.Max(1, columns);
        _cellH = size.y / Mathf.Max(1, rows);

        float paddedW = _cellW + seamOverlap;
        float paddedH = _cellH + seamOverlap;

        _tilesPerColumn = rows + 1;

        for (int c = 0; c < columns; c++)
        {
            Column col = new Column();
            col.speed = RandomRangeFloat(columnSpeedRange.x, columnSpeedRange.y);
            col.scrollY = RandomRangeFloat(0f, _cellH);

            float centerX = -_halfW + c * _cellW + _cellW * 0.5f;
            float topY = _halfH - _cellH * 0.5f;

            for (int i = 0; i < _tilesPerColumn; i++)
            {
                Image img = Instantiate(tilePrefab, transform);
                RectTransform tr = img.rectTransform;
                tr.anchorMin = tr.anchorMax = tr.pivot = new Vector2(0.5f, 0.5f);
                float spriteScale = 0.80f; //adjusst sprite size
                tr.sizeDelta = new Vector2(paddedW * spriteScale, paddedH * spriteScale);


                float y = topY - (i * _cellH - col.scrollY);
                tr.anchoredPosition = new Vector2(centerX, y);

                Sprite s = GetRandomSprite();
                img.sprite = s;
                img.enabled = s != null;
                img.color = s ? Color.white : Color.clear;

                col.tiles.Add(img);
            }

            _columns.Add(col);
        }
    }

    private Sprite GetRandomSprite()
    {
        if (spritePool == null || spritePool.Count == 0) return null;
        return spritePool[_rng.Next(spritePool.Count)];
    }

    private float RandomRangeFloat(float min, float max)
    {
        if (min > max) { var t = min; min = max; max = t; }
        return (float)(_rng.NextDouble() * (max - min) + min);
    }
}