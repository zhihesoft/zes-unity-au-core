using System.Collections.Generic;

namespace Au
{
    /// <summary>
    /// i18n data
    /// </summary>
    public class I18nData
    {
        public I18nData(string name, string json)
        {
            this.name = name;
            items = LitJson.JsonMapper.ToObject<Dictionary<string, string>>(json);
        }

        public readonly string name;

        private readonly Dictionary<string, string> items;

        private readonly Dictionary<string, I18nData> additions = new Dictionary<string, I18nData>();

        public void AddAddition(string name, string json)
        {
            var data = new I18nData(name, json);
            additions.Add(name, data);
        }

        public void RemoveAddition(string name)
        {
            additions.Remove(name);
        }

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

        public string Translate(string id)
        {
            if (TryTranslate(id, out string ret))
            {
                return ret;
            }
            return string.Empty;
        }
    }
}
