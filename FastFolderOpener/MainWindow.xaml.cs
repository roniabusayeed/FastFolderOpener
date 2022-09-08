using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace FastFolderOpener
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string databaseFilePath = Constants.NO_DATABASE_ADDED;
        Dictionary<string, Dictionary<string, string>> database;
        public MainWindow()
        {
            InitializeComponent();
            Title = Constants.APPLICATION_TITLE;
            databaseTextBlock.Text = databaseFilePath;
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
            await loadAync();
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
                setDatabaseFilePath(openFileDialog.FileName);
                await loadAync();
            }
        }

        private async Task loadAync()
        {
            projectListBox.Items.Clear();
            categoriesListBox.Items.Clear();

            // Load database.
            database = await DatabaseManager.DatabaseManager
                .GetDatabaseManager(databaseFilePath)
                .LoadAsync();

            if (database is null)
            {
                MessageBox.Show(Constants.DATABASE_LOADING_ERROR_MESSAGE,
                    Constants.APPLICATION_TITLE);
                return;
            }

            // Update projectsListBox.
            foreach (var project in database.Keys)
            {
                projectListBox.Items.Add(project);
            }
        }
    }
}
