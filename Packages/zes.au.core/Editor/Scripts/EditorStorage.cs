using UnityEditor;

namespace Au
{
    public class EditorStorage
    {
        public static EditorStorage Get(string name)
        {
            return new EditorStorage(name);
        }

        public string name { get; private set; }
        public string prefsKey { get; private set; }

        private EditorStorage(string name)
        {
            this.name = name;
            this.prefsKey = $"prefs-{name}";
        }

        private string GetKey(string key)
        {
            return $"{prefsKey}-{key}";
        }


        public string GetString(string key, string defaultValue = "")
        {
            return EditorPrefs.GetString(GetKey(key), defaultValue);
        }

        public void SetString(string key, string value)
        {
            EditorPrefs.SetString(GetKey(key), value);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return EditorPrefs.GetInt(GetKey(key), defaultValue);
        }

        public void SetInt(string key, int value)
        {
            EditorPrefs.SetInt(GetKey(key), value);
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            int value = GetInt(key, defaultValue ? 1 : 0);
            return value != 0;
        }

        public void SetBool(string key, bool value)
        {
            SetInt(key, value ? 1 : 0);
        }
    }
}
