using System.Data;
using System.IO.Compression;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using TimberLogicalBuilder.Core.Graph;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.TimberFile;

public static class TimberSaveFile
{
  public static Dictionary<string, LogicNode> LoadLogicNodes(string sourceTimber)
  {
    using var archive = ZipFile.Open(sourceTimber, ZipArchiveMode.Update);
    var worldEntry = archive.GetEntry("world.json");
    if (worldEntry == null)
      throw new InvalidOperationException("world.json not found in timber file.");
    JsonObject root;

    using (var stream = worldEntry.Open())
    using (var reader = new StreamReader(stream))
    {
      string json = reader.ReadToEnd();
      root = JsonNode.Parse(json)!.AsObject();
    }

    JsonNode existing = root["Entities"]!;
    JsonArray? existingArray = null;

    Dictionary<string, LogicNode> nodesByName = new Dictionary<string, LogicNode>();

    if(existing is JsonArray array)
    {
      EntityIngester ingest = new EntityIngester();
      existingArray = array;

      foreach(JsonNode? entity in existingArray)
      {
        if(entity != null && EntityIngester.isIngestible(entity))
        {
          LogicNode? node = ingest.Ingest(entity);

          if(node != null)
          {
            nodesByName[node.Name] = node;
          }
        }
      }
    }

    return nodesByName;
  } 
  // Add the new entities to the existing map file
  public static void IncludeEntities(string sourceTimber, string outputTimber, JsonArray entities)
  {
    File.Copy(sourceTimber, outputTimber, overwrite: true);
    using var archive = ZipFile.Open(outputTimber, ZipArchiveMode.Update);
    var worldEntry = archive.GetEntry("world.json");
    if (worldEntry == null)
      throw new InvalidOperationException("world.json not found in timber file.");
    JsonObject root;

    using (var stream = worldEntry.Open())
    using (var reader = new StreamReader(stream))
    {
      string json = reader.ReadToEnd();
      root = JsonNode.Parse(json)!.AsObject();
    }

    JsonNode existing = root["Entities"]!;
    JsonArray? existingArray = null;

    if(existing is JsonArray array)
    {
      existingArray = array;
    }

    root["Entities"] = MergeEntities(existingArray, entities);

    worldEntry.Delete();
    var newEntry = archive.CreateEntry("world.json");
    using (var stream = newEntry.Open())
    using (var writer = new StreamWriter(stream)) { writer.Write(root.ToJsonString(new JsonSerializerOptions { WriteIndented = true })); }
  }

  // Replace entities by name, if they match
  // Preserve all other entities
  // Add all completely new entities
  private static JsonArray MergeEntities(JsonArray? existing, JsonArray incoming)
  {
    if (existing == null)
    {
      return incoming;
    }

    JsonArray outgoing = new JsonArray();
    Dictionary<String, JsonNode> entitiesByName = new Dictionary<String, JsonNode>();
    Dictionary<Vector3Int, JsonNode> entitiesByLocation = new Dictionary<Vector3Int, JsonNode>();

    foreach (var ent in incoming)
    {
      if (ent == null)
      {
        continue;
      }

      // record its location
      if(TimberEntity.hasLoc(ent))
      {
        entitiesByLocation[TimberEntity.getLoc(ent)] = ent;
      }

      // and its name
      String? name = TimberEntity.getName(ent);
      if (name != null) {
        entitiesByName[name] = ent;
      }

      // and add it to the outgoing entities
      outgoing.Add(ent.DeepClone());
    }

    foreach (var ent in existing)
    {
      if (ent == null)
      {
        continue;
      }

      String? name = TimberEntity.getName(ent);
      
      // if it's unnamed and doesn't collide with anything, add it.
      if (name == null)
      {
        if(!CheckForLocationCollision(entitiesByLocation, ent))
        {
          outgoing.Add(ent.DeepClone());
        }
        continue;
      }

      // otherwise, if it has a name, make sure it isn't also in the new entities that we already added
      if (entitiesByName.ContainsKey(name))
      {
        continue;
      }

      // otherwise, it's named, but we aren't replacing it. Add it if it doesn't collide!
      if(!CheckForLocationCollision(entitiesByLocation, ent))
      {
        outgoing.Add(ent.DeepClone());
      }
    }

    return outgoing;
  }

    private static bool CheckForLocationCollision(Dictionary<Vector3Int, JsonNode> map, JsonNode entity)
  {
    // Can't collide with something that doesn't have a location
    if(!TimberEntity.hasLoc(entity))
    {
      return false;
    }

    Vector3Int loc = TimberEntity.getLoc(entity);

    return map.ContainsKey(loc);
  }
}