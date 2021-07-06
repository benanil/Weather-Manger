// A Middle Games product

using AnilTools;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using AnilTools.Save;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MiddleGames.Misc
{
    [ExecuteInEditMode]
    [CanEditMultipleObjects]
    public class StarRendering : Singleton<StarRendering>
    {

        [Header("Properties"),Tooltip("it must be low polly")] // required
        public Mesh starMesh;

        // instanced star shader
        private Material starMaterial;
        public Material StarMaterial
        {
            get
            {
                if (!Directory.Exists(Application.dataPath + "/Resources"))
                    Directory.CreateDirectory(Application.dataPath + "/Resources");

                AssetDatabase.Refresh();

                if (!Directory.Exists(Application.dataPath + "/Resources/Shaders"))
                    Directory.CreateDirectory(Application.dataPath + "/Resources/Shaders");

                AssetDatabase.Refresh();
                    
                if (!File.Exists(Application.dataPath + "Resources/Shaders/SkyShader.mat"))
                {
                    starMaterial = new Material(Shader.Find("Instanced/low star"));
                    starMaterial.enableInstancing = true;
                    AssetDatabase.CreateAsset(starMaterial, "Assets/Resources/Shaders/SkyShader.mat");
                }

                AssetDatabase.Refresh();

                return starMaterial;
            }
        }

        [Header("Customization", order = 1)]
        [ColorUsage(true,true)]
        public Color starColor = new Color(1, .35f, 1, 1);
        [Min(0)]
        [SerializeField] private Vector2 perlinHeights = new Vector2(.3f,.5f);
        [SerializeField] private Vector2 perlinScales = new Vector2(10,40);
        [SerializeField] private Vector2 perlinOffset = new Vector2(100,200);
        [SerializeField] private int M = 128, N = 128;
        [SerializeField] private Vector2 starSize = new Vector2(80, 100);

        [Header("Optimization"), Range(10, 1023)]
        public int starCount = 1023;

        public bool canUpdate;
        public bool CanUpdate
        {
            get => canUpdate;
            set {
                canUpdate = value;
                OnValidate();
            }
        }

        [SerializeField] private float maxRadius = 6000;// kubbenin en alt kısmındaki yarıçap

        private List<List<Matrix4x4>> matricies;
        private MaterialPropertyBlock materialProperty;

#if UNITY_EDITOR
        private Vector3 lastPos;
        private Vector3 lastScale;
        private Quaternion oldRot;
#endif

        [ContextMenu("reset")]
        private void Reset()
        {
            transform.localScale = new Vector3(50, 30, 65);
        }

        public void OnValidate()
        {
            if (!CanUpdate) return;

            materialProperty = new MaterialPropertyBlock();

            int currentMatrixIndex = 0;

            matricies = new List<List<Matrix4x4>>
            {
                new List<Matrix4x4>()
            };

            for (int m = 0; m < M; m++)
                for (int n = 0; n < N; n++)
                {
                    var y = Mathf.Sin(Mathf.PI * m / M) * Mathf.Sin(2 * Mathf.PI * n / N);
                    var x = Mathf.Sin(Mathf.PI * m / N) * Mathf.Cos(2 * Mathf.PI * n / N);

                    var noise  = Mathf.PerlinNoise(x * perlinScales.x + perlinOffset.x, y * perlinScales.x + perlinOffset.y);
                    var noise1 = Mathf.PerlinNoise(x * perlinScales.y + perlinOffset.x, y * perlinScales.y + perlinOffset.y); 

                    if (y < 0 || noise > perlinHeights.x || noise1 > perlinHeights.y) continue;

                    var pos = new Vector3(x, y,Mathf.Cos(Mathf.PI * m / M)) * maxRadius;

                    if (matricies[currentMatrixIndex].Count == 1023){
                        currentMatrixIndex++;
                        matricies.Add(new List<Matrix4x4>());
                    }

                    matricies[currentMatrixIndex].Add(Matrix4x4.TRS(transform.position + pos, Quaternion.identity, starSize));
                }

            materialProperty.SetColor("_Color", starColor);

            StarMaterial.EnableKeyword("_EMISSION");
        }

        private void LateUpdate()
        {
            if (!CanUpdate || !starMesh) return;

#if UNITY_EDITOR
            if (lastPos != transform.position || oldRot != transform.rotation || transform.localScale != lastScale) OnValidate();

            lastPos = transform.position;
            oldRot = transform.rotation;
            lastScale = transform.localScale;
#endif
            for (int i = 0; i < matricies.Count; i++){
                Graphics.DrawMeshInstanced(starMesh, 0, StarMaterial, matricies[i], materialProperty);
            }
        }

    }
}