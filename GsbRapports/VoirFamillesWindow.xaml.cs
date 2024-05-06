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

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour VoirFamillesWindow.xaml
    /// </summary>
    public partial class VoirFamillesWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;
        public VoirFamillesWindow(WebClient wb, string site, Secretaire laSecretaire)
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
                this.laSecretaire.ticket = d.ticket;
                string lesFamilles = d.familles.ToString();
                List<Famille> familles = JsonConvert.DeserializeObject<List<Famille>>(lesFamilles);
                this.dtgFamilles.ItemsSource = familles;


            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    MessageBox.Show(((HttpWebResponse)ex.Response).StatusCode.ToString());
                }
            }
        }

        // Classe fictive pour représenter les données de famille
        public class FamilyData
        {
            public string Name { get; set; }
            public int MemberCount { get; set; }
        }

    }
}
