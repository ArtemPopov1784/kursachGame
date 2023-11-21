using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// менеджер сцены. Отвечает за все главные параметры
/// </summary>
public class GameManager : MonoBehaviour
{
    // Эти переменные используются для отображения значения очков здоровья и денег игрока и врага
    public Text HP;
    public Text EnemyHP;
    public Text MoneyTxt;
    public Text Money2txt;

    // Это переменная для обращения к основной камере в игровой сцене
    private Camera Cam;

    // Это значения здоровья и количества денег у игрока и врага в начале игры
    public int Health = 100;
    public int EnemyHealth = 100;
    public int Money = 0;
    public int Money2 = 0;

    // Эти изображения используются для отображения экрана победы или проигрыша
    public Image GameOverScreen;
    public Image WinningScreen;

    // Массив для урона от боёв
    public int[] dmg;

    // Словарь служит для хранения различных типов врагов и их урона
    public Dictionary<string, int> damag = new Dictionary<string, int>()
    {
        ["EasyEnemy"] = 1,
        ["NormalEnemy"] = 5,
        ["HardEnemy"] = 10
    };

    // Ссылка на объект игрока
    public GameObject Player;

    // Экземпляр менеджера игры
    public static GameManager ManagerInstance;

    // Кнопки для перемещения игрока
    public Button btLeft;
    public Button btRight;

    // Текстовые поля, отображающие цены на обновления оружия для каждого типа врага
    public Text CostEasy1;
    public Text CostNormal1;
    public Text CostHard1;
    public Text CostEasy2;
    public Text CostNormal2;
    public Text CostHard2;
    /// <summary>
    /// при создании объекта
    /// </summary>
    private void Awake()
    {
        // Проверка состояния игровой сессии или загрузка игровых данных
        Check();

        // Сохраняем этот экземпляр в статической переменной, чтобы можно было легко получить доступ из других скриптов
        ManagerInstance = this;
    }
    /// <summary>
    /// При создании сцены
    /// </summary>
    void Start()
    {
        // Когда игра начинается, ищем объект камеры и начинаем корутину для пополнения денег
        Cam = FindObjectOfType<Camera>();

        // Запускаем корутину, увеличивающую количество денег каждую секунду
        StartCoroutine(MoneyGrow());

        // Экраны победы и поражения изначально не активны
        GameOverScreen.gameObject.SetActive(false);
        WinningScreen.gameObject.SetActive(false);
    }
    /// <summary>
    /// Прирост денег
    /// </summary>
    /// <returns>Раз в 1 секунду +1$</returns>
    private IEnumerator MoneyGrow()
    {
        // Увеличение количества денег каждую секунду
        Money++;
        Money2++;

        // Ожидаем 1 секунду прежде чем повторить это снова
        yield return new WaitForSeconds(1f);

        // Запускаем корутину снова
        StartCoroutine(MoneyGrow());
    }
    /// <summary>
    /// каждый кадр обновляем здоровье и деньги
    /// </summary>
    void FixedUpdate()
    {
        // Обновление интерфейса пользователя

        // Отображение текущего количества здоровья игрока и проверка на поражение
        HP.text = "У вас " + Health;
        if (Health <= 0) GameOver();

        // Отображение текущего количества здоровья врага и проверка на победу
        EnemyHP.text = "У врага " + EnemyHealth;
        if (EnemyHealth <= 0) WinGame();

        // Отображение количества денег игрока и врага
        MoneyTxt.text = "У вас " + Money;
        Money2txt.text = "У врага " + Money2;
    }
    /// <summary>
    /// Функция, уменьшающая здоровье игрока или врага
    /// </summary>
    /// <param name="count">Количество здоровья</param>
    /// <param name="layer">Уровень маски</param>
    public void DamagePlayer(int count, int layer)
    {
        // Если слой соответствует врагу(8), уменьшаем здоровье врага, иначе уменьшаем здоровье игрока
        if (layer == 8)
        {
            EnemyHealth -= count;
        }
        else Health -= count;
    }
    /// <summary>
    /// Функция, отображающая игровой экран при проигрыше
    /// </summary>
    public void GameOver()
    {
        GameOverScreen.gameObject.SetActive(true);
    }
    /// <summary>
    /// Функция, отображающая экран победы
    /// </summary>
    public void WinGame()
    {
        WinningScreen.gameObject.SetActive(true);
    }
    /// <summary>
    /// Функция для перезапуска игры. Загружает текущую сцену заново
    /// </summary>
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// Первая версия функции урона
    /// </summary>
    /// <param name="count">Количество урона</param>
    public void DamageUnitTest(int count)
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// Функция, позволяющая выбирать урон из массива урона
    /// </summary>
    /// <param name="idDamag">ID урона в массиве</param>
    /// <returns>Урон равный значению из массива</returns>
    public int dmgChoice(int idDamag)
    {
        return dmg[idDamag];
    }

    /// <summary>
    /// Проверка на платформу
    /// Если это Андроид, то включаются кнопки для перемещения по экрану
    /// </summary>
    private void Check()
    {
        // Если приложение работает на платформе UNITY_STANDALONE...
        // Проверка проводится с помощью директивы препроцессора #if
        // ПК, ноутбук
#if UNITY_STANDALONE
        // Отключает game object btLeft
        btLeft.gameObject.SetActive(false);
        // Отключает game object btRight
        btRight.gameObject.SetActive(false);
// телефон на Android
#elif UNITY_ANDROID // Также с помощью директивы препроцессора проверка если приложение работает на платформе UNITY_ANDROID
        // Активирует game object btLeft
        btLeft.gameObject.SetActive(true);
        // Активирует game object btRight
        btRight.gameObject.SetActive(true);
#elif Unity
#endif
    }
}
