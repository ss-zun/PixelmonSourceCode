using TMPro;
using UnityEngine;

public enum DungeonType
{
    Gold = 0, 
    Seed,
    Skill
}

public class DungeonSlot : MonoBehaviour
{
    public DungeonTab dungeonTab;
    public DungeonType type;

    public int rwdBNum;
    public int rwdD1;
    public int rwdD2;


    #region UI
    public TextMeshProUGUI dungeonName;
    public TextMeshProUGUI keyTxt;
    #endregion

    public void enterDungeonBtn()
    {
        dungeonTab.dgPopup.SetActive(true);
        dungeonTab.dgPopup.curSlot = this;
        dungeonTab.dgPopup.SetDgPopup(dungeonName.text);
    }

    public bool UseKey()
    {
        int keyIndex = (int)type;
        if (dungeonTab.keys[keyIndex] > 0)
        {
            SaveManager.Instance.SetFieldData($"key{keyIndex}", -1, true);
            dungeonTab.keys[keyIndex]--;
            KeyTxt();
            return true;
        }
        else
        {
            AudioManager.Instance.PlayClip(DataManager.Instance.GetData<SoundData>("SOU20008").clip);
            UIManager.Instance.ShowWarn("열쇠가 부족합니다!!");
            return false;
        }
    }

    public void KeyTxt()
    {
        keyTxt.text = dungeonTab.GetKeyString((int)type);
    }
}
