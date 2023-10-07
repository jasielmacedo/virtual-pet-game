using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using LitJson;

namespace Core.save
{
    public class MemoryCard
    {

        static string pathSaveData;

        static bool registerExporter = false;

        static void setPathLocation()
        {


#if UNITY_ANDROID
                MemoryCard.pathSaveData = Application.persistentDataPath;
                MemoryCard.pathSaveData = pathSaveData.Substring( 0, pathSaveData.LastIndexOf( '/' ) );
                MemoryCard.pathSaveData = pathSaveData + "/";
#elif UNITY_IPHONE
                MemoryCard.pathSaveData = Application.persistentDataPath.Substring(0, Application.persistentDataPath.Length - 5);
                MemoryCard.pathSaveData = pathSaveData.Substring( 0, pathSaveData.LastIndexOf( '/' ) );
                MemoryCard.pathSaveData = pathSaveData + "/Documents/";
#else
            MemoryCard.pathSaveData = UnityEngine.Application.dataPath;
            if (MemoryCard.pathSaveData[MemoryCard.pathSaveData.Length - 1] != '/')
                MemoryCard.pathSaveData += "/";

            MemoryCard.pathSaveData += "../save/";
            MemoryCard.pathSaveData = Path.GetFullPath(MemoryCard.pathSaveData);
#endif
        }

        static void checkPath()
        {
            if (string.IsNullOrEmpty(MemoryCard.pathSaveData))
                MemoryCard.setPathLocation();
        }

        public static string convertToXml<T>(T objContent) where T : class
        {
            if (objContent == null)
                return string.Empty;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringWriter str = new StringWriter();
                serializer.Serialize(str, objContent);

                return str.ToString();
            }
            catch
            {
                Debug.LogError("Error converting to XML");
            }

            return string.Empty;
        }

        public static T revertFromXml<T>(string strXml) where T : class
        {
            if (string.IsNullOrEmpty(strXml))
                return null;

            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(new StringReader(strXml)) as T;

            }
            catch
            {
                Debug.LogError("Error reverting from XML String");
            }

            return null;
        }

        public static string convertToJson<T>(T objContent) where T : class
        {
            if (!MemoryCard.registerExporter)
            {
                JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(System.Convert.ToDouble(obj)));
                JsonMapper.RegisterImporter<string, double>(input => System.Convert.ToDouble(input));
                JsonMapper.RegisterImporter<string, float>(input => System.Convert.ToSingle(input));
                JsonMapper.RegisterImporter<float, string>(input => input.ToString());
                JsonMapper.RegisterImporter<double, float>(input => System.Convert.ToSingle(input));
                MemoryCard.registerExporter = true;
            }

            if (objContent == null)
                return string.Empty;

            try
            {
                return JsonMapper.ToJson(objContent);
            }
            catch (System.Exception e)
            {
                Debug.Log("Error converting to Json" + e.Message);
            }

            return string.Empty;
        }

        public static T revertFromJson<T>(string strJson) where T : class
        {
            if (!MemoryCard.registerExporter)
            {
                JsonMapper.RegisterExporter<float>((obj, writer) => writer.Write(System.Convert.ToDouble(obj)));
                JsonMapper.RegisterImporter<string, float>(input => System.Convert.ToSingle(input));
                JsonMapper.RegisterImporter<string, double>(input => System.Convert.ToDouble(input));
                JsonMapper.RegisterImporter<float, string>(input => input.ToString());
                JsonMapper.RegisterImporter<double, float>(input => System.Convert.ToSingle(input));
                MemoryCard.registerExporter = true;
            }

            if (string.IsNullOrEmpty(strJson))
                return null;

            try
            {
                return JsonMapper.ToObject<T>(strJson);
            }
            catch
            {
                Debug.LogError("Error to revert from Json String");
            }

            return null;
        }

        public static string loadFile(string fileName)
        {
            MemoryCard.checkPath();

            try
            {
                string fileDirAndName = MemoryCard.pathSaveData + fileName;

                if (File.Exists(fileDirAndName))
                {
                    FileStream fs = File.OpenRead(fileDirAndName);
                    byte[] bytes = new byte[fs.Length];

                    fs.Read(bytes, 0, System.Convert.ToInt32(fs.Length));
                    fs.Close();


                    string saveReturn = GetString(bytes);
                    return Crypt.Decrypt(saveReturn); ;
                }
            }
            catch
            {
                Debug.Log("Error to load file");
            }

            return string.Empty;
        }

        public static bool saveFile(string fileName, string fileContent)
        {
            if (string.IsNullOrEmpty(fileContent))
                fileContent = "";

            MemoryCard.checkPath();

            try
            {
                if (!Directory.Exists(MemoryCard.pathSaveData))
                {
                    Directory.CreateDirectory(MemoryCard.pathSaveData);
                }

                string fileDirAndName = MemoryCard.pathSaveData + fileName;

                fileContent = Crypt.Encrypt(fileContent);

                byte[] byteData = GetBytes(fileContent);
                FileStream oFileStream = new FileStream(fileDirAndName, FileMode.Create);
                oFileStream.Write(byteData, 0, byteData.Length);
                oFileStream.Close();

                Debug.Log("Data saved in " + fileDirAndName);

                return true;
            }
            catch (UnityException ex)
            {
                Debug.LogError(ex.Message);
            }

            return false;
        }

        public static void removeFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            MemoryCard.checkPath();

            string todelete = MemoryCard.pathSaveData + fileName;

            try
            {
                if (File.Exists(todelete))
                {
                    File.Delete(todelete);
                }
            }
            catch (UnityException e)
            {
                Debug.Log("Error to remove File: " + e.Message);
            }
        }

        public static T loadXmlFromResources<T>(string fileNameAndDir) where T : class
        {
            if (string.IsNullOrEmpty(fileNameAndDir))
                return null;

            try
            {
                UnityEngine.Object load = Resources.Load(fileNameAndDir);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(new StringReader(load.ToString())) as T;

            }
            catch
            {
                Debug.Log("Error loading XMl from resources folder");
            }

            return null;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }



    }

}
