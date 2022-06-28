using AlarmTableGenerator.Api;
using AlarmTableGenerator.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace AlarmTableGenerator.ViewModel
{
    public class MainViewModel : BindableBase
    {
        private string _browsePath;

        public ICommand BrowseCommand { get; private set; }
        public List<IParsedFile> FileEntries { get; set; }

        private string _mainMessage;
        public string MainMessage 
        { 
            get => _mainMessage; 
            set => SetProperty(ref _mainMessage, value); 
        }

        private string _messageDetails;
        public string MessageDetails
        {
            get => _messageDetails;
            set => SetProperty(ref _messageDetails, value);
        }

        public event EventHandler FileParsedSuccessfully;

        public MainViewModel()
        {
            _browsePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            BrowseCommand = new DelegateCommand(HandleBrowse);
            FileEntries = new List<IParsedFile>();
        }

        /// <summary>
        /// Triggered by the Import File button
        /// Will run the File Parser
        /// </summary>
        private void HandleBrowse()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = Path.GetDirectoryName(_browsePath);
                dialog.Multiselect = false;
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _browsePath = dialog.FileName;
                    if (TryParseFile())
                        FileParsedSuccessfully.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Attempts to parse the selected file, only fails if the file can not be parsed at all
        /// If a file parses with errors it will succeed and log the errors
        /// </summary>
        /// <returns></returns>
        private bool TryParseFile()
        {
            MainMessage = "Parsing selected file...";
            MessageDetails = "Please wait";

            string[] fileEntries;
            try
            {
                fileEntries = File.ReadAllLines(Path.ChangeExtension(_browsePath, ".csv"));  
            }
            catch (Exception ex)
            {
                MainMessage = "Selected file failed to parse as a CSV";
                MessageDetails = ex.Message;
                return false;
            }

            if (fileEntries == null || fileEntries.Length < 1)
            {
                MainMessage = "Selected file had no entries";
                MessageDetails = "";
                return false;
            }

            var fileTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IParsedFile).IsAssignableFrom(p)
                && !p.IsInterface);

            //should be the header
            var potentialHeader = fileEntries[0].Split(",");
            foreach (var fileType in fileTypes)
            {
                var fileInstance = Activator.CreateInstance(fileType);
                var parsedInstance = fileInstance as IParsedFile;

                if (parsedInstance.IsMatchingFileType(potentialHeader))
                {
                    //file checks out, add it officially to the list and push the data into the file
                    parsedInstance.FileName = Path.GetFileName(_browsePath);

                    FileEntries.Add(parsedInstance);

                    for (var entryIndex = 1; entryIndex < fileEntries.Length; entryIndex++)
                    {
                        var entryData = fileEntries[entryIndex].Split(",");
                        if (!parsedInstance.TryAddEntry(entryIndex, entryData))
                        {
                            MainMessage = $"Line {entryIndex} of the selected file does not have the required amount of fields for this file type";
                            MessageDetails = "";
                        }
                    }
                    
                    MainMessage = "Selected file is finished parsing";
                    MessageDetails = "View contents in the below table";
                    return true;
                }

                MainMessage = "Selected file's header doesn't match an approved file type";
                MessageDetails = "Add a proper header to file or submit a different file";
                return false;
            }

            return false;
        }
    }
}
