using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskOverlay : UIBase
{
    private RectTransform rectTransform;
    [SerializeField] private CutoutMaskUI Mask;

    protected override void Awake()
    {
        base.Awake();
        rectTransform = this.GetComponent<RectTransform>();
    }

    public override void Opened(object[] param) 
    { 
        base.Opened(param);
        Transform targetTransform = param[0] as Transform;
        gameObject.transform.SetParent(targetTransform.parent);
        gameObject.transform.SetSiblingIndex(targetTransform.GetSiblingIndex() + 1);

        SetCustomSize((float)param[2], (float)param[3]);
        SetTargetPos((Vector3)param[1]);
    }

    public void SetCustomSize(float width, float height)
    {
        if (rectTransform != null)
            rectTransform.sizeDelta = new Vector2(width, height);
    }

    public void SetTargetPos(Vector3 pos)
    {
        gameObject.transform.position = pos;
        Mask.SetToCanvasSize();
    }
}
