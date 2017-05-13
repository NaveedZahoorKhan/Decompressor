using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Decompressor
{
    class Program
    {
        static void DecompressToDirectory(string sCompressedFile, string sDir, ProgressDelegate progress)
        {
            using (FileStream inFile = new FileStream(sCompressedFile, FileMode.Open, FileAccess.Read, FileShare.None))
            using (GZipStream zipStream = new GZipStream(inFile, CompressionMode.Decompress, true))
                while (DecompressFile(sDir, zipStream, progress)) ;
        }
        static bool DecompressFile(string sDir, GZipStream zipStream, ProgressDelegate progress)
        {
            string sCompressedFiler = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.None) + "\\MacroSoft\\" + DateTime.Now.Day + "-" + DateTime.Now.Month;

            //Decompress file name
            byte[] bytes = new byte[sizeof(int)];
            int Readed = zipStream.Read(bytes, 0, sizeof(int));
            if (Readed < sizeof(int))
                return false;

            int iNameLen = BitConverter.ToInt32(bytes, 0);
            bytes = new byte[sizeof(char)];
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < iNameLen; i++)
            {
                zipStream.Read(bytes, 0, sizeof(char));
                char c = BitConverter.ToChar(bytes, 0);
                sb.Append(c);
            }
            string sFileName = sb.ToString();
            if (progress != null)
                progress(sFileName);

            //Decompress file content
            bytes = new byte[sizeof(int)];
            zipStream.Read(bytes, 0, sizeof(int));
            int iFileLen = BitConverter.ToInt32(bytes, 0);

            bytes = new byte[iFileLen];
            zipStream.Read(bytes, 0, bytes.Length);

            string sFilePath = Path.Combine(sDir, sFileName);
            string sFinalDir = Path.GetDirectoryName(sFilePath);
            if (!Directory.Exists(sFinalDir))
                Directory.CreateDirectory(sFinalDir);
            Directory.GetAccessControl(sFinalDir);
            Directory.GetAccessControl(sCompressedFiler); 
            FileStream outFile = new FileStream(sFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            outFile.GetAccessControl();
            
                outFile.Write(bytes, 0, iFileLen);
            
            return true;
        }

        delegate void ProgressDelegate(string sMessage);


        static void Main(string[] args)
        {
        string sDir =  "E:\\";
           
       string sCompressedFile = DateTime.Now.Day + "-" + DateTime.Now.Month;
            Directory.GetAccessControl(sDir);
                bool bCompress = false;
            DecompressToDirectory(sCompressedFile+".gz", sDir, (fileName) => { Console.WriteLine("Decompressing {0}...", fileName); });
            Console.WriteLine("Data has been decompressed");
           // return 0;
        }
    }
}
