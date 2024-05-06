using dllRapportVisites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using dllRapportVisites;
using Newtonsoft.Json;
using System.Net;
using System.Windows.Markup;
using System.Net.Http;
using System.Collections.Specialized;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour majFamilleWindow.xaml
    /// </summary>
    public partial class majFamilleWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;
        public majFamilleWindow(WebClient wb, string site, Secretaire laSecretaire)
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

        private async void btnValider_Click(object sender, RoutedEventArgs e)
        {
            string hash = this.laSecretaire.getHashTicketMdp();
            string url = this.site + "famille";
            NameValueCollection parametre = new NameValueCollection();
            parametre.Add("ticket", hash);
            parametre.Add("libelle", this.txtLibFamille.Text);
            parametre.Add("idFamille", ((Famille)this.cmbFamille.SelectedItem).id);
            byte[] tab = await wb.UploadValuesTaskAsync(url, "POST", parametre);
            string reponse = UnicodeEncoding.UTF8.GetString(tab);
           // string ticket = reponse.Substring(2, reponse.Length - 2);
           // this.laSecretaire.ticket = ticket;

            MessageBox.Show("bien validé");
            this.Close();

        }



    }
}
