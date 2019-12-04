using System.Collections.Generic;

namespace Toefl.TextProcessor.Models
{
    public class TranslationResult
    {
        public string Expression { get; set; }

        public string Explanation { get; set; }

        public string MainTranslation { get; set; }

        public List<string> Synonyms { get; set; }
    }
}