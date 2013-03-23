using GalaSoft.MvvmLight;
using FaceRankGenerator.Model;
using GalaSoft.MvvmLight.Command;
using System.IO;
using S1Parser;
using S1Parser.Emotion;
using System.Linq;
using System.Xml.Linq;
using System.Xml;

namespace FaceRankGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                if (_welcomeTitle == value)
                {
                    return;
                }

                _welcomeTitle = value;
                RaisePropertyChanged(WelcomeTitlePropertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });
        }

        private RelayCommand _generateCommand;

        /// <summary>
        /// Gets the GenerateCommand.
        /// </summary>
        public RelayCommand GenerateCommand
        {
            get
            {
                return _generateCommand
                    ?? (_generateCommand = new RelayCommand(
                            () =>
                            {
                                GenerateXml();
                            }));
            }
        }

        private void GenerateXml()
        {
            FileStream sourcePage = new FileStream("Data/face.htm", FileMode.Open);
            FileStream rankPage = new FileStream("Data/faceRank.htm", FileMode.Open);
            EmotionParser.Init(sourcePage);
            XDocument doc = new XDocument();
            XElement root = new XElement("Root");
            doc.Add(root);

            var ranks = new HtmlDoc(rankPage).RootElement.Descendants("img");
            foreach (var image in ranks)
            {
                XElement e = null;
                foreach (var item in EmotionParser.EmotionList)
                {
                    if (image.Attributes["src"] == item.Value.Path)
                    {
                        e = new XElement("img");
                        XAttribute id = new XAttribute("Id", item.Value.Id);
                        XAttribute Path = new XAttribute("Path", item.Value.Path);
                        e.Add(id);
                        e.Add(Path);
                        System.Diagnostics.Debug.WriteLine(item.Value.Path);
                        break;
                    }
                }
                if (e != null) root.Add(e);
            }
            var writer = XmlWriter.Create("emotion_list.xml", 
                new XmlWriterSettings {
                    Indent = true,
                    NewLineOnAttributes = false
            });

            doc.WriteTo(writer);
            writer.Flush();

            WelcomeTitle = "Done";
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}