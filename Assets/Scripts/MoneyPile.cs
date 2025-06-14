using UnityEngine;

public class MoneyPile : ClickableObject
{
    [SerializeField] private int pileValue;
    public override void OnPlayerInteractMain(Player player)
    {
        //Debug.Log("boopd");
        if ((player.WareInHand() != GameLoop.Ware.Empty) && (player.WareInHand() != GameLoop.Ware.Change))
        {
            Debug.Log("Player's hand is occupied");
        }
        else
        {
            if (GameLoop.current.MoneyTotal() >= pileValue)
            {
                player.UpdateHandheld(GameLoop.Ware.Change);
                GameLoop.current.MoneyTotalModify(-pileValue);
                GameLoop.current.MoneyInHandModify(pileValue);
            }
        }
    }

}