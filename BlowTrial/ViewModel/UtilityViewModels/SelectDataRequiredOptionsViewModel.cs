using BlowTrial.Domain.Outcomes;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using MvvmExtraLite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.ViewModel
{
    public class SelectDataRequiredOptionsViewModel 
    {
        bool _isAnySelected;

        public SelectDataRequiredOptionsViewModel(IRepository _repo, IEnumerable<DataRequiredOption> selectedDataRequired = null, IEnumerable<StudyCentreModel> centres = null) 
        {
            var options = Enum.GetValues(typeof(DataRequiredOption)).Cast<DataRequiredOption>()
                .Where(d=>d!= DataRequiredOption.NotSet).ToList();
            AllOptions = selectedDataRequired==null
                ?options.Select(dro => new DataRequiredOptionsViewModel(dro) { IsSelected = true })
                    .ToList()
                :options.Select(dro => new DataRequiredOptionsViewModel(dro) { IsSelected = selectedDataRequired.Contains(dro) })
                    .ToList();

            AllCentres = centres==null
                ? _repo.LocalStudyCentres.Select(c => new StudyCentreOptionsViewModel(c) { IsSelected = true }).ToArray()
                : _repo.LocalStudyCentres.Select(c => new StudyCentreOptionsViewModel(c) { IsSelected = centres.Contains(c) }).ToArray();

            _isAnySelected = true;

            foreach (var o in AllOptions)
            {
                o.PropertyChanged += Selected_PropertyChanged;
            }

            foreach (var c in AllCentres)
            {
                c.PropertyChanged += Selected_PropertyChanged;
            }

            WasCancelled = true;

            MultiCentreOption = AllCentres.Skip(1).Any();

            SelectAll = new RelayCommand(p =>
            {
                foreach(var o in AllOptions)
                {
                    o.IsSelected = true;
                }
                foreach (var c in AllCentres)
                {
                    c.IsSelected = true;
                }
            });

            UnselectAll = new RelayCommand(p =>
            {
                foreach (var o in AllOptions)
                {
                    o.IsSelected = false;
                }
                foreach (var c in AllCentres)
                {
                    c.IsSelected = false;
                }
            });

            Close = new RelayCommand(p=>RequestClose(this, new EventArgs()));

            Apply = new RelayCommand(p =>
            {
                WasCancelled = false;
                Close.Execute(null);
            }, p => _isAnySelected);
        }

        void Selected_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName=="IsSelected")
            {
                _isAnySelected = AllOptions.Any(o => o.IsSelected) && AllCentres.Any(o => o.IsSelected);
            }
        }

        public bool WasCancelled { get; private set; }

        public IEnumerable<StudyCentreOptionsViewModel> AllCentres { get; private set; }

        public bool MultiCentreOption { get; private set; }

        public IEnumerable<StudyCentreModel> SelectedCentres
        {
            get
            {
                return (from c in AllCentres
                        where c.IsSelected
                        select c.Centre);
            }
        }

        public IEnumerable<DataRequiredOptionsViewModel> AllOptions { get; private set; }

        public IEnumerable<DataRequiredOption> SelectedDataRequired
        {
            get
            {
                return (from o in AllOptions
                        where o.IsSelected
                        select o.DataRequired);
            }
        }

        public RelayCommand SelectAll { get; private set; }
        public RelayCommand UnselectAll { get; private set; }
        public RelayCommand Apply { get; private set; }
        public RelayCommand Close { get; private set; }

        public EventHandler RequestClose;
    }

    public class StudyCentreOptionsViewModel : NotifyChangeBase
    {
        StudyCentreModel _centre;

        public StudyCentreOptionsViewModel(StudyCentreModel centre)
        {
            _centre = centre;
        }

        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) { return; }
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public string Name
        {
            get { return _centre.Name; }
        }

        public StudyCentreModel Centre
        {
            get { return _centre; }
        }
    }

    public class DataRequiredOptionsViewModel :NotifyChangeBase
    {
        public DataRequiredOptionsViewModel(DataRequiredOption dataRequired)
        {
            DataRequired = dataRequired;
            DataRequiredString = DataRequiredStrings.GetDetails(dataRequired);
        }

        bool _isSelected;

        public bool IsSelected 
        {
            get 
            {
                return _isSelected;
            }
            set
            {
                if (value==_isSelected) {return;}
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
            }
        }

        public DataRequiredOption DataRequired
        {
            get;
            private set;
        }

        public string DataRequiredString
        {
            get;
            private set;
        }
    }
}
