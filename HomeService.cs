namespace Project.BusinessComponents.Services
{
	#region Usings

	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;

	using Project.BusinessComponents.Interfaces;
	using Project.BusinessComponents.Interfaces.Database;
	using Project.BusinessComponents.Interfaces.Services;

	using Project.BusinessEntities.Builders;
	using Project.BusinessEntities.DataContracts;
	using Project.BusinessEntities.DataContracts.Enumerations;
	using Project.BusinessEntities.DataContracts.Routes;
	using Project.BusinessEntities.Models;
	using Project.BusinessEntities.Models.Constants;
	using Project.BusinessEntities.Requests;

	#endregion

	/// <summary>
	/// Implements the <see cref="IHomeService" /> interface.
	/// </summary>
	public class HomeService : IHomeService
	{
		#region Constans
		
		private const PhoneNumberFormat = "+375-({0})-{1}";
		
		#endregion
		
		#region Private Fields

		/// <summary>
		/// Reference to the <see cref="IHomeDatabaseService" /> implementer.
		/// </summary>
		private readonly IHomeDatabaseService databaseService;

		/// <summary>
		/// Reference to the <see cref="ILogger" /> implementer.
		/// </summary>
		private readonly ILogger logger;

		/// <summary>
		/// Reference to the <see cref="IMailService" /> implementer.
		/// </summary>
		private readonly IMailService mailService;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="HomeService" /> class.
		/// </summary>
		/// <param name="databaseService">The database service.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="mailService">The mail service.</param>
		public HomeService(
			IHomeDatabaseService databaseService,
			ILogger logger,
			IMailService mailService)
		{
			Contract.Requires(databaseService != null, "databaseService");
			Contract.Requires(logger != null, "logger");
			Contract.Requires(mailService != null, "mailService");

			this.databaseService = databaseService;
			this.logger = logger;
			this.mailService = mailService;
		}

		#endregion

		#region IHomeService Members

		/// <summary>
		/// Gets the home page landing.
		/// </summary>
		/// <returns>
		/// Content of the home landing page.
		///</returns>
		public string GetHomePageLanding()
		{
			var homeArticle = this.databaseService.GetArticles(ArticleType.HomePageLanding).FirstOrDefault();

			return homeArticle == null ? null : homeArticle.Content;
		}

		/// <summary>
		/// Gets the published messages.
		/// </summary>
		/// <returns>
		/// Collection of published messages.
		/// </returns>
		public IEnumerable<PublishedMessage> GetPublishedMessages()
		{
			return this.databaseService.GetPublishedMessages();
		}

		/// <summary>
		/// Gets the articles.
		/// </summary>
		/// <param name="type">The type of the articles to be selected.</param>
		/// <returns>
		/// Collection of published articles.
		/// </returns>
		public IEnumerable<Article> GetArticles(ArticleType type)
		{
			return this.databaseService.GetArticles(type);
		}

		/// <summary>
		/// Gets the article.
		/// </summary>
		/// <param name="type">The type of the article to be selected.</param>
		/// <param name="name">The name of the article to be selected.</param>
		/// <returns>
		/// The article.
		/// </returns>
		public Article GetArticle(ArticleType type, string name)
		{
			return this.databaseService.GetArticle(type, name);
		}

		/// <summary>
		/// Gets the route information.
		/// </summary>
		/// <returns>
		/// Collection of route information entities.
		/// </returns>
		public IEnumerable<RouteInfo> GetRouteInfo()
		{
			return this.databaseService.GetRouteInfo();
		}

		/// <summary>
		/// Gets the route information.
		/// </summary>
		/// <param name="name">The name of the route.</param>
		/// <returns>
		/// The route information.
		/// </returns>
		public RouteInfo GetRouteInfo(string name)
		{
			return this.databaseService.GetRouteInfo(name);
		}

		/// <summary>
		/// Gets the route info.
		/// </summary>
		/// <param name="startName">The name of the start route point.</param>
		/// <param name="endName">The name of the end route point.</param>
		/// <returns>
		/// The route info.
		/// </returns>
		public RouteInfo GetRouteInfo(string startName, string endName)
		{
			string routeName =
				this.databaseService.GetRouteInfo()
					.Where(t => t.Start.Name == startName && t.End.Name == endName)
					.Select(t => t.Link)
					.FirstOrDefault();

			if (string.IsNullOrWhiteSpace(routeName))
			{
				return new RouteInfo();
			}

			return this.GetRouteInfo(routeName);
		}

		/// <summary>
		/// Gets the contacts.
		/// </summary>
		/// <returns>
		/// The contacts view.
		/// </returns>
		public Article GetContacts()
		{
			return this.databaseService.GetContacts();
		}

		/// <summary>
		/// Gets the application form text.
		/// </summary>
		/// <returns>
		/// The text of the application form.
		/// </returns>
		public string GetApplicationFormText()
		{
			IEnumerable<AdminSettings> adminSettings = this.databaseService.GetAdminSettings();

			return GetSettingsValue(adminSettings, AdminSettingsKeys.CompanyFormText);
		}

		/// <summary>
		/// Sends the application form.
		/// </summary>
		/// <param name="request">The request that contains data about creating application form.</param>
		public void SendApplicationForm(CreateFormRequest request)
		{
			string phones = string.Join(";", GetPhoneNumbers(request.SelectedOperactorCodes, request.Phones));

			var form = new Form()
			{
				DateTime = DateTime.Now.ToLocalTime(),
				CompanyName = request.CompanyName,
				AdditionalInfo = request.AdditionalInfo,
				Site = request.Site,
				Status = FormStatus.New,
				Phones = phones,
				Routes = string.Join(";", request.Routes)
			};

			this.databaseService.AddApplicationForm(form);

			string text =
				new StringBuilder()
					.AppendLine(request.CompanyName)
					.AppendLine(phones)
					.AppendLine(request.Site)
					.AppendLine(string.Join(";", request.Routes))
					.AppendLine(request.AdditionalInfo)
					.ToString();

			this.mailService.SendMail(text);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Gets the settings value.
		/// </summary>
		/// <param name="settings">The settings.</param>
		/// <param name="name">The name.</param>
		/// <returns>
		/// Settings value.
		/// </returns>
		private static string GetSettingsValue(IEnumerable<AdminSettings> settings, string name)
		{
			AdminSettings item = settings.FirstOrDefault(s => s.Name == name);

			return item != null ? item.Value : string.Empty;
		}

		/// <summary>
		/// Gets the key from pairs.
		/// </summary>
		/// <param name="settings">The settings.</param>
		/// <param name="name">The name.</param>
		/// <returns>
		/// Company settings value.
		/// </returns>
		private static string GetSettingsValue(IEnumerable<CompanySettings> settings, string name)
		{
			CompanySettings item = settings.FirstOrDefault(s => s.Name == name);

			return (item != null) ? item.Value : string.Empty;
		}

		/// <summary>
		/// Gets the collection of phone numbers as concatenation of codes and phones.
		/// </summary>
		/// <param name="codes">The collection of codes.</param>
		/// <param name="phones">The collection of phones.</param>
		/// <returns>
		/// Collection of phone numbers.
		/// </returns>
		private static IEnumerable<string> GetPhoneNumbers(IList<string> codes, IList<string> phones)
		{
			for (int i = 0; i < phones.Count(); i++)
			{
				yield return string.Format(PhoneNumberFormat, codes[i], phones[i]);
			}
		}

		#endregion
	}
}