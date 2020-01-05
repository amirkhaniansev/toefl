using System.Collections.Generic;

namespace Toefl.TextProcessor.Models
{
    public class TranslationResult
    {
        public string Expression { get; set; }

        public string Explanation { get; set; }

        public string MainTranslation { get; set; }

        public List<string> Synonyms { get; set; }

        public TranslationResult()
        {
            this.Expression = string.Empty;
            this.Explanation = string.Empty;
            this.MainTranslation = string.Empty;
            this.Synonyms = new List<string>();
        }
    }
}