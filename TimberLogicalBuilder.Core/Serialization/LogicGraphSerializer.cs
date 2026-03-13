using System.Drawing;
using System.Text.Json.Nodes;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Serialization;

public static class LogicGraphSerializer
{
  public static SerializerSettings settings{get;} = new SerializerSettings();

  public static JsonArray Serialize(LogicGraph graph)
  {
    var array = new JsonArray();
    foreach (var node in graph.Nodes)
      array.Add(SerializeNode(node));
    return array;
  }
  
  private static JsonObject SerializeNode(LogicNode node)
  {
    if (node.IsEmpty) return SerializePlatform(node);
    if (node.RelayMode != null) return SerializeRelay(node);
    if (node.MemoryMode != null) return SerializeMemory(node);
    if (node.TimerMode != null) return SerializeTimer(node);
    if (node.CustomColor.HasValue) return SerializeIndicator(node);
    return SerializeLever(node);
  }
  
  private static JsonObject LoadTemplate(string template)
  {
    return JsonNode.Parse(template)!.AsObject();
  }
  
  private static void ApplyCommon(JsonObject json, LogicNode node)
  {
    json["Id"] = node.Id.ToString();
    var components = json["Components"]!.AsObject();
    // NamedEntity (not present on platforms)
    if (components.TryGetPropertyValue("NamedEntity", out var namedEntityNode)
        && namedEntityNode is JsonObject namedEntity)
    {
      namedEntity["EntityName"] = node.Name;
    }
    var coords = components["BlockObject"]!["Coordinates"]!;
    coords["X"] = node.Position.X;
    coords["Y"] = node.Position.Y;
    coords["Z"] = node.Position.Z;
  }
  
  private static void ApplyFaction(JsonObject json)
  {
    var template = json["Template"]!.GetValue<string>();
    json["Template"] = template.Replace("{{FACTION}}", settings.faction.ToString());
  }
  
  private static JsonObject SerializeInterval(TimerInterval interval)
  {
    return new JsonObject { [interval.Unit.ToString()] = interval.Interval };
  }
  
  private static string ColorToHex(Color color)
  {
    return $"{color.R:X2}{color.G:X2}{color.B:X2}";
  }
  
  private static JsonObject SerializeLever(LogicNode lever)
  {
    var json = LoadTemplate(Templates.LeverTemplate);
    ApplyCommon(json, lever);
    ApplyFaction(json);
    var leverNode = json["Components"]!["Lever"]!.AsObject();
    if (lever.IsSpringReturn)
      leverNode["IsSpringReturn"] = true;
    if (lever.IsPinned)
      leverNode["IsPinned"] = true;
    return json;
  }
  
  private static JsonObject SerializeRelay(LogicNode relay)
  {
    if (relay.InputA is null) throw new ArgumentNullException(nameof(relay.InputA));
    var json = LoadTemplate(Templates.RelayTemplate);
    ApplyCommon(json, relay);
    ApplyFaction(json);
    var relayNode = json["Components"]!["Relay"]!;
    relayNode["Mode"] = relay.RelayMode.ToString();
    relayNode["InputA"] = relay.InputA.Id.ToString();
    if (relay.InputB != null)
      relayNode["InputB"] = relay.InputB.Id.ToString();
    return json;
  }
  
  private static JsonObject SerializeMemory(LogicNode memory)
  {
    if (memory.InputA is null) throw new ArgumentNullException(nameof(memory.InputA));
    var json = LoadTemplate(Templates.MemoryTemplate);
    ApplyCommon(json, memory);
    ApplyFaction(json);
    var mem = json["Components"]!["Memory"]!;
    mem["Mode"] = memory.MemoryMode.ToString();
    mem["InputA"] = memory.InputA.Id.ToString();
    if (memory.InputB != null)
      mem["InputB"] = memory.InputB.Id.ToString();
    if (memory.ResetInput != null)
      mem["ResetInput"] = memory.ResetInput.Id.ToString();
    return json;
  }
  
  private static JsonObject SerializeTimer(LogicNode timer)
  {
    if (timer.InputA is null) throw new ArgumentNullException(nameof(timer.InputA));
    var json = LoadTemplate(Templates.TimerTemplate);
    ApplyCommon(json, timer);
    ApplyFaction(json);
    var t = json["Components"]!["Timer"]!;
    t["Mode"] = timer.TimerMode.ToString();
    t["Input"] = timer.InputA.Id.ToString();
    t["TimerIntervalA"] = SerializeInterval(timer.IntervalA!.Value);
    if (timer.IntervalB != null)
      t["TimerIntervalB"] = SerializeInterval(timer.IntervalB.Value);
    if (timer.ResetInput != null)
      t["ResetInput"] = timer.ResetInput.Id.ToString();
    return json;
  }
  
  private static JsonObject SerializeIndicator(LogicNode indicator)
  {
    if (indicator.InputA is null) throw new ArgumentNullException(nameof(indicator.InputA));
    var json = LoadTemplate(Templates.IndicatorTemplate);
    ApplyCommon(json, indicator);
    ApplyFaction(json);
    json["Components"]!["Automatable"]!["Input"] =
      indicator.InputA.Id.ToString();
    var indicatorNode = json["Components"]!["Indicator"]!.AsObject();
    if (indicator.IsPinned)
      indicatorNode["PinnedMode"] = "Always";
    // ReSharper disable once InvertIf
    if (indicator.CustomColor.HasValue)
    {
      var colorObj = json["Components"]!["CustomizableIlluminator"]!["CustomColor"]!.AsObject();
      var hex = ColorToHex(indicator.CustomColor.Value);
      colorObj["r"] = indicator.CustomColor.Value.R / 255f;
      colorObj["g"] = indicator.CustomColor.Value.G / 255f;
      colorObj["b"] = indicator.CustomColor.Value.B / 255f;
      colorObj["a"] = indicator.CustomColor.Value.A / 255f;
    }
    return json;
  }
  
  private static JsonObject SerializePlatform(LogicNode node, bool single = false)
  {
    var json = LoadTemplate(single
      ? Templates.PlatformTemplate
      : Templates.DoublePlatformTemplate);
    ApplyCommon(json, node);
    ApplyFaction(json);
    // Platforms must have their own unique ID
    json["Id"] = Guid.NewGuid().ToString();
    return json;
  }
  
}