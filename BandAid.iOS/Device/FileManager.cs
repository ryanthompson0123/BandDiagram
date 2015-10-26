using Foundation;
using System;
using System.IO;
using Band;
using Newtonsoft.Json;
using BandAid.iOS;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(FileManager))]

namespace BandAid.iOS
{
    [Preserve(AllMembers = true)]
    public class FileManager : IFileManager
    {
        private const string DefaultTestBenchName = "High - k Stack";

        public bool NeedsInitialLibrary
        {
            get
            {
                return !Directory.Exists(MaterialsPath);
            }
        }

        public string DocumentsPath
        {
            get
            {
                return NSFileManager.DefaultManager.GetUrls(
                    NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
            }
        }

        public string MaterialsPath
        {
            get
            {
                return Path.Combine(DocumentsPath, "materials");
            }
        }

        public string TestBenchPath
        {
            get
            {
                return Path.Combine(DocumentsPath, "testBench");
            }
        }

        public Task UnpackAssetsIfNotUnpackedAsync()
        {
            UnpackAssetsIfNotUnpacked();

            return Task.FromResult(true);
        }

        public void UnpackAssetsIfNotUnpacked()
        {
            if (NeedsInitialLibrary)
            {
                GenerateMaterialsLibrary();
                GenerateDefaultTestBenches();
            }
        }

        public Task SaveTestBenchAsync(TestBench testBench)
        {
            var outJson = JsonConvert.SerializeObject(testBench);

            var filename = Path.Combine(TestBenchPath, testBench.Name + ".json");

            Console.WriteLine(filename);
            File.WriteAllText(filename, outJson);

            return Task.FromResult(true);
        }

        public Task<TestBench> LoadTestBenchAsync(string name)
        {
            var filename = Path.Combine(TestBenchPath, name + ".json");
            var inJson = File.ReadAllText(filename);

            var bench = JsonConvert.DeserializeObject<TestBench>(inJson);
            return Task.FromResult(bench);
        }

        public Task<TestBench> LoadDefaultTestBenchAsync()
        {
            return LoadTestBenchAsync(DefaultTestBenchName);
        }

        public Task MoveTestBenchAsync(TestBench testBench, string oldName)
        {
            var oldFileName = Path.Combine(TestBenchPath, oldName + ".json");
            var newFileName = Path.Combine(TestBenchPath, testBench.Name + ".json");

            var oldScreenshotName = Path.Combine(TestBenchPath, oldName + ".png");
            var newScreenshotName = Path.Combine(TestBenchPath, testBench.Name + ".png");

            File.Move(oldFileName, newFileName);
            File.Move(oldScreenshotName, newScreenshotName);

            return Task.FromResult(true);
        }

        public Task<bool> CheckTestBenchExistsAsync(string name)
        {
            var fileName = Path.Combine(TestBenchPath, name + ".json");

            return Task.FromResult(File.Exists(fileName));
        }

        public async Task<IEnumerable<string>> EnumerateTestBenchesAsync()
        {
            await UnpackAssetsIfNotUnpackedAsync();

            var testBenches = Directory.EnumerateFiles(TestBenchPath)
                                    .Where(f => f.Contains("json"))
                                    .Select(f => Path.GetFileNameWithoutExtension(f));

            return testBenches;
        }

        public string GetScreenshotPath(string testBenchName)
        {
            return Path.Combine(TestBenchPath, testBenchName + ".png");
        }

        public void SaveTestBenchScreenshot(string testBenchName, byte[] bytes)
        {
            var screenshotPath = Path.Combine(TestBenchPath, testBenchName + ".png");

            File.WriteAllBytes(screenshotPath, bytes);
        }

        public Task<string> GetMaterialDataAsync(MaterialType materialType)
        {
            var materialFileName = "";

            switch (materialType)
            {
                case MaterialType.Dielectric:
                    materialFileName = "dielectrics";
                    break;
                case MaterialType.Metal:
                    materialFileName = "metals";
                    break;
                case MaterialType.Semiconductor:
                    materialFileName = "semiconductors";
                    break;
            }

            var path = Path.Combine(MaterialsPath, materialFileName + ".json");

            return Task.FromResult(File.ReadAllText(path));
        }

        private void GenerateMaterialsLibrary()
        {
            var metalsBundleUrl = NSBundle.MainBundle.PathForResource("metals", "json");
            var dielectricsBundleUrl = NSBundle.MainBundle.PathForResource("dielectrics", "json");
            var semiconductorsBundleUrl = NSBundle.MainBundle.PathForResource("semiconductors", "json");

            if (!Directory.Exists(MaterialsPath))
            {
                Directory.CreateDirectory(MaterialsPath);
            }

            File.Copy(metalsBundleUrl, Path.Combine(MaterialsPath, "metals.json"), true);
            File.Copy(dielectricsBundleUrl, Path.Combine(MaterialsPath, "dielectrics.json"), true);
            File.Copy(semiconductorsBundleUrl, Path.Combine(MaterialsPath, "semiconductors.json"), true);
        }

        private void GenerateDefaultTestBenches()
        {
            var highkBundleUrl = NSBundle.MainBundle.PathForResource("highk", "json");
            var nvmBundleUrl = NSBundle.MainBundle.PathForResource("nvm", "json");

            if (!Directory.Exists(TestBenchPath))
            {
                Directory.CreateDirectory(TestBenchPath);
            }

            File.Copy(highkBundleUrl, Path.Combine(TestBenchPath, DefaultTestBenchName + ".json"), true);
            File.Copy(nvmBundleUrl, Path.Combine(TestBenchPath, "NVM Stack.json"), true);
        }
    }
}
