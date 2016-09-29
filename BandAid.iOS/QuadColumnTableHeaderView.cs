using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CoreGraphics;
using UIKit;
using Foundation;

namespace BandAid.iOS
{
    public class ColumnClickEventArgs : EventArgs
    {
        public int ClickedIndex { get; set; }
    }

    public class ColumnLongPressEventArgs : EventArgs
    {
        public int Index { get; set; }
        public UIButton Button { get; set; }
    }

    partial class QuadColumnTableHeaderView : UIView
    {
        public event EventHandler<EventArgs> TitleClick;
        public event EventHandler<ColumnClickEventArgs> ColumnClick;
        public event EventHandler<ColumnLongPressEventArgs> ColumnLongPress;

        private ObservableCollection<string> headersValue;
        public ObservableCollection<string> Headers
        {
            get { return headersValue; }
            set
            {
                if (headersValue != null)
                {
                    headersValue.CollectionChanged -= Headers_CollectionChanged;
                }

                headersValue = value;

                if (headersValue != null)
                {
                    headersValue.CollectionChanged += Headers_CollectionChanged;
                }

                RemoveUnusedHeaders();
                SetUpHeaders();
            }
        }

        public List<string> HeaderHints { get; set; }

        public string TitleText
        {
            set { TitleButton.SetTitle(value, UIControlState.Normal); }
        }

        public QuadColumnTableHeaderView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Column1Button.AddGestureRecognizer(new UITapGestureRecognizer(() => OnColumnClick(Headers.Count - 4)));
            Column1Button.AddGestureRecognizer(new UILongPressGestureRecognizer((r) => OnColumnLongPress(Headers.Count - 4, Column1Button, r)));

            Column2Button.AddGestureRecognizer(new UITapGestureRecognizer(() => OnColumnClick(Headers.Count - 3)));
            Column2Button.AddGestureRecognizer(new UILongPressGestureRecognizer((r) => OnColumnLongPress(Headers.Count - 3, Column2Button, r)));

            Column3Button.AddGestureRecognizer(new UITapGestureRecognizer(() => OnColumnClick(Headers.Count - 2)));
            Column3Button.AddGestureRecognizer(new UILongPressGestureRecognizer((r) => OnColumnLongPress(Headers.Count - 2, Column3Button, r)));

            Column4Button.AddGestureRecognizer(new UITapGestureRecognizer(() => OnColumnClick(Headers.Count - 1)));
            Column4Button.AddGestureRecognizer(new UILongPressGestureRecognizer((r) => OnColumnLongPress(Headers.Count - 1, Column4Button, r)));                             
        }

        void Headers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetUpHeaders();
        }

        private void RemoveUnusedHeaders()
        {
            switch (Headers.Count)
            {
                case 0:
                    Column1Button.RemoveFromSuperview();
                    Column2Button.RemoveFromSuperview();
                    Column3Button.RemoveFromSuperview();
                    Column4Button.RemoveFromSuperview();
                    break;
                case 1:
                    Column1Button.RemoveFromSuperview();
                    Column2Button.RemoveFromSuperview();
                    Column3Button.RemoveFromSuperview();
                    break;
                case 2:
                    Column1Button.RemoveFromSuperview();
                    Column2Button.RemoveFromSuperview();
                    break;
                case 3:
                    Column1Button.RemoveFromSuperview();
                    break;
                default:
                    break;
            }
        }

        private void SetUpHeaders()
        {
            switch (Headers.Count)
            {
                case 0:
                    break;
                case 1:
                    Column4Button.SetTitle(Headers[0], UIControlState.Normal);
                    break;
                case 2:
                    Column3Button.SetTitle(Headers[0], UIControlState.Normal);
                    Column4Button.SetTitle(Headers[1], UIControlState.Normal);
                    break;
                case 3:
                    Column2Button.SetTitle(Headers[0], UIControlState.Normal);
                    Column3Button.SetTitle(Headers[1], UIControlState.Normal);
                    Column4Button.SetTitle(Headers[2], UIControlState.Normal);
                    break;
                default:
                    Column1Button.SetTitle(Headers[0], UIControlState.Normal);
                    Column2Button.SetTitle(Headers[1], UIControlState.Normal);
                    Column3Button.SetTitle(Headers[2], UIControlState.Normal);
                    Column4Button.SetTitle(Headers[3], UIControlState.Normal);
                    break;
            }
        }

        private void OnColumnClick(int index)
        {
            if (ColumnClick != null)
            {
                ColumnClick(this, new ColumnClickEventArgs
                {
                    ClickedIndex = index
                });
            }
        }

        private void OnColumnLongPress(int index, UIButton button, UIGestureRecognizer r)
        {
            if (r.State == UIGestureRecognizerState.Began)
            {
                if (ColumnLongPress != null)
                {
                    ColumnLongPress(this, new ColumnLongPressEventArgs
                    {
                        Index = index,
                        Button = button
                    });
                }
            }
        }

        partial void TitleClicked(NSObject sender)
        {
            if (TitleClick != null)
            {
                TitleClick(this, EventArgs.Empty);
            }
        }
    }
}
