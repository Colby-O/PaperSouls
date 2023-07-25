using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.Helpers
{
    internal sealed class JSONHandler
    {
        private string _path = "";
        private string _filename = "";
        private readonly string _fileExtension = ".json";
        private bool _encryptData;

        private readonly string _encryptionKey = "Poggers";
        private SymmetricAlgorithm _key;

        public JSONHandler(string path, string filename, bool encryptData = true)
        {
            _path = path;
            _filename = filename;
            _encryptData = encryptData;

            if (_encryptData) InitializesEncryptor();
        }

        /// <summary>
        /// Create a MD5 hash from a string from <paramref name="input"/>.
        /// </summary>
        private static byte[] CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return hashBytes;
            }
        }

        /// <summary>
        /// Generate the encrpytion key. This will need to be modified in the future
        /// for better security. i.e. Key should not be equal IV. 
        /// </summary>
        private void InitializesEncryptor()
        {
            _key = new DESCryptoServiceProvider();
            var fullHash = CreateMD5(_encryptionKey);
            var keyBytes = new byte[8];
            Array.Copy(fullHash, keyBytes, 8);
            _key.Key = keyBytes;
            _key.IV = keyBytes;
        }

        /// <summary>
        /// Load the game data given a <paramref name="profileID"/>.
        /// </summary>
        public GameData Load(string profileID)
        {
            string path = Path.Combine(Application.dataPath, _path, profileID, _filename + _fileExtension);

            string data = null;

            if (File.Exists(path))
            {
                try
                {
                    using FileStream fileStream = new(path, FileMode.Open);
                    if (_encryptData)
                    {
                        using CryptoStream stream = new(
                            fileStream,
                            _key.CreateDecryptor(),
                            CryptoStreamMode.Read
                            );

                        using StreamReader reader = new(stream);
                        data = reader.ReadToEnd();
                    }
                    else
                    {
                        using StreamReader reader = new(fileStream);
                        data = reader.ReadToEnd();
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError($"An error occured loading the game data to file: " +
                                        $"{path} \n {e}");
                }
            }

            return JsonUtility.FromJson<GameData>(data);
        }

        /// <summary>
        /// Saves the game data to a profile with a given <paramref name="profileID"/>.
        /// </summary>
        public void Save(GameData data, string profileID)
        {
            string path = Path.Combine(Application.dataPath, _path, profileID, _filename + _fileExtension);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                string dataJSON = JsonUtility.ToJson(data, false);

                using FileStream fileStream = new(path, FileMode.Create);
                if (_encryptData)
                {
                    using CryptoStream stream = new(
                        fileStream,
                        _key.CreateEncryptor(),
                        CryptoStreamMode.Write
                        );
                    using StreamWriter writer = new(stream);
                    writer.Write(dataJSON);
                }
                else
                {
                    using StreamWriter writer = new(fileStream);
                    writer.Write(dataJSON);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured loading the game data to file: " +
                                    $"{path} \n {e}");
            }
        }

        /// <summary>
        /// Loads all profiles
        /// </summary>
        public Dictionary<string, GameData> LoadAndGetAllProfiles()
        {
            Dictionary<string, GameData> profileDictionary = new();

            var dirInfos = new DirectoryInfo(
                Path.Combine(Application.dataPath, _path))
                .EnumerateDirectories();

            foreach (DirectoryInfo dir in dirInfos)
            {
                var profileID = dir.Name;

                var path = Path.Combine(Application.dataPath, _path, profileID, _filename + _fileExtension);

                if (!File.Exists(path))
                {
                    continue;
                }

                var profileData = Load(profileID);
                if (profileData != null)
                {
                    profileDictionary.Add(profileID, profileData);
                }
                else
                {
                    Debug.LogError($"ERROR: Failed Loading ProfileID: {profileID}");
                }
            }

            return profileDictionary;
        }
    }
}
