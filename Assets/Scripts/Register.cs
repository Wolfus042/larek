using UnityEngine;

public class Register : ClickableObject
{
    public override void OnPlayerInteractMain(Player player)
    {
        if ((player.WareInHand() == GameLoop.Ware.Change) || (player.WareInHand() == GameLoop.Ware.BuyerMoney))
        {
            player.UpdateHandheld(GameLoop.Ware.Empty);
            GameLoop.current.MoneyTotalModify(GameLoop.current.MoneyInHand());
            GameLoop.current.MoneyInHandModify(-GameLoop.current.MoneyInHand());

        }
    }

}