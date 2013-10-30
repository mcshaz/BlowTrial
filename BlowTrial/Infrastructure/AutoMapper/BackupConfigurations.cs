using AutoMapper;
using BlowTrial.Domain.Tables;
using BlowTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure.AutoMapper
{
    class BackupConfigurations : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<AppData, CloudDirectoryModel>()
                .ForMember(d => d.Error, o => o.Ignore())
                .ForMember(d => d.IsValid, o => o.Ignore());
            Mapper.CreateMap<BackupService, CloudDirectoryModel>()
                .ForMember(d => d.Error, o => o.Ignore())
                .ForMember(d => d.IsValid, o => o.Ignore())
                .ForMember(d => d.BackupIntervalMinutes, o => o.MapFrom(s => s.IntervalMins))
                .ForMember(d => d.CloudDirectory, o => o.MapFrom(s => s.Directory))
                .ForMember(d => d.BackupToCloud, o => o.MapFrom(s => s.IsToBackup));
        }
    }
}
