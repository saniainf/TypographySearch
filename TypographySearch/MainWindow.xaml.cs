using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;
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

        public MainWindow()
        {
            InitializeComponent();

            dB = new DBUtils();

            LoadDataBaseList();
        }

        private void btnClick_Click(object sender, RoutedEventArgs e)
        {
            listView.Items.Clear();

            var queryBuild = new StringBuilder();

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
            queryBuild.Append("WHERE CHARINDEX('" + txboxQuery.Text + "', ZakWork.SName) > 0");
            queryBuild.Append("OR CHARINDEX('" + txboxQuery.Text + "', ZakWork.Primech) > 0");
            queryBuild.Append("OR CHARINDEX('" + txboxQuery.Text + "', Klient.SName) > 0");
            queryBuild.Append("OR CHARINDEX('" + txboxQuery.Text + "', Manager.LastName) > 0");

            string queryString = queryBuild.ToString();

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
                        Name = reader.GetString(1),
                        Date = reader.GetDateTime(2),
                        Description = reader.GetString(3),
                        Client = reader.GetString(4),
                        Manager = reader.GetString(5)
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
    }
}
