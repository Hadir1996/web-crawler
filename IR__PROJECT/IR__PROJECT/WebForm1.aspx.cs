using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IR__PROJECT
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        IRdbEntities1 rdbEntities1 = new IRdbEntities1();
        string content = " ";
        int id = 0;
        string[] tockens;
        string term;
        
        List<string> contant = new List<string>();
        List<string> url = new List<string>();
        List<string> readertockens = new List<string>();
        List<string> readertockenscopy = new List<string>();
        List<string> readerdocid = new List<string>();
        List<string> readerfreq = new List<string>();
        List<string> readerpositions = new List<string>();
        List<string> readerbiagrams = new List<string>();
        List<string> readerbigramterms = new List<string>();
        List<string> termstobebigrammed = new List<string>();
        List<int> frequancy = new List<int>();
        List<string> positions = new List<string>();
        List<string> withOutStopW = new List<string>();
        List<string> withOutStopWcopy = new List<string>();
        List<string> biagramedwords = new List<string>();
        //    string term;
        string[] stopwords = File.ReadAllLines("c:\\users\\hp\\source\\repos\\IR__PROJECT\\IR__PROJECT\\Stopwords.txt");
        protected void Page_Load(object sender, EventArgs e)
        {
            string connetionString = null;
            SqlConnection cnn;
            connetionString = "Data Source=HADEER;Initial Catalog=IRdb;Integrated Security=True";
            cnn = new SqlConnection(connetionString);

            cnn.Open();
            SqlCommand com = new SqlCommand("select * from AllPages ", cnn);
            SqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                url.Add(reader["linkUrl"].ToString());
                contant.Add(reader["mycontent"].ToString());
            }
            cnn.Close();
        }

        /// <summary>
        /// /////////getcontent///////////////////////////////////////////////////
        /// </summary>
        protected void getcontent()
        {
            withOutStopW.Clear();
            withOutStopWcopy.Clear();


            for (int i = 0; i < contant.Count; i++)
            {

                id = i + 1;
                //split
                content = contant[i];
                content = content.Replace(',', '\t');
                content = content.Replace('.', '\t');
                content = content.Replace(';', '\t');
                content = content.Replace(':', '\t');
                tockens = content.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j <= tockens.Length - 1; j++)
                {
                    //case folding
                    tockens[j] = tockens[j].ToLower();
                    //Remove  punctuation
                    var sb = new StringBuilder();

                    foreach (char c in tockens[j])
                    {
                        if (!char.IsPunctuation(c))
                            sb.Append(c);
                    }
                    tockens[j] = sb.ToString();
                    //stop words removal
                    Boolean exit = false;
                    for (int h = 0; h <= stopwords.Length - 1; h++)
                    {
                        if (tockens[j].Equals(stopwords[h]))
                        {

                            exit = true;
                            break;
                        }
                    }



                    /////////////////////////////
                    if (exit == false)
                    {
                       

                        //stemming
                        Stemmer stemmer = new Stemmer();
                        string stem;
                        term = tockens[j];
                        stemmer.add(term);
                        stemmer.stem();
                        stem = stemmer.ToString();
                        
                        withOutStopW.Add(stem);
                        withOutStopWcopy.Add(term);
                        


                    }
                    
                }

                saveinvertedindex(withOutStopW, withOutStopWcopy , i);
                List<string> list = makebigrams();
                savebigramindex(list);
            }


        }
        /// <summary>
        /// /saveinvertedindex///////////////////////////////////
        /// </summary>
        /// <param name="tockenslist"></param>
        /// <param name="i"></param>
        protected void saveinvertedindex(List<string> tockenslist, List<string> tockenslistcopy , int i)
        {
            positions.Clear();
            //frequancy.Clear();
            string connetionString = null;
            SqlConnection cnn;
            connetionString = "Data Source=HADEER;Initial Catalog=IRdb;Integrated Security=True";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            SqlCommand com = new SqlCommand("select * from InvertedIndex ", cnn);
            SqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                readertockens.Add(reader["Term"].ToString());
                readerdocid.Add(reader["docid"].ToString());
                readerfreq.Add(reader["Frequency"].ToString());
                readerpositions.Add(reader["positions"].ToString());

            }
            cnn.Close();
            //distinctWords//
            List<string> distinct = distinctWord(tockenslist);
            List<string> distinctcopy= distinctWordcopy(tockenslistcopy);
            for (int j = 0; j < distinct.Count; j++)
            {

                if (readertockens.Contains(distinct[j]))
                {
                    int inde = readertockens.IndexOf(distinct[j]);
                    readerdocid[inde] += "," + i.ToString();
                    readerfreq[inde] += "," + frequancy[j].ToString();//
                    readerpositions[inde] += "@" + positions[j];//
                    SqlConnection con;
                    connetionString = "Data Source=HADEER;Initial Catalog=IRdb;Integrated Security=True";
                    con = new SqlConnection(connetionString);
                    SqlCommand command = new SqlCommand("update InvertedIndex set docid = @doc ,Frequency= @freq ,positions = @pos WHERE Term = @term ", con);
                    SqlCommand command2 = new SqlCommand("update invertedindexcopy set docid = @doc2  WHERE term = @term2 ", con);

                    command.Parameters.AddWithValue("@term", distinct[j]);
                    command.Parameters.AddWithValue("@doc", readerdocid[inde]);
                    command.Parameters.AddWithValue("@freq", readerfreq[inde]);
                    command.Parameters.AddWithValue("@pos", readerpositions[inde]);
                    command2.Parameters.AddWithValue("@term2", distinctcopy[j]);
                    command2.Parameters.AddWithValue("@doc2", readerdocid[inde]);
                    con.Open();
                    command.ExecuteNonQuery();
                    command2.ExecuteNonQuery();
                    con.Close();


                }
                else
                {
                    SqlConnection con;
                    connetionString = "Data Source=HADEER;Initial Catalog=IRdb;Integrated Security=True";
                    con = new SqlConnection(connetionString);
                    SqlCommand sqlCommand = new SqlCommand("INSERT INTO InvertedIndex (Term , docid , Frequency , positions) VALUES (@term1 ,@doc1 , @freq1 , @pos1)", con);
                    SqlCommand sqlCommand2 = new SqlCommand("INSERT INTO invertedindexcopy (term , docid  ) VALUES (@term2 ,@doc2)", con);
                    sqlCommand.Parameters.AddWithValue("@term1", distinct[j]);
                    sqlCommand.Parameters.AddWithValue("@doc1", i);
                    sqlCommand.Parameters.AddWithValue("@freq1", frequancy[j]);
                    sqlCommand.Parameters.AddWithValue("@pos1", positions[j]);
                    sqlCommand2.Parameters.AddWithValue("@term2", distinctcopy[j]) ;
                    sqlCommand2.Parameters.AddWithValue("@doc2", i);

                    con.Open();
                    sqlCommand.ExecuteNonQuery();
                    sqlCommand2.ExecuteNonQuery();
                    con.Close();
                }
            }

        }
        protected List<string> makebigrams()
        {
            List<string> bi = new List<string>();
            SqlConnection con;
            string connetionString = "Data Source=HADEER;Initial Catalog=IRdb;Integrated Security=True";
            con = new SqlConnection(connetionString);

            SqlCommand command = new SqlCommand("select term from invertedindexcopy", con);
            con.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                termstobebigrammed.Add(reader["term"].ToString());
            }
            con.Close();
            Bigram_Calculator bigram_Calculator = new Bigram_Calculator();
            List<string> bigramsset = new List<string>();
            for (int i = 0; i < termstobebigrammed.Count; i++) {

                bigramsset=bigram_Calculator.get_bi_grams_of_word(termstobebigrammed[i]);
            }
            for(int l = 0; l < bigramsset.Count; l++)
            {
                bi.Add(bigramsset.ElementAt(l));
            }
            return bi;
        }
        /// <summary>
        /// savebigramindex///////////////////////////////////////////////////////
        /// </summary>
        /// <param name="bigrams"></param>
        /// <param name="term"></param>
        protected void savebigramindex(List<string> bigrams)
        {
            SqlConnection con;
            string connetionString = "Data Source=HADEER;Initial Catalog=IRdb;Integrated Security=True";
            con = new SqlConnection(connetionString);
            SqlCommand com = new SqlCommand("select K_gram from bigram_index ", con);
            con.Open();
            SqlDataReader reader = com.ExecuteReader();
            while (reader.Read())
            {
                readerbiagrams.Add(reader["K_gram"].ToString());
            }
            con.Close();
            //distinctbigrams//
            List<string> distinctbigrams = distinctbigram(bigrams);
            for (int j = 0; j < distinctbigrams.Count; j++)
            {

                if (readerbiagrams.Contains(distinctbigrams[j]))
                {
                    
                    continue;

                }
                else
                {
                    //SqlCommand command = new SqlCommand("INSERT INTO bigram_index (K_gram , Term ) VALUES (@k_gram,@term) ", con);
                    SqlCommand command = new SqlCommand("INSERT INTO bigram_index (K_gram) VALUES (@var) ", con);
                    for (int i = 0; i < distinctbigrams.Count; i++)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@var", distinctbigrams[i]);
                    //    command.Parameters.AddWithValue("@term", term);

                        con.Open();
                        command.ExecuteNonQuery();
                        con.Close();

                    }
                }
            }


        }

        /// <summary>
        /// distinctWord//////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>

        protected List<string> distinctWord(List<string> document)
        {
            List<string> distinct = new List<string>();
            frequancy.Clear();
            for (int i = 0; i < document.Count; i++)
            {

                if (distinct.Contains(document[i]))
                {
                    frequancy[distinct.IndexOf(document[i])]++;
                    positions[distinct.IndexOf(document[i])] += "," + (i + 1).ToString();
                }
                else
                {
                    distinct.Add(document[i]);
                    frequancy.Add(1);
                    positions.Add((i + 1).ToString());
                }


            }
            return distinct;


       }

        protected List<string> distinctWordcopy(List<string> document)
        {
            List<string> distinct = new List<string>();
            
            for (int i = 0; i < document.Count; i++)
            {

                if (distinct.Contains(document[i]))
                {
                    continue;
                }
                else
                {
                    distinct.Add(document[i]);
                    
                }


            }
            return distinct;


        }
        /// <summary>
        /// distinctbigram////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public List<string> distinctbigram(List<string> document)
        {
            List<string> distinct = new List<string>();

            for (int i = 0; i < document.Count; i++)
            {

                if (distinct.Contains(document[i]))
                {
                    continue;
                }
                else
                {
                    distinct.Add(document[i]);
                    frequancy.Add(1);
                    positions.Add((i + 1).ToString());
                }


            }
            return distinct;

        }


        protected void Button1_Click(object sender, EventArgs e)
        {
            getcontent();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {

        }
    }
}

