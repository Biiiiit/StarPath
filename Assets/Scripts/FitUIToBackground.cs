using UnityEngine;

public class FitUIToBackground : MonoBehaviour
{
    public SpriteRenderer background;
    public RectTransform bottomBar;
    public Canvas canvas;
    public Camera cam;

    void LateUpdate()
    {
        if (!background || !bottomBar || !canvas || !cam) return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Bounds bounds = background.bounds;

        // World → Screen
        Vector3 screenBL = cam.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, 0));
        Vector3 screenBR = cam.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0));

        // Screen → Canvas local
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenBL, cam, out Vector2 localBL);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenBR, cam, out Vector2 localBR);

        float width = localBR.x - localBL.x;

        // Height = distance from bottom of canvas to background bottom
        float canvasBottom = canvasRect.rect.yMin;
        float height = localBL.y - canvasBottom;

        // ✅ Apply size
        bottomBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        bottomBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        // ✅ Keep anchored to bottom, only adjust X
        bottomBar.anchoredPosition = new Vector2(
            (localBL.x + localBR.x) / 2f,
            0
        );
    }
}