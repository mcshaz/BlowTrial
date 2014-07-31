using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure.Randomising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlowTrial.Models
{
    [Serializable]
    public class ParticipantsSummary
    {
        ParticipantsSummary(AllocationGroups group)
        {
            var arms = ArmData.GetRatio(group).Ratios.Keys;
            ColHeaders = new RandomisationArm[arms.Count];
            arms.CopyTo(ColHeaders, 0);
            RowHeaders = (DataRequiredOption[])Enum.GetValues(typeof(DataRequiredOption));
            Participants = new List<int>[RowHeaders.Length][];
            for (int i=0;i<RowHeaders.Length;i++)
            {
                Participants[i] = new List<int>[ColHeaders.Length];
                for(int j=0;j<ColHeaders.Length;j++)
                {
                    Participants[i][j] = new List<int>();
                }
            }
        }
        public ParticipantsSummary(IEnumerable<ParticipantStage> participantSummaryData) 
        {
            AddParticipants(participantSummaryData);
        }

        void AddParticipants(IEnumerable<ParticipantStage> participantSummaryData)
        {
            foreach (var sd in participantSummaryData)
            {
                int row = Array.IndexOf(RowHeaders, sd.DataRequired);
                int col = Array.IndexOf(ColHeaders, sd.Arm);
                Participants[row][col].Add(sd.Id);
            }
        }
        public void AddParticipant(int participantId, RandomisationArm arm, DataRequiredOption dataRequired)
        {
            AddParticipants(new ParticipantStage[] { new ParticipantStage { Id = participantId, Arm = arm, DataRequired = dataRequired } } );
        }

        //returns true if the given participantId requires assignment to a new DataRequiredOption, otherwise returns false.
        public RowMove AlterParticipant(int participantId, RandomisationArm arm, DataRequiredOption dataRequired)
        {
            var oldPos = Coordinates(participantId, arm);
            if (oldPos==null)
            {
                throw new KeyNotFoundException(string.Format("The participantId: {0} could not be found. Use Add Participant if the participant has not previously been registered", participantId));
            }
            int newX = Array.IndexOf(RowHeaders, dataRequired);
            if (newX == oldPos.x) { return new RowMove(newX); }
            Participants[oldPos.x][oldPos.y].RemoveAt(oldPos.z);
            Participants[newX][oldPos.y].Add(participantId);
            return new RowMove(oldPos.x, newX);
        }

        ThreeDPoint Coordinates(int participantId, RandomisationArm arm)
        {
            int y = Array.IndexOf(ColHeaders, arm);
            for (int x = 0; x < RowHeaders.Length; x++)
            {
                int indx = Participants[x][y].IndexOf(participantId);
                if (indx!=-1)
                {
                    return new ThreeDPoint(x, y,indx);
                }
            }
            return null;
        }
        
        public RandomisationArm[] ColHeaders{ get;private set; }
        public DataRequiredOption[] RowHeaders { get;private set; }
        /// <summary>
        /// [rows(datarequired)][columns(randomisationArm)][participantIds]
        /// </summary>
        public List<int>[][] Participants { get; private set; }
        
    }

    [Serializable]
    public class ParticipantStage
    {
        public int Id { get; set; }
        public RandomisationArm Arm { get;  set; }
        public DataRequiredOption DataRequired { get;  set; }
    }
    [Serializable]
    public struct ParticipantDataStage
    {
        public ParticipantDataStage(RandomisationArm arm, DataRequiredOption dataRequired)
        {
            _arm = arm;
            _dataRequired = dataRequired;
        }
        RandomisationArm _arm;
        DataRequiredOption _dataRequired;
        public RandomisationArm Arm { get { return _arm; } }
        public DataRequiredOption DataRequired { get { return _dataRequired; } }
        public override int GetHashCode()
        {
            return (int)Arm ^ (int)DataRequired;
        }
        public override bool Equals(System.Object obj)
        {
            if (obj is ParticipantDataStage)
            {
                ParticipantDataStage p = (ParticipantDataStage)obj;
                return Arm == p.Arm && DataRequired == p.DataRequired;
            }
            else
            {
                return false;
            }
            /*
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            ParticipantDataStage p = obj as ParticipantDataStage;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Arm == p.Arm) && (DataRequired == p.DataRequired);
            */
        }

        public bool Equals(ParticipantDataStage p)
        {
            // If parameter is null return false:
            //if ((object)p == null)
            //{
            //    return false;
            //}

            // Return true if the fields match:
            return (Arm == p.Arm) && (DataRequired == p.DataRequired);
        }

        public static bool operator ==(ParticipantDataStage a, ParticipantDataStage b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            //if (((object)a == null) || ((object)b == null))
            //{
            //    return false;
            //}

            // Return true if the fields match:
            return a.Arm == b.Arm && a.DataRequired == b.DataRequired;
        }

        public static bool operator !=(ParticipantDataStage a, ParticipantDataStage b)
        {
            return !(a == b);
        }
    }
    [Serializable]
    public class RowMove
    {
        public readonly int OldRow, NewRow;
        public RowMove(int sameRow)
        {
            OldRow = NewRow = sameRow;
        }
        public RowMove(int oldRow, int newRow)
        {
            OldRow = oldRow;
            NewRow = newRow;
        }
    }
    [Serializable]
    public class TwoDPoint
    {
        public readonly int x, y;

        public TwoDPoint(int x, int y)  //constructor
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            TwoDPoint p = obj as TwoDPoint;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        public bool Equals(TwoDPoint p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }

        public static bool operator ==(TwoDPoint a, TwoDPoint b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(TwoDPoint a, TwoDPoint b)
        {
            return !(a == b);
        }
    }
    [Serializable]
    class ThreeDPoint : TwoDPoint
    {
        public readonly int z;

        public ThreeDPoint(int x, int y, int z)
            : base(x, y)
        {
            this.z = z;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            ThreeDPoint p = obj as ThreeDPoint;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return base.Equals(obj) && z == p.z;
        }

        public bool Equals(ThreeDPoint p)
        {
            // Return true if the fields match:
            return base.Equals((TwoDPoint)p) && z == p.z;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ z;
        }

        public static bool operator ==(ThreeDPoint a, ThreeDPoint b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(ThreeDPoint a, ThreeDPoint b)
        {
            return !(a == b);
        }
    }
    [Serializable]
    public class ScreenedPatientsSummary
    {
        public int TotalCount { get; set; }
        public int LikelyDie24HrCount { get; set; }
        public int BadMalformCount { get; set; }
        public int BadInfectnImmuneCount { get; set; }
        public int WasGivenBcgPriorCount { get; set; }
        public int RefusedConsentCount { get; set; }
        public int MissedCount { get; set; }
    }
}
