using UnityEngine;

public static class RectTransformExtensions
{
  public static void Show(this RectTransform rect)
  {
    rect.gameObject.SetActive(true);

    if (!rect.gameObject.activeSelf)
      rect.gameObject.SetActive(true);
  }

  public static void Hide(this RectTransform rect)
  {
    if (rect.gameObject.activeSelf)
      rect.gameObject.SetActive(false);
  }

  public static float GetX(this RectTransform rect)
  {
    return rect.position.x;
  }

  public static float GetY(this RectTransform rect)
  {
    return rect.position.y;
  }

  public static float GetWidth(this RectTransform rect)
  {
    return rect.sizeDelta.x;
  }

  public static float GetHeight(this RectTransform rect)
  {
    return rect.sizeDelta.y;
  }

  public static void TransformToFit(this RectTransform rect, float newX, float newY)
  {
    float prop_new = newX / newY;
    float prop_anterior = rect.GetWidth() / rect.GetHeight();

    float newWidth = newX;
    float newHeight = newY;

    if (prop_new > prop_anterior)
    {
      newHeight = newWidth / prop_anterior;
    }
    else
    {
      newWidth = newHeight * prop_anterior;

    }
    Vector2 newSize = new Vector2(newWidth, newHeight);
    rect.sizeDelta = newSize;
  }
}
