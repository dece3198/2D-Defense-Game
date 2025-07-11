using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseSelect : MonoBehaviour
{
    private new SpriteRenderer renderer;
    [SerializeField] private Tilemap girdTileMap;
    public bool isSelect = true;

    private  void Awake()
    { 
        renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPos = new Vector2(Mathf.Round(mPos.x), Mathf.Round(mPos.y));
        transform.position = mPos;

        if(Mathf.Abs(transform.localPosition.x) > 9 || Mathf.Abs(transform.localPosition.y) > 4)
        {
            GameManager.instance.isSelect = false;
            renderer.color = Color.red;
            return;
        }

        Vector3Int currentCell = girdTileMap.WorldToCell(mPos);

        if (girdTileMap.GetTile(currentCell) == null)
        {
            GameManager.instance.isSelect = false;
            renderer.color = Color.red;
            return;
        }

        if(UnitSpawner.instance.usedTiles.Contains(currentCell))
        {
            GameManager.instance.isSelect = true;
            renderer.color = Color.green;
        }
        else
        {
            GameManager.instance.isSelect = true;
            renderer.color = Color.green;
        }
    }

}
