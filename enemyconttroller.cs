using System.Collections;
using UnityEngine;
/// <summary>
/// контроллер врага.
/// отвечает за все его функции.
/// </summary>
public class Enemycontroller : MonoBehaviour
{
    // LayerMask на котором объект будет взаимодействовать
    public LayerMask NeedLayer;

    // LayerMask который используется для определения команды объекта
    public LayerMask TeamLayer;

    // Здоровье объекта в начале
    public int HealthPoints = 20;

    // Сколько урона наносит объект
    public int DamagPoints = 10;

    // Скорость объекта
    public float Speed = 2f;

    // Ссылка на элемент интерфейса, отображающий здоровье объекта
    public GameObject HPbar;

    // Ссылка на SpriteRenderer объекта для управления его визуализацией
    public SpriteRenderer sr;

    // Ссылка на компонент анимации объекта
    private Animator animator;

    // Ссылка на компонент физики объекта (2D)
    private Rigidbody2D phisic;

    // Ссылка на объект врага
    private GameObject EnemyObj;

    // Общее количество здоровья объекта
    private int HealthPointsTotal;

    // Контроллер врага
    private EnemyController Enemy;

    // RaycastHit2D объекта, взаимодействует с другими коллайдерами
    public RaycastHit2D hit;

    // Расстояние, на которое достигает удар объекта
    public float Distance;

    // Булева переменная, определяет, наносит ли объект урон
    bool IsDamage = true;

    // Для проверки, есть ли что-нибудь между этим объектом и его целью
    public RaycastHit2D maskHit;

    // Вектор скорости объекта
    private Vector2 phisVelocity;

    // Стоимость объекта
    public int Cost;

    // Ссылка на главный управляющий класс игры (или менеджер сцены)
    private GameManager _GameManager;
    /// <summary>
    /// Метод, который вызывается до первого обновления кадра, после того как все объекты были инициализированы.
    /// </summary>
    private void Start()
    {
        try
        {
            // Находит объект типа 'GameManager' в сцене и сохраняет его в переменной '_GameManager'.
            _GameManager = FindObjectOfType<GameManager>();

            // Получает 'Animator' компонент из текущего объекта и сохраняет его в переменной 'animator'.
            animator = GetComponent<Animator>();

            // Получает 'SpriteRenderer' компонент из текущего объекта и сохраняет его в переменной 'sr'.
            sr = GetComponent<SpriteRenderer>();

            // Получает 'Rigidbody2D' компонент из текущего объекта и сохраняет его в переменной 'phisic'.
            phisic = GetComponent<Rigidbody2D>();

            // Присваивает значение 'HealthPoints' в 'HealthPointsTotal', обозначая максимальное здоровье.
            HealthPointsTotal = HealthPoints;

            // Вычисляет расстояние как размер объекта по оси X.
            Distance = gameObject.transform.localScale.x;

            // Устанавливает стоимость равной текущему значению здоровья 'HealthPoints'.
            Cost = HealthPoints;

            // есть комменты 😊
        }
        catch (System.Exception e)
        {
            // логируем ошибку во встроенной консоли Unity
            Debug.Log(e);
            throw;
        }
    }
    /// <summary>
    /// Метод обновления данных. Вызывается с частотой фиксированных кадров.
    /// </summary>
    private void FixedUpdate()
    {
        try
        {
            // Устанавливаем слой команды, соответствующий слою этого объекта.
            TeamLayer = 1 << gameObject.layer;

            // проверка уровня
            LayerCheck();
            // проверка поворота спрайта
            SpriteCheck();
            // атака
            Attack();
        }
        catch (System.Exception e)
        {
            // логируем ошибку во встроенной консоли Unity
            Debug.Log(e);
            throw;
        }
    }
    /// <summary>
    /// Метод проверки уровня объекта.
    /// 
    /// </summary>
    private void LayerCheck()
    {
        // Если этот объект находится в слое 6, это значит, что он принадлежит определенной команде.
        if (gameObject.layer == 6)
        {
            // Устанавливаем слой врага, на который будем реагировать.
            NeedLayer = 1 << 8;

            // Отменяем отражение спрайта по оси X.
            sr.flipX = false;
        }

        // Если этот объект находится в слое 8, это означает его принадлежность к другой команде.
        if (gameObject.layer == 8)
        {
            // Устанавливаем слой врага, на который будем реагировать.
            NeedLayer = 1 << 6;

            // Устанавливаем отражение спрайта по оси X.
            sr.flipX = true;
        }
    }
    /// <summary>
    /// Проверка направления поворота спрайта.
    /// В какую повёрнут, в ту идёт и атакает
    /// </summary>
    private void SpriteCheck()
    {
        // Если спрайт отражен по оси X, наш персонаж движется в одну сторону.
        if (sr.flipX)
        {
            // Устанавливаем направление движения.
            Distance = 1f;
            phisVelocity = new Vector2(Speed, 0);  // Это вектор скорости.
        }
        else  // Если спрайт не отражен, персонаж движется в другую сторону.
        {
            Distance = -1f;
            phisVelocity = new Vector2(-Speed, 0);
        }
    }
    /// <summary>
    /// Атака!!!
    /// Если на растоянии в Distance появляется какой-то объект (враг или враж. база), то происходит атака по нему.
    /// </summary>
    private void Attack()
    {
        // Выполняем лучевой замер (Raycast) в направлении движения персонажа, чтобы проверить присутствие объектов на слое 'NeedLayer'.
        maskHit = Physics2D.Raycast(gameObject.transform.position, new Vector2(Distance, 0), 1f, NeedLayer);

        // Если в результате Raycast мы нашли объект...
        if (maskHit.transform != null && maskHit.transform.tag != "PlayerBase" && maskHit.transform.tag != "EnemyBase")
        {
            // Выводим метку найденного объекта в консоль.
            Debug.Log(maskHit.transform.tag);

            // Запоминаем найденный объект врага.
            EnemyObj = maskHit.transform.gameObject;

            // Запоминаем контроллер врага.
            Enemy = EnemyObj.GetComponent<Enemycontroller>();

            // Если включено нанесение урона, начинаем атаку.
            if (IsDamage)
            {
                StartCoroutine(CooldownAttack());
            }

            // Останавливаем движение персонажа.
            phisic.velocity = Vector2.zero;

            // Меняем состояние анимации на 'неподвижное'.
            animator.SetBool("IsMove", false);
        }
        else  // Если объект не найден или он принадлежит нашей команде...
        {
            // Выполняем лучевой замер (Raycast) в направлении движения на наличие своих.
            int count = Physics2D.RaycastAll(gameObject.transform.position, new Vector2(Distance, 0), 1f, TeamLayer).Length;

            // Если рядом есть еще один наш, перестаем двигаться.
            if (count == 2)
            {
                phisic.velocity = Vector2.zero;
                animator.SetBool("IsMove", false);
            }
            else  // Если рядом нет своих, продолжаем движение.
            {
                phisic.velocity = phisVelocity;
                animator.SetBool("IsMove", true);
            }
        }
    }
    /// <summary>
    /// Сопрограмма, представляющая собой паузу между атаками.
    /// Наносит урон противнику, ожидает 1 секунду, затем позволяет нанести дополнительный урон.
    /// </summary>
    /// <returns>Возвращает ожидание.</returns>
    private IEnumerator CooldownAttack()
    {
        try
        {
            // Обозначение, что урон в текущий момент не наносится.
            IsDamage = false;
            // Применение урона к противнику.
            Enemy.Damage(DamagPoints);

            // Пауза в одну секунду. Время, в течение которого урон не будет наноситься.
            yield return new WaitForSeconds(1f);

            // После прошествия величины времени "остывания", урон снова может быть нанесён.
            IsDamage = true;
        }
        catch (System.Exception e)
        {
            // логируем ошибку во встроенной консоли Unity
            Debug.Log(e);
            throw;
        }
    }
    /// <summary>
    /// Получение урона персонажем и обновление полосы его здоровья.
    /// Если здоровье персонажа падает до 0 или меньше, персонаж уничтожается.
    /// </summary>
    /// <param name="dmg">Количество урона для применения к персонажу.</param>
    public void Damage(int dmg)
    {
        try
        {
            // Уменьшаем очки здоровья на полученный урон.
            HealthPoints -= dmg;

            // Если это убило персонажа, уничтожаем его.
            if (HealthPoints <= 0)
            {
                Destroy(gameObject);
            }

            // Обновляем отображение полосы здоровья.
            HPbar.transform.localScale = new Vector3((float)HealthPoints / HealthPointsTotal, 0.1375f, 1f);
        }
        catch (System.Exception e)
        {
            // логируем ошибку во встроенной консоли Unity 😑
            Debug.Log(e);
            throw;
        }
    }
    /// <summary>
    /// Метод для визуализации лучевых выстрелов в редакторе Unity для отладки.
    /// </summary>
    private void OnDrawGizmos()
    {
        try
        {
            // Задаем цвет луча.
            Gizmos.color = Color.red;

            // Рисуем луч.
            Gizmos.DrawRay(gameObject.transform.position, new Vector2(Distance, 0));
        }
        catch (System.Exception e)
        {
            // логируем ошибку во встроенной консоли Unity
            Debug.Log(e);
            throw;
        }
    }
}
