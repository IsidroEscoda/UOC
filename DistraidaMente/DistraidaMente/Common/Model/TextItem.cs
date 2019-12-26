using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DistraidaMente.Model
{
    public abstract class TextItem
    {
        public string Key { get; set; }

        public string Text { get; set; } // TODO: convert into localized string

        public bool Selected { get; set; }

        public Color Color { get; set; }
    }
}
