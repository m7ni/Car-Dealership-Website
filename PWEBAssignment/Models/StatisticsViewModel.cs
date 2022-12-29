namespace PWEBAssignment.Models
{
	public class StatisticsViewModel
	{

		public ProfitStat TotalLastSevenDays;
		public ProfitStat TotalLastThirtyDays;
		public ReservationsStat AverageLastThirtyDays;

	}

	public class ProfitStat
	{
		public double Profit;

		public ProfitStat(double profit)
		{
			Profit = profit;
		}
	}
	public class ReservationsStat
	{
		public double Reservations;
		public ReservationsStat(double reservations) { Reservations=reservations; }
	}
}
