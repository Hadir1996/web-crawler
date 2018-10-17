using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IR__PROJECT
{
    public class Bigram_Calculator
    {
        List<string> word_bigrams;
        double word_bigram_count;
        public List<string> get_bi_grams_of_word(string word)
        {
            List<string> bi_grams = new List<string>();
            if (word.Length > 0)
            {
                bi_grams.Add("$" + word[0].ToString());
                for (int i = 0; i < word.Length - 1; i++)
                {
                    string bi_gram = word[i].ToString() + word[i + 1].ToString();
                    bi_grams.Add(bi_gram);
                }
                bi_grams.Add(word[word.Length - 1].ToString() + "$");
            }
            return bi_grams;
        }

        private double calc_dist(string newterm)
        {
            List<string> newterm_bigrams = get_bi_grams_of_word(newterm);
            double term_bigram_count = newterm_bigrams.Count;

            double common_bigram_count = 0;
            for (int i = 0; i < word_bigrams.Count; i++)
            {
                if (newterm_bigrams.Contains(word_bigrams.ElementAt(i)))
                {
                    common_bigram_count += 1.0;
                }
            }

            double distance = (2 * common_bigram_count) / (word_bigram_count + term_bigram_count);

            return distance;
        }

        

    }
}