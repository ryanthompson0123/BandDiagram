using MonoTouch.Foundation;
using System;
using System.IO;
using Band;
using Newtonsoft.Json;
using BandAid.iOS;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(FileManager))]

namespace BandAid.iOS
{
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
            if (NeedsInitialLibrary)
            {
                GenerateMaterialsLibrary();
                GenerateDefaultTestBenches();
            }

            return Task.FromResult(true);
        }

        public Task SaveTestBenchAsync(TestBench testBench)
        {
            var outJson = JsonConvert.SerializeObject(testBench, new StructureConverter());

            var filename = Path.Combine(TestBenchPath, testBench.Name + ".json");

            Console.WriteLine(filename);
            File.WriteAllText(filename, outJson);

            return Task.FromResult(true);
        }

        public Task<TestBench> LoadTestBenchAsync(string name)
        {
            var filename = Path.Combine(TestBenchPath, name + ".json");
            var inJson = File.ReadAllText(filename);

            var bench = JsonConvert.DeserializeObject<TestBench>(inJson, new StructureConverter());
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

            File.Copy(highkBundleUrl, Path.Combine(TestBenchPath, DefaultTestBenchName), true);
            File.Copy(nvmBundleUrl, Path.Combine(TestBenchPath, "NVM Stack.json"), true);
        }
    }
}
