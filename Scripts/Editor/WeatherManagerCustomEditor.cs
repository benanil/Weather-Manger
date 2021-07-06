using UnityEngine;
using UnityEditor;
using MiddleGames.Misc;

namespace Assets.Editor.Serializer
{
    [CustomEditor(typeof(WeatherManager))]
    public class WeatherManagerCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open In Editor")) {
                WeatherManagerCustomEditorWindow.Open((WeatherManager)target);
            }
            if (GUILayout.Button("Calculate SunPositions")) {
                ((WeatherManager)target).CalculateSunPositions();
            }
            base.OnInspectorGUI();
        }
    }
}
