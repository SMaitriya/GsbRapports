﻿using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using dllRapportVisites;
using Newtonsoft.Json;

namespace GsbRapports
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebClient wb;
        private string site;
        private string ticket;
        private Secretaire laSecretaire;
        public MainWindow()
        {

            InitializeComponent();
            this.wb = new WebClient();
            this.site = ConfigurationManager.AppSettings.Get("srvLocal");
            this.laSecretaire = new Secretaire();

            this.DckMenu.Visibility= Visibility.Hidden;
            this.imgLogo.Visibility = Visibility.Hidden;
            this.txtBonjour.Visibility = Visibility.Hidden; 

        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string mdp = this.txtMdp.Password;
                string login = this.txtLogin.Text;
                string reponse; // la réponse retournée  par le serveur
                /* Création de la requête*/
                string url = this.site + "login?login=" + login;
                /*Appel à l'objet wb pour récupérer le résultat de la requête*/
                reponse = this.wb.DownloadString(url);
                /* récupération, après désérialisation et conversion*/
                this.ticket = (string)JsonConvert.DeserializeObject(reponse);
                if (this.ticket == null)
                {
                    MessageBox.Show("erreur de Login");
                    this.txtLogin.Text = "";
                }
                else
                {
                    this.laSecretaire.ticket = this.ticket;
                    this.laSecretaire.mdp = mdp;
                    /* on appelle la fonction de la classe secrétaire qui va hasher ticket+mdp */
                    string hash = this.laSecretaire.getHashTicketMdp();
                    /*On crée la requête*/
                    url = this.site + "connexion?login=" + login + "&mdp=" + hash;
                    /* On récupère la réponse du serveur de type json */
                    reponse = this.wb.DownloadString(url);
                    /*On transforme la réponse json en objet Secrétaire!!*/
                    Secretaire s = JsonConvert.DeserializeObject<Secretaire>(reponse);
                    if (s == null)
                        MessageBox.Show("erreur de mot de passe!!");
                    else
                    {
                        /* On renseigne le champ de la secrétaire pour la passer aux formulaires*/
                        this.laSecretaire.nom = s.nom;
                        this.laSecretaire.prenom = s.prenom;
                        this.laSecretaire.mdp = this.txtMdp.Password;
                        this.laSecretaire.ticket = s.ticket;
                        this.txtBonjour.Visibility = Visibility.Visible;
                        this.txtBonjour.Text = "Bonjour " + this.laSecretaire.prenom + " " + this.laSecretaire.nom;
                        this.DckMenu.Visibility = Visibility.Visible;
                        this.imgLogo.Visibility = Visibility.Visible;
                        this.stPanel.Visibility = Visibility.Hidden;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                    MessageBox.Show(((HttpWebResponse)ex.Response).StatusCode.ToString());

            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            VoirFamillesWindow w = new VoirFamillesWindow(wb, site, laSecretaire);
        
            w.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            majFamilleWindow w = new majFamilleWindow( wb,  site,  laSecretaire);
            w.Show();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            ajoutFamilleWindow w = new ajoutFamilleWindow(wb, site, laSecretaire);
            w.Show();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            GererMedicamentWindow g = new GererMedicamentWindow(wb, site, laSecretaire);
            g.Show();
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            AjouterMedicamentWindow a = new AjouterMedicamentWindow(wb, site, laSecretaire);
            a.Show();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            ModifierMedicamentWindow m = new ModifierMedicamentWindow(wb, site, laSecretaire);
            m.Show();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e)
        {
            medicamentDateWindow d = new medicamentDateWindow(wb, site, laSecretaire);
            d.Show();
        }
    }
}
