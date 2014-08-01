using BlowTrial.Domain.Outcomes;
using BlowTrial.Domain.Tables;
using BlowTrial.Infrastructure;
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
        public ParticipantsSummary(IEnumerable<IGrouping<ParticipantDataStage, int>> participantSummaryData)
        {
            RowHeaders = Enum.GetValues(typeof(DataRequiredOption)).Cast<DataRequiredOption>().Skip(1).ToArray();
            const int maxArmCount = 3;
            ColHeaders = new List<RandomisationArm>(maxArmCount);
            Participants = new List<OrderedList<int>>[RowHeaders.Length];
            
            for (int i = 0; i < Participants.Length;i++ )
            {
                Participants[i] = new List<OrderedList<int>>(maxArmCount);
            }
            foreach (var g in participantSummaryData)
            {
                Participants[RowIndex(g.Key.DataRequired)][ColIndex(g.Key.Arm)] = new OrderedList<int>(g);
            }
        }
        public ParticipantsSummary(IEnumerable<ParticipantStage> participantSummaryData)
            : this(participantSummaryData.GroupBy(p => new ParticipantDataStage { Arm = p.Arm, DataRequired = p.DataRequired },p=>p.Id))
        {
        }

        public TwoDPoint AddParticipant(int participantId, RandomisationArm arm, DataRequiredOption dataRequired)
        {
            var returnVar = new TwoDPoint (RowIndex(dataRequired), ColIndex(arm));
            Participants[returnVar.x][returnVar.y].Add(participantId);
            return returnVar;
        }

        //returns true if the given participantId requires assignment to a new DataRequiredOption, otherwise returns false.
        public RowMove AlterParticipant(int participantId, RandomisationArm arm, DataRequiredOption dataRequired)
        {
            int col = ColIndex(arm);
            int row = 0;
            var returnVar = new RowMove
            { 
                OldRow = Participants.Select(r=>new { rowIndex = row++, cellData = r[col]})
                                        .First(c=>c.cellData.Contains(participantId))
                                        .rowIndex,
                NewRow = RowIndex(dataRequired)
            };
            if (returnVar.OldRow != returnVar.NewRow)
            {
                Participants[returnVar.OldRow][col].Remove(participantId);
                Participants[returnVar.NewRow][col].Add(participantId);
            }
            return returnVar;
        }

        static int RowIndex(DataRequiredOption dataRqd)
        {
            return (int)dataRqd - 1;
        }
        public int ColIndex(RandomisationArm arm)
        {
            int returnVar = ColHeaders.IndexOf(arm);
            if (returnVar==-1)
            {
                ColHeaders.Add(arm);
                foreach (var row in Participants)
                {
                    row.Add(new OrderedList<int>());
                }
                return ColHeaders.Count - 1;
            }
            return returnVar;
        }
        public List<RandomisationArm> ColHeaders{ get;private set; }
        public DataRequiredOption[] RowHeaders { get;private set; }
        /// <summary>
        /// [rows(datarequired)][columns(randomisationArm)][participantIds]
        /// </summary>
        public List<OrderedList<int>>[] Participants { get; private set; }
        
    }

    [Serializable]
    public class ParticipantStage
    {
        public int Id { get; set; }
        public RandomisationArm Arm { get;  set; }
        public DataRequiredOption DataRequired { get;  set; }
    }
    [Serializable]
    public class ParticipantDataStage
    {
        public ParticipantDataStage(){}
        public ParticipantDataStage(RandomisationArm arm, DataRequiredOption dataRequired)
        {
            Arm = arm;
            DataRequired = dataRequired;
        }

        public RandomisationArm Arm { get; set;}
        public DataRequiredOption DataRequired { get; set; }
        public override int GetHashCode()
        {
            return (int)Arm ^ (int)DataRequired;
        }
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to ParticipantDataStage return false.
            ParticipantDataStage p = obj as ParticipantDataStage;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Arm == p.Arm) && (DataRequired == p.DataRequired);
        }

        public bool Equals(ParticipantDataStage p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Arm == p.Arm) && (DataRequired == p.DataRequired);
        }
        /*
        public static bool operator ==(ParticipantDataStage a, ParticipantDataStage b)
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
            return a.Arm == b.Arm && a.DataRequired == b.DataRequired;
        }

        public static bool operator !=(ParticipantDataStage a, ParticipantDataStage b)
        {
            return !(a == b);
        }
         * */
    }
    [Serializable]
    public class RowMove
    {
        public int OldRow { get; set; }
        public int NewRow { get; set; }
        public RowMove() { }
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
