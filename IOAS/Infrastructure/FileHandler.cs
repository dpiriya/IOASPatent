using System;
using System.Web;
using System.IO;
using System.Web.Configuration;


namespace IOAS.Infrastructure
{

    public static class ExtensionMethods
    {

        public static byte[] GetFileData(this string fileName, string filePath)
        {
            var filenameNew = HttpUtility.UrlPathEncode(fileName);
            var fullFilePath = string.Format("{0}/{1}", filePath, fileName);
            if (!File.Exists(fullFilePath))
                throw new FileNotFoundException("The file does not exist.",
                    fullFilePath);
            return File.ReadAllBytes(fullFilePath);
        }
        
      
    }
}