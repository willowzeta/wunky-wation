using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Content.Server._Funkystation.Science.Anomaly;

namespace Content.Shared._Funkystation.Science.Anomaly;

// base, universal values between all the anomaly types
// all values will be filled in by the spawning system
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(SharedBaseAnomalySystem), (AnomalySpawningSchedulerSystem))]
public sealed partial class BaseAnomalyComponent : Component
{
	// denotes if the values have already been set by the spawning system
	[DataField, AutoNetworkedField]
	public bool AlreadyInitialized = false;

    // the anomaly's current health
    [DataField, AutoNetworkedField]
    public float Stability;

    // how often the anomaly loses health
    [DataField, AutoNetworkedField]
    public float DecayFreq;

    // how much the anomaly decays when the time runs out
    [DataField, AutoNetworkedField]
    public float DecayRate;

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
    public float Severity;
}
