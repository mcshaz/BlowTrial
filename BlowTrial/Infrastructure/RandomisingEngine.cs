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
        private const int DefaultBlockSize = 4;
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
                    participant.BlockSize = DefaultBlockSize;
                }
                else if (noInBlock == DefaultBlockSize)
                {
                    participant.BlockNumber = currentBlock.First().BlockNumber + 1;
                    participant.BlockSize = DefaultBlockSize;
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
            newParticipant.IsInterventionArm = repos.Participants
                .Find(pairedParticipantId)
                .IsInterventionArm;
            if (currentBlock.Count==0) 
            {
                newParticipant.BlockNumber=1;
                newParticipant.BlockSize = DefaultBlockSize;
                return;
            }
            currentBlock.Add(newParticipant);
            int currentBlockSize = currentBlock[0].BlockSize;
            newParticipant.BlockNumber = currentBlock[0].BlockNumber;
            int sameAllocationCount = currentBlock.Count(c => c.IsInterventionArm == newParticipant.IsInterventionArm);
            if (sameAllocationCount <= (currentBlockSize / 2)) //block size adequate already
            {
                newParticipant.BlockSize = currentBlockSize;
            }
            else
            {
                currentBlockSize += 2;
                // adjust existing block
                foreach (var p in currentBlock)
                {
                    p.BlockSize = currentBlockSize;
                };
                repos.Update(currentBlock);
            }
        }
        public const int BlockWeight1 = 1000;
        public const int BlockWeight2 = 1500;
        private static IEnumerable<Participant> GetCurrentBlock(Participant newParticipant, IRepository repos)
        {
            IQueryable<Participant> weightQuery = repos.Participants.Where(p=> p.IsMale == newParticipant.IsMale);
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
            return (from p in weightQuery
                    where p.BlockNumber == maxBlockNo
//                  orderby p.RegisteredAt ascending // only necessary for random block size
                    select p).ToList();
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
