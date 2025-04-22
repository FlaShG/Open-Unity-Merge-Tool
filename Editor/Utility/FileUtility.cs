namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.IO;
    using UnityEditor;

    internal static class FileUtility
    {
        public static bool IsScene(string path)
        {
            return path.EndsWith(".unity");
        }

        public static bool IsPrefab(string path)
        {
            return path.EndsWith(".prefab");
        }

        public static bool IsScriptableObject(string path)
        {
            return path.EndsWith(".asset");
        }

        public static bool IsSupportedByMergeTool(string path)
        {
            return IsScene(path) || IsPrefab(path);
        }

        public static string CopyFile(string source, string destinationSuffix)
        {
            var destination = AttachSuffix(source, destinationSuffix);
            FileUtil.CopyFileOrDirectory(source, destination);
            return destination;
        }

        public static string MoveFile(string source, string destinationSuffix)
        {
            var destination = AttachSuffix(source, destinationSuffix);
            FileUtil.MoveFileOrDirectory(source, destination);
            return destination;
        }

        public static void DeleteFile(string source, string destinationSuffix)
        {
            var path = AttachSuffix(source, destinationSuffix);
            if (!File.Exists(path))
            {
                return;
            }
            FileUtil.DeleteFileOrDirectory(path);
        }

        public static string AttachSuffix(string path, string suffix)
        {
            return Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + suffix + Path.GetExtension(path);
        }
    }
}
