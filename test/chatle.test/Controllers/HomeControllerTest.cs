using ChatLe.Controllers;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace chatle.test.Controllers
{
    public class HomeControllerTest
    {
		[Fact]
		public void IndexTest()
		{
			ExecuteAction(controller =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockClaims = new Mock<ClaimsPrincipal>();
				var mockIndentity = new Mock<IIdentity>();
				mockIndentity.SetupGet(i => i.IsAuthenticated).Returns(true);
				mockClaims.SetupGet(c => c.Identity).Returns(mockIndentity.Object);
				mockHttpContext.SetupGet(h => h.User).Returns(mockClaims.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				var result = controller.Index();
				Assert.IsType<ViewResult>(result);

				mockIndentity.SetupGet(i => i.IsAuthenticated).Returns(false);
				result = controller.Index();
				Assert.IsType<RedirectToRouteResult>(result);
			});
		}

		public static void ExecuteAction(Action<HomeController> a)
		{
			var mockLoggerFactory = new Mock<ILoggerFactory>();
			mockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
			var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
			var controller = new HomeController(mockLoggerFactory.Object) { ViewData = viewData };
			using (controller)
			{
				a.Invoke(controller);
			}
		}

		[Fact]
		public void AboutTest()
		{
			ExecuteAction(controller =>
			{
				var result = controller.About();
				Assert.IsType<ViewResult>(result);
			});
		}

		[Fact]
		public void ContactTest()
		{
			ExecuteAction(controller =>
			{
				var result = controller.Contact();
				Assert.IsType<ViewResult>(result);
			});
		}

		[Fact]
		public void ErrorTest()
		{
			ExecuteAction(controller =>
			{
				var mockHttpContext = new Mock<HttpContext>();
				var mockFeatures = new Mock<IFeatureCollection>();
				mockHttpContext.SetupGet(h => h.Features).Returns(mockFeatures.Object);
				controller.ActionContext.HttpContext = mockHttpContext.Object;

				var result = controller.Error();
				Assert.IsType<ViewResult>(result);
			});
		}
	}
}
