using MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
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
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace QRSetter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string FILE_NAME = "TemplateCache.bin";
        private TemplateData saveData;

        private ObservableCollection<TemplateItem> Templates { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Load();
            Templates = new ObservableCollection<TemplateItem>(saveData.Items);
            templateList.ItemsSource = Templates;
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                MessageBox.Show("Please enter a value. ");
                return;
            }

            var window = new QRCodeWindow(textBox.Text);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                MessageBox.Show("Please enter a value. ");
                return;
            }

            var window = new InputWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowDialog();

            var name = window.Result;
            var item = new TemplateItem
            {
                Key = Guid.NewGuid(),
                Name = name,
                Value = textBox.Text,
            };
            Templates.Add(item);
            saveData.Items.Add(item);
            Save();
        }

        private void Save()
        {
            var binary = MessagePackSerializer.Serialize(saveData);
            using (var fs = new FileStream(FILE_NAME, FileMode.Create, FileAccess.Write))
            using (var writer = new BinaryWriter(fs))
            {
                writer.Write(binary);
            }
        }
        private void Load()
        {
            if (!File.Exists(FILE_NAME))
            {
                saveData = new TemplateData();
                return;
            }

            using (var fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                var binary = reader.ReadBytes((int)fs.Length);
                saveData = MessagePackSerializer.Deserialize<TemplateData>(binary);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var item = templateList.SelectedItem as TemplateItem;
            if (item == null)
            {
                MessageBox.Show("Please select a item. ");
                return;
            }

            textBox.Text = item.Value;
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var item = templateList.SelectedItem as TemplateItem;
            if (item == null)
            {
                MessageBox.Show("Please select a item. ");
                return;
            }

            Templates.Remove(item);
            saveData.Items.Remove(item);
            Save();
        }
    }
    [MessagePackObject]
    public class TemplateData
    {
        [Key(0)]
        public List<TemplateItem> Items { get; set; } = new List<TemplateItem>();
    }
    [MessagePackObject]
    public class TemplateItem
    {
        [Key(0)]
        public Guid Key { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Value { get; set; }
    }
}
