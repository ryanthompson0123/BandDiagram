using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Band;
using System.ComponentModel;

namespace BandAid.iOS
{
    public class StructureParameterListViewController : UITableViewController
    {
        public TestBenchViewModel ViewModel { get; set; }

        public StructureParameterListViewController(TestBenchViewModel viewModel)
            : base(UITableViewStyle.Grouped)
        {
            ViewModel = viewModel;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            TableView.RegisterClassForCellReuse(typeof(UITableViewCell), new NSString("paramCell"));
            TableView.Source = new ParameterListSource(ViewModel);
            TableView.ContentInset = new UIEdgeInsets(64, 0, 0, 0);
        }

        class ParameterListSource : UITableViewSource
        {
            private readonly TestBenchViewModel viewModel;

            public ParameterListSource(TestBenchViewModel viewModel)
            {
                this.viewModel = viewModel;
            }

            public override string TitleForHeader(UITableView tableView, int section)
            {
                return viewModel.StructureSteps[viewModel.CurrentVoltage.RoundMillivolts].Layers[section].Name;
            }

            public override int NumberOfSections(UITableView tableView)
            {
                if (viewModel.StructureSteps.Count == 0) return 0;

                return viewModel.StructureSteps[viewModel.CurrentVoltage.RoundMillivolts].Layers.Count;
            }

            public override int RowsInSection(UITableView tableview, int section)
            {
                var material = viewModel.StructureSteps[viewModel.CurrentVoltage.RoundMillivolts].Layers[section];

                return material is Metal ? 0 : 2;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = new UITableViewCell(UITableViewCellStyle.Value1, "paramCell");
                var material = viewModel.StructureSteps[viewModel.CurrentVoltage.RoundMillivolts].Layers[indexPath.Section];

                if (indexPath.Row == 0)
                {
                    cell.TextLabel.Text = "Cap. (F/cm2)";

                    if (material is Dielectric)
                    {
                        cell.DetailTextLabel.Text = ((Dielectric)material).OxideCapacitance
                            .FaradsPerSquareCentimeter.ToString();
                    }
                    else
                    {
                        cell.DetailTextLabel.Text = ((Semiconductor)material).CapacitanceDensity
                            .FaradsPerSquareCentimeter.ToString();
                    }
                }
                else
                {
                    cell.TextLabel.Text = "V. Drop (V)";

                    if (material is Dielectric)
                    {
                        cell.DetailTextLabel.Text = ((Dielectric)material).VoltageDrop
                            .Volts.ToString();
                    }
                    else
                    {
                        cell.DetailTextLabel.Text = ((Semiconductor)material).EvalPoints[0].Potential
                            .Volts.ToString();
                    }
                }

                return cell;
            }
        }
    }
}