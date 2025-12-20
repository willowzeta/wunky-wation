using Content.Server._Funkystation.Science.Anomaly.Components;
using Content.Server._Funkystation.Science.Anomaly.AnomalyTypes;
using Content.Shared._Funkystation.Science.Anomaly;
using Content.Shared.Interaction;
using Content.Shared.DoAfter;
using Content.Shared.Containers.ItemSlots;


namespace Content.Server._Funkystation.Science.Anomaly.Systems;

public sealed partial class ResearchVesselSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly ItemSlotsSystem _slots = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ResearchVesselComponent, AfterInteractEvent>(OnAfterInteract);
        SubscribeLocalEvent<AnomalyContainerComponent, AnomDoAfterEvent>(OnDoAfter);
    }

    public void OnAfterInteract(EntityUid uid, ResearchVesselComponent comp, AfterInteractEvent args)
    {
        if (!args.CanReach || args.Target is not { } target)
            return;

        args.Handled = TryAnomalyTransfer((uid, comp), args.User, args.Used, target);
    }

    public bool TryAnomalyTransfer(Entity<ResearchVesselComponent> ent, EntityUid user, EntityUid used, EntityUid target)
    {
        if (!TryComp<ResearchVesselComponent>(target, out var vesselComp)
            || !TryComp<AnomalyContainerComponent>(used, out var canComp))
            return false;

        if (canComp.ContainedAnomaly == null)
            return false;

        var needHand = user != ent.Owner;

        var doAfter =
            new DoAfterArgs(EntityManager, user, 3, new AnomDoAfterEvent(), ent.Owner, target, used)
            {
                BreakOnDamage = true,
                BreakOnMove = true,
                NeedHand = needHand,
            };
        _doAfter.TryStartDoAfter(doAfter);
        return true;
    }

    public void OnDoAfter(EntityUid uid, ResearchVesselComponent comp, DoAfterEvent args)
    {
        if (args.Used == null
            || args.Target == null)
            return;

        if (!TryComp<ItemSlotsComponent>(uid, out var vesselItemSlot)
            || !TryComp<ItemSlotsComponent>(args.Used.Value, out var canItemSlot))
            return;

        if (!TryComp<AnomalyContainerComponent>(args.Used.Value, out var anomCan))
            return;

        _slots.TryEject(args.Used.Value, "anomalyCan", args.User, out anomCan.ContainedAnomaly, canItemSlot);

        if (anomCan.ContainedAnomaly == null)
            return;

        _slots.TryInsert(uid, "researchVessel", anomCan.ContainedAnomaly.Value, args.User, vesselItemSlot);




    }
}
