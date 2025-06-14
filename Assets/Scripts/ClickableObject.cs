using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] Transform lookedOnGfx;
    public void OnPlayerLook(Player player)
    {
        Debug.Log("lookingOn " + transform.gameObject.name);
        LookingState(true);
    }

    public void OnPlayerStopLooking(Player player)
    {
        Debug.Log("OnPlayerStopLooking " + transform.gameObject.name);
        LookingState(false);
    }

    public virtual void OnPlayerInteractMain(Player player) { }
    public virtual void OnPlayerInteractSecondary(Player player) { }

    private void LookingState(bool ifLookedOn)
    {
        lookedOnGfx.gameObject.SetActive(ifLookedOn);
    }
    
}
