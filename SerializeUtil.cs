using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Timeline
{
    public static class SerializeUtil
    {
        public static T FromJSON<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Main.HBSLog?.LogError($"SerializerHelper.FromJSON for class {typeof(T).Name} tossed exception");
                Main.HBSLog?.LogException(e);
                return default;
            }
        }

        public static T FromPath<T>(string path)
        {
            if (!File.Exists(path))
            {
                Main.HBSLog?.LogWarning($"Could not find file at: {path}");
                return default;
            }

            return FromJSON<T>(File.ReadAllText(path));
        }

        public static List<T> FromPaths<T>(IEnumerable<string> paths)
        {
            var list = new List<T>();

            foreach (var path in paths)
            {
                var resource = FromPath<T>(path);
                if (resource == null)
                {
                    Main.HBSLog?.LogError($"{typeof(T).Name} did not parse at {path}");
                    break;
                }

                Main.HBSLog?.Log($"Parsed {typeof(T).Name} at path {path}");
                list.Add(resource);
            }

            return list;
        }
    }
}
