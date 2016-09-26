
using System;
using CoreGraphics;

using Foundation;
using UIKit;
using Band;
using System.ComponentModel;

namespace BandAid.iOS
{
    public partial class StructureCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("structureGridItem");

        private StructureGalleryItemViewModel viewModelValue;
        public StructureGalleryItemViewModel ViewModel
        {
            get { return viewModelValue; }
            set
            {
                if (viewModelValue != null)
                {
                    viewModelValue.PropertyChanged -= ViewModelValue_PropertyChanged;
                }

                viewModelValue = value;

                if (viewModelValue != null)
                {
                    viewModelValue.PropertyChanged += ViewModelValue_PropertyChanged;
                }

                UpdateAllBindings();
            }
        }

        public UIImageView ImageView
        {
            get { return imageView; }
        }

        public UILabel TitleLabel
        {
            get { return titleLabel; }
        }

        public bool IsSelected { get; set; }

        public StructureCollectionViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();

            if (imageView.Image != null)
            {
                imageView.Image.Dispose();
                imageView.Image = null;
            }

            titleLabel.Text = "";
        }

        private void ViewModelValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateBinding(e.PropertyName);
        }

        private UIImage GetImage(string path)
        {
            var imageData = NSData.FromFile(path);

            if (imageData == null) return null;

            return new UIImage(imageData);
        }

        private void UpdateBinding(string propertyName)
        {
            switch (propertyName)
            {
                case "IsSelected":
                    if (ViewModel.IsSelected)
                    {
                        Select();
                    }
                    else
                    {
                        Deselect();
                    }
                    return;
                case "TitleText":
                    TitleLabel.Text = ViewModel.TitleText;
                    break;
                case "ImageFile":
                    ImageView.Image = GetImage(ViewModel.ImageFile);
                    break;
            }
        }

        private void UpdateAllBindings()
        {
            UpdateBinding("IsSelected");
            UpdateBinding("TitleText");
            UpdateBinding("ImageFile");
        }

        private void Select()
        {
            IsSelected = true;

            ImageView.Layer.BorderWidth = 4.0f;
            ImageView.Layer.BorderColor = StructureGalleryViewController.HighlightColor.CGColor;
        }

        private void Deselect()
        {
            IsSelected = false;

            ImageView.Layer.BorderWidth = 0.0f;
            ImageView.Layer.BorderColor = UIColor.Clear.CGColor;
        }
    }
}