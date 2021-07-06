

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace AnilTools.Save
{
    public static partial class JsonManager
    {
        public static string EnsurePath(string suredPath, string desiredPath)
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            string[] filePaths = desiredPath.Split('/');
            string currentFile = '/' + filePaths[0];
            int index = 0;

            while (!File.Exists(suredPath + currentFile))
            {
                Directory.CreateDirectory(suredPath + currentFile);
                index++;
                if (filePaths.Length == index || filePaths[index] == string.Empty) break;
                currentFile += '/' + filePaths[index];
            }

            return suredPath + desiredPath;
        }

        public static void Save<T>(T obj, string path , string name)
        {
            CheckPath(path);

            string konum = Path.Combine(SavesFolder + path + name);
            string saveText = JsonUtility.ToJson(obj, true);

            File.WriteAllText(konum, saveText);
        }

        public static T Load<T>(string path, string name)
        {
            T objectToLoad = default;
            
            CheckPath(path);
            string konum = Path.Combine(SavesFolder + path + name);

            if (File.Exists(konum))
            {
                string loadText = File.ReadAllText(konum);
                objectToLoad = JsonUtility.FromJson<T>(loadText);
            }
            
            return objectToLoad;
        }

        public static void SaveList<T>(string path, string name, List<T> obj)
        {
            if (!path.StartsWith(Application.dataPath))
                if (Directory.Exists(SavesFolder + path))
                    Directory.Delete(SavesFolder + path, true);

            CheckPath(path);

            for (int i = 0; i < obj.Count; i++)
            {
                string konum;

                if (path.StartsWith(Application.dataPath))
                    konum = Path.Combine(path + i.ToString() + name);
                else
                    konum = Path.Combine(SavesFolder + path + i.ToString() + name);
                
                string saveText = JsonUtility.ToJson(obj[i], true);

                File.WriteAllText(konum, saveText);
            }
        }

        public static IEnumerable<T> LoadList<T>(string path, string name, List<T> unityObjects = null)
        {
            CheckPath(path);

            DirectoryInfo directoryInfo = new DirectoryInfo(SavesFolder + path);

            int count = directoryInfo.GetFiles().Length;

            if (unityObjects != null)
                if (unityObjects.GetType() == typeof(List<T>))
                    unityObjects.Capacity = count;

            for (int i = 0; i < count; i++)
            {
                string konum;
                
                if(path.StartsWith(Application.dataPath))
                    konum = Path.Combine(path + i.ToString() + name);
                else
                    konum = Path.Combine(SavesFolder + path + i.ToString() + name); 

                if (File.Exists(konum))
                {
                    string loadText = File.ReadAllText(konum);
                    yield return JsonUtility.FromJson<T>(loadText);
                }
            }
        }
    }
}
