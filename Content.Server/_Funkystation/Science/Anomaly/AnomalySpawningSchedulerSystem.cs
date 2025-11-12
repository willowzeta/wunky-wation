using Content.Server.StationEvents;
using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Shared.GameTicking.Components;
using Content.Server.Anomaly;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Map.Components;

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

        if (!TryGetRandomStation(out var chosenStation))
            return;

        if (!TryComp<StationDataComponent>(chosenStation, out var stationData))
            return;

        var grid = _stationSystem.GetLargestGrid(stationData);

        if (grid is null)
            return;

        var query = EntityQueryEnumerator<AnomalySpawningSchedulerComponent>();
        while (query.MoveNext(out var uid, out var spawningRule))
        {
            if (spawningRule.MatrixInterval > 0)
            {
                spawningRule.MatrixInterval -= frameTime;
                continue;
            }

            var amountToSpawn = 1;
            for (var i = 0; i < amountToSpawn; i++)
            {
                _anomaly.SpawnOnRandomGridLocation(grid.Value, "RandomAnomalySpawner");
                ResetAnomalyTimer(spawningRule);
                continue;
            }
        }
    }

    private void ResetAnomalyTimer(AnomalySpawningRuleComponent component)
    {
        component.MatrixInterval = component.MatrixIntervalMinMax.Next(_random);
    }
}
