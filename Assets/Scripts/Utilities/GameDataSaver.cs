using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


    public static class GameDataSaver
    {

        public static void Save<T>(T objectToSave, string prefix = "", string suffix = "")
        {
            string fileName = string.Format("{0}.dat", typeof(T).ToString());
            
            if(!string.IsNullOrEmpty(prefix))
            {
                fileName = prefix + fileName;
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                fileName = fileName + suffix;
            }
            
            string fullPath = Path.Combine(Application.persistentDataPath, fileName);

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            FileStream fileStream = new FileStream(fullPath, FileMode.Create);
            binaryFormatter.Serialize(fileStream, objectToSave);
        }

        public static T Load<T>(string prefix = "", string suffix = "")
        {
            T toReturn = default(T);

            string fileName = string.Format("{0}.dat", typeof(T).ToString());

            if (!string.IsNullOrEmpty(prefix))
            {
                fileName = prefix + fileName;
            }

            if (!string.IsNullOrEmpty(suffix))
            {
                fileName = fileName + suffix;
            }

            string fullPath = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(fullPath))
            {
                BinaryFormatter formatter = new BinaryFormatter();

                FileStream stream = new FileStream(fullPath, FileMode.Open);

                toReturn = (T)formatter.Deserialize(stream);

                stream.Close();

            }

            return toReturn;
        }
    }
}
