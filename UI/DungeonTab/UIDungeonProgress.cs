
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonProgress : UIBase
{
    [SerializeField] private GameObject popup;
    [SerializeField] private Button btn;
    [SerializeField] private PixelmonLayout dgLayout;
    private PixelmonLayout tempLayout;

    private void OnEnable()
    {
        if (dgLayout.gameObject.activeInHierarchy)
        {
            tempLayout = SkillManager.Instance.layout;
            SkillManager.Instance.layout = dgLayout;
        }
    }

    private void OnDisable()
    {
        if (SkillManager.Instance.layout == dgLayout)
        {
            SkillManager.Instance.layout = tempLayout;
        }
    }
    public void StopTime()
    {
        btn.enabled = false;
        Time.timeScale = 0;
    }

    public void GoTime()
    {
        btn.enabled = true;
        Time.timeScale = 1;
    }

    public void EscapeBtn()
    {
        Time.timeScale = 1;
        btn.enabled = true;
        popup.SetActive(false);
        StageManager.Instance.isDungeon = false;
        SaveManager.Instance.SetFieldData("key" + StageManager.Instance.dgIndex, 1, true);
        gameObject.SetActive(false);
    }
}