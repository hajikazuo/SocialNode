using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SocialNode.Mvc.TagHelpers.ActiveRoute
{
    /// <summary>
    /// Tag Helper para ativar os menus de acordo com a rota
    /// UTiliza para isso o arquivo navMenu.json na raiz do projeto
    /// </summary>
    [HtmlTargetElement(Attributes = "is-active")]
    public class ActiveRouteV2TagHelper : TagHelper
    {
        protected readonly string ClassAttribute = "class";
        protected readonly string ActiveClass = "active";

        protected readonly JsonMenu _menu;
        public ActiveRouteV2TagHelper(JsonMenu menu)
        {
            _menu = menu;
        }

        [HtmlAttributeName("active-route")]
        public string IDRota { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (ShouldBeActive())
            {
                MakeActive(output);
            }
        }

        private bool ShouldBeActive()
        {
            string currentController = ViewContext.RouteData.Values["Controller"].ToString();
            string currentAction = ViewContext.RouteData.Values["Action"].ToString();

            var cc = $"{currentController}_{currentAction}";

            var pagePersonalizada = ViewContext.ViewData["PageRouteName"];
            if (pagePersonalizada != null && pagePersonalizada.ToString().ToLower().Equals(IDRota?.ToLower()))
            {

                if (_menu.chaves.Where(w => w.rotas.ToLower().Contains(pagePersonalizada.ToString().ToLower())).Any())
                {
                    return true;
                }
            }

            var obj = _menu.chaves.Where(x => x.tagMenu.ToLower() == IDRota?.ToLower()).FirstOrDefault();
            if (obj == null)
            {
                return false;
            }
            if (obj.Controller)
            {
                return obj.rotas.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Any(r => r.Equals(currentController, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return obj.rotas.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Any(r => r.Equals(cc, StringComparison.OrdinalIgnoreCase));
            }

        }

        private void MakeActive(TagHelperOutput output)
        {
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");
            var obj = _menu.chaves.FirstOrDefault(x => x.tagMenu.Equals(IDRota, StringComparison.OrdinalIgnoreCase));
            var isParent = obj?.isParent ?? false;

            string classToAdd = isParent ? "active open" : "active";

            if (classAttr == null)
            {
                output.Attributes.Add("class", classToAdd);
            }
            else if (classAttr.Value == null || !classAttr.Value.ToString().Contains("active"))
            {
                output.Attributes.SetAttribute("class", $"{classAttr.Value?.ToString()} {classToAdd}".Trim());
            }
        }
    }
}
