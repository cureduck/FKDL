using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

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

        protected void Save(string path)
        {
            var f = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(path, f);
        }


        protected static T GetOrCreate<T>(Func<T> create, string path) where T : SaveData
        {
            T so;
            if (File.Exists(path))
            {
                try
                {
                    so = Load<T>(path);
                }
                catch (Exception e)
                {
                    File.Delete(path);
                    so = create();
                    so.Save(path);
                    Debug.Log(e);
                }
            }
            else
            {
                so = create();
                so.Save(path);
            }

            return so;
        }
    }
}