using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseBg
{
    public PxmRank rank;
    public Sprite img;
    public List<Dictionary<PxmRank, Sprite>> bgs;
}

public static class PxmRankImgUtil
{
    public static Sprite GetRankImage(PxmRank rank, List<BaseBg> bgs)
    {
        foreach (BaseBg bg in bgs)
        {
            if (bg.rank == rank)
            {
                return bg.img;
            }
        }
        return null; // 해당 rank가 없을 경우 null 반환
    }
}
