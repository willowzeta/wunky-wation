using Content.Shared.Destructible.Thresholds;

namespace Content.Server._Funkystation.Science.Anomaly;

[RegisterComponent]
public sealed partial class AnomalySpawningSchedulerComponent : Component
{
    // minimum an maximum time to next anomaly, in seconds
    [DataField]
    public MinMax MatrixIntervalMinMax = new(3 * 60, 10 * 60);

    // time to next anomaly, in seconds
    [DataField]
    public float MatrixInterval;

    // the current amount of catalyst gas that has been injected into the matrix with the resonator.
    [DataField]
    public float MatrixCapacity = 0;

    // the maximum amount of catalyst gas that can be stored in the matrix.
    // going over this threshold causes an anomalous phenomenon to spawn, which is generally bad.
    [DataField]
    public float MatrixCapacityLimit = 5000;

    // the amount of catalyst gas depleted from the matrix every game tick.
    [DataField]
    public float MatrixCapacityDecay = 100;

    // base value that dictates the overall danger of the next anomaly.
    // many other values are derived from this as a base
    [DataField]
    public float SeverityBase = 25;

    // the degree to which the base severity will deviate, so anomalies aren't super predictable
    [DataField]
    public float SeverityRandom = 5;

    // weights of each anomaly type
    [DataField]
    public float WeightAbstraction = 0.8f;

    [DataField]
    public float WeightObject = 0.5f;

    [DataField]
    public float WeightAffliction = 0.2f;

    [DataField]
    public float WeightPhenomenon = 0.1f;

    // odds of a modifier being added. phenomena can't recieve these
    // derived dynamically from SeverityBase and SeverityRandom
    [DataField]
    public float ModifierChance;

    // odds of the modifier function being called again when a modifier is added
    // derived dynamically from SeverityBase and SeverityRandom
    [DataField]
    public float ModifierRepeatChance;

    // all values past this point are here to be passed to the anomaly
    // it's important that these be defined here, since the resonator allows them to be modified before the anomaly spawns

    // the base health ("stability") of the next anomaly. the anomaly is destroyed when it reaches 0
    [DataField]
    public float StabiltyBase = 500;

    // the degree of random deviation from the base stability
    [DataField]
    public float StabilityRandom = 25;

    // how often the stability naturally decays, in seconds
    [DataField]
    public float StabilityDecayFreq = 15;

    // the amount the stability reduces every second
    [DataField]
    public float StabilityDecayRate = 5;

    // how much the stability of the anomaly will change upon hit with a APE/CHIMP particle
    [DataField]
    public float ReactivityBase = 20;

    // multiplier to reactivity for being hit with the wrong particle type
    [DataField]
    public float ReactivityFragility = 1f;

    // if the anomaly is an abstraction:
    // defines how often it pulses, in seconds
    [DataField]
    public float AbstractionPulseInterval;
    // defines the radius of the pulse's effect
    [DataField]
    public float AbstractionPulseStrength;

    // if the anomaly is an object:
    // this defines the odds of hostility, out of 100
    [DataField]
    public float ObjectHostileChance;
    // the interval between "active" periods of the hostile object
    [DataField]
    public float ObjectHostileInterval;
    // how long the hostile object remains active
    [DataField]
    public float ObjectHostileTime;
    // what kind of controller the object has
    // 0 = no ai, 1 = simplemob, 2 = ghostrole
    [DataField]
    public float ObjectIntelligence = 0;


}
[ByRefEvent]
public record struct AnomalyGeneratedEvent();


