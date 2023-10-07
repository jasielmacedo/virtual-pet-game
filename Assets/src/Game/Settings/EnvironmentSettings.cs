using UnityEngine;
using System.Collections.Generic;

namespace Game.Settings
{
    [CreateAssetMenu(fileName = "EnvironmentSettings", menuName = "Simulation/Settings", order = 999)]
    public class EnvironmentSettings : ScriptableObject
    {
        [Header("Open AI")]
        public string OpenAIApiKey = "";
        public string OpenAIModel = "gpt3.5-turbo";
        public float temperature = 1f;
        public int OpenAITimeout = 60000;
        public int OpenAIMaxAttempts = 5;

        [Header("Memory")]
        public string PineconeApiKey = "";
        public string PineconeTableName = "simulation-gpt";
        public int PineconeTimeout = 60000;
        public int PineconeIMaxAttempts = 1;

        static EnvironmentSettings cache = null;
        public static EnvironmentSettings data
        {
            get
            {
                // load if not loaded yet
                return cache ?? (cache = Resources.Load<EnvironmentSettings>("EnvironmentSettings"));
            }
        }
    }
}