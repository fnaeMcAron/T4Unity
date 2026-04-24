using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RodionController : CharacterBase
{
    [Header("Настройки Родиона")]
    public WeaponRaycast raycast;

    [Header("Множители стиля")]
    public int stylePerHit = 20;
    public int stylePerKill = 40;
    public int stylePerDodge = 10;
    public int stylePerCombo = 30;

    public override void OnCharacterSelected()
    {
        base.OnCharacterSelected();
        if (styleManager != null)
        {
            styleManager.SwitchToRodionStyle();
        }
    }

    public override void OnCharacterDeselected()
    {
        base.OnCharacterDeselected();
    }

    public override void PerformMeleeAttack()
    {
        Debug.Log("Родион: атака лобзиком");
        if (styleManager != null && styleManager.IsStyleActive())
        {
            styleManager.AddStylePoints(stylePerHit, "Атака лобзиком");
        }
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Родион: выстрел аннигилятором");
        ShootAnnihilator();
        if (styleManager != null && styleManager.IsStyleActive())
        {
            styleManager.AddStylePoints(stylePerHit, "Выстрел аннигилятором");
        }
    }

    public override void UseAbility(bool isHold)
    {
        if (isHold)
        {
            Debug.Log("Родион: таунт");
            Taunt(true);
        }
        else
        {
            Taunt(false);
        }


    }

    public override void Dodge()
    {
        Debug.Log("Родион: уворот");
        // TO DO: уворот
    }

    public override void PerformMeleeChargeAttack()
    {

    }

    public override void PerformRangedAim()
    {

    }

    void ShootAnnihilator()
    {
        if (raycast != null)
        {
            raycast.Shoot();
        }
        else
        {
            Debug.LogWarning("WeaponRaycast не назначен для Родиона");
        }
    }

    void Taunt(bool isHold)
    {

    }
}

/*
using UnityEngine;

public class RodionController : CharacterBase
{
    [Header("Настройки Родиона - Стиль")]
    public StyleManager styleManager;
    
    [Header("Оружие")]
    public GameObject jigsawWeapon; // Лобзик для ближнего боя
    public GameObject annihilatorWeapon; // Аннигилятор для дальнего боя
    
    [Header("Множители стиля")]
    public int stylePerHit = 25;
    public int stylePerKill = 50;
    public int stylePerDodge = 15;
    public int stylePerCombo = 100;
    
    [Header("Визуальные эффекты")]
    public GameObject styleTrailEffect;
    public Material[] styleMaterials; // Материалы для разных уровней стиля
    
    private bool isWeaponActive = true;
    private int comboCount = 0;
    private float lastAttackTime = 0f;
    private float comboWindow = 2f; // Окно для комбо в секундах

    void Start()
    {
        if (styleManager == null)
            styleManager = StyleManager.Instance;
            
        InitializeWeapons();
    }

    public override void PerformMeleeAttack()
    {
        Debug.Log("Родион: атака лобзиком");
        JigsawAttack();
        
        // Стиль за атаку
        styleManager?.AddStylePoints(stylePerHit, "Атака лобзиком");
        UpdateCombo();
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Родион: выстрел аннигилятором");
        AnnihilatorAttack();
        
        // Стиль за дальнюю атаку
        styleManager?.AddStylePoints(stylePerHit, "Выстрел аннигилятором");
        UpdateCombo();
    }

    public override void UseAbility()
    {
        Debug.Log("Родион: смена оружия и стильная поза");
        ToggleWeapon();
        StylePose();
    }

    public override void Dodge()
    {
        Debug.Log("Родион: стильный уворот");
        StylishDodge();
        
        // Стиль за уворот
        styleManager?.AddStylePoints(stylePerDodge, "Стильный уворот");
    }

    private void JigsawAttack()
    {
        // Ближняя атака лобзиком
        if (jigsawWeapon != null && isWeaponActive)
        {
            // Анимация атаки
            PlayWeaponSwing();
            
            // Проверка попадания
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * 2f, 1.5f);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    OnEnemyHit(hit.gameObject);
                }
            }
        }
    }

    private void AnnihilatorAttack()
    {
        // Дальняя атака аннигилятором
        if (annihilatorWeapon != null && !isWeaponActive)
        {
            // Создание снаряда аннигилятора
            GameObject projectile = CreateAnnihilatorProjectile();
            
            // Применение множителя урона от стиля
            float damageMultiplier = styleManager?.GetDamageMultiplier() ?? 1f;
            // TO DO: передать множитель урона в снаряд
        }
    }

    private void ToggleWeapon()
    {
        isWeaponActive = !isWeaponActive;
        
        // Визуальное переключение оружия
        if (jigsawWeapon != null)
            jigsawWeapon.SetActive(isWeaponActive);
            
        if (annihilatorWeapon != null)
            annihilatorWeapon.SetActive(!isWeaponActive);
            
        Debug.Log($"Родион сменил оружие на: {(isWeaponActive ? "Лобзик" : "Аннигилятор")}");
        
        // Стиль за смену оружия
        styleManager?.AddStylePoints(30, "Смена оружия");
    }

    private void StylishDodge()
    {
        // Уворот с элементами стиля
        float styleBonus = styleManager?.GetMoveSpeedBonus() ?? 0f;
        float dodgeSpeed = moveSpeed * (1.5f + styleBonus);
        
        // TO DO: реализация уворота с трейлами и эффектами
        CreateDodgeTrail();
    }

    private void StylePose()
    {
        // Стильная поза - увеличивает стиль, но оставляет уязвимым
        Debug.Log("Родион принимает стильную позу!");
        
        // Большой бонус к стилю за риск
        styleManager?.AddStylePoints(75, "Стильная поза");
        
        // Визуальный эффект
        CreatePoseEffect();
    }

    private void UpdateCombo()
    {
        // Система комбо
        if (Time.time - lastAttackTime <= comboWindow)
        {
            comboCount++;
            if (comboCount % 3 == 0) // Каждые 3 удара в комбо
            {
                styleManager?.AddStylePoints(stylePerCombo, $"Комбо x{comboCount}");
                CreateComboEffect();
            }
        }
        else
        {
            comboCount = 1;
        }
        
        lastAttackTime = Time.time;
    }

    private void OnEnemyHit(GameObject enemy)
    {
        // Обработка попадания по врагу
        Debug.Log($"Родион попал по врагу! Стиль +{stylePerHit}");
        
        // TO DO: логика нанесения урона с учетом множителя стиля
        float damageMultiplier = styleManager?.GetDamageMultiplier() ?? 1f;
        
        // Визуальная обратная связь
        CreateHitEffect(enemy.transform.position);
    }

    public void OnEnemyKilled()
    {
        // Вызывается при убийстве врага
        styleManager?.AddStylePoints(stylePerKill, "Убийство");
        comboCount++;
    }

    // Визуальные эффекты
    private void CreateAnnihilatorProjectile()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = transform.position + transform.forward + Vector3.up;
        projectile.transform.localScale = Vector3.one * 0.5f;
        
        // Цвет в зависимости от уровня стиля
        Color styleColor = styleManager?.currentStyleLevel.styleColor ?? Color.white;
        projectile.GetComponent<Renderer>().material.color = styleColor;
        
        // Физика снаряда
        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = transform.forward * 15f;
        
        Destroy(projectile, 3f);
    }

    private void CreateDodgeTrail()
    {
        if (styleTrailEffect != null)
        {
            GameObject trail = Instantiate(styleTrailEffect, transform.position, Quaternion.identity);
            trail.transform.SetParent(transform);
            
            // Цвет трейла в зависимости от стиля
            Color styleColor = styleManager?.currentStyleLevel.styleColor ?? Color.white;
            var trailRenderer = trail.GetComponent<TrailRenderer>();
            if (trailRenderer != null)
            {
                trailRenderer.startColor = styleColor;
                trailRenderer.endColor = new Color(styleColor.r, styleColor.g, styleColor.b, 0f);
            }
            
            Destroy(trail, 2f);
        }
    }

    private void CreatePoseEffect()
    {
        GameObject poseAura = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        poseAura.transform.position = transform.position;
        poseAura.GetComponent<Renderer>().material.color = styleManager?.currentStyleLevel.styleColor ?? Color.yellow;
        poseAura.transform.localScale = Vector3.one * 3f;
        
        // Сделать невидимым коллайдер
        Destroy(poseAura.GetComponent<Collider>());
        Destroy(poseAura, 1.5f);
    }

    private void CreateComboEffect()
    {
        GameObject comboText = GameObject.CreatePrimitive(PrimitiveType.Quad);
        comboText.transform.position = transform.position + Vector3.up * 3f;
        comboText.GetComponent<Renderer>().material.color = styleManager?.currentStyleLevel.styleColor ?? Color.red;
        comboText.transform.localScale = new Vector3(2f, 1f, 1f);
        Destroy(comboText, 1f);
    }

    private void CreateHitEffect(Vector3 position)
    {
        GameObject hitEffect = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hitEffect.transform.position = position;
        hitEffect.GetComponent<Renderer>().material.color = styleManager?.currentStyleLevel.styleColor ?? Color.red;
        hitEffect.transform.localScale = Vector3.one * 0.3f;
        Destroy(hitEffect, 0.5f);
    }

    private void PlayWeaponSwing()
    {
        // TO DO: анимация взмаха оружием
        Debug.Log("Взмах оружием!");
    }

    private void InitializeWeapons()
    {
        // Инициализация начального состояния оружия
        if (jigsawWeapon != null)
            jigsawWeapon.SetActive(isWeaponActive);
            
        if (annihilatorWeapon != null)
            annihilatorWeapon.SetActive(!isWeaponActive);
    }

    // Обновление визуала в зависимости от стиля
    void Update()
    {
        UpdateStyleVisuals();
    }

    private void UpdateStyleVisuals()
    {
        // TO DO: обновление материалов, эффектов в зависимости от уровня стиля
        // Например: свечение, частицы, пост-обработка
    }
}
*/
