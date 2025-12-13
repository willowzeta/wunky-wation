using Content.Server.Station.Components;
using Content.Server.Station.Systems;
using Content.Server.GameTicking.Rules;
using Content.Shared.GameTicking.Components;
using Content.Server._Funkystation.Science.Anomaly.AnomalyTypes;
using Content.Server._Funkystation.Science.Anomaly.Components;
using Content.Server.Anomaly;
using Content.Shared.Physics;
using Content.Shared.CCVar;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Chat.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Map.Components;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Configuration;

namespace Content.Server._Funkystation.Science.Anomaly.Systems;

public sealed class AnomalySpawningSchedulerSystem : GameRuleSystem<AnomalySpawningSchedulerComponent>
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly AnomalySystem _anomaly = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] protected readonly StationSystem _stationSystem = default!;
    [Dependency] private readonly SharedMapSystem _mapSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly AtmosphereSystem _atmosphere = default!;
    [Dependency] private readonly IConfigurationManager _configuration = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

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
        }
    }

    private void ResetAnomalyTimer(AnomalySpawningSchedulerComponent component)
    {
        component.MatrixInterval = component.MatrixIntervalMinMax.Next(_random);
    }

    public void SetAnomalyValues(AnomalySpawningSchedulerComponent schedComp)
    {

        var anomQuery = EntityQueryEnumerator<BaseAnomalyComponent>();
		while (anomQuery.MoveNext(out var uid, out var comp))
		{
			if(comp.AlreadyInitialized)
				continue;

			// there's GOTTA be a better way to do this
			comp.Severity = _random.Next(schedComp.SeverityBase - schedComp.SeverityRandom, schedComp.SeverityBase + schedComp.SeverityRandom);
			comp.Stability = _random.Next(schedComp.StabilityBase - schedComp.StabilityRandom, schedComp.StabilityBase + schedComp.StabilityRandom);
			comp.DecayFreq = TimeSpan.FromSeconds(schedComp.StabilityDecayFreq);
            comp.DecayRate = schedComp.StabilityDecayRate;
			comp.Reactivity = schedComp.ReactivityBase;
			comp.Fragility = schedComp.ReactivityFragility;
			continue;
		}
    }

    // stapled together from code in AnomalySpawnRule and AnomalySystem.Generator
    // can you believe that I'm not very good at this?
    public void SpawnAnomaly(string anomType)
    {
        if (!TryGetRandomStation(out var chosenStation))
            return;

        if (!TryComp<StationDataComponent>(chosenStation, out var stationData))
            return;

        var grid = _stationSystem.GetLargestGrid(stationData);

        if (grid is null)
            return;

        if (!TryComp<MapGridComponent>(grid.Value, out var gridComp))
            return;

        var xform = Transform(grid.Value);

        var targetCoords = xform.Coordinates;
        var gridBounds = gridComp.LocalAABB.Scale(_configuration.GetCVar(CCVars.AnomalyGenerationGridBoundsScale));

        for (var i = 0; i < 25; i++)
        {
            var randomX = _random.Next((int) gridBounds.Left, (int) gridBounds.Right);
            var randomY = _random.Next((int) gridBounds.Bottom, (int)gridBounds.Top);

            var tile = new Vector2i(randomX, randomY);

            // no air-blocked areas.
            if (_atmosphere.IsTileSpace(grid.Value, xform.MapUid, tile) ||
                _atmosphere.IsTileAirBlocked(grid.Value, tile, mapGridComp: gridComp))
            {
                continue;
            }

            // don't spawn inside of solid objects
            var physQuery = GetEntityQuery<PhysicsComponent>();
            var valid = true;

            // TODO: This should be using static lookup.
            foreach (var ent in _mapSystem.GetAnchoredEntities(grid.Value, gridComp, tile))
            {
                if (!physQuery.TryGetComponent(ent, out var body))
                    continue;
                if (body.BodyType != BodyType.Static ||
                    !body.Hard ||
                    (body.CollisionLayer & (int) CollisionGroup.Impassable) == 0)
                    continue;

                valid = false;
                break;
            }
            if (!valid)
                continue;

            var pos = _mapSystem.GridTileToLocal(grid.Value, gridComp, tile);

            targetCoords = pos;
            break;
        }

        Spawn(anomType, targetCoords);
    }
}
