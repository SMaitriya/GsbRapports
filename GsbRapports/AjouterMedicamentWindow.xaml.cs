using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour AjouterMedicamentWindow.xaml
    /// </summary>
    public partial class AjouterMedicamentWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;

        public AjouterMedicamentWindow(WebClient wb, string site, Secretaire laSecretaire)
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

        private void cmbFamille_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void  btnValider_Click(object sender, RoutedEventArgs e)
        {


            if (cmbFamille.SelectedItem != null)
            {
                string idMedicament = this.txtImma.Text;
                string Nom = this.txtNom.Text;
                string Effets = this.txtEffets.Text;
                string Composition = this.txtComp.Text;
                string ContreIndication = this.txtContreIndic.Text;
                string idFamille = ((Famille)cmbFamille.SelectedItem).id.ToString();

                string hash = this.laSecretaire.getHashTicketMdp();


                if (Regex.IsMatch(idMedicament, )
                if () if (Regex.IsMatch(this.txtId.Text, @"^[A-Z]{3}$") && (this.txtId.Text[0] == this.txtLibelle.Text[0]))

                        if (string.IsNullOrEmpty(Nom) || string.IsNullOrEmpty(Effets) || string.IsNullOrEmpty(Composition) || string.IsNullOrEmpty(ContreIndication) || string.IsNullOrEmpty(idMedicament))

                {
                    MessageBox.Show("Ne pas laissez de champs vides !");
                    return;
                }


                
                else { 

                    string url = this.site + "medicaments?ticket=" + hash + "&idMedicament=" + idMedicament + "&effets=" + Effets + "&contreIndications=" + ContreIndication + "&composition=" + Composition + "&idFamille=" + idFamille;
                    NameValueCollection parametre = new NameValueCollection();
                    parametre.Add("ticket", hash);
                    parametre.Add("idMedicament", idMedicament);
                    parametre.Add("nomCommercial", Nom);
                    parametre.Add("effets", Effets);
                    parametre.Add("composition", Composition);
                    parametre.Add("contreIndications", ContreIndication);




                    byte[] tab = await wb.UploadValuesTaskAsync(url, "POST", parametre);
                    string reponse = UnicodeEncoding.UTF8.GetString(tab);
                    string ticket = reponse.Substring(2, reponse.Length - 2);
                    this.laSecretaire.ticket = ticket;

                    MessageBox.Show("bien ajouté");
                    this.Close();


                }
                

            }
            else
            {
                MessageBox.Show("Choisir une famille de medicament");
                return;
            }
             
            
        }

   
    }
}
