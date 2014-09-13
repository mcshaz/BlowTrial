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
    public class FailedRestoreEvent : EventArgs
    {
        public FailedRestoreEvent(string filename, Exception exception)
        {
            this.Exception = exception;
            Filename = filename;
        }
        public string Filename { get; private set; }
        public Exception Exception { get; private set; }
    }
    public class ScreenedPatientEventArgs : EventArgs
    {
        public ScreenedPatientEventArgs(ScreenedPatient screenedPatient)
        {
            this.ScreenedPatient = screenedPatient;
        }

        public ScreenedPatient ScreenedPatient { get; private set; }
    }
    public enum CRUD { Created, Updated, Deleted }
    public class ProtocolViolationEventArgs : EventArgs
    {
        public ProtocolViolationEventArgs(ProtocolViolation violation, CRUD eventType)
        {
            this.Violation = violation;
            this.EventType = eventType;
        }

        public ProtocolViolation Violation { get; private set; }
        public CRUD EventType { get; private set; }
    }
}
