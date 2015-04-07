using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System;
using BlowTrial.Domain.Tables;
using BlowTrial.Models;
using System.Windows.Media;

namespace BlowTrial.Infrastructure.Automapper
{
    public class ScreenProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ScreenedPatient, PatientDemographicsModel>()
                .ForMember(d=>d.DateOfBirth, opt=>opt.Ignore())
                .ForMember(d=>d.TimeOfBirth, opt=>opt.Ignore())
                .ForMember(d=>d.GestAgeDays, opt=>opt.Ignore())
                .ForMember(d=>d.GestAgeWeeks, opt=>opt.Ignore());
        }
    }

    public class PatientProfiles : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<VaccineAdministered, VaccineAdministeredModel>()
                .ForMember(d => d.VaccineGiven, o => o.MapFrom(d => d.VaccineGiven))
                .ForMember(d => d.AdministeredAtDateTime, o => o.MapFrom(s => s.AdministeredAt))
                .ForMember(d => d.AdministeredAtTime, o => o.Ignore())
                .ForMember(d => d.AdministeredAtDate, o => o.Ignore());

            Mapper.CreateMap<Participant, ParticipantBaseModel>()
                .Include<Participant, ParticipantProgressModel>()
                .ForMember(d => d.DeathOrLastContactDate, o => o.Ignore())
                .ForMember(d => d.DeathOrLastContactTime, o => o.Ignore())
                .ForMember(d => d.DischargeDate, o => o.Ignore())
                .ForMember(d => d.DischargeTime, o => o.Ignore())
                .ForMember(d => d.StudyCentre, o => o.Ignore())
                .ForMember(d => d.Becomes28On, o => o.Ignore())
                .ForMember(d => d.DataRequired, o => o.Ignore())
                .ForMember(d=>d.AgeDays, o=>o.MapFrom(s=> (DateTime.Now - s.DateTimeBirth).Days));

            Mapper.CreateMap<Participant, ParticipantProgressModel>()
                .ForMember(d=>d.VaccineModelsAdministered, o=>o.MapFrom(s=>s.VaccinesAdministered))
                .ForMember(d=>d.VaccinesAdministered, o=>o.Ignore())
                .AfterMap((s,d) => {
                    foreach (var v in d.VaccineModelsAdministered)
                    {
                        v.AdministeredTo = d;
                    }
                });

            Mapper.CreateMap<Participant, PatientDemographicsModel>()
                .ForMember(d => d.GestAgeBirth, o => o.MapFrom(s => s.GestAgeBirth))
                .ForMember(d => d.TimeOfBirth, o => o.Ignore())
                .ForMember(d => d.TimeOfEnrollment, o => o.Ignore())
                .ForMember(d => d.DateOfBirth, o => o.Ignore())
                .ForMember(d => d.DateOfEnrollment, o => o.Ignore())
                .ForMember(d => d.DateOfAdmission, o => o.Ignore())
                .ForMember(d => d.BadInfectnImmune, o => o.MapFrom(s=>false))
                .ForMember(d => d.BadMalform, o => o.MapFrom(s => false))
                .ForMember(d => d.RefusedConsent, o => o.MapFrom(s => false))
                .ForMember(d => d.WasGivenBcgPrior, o => o.MapFrom(s=>false))
                .ForMember(d => d.LikelyDie24Hr, o => o.MapFrom(s => false))
                .ForMember(d => d.Missed, o => o.MapFrom(s => false))
                .ForMember(d => d.DateTimeOfEnrollment, o => o.MapFrom(s=>s.RegisteredAt))
                .ForMember(d => d.EnvelopeNumber, o => o.MapFrom(s=>s.WasEnvelopeRandomised && s.Id<=EnvelopeDetails.MaxEnvelopeNumber?(int?)s.Id:null)) // because multiple siblings will have was envelope randomised =true;
                .ForMember(d => d.IsInborn, o => o.MapFrom(s=>s.Inborn))
                .ForMember(d => d.StudyCentre, o => o.Ignore())
                .ForMember(d => d.GestAgeWeeks, o => o.Ignore())
                .ForMember(d => d.GestAgeDays, o => o.Ignore())
                .ForMember(d => d.HasNoPhone, o=>o.MapFrom(s=>s.PhoneNumber==null));


            Mapper.CreateMap<Participant, ParticipantCsvModel>()
                .ForMember(d=>d.CauseOfDeathId, o=>o.MapFrom(s=>(int)s.CauseOfDeath))
                .ForMember(d => d.OutcomeAt28Id, o => o.MapFrom(s => (int)s.OutcomeAt28Days));

            Mapper.CreateMap<ScreenedPatient, ScreenedPatientCsvModel>();

            Mapper.CreateMap<ProtocolViolation, ProtocolViolationModel>();

            Mapper.CreateMap<StudyCentreModel, StudySiteItemModel>()
                .ForMember(d=>d.SiteBackgroundColour, o=>o.MapFrom(s=>((SolidColorBrush)s.BackgroundColour).Color))
                .ForMember(d => d.SiteTextColour, o => o.MapFrom(s => ((SolidColorBrush)s.TextColour).Color))
                .ForMember(d=>d.AllLocalSites, o=>o.Ignore())
                .ForMember(d=>d.SiteName, o=>o.MapFrom(s=>s.Name))
                .ForMember(d => d.MaxParticipantAllocations, o => o.MapFrom(s => s.MaxIdForSite - s.Id + (s.Id == 1 ? 2 : 1)))
                .ForMember(d=>d.IsToHospitalDischarge, o=>o.MapFrom(s=>s.RandomisedMessage.IsToHospitalDischarge))
                .ForMember(d => d.IsOpvInIntervention, o => o.MapFrom(s => s.RandomisedMessage.IsOpvInIntervention));
        }
    }
}