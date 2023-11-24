using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text HP;
    public Text EnemyHP;
    public Text MoneyTxt;
    public Text Money2txt;

    public void UpdateHealth(int health)
    {
        HP.text = "У вас " + health;
    }

    public void UpdateEnemyHealth(int enemyHealth)
    {
        EnemyHP.text = "У врага " + enemyHealth;
    }

    public void UpdateMoney(int money)
    {
        MoneyTxt.text = "У вас " + money;
    }

    public void UpdateEnemyMoney(int enemyMoney)
    {
        Money2txt.text = "У врага " + enemyMoney;
    }

}