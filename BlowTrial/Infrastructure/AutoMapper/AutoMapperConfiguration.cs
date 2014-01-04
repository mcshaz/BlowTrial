using AutoMapper;
using BlowTrial.Infrastructure.Automapper;
using System;
using System.Diagnostics;

namespace BlowTrial.Models
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {

                x.AddProfile<PatientProfiles>();

            });
#if DEBUG
            try
            {
                Mapper.AssertConfigurationIsValid();
            }
            catch(AutoMapperConfigurationException e)
            {
                Debug.Fail(e.Message);
                throw;
            }
            catch(Exception)
            {
                throw;
            }
#endif
        }
    }
}