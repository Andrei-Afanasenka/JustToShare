namespace Project.Controllers
{
	#region Usings

	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Text;
	using System.Web.Mvc;

	using Project.BusinessComponents.Interfaces;
	using Project.BusinessComponents.Interfaces.Services;

	using Project.BusinessEntities.DataContracts;
	using Project.BusinessEntities.DataContracts.Enumerations;
	using Project.BusinessEntities.Requests;

	using Project.ViewModels.Home;

	#endregion

	/// <summary>
	/// Home controller.
	/// </summary>
	public class HomeController : Controller
	{
		#region Private Fields

		/// <summary>
		/// Reference to the <see cref="IHomeService" /> implementer.
		/// </summary>
		private readonly IHomeService homeService;

		/// <summary>
		/// Reference to the <see cref="ILogger" /> implementer.
		/// </summary>
		private readonly ILogger logger;

		/// <summary>
		/// Reference to the <see cref="ICommonService" /> implementer.
		/// </summary>
		private readonly ICommonService commonService;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="HomeController" /> class.
		/// </summary>
		/// <param name="homeService">The home service.</param>
		/// <param name="logger">The logger.</param>
		/// <param name="commonService">The common service.</param>
		public HomeController(
			IHomeService homeService,
			ILogger logger,
			ICommonService commonService)
		{
			Contract.Requires(homeService != null, "homeService");
			Contract.Requires(logger != null, "logger");
			Contract.Requires(commonService != null, "commonService");

			this.homeService = homeService;
			this.logger = logger;
			this.commonService = commonService;
		}

		#endregion

		#region Action Methods

		/// <summary>
		/// Index action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/
		/// </remarks>
		public ActionResult Index()
		{
			return this.View();
		}

		/// <summary>
		/// About action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/About/
		/// </remarks>
		public ActionResult About()
		{
			return this.View();
		}

		/// <summary>
		/// Contacts action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/Contact/
		/// </remarks>
		public ActionResult Contacts()
		{
			return this.View(this.homeService.GetContacts());
		}

		/// <summary>
		/// GetPublishedMessages action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/GetPublishedMessages
		/// </remarks>
		public JsonResult GetPublishedMessages()
		{
			return this.Json(this.homeService.GetPublishedMessages(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// GetNews action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/GetNews
		/// </remarks>
		public JsonResult GetNews()
		{
			return this.Json(this.homeService.GetArticles(ArticleType.News), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// GetFAQ action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/GetFAQ
		/// </remarks>
		public JsonResult GetFAQ()
		{
			return this.Json(this.homeService.GetArticles(ArticleType.FAQ), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// Get Header Articles action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/GetHeaderArticles
		/// </remarks>
		public JsonResult GetHeaderArticles()
		{
			return this.Json(this.homeService.GetArticles(ArticleType.HeaderArticle), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// GetRouteInfo action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/GetRouteInfo
		/// </remarks>
		public JsonResult GetRouteInfo()
		{
			return this.Json(this.homeService.GetRouteInfo(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// News action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/News/id
		/// </remarks>
		public ActionResult News(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return this.View("Index");
			}

			Article article = this.homeService.GetArticle(ArticleType.News, id);

			return (article != null) ? this.View(article) : this.View("Index");
		}

		/// <summary>
		/// FAQ action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/FAQ/id
		/// </remarks>
		public ActionResult FAQ(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return this.View("Index");
			}

			Article article = this.homeService.GetArticle(ArticleType.FAQ, id);

			return (article != null) ? this.View(article) : this.View("Index");
		}

		/// <summary>
		/// Header Articles action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/HeaderArticles/id
		/// </remarks>
		public ActionResult HeaderArticles(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return this.View("Index");
			}

			Article article = this.homeService.GetArticle(ArticleType.HeaderArticle, id);

			return (article != null) ? this.View(article) : this.View("Index");
		}

		/// <summary>
		/// Route action.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="date">The date.</param>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/Route/id/date
		/// </remarks>
		public ActionResult Route(string id, string date)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				return this.View("Index");
			}

			RouteInfo routeInfo = this.homeService.GetRouteInfo(id);
			
			if (routeInfo == null)
			{
				return this.View("Index");
			}
			
			return this.View("Search", new SearchViewModel
			{
				From = routeInfo.Start.Name,
				To = routeInfo.End.Name,
				Content = routeInfo.Content,
				MetaKeyWords = routeInfo.MetaKeyWords,
				MetaDescription = routeInfo.MetaDescription,
				DateView = date
			});
		}

		/// <summary>
		/// Search action.
		/// </summary>
		/// <param name="model">The model that contains search parameters.</param>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/Search/model
		/// </remarks>
		public ActionResult Search(SearchViewModel model)
		{
			var routeInfo = this.homeService.GetRouteInfo(model.From, model.To);
			
			if (!string.IsNullOrWhiteSpace(routeInfo.Link))
			{
				return this.Redirect(routeInfo.Link + "?date=" + model.DateView);
			}

			model.Content = routeInfo.Content;
			model.MetaKeyWords = routeInfo.MetaKeyWords;
			model.MetaDescription = routeInfo.MetaDescription;
			
			return this.View(model);
		}

		/// <summary>
		/// Form action.
		/// </summary>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// GET: /Home/Form
		/// </remarks>
		public ActionResult Form()
		{
			var viewModel = new FormViewModel()
			{
				Content = this.homeService.GetApplicationFormText(),
				OperatorCodes = this.commonService.GetOperatorCodes()
			};

			return this.View(viewModel);
		}

		/// <summary>
		/// Form action.
		/// </summary>
		/// <param name="viewModel">The view model.</param>
		/// <returns>
		/// Action result.
		/// </returns>
		/// <remarks>
		/// POST: /Home/Form
		/// </remarks>
		[HttpPost]
		public ActionResult Form(FormViewModel viewModel)
		{
			var request = new CreateFormRequest()
			{
				CompanyName = viewModel.CompanyName,
				Site = viewModel.Site,
				AdditionalInfo = viewModel.AdditionalInfo,
				Routes = viewModel.Routes,
				SelectedOperactorCodes = viewModel.SelectedOperactorCodes,
				Phones = viewModel.Phones
			};

			this.homeService.SendApplicationForm(request);

			return new EmptyResult();
		}

		#endregion

		#region Overriden Methods

		/// <summary>
		/// Called when an unhandled exception occurs in the action.
		/// </summary>
		/// <param name="filterContext">Information about the current request and action.</param>
		protected override void OnException(ExceptionContext filterContext)
		{
			this.logger.LogException(filterContext.Exception, "Exception was caught in the Home Controller.");

			filterContext.ExceptionHandled = true;
			filterContext.Result = this.View("Error");
		}

		#endregion
	}
}