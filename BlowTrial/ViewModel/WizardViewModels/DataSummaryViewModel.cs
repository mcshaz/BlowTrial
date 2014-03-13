﻿using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.ViewModel
{
    public class DataSummaryViewModel : WorkspaceViewModel
    {
        public DataSummaryViewModel(IRepository repository) : base(repository)
        {
            ParticipantData = _repository.GetParticipantSummary();
            ScreenedPatientData = _repository.GetScreenedPatientSummary();
            _repository.ParticipantAdded += _repository_ParticipantAdded;
            _repository.ParticipantUpdated += _repository_ParticipantUpdated;
            _repository.ScreenedPatientAdded += _repository_ScreenedPatientAdded;
        }

        void _repository_ParticipantUpdated(object sender, Domain.Providers.ParticipantEventArgs e)
        {
            //too hard - can alter this later 
            ParticipantData = _repository.GetParticipantSummary();
            NotifyPropertyChanged("ParticipantData");
        }

        void _repository_ScreenedPatientAdded(object sender, Domain.Providers.ScreenedPatientEventArgs e)
        {
            ScreenedPatientData.TotalCount++;
            if (e.ScreenedPatient.BadInfectnImmune)
            {
                ScreenedPatientData.BadInfectnImmuneCount++;
            }
            if (e.ScreenedPatient.BadMalform)
            {
                ScreenedPatientData.BadMalformCount++;
            }
            if (e.ScreenedPatient.LikelyDie24Hr)
            {
                ScreenedPatientData.LikelyDie24HrCount++;
            }
            if (e.ScreenedPatient.Missed==true)
            {
                ScreenedPatientData.MissedCount++;
            }
            if (e.ScreenedPatient.RefusedConsent == true)
            {
                ScreenedPatientData.RefusedConsentCount++;
            }
            if (e.ScreenedPatient.WasGivenBcgPrior)
            {
                ScreenedPatientData.WasGivenBcgPriorCount++;
            }
            NotifyPropertyChanged("ScreenedPatientData");
        }

        void _repository_ParticipantAdded(object sender, Domain.Providers.ParticipantEventArgs e)
        {
            ParticipantData.TotalCount++;
            if (e.Participant.IsInterventionArm)
            {
                ParticipantData.InterventionArmCount++;
            }
            NotifyPropertyChanged("ParticipantData");
        }
        public ParticipantsSummary ParticipantData { get; private set; }
        public ScreenedPatientsSummary ScreenedPatientData { get; private set; }
    }
}
