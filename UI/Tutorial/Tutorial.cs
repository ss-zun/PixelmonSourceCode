
public class Tutorial : UIBase
{
    public void OnTutorialOff()
    {
        SaveManager.Instance.SetFieldData(nameof(SaveManager.Instance.userData.tutoIndex), 1); //여신 종료.
        GuideManager.Instance.GuideNumTrigger(GuideManager.Instance.guideNum); //0번 이벤트: 알까기.
        GuideManager.Instance.GuideArrow.SetActive(true); //화살표 ON.
        UIManager.Hide<Tutorial>();
    }
}