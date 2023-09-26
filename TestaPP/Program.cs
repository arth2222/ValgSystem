using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TestaPP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program p=new Program();    
            Random pid=new Random();
            Random kid=new Random();

            for(int i=0;i<5000000;i++)
            {
                p.InsertVote(pid.Next(4,6), pid.Next(1, 8));
            }
        }

        private void InsertVote(int pid,int kid)
        {
            SqlParameter param;
            var connectionString = ConfigurationManager.ConnectionStrings["ConnCms"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Vote (Kid,Pid) Values(@kid,@pid)", conn);//@ betyr at det er et parameter
                cmd.CommandType = CommandType.Text;

                param = new SqlParameter("@kid", SqlDbType.Int);
                param.Value = kid; //hardcode verdien til Fredrikstad. Denne må endres om det kan velges kommune fra en liste
                cmd.Parameters.Add(param);

                param = new SqlParameter("@pid", SqlDbType.Int);
                param.Value = pid;
                cmd.Parameters.Add(param);

                //SqlDataReader reader = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                //dt.Load(reader);
                //reader.Close();
                conn.Close();
            }
        }
    }
}
