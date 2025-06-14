using UnityEngine;

public class Buyer : ClickableObject
{
    public enum BuyerPhase
    {
        Approaching,
        WaitForWare,
        WaitForChange,
        Leaving
    }

    [SerializeField] private Animator animator;
    [SerializeField] GameObject debugPole;

    private BuyerPhase buyerPhase;
    [SerializeField] private float approachTime = 5f;
    private float timeSpendApproaching;
    private Vector3 targetForApproach = Vector3.zero;
    private Vector3 spawnPoint = Vector3.zero;

    private float timeSmooth; 

    private GameLoop.Ware wareNeeded = GameLoop.Ware.Empty;
    private int changeNeeded;


    private void Awake()
    {
        spawnPoint = transform.position;
        buyerPhase = BuyerPhase.Approaching;
        timeSpendApproaching = 0f;
    }

    public void SetTargetForApproach(Transform _targetForApproach)
    {
        targetForApproach = _targetForApproach.position;
    }



    private void Update()
    {
        BuyerBehaviour();
    }

    private void BuyerBehaviour()
    {
        switch (buyerPhase)
        {
            case BuyerPhase.Approaching:
                animator.SetBool("isWalking", true);
                GameLoop.current.UpdateBuyerInfo("Ожидаем нового клиента...");


                if (targetForApproach != null)
                {
                    RotateBuyerTowards(targetForApproach);
                    timeSpendApproaching += Time.deltaTime;
                    //timeSmooth = Mathf.Clamp01(timeSpendApproaching / approachTime);
                    //timeSmooth = Mathf.SmoothStep(0f, 1f, timeSpendApproaching / approachTime);
                    transform.position = Vector3.Lerp(spawnPoint, targetForApproach, timeSpendApproaching / approachTime);
                    if (timeSpendApproaching / approachTime >= 1f)
                    {
                        buyerPhase = BuyerPhase.WaitForWare;
                        transform.position = targetForApproach;
                    }
                }
                else
                {
                    Debug.Log("waiting for targetForApproach");
                }
                break;

            case BuyerPhase.WaitForWare:
                timeSpendApproaching = 0f;
                animator.SetBool("isWalking", false);
                RotateBuyerTowards(GameLoop.current.buyerShopLookPoint.position);
                if (wareNeeded == GameLoop.Ware.Empty)
                {
                    wareNeeded = GameLoop.current.GetRandomWareInStock();
                }
                else
                {
                    GameLoop.current.UpdateBuyerInfo(wareNeeded);
                }
                break;
            case BuyerPhase.WaitForChange:
                animator.SetBool("isWalking", false);
                if (changeNeeded == 0) {
                    buyerPhase = BuyerPhase.Leaving;
                } else {
                    GameLoop.current.UpdateBuyerInfo("Покупатель ждет сдачу (" + changeNeeded + "₽)");
                }

                break;

            case BuyerPhase.Leaving:
                GameLoop.current.UpdateBuyerInfo("Довольный клиент уходит");
                animator.SetBool("isWalking", true);
                timeSpendApproaching += Time.deltaTime;
                transform.position = Vector3.Lerp(targetForApproach, spawnPoint, timeSpendApproaching / approachTime);
                RotateBuyerTowards(spawnPoint);
                if (timeSpendApproaching / approachTime >= 1f) { DestroyBuyer(); }
                break;

            default:
                GameLoop.current.UpdateBuyerInfo("default BuyerPhase (ОШИБКА)");
                break;
        }
    }

    private void DestroyBuyer()
    {
        GameLoop.current.OnBuyerLeft();
        Destroy(this.gameObject);
        //destroy callback
    }

    private void RotateBuyerTowards(Vector3 target) {
        Vector3 direction = target - transform.position;
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }

    private int CalculateMyBuyerMoney() {
        int price = GameLoop.current.GetWarePrice(wareNeeded);
        float chanceNoChangeNeed = 0.75f;
        if (Random.value < chanceNoChangeNeed) {
            changeNeeded = 0;
            return price;
        } else {
            changeNeeded = (((price + 49) / 50) * 50) - price;
            return ((price + 49) / 50) * 50; // round Up to nearest 50 
        }
    }

    public override void OnPlayerInteractMain(Player player) {
        if ( (wareNeeded != GameLoop.Ware.Empty) && (buyerPhase == BuyerPhase.WaitForWare) ) {
            if (player.WareInHand() == wareNeeded) {
                player.UpdateHandheld(GameLoop.Ware.BuyerMoney);
                GameLoop.current.MoneyInHandModify(CalculateMyBuyerMoney());
                buyerPhase = BuyerPhase.WaitForChange;
            }
        }

        if ( (buyerPhase == BuyerPhase.WaitForChange) && (player.WareInHand() == GameLoop.Ware.Change) ) {
            if (GameLoop.current.MoneyInHand() >= changeNeeded) {
                player.UpdateHandheld(GameLoop.Ware.Empty);
                GameLoop.current.MoneyInHandModify(-GameLoop.current.MoneyInHand());
                changeNeeded = 0;
            }
        }
        /*animator.SetBool("isWalking", !animator.GetBool("isWalking") );
        Debug.Log("iswalking = " + animator.GetBool("isWalking").ToString());*/
    }
}