using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    public class ParticipantEventArgs : EventArgs
    {
        public ParticipantEventArgs(Participant newParticipant)
        {
            NewParticipant = newParticipant;
        }

        public Participant NewParticipant { get; private set; }
    }
    public class ScreenedPatientEventArgs : EventArgs
    {
        public ScreenedPatientEventArgs(ScreenedPatient newScreenedPatient)
        {
            NewScreenedPatient = newScreenedPatient;
        }

        public ScreenedPatient NewScreenedPatient { get; private set; }
    }
}
