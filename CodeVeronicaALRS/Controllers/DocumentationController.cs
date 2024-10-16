using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

namespace ALRS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentationController : Controller
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        public DocumentationController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        [HttpGet("docs")]
        public IActionResult Index()
        {
            try
            {
                var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items
                    .Where(x => x.AttributeRouteInfo != null)
                    .Select(x => new
                    {
                        Action = x.RouteValues["action"],
                        Controller = x.RouteValues["controller"],
                        Route = x.AttributeRouteInfo.Template,
                        Methods = x.ActionConstraints != null ?
                            string.Join(", ", x.ActionConstraints.Select(ac => ac.GetType().Name)) : "GET", // Default to GET
                        Parameters = GetParameters(x.RouteValues["controller"], x.RouteValues["action"])
                    })
                    .ToList();

                return Json(routes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while getting the docs.", details = ex.Message });
            }
        }

        private object GetParameters(string controllerName, string actionName)
        {
            var controllerType = Assembly.GetExecutingAssembly().GetTypes()
                .FirstOrDefault(t => t.Name == controllerName + "Controller");

            if (controllerType == null)
            {
                return null;
            }

            var method = controllerType.GetMethods()
                .FirstOrDefault(m => m.Name == actionName);

            if (method == null)
            {
                return null;
            }

            var parameters = method.GetParameters();

            var parameterInfo = new List<object>();
            foreach (var parameter in parameters)
            {
                var parameterType = parameter.ParameterType;

                if (parameterType.IsClass && parameterType != typeof(string))
                {
                    var schema = GetModelSchema(parameterType);
                    parameterInfo.Add(new
                    {
                        Name = parameter.Name,
                        Schema = schema
                    });
                }
                else
                {
                    parameterInfo.Add(new
                    {
                        Name = parameter.Name,
                        Type = parameterType.Name
                    });
                }
            }

            return parameterInfo;
        }

        private object GetModelSchema(Type modelType)
        {
            var properties = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var schema = new Dictionary<string, object>();

            foreach (var prop in properties)
            {
                schema[prop.Name] = prop.PropertyType.Name switch
                {
                    "Int32" => 0,
                    "String" => "string",

                    _ => null
                };
            }

            return schema;
        }
    }
}
