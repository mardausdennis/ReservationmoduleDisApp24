using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

public class TestModel : PageModel
{
    public IActionResult OnGet()
    {
        Console.WriteLine("Test OnGet called");
        return Page();
    }
}
