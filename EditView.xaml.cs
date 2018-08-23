using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace IndoorFinder
{
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class EditView : UserControl
    {
        private BuildingList _buildingList { get; set; }
        public EditView()
        {
            InitializeComponent();
            _buildingList = new BuildingList();
            listBoxBuilding.ItemsSource = _buildingList.GetBuildings().Select(C => C.Name);
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            _buildingList.AddBuilding(textBoxBuliding.Text, textBoxMap.Text, textBoxPath.Text);
            listBoxBuilding.ItemsSource = null;
            listBoxBuilding.ItemsSource = _buildingList.GetBuildings().Select(C => C.Name);
        }

        private void listBoxBuilding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int buildingIndex = listBoxBuilding.SelectedIndex;
                Building thisBuilding = _buildingList.GetBuildings()[buildingIndex];
                listBoxMap.ItemsSource = null;
                listBoxMap.ItemsSource = thisBuilding.GetMaps().Select(C => C.Name);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _buildingList.WriteXml();
            }
        }
    }

    public class BuildingList
    {
        private ObservableCollection<Building> _buildings { get; set; }

        public BuildingList()
        {
            _buildings = new ObservableCollection<Building>();
            loadXml();
        }

        private void loadXml()
        {
            XElement root;
            try
            {
                root = XElement.Load("Buildings.xml");
            }
            catch 
            {
                MessageBox.Show("No initial xml file.");
                return;
            }

            foreach (var building in root.Elements("Building"))
            {
                Building thisBuilding = new Building((string)building.Attribute("Name"));
                _buildings.Add(thisBuilding);

                foreach (var map in building.Elements("Map"))
                {
                    thisBuilding.AddMap((string)map.Attribute("Name"), (string)map.Attribute("Path"));
                }
            }
        }

        public ObservableCollection<Building> GetBuildings()
        {
            return _buildings;
        }

        public void AddBuilding(string buildingName, string mapName, string mapPath)
        {
            Building thisBuilding = IsExisted(buildingName);

            if (thisBuilding == null)
            {
                thisBuilding = new Building(buildingName);
                _buildings.Add(thisBuilding);
            }

            thisBuilding.AddMap(mapName, mapPath);
        }

        private Building IsExisted(string buildingName)
        {
            foreach (Building building in _buildings)
            {
                if (building.Name == buildingName)
                {
                    return building;
                }
            }
            return null;
        }

        public void WriteXml()
        {
            XmlDocument xmlBuildings = new XmlDocument();

            XmlElement building_List = xmlBuildings.CreateElement("Building_List");
            xmlBuildings.AppendChild(building_List);

            foreach (Building building in _buildings)
            {
                building_List.AppendChild(building.BuildingXml(xmlBuildings));
            }

            xmlBuildings.Save("Buildings.xml");
        }
    }

    public class Building
    {
        private List<Map> _maps { get; set; }
        public string Name { get; }

        public Building(string buildingName)
        {
            _maps = new List<Map>();
            Name = buildingName;
        }

        public List<Map> GetMaps()
        {
            return _maps;
        }

        private Map IsExisted(string mapName)
        {
            foreach (Map map in _maps)
            {
                if (map.Name == mapName)
                {
                    return map;
                }
            }
            return null;
        }

        public void AddMap(string mapName, string mapPath)
        {
            Map thisMap = IsExisted(mapName);

            if (thisMap == null)
            {
                thisMap = new Map(mapName);
                _maps.Add(thisMap);
            }

            thisMap.ChangePath(mapPath);
        }

        public void DeleteMap(string mapName)
        {

        }

        public XmlElement BuildingXml(XmlDocument document)
        {
            XmlElement building = document.CreateElement("Building");
            building.SetAttribute("Name", this.Name);

            foreach (Map map in _maps)
            {
                building.AppendChild(map.MapXml(document));
            }

            return building;
        }
    }

    public class Map
    {
        public string Name { get; }
        public string Mode { get; set; }
        private string _path { get; set; }
        private Image _image { get; set; }
        public List<Beacon> Beacons { get; }

        public Map(string name)
        {
            Beacons = new List<Beacon>();
            Name = name;
        }

        public void ChangePath(string path)
        {
            _path = path;
        }

        public XmlElement MapXml(XmlDocument document)
        {
            XmlElement map = document.CreateElement("Map");
            map.SetAttribute("Name", this.Name);
            map.SetAttribute("Path", this._path);
            return map;
        }
    }
}
