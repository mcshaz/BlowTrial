using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BlowTrial.Infrastructure.Extensions;

namespace BlowTrial.Models
{
    public class StudyCentreModel
    {
        public int Id { get; set; }
        public Guid DuplicateIdCheck { get; set; }
        public String Name { get; set; }
        public int ArgbTextColour 
        { 
            set 
            { 
                TextColour = new SolidColorBrush(value.ToColor()); 
            } 
        }
        public int ArgbBackgroundColour 
        { 
            set 
            { 
                BackgroundColour = new SolidColorBrush(value.ToColor()); 
            } 
        }
        public Brush TextColour
        {
            get;
            private set;
        }
        public Brush BackgroundColour
        {
            get;
            private set;
        }
        public string PhoneMask { get; set; }
        public string HospitalIdentifierMask { get; set; }
        public int MaxIdForSite { get; set; }
    }
}
