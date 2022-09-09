using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
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
using Newtonsoft.Json;

namespace FastFolderOpener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string databaseFilePath;
        Dictionary<string, Dictionary<string, string>> database;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = Constants.APPLICATION_TITLE;
            string path = await Config.GetInstance().GetAsync("databaseFilePath") ?? string.Empty;
            setDatabaseFilePath(path);
            await loadAync();
        }

        private void projectListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string project = ((sender as ListBox).SelectedItem as string);
            if (project != null)
            {
                Dictionary<string, string> categories = database[project];
                categoriesListBox.Items.Clear();
                foreach (var category in categories.Keys)
                {
                    categoriesListBox.Items.Add(category);
                }
            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            string project = projectListBox.SelectedItem as string;
            string category = categoriesListBox.SelectedItem as string;
            if (project != null && category != null)
            {
                try
                {
                    Process.Start("explorer.exe", database[project][category]);
                } catch
                {
                    MessageBox.Show(Constants.COULD_NOT_OPEN_DIRECTORY_MESSAGE,
                    Constants.APPLICATION_TITLE);
                }
            } else
            {
                MessageBox.Show(Constants.PROJECT_OR_CATEGORY_NOT_SELECTED_MESSAGE,
                    Constants.APPLICATION_TITLE);
            }
        }

        private void editShortcutsButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.OpenWithNotepad(databaseFilePath);
        }

        private async void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!await loadAync())
            {
                MessageBox.Show(Constants.DATABASE_LOADING_ERROR_MESSAGE,
                    Constants.APPLICATION_TITLE);
            }
        }

        private void setDatabaseFilePath(string path)
        {
            databaseFilePath = path;

            // Update UI as well.
            databaseTextBlock.FontWeight = FontWeights.SemiBold;
            databaseTextBlock.Text = path;
        }
        private async void addDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            using var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                setDatabaseFilePath(path);
                if (!await loadAync())
                {
                    MessageBox.Show(Constants.DATABASE_LOADING_ERROR_MESSAGE,
                    Constants.APPLICATION_TITLE);
                }
                await Config.GetInstance().AddAsync("databaseFilePath", path);
            }
        }
        private async Task<bool> loadAync()
        {
            projectListBox.Items.Clear();
            categoriesListBox.Items.Clear();

            // Load database.
            database = await DatabaseManager.DatabaseManager
                .GetDatabaseManager(databaseFilePath)
                .LoadAsync();

            if (database is null)
            {
                return false;
            }

            // Update projectsListBox.
            foreach (var project in database.Keys)
            {
                projectListBox.Items.Add(project);
            }
            return true;
        }
    }
}
