namespace Serialization.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    using Newtonsoft.Json;

    public class JsonSerializer
    {
        public string jsonData;


        public object Deserialize(string data)
        {

            //var obj = JsonConvert.DeserializeObject<ScoreSelector>(data, new JsonSerializerSettings
            //{
            //    TypeNameHandling = TypeNameHandling.Auto,
            //    NullValueHandling = NullValueHandling.Ignore,
            //    PreserveReferencesHandling = PreserveReferencesHandling.All
            //});
            throw new NotImplementedException();
        }

        public string Serialize(object item, bool prettyPrint)
        {
            //if(item != null)
            //{
            //   JsonConvert.PopulateObject(jsonData, item, new JsonSerializerSettings
            //   {
            //       TypeNameHandling = TypeNameHandling.Auto
            //   });
            //}
            //else
            //{
            //   item = JsonConvert.DeserializeObject<ScoreSelector>(jsonData, new JsonSerializerSettings
            //   {
            //       TypeNameHandling = TypeNameHandling.Auto,
            //       NullValueHandling = NullValueHandling.Ignore
            //   });
            //}
            throw new NotImplementedException();

        }



    }


}
