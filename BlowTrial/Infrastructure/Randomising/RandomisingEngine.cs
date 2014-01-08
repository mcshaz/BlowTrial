using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Interfaces;
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
        public static void ForcePairedAllocation(Participant newParticipant, int pairedParticipantId, IRepository repos)
        {
            var currentBlock = GetCurrentBlock(newParticipant, repos).ToList();
            newParticipant.IsInterventionArm = repos
                .FindParticipant(pairedParticipantId)
                .IsInterventionArm;
            if (currentBlock.Count==0) 
            {
                newParticipant.BlockNumber=1;
                newParticipant.BlockSize = GetNextBlockSize();
                return;
            }
            newParticipant.BlockNumber = currentBlock.First().BlockNumber;
            currentBlock.Add(newParticipant);
            if (BalanceBlocks(currentBlock))
            {
                currentBlock.RemoveAt(currentBlock.Count - 1);
                repos.Update(currentBlock);
            }
        }
        /// <summary>
        /// Balance allocations in a block when a new member is forced in. Used for twin allocations
        /// </summary>
        /// <param name="currentBlock">the block to be analysed the block must be ordered so that the new allocation is LAST</param>
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
        static IEnumerable<Participant> GetCurrentBlock(Participant newParticipant, IRepository repos)
        {
            return QueryForCurrentBlock(repos.Participants, newParticipant);
        }
        static List<Participant> QueryForCurrentBlock(IQueryable<Participant> entitySet, Participant newParticipant)
        {
            IQueryable<Participant> weightQuery = entitySet.Where(p => p.IsMale == newParticipant.IsMale && p.CentreId == newParticipant.CentreId);
            if (newParticipant.AdmissionWeight<BlockWeight1)
            {
                weightQuery = weightQuery.Where(p=>p.AdmissionWeight<BlockWeight1);
            }
            else if (newParticipant.AdmissionWeight<BlockWeight2)
            {
                weightQuery=weightQuery.Where(p=>p.AdmissionWeight>=BlockWeight1 && p.AdmissionWeight<BlockWeight2);
            }
            else
            {
                weightQuery=weightQuery.Where(p2=>p2.AdmissionWeight>=BlockWeight2);
            }
            int maxBlockNo = (from p in weightQuery
                              orderby p.BlockNumber descending
                              select p.BlockNumber).FirstOrDefault();
            if (maxBlockNo == 0) { return new List<Participant>(); }
            return (from p in weightQuery
                    where p.BlockNumber == maxBlockNo
                    orderby p.RegisteredAt descending 
                    select p).ToList();
        }
        public static void ResetBlock(int blockToReset, IRepository repos)
        {
            var participants = (from p in repos.Participants
                                where p.BlockNumber == blockToReset
                                select p).ToList();
            foreach (Participant p in participants)
            {
                IQueryable<Participant> otherBlocks = repos.Participants.Where(allPart => allPart.BlockNumber != blockToReset);
                var blockToUse = QueryForCurrentBlock(otherBlocks, p);
                if (blockToUse.Count == 0)
                {
                    p.BlockNumber = 1;
                    p.BlockSize = GetNextBlockSize();
                    repos.Update(p);
                }
                else 
                {
                    p.BlockNumber = blockToUse.First().BlockNumber;
                    blockToUse.Add(p);
                    if (BalanceBlocks(blockToUse))
                    {
                        repos.Update(blockToUse);
                    }
                    else 
                    {
                        repos.Update(p);
                    }
                }
            }
        }
    }
    public static class BlockRandomisation
    {
        const double third = 1 / 3;
        const double twothird = 2 / 3;
        public static int BlockSize()
        {
            double rdm = new Random().NextDouble();
            if (rdm < third) return 4;
            if (rdm < twothird) return 6;
            return 8;
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
