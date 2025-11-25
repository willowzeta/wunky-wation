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

    public void OnAfterInteract(Entity<AnomalyContainerComponent> ent, AfterInteractEvent args)
    {

    }

    [ByRefEvent]
    public record struct TryContainAnomalyEvent();
}
