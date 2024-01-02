//default usings added by visual studio
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//add the following usings to project
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace BoilerPlateDraft.Code_Repo
{
	public class Encryption
	{
    //function will encrypt string passed in signature variable plainText using the 
    //passed in key
    //utilizes AES encryption
    public static string EncryptString(string key, string plainText)
    {
      byte[] iv = new byte[16];
      byte[] array;

      using (Aes aes = Aes.Create())
      {
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
          {
            using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
            {
              streamWriter.Write(plainText);
            }

            array = memoryStream.ToArray();
          }
        }
      }

      return Convert.ToBase64String(array);
    }

    //function will decrypt string passed in signature variable cipherText using the 
    //passed in key - key must match key used to encrypt text
    //utilizes AES encryption
    public static string DecryptString(string key, string cipherText)
    {
      byte[] iv = new byte[16];
      byte[] buffer = Convert.FromBase64String(cipherText);

      using (Aes aes = Aes.Create())
      {
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = iv;
        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using (MemoryStream memoryStream = new MemoryStream(buffer))
        {
          using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
          {
            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
            {
              return streamReader.ReadToEnd();
            }
          }
        }
      }
    }
  }
}