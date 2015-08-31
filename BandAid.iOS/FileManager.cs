using MonoTouch.Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Band;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BandAid.iOS
{
    public static class FileManager
    {
        public static bool NeedsInitialLibrary
        {
            get
            {
                return !Directory.Exists(MaterialsPath);
            }
        }

        public static string DocumentsPath
        {
            get
            {
                return NSFileManager.DefaultManager.GetUrls(
                    NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
            }
        }

        public static string MaterialsPath
        {
            get
            {
                return Path.Combine(DocumentsPath, "materials");
            }
        }

        public static string StructuresPath
        {
            get
            {
                return Path.Combine(DocumentsPath, "structures");
            }
        }

        public static void UnpackAssetsIfNotUnpacked()
        {
            if (NeedsInitialLibrary)
            {
                GenerateMaterialsLibrary();
                GenerateDefaultStructures();
            }
        }

        public static void SaveStructure(TestBenchViewModel structure)
        {
            var obj = new JObject();
            obj["name"] = structure.Name;
            obj["minVoltage"] = structure.MinVoltage.Volts;
            obj["maxVoltage"] = structure.MaxVoltage.Volts;
            obj["stepSize"] = structure.StepSize.Volts;
            obj["currentStep"] = structure.CurrentVoltage.Volts;

            var structureJson = JsonConvert.SerializeObject(structure.ReferenceStructure,
                                new StructureConverter());
            obj["referenceStructure"] = JObject.Parse(structureJson);

            var outDir = Path.Combine(DocumentsPath, "save");
            var filename = Path.Combine(outDir, structure.Name + ".json");

            Console.WriteLine(filename);
            File.WriteAllText(filename, obj.ToString());
        }

        private static void GenerateMaterialsLibrary()
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

        private static void GenerateDefaultStructures()
        {
            var highkBundleUrl = NSBundle.MainBundle.PathForResource("highk", "json");
            var nvmBundleUrl = NSBundle.MainBundle.PathForResource("nvm", "json");

            if (!Directory.Exists(StructuresPath))
            {
                Directory.CreateDirectory(StructuresPath);
            }

            File.Copy(highkBundleUrl, Path.Combine(StructuresPath, "High-k Stack.json"), true);
            File.Copy(nvmBundleUrl, Path.Combine(StructuresPath, "NVM Stack.json"), true);
        }
    }
}
