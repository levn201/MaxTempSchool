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

            // Eventhandler hinzufügen, um Änderungen zu speichern
            MyComboBox.SelectionChanged += MyComboBox_SelectionChanged_1;

            // Beispieldaten in der ComboBox (Server S1, S2, S3)
            MyComboBox.Items.Add(new ComboBoxItem { Content = "S1" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "S2" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "S3" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "S4" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "SB" });
            MyComboBox.Items.Add(new ComboBoxItem { Content = "SD" });
        }

        // Methode, die bei einer Änderung der Auswahl aufgerufen wird
        private void MyComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)MyComboBox.SelectedItem;
            if (selectedItem != null)
            {
                selectedValue = selectedItem.Content.ToString();
            }
        }

        // Methode zum Filtern der Daten nach dem Server
        private void FilterAndDisplayDataByServer(string server)
        {
            // Überprüfen, ob die Datei existiert
            string filePath = @"temps.csv"; // Relativer Pfad zur CSV-Datei
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Datei nicht gefunden: {filePath}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 1. CSV-Datei einlesen
                using (FileStream infoAusgabe = new FileStream(filePath, FileMode.Open))
                using (StreamReader reader = new StreamReader(infoAusgabe))
                {
                    List<TempData> datenListe = new List<TempData>();

                    // 2. Zeilenweise die Daten lesen und nach dem Server filtern
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
                                tempData.Temperatur = double.Parse(werte[2]);
                                datenListe.Add(tempData);
                            }
                        }
                    }

                    // Gefilterte Daten ausgeben
                    if (datenListe.Any())
                    {
                        var sortedData = datenListe.OrderBy(d => d.Datum);

                        lblAusgabe.Text = string.Join("\n", sortedData.Select(d => $"{d.Server}, {d.Datum}, {d.Temperatur}"));
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

        // Eventhandler für den "Auswerten"-Button
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

        // Klasse zum Speichern der Temperaturdaten
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
