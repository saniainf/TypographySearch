using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Windows;
using TypographySearch.Models;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;

namespace TypographySearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DBUtils dB;

        public MainWindow()
        {
            InitializeComponent();

            dB = new DBUtils();

            LoadDataBaseList();
        }

        private string GetQueryString()
        {
            var queryBuild = new StringBuilder();
            var words = GetQueryWords();
            if (words.Count == 0) return string.Empty;
            queryBuild.Append("SELECT ");
            queryBuild.Append("ZakWork.Id, ");
            queryBuild.Append("ZakWork.SName as OrderName, ");
            queryBuild.Append("ZakWork.DateNach, ");
            queryBuild.Append("ZakWork.Primech, ");
            queryBuild.Append("Klient.SName, ");
            queryBuild.Append("Manager.LastName ");
            queryBuild.Append("FROM ZakWork ");
            queryBuild.Append("LEFT JOIN Manager ON ZakWork.IdManager = Manager.Id ");
            queryBuild.Append("LEFT JOIN Klient ON ZakWork.IdKlient = Klient.Id ");
            queryBuild.Append("WHERE ");
            for (int i = 0; i < words.Count; i++)
            {
                queryBuild.Append("(CHARINDEX('" + words[i] + "', ZakWork.SName) > 0");
                queryBuild.Append("OR CHARINDEX('" + words[i] + "', ZakWork.Primech) > 0");
                queryBuild.Append("OR CHARINDEX('" + words[i] + "', Klient.SName) > 0");
                queryBuild.Append("OR CHARINDEX('" + words[i] + "', Manager.LastName) > 0) ");
                if (i + 1 < words.Count)
                    queryBuild.Append("AND ");
            }
            return queryBuild.ToString();
        }

        private List<string> GetQueryWords()
        {
            return txboxQuery.Text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private void btnClick_Click(object sender, RoutedEventArgs e)
        {
            string query = GetQueryString();
            if (query == string.Empty) return;
            List<Order> orders = new List<Order>();
            try
            {
                var connection = dB.GetConnection();
                OleDbCommand command = new OleDbCommand(query, connection);

                connection.Open();
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    orders.Add(new Order
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Date = reader.GetDateTime(2),
                        Description = reader.GetString(3),
                        Client = reader.GetString(4),
                        Manager = reader.GetString(5)
                    });
                }
                reader.Close();
                connection.Close();

                listView.ItemsSource = orders;
                lblStatus.Text = "Поиск закончен";
            }
            catch (Exception exception)
            {
                lblStatus.Text = "Error: " + exception.Message;
            }
        }

        private void cmbDataBases_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbDataBases.SelectedItem.ToString() != dB.Database)
            {
                dB.Database = cmbDataBases.SelectedItem.ToString();
            }
        }

        private void LoadDataBaseList()
        {
            var connection = dB.GetConnection();
            connection.Open();
            DataTable myData = connection.GetSchema(OleDbMetaDataCollectionNames.Catalogs);
            connection.Close();
            foreach (DataRow row in myData.Rows)
                cmbDataBases.Items.Add(row[0]);
            cmbDataBases.SelectedItem = connection.Database;
        }

        #region column sorting

        GridViewColumnHeader _lastHeaderClicked = null;
        ListSortDirection _lastDirection = ListSortDirection.Ascending;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }
        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listView.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }
        #endregion
    }
}