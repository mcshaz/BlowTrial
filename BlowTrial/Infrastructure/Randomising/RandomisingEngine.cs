using BlowTrial.Domain.Interfaces;
using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Infrastructure.Randomising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlowTrial.Infrastructure.Extensions;

namespace BlowTrial.Infrastructure.Randomising
{
    public static class Engine
    {
        /*
        public static WeightCategories GetWeightCat(int weight)
        {
            return weight < Engine.BlockWeight1 ?
                    WeightCategories.LowestWeight
                    : weight < Engine.BlockWeight2 ?
                        WeightCategories.MidWeight
                        : WeightCategories.HighestWeight;
        }
        */
#region constants
        public const int BlockWeight1 = 1000;
        public const int BlockWeight2 = 1500;
        public const int MaxBirthWeightGrams = 1999;
#endregion

        static AllocationGroups GetNextAllocationGroup(int studyCentreId, RandomisationStrata strata, ITrialDataContext context = null)
        {
            AllocationGroups returnVar = BlowTrial.Helpers.BlowTrialDataService.GetDefaultAllocationGroup();
            if (returnVar == AllocationGroups.IndiaThreeArmUnbalanced)
            {  
                var alloc = context.BalancedAllocations.First(a=>a.RandomisationCategory == strata && a.StudyCentreId==studyCentreId);
                if (!alloc.IsEqualised) 
                {
                    var catQuery = from p in context.Participants
                                   where p.CentreId == studyCentreId && p.Block.RandomisationCategory == strata
                                   select p;
                    if ((double)catQuery.Count()/catQuery.Count(p=>p.TrialArm == RandomisationArm.DanishBcg) >= 3){
                        alloc.IsEqualised = true;
                        context.SaveChanges(true);
                        if (context.BalancedAllocations.All(a=>a.IsEqualised))
                        {
                            BlowTrial.Helpers.BlowTrialDataService.SetDefaultAllocationGroup(AllocationGroups.IndiaThreeArmBalanced);
                        }
                    }
                }
                if (alloc.IsEqualised)
                {
                    return AllocationGroups.IndiaThreeArmBalanced;
                }
            }
            return returnVar;
        }
        public static void CreateNewBlock(ITrialDataContext context)
        {
            
        }
        public static void CreateAllocation(Participant participant, ITrialDataContext context)
        {
            //try
            //{
            RandomisationStrata strata;
            var currentBlock = GetCurrentBlock(participant, context, out strata);
            BlockComponent component = (currentBlock == null)?null:currentBlock.GetComponents();
            if (currentBlock==null || context.Participants.Count(p=>p.AllocationBlockId == currentBlock.Id) == component.TotalBlockSize())
            {
                currentBlock = CreateNewAllocationBlock(participant, strata,out component ,context);                
            }
            participant.AllocationBlockId = currentBlock.Id;
            participant.TrialArm = BlockRandomisation.NextAllocation(from p in context.Participants 
                                                                     where p.AllocationBlockId==currentBlock.Id 
                                                                     select p.TrialArm, component);
        }

        static AllocationBlock CreateNewAllocationBlock(Participant participant, RandomisationStrata strata, out BlockComponent component,ITrialDataContext context)
        {
            var block = GetNextAllocationGroup(participant.CentreId, strata, context);
            component = ArmData.GetRatio(block);
            var returnVar = new AllocationBlock
                {
                    Id = context.AllocationBlocks.GetNextId(participant.CentreId, participant.Centre.MaxIdForSite),
                    AllocationGroup = block, 
                    GroupRepeats= component.Repeats
                };
            context.AllocationBlocks.Add(returnVar);
                
            return returnVar;
        }
        /*
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static WeightCategories GetRandomisingCategory(Participant newParticipant)
        {
            int returnInt;
            if (newParticipant.AdmissionWeight < BlockWeight1)
            {
                returnInt = 1;
            }
            else if (newParticipant.AdmissionWeight >= BlockWeight2)
            {
                returnInt = 5;
            }
            else
            {
                returnInt = 3;
            }
            if (!newParticipant.IsMale)
            {
                returnInt++;
            }
            return (WeightCategories)returnInt;
        }
       */
        public static void RemoveAllocationFromArm(Participant participant,ITrialDataContext context)
        {
            RandomisationStrata strata;
            var blocks = GetDescendingBlocks(participant, context, out strata);

            //participant is in top block anyway (or no blocks yet existing for strata) - do nothing
            int topBlockId = blocks.Select(b=>b.Id).FirstOrDefault();
            if (topBlockId != 0 && participant.AllocationBlockId != topBlockId)
            {
                //participant is not in top block, but another participant is able to replace position in earlier block
                Participant movableParticipant = context.Participants.FirstOrDefault(p=>p.AllocationBlockId == topBlockId && p.TrialArm == participant.TrialArm);
                if (movableParticipant == null)
                {
                    //no top allocations: merge with next block down
                    AllocationBlock secondTopBlock = blocks.Skip(1).FirstOrDefault();
                    if (secondTopBlock == null)
                    {
                        //there is nothing we can do
                        return;
                    }
                    AllocationBlock topBlock = blocks.FirstOrDefault();
                    if (topBlock.Participants == null) {topBlock.Participants = context.Participants.Where(p=>p.AllocationBlockId == topBlockId).ToList();}
                    if (secondTopBlock.Participants == null) {secondTopBlock.Participants = context.Participants.Where(p=>p.AllocationBlockId == secondTopBlock.Id).ToList();}
                    secondTopBlock.MergeBlock(topBlock);
                    context.AllocationBlocks.Remove(topBlock);
                    movableParticipant = secondTopBlock.Participants.First(p=>p.TrialArm == participant.TrialArm);
                }
                movableParticipant.AllocationBlockId = participant.AllocationBlockId;
            }
            participant.AllocationBlockId = 0;
        }
        public static void ForceAllocationToArm(Participant participant,ITrialDataContext context)
        {
            RandomisationStrata strata;
            var currentBlock = GetCurrentBlock(participant, context, out strata);
            if (currentBlock==null)
            {
                BlockComponent component;
                currentBlock = CreateNewAllocationBlock(participant, strata, out component ,context);
            }
            else if (currentBlock.GetCountInArm(participant.TrialArm) <= context.Participants.Count(p=>p.AllocationBlockId == currentBlock.Id && p.TrialArm==participant.TrialArm && p.Id != participant.Id))
            {
                currentBlock.GroupRepeats++;
            }
            participant.AllocationBlockId = currentBlock.Id;
            participant.Block = currentBlock;
        }
        /// <summary>
        /// Balance allocations in a block when a new member is forced in. Used for twin allocations, and when violating to change enrollment info
        /// </summary>
        /// <param name="currentBlock">the block to be analysed. The list must be ordered so that the new allocation is LAST</param>
        /// <returns>whether entire block required resizing</returns>

        static AllocationBlock GetCurrentBlock(Participant participant, ITrialDataContext context, out RandomisationStrata strata)
        {
            return GetDescendingBlocks(participant, context, out strata).FirstOrDefault();
        }

        static IOrderedQueryable<AllocationBlock> GetDescendingBlocks(Participant participant, ITrialDataContext context, out RandomisationStrata strata)
        {
            var internalStrata = strata = RandomisingExtensions.RandomisationCategory(participant);
            if (participant.Centre == null)
            {
                participant.Centre = context.StudyCentres.Find(participant.CentreId);
            }
            return (from b in context.AllocationBlocks
                    where b.RandomisationCategory == internalStrata && b.Id >= participant.CentreId && b.Id <= participant.Centre.MaxIdForSite
                    orderby b.Id descending
                    select b);
        }

        static string SqlToUnsetIncorrectEnvelopes()
        {
            StringBuilder returnVar = new StringBuilder("UPDATE [Participants] SET [BlockNumber] = NULL, [BlockSize] = 0 WHERE ");
            var envelopeDetails = EnvelopeDetails.GetAllEnvelopeNumbersByStrata();
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 0 AND AdmissionWeight >= {1}) OR ",string.Join(",",envelopeDetails[RandomisationStrata.SmallestWeightFemale]),BlockWeight1);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 1 AND AdmissionWeight >= {1}) OR ", string.Join(",", envelopeDetails[RandomisationStrata.SmallestWeightMale]), BlockWeight1);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 0 AND (AdmissionWeight < {1} OR AdmissionWeight >= {2})) OR ", string.Join(",", envelopeDetails[RandomisationStrata.MidWeightFemale]), BlockWeight1, BlockWeight2);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 1 AND (AdmissionWeight < {1} OR AdmissionWeight >= {2})) OR ", string.Join(",", envelopeDetails[RandomisationStrata.MidWeightMale]), BlockWeight1, BlockWeight2);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 0 AND AdmissionWeight < {1}) OR ", string.Join(",", envelopeDetails[RandomisationStrata.TopWeightFemale]), BlockWeight2);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 1 AND AdmissionWeight < {1});", string.Join(",", envelopeDetails[RandomisationStrata.TopWeightMale]), BlockWeight2);
            return returnVar.ToString();
        }

    }
    public static class BlockRandomisation
    {
        public static int BlockSize()
        {
            return (new Random().Next(4) + 1) * 2; //A 32-bit signed integer greater than or equal to zero, and less than maxValue; that is, the range of return values ordinarily includes zero but not maxValue. However, if maxValue equals zero, maxValue is returned.
        }
        public static bool IsNextAllocationIntervention(IEnumerable<bool> interventionArmWithinBlock, int blockSize)
        {
            double remainingAllocations = blockSize;
            double remainingInterventions = blockSize/2;
            foreach (bool isIntervention in interventionArmWithinBlock)
            {
                remainingAllocations--;
                if (isIntervention)
                {
                    remainingInterventions--;
                }
            }
            if (remainingAllocations <= 0) throw new ArgumentException("No remaining allocations");
            double Pintervention = remainingInterventions / remainingAllocations;
            double rdm = new Random().NextDouble();
            return (rdm <= Pintervention);
        }
        internal static RandomisationArm NextAllocation(IEnumerable<RandomisationArm> currentBlock, BlockComponent block)
        {
            return NextAllocation(currentBlock, block.GetAllocations());
        }
        public static RandomisationArm NextAllocation(IEnumerable<RandomisationArm> currentBlock, IDictionary<RandomisationArm, int> ratios)
        {
            double totalRemainingAllocations = ratios.Values.Sum();
            var remainingAllocations = new List<KeyValuePair<RandomisationArm, int>>();
            foreach (var g in currentBlock.GroupBy(a => a))
            {
                int grpCount = g.Count();
                totalRemainingAllocations -= grpCount;
                remainingAllocations.Add(new KeyValuePair<RandomisationArm, int>(g.Key,ratios[g.Key] - grpCount));
            }

            if (totalRemainingAllocations <= 0) throw new ArgumentException("No remaining allocations");

            double cumulativeP = 0;

            double rdm = new Random().NextDouble();
            //not worth a binary search, as unlikely to be more than 4 allocations
            return remainingAllocations.First(a=>
                (cumulativeP += a.Value / totalRemainingAllocations) <= rdm).Key;
        }
    }
}
