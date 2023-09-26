using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ValgSystem
{
    public partial class ViewResult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //hente ut alle alle 3 tabeller
            DataTable partis=GetAllParti();
            DataTable votes = GetAllVotes();
            DataTable kommunes = GetAllKommune();

            //lage en ny tabell med partinavn istendfor Pids ved å joine parti og votes og putte inn til en ny. Parti og Kid er cols i den nye tabellen

            var res = from p in partis.AsEnumerable()
                      join v in votes.AsEnumerable()
                          on p.Field<int>("Pid") equals v.Field<int>("Pid")
                      select new
                      {
                          Parti = p.Field<string>("Parti"),
                          Kid = v.Field<int>("Kid")
                      };

            //lage en ny tabell med partinavn, kommunenavn, istedenfor tabellen med kun ids. en rad per stemme. feks fredrikstad venstre. da må vi koble 3 tabeller sammen

            var joinAll3=from p in partis.AsEnumerable()
            join v in votes.AsEnumerable() on p.Field<int>("Pid") equals v.Field<int>("Pid")
            join k in kommunes.AsEnumerable() on v.Field<int>("Kid") equals k.Field<int>("Kid")
                        select new
            {
                Kommune = k.Field<string>("Kommune"),
                Parti = p.Field<string>("Parti")
            };



            //noen eksempler på litt uthentinger
            int hoyreStemmer = res.Where(p => p.Parti == "H").Count();
            int hoyreStemmerFredrikstad = res.Where(p => p.Parti == "H" && p.Kid == 1).Count();
            decimal hoyreStemmerProsentTotalt = ((decimal)hoyreStemmer / votes.Rows.Count) * 100;

            ////////////////////////////////////////////
            var resultatStemmerAllePartierMedProsent = from r in res
                                                       group r by r.Parti into grp
                                                       select new { Parti = grp.Key, AntallStemmer = grp.Count(), Prosent = (decimal)grp.Count() / res.Count() * 100 };


            //var resultatStemmerAllePartierMedProsentPerKommune = from r in joinAll3


            //                                                     group r by r.Parti into grp

            //                                                     select new { Parti = grp.Key, AntallStemmer = grp.Count(), Prosent = (decimal)grp.Count() / joinAll3.Count() * 100};




            //Using Method Syntax - henter ut alle partier per kommune med antall stemmer - antall rader blir antall kommuner * antall partier
            var GroupByMultipleKeys = joinAll3
                .GroupBy(x => new { x.Kommune, x.Parti })
                .Select(g => new
                {
                    Kommune = g.Key.Kommune,
                    Parti = g.Key.Parti,
                    Stemmer = g.Where(s=>s.Parti==g.Key.Parti).Count()
                    //her kan også prosenten regnes ut, og andre ting om du vil
                }).OrderBy(k=>k.Kommune);;






            GridView1.DataSource = GroupByMultipleKeys;
            GridView1.DataBind();
            //https://tutorials.eu/how-to-use-linq-in-datatables-in-csharp/
            //https://www.geeksforgeeks.org/linq-join-inner-join/
            //https://www.codeproject.com/Questions/5327489/How-do-group-by-and-count-in-lambda-expression
            //https://www.csharptutorial.net/csharp-linq/linq-thenby/
        }

        private DataTable GetAllVotes()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ConnCms"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * from Vote", conn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                conn.Close();
            }
            return dt;
        }

        private DataTable GetAllParti()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ConnCms"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * from Parti", conn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                conn.Close();
            }
            return dt;
        }

        private DataTable GetAllKommune()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ConnCms"].ConnectionString;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * from Kommune", conn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
                reader.Close();
                conn.Close();
            }
            return dt;
        }
    }
}