using System.Collections.Generic;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    public static GameLoop current;



    [SerializeField] private UIController uiController;
    [SerializeField] private GameObject buyerPrefab;
    [SerializeField] private Transform buyerSpawnPoint;
    [SerializeField] private Transform buyerBuyPoint;
    public Transform buyerShopLookPoint;

    [SerializeField] private float workDayDuration;
    private float workdayTimeLeft;
    [SerializeField] private float newBuyerCooldown;
    [SerializeField] private float newBuyerCooldownDiff;
    private float currentBuyerCooldown;

    

    private bool buyerPresent = false;

    [SerializeField] private List<ItemDispenser> itemDispencers;
    [SerializeField] private int moneyTotal;
    private int moneyInHand;

    public enum Ware
    {
        Empty,
        Watermelon,
        Melon,
        Beer,
        Strawberry,
        BuyerMoney,
        Change
    }

    public enum GamePhase {
        StartScreen,
        Gameloop,
        Shop
    }

    private GamePhase currentGamePhase;


    private void Start()
    {
        if (GameLoop.current == null)
        {
            GameLoop.current = this;
        }
        else
        {
            Destroy(gameObject);
        }
        if(uiController == null) {
            Debug.LogError("UiController == null");
        }
        GotoStartScreen();

    }


    public void GotoGameloop() {
        currentGamePhase = GamePhase.Gameloop;
        UpdateGamePhase();
    }

    public void GotoShop() {
        currentGamePhase = GamePhase.Shop;
        UpdateGamePhase();
    }

    public void GotoStartScreen() {
        currentGamePhase = GamePhase.StartScreen;
        UpdateGamePhase();
    }
    
    private void UpdateGamePhase() {
        if (moneyInHand >= 0) {
            MoneyTotalModify(moneyInHand);
            MoneyInHandModify(-moneyInHand);
        }

        switch (currentGamePhase) {
            case (GamePhase.StartScreen):
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.Confined;
                uiController.ToggleGamePhaseUi(currentGamePhase);
                break;
            case (GamePhase.Shop):
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.Confined;
                uiController.ToggleGamePhaseUi(currentGamePhase);
                break;
            case (GamePhase.Gameloop):
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                workdayTimeLeft = workDayDuration;
                currentBuyerCooldown = 0f;
                MoneyTotalModify(0);
                MoneyInHandModify(0);
                foreach(ItemDispenser itemDispencer in itemDispencers) {
                    itemDispencer.Restock();
                }

                uiController.ToggleGamePhaseUi(currentGamePhase);
                break;
        }
    }
    
    
    //GameObject newBuyerDebug = null;

    private void Update()
    {
        /*
                Debug.LogWarning("GameLoop update debug");

                if (newBuyerDebug == null) {
                    newBuyerDebug = Instantiate(buyerPrefab, buyerSpawnPoint);
                    Debug.Log("spawn");
                    newBuyerDebug.GetComponent<Buyer>().SetTargetForApproach(buyerBuyPoint);

                }

        return;
        */

        workdayTimeLeft -= Time.deltaTime;
        uiController.WorkTimeUpdate(workdayTimeLeft);

        if (workdayTimeLeft <= 0f) {
            workdayTimeLeft = 0f;
            return;
            //waiting for Buyer callback;
        }

        if (!buyerPresent && currentBuyerCooldown <= 0f)
        {
            GameObject newBuyer;
            newBuyer = Instantiate(buyerPrefab, buyerSpawnPoint);
            newBuyer.GetComponent<Buyer>().SetTargetForApproach(buyerBuyPoint);
            buyerPresent = true;
            //time to spawn 
        }
        else if (!buyerPresent)
        {
            //waiting
            currentBuyerCooldown -= Time.deltaTime;
        }
        else
        {
            //buyer present
            currentBuyerCooldown = newBuyerCooldown;
        }
        
    }


    public void OnBuyerLeft() {
        UpdateBuyerInfo("Ожидаем нового клиента...");
        buyerPresent = false;
        if (workdayTimeLeft <= 0f) {
            currentGamePhase = GamePhase.Shop;
            UpdateGamePhase();
        }
    }
    public void MoneyTotalModify(int delta) { 
        moneyTotal += delta;
        uiController.UpdateMoneyTotal(moneyTotal);
    }
    public void MoneyInHandModify(int delta) { 
        moneyInHand += delta;
        uiController.UpdateMoneyChange(moneyInHand);
    }

    public void UpdateBuyerInfo(string _string) {
        uiController.UpdateBuyerInfo(_string);
    }
    public void UpdateBuyerInfo(Ware wareNeeded) {
        string result;
        switch (wareNeeded){
            case Ware.Melon:
                result = "Покупателю нужна Дыня";
                break;
            case Ware.Watermelon:
                result = "Покупателю нужен Арбуз";
                break;
            case Ware.Beer:
                result = "Покупателю нужно Пиво!";
                break;
            case Ware.Strawberry:
                result = "Покупателю нужна Клубника";
                break;
            default:
                result = "Покупателю нужно что-то странное (ОШИБКА)";
                break;
        }
        result += " (cтоимость " + GetWarePrice(wareNeeded) + "₽)";
        uiController.UpdateBuyerInfo(result);
    }

    public int MoneyTotal() { return moneyTotal; }
    public int MoneyInHand() { return moneyInHand; }

    public Ware GetRandomWareInStock()
    {
        List<ItemDispenser> actualWares = new List<ItemDispenser>();
        actualWares = itemDispencers;
        for (int i = actualWares.Count - 1; i >= 0; i--)
        {
            if (actualWares[i].CurrentItemCount() == 0)
            {
                actualWares.RemoveAt(i);
            }
        }
        return actualWares[Random.Range(0, actualWares.Count)].WareOfDispencer();
    }

    public int GetWarePrice(GameLoop.Ware ware) {
        switch (ware) {
            case Ware.Melon:
                return 43; 
            case Ware.Watermelon:
                return 38;
            case Ware.Beer:
                return 75;
            case Ware.Strawberry:
                return 220;
            default:
                return -1;
        }
    }
}