using System.Collections.Generic;

namespace Au
{
    /// <summary>
    /// i18n data
    /// </summary>
    public class I18nData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">data name</param>
        /// <param name="json">data json</param>
        public I18nData(string name, string json)
        {
            this.name = name;
            items = LitJson.JsonMapper.ToObject<Dictionary<string, string>>(json);
        }

        public readonly string name;

        private readonly Dictionary<string, string> items;

        private readonly Dictionary<string, I18nData> additions = new Dictionary<string, I18nData>();

        /// <summary>
        /// Add addition i18n data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="json"></param>
        public void AddAddition(string name, string json)
        {
            var data = new I18nData(name, json);
            additions.Add(name, data);
        }

        /// <summary>
        /// Remove addition data
        /// </summary>
        /// <param name="name"></param>
        public void RemoveAddition(string name)
        {
            additions.Remove(name);
        }

        /// <summary>
        /// Try to translate and languange id
        /// </summary>
        /// <param name="id">language id</param>
        /// <param name="ret">return string</param>
        /// <returns>succ return true</returns>
        public bool TryTranslate(string id, out string ret)
        {
            if (items.TryGetValue(id, out ret))
            {
                return true;
            }

            foreach (var addition in additions.Values)
            {
                if (addition.TryTranslate(id, out ret))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Translate an id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>translated string or id if failed</returns>
        public string Translate(string id)
        {
            if (TryTranslate(id, out string ret))
            {
                return ret;
            }
            return id;
        }
    }
}
