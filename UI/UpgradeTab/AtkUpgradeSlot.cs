using UnityEngine;

public class AtkUpgradeSlot : UpgradeSlot
{
    protected override void SetValueTxt()
    {
        slotValueTxt.text = CurValue.ToString();

        if (CurLv >= maxLv)
        {
            nextValueTxt.gameObject.SetActive(false);
        }
        else
        {
            nextValueTxt.text = nextValue.ToString();
        }
    }
 
    protected override float ValuePerLv(int reachLv)
    {
        return 9 + reachLv;
    }
}