using System;

namespace Go.Tools
{
	public class DateTimeComponents
	{
		public DateTimeComponents()
		{
			dt = DateTime.Now;
		}
		public DateTimeComponents(DateTime dt)
		{
			this.dt = dt;
		}
		private DateTime	dt;

		public System.DateTime DateTime
		{
			get 
			{
				// Check if file is in local current day light saving time
				if (!TimeZone.CurrentTimeZone.IsDaylightSavingTime(dt))
				{
					// Not in day light saving time - adjust time
					return dt.AddHours(1);
				}
				else
				{
					// Day light saving time - don't adjust time
					return dt;
				}
			}
		}
		public int		Year				{ get { return this.DateTime.Year; } }
		public int		Month				{ get { return this.DateTime.Month; } }
		public int		Day					{ get { return this.DateTime.Day; } }
		public int		Hour				{ get { return this.DateTime.Hour; } }
		public int		Minute				{ get { return this.DateTime.Minute; } }
		public int		Second				{ get { return this.DateTime.Second; } }
		public string	UniversalDate		{ get { return string.Format("{0:yyyyMMdd}", this.DateTime); } }
		public string	FormattedDate		{ get { return string.Format("{0:dd-MM-yyyy}", this.DateTime); } }
		public string	UniversalTime		{ get { return string.Format("{0:HHmm}", this.DateTime); } }
		public string	FormattedTime		{ get { return string.Format("{0:HH:mm}", this.DateTime); } }
		public string	UniversalDateTime	{ get { return string.Format("{0:yyyyMMddHHmm}", this.DateTime); } }
		public string	FormattedDateTime	{ get { return string.Format("{0:dd-MM-yyyy HH:mm}", this.DateTime); } }
		public string	FormattedDateTimeDay{ get { return string.Format("{0:ddd dd-MM-yyyy HH:mm}", this.DateTime); } }
	}
}
