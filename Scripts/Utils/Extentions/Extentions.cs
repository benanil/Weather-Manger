using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UrFairy;

namespace AnilTools
{
    public static class Extentions
    {
        // Navmesh Extentions
        public static void SetNav(this NavMeshAgent nav, bool value)
        {
            nav.isStopped = !value;
            nav.updateRotation = value;
        }

        // canvasGroup Extentions
        public static void SetVisible(this CanvasGroup canvasGroup , bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }

        public static void Set(this ref float value, float target)
        {
            value = target;    
        }

        public static float Diffrance(this ref float value, float target)
        {
            return Mathf.Sqrt(Mathf.Pow(value - target,2));
        }

        // Transform Extentsions
        public static void ZeroTransform(this Transform transform)
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }

        // Vector Extentions

        /// <summary>
        /// <b><para>it will only change z axis</para> </b>
        /// it is better for Memory allocation
        /// </summary>
        public static Vector3 OnlyY(this Vector3 pos)
        {
            pos.z = 0;
            pos.x = 0;
            return pos;
        }

        public static Vector3 OnlyY(this Vector3 pos, float value)
        {
            pos.z = 0;
            pos.x = 0;
            pos.y = value;
            return pos;
        }

        /// <summary>
        /// <b><para>it will set x and z axis</para> </b>
        /// it is better for Memory allocation
        /// </summary>
        public static Vector3 WithoutY(this Vector3 pos,float x , float z)
        {
            pos.z = z;
            pos.x = x;
            return pos;
        }

        /// <summary>
        /// <b><para>it will only change z axis</para></b>
        /// it is better for Memory allocation
        /// </summary>
        public static Vector3 OnlyZ(this Vector3 pos, float value)
        {
            pos.x = 0;
            pos.y = 0;
            pos.z = value;
            return pos;
        }

        // Quaternion Extentions
        public static Quaternion OnlyYRot(this Transform transform, Quaternion quaternion)
        {
            transform.eulerAngles = transform.eulerAngles.Y(quaternion.eulerAngles.y); 
            return transform.rotation;
        }

        public static Quaternion OnlyYRot(this Transform transform, float angle)
        {
            transform.eulerAngles = transform.eulerAngles.Y(angle);
            return transform.rotation;
        }

        public static Quaternion OnlyYRot(this Quaternion transform)
        {
            transform.eulerAngles = transform.eulerAngles.X(0);
            transform.eulerAngles = transform.eulerAngles.Z(0);
            return transform;
        }

        public static Quaternion rotate(this Quaternion transform,Vector3 rotation)
        {
            transform.eulerAngles += rotation;
            return transform;
        }

        public static Quaternion rotateX(this Quaternion transform, float rotation)
        {
            transform.eulerAngles += Vector3.right * rotation;
            return transform;
        }
        
        public static Quaternion rotateY(this Quaternion transform, float rotation)
        {
            transform.eulerAngles += Vector3.up * rotation;
            return transform;
        }

        public static Quaternion rotateZ(this Quaternion transform, float rotation)
        {
            transform.eulerAngles += Vector3.forward * rotation;
            return transform;
        }

        // List Extentions
        public static void ForEachDoBreak<T>(this List<T> list, Action endAction, Func<bool> EndCondition)
        {
            for (byte i = 0; i < list.Count; i++){
                if (EndCondition.Invoke()){
                    endAction.Invoke();
                    break;
                }
            }
        }

        public static void ForEachBreak<T>(this List<T> list, Func<T,bool> endAction)
        {
            for (byte i = 0; i < list.Count; i++) {
                if (endAction.Invoke(list[i])){
                    break;
                }
            }
        }

        public static void ForEachWithIndex<T>(this List<T> list, Action<int> endAction)
        {
            for (short i = 0; i < list.Count; i++)
                endAction.Invoke(i);
        }

        // String Extention
        public static bool Compare(this string word, string comparedWord , byte errorCount = 1)
        {
            byte equalCount = 0;
            for (byte i = 0; i < comparedWord.Length; i++){
                if (word[i].Equals(comparedWord[i])){
                    equalCount++;
                }
            }
            return equalCount >= word.Length -errorCount;
        }

        public static bool Equal(this string word, string comparedWord)
        {
            byte equalCount = 0;
            for (byte i = 0; i < comparedWord.Length; i++){
                if (word[i].Equals(comparedWord[i])){
                    equalCount++;
                }
            }
            return equalCount >= word.Length -1;
        }

        // animator extentions
        public static void SetFloatLerp(this Animator animator, int hashset, float value, float speed)
        {
            animator.SetFloat(hashset, Mathf.Lerp(animator.GetFloat(hashset), value , Time.deltaTime * speed));
        }

        public static float Difrance(this Animator animator, int hashset, float value)
        {
            return animator.GetFloat(hashset).Difrance(value);
        }

        // Particle Extentions
        public static void ChangeColor(this ParticleSystem.MainModule main, Color color)
        {
            main.startColor = color;
        }

        // Array extention
        // List || array Extentions
        
        // example:
        //  this.FindInside((x) =>
        //   {
        //       return x.width + height; // it could be any object
        //   });
        public static exit FindInside<from,exit>(this from main,Func<from,exit> result)
        {
            return result.Invoke(main);
        }

        // example:
        //  this.FindArrayInside((x) =>
        //   {
        //       return x.boxes;
        //   });
        public static exit[] FindArrayInside<from, exit>(this from main, Func<from, exit[]> result)
        {
            return result.Invoke(main);
        }

        // example:
        //  FindObjectsOfType<Pickable>().FindListInside((x) =>x.GetComponent<Rigidbody>());
        public static List<exit> FindListInside<from, exit>(this from[] main, Func<from, exit> result)
        {
            var resultArray = new List<exit>();

            for (int i = 0; i < main.Length; i++)
            {
                resultArray.Add(result.Invoke(main[i]));
            }

            return resultArray;
        }

        // fromun içindeki objeleri bulup çıkarır
        // example:
        //  FindObjectsOfType<Pickable>().GetComponents<Rigidbody>().ToList()
        public static List<exit> GetComponents <from,exit>(this List<from> list) where from : Component
        {
            var components = new List<exit>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].TryGetComponent<exit>(out exit result))
                {
                    components.Add(result);
                }
            }

            return components;
        }
        public static void Foreach<T>(this T[] array,Action<T> action)
        {
            foreach (var item in array) action.Invoke(item);
        }

        public static T GetRandom<T>(this T[] array)
        {
            if (array.Length == 0) return default;
            return array[RandomReal.Range(0 , array.Length)];
        }

        public static T GetRandom<T>(this List<T> array)
        {
            if (array.Count == 0) return default;
            return array[RandomReal.Range(0, array.Count)];
        }

        // layermask extention
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        public static bool Contains(this int mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        // Color Extentions
        public static float Difrance(this Color a, Color b)
        {
            return Mathf.Pow(a.r - b.r, 2) + Mathf.Pow(a.g - b.g, 2) + Mathf.Pow(a.b - b.b, 2) + Mathf.Pow(a.a - b.a, 2) / 4;
        }

        public static float DifranceOpposite(this Color a, Color b)
        {
            return (a.r - b.r) + (a.g - b.g) + (a.b - b.b);
        }

        public static Color Opposite(this Color a)
        {
            Color.RGBToHSV(a, out float H, out float S, out float V);
            float negativeH = (H + 0.5f) % 1f;
            return Color.HSVToRGB(negativeH, S, V);
        }

        public static Color32 Opposite(this Color32 a)
        {
            Color.RGBToHSV(a, out float H, out float S, out float V);
            float negativeH = (H + 0.5f) % 1f;
            return Color.HSVToRGB(negativeH, S, V);
        }

        public static Color32[] Reverse(this Texture2D texture2D)
        {
            var oldPixels = texture2D.GetPixels32();
            
            for (int i = 0; i < oldPixels.Length; i++)
            {
                oldPixels[i] = oldPixels[i].Opposite();
            }

            return oldPixels;
        }

        // Distance
        public static float DistanceSqr(this Transform transform, Vector3 pos)
        {
            return (pos - transform.position).sqrMagnitude;
        }

        public static float Distance(this Transform transform, Vector3 pos)
        {
            return Vector3.Distance(transform.position, pos);
        }

        public static float DistanceSqr(this Transform a, Transform b)
        {
            return (a.position - b.position).sqrMagnitude;
        }

        public static float Distance(this Transform a, Transform b)
        {
            return Vector3.Distance(a.position, b.position);
        }

        public static float DistanceSqr(this ref Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude;
        }
        
        public static float Distance(this Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        public static float Distance2D(this Vector3 a, Vector3 b) // returns distance in xz coordinates
        {
            return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.z - b.z,2));
        }

    }
}
