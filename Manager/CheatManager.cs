using System.Numerics;
using UnityEngine;

public class CheatManager : Singleton<CheatManager>
{
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            switch(Input.inputString)
            {
                case "s":
                case "S":
                    NextStage();
                    break;
                case "g":
                case "G":
                    BigInteger money = 100000000;
                    SaveManager.Instance.SetFieldData("gold", money, true);
                    break;
                case "d":
                case "D":
                    SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.diamond), 10000, true);
                    break;
                case "q":
                case "Q":
                    QuestCheat();
                    break;
                case "e":
                case "E":
                    BigInteger exp = 100000;
                    SaveManager.Instance.SetFieldData("userExp", exp, true);
                    break;
                case "l":
                case "L":
                    if (SaveManager.Instance.userData.eggLv < 10)
                        SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.eggLv), 1, true);
                    break;
                default:
                    break;
            }
        }
    }

    private void NextStage()
    {
        StageManager.Instance.killCount = StageManager.Instance.data.nextStageCount;
    }

    private void QuestCheat()
    {
        QuestManager.Instance.curProgress = QuestManager.Instance.data.goal;
    }
#endif
}