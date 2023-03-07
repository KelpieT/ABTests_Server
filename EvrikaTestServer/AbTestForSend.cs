using System.Globalization;

namespace EvrikaTestServer
{
	public class AbTest
	{
		public int id;
		public string name;
		public string startDate;
		public string endDate;
		public string startCondition;
		public string endCondition;
		public AbTest(ABTest abTest)
		{
			this.id = abTest.id;
			this.name = abTest.name;
			this.startDate = abTest.startDate.ToString(CultureInfo.InvariantCulture);
			this.endDate = abTest.endDate.ToString(CultureInfo.InvariantCulture);
			this.startCondition = abTest.startCondition;
			this.endCondition = abTest.endCondition;
		}
	}
}

