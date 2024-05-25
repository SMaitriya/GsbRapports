using dllRapportVisites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Eventing.Reader;
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

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour ajoutFamilleWindow.xaml
    /// </summary>
    public partial class ajoutFamilleWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;
        public ajoutFamilleWindow(WebClient wb, string site, Secretaire laSecretaire)
        {
            InitializeComponent();
            this.wb = wb;
            this.site = site;
            this.laSecretaire = laSecretaire;
        }

  

        private async void btnValider_Click(object sender, RoutedEventArgs e)
        {
            string hash = this.laSecretaire.getHashTicketMdp();
            string url = this.site + "familles?ticket=" + hash;
            NameValueCollection parametre = new NameValueCollection();
            if (Regex.IsMatch(this.txtId.Text, @"^[A-Z]{3}$") && (this.txtId.Text[0] == this.txtLibelle.Text[0]))

            {
                parametre.Add("ticket", hash);
                parametre.Add("libelle", this.txtLibelle.Text);
                parametre.Add("idFamille", this.txtId.Text);

                byte[] tab = await wb.UploadValuesTaskAsync(url, "POST", parametre);
                string reponse = UnicodeEncoding.UTF8.GetString(tab);
                 string ticket = reponse.Substring(2, reponse.Length - 2);
                 this.laSecretaire.ticket = ticket;

                MessageBox.Show("bien ajouté");

            }
            else
            {
                MessageBox.Show("erreur");
            }
            
         
         
        }
    }
}
