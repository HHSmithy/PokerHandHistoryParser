using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Utils.IO
{
    public interface IFileReader
    {
        string ReadAllText(string path);
        string ReadAllText(string path, Encoding encoding);

        void WriteAllText(string path, string contents);
        void WriteAllText(string path, string contents, Encoding encoding);

        void AppendAllText(string path, string contents);
        void AppendAllText(string path, string contents, Encoding encoding);

        DateTime GetLastModified(string path);
        FileInfo GetFileInfo(string path);

        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);

        string GetFileName(string filePath);

        bool FileExists(string path);
        bool DirectoryExists(string path);
        void CreateDirectory(string dirPath);

        void Copy(string sourceFile, string destPath);

        void DeleteFile(string filePath);
        void DeleteDirectory(string dirPath);

        string GetDirectory(string path);
    }
}
