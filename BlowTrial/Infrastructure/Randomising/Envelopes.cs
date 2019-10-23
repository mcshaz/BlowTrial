using BlowTrial.Domain.Outcomes;
using BlowTrial.Infrastructure.Randomising;
using System.Collections.Generic;
using System.Linq;
namespace BlowTrial.Infrastructure
{
    public class Envelope
    {
	    public int WeightLessThan {get; set;}
	    public bool IsMale {get; set;}
	    public bool IsInterventionArm {get; set;}
        public int BlockSize { get; set; }
        public int BlockNumber { get; set; }

        public RandomisationStrata RandomisationCategory
        {
            get
            {
                int returnInt;
                if (WeightLessThan == Engine.BlockWeight1)
                {
                    returnInt = 1;
                }
                else if (WeightLessThan == Engine.BlockWeight2)
                {
                    returnInt = 3;
                }
                else
                {
                    returnInt = 5;
                }
                if (!IsMale)
                {
                    returnInt++;
                }
                return (RandomisationStrata)returnInt;
            }
        }
        /*
        internal const string UpdateCategoriesSql = "Update [Participants] Set [RandomisationCategory] = 1 Where [Id] BETWEEN '1' AND '20';" +
                "Update [Participants] Set [RandomisationCategory] = 2 Where [Id] BETWEEN '1011' AND '1030';" +
                "Update [Participants] Set [RandomisationCategory] = 3 Where [Id] BETWEEN '2021' AND '2081';" +
                "Update [Participants] Set [RandomisationCategory] = 4 Where [Id] BETWEEN '3029' AND '3089';" +
                "Update [Participants] Set [RandomisationCategory] = 5 Where [Id] BETWEEN '4037' AND '4275';" +
                "Update [Participants] Set [RandomisationCategory] = 6 Where [Id] BETWEEN '5045' AND '5283';";
         * */
    }
    public static class EnvelopeDetails
    {
        public static ILookup<RandomisationStrata, int> GetAllEnvelopeNumbersByStrata()
        {
            var allEnvelopes = new List<KeyValuePair<int,Envelope>>(400);
            for (int i=1;i<=MaxEnvelopeNumber;i++)
            {
                Envelope e = GetEnvelope(i);
                if (e!=null)
                {
                    allEnvelopes.Add(new KeyValuePair<int,Envelope>(i,e));
                }
            }
            return allEnvelopes.ToLookup(p => p.Value.RandomisationCategory,e=>e.Key);
        }
        public static IDictionary<int, RandomisationStrata> GetStrataByBlockNumber()
        {
            // var allEnvelopes = new List<Envelope>(400);
            int lastBlockNumber = 0;
            var returnVar = new Dictionary<int, RandomisationStrata>();
            for (int i = 1; i <= MaxEnvelopeNumber; i++)
            {
                Envelope e = GetEnvelope(i);
                if (e != null && lastBlockNumber != e.BlockNumber)
                {
                    lastBlockNumber = e.BlockNumber;
                    returnVar.Add(e.BlockNumber, e.RandomisationCategory);
                }
            }
            return returnVar;
        }
        public static HashSet<int> GetAllEnvelopeNumbers()
        {
            var returnVar = new HashSet<int>();
            for (int i = 1; i <= MaxEnvelopeNumber; i++)
            {
                if (GetEnvelope(i) != null)
                {
                    returnVar.Add(i);
                }
            }
            return returnVar;
        }
        public const int MaxEnvelopeNumber = 5283;
        public const int FirstAvailableBlockNumber = 1752;
        public static Envelope GetEnvelope(int Id)
        {
            switch (Id)
            {
                case 1:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 1, BlockSize = 4 };
                case 2:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 1, BlockSize = 4 };
                case 3:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 1, BlockSize = 4 };
                case 4:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 1, BlockSize = 4 };
                case 5:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 2, BlockSize = 2 };
                case 6:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 2, BlockSize = 2 };
                case 7:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 3, BlockSize = 4 };
                case 8:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 3, BlockSize = 4 };
                case 9:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 3, BlockSize = 4 };
                case 10:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 3, BlockSize = 4 };
                case 11:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 4, BlockSize = 2 };
                case 12:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 4, BlockSize = 2 };
                case 13:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 5, BlockSize = 2 };
                case 14:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 5, BlockSize = 2 };
                case 15:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 6, BlockSize = 2 };
                case 16:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 6, BlockSize = 2 };
                case 17:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 7, BlockSize = 2 };
                case 18:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 7, BlockSize = 2 };
                case 19:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = false, BlockNumber = 8, BlockSize = 4 };
                case 20:
                    return new Envelope { WeightLessThan = 1000, IsMale = true, IsInterventionArm = true, BlockNumber = 8, BlockSize = 4 };
                case 1011:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 337, BlockSize = 2 };
                case 1012:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 337, BlockSize = 2 };
                case 1013:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 338, BlockSize = 4 };
                case 1014:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 338, BlockSize = 4 };
                case 1015:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 338, BlockSize = 4 };
                case 1016:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 338, BlockSize = 4 };
                case 1017:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 339, BlockSize = 4 };
                case 1018:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 339, BlockSize = 4 };
                case 1019:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 339, BlockSize = 4 };
                case 1020:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 339, BlockSize = 4 };
                case 1021:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 340, BlockSize = 2 };
                case 1022:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 340, BlockSize = 2 };
                case 1023:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 341, BlockSize = 2 };
                case 1024:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 341, BlockSize = 2 };
                case 1025:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 342, BlockSize = 4 };
                case 1026:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 342, BlockSize = 4 };
                case 1027:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 342, BlockSize = 4 };
                case 1028:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 342, BlockSize = 4 };
                case 1029:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = false, BlockNumber = 343, BlockSize = 2 };
                case 1030:
                    return new Envelope { WeightLessThan = 1000, IsMale = false, IsInterventionArm = true, BlockNumber = 343, BlockSize = 2 };
                case 2021:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 664, BlockSize = 2 };
                case 2022:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 664, BlockSize = 2 };
                case 2023:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 665, BlockSize = 2 };
                case 2024:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 665, BlockSize = 2 };
                case 2025:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 666, BlockSize = 4 };
                case 2026:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 666, BlockSize = 4 };
                case 2027:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 666, BlockSize = 4 };
                case 2028:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 666, BlockSize = 4 };
                case 2029:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 667, BlockSize = 4 };
                case 2030:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 667, BlockSize = 4 };
                case 2031:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 667, BlockSize = 4 };
                case 2032:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 667, BlockSize = 4 };
                case 2033:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 668, BlockSize = 2 };
                case 2034:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 668, BlockSize = 2 };
                case 2035:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 669, BlockSize = 4 };
                case 2036:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 669, BlockSize = 4 };
                case 2037:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 669, BlockSize = 4 };
                case 2038:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 669, BlockSize = 4 };
                case 2039:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 670, BlockSize = 4 };
                case 2040:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 670, BlockSize = 4 };
                case 2041:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 670, BlockSize = 4 };
                case 2042:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 670, BlockSize = 4 };
                case 2043:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 671, BlockSize = 2 };
                case 2044:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 671, BlockSize = 2 };
                case 2045:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 672, BlockSize = 2 };
                case 2046:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 672, BlockSize = 2 };
                case 2047:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 673, BlockSize = 4 };
                case 2048:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 673, BlockSize = 4 };
                case 2049:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 673, BlockSize = 4 };
                case 2050:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 673, BlockSize = 4 };
                case 2051:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 674, BlockSize = 4 };
                case 2052:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 674, BlockSize = 4 };
                case 2053:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 674, BlockSize = 4 };
                case 2054:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 674, BlockSize = 4 };
                case 2055:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 675, BlockSize = 4 };
                case 2056:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 675, BlockSize = 4 };
                case 2057:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 675, BlockSize = 4 };
                case 2058:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 675, BlockSize = 4 };
                case 2059:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 676, BlockSize = 2 };
                case 2060:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 676, BlockSize = 2 };
                case 2061:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 677, BlockSize = 2 };
                case 2062:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 677, BlockSize = 2 };
                case 2063:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 678, BlockSize = 4 };
                case 2064:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 678, BlockSize = 4 };
                case 2065:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 678, BlockSize = 4 };
                case 2066:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 678, BlockSize = 4 };
                case 2067:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 679, BlockSize = 2 };
                case 2068:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 679, BlockSize = 2 };
                case 2069:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 680, BlockSize = 2 };
                case 2070:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 680, BlockSize = 2 };
                case 2071:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 681, BlockSize = 2 };
                case 2072:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 681, BlockSize = 2 };
                case 2073:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 682, BlockSize = 2 };
                case 2074:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 682, BlockSize = 2 };
                case 2075:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 683, BlockSize = 4 };
                case 2076:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 683, BlockSize = 4 };
                case 2077:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 683, BlockSize = 4 };
                case 2078:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 683, BlockSize = 4 };
                case 2079:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 684, BlockSize = 2 };
                case 2080:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = false, BlockNumber = 684, BlockSize = 2 };
                case 2081:
                    return new Envelope { WeightLessThan = 1500, IsMale = true, IsInterventionArm = true, BlockNumber = 685, BlockSize = 2 };
                case 3029:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1010, BlockSize = 4 };
                case 3030:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1010, BlockSize = 4 };
                case 3031:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1010, BlockSize = 4 };
                case 3032:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1010, BlockSize = 4 };
                case 3033:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1011, BlockSize = 2 };
                case 3034:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1011, BlockSize = 2 };
                case 3035:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1012, BlockSize = 2 };
                case 3036:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1012, BlockSize = 2 };
                case 3037:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1013, BlockSize = 4 };
                case 3038:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1013, BlockSize = 4 };
                case 3039:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1013, BlockSize = 4 };
                case 3040:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1013, BlockSize = 4 };
                case 3041:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1014, BlockSize = 2 };
                case 3042:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1014, BlockSize = 2 };
                case 3043:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1015, BlockSize = 4 };
                case 3044:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1015, BlockSize = 4 };
                case 3045:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1015, BlockSize = 4 };
                case 3046:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1015, BlockSize = 4 };
                case 3047:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1016, BlockSize = 4 };
                case 3048:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1016, BlockSize = 4 };
                case 3049:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1016, BlockSize = 4 };
                case 3050:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1016, BlockSize = 4 };
                case 3051:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1017, BlockSize = 4 };
                case 3052:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1017, BlockSize = 4 };
                case 3053:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1017, BlockSize = 4 };
                case 3054:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1017, BlockSize = 4 };
                case 3055:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1018, BlockSize = 4 };
                case 3056:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1018, BlockSize = 4 };
                case 3057:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1018, BlockSize = 4 };
                case 3058:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1018, BlockSize = 4 };
                case 3059:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1019, BlockSize = 2 };
                case 3060:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1019, BlockSize = 2 };
                case 3061:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1020, BlockSize = 4 };
                case 3062:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1020, BlockSize = 4 };
                case 3063:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1020, BlockSize = 4 };
                case 3064:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1020, BlockSize = 4 };
                case 3065:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1021, BlockSize = 2 };
                case 3066:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1021, BlockSize = 2 };
                case 3067:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1022, BlockSize = 4 };
                case 3068:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1022, BlockSize = 4 };
                case 3069:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1022, BlockSize = 4 };
                case 3070:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1022, BlockSize = 4 };
                case 3071:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1023, BlockSize = 4 };
                case 3072:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1023, BlockSize = 4 };
                case 3073:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1023, BlockSize = 4 };
                case 3074:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1023, BlockSize = 4 };
                case 3075:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1024, BlockSize = 2 };
                case 3076:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1024, BlockSize = 2 };
                case 3077:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1025, BlockSize = 2 };
                case 3078:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1025, BlockSize = 2 };
                case 3079:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1026, BlockSize = 2 };
                case 3080:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1026, BlockSize = 2 };
                case 3081:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1027, BlockSize = 4 };
                case 3082:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1027, BlockSize = 4 };
                case 3083:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1027, BlockSize = 4 };
                case 3084:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1027, BlockSize = 4 };
                case 3085:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1028, BlockSize = 4 };
                case 3086:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = false, BlockNumber = 1028, BlockSize = 4 };
                case 3087:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1028, BlockSize = 4 };
                case 3088:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1028, BlockSize = 4 };
                case 3089:
                    return new Envelope { WeightLessThan = 1500, IsMale = false, IsInterventionArm = true, BlockNumber = 1029, BlockSize = 4 };
                case 4037:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1340, BlockSize = 2 };
                case 4038:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1340, BlockSize = 2 };
                case 4039:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1341, BlockSize = 2 };
                case 4040:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1341, BlockSize = 2 };
                case 4041:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1342, BlockSize = 2 };
                case 4042:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1342, BlockSize = 2 };
                case 4043:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1343, BlockSize = 2 };
                case 4044:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1343, BlockSize = 2 };
                case 4045:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1344, BlockSize = 4 };
                case 4046:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1344, BlockSize = 4 };
                case 4047:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1344, BlockSize = 4 };
                case 4048:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1344, BlockSize = 4 };
                case 4049:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1345, BlockSize = 2 };
                case 4050:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1345, BlockSize = 2 };
                case 4051:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1346, BlockSize = 2 };
                case 4052:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1346, BlockSize = 2 };
                case 4053:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1347, BlockSize = 4 };
                case 4054:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1347, BlockSize = 4 };
                case 4055:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1347, BlockSize = 4 };
                case 4056:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1347, BlockSize = 4 };
                case 4057:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1348, BlockSize = 2 };
                case 4058:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1348, BlockSize = 2 };
                case 4059:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1349, BlockSize = 4 };
                case 4060:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1349, BlockSize = 4 };
                case 4061:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1349, BlockSize = 4 };
                case 4062:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1349, BlockSize = 4 };
                case 4063:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1350, BlockSize = 2 };
                case 4064:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1350, BlockSize = 2 };
                case 4065:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1351, BlockSize = 4 };
                case 4066:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1351, BlockSize = 4 };
                case 4067:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1351, BlockSize = 4 };
                case 4068:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1351, BlockSize = 4 };
                case 4069:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1352, BlockSize = 4 };
                case 4070:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1352, BlockSize = 4 };
                case 4071:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1352, BlockSize = 4 };
                case 4072:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1352, BlockSize = 4 };
                case 4073:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1353, BlockSize = 4 };
                case 4074:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1353, BlockSize = 4 };
                case 4075:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1353, BlockSize = 4 };
                case 4076:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1353, BlockSize = 4 };
                case 4077:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1354, BlockSize = 4 };
                case 4078:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1354, BlockSize = 4 };
                case 4079:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1354, BlockSize = 4 };
                case 4080:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1354, BlockSize = 4 };
                case 4081:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1355, BlockSize = 2 };
                case 4082:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1355, BlockSize = 2 };
                case 4083:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1356, BlockSize = 2 };
                case 4084:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1356, BlockSize = 2 };
                case 4085:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1357, BlockSize = 2 };
                case 4086:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1357, BlockSize = 2 };
                case 4087:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1358, BlockSize = 2 };
                case 4088:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1358, BlockSize = 2 };
                case 4089:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1359, BlockSize = 2 };
                case 4090:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1359, BlockSize = 2 };
                case 4091:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1360, BlockSize = 2 };
                case 4092:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1360, BlockSize = 2 };
                case 4093:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1361, BlockSize = 2 };
                case 4094:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1361, BlockSize = 2 };
                case 4095:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1362, BlockSize = 2 };
                case 4096:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1362, BlockSize = 2 };
                case 4097:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1363, BlockSize = 2 };
                case 4098:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1363, BlockSize = 2 };
                case 4099:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1364, BlockSize = 4 };
                case 4100:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1364, BlockSize = 4 };
                case 4101:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1364, BlockSize = 4 };
                case 4102:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1364, BlockSize = 4 };
                case 4103:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1365, BlockSize = 2 };
                case 4104:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1365, BlockSize = 2 };
                case 4105:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1366, BlockSize = 2 };
                case 4106:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1366, BlockSize = 2 };
                case 4107:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1367, BlockSize = 4 };
                case 4108:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1367, BlockSize = 4 };
                case 4109:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1367, BlockSize = 4 };
                case 4110:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1367, BlockSize = 4 };
                case 4111:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1368, BlockSize = 4 };
                case 4112:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1368, BlockSize = 4 };
                case 4113:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1368, BlockSize = 4 };
                case 4114:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1368, BlockSize = 4 };
                case 4115:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1369, BlockSize = 4 };
                case 4116:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1369, BlockSize = 4 };
                case 4117:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1369, BlockSize = 4 };
                case 4118:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1369, BlockSize = 4 };
                case 4119:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1370, BlockSize = 4 };
                case 4120:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1370, BlockSize = 4 };
                case 4121:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1370, BlockSize = 4 };
                case 4122:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1370, BlockSize = 4 };
                case 4123:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1371, BlockSize = 2 };
                case 4124:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1371, BlockSize = 2 };
                case 4125:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1372, BlockSize = 4 };
                case 4126:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1372, BlockSize = 4 };
                case 4127:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1372, BlockSize = 4 };
                case 4128:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1372, BlockSize = 4 };
                case 4129:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1373, BlockSize = 4 };
                case 4130:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1373, BlockSize = 4 };
                case 4131:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1373, BlockSize = 4 };
                case 4132:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1373, BlockSize = 4 };
                case 4133:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1374, BlockSize = 2 };
                case 4134:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1374, BlockSize = 2 };
                case 4135:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1375, BlockSize = 4 };
                case 4136:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1375, BlockSize = 4 };
                case 4137:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1375, BlockSize = 4 };
                case 4138:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1375, BlockSize = 4 };
                case 4139:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1376, BlockSize = 4 };
                case 4140:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1376, BlockSize = 4 };
                case 4141:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1376, BlockSize = 4 };
                case 4142:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1376, BlockSize = 4 };
                case 4143:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1377, BlockSize = 2 };
                case 4144:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1377, BlockSize = 2 };
                case 4145:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1378, BlockSize = 2 };
                case 4146:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1378, BlockSize = 2 };
                case 4147:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1379, BlockSize = 4 };
                case 4148:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1379, BlockSize = 4 };
                case 4149:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1379, BlockSize = 4 };
                case 4150:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1379, BlockSize = 4 };
                case 4151:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1380, BlockSize = 2 };
                case 4152:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1380, BlockSize = 2 };
                case 4153:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1381, BlockSize = 4 };
                case 4154:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1381, BlockSize = 4 };
                case 4155:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1381, BlockSize = 4 };
                case 4156:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1381, BlockSize = 4 };
                case 4157:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1382, BlockSize = 4 };
                case 4158:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1382, BlockSize = 4 };
                case 4159:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1382, BlockSize = 4 };
                case 4160:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1382, BlockSize = 4 };
                case 4161:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1383, BlockSize = 2 };
                case 4162:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1383, BlockSize = 2 };
                case 4163:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1384, BlockSize = 4 };
                case 4164:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1384, BlockSize = 4 };
                case 4165:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1384, BlockSize = 4 };
                case 4166:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1384, BlockSize = 4 };
                case 4167:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1385, BlockSize = 4 };
                case 4168:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1385, BlockSize = 4 };
                case 4169:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1385, BlockSize = 4 };
                case 4170:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1385, BlockSize = 4 };
                case 4171:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1386, BlockSize = 4 };
                case 4172:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1386, BlockSize = 4 };
                case 4173:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1386, BlockSize = 4 };
                case 4174:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1386, BlockSize = 4 };
                case 4175:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1387, BlockSize = 4 };
                case 4176:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1387, BlockSize = 4 };
                case 4177:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1387, BlockSize = 4 };
                case 4178:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1387, BlockSize = 4 };
                case 4179:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1388, BlockSize = 2 };
                case 4180:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1388, BlockSize = 2 };
                case 4181:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1389, BlockSize = 2 };
                case 4182:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1389, BlockSize = 2 };
                case 4183:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1390, BlockSize = 2 };
                case 4184:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1390, BlockSize = 2 };
                case 4185:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1391, BlockSize = 2 };
                case 4186:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1391, BlockSize = 2 };
                case 4187:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1392, BlockSize = 2 };
                case 4188:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1392, BlockSize = 2 };
                case 4189:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1393, BlockSize = 2 };
                case 4190:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1393, BlockSize = 2 };
                case 4191:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1394, BlockSize = 4 };
                case 4192:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1394, BlockSize = 4 };
                case 4193:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1394, BlockSize = 4 };
                case 4194:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1394, BlockSize = 4 };
                case 4195:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1395, BlockSize = 2 };
                case 4196:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1395, BlockSize = 2 };
                case 4197:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1396, BlockSize = 4 };
                case 4198:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1396, BlockSize = 4 };
                case 4199:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1396, BlockSize = 4 };
                case 4200:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1396, BlockSize = 4 };
                case 4201:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1397, BlockSize = 2 };
                case 4202:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1397, BlockSize = 2 };
                case 4203:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1398, BlockSize = 2 };
                case 4204:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1398, BlockSize = 2 };
                case 4205:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1399, BlockSize = 2 };
                case 4206:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1399, BlockSize = 2 };
                case 4207:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1400, BlockSize = 4 };
                case 4208:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1400, BlockSize = 4 };
                case 4209:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1400, BlockSize = 4 };
                case 4210:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1400, BlockSize = 4 };
                case 4211:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1401, BlockSize = 2 };
                case 4212:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1401, BlockSize = 2 };
                case 4213:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1402, BlockSize = 2 };
                case 4214:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1402, BlockSize = 2 };
                case 4215:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1403, BlockSize = 4 };
                case 4216:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1403, BlockSize = 4 };
                case 4217:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1403, BlockSize = 4 };
                case 4218:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1403, BlockSize = 4 };
                case 4219:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1404, BlockSize = 2 };
                case 4220:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1404, BlockSize = 2 };
                case 4221:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1405, BlockSize = 2 };
                case 4222:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1405, BlockSize = 2 };
                case 4223:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1406, BlockSize = 4 };
                case 4224:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1406, BlockSize = 4 };
                case 4225:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1406, BlockSize = 4 };
                case 4226:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1406, BlockSize = 4 };
                case 4227:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1407, BlockSize = 4 };
                case 4228:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1407, BlockSize = 4 };
                case 4229:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1407, BlockSize = 4 };
                case 4230:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1407, BlockSize = 4 };
                case 4231:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1408, BlockSize = 4 };
                case 4232:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1408, BlockSize = 4 };
                case 4233:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1408, BlockSize = 4 };
                case 4234:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1408, BlockSize = 4 };
                case 4235:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1409, BlockSize = 4 };
                case 4236:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1409, BlockSize = 4 };
                case 4237:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1409, BlockSize = 4 };
                case 4238:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1409, BlockSize = 4 };
                case 4239:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1410, BlockSize = 4 };
                case 4240:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1410, BlockSize = 4 };
                case 4241:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1410, BlockSize = 4 };
                case 4242:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1410, BlockSize = 4 };
                case 4243:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1411, BlockSize = 4 };
                case 4244:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1411, BlockSize = 4 };
                case 4245:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1411, BlockSize = 4 };
                case 4246:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1411, BlockSize = 4 };
                case 4247:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1412, BlockSize = 2 };
                case 4248:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1412, BlockSize = 2 };
                case 4249:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1413, BlockSize = 4 };
                case 4250:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1413, BlockSize = 4 };
                case 4251:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1413, BlockSize = 4 };
                case 4252:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1413, BlockSize = 4 };
                case 4253:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1414, BlockSize = 2 };
                case 4254:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1414, BlockSize = 2 };
                case 4255:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1415, BlockSize = 4 };
                case 4256:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1415, BlockSize = 4 };
                case 4257:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1415, BlockSize = 4 };
                case 4258:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1415, BlockSize = 4 };
                case 4259:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1416, BlockSize = 2 };
                case 4260:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1416, BlockSize = 2 };
                case 4261:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1417, BlockSize = 4 };
                case 4262:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1417, BlockSize = 4 };
                case 4263:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1417, BlockSize = 4 };
                case 4264:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1417, BlockSize = 4 };
                case 4265:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1418, BlockSize = 2 };
                case 4266:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1418, BlockSize = 2 };
                case 4267:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1419, BlockSize = 2 };
                case 4268:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1419, BlockSize = 2 };
                case 4269:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1420, BlockSize = 2 };
                case 4270:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1420, BlockSize = 2 };
                case 4271:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1421, BlockSize = 4 };
                case 4272:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1421, BlockSize = 4 };
                case 4273:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = false, BlockNumber = 1421, BlockSize = 4 };
                case 4274:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1421, BlockSize = 4 };
                case 4275:
                    return new Envelope { WeightLessThan = 2000, IsMale = true, IsInterventionArm = true, BlockNumber = 1422, BlockSize = 4 };
                case 5045:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1671, BlockSize = 2 };
                case 5046:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1671, BlockSize = 2 };
                case 5047:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1672, BlockSize = 4 };
                case 5048:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1672, BlockSize = 4 };
                case 5049:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1672, BlockSize = 4 };
                case 5050:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1672, BlockSize = 4 };
                case 5051:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1673, BlockSize = 4 };
                case 5052:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1673, BlockSize = 4 };
                case 5053:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1673, BlockSize = 4 };
                case 5054:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1673, BlockSize = 4 };
                case 5055:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1674, BlockSize = 2 };
                case 5056:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1674, BlockSize = 2 };
                case 5057:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1675, BlockSize = 4 };
                case 5058:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1675, BlockSize = 4 };
                case 5059:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1675, BlockSize = 4 };
                case 5060:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1675, BlockSize = 4 };
                case 5061:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1676, BlockSize = 2 };
                case 5062:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1676, BlockSize = 2 };
                case 5063:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1677, BlockSize = 2 };
                case 5064:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1677, BlockSize = 2 };
                case 5065:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1678, BlockSize = 4 };
                case 5066:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1678, BlockSize = 4 };
                case 5067:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1678, BlockSize = 4 };
                case 5068:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1678, BlockSize = 4 };
                case 5069:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1679, BlockSize = 4 };
                case 5070:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1679, BlockSize = 4 };
                case 5071:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1679, BlockSize = 4 };
                case 5072:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1679, BlockSize = 4 };
                case 5073:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1680, BlockSize = 2 };
                case 5074:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1680, BlockSize = 2 };
                case 5075:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1681, BlockSize = 2 };
                case 5076:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1681, BlockSize = 2 };
                case 5077:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1682, BlockSize = 2 };
                case 5078:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1682, BlockSize = 2 };
                case 5079:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1683, BlockSize = 4 };
                case 5080:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1683, BlockSize = 4 };
                case 5081:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1683, BlockSize = 4 };
                case 5082:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1683, BlockSize = 4 };
                case 5083:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1684, BlockSize = 4 };
                case 5084:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1684, BlockSize = 4 };
                case 5085:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1684, BlockSize = 4 };
                case 5086:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1684, BlockSize = 4 };
                case 5087:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1685, BlockSize = 4 };
                case 5088:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1685, BlockSize = 4 };
                case 5089:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1685, BlockSize = 4 };
                case 5090:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1685, BlockSize = 4 };
                case 5091:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1686, BlockSize = 2 };
                case 5092:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1686, BlockSize = 2 };
                case 5093:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1687, BlockSize = 2 };
                case 5094:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1687, BlockSize = 2 };
                case 5095:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1688, BlockSize = 2 };
                case 5096:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1688, BlockSize = 2 };
                case 5097:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1689, BlockSize = 2 };
                case 5098:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1689, BlockSize = 2 };
                case 5099:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1690, BlockSize = 4 };
                case 5100:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1690, BlockSize = 4 };
                case 5101:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1690, BlockSize = 4 };
                case 5102:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1690, BlockSize = 4 };
                case 5103:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1691, BlockSize = 2 };
                case 5104:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1691, BlockSize = 2 };
                case 5105:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1692, BlockSize = 4 };
                case 5106:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1692, BlockSize = 4 };
                case 5107:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1692, BlockSize = 4 };
                case 5108:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1692, BlockSize = 4 };
                case 5109:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1693, BlockSize = 4 };
                case 5110:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1693, BlockSize = 4 };
                case 5111:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1693, BlockSize = 4 };
                case 5112:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1693, BlockSize = 4 };
                case 5113:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1694, BlockSize = 4 };
                case 5114:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1694, BlockSize = 4 };
                case 5115:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1694, BlockSize = 4 };
                case 5116:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1694, BlockSize = 4 };
                case 5117:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1695, BlockSize = 4 };
                case 5118:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1695, BlockSize = 4 };
                case 5119:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1695, BlockSize = 4 };
                case 5120:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1695, BlockSize = 4 };
                case 5121:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1696, BlockSize = 2 };
                case 5122:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1696, BlockSize = 2 };
                case 5123:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1697, BlockSize = 4 };
                case 5124:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1697, BlockSize = 4 };
                case 5125:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1697, BlockSize = 4 };
                case 5126:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1697, BlockSize = 4 };
                case 5127:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1698, BlockSize = 2 };
                case 5128:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1698, BlockSize = 2 };
                case 5129:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1699, BlockSize = 4 };
                case 5130:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1699, BlockSize = 4 };
                case 5131:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1699, BlockSize = 4 };
                case 5132:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1699, BlockSize = 4 };
                case 5133:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1700, BlockSize = 2 };
                case 5134:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1700, BlockSize = 2 };
                case 5135:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1701, BlockSize = 4 };
                case 5136:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1701, BlockSize = 4 };
                case 5137:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1701, BlockSize = 4 };
                case 5138:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1701, BlockSize = 4 };
                case 5139:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1702, BlockSize = 2 };
                case 5140:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1702, BlockSize = 2 };
                case 5141:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1703, BlockSize = 4 };
                case 5142:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1703, BlockSize = 4 };
                case 5143:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1703, BlockSize = 4 };
                case 5144:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1703, BlockSize = 4 };
                case 5145:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1704, BlockSize = 2 };
                case 5146:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1704, BlockSize = 2 };
                case 5147:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1705, BlockSize = 2 };
                case 5148:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1705, BlockSize = 2 };
                case 5149:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1706, BlockSize = 2 };
                case 5150:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1706, BlockSize = 2 };
                case 5151:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1707, BlockSize = 2 };
                case 5152:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1707, BlockSize = 2 };
                case 5153:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1708, BlockSize = 2 };
                case 5154:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1708, BlockSize = 2 };
                case 5155:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1709, BlockSize = 2 };
                case 5156:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1709, BlockSize = 2 };
                case 5157:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1710, BlockSize = 4 };
                case 5158:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1710, BlockSize = 4 };
                case 5159:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1710, BlockSize = 4 };
                case 5160:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1710, BlockSize = 4 };
                case 5161:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1711, BlockSize = 2 };
                case 5162:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1711, BlockSize = 2 };
                case 5163:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1712, BlockSize = 2 };
                case 5164:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1712, BlockSize = 2 };
                case 5165:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1713, BlockSize = 4 };
                case 5166:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1713, BlockSize = 4 };
                case 5167:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1713, BlockSize = 4 };
                case 5168:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1713, BlockSize = 4 };
                case 5169:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1714, BlockSize = 2 };
                case 5170:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1714, BlockSize = 2 };
                case 5171:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1715, BlockSize = 4 };
                case 5172:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1715, BlockSize = 4 };
                case 5173:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1715, BlockSize = 4 };
                case 5174:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1715, BlockSize = 4 };
                case 5175:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1716, BlockSize = 2 };
                case 5176:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1716, BlockSize = 2 };
                case 5177:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1717, BlockSize = 4 };
                case 5178:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1717, BlockSize = 4 };
                case 5179:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1717, BlockSize = 4 };
                case 5180:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1717, BlockSize = 4 };
                case 5181:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1718, BlockSize = 4 };
                case 5182:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1718, BlockSize = 4 };
                case 5183:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1718, BlockSize = 4 };
                case 5184:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1718, BlockSize = 4 };
                case 5185:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1719, BlockSize = 4 };
                case 5186:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1719, BlockSize = 4 };
                case 5187:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1719, BlockSize = 4 };
                case 5188:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1719, BlockSize = 4 };
                case 5189:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1720, BlockSize = 4 };
                case 5190:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1720, BlockSize = 4 };
                case 5191:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1720, BlockSize = 4 };
                case 5192:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1720, BlockSize = 4 };
                case 5193:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1721, BlockSize = 2 };
                case 5194:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1721, BlockSize = 2 };
                case 5195:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1722, BlockSize = 2 };
                case 5196:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1722, BlockSize = 2 };
                case 5197:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1723, BlockSize = 2 };
                case 5198:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1723, BlockSize = 2 };
                case 5199:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1724, BlockSize = 2 };
                case 5200:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1724, BlockSize = 2 };
                case 5201:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1725, BlockSize = 2 };
                case 5202:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1725, BlockSize = 2 };
                case 5203:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1726, BlockSize = 4 };
                case 5204:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1726, BlockSize = 4 };
                case 5205:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1726, BlockSize = 4 };
                case 5206:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1726, BlockSize = 4 };
                case 5207:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1727, BlockSize = 4 };
                case 5208:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1727, BlockSize = 4 };
                case 5209:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1727, BlockSize = 4 };
                case 5210:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1727, BlockSize = 4 };
                case 5211:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1728, BlockSize = 4 };
                case 5212:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1728, BlockSize = 4 };
                case 5213:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1728, BlockSize = 4 };
                case 5214:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1728, BlockSize = 4 };
                case 5215:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1729, BlockSize = 4 };
                case 5216:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1729, BlockSize = 4 };
                case 5217:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1729, BlockSize = 4 };
                case 5218:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1729, BlockSize = 4 };
                case 5219:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1730, BlockSize = 4 };
                case 5220:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1730, BlockSize = 4 };
                case 5221:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1730, BlockSize = 4 };
                case 5222:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1730, BlockSize = 4 };
                case 5223:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1731, BlockSize = 4 };
                case 5224:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1731, BlockSize = 4 };
                case 5225:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1731, BlockSize = 4 };
                case 5226:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1731, BlockSize = 4 };
                case 5227:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1732, BlockSize = 4 };
                case 5228:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1732, BlockSize = 4 };
                case 5229:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1732, BlockSize = 4 };
                case 5230:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1732, BlockSize = 4 };
                case 5231:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1733, BlockSize = 2 };
                case 5232:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1733, BlockSize = 2 };
                case 5233:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1734, BlockSize = 4 };
                case 5234:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1734, BlockSize = 4 };
                case 5235:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1734, BlockSize = 4 };
                case 5236:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1734, BlockSize = 4 };
                case 5237:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1735, BlockSize = 4 };
                case 5238:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1735, BlockSize = 4 };
                case 5239:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1735, BlockSize = 4 };
                case 5240:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1735, BlockSize = 4 };
                case 5241:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1736, BlockSize = 4 };
                case 5242:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1736, BlockSize = 4 };
                case 5243:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1736, BlockSize = 4 };
                case 5244:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1736, BlockSize = 4 };
                case 5245:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1737, BlockSize = 2 };
                case 5246:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1737, BlockSize = 2 };
                case 5247:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1738, BlockSize = 2 };
                case 5248:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1738, BlockSize = 2 };
                case 5249:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1739, BlockSize = 2 };
                case 5250:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1739, BlockSize = 2 };
                case 5251:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1740, BlockSize = 2 };
                case 5252:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1740, BlockSize = 2 };
                case 5253:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1741, BlockSize = 2 };
                case 5254:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1741, BlockSize = 2 };
                case 5255:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1742, BlockSize = 2 };
                case 5256:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1742, BlockSize = 2 };
                case 5257:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1743, BlockSize = 4 };
                case 5258:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1743, BlockSize = 4 };
                case 5259:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1743, BlockSize = 4 };
                case 5260:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1743, BlockSize = 4 };
                case 5261:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1744, BlockSize = 2 };
                case 5262:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1744, BlockSize = 2 };
                case 5263:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1745, BlockSize = 2 };
                case 5264:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1745, BlockSize = 2 };
                case 5265:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1746, BlockSize = 4 };
                case 5266:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1746, BlockSize = 4 };
                case 5267:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1746, BlockSize = 4 };
                case 5268:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1746, BlockSize = 4 };
                case 5269:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1747, BlockSize = 2 };
                case 5270:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1747, BlockSize = 2 };
                case 5271:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1748, BlockSize = 2 };
                case 5272:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1748, BlockSize = 2 };
                case 5273:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1749, BlockSize = 4 };
                case 5274:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1749, BlockSize = 4 };
                case 5275:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1749, BlockSize = 4 };
                case 5276:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1749, BlockSize = 4 };
                case 5277:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1750, BlockSize = 4 };
                case 5278:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1750, BlockSize = 4 };
                case 5279:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1750, BlockSize = 4 };
                case 5280:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1750, BlockSize = 4 };
                case 5281:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1751, BlockSize = 4 };
                case 5282:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = false, BlockNumber = 1751, BlockSize = 4 };
                case 5283:
                    return new Envelope { WeightLessThan = 2000, IsMale = false, IsInterventionArm = true, BlockNumber = 1751, BlockSize = 4 };
                default:
                    return null;
            }
        }
    }
}