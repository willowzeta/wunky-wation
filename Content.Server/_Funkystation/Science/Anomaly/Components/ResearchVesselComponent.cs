namespace Content.Server._Funkystation.Science.Anomaly.Components;

[RegisterComponent]
public sealed partial class ResearchVesselComponent : Component
{
    //uid of stored anomaly
    [DataField]
    public EntityUid? Anomaly;


}
