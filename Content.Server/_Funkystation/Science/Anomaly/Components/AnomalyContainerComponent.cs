using Robust.Shared.Timing;

namespace Content.Server._Funkystation.Science.Anomaly.Components;

[RegisterComponent]
public sealed partial class AnomalyContainerComponent : Component
{
    [DataField]
    public EntityUid? ContainedAnomaly;
}
