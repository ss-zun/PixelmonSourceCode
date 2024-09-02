using UnityEngine;
using UnityEditor;


public class AutoBoxCollider : MonoBehaviour
{
    [MenuItem("Tools/GameObject/AutoBoxCollider2D")]
    public static void AdjustBounds()
    {
        GameObject selectedGameObject = Selection.activeGameObject;
        if (selectedGameObject != null)
        {
            SpriteRenderer sr = selectedGameObject.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                BoxCollider2D box = selectedGameObject.GetComponentInChildren<BoxCollider2D>();
                if (box == null) box = selectedGameObject.AddComponent<BoxCollider2D>();
                box.offset = sr.bounds.center - selectedGameObject.transform.position;
                box.size = sr.bounds.size;
            }
            else
            {
                //Debug.LogError("No SpriteRenderer found in the selected GameObject.");
            }
        }
    }
}