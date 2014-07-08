using BlowTrial.Domain.Outcomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Helpers
{
    class OutcomeAt28DaysSplitter
    {
        #region Constructors
        public OutcomeAt28DaysSplitter(OutcomeAt28DaysOption outcome)
        {
            _outcomeAt28Days = outcome;
            switch (_outcomeAt28Days)
            {
                case OutcomeAt28DaysOption.DischargedAndLikelyToHaveDied:
                    _postDischargeOutcomeKnown = false;
                    _diedAfterDischarge = true;
                    break;
                case OutcomeAt28DaysOption.DischargedAndKnownToHaveDied:
                    _postDischargeOutcomeKnown = true;
                    _diedAfterDischarge = true;
                    break;
                case OutcomeAt28DaysOption.DischargedAndLikelyToHaveSurvived:
                    _postDischargeOutcomeKnown = false;
                    _diedAfterDischarge = false;
                    break;
                case OutcomeAt28DaysOption.DischargedAndKnownToHaveSurvived:
                    _postDischargeOutcomeKnown = true;
                    _diedAfterDischarge = false;
                    break;
            }
        }
        #endregion
        #region Properties
        public OutcomeAt28DaysOption OutcomeAt28Days 
        {
            get { return _outcomeAt28Days; }
        }
        public OutcomeAt28DaysOption OutcomeAt28orDischarge
        {
            get
            {
                return (_outcomeAt28Days < OutcomeAt28DaysOption.DischargedBefore28Days)
                    ? _outcomeAt28Days
                    : OutcomeAt28DaysOption.DischargedBefore28Days;
            }
            set
            {
                _outcomeAt28Days = value;
                SetOutcomeAt28Days();
            }
        }
        public bool? DiedAfterDischarge 
        {
            get 
            {
                return _diedAfterDischarge;
            }
            set 
            {
                _diedAfterDischarge = value;
                SetOutcomeAt28Days();
            }
        }
        public bool? PostDischargeOutcomeKnown 
        {
            get 
            {
                return _postDischargeOutcomeKnown;
            }
            set 
            {
                _postDischargeOutcomeKnown = value;
                SetOutcomeAt28Days();
            }
        }
        public bool PostDischargeFieldsComplete
        {
            get 
            {
                return _outcomeAt28Days== OutcomeAt28DaysOption.DischargedAndOutcomeCompletelyUnknown
                    || (_postDischargeOutcomeKnown.HasValue && _diedAfterDischarge.HasValue);
            }
        }
        public bool OutcomeCompletelyUnknown
        {
            get
            {
                return _outcomeAt28Days == OutcomeAt28DaysOption.DischargedAndOutcomeCompletelyUnknown;
            }
            set
            {
                if (value)
                {
                    _outcomeAt28Days = OutcomeAt28DaysOption.DischargedAndOutcomeCompletelyUnknown;
                    _diedAfterDischarge = _postDischargeOutcomeKnown = null;
                }
                else if (_outcomeAt28Days == OutcomeAt28DaysOption.DischargedAndOutcomeCompletelyUnknown)
                {
                    _outcomeAt28Days = OutcomeAt28DaysOption.DischargedBefore28Days;
                }
            }
        }
        #endregion
        #region Fields 
        OutcomeAt28DaysOption _outcomeAt28Days;
        bool? _postDischargeOutcomeKnown;
        bool? _diedAfterDischarge;
        #endregion
        #region Methods
        
        void SetOutcomeAt28Days()
        {
            if (_outcomeAt28Days < OutcomeAt28DaysOption.DischargedBefore28Days)
            {
                return;
            }
            else if (_diedAfterDischarge.HasValue && _postDischargeOutcomeKnown.HasValue)
            {
                if (_diedAfterDischarge.Value)
                {
                    if (_postDischargeOutcomeKnown.Value)
                    {
                        _outcomeAt28Days = OutcomeAt28DaysOption.DischargedAndKnownToHaveDied;
                    }
                    else
                    {
                        _outcomeAt28Days = OutcomeAt28DaysOption.DischargedAndLikelyToHaveDied;
                    }
                }
                else // Survived
                {
                    if (_postDischargeOutcomeKnown.Value)
                    {
                        _outcomeAt28Days = OutcomeAt28DaysOption.DischargedAndKnownToHaveSurvived;
                    }
                    else
                    {
                        _outcomeAt28Days = OutcomeAt28DaysOption.DischargedAndLikelyToHaveSurvived;
                    }
                }
            }
            else
            {
                _outcomeAt28Days = OutcomeAt28DaysOption.DischargedBefore28Days;
            }
        }
        #endregion
    }
}
