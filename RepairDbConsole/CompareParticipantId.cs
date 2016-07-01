using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepairDbConsole
{
    class CompareParticipantId
    {
        class ParticipantMin
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string HospitalIdentifier { get; set; }
        }
        internal static void CompareDb(string path1, string path2)
        {
            var connection1 = new SqlCeConnectionStringBuilder();
            connection1.DataSource = path1;
            var connection2 = new SqlCeConnectionStringBuilder();
            connection2.DataSource = path2;
            connection1.Password = connection2.Password = "yu8wV6076HN";
            using (SqlCeConnection con1 = new SqlCeConnection(connection1.ToString()),
                con2 = new SqlCeConnection(connection2.ToString()))
            {
                const string maxQuery = "SELECT max(Id) From Participants";
                con1.Open();
                con2.Open();
                int max1 = (int)(new SqlCeCommand(maxQuery, con1)).ExecuteScalar();
                int counter = 0;
                int max2 = (int)(new SqlCeCommand(maxQuery, con2)).ExecuteScalar();
                int max = (max1 > max2) ? max2 : max1;
                string queryString = string.Format("SELECT Id,Name,HospitalIdentifier FROM Participants WHERE Id <= {0} order by Id", max);
                SqlCeCommand cmd1 = new SqlCeCommand(queryString, con1);
                SqlCeCommand cmd2 = new SqlCeCommand(queryString, con2);

                SqlCeDataReader reader1 = cmd1.ExecuteReader();
                SqlCeDataReader reader2 = cmd2.ExecuteReader();
                while (reader1.Read())
                {
                    int currentId = (int)reader1[0];
                    if (currentId > max) { break; }
                    while (reader2.Read() && (int)reader2[0] < currentId) 
                    {
                        Console.WriteLine("db1 Id {0} db2 Id {1}", currentId, reader2[0]);
                    }
                    if (!currentId.Equals(reader2[0]))
                    {
                        Console.WriteLine("db1 Id {0} db2 Id {1}", currentId, reader2[0]);
                    }
                    if (!(reader1[1].Equals(reader2[1]) || reader1[2].Equals(reader2[2])))
                    {
                        Console.WriteLine("Id {0} Name {1} and {2} HospitalId {3} and {4}", currentId, reader1[1], reader2[1], reader1[2], reader2[2]);
                    }
                    else
                    {
                        Console.WriteLine("match for Id {0}", currentId);
                    }
                    counter++;
                    if (counter % 30 == 0)
                    {
                        Console.ReadLine();
                    }
                }
                reader1.Close();
                reader2.Close();
            }
            Console.ReadLine();
        }
    }
}
