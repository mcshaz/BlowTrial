using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.ViewModel
{
    public abstract class WizardPageViewModel : CrudWorkspaceViewModel
    {
        bool _isCurrentPage;
        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set
            {
                if (value == _isCurrentPage)
                    return;

                _isCurrentPage = value;
                NotifyPropertyChanged("IsCurrentPage");
            }
        }

    }
}
