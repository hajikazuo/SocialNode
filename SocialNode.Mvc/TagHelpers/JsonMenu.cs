namespace SocialNode.Mvc.TagHelpers
{
    public class JsonMenu
    {
        public List<Chave> chaves { get; set; }
    }

    /// <summary>
    /// tagMenu é a tag que é postada no menu.cshtml para ativar
    /// </summary>
    public class Chave
    {

        public string tagMenu { get; set; }
        public string rotas { get; set; }
        public bool isParent { get; set; }

        /// <summary>
        /// Se é o li principal (controller) no menu.cshtml
        /// </summary>
        public bool Controller { get; set; }
    }
}
