using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.EventArguments
{
    enum CancelOrSaveOption
    {
        Cancel,
        Save
    }
    class CancelledOrSavedEventArgs : EventArgs
    {
        public CancelledOrSavedEventArgs(CancelOrSaveOption option)
        {
            Option = option;
        }
        public CancelOrSaveOption Option { get; private set; }
    }
}
