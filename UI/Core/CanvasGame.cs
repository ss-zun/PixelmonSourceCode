using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasGame : MonoBehaviour
{
    [SerializeField] private List<Transform> parents;
    [SerializeField] private Button tabOverlay;
    [SerializeField] private GameObject AllFade;

    private IEnumerator Start()
    {
        UILoading.Hide();
        UIManager.SetParents(parents);
        UIManager.SetCanvas(transform);
        UIManager.SetTabOverlay(tabOverlay);
        yield return new WaitUntil(() => DataManager.Instance.isInit);
        yield return UIManager.Show<UIMiddleBar>();
        yield return new WaitUntil(() => AllFade.activeInHierarchy == false);

        if (SaveManager.Instance.userData.tutoIndex == 0)
        {
            yield return UIManager.Show<Tutorial>();
        }
    }
}
