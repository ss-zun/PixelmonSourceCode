using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitPanel : UIBase
{
    public override void HideDirect()
    {
        UIManager.Hide<GameExitPanel>();
    }

    public void OnClickExitBtn()
    {
#if UNITY_EDITOR
        // 에디터 모드에서 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 빌드된 애플리케이션에서는 애플리케이션 종료
    Application.Quit();
#endif
    }
}
