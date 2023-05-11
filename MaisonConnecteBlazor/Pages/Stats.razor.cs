using MaisonConnecteBlazor.Components.Base;
using MaisonConnecteBlazor.Database;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using System.Globalization;

namespace MaisonConnecteBlazor.Pages
{
    /// <summary>
    /// Classe stockant les informations des statistique par date
    /// </summary>
    public class EmballageStatistiques
    {
        public DateTime Date { get; set; }
        public int Quantite { get; set; }
    }

    /// <summary>
    /// Classe stockant les informations des statistique par ID
    /// </summary>
    public class EmballageStatistiquesID
    {
        public int ID { get; set; }
        public int Quantite { get; set; }
    }

    /// <summary>
    /// Classe gérant la page de statistique
    /// </summary>
    public partial class Stats : MaisonConnecteBase
    {
        /// <summary>
        /// Dictionaire qui contient les évenements qui sont visible avec les statistiques
        /// </summary>
        public static Dictionary<string, string> EventMap = new Dictionary<string, string>()
        {
            { EventEnum.LumiereAllume, "Lumière allumée" },
            { EventEnum.LumiereFerme, "Lumière fermée" },
            { EventEnum.CouleurLED, "Changement de la couleur de la LED" },
            { EventEnum.StatusPorteChange, "Changement du status de la porte" },
        };

        /// <summary>
        /// Dictionaire contenant les unités de temps pour le tri
        /// </summary>
        public static Dictionary<string, string> CarteTemps = new Dictionary<string, string>()
        {
            { "day", "Trier par jour" },
            { "week", "Trier par semaine" },
            { "month", "Trier par mois" },
        };

        /// <summary>
        /// Dictionaire contenant les noms des mois par rapport à leur index
        /// </summary>
        public static Dictionary<int, string> CarteMois = new Dictionary<int, string>()
        {
            { 1, "Jan." },
            { 2, "Fev." },
            { 3, "Mar." },
            { 4, "Avr." },
            { 5, "Mai" },
            { 6, "Juin" },
            { 7, "Juil." },
            { 8, "Août" },
            { 9, "Sep." },
            { 10, "Oct." },
            { 11, "Nov." },
            { 12, "Dec." },
        };

        // Déclaration des variables
        public string Evenement { get; set; } = string.Empty;
        public DateTime? DateDebut { get; set; } = DateTime.Now.AddDays(-1);
        public DateTime? DateFin { get; set; } = DateTime.Now;
        public string TempsGraphique { get; set; } = string.Empty;
        public List<ChartSeries> DonneesGraphique { get; set; } = new List<ChartSeries>();
        public string[] AxeXGraphique { get; set; } = new string[1];

        private DateTime RealDateDebut;
        private DateTime RealDateFin;

        public Stats() { }

        /// <summary>
        /// Tache qui s'occupe de mettre à jour le graphique
        /// </summary>
        public async Task MiseAJourGraphique()
        {
            // On valide que les dates sont valide
            if(DateDebut >= DateFin)
            {
                Snackbar.Add("La date de fin doit être supérieure à celle de début", MudBlazor.Severity.Error);
                return;
            }

            RealDateDebut = (DateTime)DateDebut!;
            RealDateDebut = RealDateDebut.Date;
            RealDateFin = (DateTime)DateFin!;
            RealDateFin = RealDateFin.Date;

            // Validation de l'évènement ainsi que de l'unité de temps
            if(string.IsNullOrEmpty(Evenement) || string.IsNullOrEmpty(TempsGraphique))
            {
                Snackbar.Add("Vous devez spécifier un évenement et une unité de temps", MudBlazor.Severity.Error);
                return;
            }

            await InvokeAsync(SpinnerService.Show);

            // On met a jour le graphique selon l'unité de temps
            switch(TempsGraphique)
            {
                case "day":
                    await MiseAJourGraphiqueJour();
                    break;
                case "week":
                    await MiseAJourGraphiqueSemaine();
                    break;
                case "month":
                    await MiseAJourGraphiqueMois();
                    break;
            }

            await InvokeAsync(SpinnerService.Hide);
        }

        /// <summary>
        /// Méthode qui met à jour le graphique selon l'unité de temps (jour)
        /// </summary>
        public async Task MiseAJourGraphiqueJour()
        {
            // Initialise nos listes
            DonneesGraphique.Clear();
            List<string> AxeX = new List<string>();
            List<double> DonneesGraphiqueDouble = new List<double>();

            // Obtention des statistiques
            DBConnect context = new DBConnect();
            List<EmballageStatistiques> statistiques = await context.db.Events.AsNoTracking().Where(maisonEvent => maisonEvent.Event1 == Evenement && maisonEvent.Date.Date >= RealDateDebut && maisonEvent.Date.Date <= RealDateFin).GroupBy(row => row.Date.Date).Select(group => new EmballageStatistiques(){ Date = group.Key, Quantite = group.Count() }).ToListAsync();

            // On calcule la différence de jours
            TimeSpan differencesDeJours = (TimeSpan)(DateFin! - DateDebut!);
            DateTime dateDebutAxe = (DateTime)DateFin!;

            // On valide qu'il n'y a pas plus de 14 jours
            if(differencesDeJours.TotalDays > 14)
            {
                // On commence il y a 14 jours
                dateDebutAxe = dateDebutAxe.AddDays(-13);

                // On boucle dans les jours
                for(int i = 0; i < 14; i++)
                {
                    DonneesGraphiqueDouble.Add(ObtenirDonneesGraphiqueJourCompteur(ref statistiques, dateDebutAxe.Date));
                    AxeX.Add(dateDebutAxe.ToString("MM/dd"));
                    dateDebutAxe = dateDebutAxe.AddDays(1);
                }
            } else
            {
                // On commence à la date de début
                dateDebutAxe = dateDebutAxe.AddDays(-differencesDeJours.TotalDays);

                // On boucle dans les jours
                for(int i = 0; i < differencesDeJours.TotalDays; i++)
                {
                    DonneesGraphiqueDouble.Add(ObtenirDonneesGraphiqueJourCompteur(ref statistiques, dateDebutAxe.Date));
                    AxeX.Add(dateDebutAxe.ToString("MM/dd"));
                    dateDebutAxe = dateDebutAxe.AddDays(1);
                }
            }

            // On ajoute les statistiques dans les données du graphique
            DonneesGraphique.Add(new ChartSeries() { Name = EventMap[Evenement], Data = DonneesGraphiqueDouble.ToArray() });

            // On met l'axe des X sous forme de tableau de string
            AxeXGraphique = AxeX.ToArray();

            // On met à jour le graphique
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Méthode qui met à jour le graphique selon l'unité de temps (semaine)
        /// </summary>
        public async Task MiseAJourGraphiqueSemaine()
        {
            // Initialise nos listes
            DonneesGraphique.Clear();
            List<string> AxeX = new List<string>();
            List<double> DonneesGraphiqueDouble = new List<double>();

            // Obtention des statistiques
            DBConnect context = new DBConnect();
            List<EmballageStatistiquesID> statistiques = context.db.Events.AsNoTracking().Where(maisonEvent => maisonEvent.Event1 == Evenement && maisonEvent.Date.Date >= RealDateDebut && maisonEvent.Date.Date <= RealDateFin).AsEnumerable().GroupBy(row => ObtenirIndexSemaine(row.Date.Date)).Select(group => new EmballageStatistiquesID() { ID = group.Key, Quantite = group.Count() }).ToList();

            // On calcul la différence de jours
            TimeSpan differenceDeJours = (TimeSpan)(DateFin! - DateDebut!);
            DateTime dateDebutAxe = (DateTime)DateFin!;

            // On valide si il y a plus de 12 semaines
            if ((differenceDeJours.TotalDays / 7) > 12)
            {
                // On commence 12 semaines dans le passée
                dateDebutAxe = dateDebutAxe.AddDays(-(11 * 7));

                // On boucle dans les semaines
                for (int i = 0; i < 12; i++)
                {
                    DonneesGraphiqueDouble.Add(ObtenirDonneesGraphiqueIndexCompteur(ref statistiques, ObtenirIndexSemaine(dateDebutAxe)));
                    AxeX.Add("Sem. " + ObtenirIndexSemaine(dateDebutAxe));
                    dateDebutAxe = dateDebutAxe.AddDays(7);
                }
            }
            else
            {
                // On commence à la date de début
                dateDebutAxe = dateDebutAxe.AddDays(-differenceDeJours.TotalDays);

                // On boucle dans les semaines
                for (int i = 0; i < Math.Ceiling(differenceDeJours.TotalDays / 7); i++)
                {
                    DonneesGraphiqueDouble.Add(ObtenirDonneesGraphiqueIndexCompteur(ref statistiques, ObtenirIndexSemaine(dateDebutAxe)));
                    AxeX.Add("Sem. " + ObtenirIndexSemaine(dateDebutAxe));
                    dateDebutAxe = dateDebutAxe.AddDays(7);
                }
            }

            // On ajoute les statistiques dans les données du graphique
            DonneesGraphique.Add(new ChartSeries() { Name = EventMap[Evenement], Data = DonneesGraphiqueDouble.ToArray() });

            // On met l'axe des X sous forme de tableau de string
            AxeXGraphique = AxeX.ToArray();

            // On met à jour le graphique
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Méthode qui met à jour le graphique selon l'unité de temps (mois)
        /// </summary>
        public async Task MiseAJourGraphiqueMois()
        {
            // Initialise nos listes
            DonneesGraphique.Clear();
            List<string> AxeX = new List<string>();
            List<double> DonneesGraphiqueDouble = new List<double>();

            // Obtention des statistiques
            DBConnect context = new DBConnect();
            List<EmballageStatistiquesID> statistiques = await context.db.Events.AsNoTracking().Where(maisonEvent => maisonEvent.Event1 == Evenement && maisonEvent.Date.Date >= RealDateDebut && maisonEvent.Date.Date <= RealDateFin).GroupBy(row => row.Date.Date.Month).Select(group => new EmballageStatistiquesID() { ID = group.Key, Quantite = group.Count() }).ToListAsync();

            // Calcul de la différence de mois
            int differenceMois = DateFin!.Value.Year * 12 + DateFin!.Value.Month - (DateDebut!.Value.Year * 12 + DateDebut!.Value.Month);
            DateTime dateDebutAxis = (DateTime)DateFin!;

            // Si la différence des mois est supérieur à 12, on prends les 12 premiers mois
            if (differenceMois > 12)
            {
                // On met la date selon le début
                dateDebutAxis = dateDebutAxis.AddMonths(-11);

                // On boucle pour chaque mois 
                for (int i = 0; i < 12; i++)
                {
                    DonneesGraphiqueDouble.Add(ObtenirDonneesGraphiqueIndexCompteur(ref statistiques, dateDebutAxis.Date.Month));
                    AxeX.Add(CarteMois[dateDebutAxis.Date.Month]);
                    dateDebutAxis = dateDebutAxis.AddMonths(1);
                }
            }
            else // Sinon on fait les mois entre les deux dates sélectionnés
            {
                // On met la date selon le début
                dateDebutAxis = dateDebutAxis.AddMonths(-differenceMois);

                // On boucle pour chaque mois 
                for (int i = 0; i < differenceMois + 1; i++)
                {
                    DonneesGraphiqueDouble.Add(ObtenirDonneesGraphiqueIndexCompteur(ref statistiques, dateDebutAxis.Date.Month));
                    AxeX.Add(CarteMois[dateDebutAxis.Date.Month]);
                    dateDebutAxis = dateDebutAxis.AddMonths(1);
                }
            }

            // On ajoute les statistiques dans les données du graphique
            DonneesGraphique.Add(new ChartSeries() { Name = EventMap[Evenement], Data = DonneesGraphiqueDouble.ToArray() });

            // On met l'axe des X sous forme de tableau de string
            AxeXGraphique = AxeX.ToArray();

            // On met à jour le graphique
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Méthode qui permet d'obtenir l'index de semaine d'une date
        /// </summary>
        /// <param name="temps">DateTime, Date qu'on veut savoir l'index de semaine</param>
        /// <returns>int, l'index de la semaine dans l'année</returns>
        private int ObtenirIndexSemaine(DateTime temps)
        {
            // On obtient le jour de la semaine
            DayOfWeek jour = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(temps);
            
            // Formattage pour toujours avoir le bon index
            if(jour >= DayOfWeek.Monday && jour <= DayOfWeek.Wednesday)
            {
                temps = temps.AddDays(3);
            }

            // On retourne l'index
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(temps, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Méthode qui sert à obtenir les statistique à partir d'une Date
        /// </summary>
        /// <param name="statistiques">ref List, Référence d'une liste contenant les statistiques</param>
        /// <param name="date">DateTime, Date à chercher</param>
        /// <returns>int, La quantité de l'évenement, si introuvable on retourne 0</returns>
        private int ObtenirDonneesGraphiqueJourCompteur(ref List<EmballageStatistiques> statistiques, DateTime date)
        {
            EmballageStatistiques? emballageStatistiques = statistiques.SingleOrDefault(stat => stat.Date.Date == date.Date);
            if (emballageStatistiques != null)
            {
                return emballageStatistiques.Quantite;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Méthode qui sert à obtenir les statistique à partir d'un ID
        /// </summary>
        /// <param name="statistiques">ref List, Référence d'une liste contenant les statistiques</param>
        /// <param name="Index">int, Index à chercher</param>
        /// <returns>int, La quantité de l'évenement, si introuvable on retourne 0</returns>
        private int ObtenirDonneesGraphiqueIndexCompteur(ref List<EmballageStatistiquesID> statistiques, int Index)
        {
            EmballageStatistiquesID? emballageStatistiques = statistiques.SingleOrDefault(stat => stat.ID == Index);
            if (emballageStatistiques != null)
            {
                return emballageStatistiques.Quantite;
            }
            else
            {
                return 0;
            }
        }
    }
}
