using Robust.Shared.Timing;
using Robust.Shared.Physics.Events;
using Content.Server._Funkystation.Science.Anomaly.AnomalyTypes;
using Content.Server.Anomaly.Components;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Interaction;

namespace Content.Server._Funkystation.Science.Anomaly.Systems;

public sealed class BaseAnomalySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly ExplosionSystem _explosion = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BaseAnomalyComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<BaseAnomalyComponent, StartCollideEvent>(OnStartCollide);
    }

    public override void Update(float frameTime)
    {
        var curTime = _gameTiming.CurTime;

        var query = EntityQueryEnumerator<BaseAnomalyComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.NextDecay > curTime)
                continue;

            //handle decay
            comp.Stability -= comp.DecayRate;
            if (comp.Stability <= 0)
                Destabilize(uid);

            comp.NextDecay += comp.DecayFreq;
        }
    }

    private void Destabilize(EntityUid anom)
    {
        _explosion.TriggerExplosive(anom);
        Del(anom);
    }

    private void OnMapInit(Entity<BaseAnomalyComponent> ent, ref MapInitEvent args)
    {
        Console.WriteLine("Experimental case confirmed");

        ent.Comp.NextDecay = _gameTiming.CurTime + ent.Comp.DecayFreq;
    }

    private void OnStartCollide(Entity<BaseAnomalyComponent> ent, ref StartCollideEvent args)
    {
        // I'm not rewriting the particle system from scratch I'm not that much of a sicko
        if (!TryComp<AnomalousParticleComponent>(args.OtherEntity, out var particle))
            return;

        if (args.OtherFixtureId != particle.FixtureId)
            return;

        if (particle.ParticleType == ent.Comp.StabilizingParticle)
        {
            ent.Comp.Stability += ent.Comp.Reactivity;
            ent.Comp.Stability = Math.Clamp(ent.Comp.Stability, 0, 1000);
        }
        else
        {
            ent.Comp.Stability -= (ent.Comp.Reactivity * ent.Comp.Fragility);
            ent.Comp.Stability = Math.Clamp(ent.Comp.Stability, 0, 1000);
        }
    }
}

// event called when an anomaly spawns, so the detector can read it
[ByRefEvent]
public record struct AnomalyGeneratedEvent();
