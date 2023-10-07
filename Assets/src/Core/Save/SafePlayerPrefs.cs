using UnityEngine;
using System.Collections.Generic;

namespace Core.save
{
    public class SafePlayerPrefs
    {

        private string m_key;
        private List<string> m_properties = new List<string>();

        public SafePlayerPrefs(string key, params string[] properties)
        {
            this.m_key = key;
            foreach(string property in properties)
                this.m_properties.Add(property);
        }

        private string GenerateCheckSum()
        {
            string hash = "";
            foreach(string property in m_properties)
            {
                hash += property + ":";
                if(PlayerPrefs.HasKey(property))
                    hash += PlayerPrefs.GetString(property);
            }
            return Crypt.Md5Sum(hash);
        }

        public void Save()
        {
            string checksum = GenerateCheckSum();
            PlayerPrefs.SetString("CHECKSUM"+m_key,checksum);
            PlayerPrefs.Save();
        }

        public bool HasBeenEdited()
        {
            if(!PlayerPrefs.HasKey("CHECKSUM"+m_key))
                return true;

            string checksumSaved = PlayerPrefs.GetString("CHECKSUM"+m_key);
            string checksumReal = GenerateCheckSum();

            return !checksumSaved.Equals(checksumReal);
        }
    }
}
