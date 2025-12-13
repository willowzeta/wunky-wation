using Content.Server._Funkystation.Science.Anomaly.AnomalyTypes;
using Content.Server._Funkystation.Science.Anomaly.Components;
using Content.Server._Funkystation.Science.Anomaly.Misc;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;

namespace Content.Server._Funkystation.Science.Anomaly.Systems;

public sealed class AnomalyContainerSystem : EntitySystem
{
    [Dependency] private readonly ItemSlotsSystem _slots = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnomalyContainerComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<AnomalyContainerComponent, AnomCanDoAfterEvent>(OnDoAfter);
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

        var ev = new TryContainAnomalyEvent();
        RaiseLocalEvent(target, ref ev);

        var doAfter =
            new DoAfterArgs(EntityManager, user, 10, new AnomCanDoAfterEvent(), ent.Owner, target)
            {
                BreakOnDamage = true,
                BreakOnMove = true,
            };

        _doAfter.TryStartDoAfter(doAfter);
        return true;
    }

    public void OnDoAfter(EntityUid uid, AnomalyContainerComponent comp, AnomCanDoAfterEvent args)
    {
        if (!TryComp<ItemSlotsComponent>(uid, out var itemSlots))
        {
            return;
        }

        if (args.Target == null)
        {
            return;
        }

        EnsureComp<ItemComponent>(args.Target.Value);
        _slots.TryInsert(uid, "anomalyCan", args.Target.Value, args.User, itemSlots);
    }


    [ByRefEvent]
    public record struct TryContainAnomalyEvent();
}
