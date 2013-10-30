using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Helpers
{
    public class SelectableItem<Tkey>
    {
        public SelectableItem(Tkey key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }
        public string DisplayName {get; private set;}
        public Tkey Key {get; private set;}
        bool _isEnabled;
        public bool IsEnabled 
        { 
            get { return _isEnabled; } 
            set { _isEnabled = value; } 
        }
    }
}
