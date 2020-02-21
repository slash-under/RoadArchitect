﻿#region "Imports"
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
#endregion


namespace RoadArchitect
{
    public static class TerrainHistoryUtility
    {
        //http://forum.unity3d.com/threads/32647-C-Sharp-Binary-Serialization
        //http://answers.unity3d.com/questions/363477/c-how-to-setup-a-binary-serialization.html

        // === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
        // Do not change this
        public sealed class VersionDeserializationBinder : SerializationBinder
        {
            public override System.Type BindToType(string assemblyName, string typeName)
            {
                if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
                {
                    System.Type typeToDeserialize = null;
                    assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
                    // The following line of code returns the type.
                    typeToDeserialize = System.Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
                    return typeToDeserialize;
                }
                return null;
            }
        }


        /// <summary> Saves the Terrain History to disk </summary>
        public static void SaveTerrainHistory(List<GSDTerrainHistoryMaker> _obj, GSDRoad _road)
        {
            string path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (string.IsNullOrEmpty(path) || path.Length < 2)
            {
                return;
            }
            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Binder = new VersionDeserializationBinder();
            bformatter.Serialize(stream, _obj);
            _road.TerrainHistoryByteSize = (stream.Length * 0.001f).ToString("n0") + " kb";
            stream.Close();
        }


        /// <summary> Deletes the Terrain History from disk </summary>
        public static void DeleteTerrainHistory(GSDRoad _road)
        {
            string path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }


        /// <summary> Loads the Terrain History from disk </summary>
        public static List<GSDTerrainHistoryMaker> LoadTerrainHistory(GSDRoad _road)
        {
            string path = CheckNonAssetDirTH() + GetRoadTHFilename(ref _road);
            if (string.IsNullOrEmpty(path) || path.Length < 2)
            {
                return null;
            }
            if (!File.Exists(path))
            {
                return null;
            }
            List<GSDTerrainHistoryMaker> result;
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Binder = new VersionDeserializationBinder();
            //			try{
            result = (List<GSDTerrainHistoryMaker>)bFormatter.Deserialize(stream) as List<GSDTerrainHistoryMaker>;
            //			}catch{
            //				result = null;	
            //			}
            stream.Close();
            return result;
        }


        /// <summary> Generates the Terrain History file name </summary>
        private static string GetRoadTHFilename(ref GSDRoad _road)
        {
            //string sceneName = System.IO.Path.GetFileName(UnityEditor.EditorApplication.currentScene).ToLower().Replace(".unity","");
            string sceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            sceneName = sceneName.Replace("/", "");
            sceneName = sceneName.Replace(".", "");
            string roadName = _road.GSDRS.transform.name.Replace("RoadArchitectSystem", "RAS") + "-" + _road.transform.name;
            return sceneName + "-" + roadName + "-TH.gsd";
        }


        /// <summary> Returns the path to the RoadArchitect folder where Terrain History is saved </summary>
        public static string GetDirBase()
        {
            return UnityEngine.Application.dataPath.Replace("/Assets", "/GSD/");
        }


        /// <summary> Returns the path where Terrain History is saved </summary>
        public static string GetTHDir()
        {
            string path = GetDirBase() + "TH/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        /// <summary> Checks if RoadArchitect folder exists </summary>
        public static string CheckRoadArchitectDirectory()
        {
            string path = GetDirBase();
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            if (System.IO.Directory.Exists(path))
            {
                return path + "/";
            }
            else
            {
                return "";
            }
        }


        public static string CheckNonAssetDirTH()
        {
            CheckRoadArchitectDirectory();
            string path = GetTHDir();
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            if (System.IO.Directory.Exists(path))
            {
                return path;
            }
            else
            {
                return "";
            }
        }


        public static string CheckNonAssetDirLibrary()
        {
            CheckRoadArchitectDirectory();
            string xPath = RootUtils.GetDirLibrary();
            if (!System.IO.Directory.Exists(xPath))
            {
                System.IO.Directory.CreateDirectory(xPath);
            }
            if (System.IO.Directory.Exists(xPath))
            {
                return xPath;
            }
            else
            {
                return "";
            }
        }


        public static void CheckNonAssetDirs()
        {
            CheckRoadArchitectDirectory();
            CheckNonAssetDirTH();
            CheckNonAssetDirLibrary();
        }
    }
}