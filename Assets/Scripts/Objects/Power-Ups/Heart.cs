using UnityEngine;

[CreateAssetMenu(fileName = "HeartItem", menuName = "AR-Shooter/Items/Heart")]
public class Heart : CollectableItem
{
    [Header("Healing")]
    [SerializeField] [Min(1)] private int _healAmount = 1;

    protected override bool CanCollect(CollectableContext context)
    {
        return base.CanCollect(context) && context.Player.LifeCurrent < context.Player.LifeMax;
    }

    protected override void OnCollect(CollectableContext context)
    {
        int restoredLife = context.Player.RestoreLife(_healAmount);

        Debug.Log($"{DisplayName} restauró {restoredLife} puntos de vida.", context.Player);
    }
}
