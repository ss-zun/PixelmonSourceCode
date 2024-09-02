using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasIntro : MonoBehaviour
{
    [SerializeField] private List<Transform> parents;

    IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync("DontDestroy", LoadSceneMode.Additive);
        UIManager.SetParents(parents);
        parents[0].gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        GameManager.Instance.OnInit();
        yield return new WaitUntil(() => GameManager.isInit);
        SceneManager.LoadSceneAsync("GameScene");
    }
}