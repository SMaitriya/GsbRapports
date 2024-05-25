using dllRapportVisites;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour medicamentDateWindow.xaml
    /// </summary>
    public partial class medicamentDateWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;
        public medicamentDateWindow(WebClient wb, string site, Secretaire laSecretaire)
        {
            InitializeComponent();
            InitializeComponent();
            this.wb = wb;
            this.site = site;
            this.laSecretaire = laSecretaire;
            
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Selection la date a partir DatePickers
            DateTime? dateDebut = dpDateDebut.SelectedDate;
            DateTime? dateFin = dpDateFin.SelectedDate;

            if (dateDebut.HasValue && dateFin.HasValue)
            {
                string dateDebutStr = dateDebut.Value.ToString("yyyy-MM-dd");
                string dateFinStr = dateFin.Value.ToString("yyyy-MM-dd");

                string hash = this.laSecretaire.getHashTicketMdp();
                string url = $"{this.site}medicaments?ticket={hash}&dateDebut={dateDebutStr}&dateFin={dateFinStr}";
                string reponse = await this.wb.DownloadStringTaskAsync(url);
                dynamic d = JsonConvert.DeserializeObject(reponse);
                this.laSecretaire.ticket = d.ticket; // la secrétaire à jour
                string lesMedicaments = d.medicaments.ToString();
                List<Medicament> list = JsonConvert.DeserializeObject<List<Medicament>>(lesMedicaments);
                list = list.OrderBy(m => m.nomCommercial).ToList();
                this.dtg.ItemsSource = list;
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner des dates valides.");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Medicament> medicaments = dtg.ItemsSource as List<Medicament>;
                if (medicaments == null)
                {
                    MessageBox.Show("Aucun médicament à exporter.");
                    return;
                }

                // Créer un objet XmlSerializer pour sérialiser les médicaments
                XmlSerializer serializer = new XmlSerializer(typeof(List<Medicament>));

                // Afficher la boîte de dialogue de sauvegarde de fichier
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Fichier XML (*.xml)|*.xml";
                saveFileDialog.Title = "Enregistrer le fichier XML";
                saveFileDialog.ShowDialog();

                // Vérifier si l'utilisateur a sélectionné un emplacement de fichier
                if (!string.IsNullOrWhiteSpace(saveFileDialog.FileName))
                {
                    // Créer un XmlWriter à partir du nom de fichier
                    using (XmlWriter xmlWriter = XmlWriter.Create(saveFileDialog.FileName))
                    {
                        // Sérialiser les médicaments en utilisant l'XmlWriter
                        serializer.Serialize(xmlWriter, medicaments);
                    }

                    MessageBox.Show("Fichier Exporté avec succès.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Une erreur s'est produite lors de l'exportation des médicaments : " + ex.Message);
            }
        }
    }
    
}
