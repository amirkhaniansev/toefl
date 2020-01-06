using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using Toefl.TextProcessor.Service;
using Toefl.TextProcessor.Models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Toefl.ToeflDesktopUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITranslator translator;

        private string input;
        private string translation;
        private string explanation;
        private ObservableCollection<string> synonyms;
        private ObservableCollection<TranslationResult> translationResults;

        private bool isBusy;

        public string Input
        {
            get => this.input;

            set => this.Set(ref this.input, value);
        }

        public string Translation
        {
            get => this.translation;

            set => this.Set(ref this.translation, value);
        }

        public string Explanation
        {
            get => this.explanation;

            set => this.Set(ref this.explanation, value);
        }

        public ObservableCollection<string> Synonyms
        {
            get => this.synonyms;

            set => this.Set(ref this.synonyms, value);
        }

        public ObservableCollection<TranslationResult> TranslationResults
        {
            get => this.translationResults;

            set => this.Set(ref this.translationResults, value);
        }

        public ICommand CloseCommand { get; }
        public ICommand TranslateCommand { get; }
        public ICommand RemoveSynonymCommand { get; }
        public ICommand SaveCommand { get; }

        public MainViewModel()
        {
            this.translationResults = new ObservableCollection<TranslationResult>();
            this.translator = new GooglePuppeteerTranslator(new GoogleParser(), ConfigurationManager.AppSettings["url"]);

            this.CloseCommand = new RelayCommand(this.Close);
            this.TranslateCommand = new RelayCommand(this.Translate, this.CanTranslate);
            this.SaveCommand = new RelayCommand(this.Save, this.CanSave);
            this.RemoveSynonymCommand = new RelayCommand<string>(this.RemoveSynonym, this.CanRemoveSynonym);
        }

        private void Close()
        {
            this.translator.Dispose();
        }

        private async void Translate()
        {
            try
            {
                this.isBusy = true;
                var result = await this.translator.TranslateAsync(this.input);
                if (result == null)
                    return;

                this.Explanation = result.Explanation;
                this.Translation = result.MainTranslation;
                this.Synonyms = new ObservableCollection<string>(result.Synonyms);

                this.translationResults.Add(result);
            }
            catch
            {
                MessageBox.Show("Unable to translate. Sorry for inconvenience.");
            }
            finally
            {
                this.isBusy = false;
            }
        }

        private async void Save()
        {
            try
            {
                this.isBusy = true;
                var dlg = new SaveFileDialog();
                dlg.FileName = DateTime.Now.ToString();
                dlg.DefaultExt = ".json";
                dlg.Filter = "Text documents (.json)|*.json";

                var result = dlg.ShowDialog();
                if (!result.GetValueOrDefault(false))
                    return;

                var path = dlg.FileName;
                var content = JsonConvert.SerializeObject(this.translationResults, Formatting.Indented);
                using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(content);
                }
            }
            catch
            {
                MessageBox.Show("Unable to save file. Sorry for inconvenience.");
            }
            finally
            {
                this.isBusy = false;
            }
        }

        private void RemoveSynonym(string synonym)
        {
            this.synonyms.Remove(synonym);
        }

        private bool CanSave()
        {
            return !this.isBusy && this.translationResults.Count != 0;
        }

        private bool CanRemoveSynonym(string synonym)
        {
            return !string.IsNullOrEmpty(synonym) && this.synonyms != null && this.synonyms.Count > 1;
        }
        
        private bool CanTranslate()
        {
            return !this.isBusy && !string.IsNullOrEmpty(this.input);
        }
    }
}