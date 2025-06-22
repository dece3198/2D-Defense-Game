using UnityEngine;

[CreateAssetMenu(fileName = "New Rank", menuName = "New Rank/Rank")]
public class Rank : ScriptableObject
{
    public string rankName;
    public Sprite rankImage;
    public Sprite[] pieces;
    public Rank nextRank;
    public RankNumber rankNumber;
}
