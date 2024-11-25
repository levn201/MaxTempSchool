using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MaxTemp
{
    public partial class MainWindow : Window
    {
        private string selectedValue;

        public MainWindow()
        {
            InitializeComponent();

            MyComboBox.SelectionChanged += MyComboBox_SelectionChanged_1;

            MyComboBox.Items.Add(new ComboBoxItem { Content = "S1" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "S2" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "S3" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "S4" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "SB" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "SD" });

        }

        private void MyComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)MyComboBox.SelectedItem;
            if (selectedItem != null)
            {
                selectedValue = selectedItem.Content.ToString();
            }
        }

        private void FilterAndDisplayDataByServer(string server)
        {

            string filePath = @"temps.csv";
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Datei nicht gefunden: {filePath}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {

                using (FileStream infoAusgabe = new FileStream(filePath, FileMode.Open))
                using (StreamReader reader = new StreamReader(infoAusgabe))
                {
                    List<TempData> datenListe = new List<TempData>();


                    while (!reader.EndOfStream)
                    {
                        string zeile = reader.ReadLine();
                        string[] werte = zeile.Split(',');

                        if (werte.Length == 3)
                        {
                            if (werte[0] == selectedValue)
                            {
                                TempData tempData = new TempData();
                                tempData.Server = werte[0];
                                tempData.Datum = DateTime.Parse(werte[1]);
                                tempData.Temperatur = double.Parse(werte[2].Replace(".",","));
                                datenListe.Add(tempData);
                            }
                        }
                    }


                    if (datenListe.Any())
                    {
                        var sortedData = datenListe.OrderByDescending(t => t.Temperatur); //Sotierung nach entweder dem Datum oder der Temperatur 

                        lblAusgabe.Text =string.Join("\n", sortedData.Select(d => $"{d.Server}, {d.Datum}, {d.Temperatur}"));
                    }
                    else
                    {
                        lblAusgabe.Text = $"Keine Daten für Server {server} gefunden.";
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Einlesen der Datei: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAuswerten_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedValue))
            {
                FilterAndDisplayDataByServer(selectedValue);
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie einen Server aus.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        public class TempData
        {
            public string Server { get; set; }
            public DateTime Datum { get; set; }
            public double Temperatur { get; set; }

            public override string ToString()
            {
                return $"{Server}, {Datum}, {Temperatur}";
            }
        }


        List<TempData> datenListe = new List<TempData>();

    }
}