using Content.Server._Funkystation.Science.Anomaly.AnomalyTypes;
using Content.Server._Funkystation.Science.Anomaly.Components;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Containers.ItemSlots;

namespace Content.Server._Funkystation.Science.Anomaly.Systems;

public sealed class AnomalyContainerSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _slots = default!;

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
        if (!TryComp<BaseAnomalyComponent>(target, out var anomalyComp))
        {
            return false;
        }

        if (!TryComp<ItemSlotsComponent>(ent, out var itemSlots))
        {
            return false;
        }


        EnsureComp<ItemComponent>(target);

        var ev = new TryContainAnomalyEvent();
        RaiseLocalEvent(target, ref ev);

        _slots.TryInsert(ent.Owner, "anomalyCan", target, user, itemSlots);

        return true;
    }


    [ByRefEvent]
    public record struct TryContainAnomalyEvent();
}
