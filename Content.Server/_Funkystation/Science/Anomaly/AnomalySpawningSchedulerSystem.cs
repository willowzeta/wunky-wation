using Content.Server.StationEvents;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared.GameTicking.Components;
using Content.Shared._Funkystation.Science.Anomaly;
using Content.Server.Anomaly;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Map.Components;
using Robust.Shared.GameObjects;

namespace Content.Server._Funkystation.Science.Anomaly;

public sealed class AnomalySpawningSchedulerSystem : GameRuleSystem<AnomalySpawningSchedulerComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly AnomalySystem _anomaly = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] protected readonly StationSystem _stationSystem = default!;

    protected override void Started(EntityUid uid, AnomalySpawningSchedulerComponent spawningRule, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, spawningRule, gameRule, args);

        ResetAnomalyTimer(spawningRule);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<AnomalySpawningSchedulerComponent>();
        while (query.MoveNext(out var uid, out var spawningRule))
        {
            if (spawningRule.MatrixInterval > 0)
            {
                spawningRule.MatrixInterval -= frameTime;
                continue;
            }

            SpawnAnomaly("BaseAnomAbstraction");
			SetAnomalyValues(spawningRule);
            ResetAnomalyTimer(spawningRule);

			var ev = new AnomalyGeneratedEvent();
        	RaiseLocalEvent(ref ev);
            continue;
        }
    }

    private void ResetAnomalyTimer(AnomalySpawningSchedulerComponent component)
    {
        component.MatrixInterval = component.MatrixIntervalMinMax.Next(_random);
    }

	private void SpawnAnomaly(string protoId)
	{

        if (!TryGetRandomStation(out var chosenStation))
            return;

        if (!TryComp<StationDataComponent>(chosenStation, out var stationData))
            return;

        var grid = _stationSystem.GetLargestGrid(stationData);

        if (grid is null)
            return;

		EntityManager.SpawnAttachedTo(protoId, grid.Value);
	}

    private void SetAnomalyValues(AnomalySpawningSchedulerComponent schedComp)
    {

        var anomQuery = EntityQueryEnumerator<BaseAnomalyComponent>();
		while (anomQuery.MoveNext(out var uid, out var comp))
		{
			if(comp.AlreadyInitialized)
				continue;

			// there's GOTTA be a better way to do this
			comp.Severity = _random.Next(schedComp.SeverityBase - schedComp.SeverityRandom, schedComp.SeverityBase + schedComp.SeverityRandom);
			comp.Stability = _random.Next(schedComp.StabilityBase - schedComp.StabilityRandom, schedComp.StabilityBase + schedComp.StabililtyRandom);
			comp.DecayFreq = schedComp.StabilityDecayFreq;
			comp.DecayRate = schedComp.StabilityDecayRate;
			comp.Reactivity = schedComp.ReactivityBase;
			comp.Fragility = schedComp.FragilityBase;
			continue;
		}
    }


}
