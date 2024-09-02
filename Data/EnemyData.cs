using System;
using System.Numerics;

[Serializable]
public class EnemyData : IData
{
    #region json keys
    public string rcode;
    public string name;
    public string spawnWorldId;
    public bool isBoss;
    public float spd;
    private int atk;
    public BigInteger Atk { get { return atk; } set { atk = (int)value; } }
    public float atkRange;
    private int hp;
    public BigInteger Hp { get { return hp; } set { hp = (int)value; } }
    private int def;
    public BigInteger Def { get { return def; } set { def = (int)value; } }
    public string rewardTypes;
    public string rewardRates;
    public string rewardValues;
    #endregion

    #region converted arrays
    public string[] rewardType => rewardTypes.Split(" ");
    public float[] rewardRate => Array.ConvertAll(rewardRates.Split(" "), float.Parse);
    public int[] rewardValue => Array.ConvertAll(rewardValues.Split(" "), int.Parse);
    #endregion

    string IData.Rcode => rcode;  // 명시적 인터페이스 구현
}