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
                        string[] werte = zeile.Split(','); // Annahme: CSV-Werte durch Komma getrennt

                        // Fehlerprüfung: Sind genügend Spalten vorhanden?
                        if (werte.Length < 4)
                        {
                            continue; // Ungültige Zeile ignorieren
                        }

                        // Filter nach Server
                        if (werte[0] == server)
                        {
                            // Konvertierung der Temperatur. Wenn nicht konvertierbar, Zeile überspringen
                            if (int.TryParse(werte[3], out int temperatur))
                            {
                                // Konvertierung von Datum und Uhrzeit
                                if (DateTime.TryParse(werte[1], out DateTime datum) && TimeSpan.TryParse(werte[2], out TimeSpan uhrzeit))
                                {
                                    TempData daten = new TempData
                                    {
                                        Server = werte[0],          // 1. Spalte: Server
                                        Datum = datum,              // 2. Spalte: Datum
                                        Uhrzeit = uhrzeit,          // 3. Spalte: Uhrzeit
                                        Temperatur = temperatur      // 4. Spalte: Temperatur
                                    };

                                    datenListe.Add(daten); // Hinzufügen zur Liste
                                }
                            }
                        }
                    }

                    // 3. Gefilterte Daten ausgeben
                    if (datenListe.Count > 0)
                    {
                        StringBuilder ausgabeText = new StringBuilder();
                        foreach (var daten in datenListe)
                        {
                            ausgabeText.AppendLine($"Server: {daten.Server}, Datum: {daten.Datum.ToShortDateString()}, Uhrzeit: {daten.Uhrzeit}, Temperatur: {daten.Temperatur}°C");
                        }

                        lblAusgabe.Text = ausgabeText.ToString(); // Ausgabe in der TextBox
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
            public TimeSpan Uhrzeit { get; set; }
            public int Temperatur { get; set; }
        }
    }
}
