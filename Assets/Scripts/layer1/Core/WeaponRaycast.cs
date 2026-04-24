using System.Collections;
using UnityEngine;
using TMPro;
//using UnityEditor.UIElements;

public class WeaponRaycast : MonoBehaviour
{
    // Прикрепление к скрипту нужных объектов
    public WormManager playerHealth; // Ссылка на скрипт здоровья игрока
    public Camera playerCamera; // Ссылка на камеру игрока
    public TMP_Text healthIndicator; // Текстовый элемент для отображения здоровья врага
    public Transform gunEnd; // Точка, откуда будут вылетать пули
    public SkinnedMeshRenderer skinnedMesh;
    public LineRenderer tracerEffect; // Эффект следа пули
    public GameObject impactEffect; // Эффект удара пули
    //public TagField enemyLayerMask;

    // Характеристики оружия
    public float weaponRange = 50f; // Дальность оружия
    public float takingDamage = 1f; // Наносимый себе урон
    public float dealingDamage = 50f; // Наносимый другим урон
    public float takingHeal = 500f; // Лечение от убийства врага

    // Дебаг
    public float tracerEffectDuration = 0.2f; // Длительность эффекта следа пули
    public float impactEffectDuration = 1f; // Длительность эффекта попадания

    void OnDestroy()
    {
        //PlayerHealth.OnDeath -= HealPlayer; // Отписываемся от события при уничтожении объекта
    }

    void HealPlayer()
    {
        //playerHealth.Heal(takingHeal); // Пример лечения игрока при смерти врага
    }

    void Start()
    {
        playerCamera = FindObjectOfType<Camera>();
        // Отключаем эффект следа пули в начале
        if (tracerEffect != null)
        {
            tracerEffect.enabled = false;
        }
        //PlayerHealth.OnDeath += HealPlayer; // Подписываемся на событие смерти игрока
    }

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Shoot();
            playerHealth.TakeDamage(takingDamage);
        }*/
        CheckEnemyHealth(); // Проверяем здоровье врага и отображаем его
    }

    public void Shoot()
    {
        if (skinnedMesh != null)
        {
            gunEnd.transform.position = skinnedMesh.bounds.center;
        }
        // Создаем луч из центра экрана игрока
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;

        // Проверяем попадание луча в объект
        if (Physics.Raycast(ray, out hitInfo, weaponRange))
        {
            SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
            Enemy hitPlayerHealth = hitInfo.collider.GetComponent<Enemy>(); // Получаем компонент PlayerHealth на объекте, в который попал луч
            if (hitPlayerHealth != null) // Если объект имеет компонент PlayerHealth, наносим ему урон
            {
                hitPlayerHealth.TakeDamage(dealingDamage);
                Debug.Log($"Нанесено {dealingDamage} урона");
            }

            GameObject impactObj = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)); // Воспроизводим эффект попадания
            Destroy(impactObj, impactEffectDuration); // Уничтожаем эффект через определенное время
            //Debug.Log("Попадание");

            // Отображаем след пули
            LineRenderer tracer = Instantiate(tracerEffect, gunEnd.position, Quaternion.identity);
            StartCoroutine(ShowTracerEffect(tracer, gunEnd.position, hitInfo.point));
        }
        else
        {
            Debug.Log("Промах");
            Vector3 endPoint = ray.origin + ray.direction * Mathf.Min(weaponRange, weaponRange); // Определяем точку, где луч должен закончиться
            GameObject impactObj = Instantiate(impactEffect, endPoint, Quaternion.identity); // Воспроизводим эффект попадания на этой точке
            Destroy(impactObj, impactEffectDuration); // Уничтожаем эффект через определенное время

            // Отображаем след пули
            LineRenderer tracer = Instantiate(tracerEffect, gunEnd.position, Quaternion.identity);
            StartCoroutine(ShowTracerEffect(tracer, gunEnd.position, endPoint));
        }
    }

    IEnumerator ShowTracerEffect(LineRenderer tracer, Vector3 start, Vector3 end)
    {
        tracer.SetPosition(0, start); // Устанавливаем начальную точку следа пули
        Vector3 endPoint = start + (end - start).normalized * Mathf.Min(Vector3.Distance(start, end), weaponRange); // Вычисляем конечную точку следа пули на основе weaponRange
        tracer.SetPosition(1, endPoint); // Устанавливаем конечную точку следа пули
        tracer.enabled = true; // Включаем эффект следа пули
        yield return new WaitForSeconds(tracerEffectDuration);    // Ждем некоторое время
        tracer.enabled = false; // Выключаем эффект следа пули
        Destroy(tracer.gameObject); // Уничтожаем объект следа пули
    }

    void CheckEnemyHealth()
    {
        // Создаем луч из центра экрана игрока
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, weaponRange)) // Проверяем, попал ли луч во что-то на слое врагов
        {
            Enemy enemyHealth = hit.collider.GetComponent<Enemy>(); // Получаем компонент PlayerHealth у врага
            if (enemyHealth != null) // Если враг имеет компонент PlayerHealth, отображаем его здоровье
            {
                healthIndicator.text = "Enemy Health: " + Mathf.RoundToInt(enemyHealth.health).ToString(); // Отображаем здоровье в текстовом элементе
            }
            else
            {
                healthIndicator.text = ""; // Если враг не имеет компонента PlayerHealth, скрываем текстовый элемент
            }
        }
        else
        {
            healthIndicator.text = ""; // Если луч не попал во что-то на слое врагов, скрываем текстовый элемент
        }
    }
}