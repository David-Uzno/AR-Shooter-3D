using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RapidFireItem", menuName = "AR-Shooter/Items/Rapid Fire")]
public class GunSpeed : CollectableItem
{
    [Header("Rapid Fire")]
    [SerializeField] [Min(0.05f)] private float _cooldownMultiplier = 0.5f;
    [SerializeField] [Min(0.1f)] private float _duration = 5f;

    protected override bool CanCollect(CollectableContext context)
    {
        return base.CanCollect(context) && context.Shooting != null;
    }

    protected override void OnCollect(CollectableContext context)
    {
        int modifierId = context.Shooting.AddShotCooldownMultiplier(_cooldownMultiplier);

        if (modifierId < 0)
        {
            return;
        }

        StartRoutine(context, RemoveModifierAfterDuration(context.Shooting, modifierId));

        Debug.Log($"{DisplayName} activó disparo rápido durante {_duration:F1} segundos.", context.Player);
    }

    private IEnumerator RemoveModifierAfterDuration(Shooting shooting, int modifierId)
    {
        yield return new WaitForSeconds(_duration);

        if (shooting != null)
        {
            shooting.RemoveShotCooldownMultiplier(modifierId);
        }
    }
}
