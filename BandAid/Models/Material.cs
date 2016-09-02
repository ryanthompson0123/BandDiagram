using Band.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Band
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MaterialType
    {
        [EnumMember(Value = "metal")]
        Metal, 

        [EnumMember(Value = "dielectric")]
        Dielectric,

        [EnumMember(Value = "semiconductor")]
        Semiconductor 
    }

    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(Material.Converter))]
    public abstract class Material : ObservableObject
	{
        [JsonProperty]
        private string id;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        [JsonProperty]
        private string name;
		public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        [JsonProperty]
        private string notes;
        public string Notes
        {
            get { return notes; }
            set { SetProperty(ref notes, value); }
        }

        [JsonProperty]
        private Color fillColor;
		public Color FillColor
        {
            get { return fillColor; }
            set { SetProperty(ref fillColor, value); }
        }

        [JsonProperty]
        private Length thickness;
        public Length Thickness
        {
            get { return thickness; }
            set { SetProperty(ref thickness, value); }
        }

        [JsonProperty]
        private Energy workFunction;
        public virtual Energy WorkFunction
        {
            get { return workFunction; }
            set { SetProperty(ref workFunction, value); }
        }

		public List<EvalPoint> EvalPoints { get; set; }

        public abstract Energy EnergyFromVacuumToTopBand { get; }
        public abstract Energy EnergyFromVacuumToBottomBand { get; }
        public abstract Energy EnergyFromVacuumToEfi { get; }

        [JsonIgnore]
        public Structure ParentStructure { get; set; }

        [JsonProperty(PropertyName="materialType")]
        public MaterialType MaterialType
        {
            get
            {
                if (this is Dielectric)
                {
                    return MaterialType.Dielectric;
                }

                if (this is Metal)
                {
                    return MaterialType.Metal;
                }

                return MaterialType.Semiconductor;
            }
            set
            {
                return;
            }
        }

		protected Material()
		{
			EvalPoints = new List<EvalPoint>();
            Id = Guid.NewGuid().ToString();
		}

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            EvalPoints = new List<EvalPoint>();
            Prepare();
        }

        protected void InitClone(Material material, Length thickness)
        {
            material.Name = Name;
            material.Notes = Notes;
            material.FillColor = FillColor;
            material.Thickness = thickness;
            material.Id = Id;

            if (material.MaterialType != MaterialType.Semiconductor || ParentStructure != null)
            {
                material.WorkFunction = WorkFunction;
            }

            material.EvalPoints = EvalPoints.Select(ep => ep.DeepClone()).ToList();
        }

        public Material DeepClone()
        {
            return WithThickness(Thickness);
        }

        public abstract Material WithThickness(Length thickness);

		public abstract ElectricField GetElectricField(Length location);
		public abstract ElectricPotential GetPotential(Length location);
		public abstract void Prepare();

        public PlotDataSet GetPotentialDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                PlotColor = FillColor,
                LineThickness = 2
            };

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                var potential = point.Potential;
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y =potential.Volts
                });
            }

            return dataset;
        }

        public abstract List<PlotDataSet> GetEnergyDatasets(Length offset);

        public virtual PlotDataSet GetElectricFieldDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                PlotColor = FillColor,
                LineThickness = 2
            };

            if (ParentStructure.Layers.IndexOf(this) > 0)
            {
                var previousMaterial = ParentStructure.Layers[ParentStructure.Layers.IndexOf(this) - 1];
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = offset.Nanometers,
                    Y = previousMaterial.GetElectricFieldDataset(Length.Zero).DataPoints.Last().Y
                });
            }

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = point.ElectricField.MegavoltsPerCentimeter
                });
            }

            return dataset;
        }

        public virtual PlotDataSet GetChargeDensityDataset(Length offset)
        {
            var dataset = new PlotDataSet
            {
                Name = Name,
                PlotColor = FillColor,
                LineThickness = 2
            };

            foreach (var point in EvalPoints)
            {
                var location = point.Location + offset;
                var charge = point.ChargeDensity;

                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = 0.0
                });
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = charge.MicroCoulombsPerSquareCentimeter
                });
                dataset.DataPoints.Add(new PlotDataPoint
                {
                    X = location.Nanometers, 
                    Y = 0.0
                });
            }

            return dataset;
        }

        public ElectricField GetLastElectricField()
        {
            return EvalPoints.Last().ElectricField;
        }

        public static Material Create(MaterialType materialType)
        {
            switch (materialType)
            {
                case MaterialType.Metal:
                    return new Metal
                    {
                        FillColor = Color.Black,
                        Thickness = Length.FromNanometers(5.0)
                    };
                case MaterialType.Dielectric:
                    return new Dielectric
                    {
                        FillColor = Color.Black,
                        Thickness = Length.FromNanometers(5.0)
                    };
                case MaterialType.Semiconductor:
                    return new Semiconductor
                    {
                        FillColor = Color.Black,
                        Thickness = Length.FromNanometers(50.0)
                    };
                default:
                    return null;
            }
        }

        public class Converter : JsonCreationConverter<Material>
        {
            protected override Material Create(Type objectType, JObject jObject)
            {
                var materialType = (string)jObject["materialType"];

                switch (materialType)
                {
                    case "metal":
                        return new Metal();
                    case "dielectric":
                        return new Dielectric();
                    case "semiconductor":
                        return new Semiconductor();
                    default:
                        return null;
                }
            }
        }
	}
}