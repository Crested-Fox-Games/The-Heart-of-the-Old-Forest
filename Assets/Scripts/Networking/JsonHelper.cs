using UnityEngine;

public static class JsonHelper
{
    /// <summary>
    /// This is a generic method, the T is a placeholder for the type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T[] FromJson<T>(string json)
    {
        //This wraps the json so that unity can deserialize it
        string newJson = "{ \"lobbies\":" + json + "}";

        //This deserializes the json into C#
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);

        //This returns the array
        return wrapper.lobbies;
    }

    private class Wrapper<T>
    {
        public T[] lobbies;
    }
}

