using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FeiniuBus.AspNetCore.Buffering.Test
{
    public class MultipartFormDataTests
    {
        [Fact]
        public async Task BufferRequest_MultipartFormData()
        {
            var file  = await DownloadFileAsync().ConfigureAwait(false);
            var md5 = MD5.Create();
            var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            var value = md5.ComputeHash(fileStream);
            var hex = EncodeToString(value, true);
            
            Console.WriteLine(hex);
            Assert.NotEmpty(hex);
        }
        
        private static string EncodeToString(byte[] data, bool lowercase)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString(lowercase ? "x2" : "X2", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }
        
        private async Task<string> DownloadFileAsync()
        {
            const string url =
                "https://desktop.githubusercontent.com/releases/1.0.9-38a677e8/GitHubDesktop.zip";

            var client = new HttpClient();
            var stream = await client.GetStreamAsync(url).ConfigureAwait(false);

            var fileName = url.Substring(url.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete,
                1024 * 16, FileOptions.Asynchronous | FileOptions.DeleteOnClose | FileOptions.SequentialScan);
            
            try
            {
                var buffer = new byte[8192];
                var isMoreToRead = true;

                do
                {
                    var read = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (read == 0)
                    {
                        isMoreToRead = false;
                    }
                    else
                    {
                        var data = new byte[read];
                        buffer.ToList().CopyTo(0, data, 0, read);
                        await fileStream.WriteAsync(data, 0, data.Length);
                    }
                } while (isMoreToRead);

                return filePath;
            }
            finally
            {
                await fileStream.FlushAsync();
                fileStream.Dispose();
                stream.Dispose();
            }
        }
    }
}