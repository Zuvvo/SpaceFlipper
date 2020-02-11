using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollider
{

}

public interface IRayCollider : ICollider
{
    void Raycast();
    List<IRayCollider> HandleCollision(List<IRayCollider> collidersToSkip);
    void RegisterObject();
    void RegisterCollision(RaycastHit2D rayHit);
    void OnFixedUpdateTick();
    void Unregister();
}