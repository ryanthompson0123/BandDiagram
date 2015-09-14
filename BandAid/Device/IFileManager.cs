using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        Task<TestBench> LoadDefaultTestBenchAsync();

        Task<bool> CheckTestBenchExistsAsync(string name);
    }
}
