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
    public class StatsWrapper
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// Classe stockant les informations des statistique par ID
    /// </summary>
    public class IDStatsWrapper
    {
        public int ID { get; set; }
        public int Count { get; set; }
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
            { EventEnum.LightOn, "Lumière allumée" },
            { EventEnum.LightOff, "Lumière fermée" },
            { EventEnum.LEDColor, "Changement de la couleur de la LED" },
            { EventEnum.DoorStatusChanged, "Changement du status de la porte" },
        };

        /// <summary>
        /// Dictionaire contenant les unités de temps pour le tri
        /// </summary>
        public static Dictionary<string, string> TimeMap = new Dictionary<string, string>()
        {
            { "day", "Trier par jour" },
            { "week", "Trier par semaine" },
            { "month", "Trier par mois" },
        };

        /// <summary>
        /// Dictionaire contenant les noms des mois par rapport à leur index
        /// </summary>
        public static Dictionary<int, string> MonthMap = new Dictionary<int, string>()
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
        public string Event { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; } = DateTime.Now.AddDays(-1);
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public string GraphTime { get; set; } = string.Empty;
        public List<ChartSeries> ChartData { get; set; } = new List<ChartSeries>();
        public string[] GraphXAxis { get; set; } = new string[1];

        public Stats() { }

        /// <summary>
        /// Tache qui s'occupe de mettre à jour le graphique
        /// </summary>
        public async Task UpdateGraph()
        {
            // On valide que les dates sont valide
            if(StartDate >= EndDate)
            {
                Snackbar.Add("La date de fin doit être supérieure à celle de début", MudBlazor.Severity.Error);
                return;
            }

            // Validation de l'évènement ainsi que de l'unité de temps
            if(string.IsNullOrEmpty(Event) || string.IsNullOrEmpty(GraphTime))
            {
                Snackbar.Add("Vous devez spécifier un évenement et une unité de temps", MudBlazor.Severity.Error);
                return;
            }

            await InvokeAsync(SpinnerService.Show);

            // On met a jour le graphique selon l'unité de temps
            switch(GraphTime)
            {
                case "day":
                    await UpdateGraphDay();
                    break;
                case "week":
                    await UpdateGraphWeek();
                    break;
                case "month":
                    await UpdateGraphMonth();
                    break;
            }

            await InvokeAsync(SpinnerService.Hide);
        }

        /// <summary>
        /// Méthode qui met à jour le graphique selon l'unité de temps (jour)
        /// </summary>
        public async Task UpdateGraphDay()
        {
            // Initialise nos listes
            ChartData.Clear();
            List<string> XAxis = new List<string>();
            List<double> ChartDataDouble = new List<double>();

            // Obtention des statistiques
            DBConnect context = new DBConnect();
            List<StatsWrapper> stats = await context.db.Events.AsNoTracking().Where(maisonEvent => maisonEvent.Event1 == Event && maisonEvent.Date >= StartDate && maisonEvent.Date <= EndDate).GroupBy(row => row.Date.Date).Select(group => new StatsWrapper(){ Date = group.Key, Count = group.Count() }).ToListAsync();

            // On calcule la différence de jours
            TimeSpan differenceOfDays = (TimeSpan)(EndDate! - StartDate!);
            DateTime startDateAxis = (DateTime)EndDate!;

            // On valide qu'il n'y a pas plus de 14 jours
            if(differenceOfDays.TotalDays > 14)
            {
                // On commence il y a 14 jours
                startDateAxis = startDateAxis.AddDays(-13);

                // On boucle dans les jours
                for(int i = 0; i < 14; i++)
                {
                    ChartDataDouble.Add(GetChartDataDayCount(ref stats, startDateAxis.Date));
                    XAxis.Add(startDateAxis.ToString("MM/dd"));
                    startDateAxis = startDateAxis.AddDays(1);
                }
            } else
            {
                // On commence à la date de début
                startDateAxis = startDateAxis.AddDays(-differenceOfDays.TotalDays);

                // On boucle dans les jours
                for(int i = 0; i < differenceOfDays.TotalDays; i++)
                {
                    ChartDataDouble.Add(GetChartDataDayCount(ref stats, startDateAxis.Date));
                    XAxis.Add(startDateAxis.ToString("MM/dd"));
                    startDateAxis = startDateAxis.AddDays(1);
                }
            }

            // On ajoute les statistiques dans les données du graphique
            ChartData.Add(new ChartSeries() { Name = EventMap[Event], Data = ChartDataDouble.ToArray() });

            // On met l'axe des X sous forme de tableau de string
            GraphXAxis = XAxis.ToArray();

            // On met à jour le graphique
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Méthode qui met à jour le graphique selon l'unité de temps (semaine)
        /// </summary>
        public async Task UpdateGraphWeek()
        {
            // Initialise nos listes
            ChartData.Clear();
            List<string> XAxis = new List<string>();
            List<double> ChartDataDouble = new List<double>();

            // Obtention des statistiques
            DBConnect context = new DBConnect();
            List<IDStatsWrapper> stats = context.db.Events.AsNoTracking().Where(maisonEvent => maisonEvent.Event1 == Event && maisonEvent.Date >= StartDate && maisonEvent.Date <= EndDate).AsEnumerable().GroupBy(row => GetWeekIndex(row.Date.Date)).Select(group => new IDStatsWrapper() { ID = group.Key, Count = group.Count() }).ToList();

            // On calcul la différence de jours
            TimeSpan differenceOfDays = (TimeSpan)(EndDate! - StartDate!);
            DateTime startDateAxis = (DateTime)EndDate!;

            // On valide si il y a plus de 12 semaines
            if ((differenceOfDays.TotalDays / 7) > 12)
            {
                // On commence 12 semaines dans le passée
                startDateAxis = startDateAxis.AddDays(-(11 * 7));

                // On boucle dans les semaines
                for (int i = 0; i < 12; i++)
                {
                    ChartDataDouble.Add(GetChartDataIndexCount(ref stats, GetWeekIndex(startDateAxis)));
                    XAxis.Add("Sem. " + GetWeekIndex(startDateAxis));
                    startDateAxis = startDateAxis.AddDays(7);
                }
            }
            else
            {
                // On commence à la date de début
                startDateAxis = startDateAxis.AddDays(-differenceOfDays.TotalDays);

                // On boucle dans les semaines
                for (int i = 0; i < Math.Ceiling(differenceOfDays.TotalDays / 7); i++)
                {
                    ChartDataDouble.Add(GetChartDataIndexCount(ref stats, GetWeekIndex(startDateAxis)));
                    XAxis.Add("Sem. " + GetWeekIndex(startDateAxis));
                    startDateAxis = startDateAxis.AddDays(7);
                }
            }

            // On ajoute les statistiques dans les données du graphique
            ChartData.Add(new ChartSeries() { Name = EventMap[Event], Data = ChartDataDouble.ToArray() });

            // On met l'axe des X sous forme de tableau de string
            GraphXAxis = XAxis.ToArray();

            // On met à jour le graphique
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Méthode qui met à jour le graphique selon l'unité de temps (mois)
        /// </summary>
        public async Task UpdateGraphMonth()
        {
            // Initialise nos listes
            ChartData.Clear();
            List<string> XAxis = new List<string>();
            List<double> ChartDataDouble = new List<double>();

            // Obtention des statistiques
            DBConnect context = new DBConnect();
            List<IDStatsWrapper> stats = await context.db.Events.AsNoTracking().Where(maisonEvent => maisonEvent.Event1 == Event && maisonEvent.Date >= StartDate && maisonEvent.Date <= EndDate).GroupBy(row => row.Date.Date.Month).Select(group => new IDStatsWrapper() { ID = group.Key, Count = group.Count() }).ToListAsync();

            // Calcul de la différence de mois
            int monthDifference = EndDate!.Value.Year * 12 + EndDate!.Value.Month - (StartDate!.Value.Year * 12 + StartDate!.Value.Month);
            DateTime startDateAxis = (DateTime)EndDate!;

            // Si la différence des mois est supérieur à 12, on prends les 12 premiers mois
            if (monthDifference > 12)
            {
                // On met la date selon le début
                startDateAxis = startDateAxis.AddMonths(-11);

                // On boucle pour chaque mois 
                for (int i = 0; i < 12; i++)
                {
                    ChartDataDouble.Add(GetChartDataIndexCount(ref stats, startDateAxis.Date.Month));
                    XAxis.Add(MonthMap[startDateAxis.Date.Month]);
                    startDateAxis = startDateAxis.AddMonths(1);
                }
            }
            else // Sinon on fait les mois entre les deux dates sélectionnés
            {
                // On met la date selon le début
                startDateAxis = startDateAxis.AddMonths(-monthDifference);

                // On boucle pour chaque mois 
                for (int i = 0; i < monthDifference + 1; i++)
                {
                    ChartDataDouble.Add(GetChartDataIndexCount(ref stats, startDateAxis.Date.Month));
                    XAxis.Add(MonthMap[startDateAxis.Date.Month]);
                    startDateAxis = startDateAxis.AddMonths(1);
                }
            }

            // On ajoute les statistiques dans les données du graphique
            ChartData.Add(new ChartSeries() { Name = EventMap[Event], Data = ChartDataDouble.ToArray() });

            // On met l'axe des X sous forme de tableau de string
            GraphXAxis = XAxis.ToArray();

            // On met à jour le graphique
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Méthode qui permet d'obtenir l'index de semaine d'une date
        /// </summary>
        /// <param name="time">DateTime, Date qu'on veut savoir l'index de semaine</param>
        /// <returns>int, l'index de la semaine dans l'année</returns>
        private int GetWeekIndex(DateTime time)
        {
            // On obtient le jour de la semaine
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            
            // Formattage pour toujours avoir le bon index
            if(day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // On retourne l'index
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Méthode qui sert à obtenir les statistique à partir d'une Date
        /// </summary>
        /// <param name="stats">ref List, Référence d'une liste contenant les statistiques</param>
        /// <param name="date">DateTime, Date à chercher</param>
        /// <returns>int, La quantité de l'évenement, si introuvable on retourne 0</returns>
        private int GetChartDataDayCount(ref List<StatsWrapper> stats, DateTime date)
        {
            StatsWrapper? statsWrapper = stats.SingleOrDefault(stat => stat.Date.Date == date.Date);
            if (statsWrapper != null)
            {
                return statsWrapper.Count;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Méthode qui sert à obtenir les statistique à partir d'un ID
        /// </summary>
        /// <param name="stats">ref List, Référence d'une liste contenant les statistiques</param>
        /// <param name="Index">int, Index à chercher</param>
        /// <returns>int, La quantité de l'évenement, si introuvable on retourne 0</returns>
        private int GetChartDataIndexCount(ref List<IDStatsWrapper> stats, int Index)
        {
            IDStatsWrapper? statsWrapper = stats.SingleOrDefault(stat => stat.ID == Index);
            if (statsWrapper != null)
            {
                return statsWrapper.Count;
            }
            else
            {
                return 0;
            }
        }
    }
}
