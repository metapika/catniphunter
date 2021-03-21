using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public int currentMoney = 2000;
    public int fee = 200;
    public TextMeshProUGUI moneyText;
    public static MoneyManager singleton;

    private void Awake() {
        if (singleton == null) singleton = this;
        else if (singleton != this) { Destroy(this.gameObject);  return; }
        
        DontDestroyOnLoad(gameObject);
        
        UpdateUI();
    }
    public void AddMoney(int amount)
    {
        currentMoney += amount;

        UpdateUI();
    }

    public void RemoveMoney(int amount)
    {
        if ((currentMoney - amount) < 0)
        {
            currentMoney = 0;
        } else {
            currentMoney -= amount;
        }

        UpdateUI();
    }

    private void UpdateUI() {
        moneyText.text = currentMoney.ToString();
    }
}
