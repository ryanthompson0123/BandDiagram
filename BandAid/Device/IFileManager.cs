using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Band
{
    public interface IFileManager
    {
        bool NeedsInitialLibrary { get; }
        string DocumentsPath { get; }
        string MaterialsPath { get; }
        string TestBenchPath { get; }

        Task UnpackAssetsIfNotUnpackedAsync();

        Task SaveTestBenchAsync(TestBench testBench);
        Task<TestBench> LoadTestBenchAsync(string name);
        Task MoveTestBenchAsync(TestBench testBench, string oldName);
        Task DeleteTestBenchAsync(string name);

        Task SaveTestBenchScreenshotAsync(string name, Stream imageStream);

        Task<TestBench> LoadDefaultTestBenchAsync();

        Task<bool> CheckTestBenchExistsAsync(string name);

        Task<IEnumerable<string>> EnumerateTestBenchesAsync();
        Task<string> GetMaterialDataAsync(MaterialType materialType);

        string GetScreenshotPath(string testBenchName);

        Task CopyScreenshotAsync(string sourceName, string destinationName);
    }
}
