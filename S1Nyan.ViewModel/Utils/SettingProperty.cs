using System;
using System.IO.IsolatedStorage;

namespace S1Nyan.Utils
{
    public class SettingProperty<T>
        where T : IEquatable<T>
    {
        private static readonly IsolatedStorageSettings Settings = IsolatedStorageSettings.ApplicationSettings;

        public SettingProperty(string key, T defaultValue = default(T))
        {
            _key = key;
            _defaultValue = defaultValue;
        }

        private readonly string _key;
        private readonly T _defaultValue;
        private T _value;
        private bool valueHasInited;
        public T Value
        {
            get
            {
                return valueHasInited ? _value : (_value = GetValueOrDefault());
            }
            set
            {
                if (Value.Equals(value)) return;

                _value = value;
                AddOrUpdateValue(value);
                Save();
            }
        }

        private T GetValueOrDefault()
        {
            return Settings.Contains(_key) ? (T) Settings[_key] : _defaultValue;
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        public bool AddOrUpdateValue(T value)
        {
            bool valueChanged = false;

            // If the key exists
            if (Settings.Contains(_key))
            {
                // If the value has changed
                if (!((T)Settings[_key]).Equals(value))
                {
                    // Store the new value
                    Settings[_key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                Settings.Add(_key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        public static void Save()
        {
            Settings.Save();
        }

    }
}