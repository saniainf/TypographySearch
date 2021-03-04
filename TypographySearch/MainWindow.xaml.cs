using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using TypographySearch.Models;

namespace TypographySearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DBUtils dB;
        readonly string queryStringClient = "SELECT [Id],[SName] FROM [Klient]";
        readonly string queryStringManager = "SELECT [Id],[LastName] FROM [Manager]";
        readonly Dictionary<int, string> client;
        readonly Dictionary<int, string> manager;

        public MainWindow()
        {
            InitializeComponent();

            client = new Dictionary<int, string>();
            manager = new Dictionary<int, string>();
            dB = new DBUtils();

            LoadDataBaseList();
            LoadManagerClientData();
        }

        private void btnClick_Click(object sender, RoutedEventArgs e)
        {
            listView.Items.Clear();

            string queryString = "SELECT [Id],[DateNach],[SName],[Primech],[IdKlient],[IdManager] FROM [ZakWork] " +
                                    "where CHARINDEX('" + txboxQuery.Text + "', SName) > 0 " +
                                    "or CHARINDEX('" + txboxQuery.Text + "', Primech) > 0";

            try
            {
                var connection = dB.GetConnection();
                OleDbCommand command = new OleDbCommand(queryString, connection);

                connection.Open();

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listView.Items.Add(new Order
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(2),
                        Date = reader.GetDateTime(1),
                        Description = reader.GetString(3),
                        Client = client[reader.GetInt32(4)],
                        Manager = manager[reader.GetInt32(5)]
                    });
                }
                reader.Close();

                connection.Close();

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
                LoadManagerClientData();
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

        private void LoadManagerClientData()
        {
            var connection = dB.GetConnection();
            OleDbCommand commandClientLoad = new OleDbCommand(queryStringClient, connection);
            OleDbCommand commandManagerLoad = new OleDbCommand(queryStringManager, connection);
            client.Clear();
            manager.Clear();

            connection.Open();

            OleDbDataReader reader = commandClientLoad.ExecuteReader();
            while (reader.Read())
            {
                client.Add(reader.GetInt32(0), reader.GetString(1));
            }
            reader.Close();

            reader = commandManagerLoad.ExecuteReader();
            while (reader.Read())
            {
                manager.Add(reader.GetInt32(0), reader.GetString(1));
            }
            reader.Close();

            connection.Close();

            lblStatus.Text = "Данные из " + dB.Database + " загружены";
        }
    }
}
