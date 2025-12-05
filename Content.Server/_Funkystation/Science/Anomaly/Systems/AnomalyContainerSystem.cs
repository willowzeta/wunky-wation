using Content.Server._Funkystation.Science.Anomaly.AnomalyTypes;
using Content.Server._Funkystation.Science.Anomaly.Components;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;

namespace Content.Server._Funkystation.Science.Anomaly.Systems;

public sealed class AnomalyContainerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnomalyContainerComponent, AfterInteractEvent>(OnAfterInteract);
    }

    public void OnAfterInteract(EntityUid uid, AnomalyContainerComponent comp, AfterInteractEvent args)
    {
        if (!args.CanReach || args.Target is not { } target)
            return;

        args.Handled = TryContainAnomaly((uid, comp), args.User, target);
    }

    public bool TryContainAnomaly(Entity<AnomalyContainerComponent> ent, EntityUid user, EntityUid target)
    {
        if (!TryComp<BaseAnomalyComponent>(target, out var anomaly))
        {
            return false;
        }

        return true;
    }


    [ByRefEvent]
    public record struct TryContainAnomalyEvent();
}
