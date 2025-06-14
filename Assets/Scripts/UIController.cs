using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour{


    [SerializeField] private GameObject startUi;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject gameloopUi;
    [SerializeField] private GameObject pauseUi; //??

    [SerializeField] private TMP_Text shopMenuBalance;

    [SerializeField] TMP_Text textMoneyTotal;
    [SerializeField] TMP_Text textMoneyChange;
    [SerializeField] TMP_Text buyerInfoText;
    [SerializeField] TMP_Text workTimeText;

    public void ShowChangeValue(bool val) { textMoneyChange.gameObject.SetActive(val); }

    public void UpdateMoneyTotal(int moneyTotal) {
        textMoneyTotal.text = "В кассе: " + moneyTotal.ToString() + "₽";
        shopMenuBalance.text = "Баланс: " + moneyTotal.ToString() + "₽";
    }

    public void UpdateMoneyChange(int changeVal) {
        if (changeVal > 0) {
            textMoneyChange.text = "В ладони: " + changeVal.ToString() + "₽";
        } else {
            textMoneyChange.text = string.Empty;
        }


    }

    public void UpdateBuyerInfo(string _string) {
        buyerInfoText.text = _string;
    }

    public void WorkTimeUpdate(float workTimeLeft) {
        if (workTimeLeft > 0) {
            workTimeText.text = "До конца смены: " + Mathf.CeilToInt(workTimeLeft);
        } else {
            workTimeText.text = "Конец смены. Ожидаем уход последнего покупателя...";
        }

    }

    public void ToggleGamePhaseUi(GameLoop.GamePhase gamePhase) {
        startUi?.SetActive(false);
        shopUI?.SetActive(false);
        gameloopUi?.SetActive(false);
        switch (gamePhase) {
            case (GameLoop.GamePhase.StartScreen):
                startUi?.SetActive(true);
                break;
            case (GameLoop.GamePhase.Shop):
                shopUI?.SetActive(true);
                break;
            case (GameLoop.GamePhase.Gameloop):
                gameloopUi?.SetActive(true);
                break;
        }
    }


    public void GotoGameButtonClick() {
        //Debug.Log("OnStartButtonClick");
        GameLoop.current.GotoGameloop();
    }

}
