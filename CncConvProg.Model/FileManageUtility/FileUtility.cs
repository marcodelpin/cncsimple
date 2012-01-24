using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CncConvProg.Model.FileManageUtility
{
    public static class FileUtility
    {
        public static TRet DeepCopy<TRet>(TRet cloneThis)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, cloneThis);
            ms.Flush();
            ms.Position = 0;
            return (TRet)bf.Deserialize(ms);
        }

        public static void SerializeToFile<TRet>(string filePath, TRet objectToSerialize)
        {
            Stream stream = File.Open(filePath, FileMode.OpenOrCreate);
            var bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public static TRet Deserialize<TRet>(string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open);
            var bformatter = new BinaryFormatter();
            var o = (TRet)bformatter.Deserialize(stream);
            stream.Close();
            return o;
        }
    }
}
