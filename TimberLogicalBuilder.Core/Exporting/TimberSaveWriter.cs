using System.Data;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using TimberLogicalBuilder.Core.Structs;

namespace TimberLogicalBuilder.Core.Exporting;

public static class TimberSaveWriter
{
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

    if(existing != null && existing.GetType() == typeof(System.Text.Json.Nodes.JsonArray))
    {
      existingArray = (JsonArray) existing;
    }

    root["Entities"] = mergeEntities(existingArray, entities);

    worldEntry.Delete();
    var newEntry = archive.CreateEntry("world.json");
    using (var stream = newEntry.Open())
    using (var writer = new StreamWriter(stream)) { writer.Write(root.ToJsonString(new JsonSerializerOptions { WriteIndented = true })); }
  }

  // Replace entities by name, if they match
  // Preserve all other entities
  // Add all completely new entities
  private static JsonArray mergeEntities(JsonArray? existing, JsonArray incoming)
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
      if(hasLoc(ent))
      {
        entitiesByLocation[getLoc(ent)] = ent;
      }

      // and its name
      String? name = getName(ent);
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

      String? name = getName(ent);
      
      // if it's unnamed and doesn't collide with anything, add it.
      if (name == null)
      {
        if(!checkForLocationCollision(entitiesByLocation, ent))
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
      if(!checkForLocationCollision(entitiesByLocation, ent))
      {
        outgoing.Add(ent.DeepClone());
      }
    }

    return outgoing;
  }

  private static bool checkForLocationCollision(Dictionary<Vector3Int, JsonNode> map, JsonNode entity)
  {
    // Can't collide with something that doesn't have a location
    if(!hasLoc(entity))
    {
      return false;
    }

    Vector3Int loc = getLoc(entity);

    return map.ContainsKey(loc);
  }

  private static String? getName(JsonNode entity)
  {
    return entity["Components"]?["NamedEntity"]?["EntityName"]?.ToString();
  }

  private static String? getTemplate(JsonNode entity)
  {
    return entity["Template"]?.ToString();
  }

// For example: Beavers don't have a BlockObject, they have a Character.
  private static bool hasLoc(JsonNode entity)
  {
    JsonNode? location = entity["Components"]?["BlockObject"]?["Coordinates"];

    return (location != null);
  }
  private static Vector3Int getLoc(JsonNode entity)
  {
    JsonNode? location = entity["Components"]?["BlockObject"]?["Coordinates"];

    if (location == null)
    {
      throw new NoNullAllowedException();
    }

    int x = (int) (location["X"] ?? "-1");
    int y = (int) (location["Y"] ?? "-1");
    int z = (int) (location["Z"] ?? "-1");

    if(x == -1 || y == -1 || z == -1)
    {
      throw new NoNullAllowedException();
    }

    return new Vector3Int(x, y, z);
  }
}