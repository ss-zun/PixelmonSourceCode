using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutMaskUI : Image
{
    public void SetToCanvasSize()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
            RectTransform rectTransform = this.GetComponent<RectTransform>();

            rectTransform.sizeDelta = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.position = canvasRectTransform.position;
        }
    }

    public override Material materialForRendering{
        get
        {
            Material material = new Material(base.materialForRendering);
            material.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return material;
        }
    }
}
