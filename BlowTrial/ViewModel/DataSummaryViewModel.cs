using BlowTrial.Domain.Outcomes;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BlowTrial.Infrastructure.Extensions;
using BlowTrial.Properties;
using BlowTrial.Helpers;
using BlowTrial.Infrastructure;
using BlowTrial.Domain.Providers;

namespace BlowTrial.ViewModel
{
    public class DataSummaryViewModel : WorkspaceViewModel
    {
        AgeUpdatingService _ageService;
        public DataSummaryViewModel(IRepository repository) : base(repository)
        {
            _participantData = _repository.GetParticipantSummary();
            ParticipantData = new ParticipantSummaryViewModel(_participantData);
            ScreenedPatientData = _repository.GetScreenedPatientSummary();
            _repository.ParticipantAdded += _repository_ParticipantAdded;
            _repository.ParticipantUpdated += _repository_ParticipantUpdated;
            _repository.ScreenedPatientAdded += _repository_ScreenedPatientAdded;
            _ageService = AgeUpdatingMediator.GetService(repository);
            _ageService.OnAgeIncrement += OnNewAge;

        }
        void OnNewAge(object sender, AgeIncrementingEventArgs e)
        {
            _repository_ParticipantUpdated(null, new ParticipantEventArgs(_repository.FindParticipant(e.Participant.Id)));
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

        void _repository_ParticipantAdded(object sender, ParticipantEventArgs e)
        {
            var newPos =_participantData.AddParticipant(e.Participant.Id, e.Participant.TrialArm, ParticipantBaseModel.DataRequiredFunc(e.Participant));

            if (_participantData.ColHeaders.Count > ParticipantData.ColHeaders.Count) //1st patient randomised to new arm
            {
                int newColIndex = _participantData.ColHeaders.Count - 1;
                for (var i=0;i<_participantData.Participants.Length;i++)
                {
                    ParticipantData.Row[i].SummaryCells.Add(new ParticipantSummaryItemViewModel(_participantData.Participants[i][newColIndex]));
                }
                ParticipantData.ColHeaders.Add(ParticipantBaseModel.GetTrialArmDescription(_participantData.ColHeaders[newColIndex]));
            }
            ParticipantData.Row[newPos.x].SummaryCells[newPos.y].ParticipantIds = _participantData.Participants[newPos.x][newPos.y];
        }

        void _repository_ParticipantUpdated(object sender, ParticipantEventArgs e)
        {
            var move = _participantData.AlterParticipant(e.Participant.Id, e.Participant.TrialArm, ParticipantBaseModel.DataRequiredFunc(e.Participant));
            if (move.OldRow != move.NewRow)
            {
                int col = _participantData.ColIndex(e.Participant.TrialArm);
                ParticipantData.Row[move.OldRow].SummaryCells[col].ParticipantIds = _participantData.Participants[move.OldRow][col];
                ParticipantData.Row[move.NewRow].SummaryCells[col].ParticipantIds = _participantData.Participants[move.NewRow][col];
            }

        }

        ParticipantsSummary _participantData;
        public ParticipantSummaryViewModel ParticipantData { get; private set; }
        public ScreenedPatientsSummary ScreenedPatientData { get; private set; }
        ~DataSummaryViewModel()
        {
            _ageService.OnAgeIncrement -= OnNewAge;
        }
    }
    public class ParticipantSummaryViewModel : NotifyChangeBase
    {
        public ParticipantSummaryViewModel(ParticipantsSummary summary)
        {
            ColHeaders = new ObservableCollection<string>(summary.ColHeaders.Select(c => ParticipantBaseModel.GetTrialArmDescription(c)));
            Row = new ParticipantSummaryRowViewModel[summary.RowHeaders.Length];
            for (var i = 0; i < Row.Length;i++ )
            {
                Row[i] = new ParticipantSummaryRowViewModel
                {
                    RowHeader = DataRequiredStrings.GetDetails(summary.RowHeaders[i]),
                    SummaryCells = summary.Participants[i].Select(p => new ParticipantSummaryItemViewModel(p)).ToList()
                };
            }
        }
        public ParticipantSummaryRowViewModel[] Row { get; private set; }
        public ObservableCollection<string> ColHeaders { get; private set; } //???observablecollection
    }
    public class ParticipantSummaryRowViewModel
    {
        public string RowHeader { get; set; }
        public List<ParticipantSummaryItemViewModel> SummaryCells { get; set; } 
    }
    public class ParticipantSummaryItemViewModel : NotifyChangeBase
    {
        public ParticipantSummaryItemViewModel(ICollection<int> participantIds)
        {
            ParticipantIds = participantIds;
        }
        public ICollection<int> ParticipantIds 
        {
            set 
            {
                IdList = string.Join(",", value);
                Count = value.Count;
                NotifyPropertyChanged("IdList", "Count");
            }
        }
        public string IdList { get; private set; }
        public int Count { get; private set; }
    }
}
