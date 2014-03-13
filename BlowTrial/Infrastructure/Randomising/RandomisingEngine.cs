using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
using BlowTrial.Infrastructure.Randomising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Infrastructure
{
    public static class RandomisingEngine
    {
        const int MaxAllocationsPerBlock = 4;
        const int RandomAllocationMax = (MaxAllocationsPerBlock / 2) + 1;
        static int GetNextBlockSize()
        {
            return (new Random()).Next(1,RandomAllocationMax)*2;
        }
        public static void CreateAllocation(Participant participant, IRepository repos)
        {
            //try
            {
                IEnumerable<Participant> currentBlock = GetCurrentBlock(participant, repos);
                int noInBlock = currentBlock.Count();
                if (noInBlock == 0)
                {
                    //for random block size add:
                    // newAllocation.BlockSize = BlockRandomisation.BlockSize();
                    participant.BlockNumber = 1;
                    participant.BlockSize = GetNextBlockSize();
                }
                else if (noInBlock == currentBlock.First().BlockSize)
                {
                    participant.BlockNumber = currentBlock.First().BlockNumber + 1;
                    participant.BlockSize = GetNextBlockSize();
                    currentBlock = new List<Participant>();
                }
                else
                {
                    // for Random blocksize need to get the current blocksize 
                    // int currentBlock.First().Blocksize;
                    participant.BlockNumber = currentBlock.First().BlockNumber;
                    participant.BlockSize = currentBlock.First().BlockSize;
                }
                participant.IsInterventionArm = BlockRandomisation.nextAllocation(participant.BlockSize, currentBlock, c => c.IsInterventionArm);
            }
            /*catch (Exception ex)
            {
                throw ex;
            }
            */
        }
        public static RandomisationCategories GetRandomisingCategory(Participant newParticipant)
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
            return (RandomisationCategories)returnInt;
        }
        public static void ForceAllocationToArm(Participant participant, IRepository repos)
        {
            var currentBlock = GetCurrentBlock(participant, repos).Where(p=>p.Id != participant.Id).ToList();
            if (currentBlock.Count==0) 
            {
                participant.BlockNumber=1;
                participant.BlockSize = GetNextBlockSize();
                return;
            }
            participant.BlockNumber = currentBlock.First().BlockNumber;
            currentBlock.Add(participant);
            if (BalanceBlocks(currentBlock))
            {
                currentBlock.RemoveAt(currentBlock.Count - 1);
                repos.Update(currentBlock);
            }
        }
        /// <summary>
        /// Balance allocations in a block when a new member is forced in. Used for twin allocations, and when violating to change enrollment info
        /// </summary>
        /// <param name="currentBlock">the block to be analysed. The list must be ordered so that the new allocation is LAST</param>
        /// <returns>whether entire block required resizing</returns>
        static bool BalanceBlocks(IList<Participant> currentBlock)
        {
            Participant newParticipant;
            switch (currentBlock.Count)
            {
                case 0:
                    throw new ArgumentException("currentBlock must contain at least 1 element");
                case 1:
                    newParticipant = currentBlock[0];
                    newParticipant.BlockSize = GetNextBlockSize();
                    return false;
            }
            newParticipant = currentBlock[currentBlock.Count - 1];
            int currentBlockSize = currentBlock[0].BlockSize;
            int sameAllocationCount = currentBlock.Count(c => c.IsInterventionArm == newParticipant.IsInterventionArm);
            if (sameAllocationCount <= (currentBlockSize / 2)) //block size adequate already
            {
                newParticipant.BlockSize = currentBlockSize;
                return false;
            }
            currentBlockSize += 2;
            // adjust existing block
            foreach (var p in currentBlock)
            {
                p.BlockSize = currentBlockSize;
            };
            return true;
        }
        public const int BlockWeight1 = 1000;
        public const int BlockWeight2 = 1500;
        public const int MaxBirthWeightGrams = 1999;
        static IEnumerable<Participant> GetCurrentBlock(Participant newParticipant, IRepository repos)
        {
            return QueryForCurrentBlock(repos.Participants, newParticipant).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="participant"></param>
        /// <param name="repos"></param>
        /// <returns>whether reasignment was necessary</returns>
        public static void ReasignBlockRandomisingData(Participant participant, bool newIsMale, int newAdmissionWeight ,IRepository repos)
        {
            //assign from currentblock in the old randomising category to block participant is currently in
            //options are:
            //currentBlock contains participant - move participant, nothing else required
            //participant in lower block than currentblock:
            //  currentblock contains same arm => move to block used by participant
            //  currentblock all in other arm:
            //      move from block below
            //      coalesce 2 blocks!
            var blocks = QueryForAllSameBlocks(repos.Participants, participant);
            
            if (!blocks.First().Any(p=>p.Id == participant.Id))
            {
                Participant replacingParticipant = blocks.First().FirstOrDefault(p=>p.IsInterventionArm == participant.IsInterventionArm);
                if(replacingParticipant == null)
                {
                    var coalescingBlock = blocks.Take(2).SelectMany(g=>g).ToList();
                    replacingParticipant = coalescingBlock.First(p=>p.IsInterventionArm == participant.IsInterventionArm);
                    coalescingBlock.Remove(replacingParticipant);
                    int newBlockSize = coalescingBlock.Count(p=>p.IsInterventionArm != participant.IsInterventionArm)*2;
                    foreach (var p in coalescingBlock)
                    {
                        p.BlockNumber = replacingParticipant.BlockNumber;
                        p.BlockSize = newBlockSize;
                    }
                    repos.Update(coalescingBlock);
                }
                replacingParticipant.BlockNumber = participant.BlockNumber;
                repos.Update(replacingParticipant);
            }
            participant.AdmissionWeight = newAdmissionWeight;
            participant.IsMale = newIsMale;
            //Handle block participant is going to
            ForceAllocationToArm(participant,repos);
        }

        /// <summary>
        /// Locates maximum block number for the given participant according to the block randomisation criteria
        /// ie gender, weight category and study site
        /// </summary>
        /// <param name="entitySet"></param>
        /// <param name="newParticipant"></param>
        /// <returns></returns>
        static IQueryable<Participant> QueryForCurrentBlock(IQueryable<Participant> entitySet, Participant newParticipant)
        {
            return QueryForAllSameBlocks(entitySet, newParticipant).FirstOrDefault().AsQueryable();
        }
        static IOrderedQueryable<IGrouping<int, Participant>> QueryForAllSameBlocks(IQueryable<Participant> entitySet, Participant newParticipant)
        {
            IQueryable<Participant> categoryQuery = entitySet.Where(p => p.IsMale == newParticipant.IsMale && p.CentreId == newParticipant.CentreId && p.BlockNumber.HasValue);
            if (newParticipant.AdmissionWeight < BlockWeight1)
            {
                categoryQuery = categoryQuery.Where(p => p.AdmissionWeight < BlockWeight1);
            }
            else if (newParticipant.AdmissionWeight < BlockWeight2)
            {
                categoryQuery = categoryQuery.Where(p => p.AdmissionWeight >= BlockWeight1 && p.AdmissionWeight < BlockWeight2);
            }
            else
            {
                categoryQuery = categoryQuery.Where(p2 => p2.AdmissionWeight >= BlockWeight2);
            }
            return (from p in categoryQuery
                    orderby p.RegisteredAt descending 
                    group p by p.BlockNumber.Value into blocks
                    orderby blocks.Key descending
                    select blocks);
        }
        /// <summary>
        /// When adding in envelope numbers after computerised randomisation has begun, need to reclear lower block numbers
        /// </summary>
        public static void UnsetComputerisedBlocks(IRepository repos)
        {
            int?[] computerRandomisedBlockNumbers = (from p in repos.Participants
                                                    where p.BlockNumber.HasValue && (!p.WasEnvelopeRandomised || p.MultipleSiblingId.HasValue)
                                                    group p by p.BlockNumber into distinctBlocks
                                                    select distinctBlocks.Key).ToArray();
            repos.Database.ExecuteSqlCommand(string.Format("UPDATE [Participants] SET [BlockNumber] = NULL, [BlockSize] = 0 WHERE [WasEnvelopeRandomised] = 0 OR [MultipleSiblingId] IS NOT NULL"));
            var parts = (from p in repos.Participants
                         where computerRandomisedBlockNumbers.Contains(p.BlockNumber)
                         select p).ToList();
            List<Participant> partsForUpdate = new List<Participant>(parts.Count);
            foreach (var p in parts)
            {
                var e = EnvelopeDetails.GetEnvelope(p.Id);
                if (e != null && (e.BlockNumber != p.BlockNumber || e.BlockSize != p.BlockSize))
                {
                    if (e.RandomisationCategory == RandomisingExtensions.RandomisationCategory(p))
                    {
                        p.BlockNumber = e.BlockNumber;
                        p.BlockSize = e.BlockSize;
                    }
                    else
                    {
                        p.BlockNumber = null;
                        p.BlockSize = 0;
                    }
                    partsForUpdate.Add(p);
                }
            }
            repos.Update(partsForUpdate);

        }

        static string SqlToUnsetIncorrectEnvelopes()
        {
            StringBuilder returnVar = new StringBuilder("UPDATE [Participants] SET [BlockNumber] = NULL, [BlockSize] = 0 WHERE ");
            var envelopeDetails = EnvelopeDetails.GetAllEnvelopeNumbers();
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 0 AND AdmissionWeight >= {1}) OR ",string.Join(",",envelopeDetails[RandomisationCategories.SmallestWeightFemale]),BlockWeight1);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 1 AND AdmissionWeight >= {1}) OR ",string.Join(",",envelopeDetails[RandomisationCategories.SmallestWeightMale]),BlockWeight1);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 0 AND (AdmissionWeight < {1} OR AdmissionWeight >= {2})) OR ", string.Join(",", envelopeDetails[RandomisationCategories.MidWeightFemale]), BlockWeight1, BlockWeight2);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 1 AND (AdmissionWeight < {1} OR AdmissionWeight >= {2})) OR ", string.Join(",", envelopeDetails[RandomisationCategories.MidWeightMale]), BlockWeight1, BlockWeight2);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 0 AND AdmissionWeight < {1}) OR ",string.Join(",",envelopeDetails[RandomisationCategories.TopWeightFemale]),BlockWeight2);
            returnVar.AppendFormat("(Id IN ({0}) AND IsMale = 1 AND AdmissionWeight < {1});",string.Join(",",envelopeDetails[RandomisationCategories.TopWeightMale]),BlockWeight2);
            return returnVar.ToString();
        }
        /// <summary>
        /// When returning to computerised randomisation
        /// </summary>
        public static void BalanceUnsetBlocks(IRepository repos)
        {
            int nextBalanceBlockNo = ((from p in repos.Participants
                                      orderby p.BlockNumber descending
                                      select p.BlockNumber).FirstOrDefault() ?? 0 )+ 1;
            int nextUnbalanceBlockNumber = nextBalanceBlockNo + 1;
            //
            //Unset incorrectly envelope randomised patients
            //repos.Database.ExecuteSqlCommand(SqlToUnsetIncorrectEnvelopes());

            foreach (var rc in repos.Participants.ToLookup(p=>RandomisingExtensions.RandomisationCategory(p))) //loop through each randomisation category
            {
                List<Participant> unbalancedParticipants = new List<Participant>();
                unbalancedParticipants.AddRange(rc.GroupBy(r => r.BlockNumber).Where(r=>r.Count() != r.First().BlockSize).SelectMany(r=>r));

                int interventionCount = unbalancedParticipants.Count(p => p.IsInterventionArm);
                int controlCount = unbalancedParticipants.Count - interventionCount;
                int balanceControlCount  = interventionCount > controlCount ? controlCount : interventionCount;
                int balanceInterventionCount = balanceControlCount;
                int balanceBlockSize = 2* balanceInterventionCount;
                int unbalanceBlockSize = Math.Abs(interventionCount - controlCount)*2;
                foreach (var p in unbalancedParticipants)
                {
                    if ((p.IsInterventionArm && balanceInterventionCount > 0) || (!p.IsInterventionArm && balanceControlCount > 0))
                    {
                        p.BlockSize = balanceBlockSize;
                        p.BlockNumber = nextBalanceBlockNo;
                        if (p.IsInterventionArm)
                        {
                            balanceInterventionCount--;
                        }
                        else
                        {
                            balanceControlCount--;
                        }
                    }
                    else
                    {
                        p.BlockSize = unbalanceBlockSize;
                        p.BlockNumber = nextUnbalanceBlockNumber;
                    }
                }
                repos.Update(unbalancedParticipants);
                nextBalanceBlockNo += 2;
                nextUnbalanceBlockNumber += 2;
            }
        }
    }
    public static class BlockRandomisation
    {
        public static int BlockSize()
        {
            return (new Random().Next(4) + 1) * 2; //A 32-bit signed integer greater than or equal to zero, and less than maxValue; that is, the range of return values ordinarily includes zero but not maxValue. However, if maxValue equals zero, maxValue is returned.
        }
        public static bool nextAllocation<T>(int blockSize, IEnumerable<T> patientDataCollection, Func<T, bool> predicate)
        {
            double remainingAllocations = blockSize - patientDataCollection.Count();
            if (remainingAllocations <= 0) throw new ArgumentException("patientDataCollection must be smaller than blockSize.");
            double remainingInterventions = blockSize / 2 - patientDataCollection.Count(predicate);
            double Pintervention = remainingInterventions / remainingAllocations;
            double rdm = new Random().NextDouble();
            return (rdm <= Pintervention);
        }
    }
}
