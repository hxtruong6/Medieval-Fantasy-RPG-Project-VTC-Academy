using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Special Ability/Area Effect/Tumble"))]
public class TumbleConfig : AbilityConfig {
    public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
    {
        return objectToAttachTo.AddComponent<TumbleBehaviour>();
    }

}
