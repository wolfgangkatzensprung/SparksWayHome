using UnityEngine;

public interface IInteractable
{
    bool Interact(Player player);
    Transform GetTransform();
}

//public enum InteractionType
//{
//    Short,
//    Long
//}