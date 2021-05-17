using System.Reflection;
using System.Threading.Tasks;
using CustomBanners;
using SiraUtil.Tools;

namespace CustomBanners.Helpers
{
    public static class EmbeddedAssetUtilities
    {
        /// <summary>
        /// Reads an embedded resource
        /// </summary>
        /// <param name="relativePath">path relative to the root namespace</param>
        /// <param name="data">the read data</param>
        /// <returns></returns>
        public static async Task<ReadDataResult> ReadAsync(string relativePath)
        {
            var result = new ReadDataResult();

            var fullPath = $"{Plugin.Name}.{relativePath}";

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullPath);

            if (stream == null) return result;
            
            result.Data = new byte[stream.Length];
            await stream.ReadAsync(result.Data, 0, (int)stream.Length);

            result.Success = true;

            return result;
        }

        public struct ReadDataResult
        {
            public bool Success;
            public byte[] Data;
        }
    }
}
