using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DynamicPavingEventPayload
{

}

public class DynamicPaving : MonoBehaviour
{
    public delegate void OnDynamicPavingEvent(DynamicPavingEventPayload g);
    // the event listener is being sent a command, but that command is specific to each tile. 
    
    // I like to be able to use touch events to dictate certain commands 
    
    // The following command:
    
    // get a paving type pallete from the bottom menu
    // drag your finger above the tiles and swipe the tiles you want to change dynamically 

    // Change the material color and shader once touched. Call this command "ChangePavingTile" 
    // "PickPavingPallete", "TouchPavementTile", "CheckForPavementChange (Listen for)",
    
    // PavementPalettePayload ( travel mode type , travel speed type, streetActivity, set Time For Activity)
    
    // responsibility: dictate street activity based on payload set by the pavement. Have appropriate methods that 
    // the pavement is dictating to objects crossing by colliders. let the pavement call the methods based on data that is being sent from the
    // pavement. (holy shit!) 

    // state change: when the pavement type changes after touch raycast hits pavement 

    // state change methods: CheckForPavementTouch() (subscribe to TouchManager), ChangePavementType(),
    

    // problem: you have to keep track of the data being transfered from one object to another (Pallete Payload -> sent to Pavement -> refactor)
    // paint as a service


    // Question: should the Touch Manager handle DynamicPavingEvents, or should the Touch Manager send out events, and the touch manager handle subscriptions?
    // prefer the latter. 
    
    public virtual void EnableEventSubscription()
    {

    }

    public virtual void DisableEventSubscription()
    {

    } 
}
