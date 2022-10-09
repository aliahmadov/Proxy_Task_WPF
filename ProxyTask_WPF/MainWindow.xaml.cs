using System;
using System.Collections.Generic;
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

namespace ProxyTask_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    interface ICache
    {
        List<string> GetValue(string key);
        void SetValue(string key);
    }

    class RemoteDatabase : ICache
    {
        public List<string> Database { get; set; }

        public RemoteDatabase()
        {
            Database = new List<string>();
            var fileText = File.ReadAllText(@"-/../../../Files\Notes.txt");
            var database = string.Join("", fileText.Split('\r')).Split('\n');
            Database = database.ToList();
        }

        public List<string> GetValue(string key)
        {
            if (Database.Contains(key))
            {
                return Database.FindAll(c => c.StartsWith(key));
            }
            else
            {
                Database.Add(key);
                return Database.FindAll(c => c.StartsWith(key));
            }
        }
        public void SetValue(string key)
        {
            Database.Add(key);
        }
    }
    class CacheProxy : ICache
    {
        public RemoteDatabase RemoteDatabase { get; set; }

        public List<string> UsedWordsCache { get; set; }

        public CacheProxy()
        {
            RemoteDatabase = new RemoteDatabase();
            UsedWordsCache = new List<string>()
            {
                "apple",
                "aluminium",
                "bob",
                "creativity",
                "gold",
                "martial",
                "none"
            };

        }
        public List<string> GetValue(string key)
        {
            return UsedWordsCache.FindAll(c => c.StartsWith(key));
        }

        public void SetValue(string key)
        {
            var word = RemoteDatabase.Database.Find(c => c == key);
            if (word != null)
            {
                if (!UsedWordsCache.Contains(key))
                    UsedWordsCache.Add(word);
                else
                    MessageBox.Show($"{key} already exists in cache");
            }
            else
            {
                MessageBox.Show("Not found in database");
                MessageBox.Show($"Therefore, {key} added to database");
                RemoteDatabase.Database.Add(key);
            }
        }
    }


    public partial class MainWindow : Window
    {

        RemoteDatabase database = new RemoteDatabase();

        CacheProxy proxy = new CacheProxy();
        public List<string> listBoxViewStrings { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            listBoxViewStrings = new List<string>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = textBox.Text;
            proxy.SetValue(textBox.Text);
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox.Text.Length > 0)
            {
                listBoxViewStrings = proxy.GetValue(textBox.Text);
                listBox.ItemsSource = listBoxViewStrings;
            }
            else
            {
                listBox.ItemsSource = null;
            }
        }
    }
}
