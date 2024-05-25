using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour ModifierMedicamentWindow.xaml
    /// </summary>
    public partial class ModifierMedicamentWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;

        public ModifierMedicamentWindow(WebClient wb, string site, Secretaire laSecretaire)
        {
            InitializeComponent();
            this.wb = wb;
            this.site = site;
            this.laSecretaire = laSecretaire;
            getFamilles();
        }

        private async void getFamilles()
        {
            try
            {
                string hash = this.laSecretaire.getHashTicketMdp();
                string url = this.site + "familles?ticket=" + hash;
                string reponse = await this.wb.DownloadStringTaskAsync(url);
                dynamic d = JsonConvert.DeserializeObject(reponse);
                this.laSecretaire.ticket = d.ticket; // la secrétaire à jour
                string lesFamilles = d.familles.ToString();
                List<Famille> list = JsonConvert.DeserializeObject<List<Famille>>(lesFamilles);
                this.cmbFamille.ItemsSource = list;
                this.cmbFamille.DisplayMemberPath = "libelle";

            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    MessageBox.Show(((HttpWebResponse)ex.Response).StatusCode.ToString());
                }
            }
        }

        private async void cmbFamille_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
                string idFamille = ((Famille)cmbFamille.SelectedItem).id.ToString();
                string hash = this.laSecretaire.getHashTicketMdp();
                string url = this.site + "medicaments?ticket=" + hash + "&idFamille=" + idFamille;

                 string reponse = await this.wb.DownloadStringTaskAsync(url);
                dynamic d = JsonConvert.DeserializeObject(reponse);
                this.laSecretaire.ticket = d.ticket; // la secrétaire à jour
                string Medicaments = d.medicaments.ToString();
                List<Medicament> list = JsonConvert.DeserializeObject<List<Medicament>>(Medicaments);
                this.cmbMedicament.ItemsSource = list;
                this.cmbMedicament.DisplayMemberPath = "nomCommercial";

            
        }

        private void cmbMedicament_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cmbMedicament.SelectedItem != null)
            {
                string Composition = ((Medicament)cmbMedicament.SelectedItem).composition;
                string Effets = ((Medicament)cmbMedicament.SelectedItem).effets;
                string ContreIndication = ((Medicament)cmbMedicament.SelectedItem).contreIndications;
                string idMedicament = ((Medicament)cmbMedicament.SelectedItem).id.ToString();
               
            
                if (string.IsNullOrEmpty(Composition) || string.IsNullOrEmpty(Effets) || string.IsNullOrEmpty(ContreIndication))
                {
                    MessageBox.Show("Ne pas laisser de champs vide !");
                    return;

                }

                string hash = this.laSecretaire.getHashTicketMdp();
                string url = this.site + "medicament?ticket=" + hash + "&idMedicament=" + idMedicament + "effets=" + Effets + "contreIndications=" + ContreIndication + "composition=" + Composition;

                NameValueCollection parametre = new NameValueCollection();
                parametre.Add("ticket", hash);
                parametre.Add("effets", Effets);
                parametre.Add("contreIndications", ContreIndication);
                parametre.Add("composition", Composition);
                parametre.Add("idMedicament", idMedicament);

                byte[] tab = await wb.UploadValuesTaskAsync(url, "POST", parametre);
                string reponse = UnicodeEncoding.UTF8.GetString(tab);
                string ticket = reponse.Substring(2, reponse.Length - 2);
                this.laSecretaire.ticket = ticket;
                MessageBox.Show("bien modifié");
                this.Close();
            }
            else
            {
                MessageBox.Show("Selectionner la famille du medicament et le medicament !");
                return;
            }
        }
    }
}


