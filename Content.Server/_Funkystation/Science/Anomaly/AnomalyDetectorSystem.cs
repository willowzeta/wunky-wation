using Content.Server.Anomaly;
using Content.Shared.Anomaly.Components;
using Content.Server.Chat.Systems;
using Content.Shared.Radio;
using Content.Server.Radio.EntitySystems;
using Robust.Shared.Prototypes;
using System;

namespace Content.Server._Funkystation.Science.Anomaly;

public class AnomalyDetectorSystem : EntitySystem

{
    [Dependency] private readonly RadioSystem _radio = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly ChatSystem _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnomalyGeneratedEvent>(OnAnomalyGenerated);
    }

    private void OnAnomalyGenerated(ref AnomalyGeneratedEvent args)
    {

        var anomAnnounceName = "Anomaly Detector";

        var query = EntityQueryEnumerator<AnomalyDetectorComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            var anomClass = GetAnomalyClass();

            var anomNotif = $"A {anomClass}-class anomaly has been detected on station!";

            _chat.DispatchGlobalAnnouncement(anomNotif, anomAnnounceName, playSound: false, colorOverride: Color.Purple);
            break;
        }
    }

    private string GetAnomalyClass()
    {
        var classAnom = "Unknown";

        var query = EntityQueryEnumerator<AnomalyComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (TryComp<DetectedAnomalyComponent>(uid, out var anomComp))
            {
                break;
            }

            var severity = Convert.ToInt32(comp.Severity * 100);
            switch (severity / 15)
            {
                case 0:
                    classAnom = "Foxglove";
                    break;
                case 1:
                    classAnom = "Hawthorne";
                    break;
                case 2:
                    classAnom = "Rosemary";
                    break;
                case 3:
                    classAnom = "Wintergreen";
                    break;
                case 4:
                    classAnom = "Goldenseal";
                    break;
                case 5:
                    classAnom = "Belladonna";
                    break;
                case 6:
                    classAnom = "Wormwood";
                    break;
            }

            EnsureComp<DetectedAnomalyComponent>(uid);
        }

        return classAnom;
    }
}



