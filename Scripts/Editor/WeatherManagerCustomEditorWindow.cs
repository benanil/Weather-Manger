using MiddleGames.Misc;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Assets.Editor.Serializer
{
    public class WeatherManagerCustomEditorWindow : ExtendedEditorWindow
    {
        public static string filePath
        {
            get
            {
                if (!Directory.Exists(Application.dataPath + "/Resources/Scriptables"))
                    Directory.CreateDirectory(Application.dataPath + "/Resources/Scriptables");

                AssetDatabase.Refresh();

                if (!Directory.Exists(Application.dataPath + "/Resources/Scriptables/Weather Configs"))
                    Directory.CreateDirectory(Application.dataPath + "/Resources/Scriptables/Weather Configs");
                
                AssetDatabase.Refresh();
                return "Assets/Resources/Scriptables/Weather Configs/";
            }
        }
        private static WeatherManagerCustomEditorWindow window;

        private static Action DrawPropertiesAction = DrawSettings;
        private static Action DrawSideBarAction = () => { };

        private static bool DrawSideBar;

        private static WeatherConfig currentWeaterConfig;
        
        private bool nameChanging;
        
        private static GUIStyle HeaderStyle;
        private Vector2 scrollWiewScale;

        string fileName = "Config";

        public static void Open(WeatherManager weatherManager)
        {
            EnsureSun();
            window = GetWindow<WeatherManagerCustomEditorWindow>("Weather Manager");
            window.serializedObject = new SerializedObject(weatherManager);
        }

        [MenuItem("Graph/WeatherManager")]
        public static void Open()
        {
            EnsureSun();
            WeatherManager weatherManager = WeatherManager.instance;
            window = GetWindow<WeatherManagerCustomEditorWindow>("Weather Manager");
            window.serializedObject = new SerializedObject(weatherManager);
        }

        public static void EnsureSun()
        { 
            if (RenderSettings.sun == null)
            {
                RenderSettings.sun = FindObjectsOfType<Light>().Where(x => x.type == LightType.Directional).FirstOrDefault();
                if (RenderSettings.sun == null) {
                    RenderSettings.sun = new GameObject("Directional Light").AddComponent<Light>();
                    RenderSettings.sun.type = LightType.Directional;
                }
            }
        }

        private void OnGUI()
        {
            HeaderStyle = new GUIStyle { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            HeaderStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));

            if (GUILayout.Button("edit configs"))
            {
                DrawSideBar = true;

                DrawPropertiesAction = () =>
                {
                    // currentWeaterConfig displayer
                    if (currentWeaterConfig)
                    {
                        var serializedObject = new SerializedObject(currentWeaterConfig);
                        var fields = typeof(WeatherConfig).GetFields();

                        serializedObject.Update();

                        EditorGUILayout.BeginHorizontal();

                        if (currentWeaterConfig.name == null || currentWeaterConfig.name == string.Empty && !nameChanging)
                             EditorGUILayout.LabelField("config name is not Defined",HeaderStyle);
                        else EditorGUILayout.LabelField(currentWeaterConfig.name, HeaderStyle);

                        if (GUILayout.Button("/",GUILayout.Width(20),GUILayout.Height(20))) {
                            nameChanging = !nameChanging;
                        }

                        if (nameChanging) currentWeaterConfig.name = EditorGUILayout.TextField(currentWeaterConfig.name);

                        EditorGUILayout.EndHorizontal();

                        scrollWiewScale = EditorGUILayout.BeginScrollView(scrollWiewScale);
                        EditorGUI.BeginChangeCheck();

                        for (int i = 0; i < fields.Length; i++){
                            var Property = serializedObject.FindProperty(fields[i].Name);
                            var attributes = Attribute.GetCustomAttributes(fields[i]);
                            bool hasNonserializedAttribute = false;

                            foreach (var attribute in attributes){
                                if (attribute is NonSerializedAttribute nonSerialized){
                                    hasNonserializedAttribute = true;
                                }
                            }
                            if (!hasNonserializedAttribute) EditorGUILayout.PropertyField(Property,true); 
                        }

                        if (EditorGUI.EndChangeCheck()){
                            OnValueChanged();
                        }
                     
                        serializedObject.ApplyModifiedProperties();
                        EditorGUILayout.BeginHorizontal();
                    
                        if (GUILayout.Button("Set Scene Values", GUILayout.Width(120)))
                            if (currentWeaterConfig)
                                currentWeaterConfig.SetImmediate();

                        if (GUILayout.Button("Get Scene Values", GUILayout.Width(120)))
                        {
                            Undo.RecordObject(currentWeaterConfig, "change whether config");

                            currentWeaterConfig.fog = RenderSettings.fog;
                            currentWeaterConfig.fogDensity = RenderSettings.fogDensity;
                            currentWeaterConfig.FogColor = RenderSettings.fogColor;
                            currentWeaterConfig.SunIntensity = RenderSettings.sun.intensity;
                            currentWeaterConfig.SunLightColor = RenderSettings.sun.color;
                            currentWeaterConfig.sunAngle = WeatherConfig.SkyboxMaterial.GetFloat("_SunAngle");
                            currentWeaterConfig.SunRotation = RenderSettings.sun.transform.localEulerAngles;
                            
                            currentWeaterConfig.skyboxProperties.GetValuesFromScene();
                        }

                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.EndHorizontal();
                    }
                    else EditorGUILayout.LabelField("Please chose at least one config");
                };

                DrawSideBarAction = () =>
                {
                    foreach (WeatherConfig config in WeatherManager.instance.weatherConfigs){
                        if (GUILayout.Button(config.name)){
                            currentWeaterConfig = config;
                            currentWeaterConfig.SetImmediate();
                        }
                    }

                    fileName = EditorGUILayout.TextField(fileName);
                    
                    if (GUILayout.Button("Create New"))
                    {
                        var config = CreateInstance<WeatherConfig>();

                        AssetDatabase.CreateAsset(config, filePath + fileName + ".asset");

                        WeatherManager.instance.weatherConfigs.Add(config);
                    }
                };
            }

            if (GUILayout.Button("Settings"))
            {
                DrawSideBar = true;
                DrawPropertiesAction = DrawSettings;
                DrawSideBarAction = () => 
                {
                    // settings side bar
                };
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            
            if (DrawSideBar)
            {
                EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
                DrawSideBarAction.Invoke();
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

            DrawPropertiesAction?.Invoke();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawSettings()
        { 
            EditorGUILayout.LabelField("                                            ");
            EditorGUILayout.LabelField("                                            ");
            EditorGUILayout.LabelField("                                            ");
            EditorGUILayout.LabelField("        you can save and load configs       ", HeaderStyle);
            EditorGUILayout.LabelField("                                            ");
            EditorGUILayout.LabelField("                                            ");
            EditorGUILayout.LabelField("                                            ");

            if (GUILayout.Button("Save To Json")) WeatherManager.instance.SaveToJson();
            if (GUILayout.Button("Load FromJson")) WeatherManager.instance.LoadFromJson();
        }

        private void OnValueChanged()
        {
            currentWeaterConfig.SetImmediate();

        }
    }
}
