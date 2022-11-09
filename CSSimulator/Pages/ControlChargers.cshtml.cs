using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CSSimulator.Pages
{
    public class chargerGrainModel : PageModel
    {
        private readonly ILogger<chargerGrainModel> _logger;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int Count { get; set; }
        public int PageSize { get; set; } = 24;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public chargerGrainModel(ILogger<chargerGrainModel> logger)
        {
            _logger = logger;
        }

        public List<ChargerGrainStorage.ChargerGrainsDTO> GetPaginatedResult(int currentPage, int pageSize = 10)
        {
            var data = ChargerGrainStorage.chargerGrains;
            Count = ChargerGrainStorage.chargerGrains.Count;
            return data.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            // OrderBy(d => int.Parse(d.identity)).
        }


        public void OnGet()
        {
        }
    }
}