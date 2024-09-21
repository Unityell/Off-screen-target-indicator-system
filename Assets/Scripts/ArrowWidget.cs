using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ArrowWidget : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Icon iconPrefab;
    [SerializeField] private Canvas canvas;
    private Camera mainCamera;

    // Пул всех иконок
    private List<Icon> iconPool = new List<Icon>();
    
    // Список точек
    [SerializeField] private List<Transform> points = new List<Transform>();

    void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Setup(Transform Player)
    {
        player = Player;
    }

    // Создание новой иконки
    private Icon CreateNewIcon()
    {
        var newIcon = Instantiate(iconPrefab, transform);
        newIcon.Initialization(mainCamera, canvas);
        newIcon.image.color = new Color(1, 1, 1, 0); // Иконка не видима по умолчанию
        iconPool.Add(newIcon);
        return newIcon;
    }

    // Добавление новой точки
    public void AddPoint(Transform point)
    {
        points.Add(point);
    }

    // Удаление точки
    public void RemovePoint(Transform point)
    {
        points.Remove(point);
    }

    // Проверяем видимость объекта в пределах канваса
    private bool IsObjectVisibleInCanvas(Vector3 position)
    {
        Vector2 viewPos = mainCamera.WorldToViewportPoint(position);
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1;
    }

    // Обновляем иконки
    private void UpdatePointer()
    {
        if (player == null || points.Count == 0) return;

        foreach (var point in points.ToList()) // Используем ToList для безопасной итерации
        {
            if (!point) continue;

            // Проверяем, есть ли уже иконка для этой точки
            bool hasMatchingIcon = iconPool.Exists(icon => icon.gameObject.activeSelf && icon.Target == point);
            if (!hasMatchingIcon)
            {
                // Если таргет за экраном, ищем неактивную иконку
                if (!IsObjectVisibleInCanvas(point.position))
                {
                    var inactiveIcon = iconPool.Find(icon => !icon.gameObject.activeSelf);
                    if (inactiveIcon != null)
                    {
                        inactiveIcon.Target = point;
                        // Сначала перемещаем, затем показываем
                        inactiveIcon.MoveToPoint(player.position);
                        inactiveIcon.image.color = new Color(1, 1, 1, 1); // Делаем видимой
                        inactiveIcon.gameObject.SetActive(true);
                    }
                    else
                    {
                        var newIcon = CreateNewIcon();
                        newIcon.Target = point;
                        newIcon.MoveToPoint(player.position); // Перемещаем к игроку
                        newIcon.image.color = new Color(1, 1, 1, 1); // Делаем видимой
                        newIcon.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    // Контролируем жизнь иконок
    private void IconsLifeController()
    {
        if (player == null) return;
        
        foreach (var icon in iconPool)
        {
            if (!icon.gameObject.activeSelf) continue;

            // Проверяем, есть ли у иконки цель
            if (icon.Target == null || !icon.Target.gameObject.activeSelf)
            {
                icon.gameObject.SetActive(false);
                icon.image.color = new Color(1, 1, 1, 0); // Делаем иконку невидимой
                icon.Target = null; // Зануляем таргет
            }
            else
            {
                // Проверяем, находится ли цель в пределах видимости
                if (IsObjectVisibleInCanvas(icon.Target.position))
                {
                    // Цель видима — скрываем иконку и зануляем таргет
                    icon.gameObject.SetActive(false);
                    icon.image.color = new Color(1, 1, 1, 0); // Делаем иконку невидимой
                    icon.Target = null; // Зануляем таргет
                }
                else
                {
                    // Если цель за экраном, показываем иконку и перемещаем её к игроку
                    icon.MoveToPoint(player.position);
                    icon.image.color = new Color(1, 1, 1, 1); // Делаем иконку видимой
                }
            }
        }
    }

    void FixedUpdate()
    {
        IconsLifeController();
    }

    void Update()
    {
        UpdatePointer();
    }
}
