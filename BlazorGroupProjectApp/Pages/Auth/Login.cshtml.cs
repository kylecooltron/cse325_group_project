using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace BlazorGroupProjectApp.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public string? ErrorMessage { get; private set; }

    public IActionResult OnGet(string? returnUrl = null, string? error = null)
    {
        if (error != null)
            ErrorMessage = "Sign-in failed. Please try again.";

        return Page();
    }

    public IActionResult OnPostGoogleLogin(string? returnUrl = null)
    {
        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = "/";

        var redirectUrl = Url.Page("/Auth/Login", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return Challenge(properties, "Google");
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
            return RedirectToPage(new { error = remoteError });

        if (!Url.IsLocalUrl(returnUrl))
            returnUrl = "/";

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToPage(new { error = "external_login_info_missing" });

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email == null)
            return RedirectToPage(new { error = "no_email" });

        var givenName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? email;

        // Find or create the user
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return RedirectToPage(new { error = "create_failed" });

            await _userManager.AddLoginAsync(user, info);
        }

        // Keep the stored GivenName claim in sync
        await EnsureGivenNameClaimAsync(user, givenName);

        // Sign in fresh so the cookie includes the updated claims immediately
        await _signInManager.SignInAsync(user, isPersistent: true);
        return LocalRedirect(returnUrl);
    }

    private async Task EnsureGivenNameClaimAsync(IdentityUser user, string givenName)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var existing = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
        if (existing != null && existing.Value == givenName) return;
        if (existing != null)
            await _userManager.ReplaceClaimAsync(user, existing, new Claim(ClaimTypes.GivenName, givenName));
        else
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, givenName));
    }
}
