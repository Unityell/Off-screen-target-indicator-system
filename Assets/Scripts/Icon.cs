using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    public Image image;
    private Camera mainCamera;
    private RectTransform canvasRect;
    [HideInInspector] public Transform Target;

    public void Initialization(Camera camera, Canvas canvas)
    {
        canvasRect = canvas.transform as RectTransform;
        mainCamera = camera;
        transform.eulerAngles = Vector3.zero;
    }

    public void MoveToPoint(Vector3 playerPosition)
    {
        Vector3 fromTargetToPlayer = Target.transform.position - playerPosition;
        Ray ray = new Ray(playerPosition, fromTargetToPlayer);

        // Вычисляем расстояние до ближайшей плоскости фруструма камеры
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < 4; i++) // Работаем только с первыми 4 плоскостями
        {
            if (planes[i].Raycast(ray, out float distance))
            {
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }

        // Определяем позицию иконки
        Vector3 screenPos = mainCamera.WorldToScreenPoint(ray.GetPoint(minDistance));
        screenPos.z = 0;
        transform.position = screenPos;

        RectTransform rectTransform = transform as RectTransform;

        // Клэмпинг позиции иконки, чтобы она оставалась внутри экрана
        Vector2 clampedPosition = rectTransform.anchoredPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -canvasRect.sizeDelta.x / 2 + rectTransform.sizeDelta.x / 2, canvasRect.sizeDelta.x / 2 - rectTransform.sizeDelta.x / 2);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -canvasRect.sizeDelta.y / 2 + rectTransform.sizeDelta.y / 2, canvasRect.sizeDelta.y / 2 - rectTransform.sizeDelta.y / 2);
        rectTransform.anchoredPosition = clampedPosition;

        // Поворачиваем иконку в сторону цели
        Vector3 directionToTarget = (mainCamera.WorldToScreenPoint(Target.position) - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
        transform.eulerAngles += Vector3.forward * 90; // Корректируем угол поворота
    }
}
