using System;
using System.Threading.Tasks;
using Toefl.TextProcessor.Models;

namespace Toefl.TextProcessor.Service
{
    public interface ITranslator : IDisposable
    {
        Task<TranslationResult> TranslateAsync(string expression);
    }
}