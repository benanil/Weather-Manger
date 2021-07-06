
using AnilTools;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using AnilTools.Save;
using System.IO;
using System;

namespace MiddleGames.Misc
{
    [Serializable]
    public struct SkyboxProperty{
        public Material material;
        public float MorningAmbient;
        public bool isWindow;
        [Tooltip("only for night")]
        public float glowMultipler;
        public bool PointLightAtNight;
        public bool SpecularAtMorning;
    }

    public class WeatherManager : Singleton<WeatherManager> {

        public static string SavesFolder
        {
            get
            {
                string directory = Application.persistentDataPath + "/Saves";
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                return directory + '/';
            }
        } 

        public List<WeatherConfig> weatherConfigs;
        [Tooltip("these are activated at night")] // its can be a street lamb or fire point light etc.
        public Light[] PointLights;
        [Tooltip("in the nights we need materials more darker")]
        public List<SkyboxProperty> MaterialProperties; // niye adi skybox prop alakası yok : material properties
        [ColorUsage(true,true)]
        public Color GlobalGlowColor;

        public bool LoadFromJsonAtStart;
        public short SecondsBetweenWeathers = 600; // 10dk
        
        public float SunSize  = 0.05f;

        [Tooltip("this is which weather config do yo want to start at begining of scene loading if it is 0 means early morning not even has sun")]
        public int StartConfingIndex = 4;

        private readonly Queue<WeatherConfig> weatherConfigsQueue = new Queue<WeatherConfig>();

        private WaitForSeconds waitTime;
        private WaitForSeconds halfWaitTime;

        private volatile WeatherConfig currentWeather;

        public string WeatherName => currentWeather.name;

        public bool Active;

        [ContextMenu("fix")]
        public void DisableSahaderKewords(){
            Shader.globalMaximumLOD = 200;
        }

        [ContextMenu("CalculateSunPositions")]
        public void CalculateSunPositions()
        {
            for (int i = 0; i < weatherConfigs.Count; i++)
            {
                weatherConfigs[i].sunAngle = 3.11f - (i * (Mathf.PI / weatherConfigs.Count));
            }
        }

        private void OnValidate()
        {
            WeatherConfig.SkyboxMaterial.SetFloat("_SunSize", SunSize);
        }

        private void Awake(){
            if (!Active) return;

            waitTime = new WaitForSeconds(SecondsBetweenWeathers);
            halfWaitTime = new WaitForSeconds(SecondsBetweenWeathers / 2);

            if (LoadFromJsonAtStart){
                LoadFromJson();
            }

            StartCoroutine(ResetConfigs(() =>
            { 
                StartCoroutine(UpdateWeather());
            }));
            OnValidate();
        }
        
        private IEnumerator UpdateWeather(){
            for (int i = 0; i < StartConfingIndex; i++)
            {
                currentWeather = weatherConfigsQueue.Dequeue();
            }
            currentWeather.SetImmediate();
            
            yield return halfWaitTime; // sabah ilk saat

            while (true) 
            {
                currentWeather = weatherConfigsQueue.Dequeue();
                currentWeather.SetLerp();

                if (weatherConfigsQueue.Count == 0) { // gece
                    StartCoroutine(ResetConfigs(null));
                    yield return halfWaitTime;
                }

                yield return waitTime;
            } 
        }

        private IEnumerator ResetConfigs(Action callback) {
            weatherConfigsQueue.Clear();
            for (int i = 0; i < weatherConfigs.Count; i++) {
                weatherConfigsQueue.Enqueue(weatherConfigs[i]);
                yield return new WaitForEndOfFrame();
            }

            callback?.Invoke();
        }

        public void SaveToJson() {
            JsonManager.SaveList("Weather/", "WeatherConfigs.json", weatherConfigs);
            Debug2.Log("succsesfully saved", Color.green);
        }

        public void LoadFromJson() {
            LoadList("Weather/", "WeatherConfigs.json");
        }

        public void LoadList(string path, string name)
        {
            CheckPath(path);

            for (int i = 0; i < weatherConfigs.Count; i++) {
                string konum = Path.Combine(SavesFolder + path + i.ToString() + name);

                if (File.Exists(konum)) {
                    string loadText = File.ReadAllText(konum);
                    JsonUtility.FromJsonOverwrite(loadText, weatherConfigs[i]);
                }
            }

            Debug2.Log("succsesfully Loaded", Color.green);
        }

        private static void CheckPath(string path)
        {
            if (path.StartsWith(Application.dataPath)) {
                if (!File.Exists(path)) Directory.CreateDirectory(path);
                return;
            }
            if (!File.Exists(SavesFolder))        Directory.CreateDirectory(SavesFolder);
            if (!File.Exists(SavesFolder + path)) Directory.CreateDirectory(SavesFolder + path);
        }

    }
}