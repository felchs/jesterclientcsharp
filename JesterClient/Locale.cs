using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JesterClient
{
	public class Locale
	{
		public static Locale PORTUGUESE = new Locale("pt", "BR");
		public static Locale ENGLISH = new Locale("en", "EN");
		
		private string language;
		private string country;
		
		public Locale(String language, String country)
		{
			this.language = language;
			this.country = country;
		}
		
		public static Locale create(String language, String country)
		{
			return new Locale(language, country);
		}
		
		public string toString()
		{
			return language + "_" + country;
		}
		
		public string getLanguage()
		{
			return language;
		}
		
		public string getCountry()
		{
			return country;
		}
	}
}
