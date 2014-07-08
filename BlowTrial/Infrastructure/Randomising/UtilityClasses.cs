using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace BlowTrial.Infrastructure.Randomising
{
    //public enum WeightCategories { LowestWeight, MidWeight, HighestWeight }
    /*
    internal class ArmRatio
    {
        internal ArmRatio(RandomisationArm arm, byte ratio)
        {
            Arm = arm;
            Ratio = ratio;
        }
        internal RandomisationArm Arm { get; private set; }
        internal byte Ratio { get; private set; }
    }
     * */
    //to internal 
    public class BlockComponent
    {
        internal BlockComponent(byte repeats, IDictionary<RandomisationArm, int> ratios)
        {
            Repeats = repeats;
            Ratios = ratios as ReadOnlyDictionary<RandomisationArm,int>
                ?? new ReadOnlyDictionary<RandomisationArm,int>(ratios);
        }
        public ReadOnlyDictionary<RandomisationArm, int> Ratios { get; private set; }
        public byte Repeats { get; private set; }
        internal IDictionary<RandomisationArm, int> GetAllocations()
        {
            if (Repeats == 1) { return Ratios; }
            var returnVar = new Dictionary<RandomisationArm, int>(Ratios.Count);
            foreach (var kv in Ratios)
            {
                returnVar.Add(kv.Key, kv.Value * Repeats);
            }
            return returnVar;
        }
        internal BlockComponent Clone(byte repeats)
        {
            return new BlockComponent(repeats, this.Ratios);
        }
        internal int TotalBlockSize()
        {
            return Ratios.Values.Sum() * Repeats;
        }
    }
    //to internal
    public static class ArmData
    {
        static ReadOnlyDictionary<AllocationGroups, BlockComponent> _allBlocks;

        internal static ReadOnlyDictionary<AllocationGroups, BlockComponent> GetAllBlocks()
        {
            if (_allBlocks == null)
            {
                var componentDict = new Dictionary<AllocationGroups,BlockComponent>(3);
                var ratios = new Dictionary<RandomisationArm, int>(3);
                ratios.Add(RandomisationArm.Control, 1);
                ratios.Add(RandomisationArm.RussianBCG, 1);
                componentDict.Add(AllocationGroups.IndiaTwoArm, new BlockComponent(2, ratios));
                ratios = new Dictionary<RandomisationArm, int>(ratios);
                ratios.Add(RandomisationArm.DanishBcg, 1);
                componentDict.Add(AllocationGroups.IndiaThreeArmBalanced, new BlockComponent(2, ratios));
                ratios = new Dictionary<RandomisationArm, int>(ratios);
                ratios[RandomisationArm.DanishBcg] = 2;
                componentDict.Add(AllocationGroups.IndiaThreeArmUnbalanced, new BlockComponent(1, ratios));
                _allBlocks = new ReadOnlyDictionary<AllocationGroups,BlockComponent>(componentDict);
            }
            return _allBlocks;
        }

        public static BlockComponent GetRatio(AllocationGroups group)
        {
            return GetAllBlocks()[group];
        }
        public static BlockComponent GetComponents(this AllocationBlock block)
        {
            return GetRatio(block.AllocationGroup).Clone(block.GroupRepeats);
        }
        internal static int GetCountInArm(this AllocationBlock block, RandomisationArm arm)
        {
            return GetRatio(block.AllocationGroup).Ratios[arm] * block.GroupRepeats;
        }
        /// <summary>
        /// will merge secondaryBlock into existing & full (primary) block, also merging the allocationBlockId of the secondaryBlock participants
        /// NOTE the secondary block reference in the database will have to be removed.
        /// </summary>
        /// <param name="primaryBlock"></param>
        /// <param name="secondaryBlock"></param>
        internal static void MergeBlock(this AllocationBlock mergeInto, AllocationBlock mergeFrom)
        {
            if (mergeInto.Id == mergeFrom.Id)
            {
                throw new ArgumentException("AllocationBlocks cannot be merged if they have the same Id!");
            }
            if (mergeInto.RandomisationCategory != mergeFrom.RandomisationCategory)
            {
                throw new ArgumentException("AllocationBlocks cannot be merged if they do not belong to the same randomisation strata");
            }
            if (mergeInto.RandomisationCategory == mergeFrom.RandomisationCategory)
            {
                mergeInto.GroupRepeats += mergeFrom.GroupRepeats;
            }
            else
            {
                if (mergeFrom.Participants == null)
                {
                    throw new ArgumentException("The secondary block must have an ICollection of participants != null");
                }
                if (mergeInto.Participants == null)
                {
                    throw new ArgumentException("The primary block must have an ICollection of participants != null");
                }
                mergeInto.AllocationGroup = mergeFrom.AllocationGroup; //?best way to do this - important if block 2 1st transition to 3 arm randomisation
                mergeInto.Participants = new List<Participant>(mergeInto.Participants.Concat(mergeFrom.Participants));
                foreach (Participant p in mergeFrom.Participants)
                {
                    p.AllocationBlockId = mergeInto.Id;
                }
                var primaryRatios = GetRatio(mergeInto.AllocationGroup).Ratios;
                double maxRepeats = 0;
                foreach (var p in mergeInto.Participants.GroupBy(p=>p.TrialArm))
                {
                    double ratio = Math.Ceiling(p.Count() / (double)primaryRatios[p.Key]);
                    if (ratio>maxRepeats)
                    {
                        maxRepeats = ratio;
                    }
                }
                mergeInto.GroupRepeats = (byte)maxRepeats;
            }
        }
    }
}
