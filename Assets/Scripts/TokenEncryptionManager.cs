using UnityEngine;
using System.Security.Cryptography;
using System.Text;

[System.Serializable]
public class ConfigData
{
    public string AES_KEY;
    public string AES_IV;
}
public class TokenEncryptionManager {

    private static TextAsset configFile = Resources.Load<TextAsset>("key_config");
    private static ConfigData keyConfig = JsonUtility.FromJson<ConfigData>(configFile.text);

    private static readonly string Key = keyConfig.AES_KEY;
    private static readonly string IV = keyConfig.AES_IV;
}
