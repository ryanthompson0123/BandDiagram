
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Band;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Band.Units;

namespace BandAid.iOS
{
    public partial class MaterialSelectViewController : UITableViewController
    {
        public MaterialType MaterialType { get; set; }
        public List<Material> Materials { get; set; }

        public MaterialSelectViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            LoadMaterials();
            TableView.Source = new MaterialSource(this);

            PreferredContentSize = new SizeF(360, 540);
        }

        private void LoadMaterials()
        {
            if (!MaterialsAreCopiedLocal())
            {
                CopyMaterialsToLocal();
            }

            Materials = new List<Material>();

            switch (MaterialType)
            {
                case MaterialType.Dielectric:
                    var dielectrics = File.ReadAllText(DielectricsPath);
                    Materials.AddRange(JsonConvert.DeserializeObject<List<Dielectric>>(dielectrics, new DielectricConverter()));
                    break;
                case MaterialType.Metal:
                    var metals = File.ReadAllText(MetalsPath);
                    Materials.AddRange(JsonConvert.DeserializeObject<List<Metal>>(metals, new MetalConverter()));
                    break;
                case MaterialType.Semiconductor:
                    var semiconductors = File.ReadAllText(SemiconductorsPath);
                    Materials.AddRange(JsonConvert.DeserializeObject<List<Semiconductor>>(semiconductors, new SemiconductorConverter()));
                    break;
            }
        }

        public LayersTableViewController LayersController { get; set; }

        public async void OnRowSelected(int row)
        {
            if (MaterialType == MaterialType.Semiconductor)
            {
                var alert = UIAlertController.Create("BandAid", "Select Doping Type", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("N Type", UIAlertActionStyle.Default, async action =>
                {
                    ((Semiconductor)Materials[row]).DopingType = DopingType.N;
                    await DismissViewControllerAsync(true);
                    LayersController.OnLayerAdded(Materials[row]);
                }));
                alert.AddAction(UIAlertAction.Create("P Type", UIAlertActionStyle.Default, async action =>
                {
                    ((Semiconductor)Materials[row]).DopingType = DopingType.P;
                    await DismissViewControllerAsync(true);
                    LayersController.OnLayerAdded(Materials[row]);
                }));

                await PresentViewControllerAsync(alert, true);
            }
            else
            {
                var alert = UIAlertController.Create("BandAid", "Enter Thickness (nm)", UIAlertControllerStyle.Alert);
                alert.AddTextField(textField =>
                {
                    textField.KeyboardType = UIKeyboardType.DecimalPad;
                });
                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, async action =>
                {
                    var thickness = Length.FromNanometers(Double.Parse(alert.TextFields[0].Text));
                    Material material;
                    if (MaterialType == MaterialType.Dielectric)
                    {
                        var selectedD = (Dielectric)Materials[row];
                        var newD = new Dielectric(thickness)
                        {
                            BandGap = selectedD.BandGap,
                            ElectronAffinity = selectedD.ElectronAffinity,
                            DielectricConstant = selectedD.DielectricConstant,
                            Name = selectedD.Name,
                            Notes = selectedD.Notes,
                            FillColor = selectedD.FillColor
                        };
                        material = newD;
                    }
                    else
                    {
                        var selectedM = (Metal)Materials[row];
                        var newM = new Metal(thickness)
                        {
                            Name = selectedM.Name,
                            Notes = selectedM.Notes,
                            FillColor = selectedM.FillColor
                        };

                        newM.SetWorkFunction(selectedM.WorkFunction);
                        material = newM;
                    }

                    await DismissViewControllerAsync(true);
                    LayersController.OnLayerAdded(material);
                }));

                await PresentViewControllerAsync(alert, true);
            }
        }

        private bool MaterialsAreCopiedLocal()
        {
            return File.Exists(MetalsPath);
        }

        private void CopyMaterialsToLocal()
        {
            var metalsBundleUrl = NSBundle.MainBundle.PathForResource("metals", "json");
            var dielectricsBundleUrl = NSBundle.MainBundle.PathForResource("dielectrics", "json");
            var semiconductorsBundleUrl = NSBundle.MainBundle.PathForResource("semiconductors", "json");
                   File.Copy(metalsBundleUrl, MetalsPath);
            File.Copy(dielectricsBundleUrl, DielectricsPath);
            File.Copy(semiconductorsBundleUrl, SemiconductorsPath);
        }

        private static string DocumentsPath
        {
            get
            {
                return NSFileManager.DefaultManager.GetUrls(
                    NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
            }
        }

        private static string MetalsPath
        {
            get { return Path.Combine(DocumentsPath, "metals.json"); }
        }

        private static string DielectricsPath
        {
            get { return Path.Combine(DocumentsPath, "dielectrics.json"); }
        }

        private static string SemiconductorsPath
        {
            get { return Path.Combine(DocumentsPath, "semiconductors.json"); }
        }

        class MaterialSource : UITableViewSource
        {
            private readonly MaterialSelectViewController viewController;
            private readonly string[] dielectricLabels = { "Dielectric Constant", "Band Gap", "Electron Affinity" };
            private readonly string[] metalLabels = { "Work Function" };
            private readonly string[] semiconductorLabels = { "Dielectric Constant", "Band Gap", "Electron Affinity", 
                "Intrinsic Carrier Concentration", "Dopant Concentration" };

            public MaterialSource(MaterialSelectViewController viewController)
            {
                this.viewController = viewController;
            }

            public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                switch (viewController.MaterialType)
                {
                    case MaterialType.Dielectric:
                        return 44.0f + (14.0f * 2.0f);
                    case MaterialType.Metal:
                        return 44.0f;
                    case MaterialType.Semiconductor:
                        return 44.0f + (14.0f * 4.0f);
                    default:
                        return 44.0f;
                }
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = (MaterialCell)tableView.DequeueReusableCell("MaterialCell");

                switch (viewController.MaterialType)
                {
                    case MaterialType.Dielectric:
                        var dielectric = (Dielectric)viewController.Materials[indexPath.Row];
                        cell.TitleLabel.Text = dielectric.Name;
                        cell.LeftColumnLabel.Text = String.Join("\n", dielectricLabels);
                        cell.RightColumnLabel.Text = String.Format("{0}\n{1}\n{2}", dielectric.DielectricConstant,
                            dielectric.BandGap.ElectronVolts, dielectric.ElectronAffinity.ElectronVolts);
                        break;
                    case MaterialType.Metal:
                        var metal = (Metal)viewController.Materials[indexPath.Row];
                        cell.TitleLabel.Text = metal.Name;
                        cell.LeftColumnLabel.Text = metalLabels[0];
                        cell.RightColumnLabel.Text = String.Format("{0}", metal.WorkFunction.ElectronVolts);
                        break;
                    case MaterialType.Semiconductor:
                        var semiconductor = (Semiconductor)viewController.Materials[indexPath.Row];
                        cell.TitleLabel.Text = semiconductor.Name;
                        cell.LeftColumnLabel.Text = String.Join("\n", semiconductorLabels);
                        cell.RightColumnLabel.Text = String.Format("{0}\n{1}\n{2}\n{3}\n{4}",
                            semiconductor.DielectricConstant, semiconductor.BandGap.ElectronVolts,
                            semiconductor.ElectronAffinity.ElectronVolts, 
                            semiconductor.IntrinsicCarrierConcentration.PerCubicCentimeter,
                            semiconductor.DopantConcentration.PerCubicCentimeter);
                        break;
                }

                return cell;
            }

            public override int RowsInSection(UITableView tableview, int section)
            {
                if (viewController.Materials == null) return 0;

                return viewController.Materials.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                viewController.OnRowSelected(indexPath.Row);
            }
        }
    }
}