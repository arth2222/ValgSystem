using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ValgSystem
{
    public partial class prosent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataSource = GetVoteCount();
            GridView1.DataBind();

            GridViewProsent.DataSource = GetVoteProsent();
            GridViewProsent.DataBind();

        }

        //Spørringen teller opp antall stemmer for hvert parti
        private DataTable GetVoteCount()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["valg"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select parti.Parti, count(stemme.Pid) as Stemmer from stemme inner join Parti on Parti.Pid=Stemme.Pid group by stemme.pid,parti.Parti", conn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                conn.Close();
            }
            return dt;
        }

        private DataTable GetVoteProsent()
        {
            DataTable dt = GetVoteCount();

            //Nå skal det addes en ny kolonne til datatabellen. Denne skal inneholde prosent
            //Vi har tallene vi trenger. Totalt antall stemmer og antall stemmer per parti
            //partistemmer / total * 100     det blir prosenten

            //vi henter først ut hvor mange stemmer det er totalt. Vi bruker Linq sin funksjon Sum
            int totalStemmer = dt.AsEnumerable().Sum(s => s.Field<int>("Stemmer"));

            //Vi legger til en ny kolonne for prosent
            dt.Columns.Add("Prosent", typeof(System.Int32));


            //vi går igjennom hver rad og legger til prosentverdien
            foreach (DataRow row in dt.Rows)
            {
                //vi gjør utregningen og legger til denne i den nye kolonnen
                //row["Prosent"] = Convert.ToDouble( (int)row["Stemmer"] / totalStemmer * 100);
                row["Prosent"] = (double)(int)row["Stemmer"] / totalStemmer * 100;
            }
            
            return dt;
        }

        

    }
}


//Teller opp stemmene for hver parti. Altså group by
//select parti.Parti, count(stemme.Pid)
//from stemme
//inner join Parti on Parti.Pid= Stemme.Pid
//group by stemme.pid, parti.Parti