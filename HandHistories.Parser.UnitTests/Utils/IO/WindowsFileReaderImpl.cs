using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace HandHistories.Parser.UnitTests.Utils.IO
{
    public class WindowsFileReaderImpl : IFileReader
    {
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }

        public void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

        public void AppendAllText(string path, string contents, Encoding encoding)
        {
            File.AppendAllText(path, contents, encoding);
        }

        public DateTime GetLastModified(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }        

        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string dirPath)
        {
            Directory.CreateDirectory(dirPath);
        }

        public void Copy(string sourceFile, string destPath)
        {
            File.Copy(sourceFile, destPath, true);
        }

        public void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public void DeleteDirectory(string dirPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);

            if (directoryInfo.Exists == false)
            {
                return;
            }

            try
            {
                // delete all files
                foreach (System.IO.FileInfo file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                // delete all sub-directories
                foreach (System.IO.DirectoryInfo subDirectory in directoryInfo.GetDirectories())
                {
                    directoryInfo.Delete(true);
                }

                // delete the root directory
                Directory.Delete(dirPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Directy empty silently failing with " + ex.Message);
            }       
        }

        public string GetDirectory(string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}