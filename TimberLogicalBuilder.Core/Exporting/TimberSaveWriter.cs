using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TimberLogicalBuilder.Core.Exporting;

public static class TimberSaveWriter
{
  public static void WriteEntities(string sourceTimber, string outputTimber, JsonArray entities)
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

    root["Entities"] = entities;

    worldEntry.Delete();
    var newEntry = archive.CreateEntry("world.json");
    using (var stream = newEntry.Open())
    using (var writer = new StreamWriter(stream)) { writer.Write(root.ToJsonString(new JsonSerializerOptions { WriteIndented = true })); }
  }
}