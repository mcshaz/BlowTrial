using BlowTrial.Domain.Outcomes;
using BlowTrial.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlowTrial.Infrastructure
{
    public enum DischargeFollowUp { Hospital, NICU }
    /// <summary>
    /// This exists because some sites have subtly different local ethics aproval
    /// </summary>
    public class RandomisingMessages
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isOpvInIntervention"></param>
        /// <param name="isToHospitalDischarge">if false, will be to NICU discharge</param>
        public RandomisingMessages(bool isOpvInIntervention, bool isToHospitalDischarge)
        {
            IsOpvInIntervention = isOpvInIntervention;
            IsToHospitalDischarge = isToHospitalDischarge;
            DischargeExplanation = string.Format(Strings.RandomisingMessages_DischargeFrom, 
                isToHospitalDischarge?Strings.RandomisingMessages_Hospital:Strings.RandomisingMessages_NICU);
            _controlInstructions = string.Format(Strings.RandomisingMessages_DefaultControl, DischargeExplanation);
            _interventionInstructions = isOpvInIntervention
                ?string.Format(Strings.RandomisingMessages_DefaultIntervention, "{0} " + Strings.And + ' ' + Strings.Vaccine_Opv)
                :Strings.RandomisingMessages_DefaultIntervention;
        }
        public bool IsOpvInIntervention { get; private set; }
        public bool IsToHospitalDischarge { get; private set; }
        string _interventionInstructions;
        string _controlInstructions;
        public string InstructionsFor(RandomisationArm arm)
        {
            string vaccineName = null;
            switch (arm)
            {
                case RandomisationArm.Control:
                    return _controlInstructions;
                case RandomisationArm.DanishBcg:
                    vaccineName = Strings.Vaccine_DanishBcg;
                    break;
                case RandomisationArm.MoreauBcg:
                    vaccineName = Strings.Vaccine_BcgBrazil;
                    break;
                case RandomisationArm.RussianBCG:
                    vaccineName = Strings.Vaccine_RussianBcg;
                    break;
                case RandomisationArm.GreenSignalBcg:
                    vaccineName = Strings.Vaccine_GreenSignalBcg;
                    break;
                case RandomisationArm.JapanBcg:
                    vaccineName = Strings.Vaccine_BcgJapan;
                    break;
                default:
                    throw new ArgumentException("invalid value for enum RandomisationArm");
            }
            return string.Format(_interventionInstructions, vaccineName);
        }

        public string DischargeExplanation { get; private set; }
    }
}
