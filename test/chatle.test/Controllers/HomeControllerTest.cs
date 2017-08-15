using ChatLe.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Security.Principal;
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
				controller.ControllerContext.HttpContext = mockHttpContext.Object;

				var result = controller.Index();
				Assert.IsType<ViewResult>(result);

				mockIndentity.SetupGet(i => i.IsAuthenticated).Returns(false);
				result = controller.Index();
				Assert.IsType<RedirectToRouteResult>(result);
			});
		}

        internal static void ExecuteAction(Action<HomeController> a)
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
				controller.ControllerContext.HttpContext = mockHttpContext.Object;

				var result = controller.Error();
				Assert.IsType<ViewResult>(result);
			});
		}
	}
}
