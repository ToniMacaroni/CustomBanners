using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomBanners.Helpers
{
    public static class CommonExtensions
    {
        public static FileInfo File(this DirectoryInfo dir, string name)
        {
            return new FileInfo(Path.Combine(dir.FullName, name));
        }

        public static async Task<string> ReadFileTextAsync(this FileInfo file)
        {
            return await ReadFileTextAsync(file.FullName);
        }

        public static async Task<string> ReadFileTextAsync(string path)
        {
            var data = await ReadFileDataAsync(path);
            return Encoding.UTF8.GetString(data);
        }

        public static async Task<byte[]> ReadFileDataAsync(this FileInfo file)
        {
            return await ReadFileDataAsync(file.FullName);
        }

        public static async Task<byte[]> ReadFileDataAsync(string path)
        {
            FileStream sourceStream = new FileStream(path,
                FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);

            byte[] data = new byte[sourceStream.Length];
            await sourceStream.ReadAsync(data, 0, data.Length);

            return data;
        }

        public static Texture2D CreateTexture(byte[] data, string name)
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(data);
            tex.name = name;
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Trilinear;
            return tex;
        }
    }
}
