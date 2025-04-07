using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseSelect : MonoBehaviour
{
    public static MouseSelect instance;
    private new SpriteRenderer renderer;
    [SerializeField] private Tilemap groundTileMap;
    public bool isSelect = true;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        instance = this;
    }

    private void Update()
    {
        Vector2 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPos = new Vector2(Mathf.Round(mPos.x), Mathf.Round(mPos.y));
        transform.position = mPos;

        if(Mathf.Abs(transform.localPosition.x) > 9 || Mathf.Abs(transform.localPosition.y) > 4)
        {
            renderer.color = Color.red;
        }
        else
        {
            Vector3Int currentCell = groundTileMap.WorldToCell(mPos);
            if (groundTileMap.GetTile(currentCell) != null)
            {
                renderer.color = Color.green;
            }
            else
            {
                renderer.color = Color.red;
            }

        }
    }

}
