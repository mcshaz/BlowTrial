using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Domain.Providers
{
    public class ParticipantEventArgs : EventArgs
    {
        public ParticipantEventArgs(Participant participant)
        {
            this.Participant = participant;
        }

        public Participant Participant { get; private set; }
    }
    public class ScreenedPatientEventArgs : EventArgs
    {
        public ScreenedPatientEventArgs(ScreenedPatient screenedPatient)
        {
            this.ScreenedPatient = screenedPatient;
        }

        public ScreenedPatient ScreenedPatient { get; private set; }
    }
    public class ProtocolViolationEventArgs : EventArgs
    {
        public ProtocolViolationEventArgs(ProtocolViolation violation)
        {
            this.Violation = violation;
        }

        public ProtocolViolation Violation { get; private set; }
    }
}
