using Content.Shared.Anomaly;

namespace Content.Server._Funkystation.Science.Anomaly.AnomalyTypes;

// base, universal values between all the anomaly types
// all values will be filled in by the spawning system
[RegisterComponent]
[AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class BaseAnomalyComponent : Component
{
    // denotes if the values have already been set by the spawning system
    [DataField, AutoNetworkedField]
    public bool AlreadyInitialized = false;

    // the anomaly's current health
    [DataField, AutoNetworkedField]
    public float Stability;

    // time of next decay event
    [DataField, AutoNetworkedField]
    public TimeSpan NextDecay = TimeSpan.Zero;

    // how often the anomaly loses health
    [DataField, AutoNetworkedField]
    public TimeSpan DecayFreq = TimeSpan.Zero;

    // how much the anomaly decays when a decay event is called
    [DataField, AutoNetworkedField]
    public int DecayRate;

    // when the anomaly is stable enough to be contained
    [DataField, AutoNetworkedField]
    public float ContainThreshold;

    // the degree to which stability changes when hit with an APE particle
    [DataField, AutoNetworkedField]
    public float Reactivity;

    // multiplier used when hit with the wrong particle type
    [DataField, AutoNetworkedField]
    public float Fragility;

    // base severity. placeholder for now
    [DataField, AutoNetworkedField]
    public int Severity;

    // the particle type that increases stability
    [DataField, AutoNetworkedField]
    public AnomalousParticleType StabilizingParticle;
}

