using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.ViewModel
{
    public abstract class CrudWorkspaceViewModel : WorkspaceViewModel
    {
        #region fields
        bool? _wasValidOnLastNotify;
        #endregion
        public CrudWorkspaceViewModel(IRepository repository)
            : base(repository)
        {
        }
        public CrudWorkspaceViewModel()
            : base()
        {
        }
        public bool WasValidOnLastNotify 
        { 
            get
            {
                return _wasValidOnLastNotify ?? (_wasValidOnLastNotify = IsValid()).Value;
            }
        }
        public abstract bool IsValid();
        protected override void NotifyPropertyChanged(params string[] propertyNames)
        {
            base.NotifyPropertyChanged(propertyNames);
            if (propertyNames.Length>1 || propertyNames[0]!="DisplayName")
            {
                _wasValidOnLastNotify = IsValid();
            }
        }
    }
}
