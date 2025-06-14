using System.Collections.Generic;
using UnityEngine;

public class ItemDispenser : ClickableObject
{
    [SerializeField] private int maxItemCount;
    [SerializeField] private List<Transform> itemGfxTransforms;
    private int currentItemCount;

    [SerializeField] private GameLoop.Ware itemType = GameLoop.Ware.Empty;

    public void Awake()
    {
        if (maxItemCount != itemGfxTransforms.Count)
        {
            Debug.LogError("setup of ItemDispencer (" + transform.name + ") done wrong!");
        } else {
            //Debug.Log(transform.name + " set up correctly");
        }
        if (itemType == GameLoop.Ware.Empty)
        {
            Debug.LogError("set WaresType of " + transform.name + "!!");
        }
        currentItemCount = maxItemCount;
    }

    private void UpdateItemQuantity(int stock)
    {
        if (stock > maxItemCount) { Debug.LogError("wrong stock number (" + stock.ToString() + " > " + maxItemCount.ToString() + "!! )"); }
        if (stock < 0) { stock = 0; }

        foreach (Transform itemGfx in itemGfxTransforms)
        {
            itemGfx.gameObject.SetActive(false);
        }
        for (int i = 0; i < stock; i++)
        {
            itemGfxTransforms[i].gameObject.SetActive(true);
            //Debug.Log("setactive i=" + i);
        }
    }

    public void Restock() {
        UpdateItemQuantity(maxItemCount);
    }

    public int CurrentItemCount(){ return currentItemCount; }
    public GameLoop.Ware WareOfDispencer(){ return itemType; }
    public void TakeAnItem(Player player)
    {
        if (currentItemCount > 1)
        {
            player.UpdateHandheld(itemType);
            currentItemCount += -1;
            UpdateItemQuantity(currentItemCount);
        }


    }



    public override void OnPlayerInteractMain(Player player)
    {
        if (player.WareInHand() != GameLoop.Ware.Empty)
        {
            Debug.Log("Player's hand not empty");
        }
        else
        {
            TakeAnItem(player);
        }

    }
    public override void OnPlayerInteractSecondary(Player player)
    {
        if (player.WareInHand() == itemType) {
            if (currentItemCount < maxItemCount) {
                player.UpdateHandheld(GameLoop.Ware.Empty);
                currentItemCount += 1;
                UpdateItemQuantity(currentItemCount);
            }
        }

    }
}
