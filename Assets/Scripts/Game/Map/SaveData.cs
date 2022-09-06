using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Game
{
    public abstract class SaveData
    {
        protected static T Load<T>(string path)
        {
            var f = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(f, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        [Button]
        protected void Save(string path)
        {
            var f = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            File.WriteAllText(path, f);
        }
    }
}