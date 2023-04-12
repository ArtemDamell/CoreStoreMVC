using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace CoreStoreMVC.Extensions
{
    public static class SessionExtension
    {
        /// <summary>
        /// Sets the specified key and value in the session.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        /// <summary>
        /// Retrieves the value of the specified key from the session.
        /// </summary>
        /// <typeparam name="T">The type of the value to be retrieved.</typeparam>
        /// <param name="session">The session from which the value should be retrieved.</param>
        /// <param name="key">The key of the value to be retrieved.</param>
        /// <returns>The value of the specified key.</returns>
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }
    }
}
