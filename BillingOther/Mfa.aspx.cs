using BillingOther.App_Code.BLL;
using System;
using System.Web;
using System.Web.UI;

namespace BillingOther
{
    public partial class Mfa : Page
    {
        private readonly MfaService mfa = new MfaService();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!mfa.IsRequired) { Response.Redirect("~/Login.aspx", true); return; }
            PendingLogin pending = Session["PendingLogin"] as PendingLogin;
            if (pending == null) { Response.Redirect("~/Login.aspx", true); return; }
            if (!IsPostBack) BindState(pending, null);
        }

        private void BindState(PendingLogin pending, string knownSecret)
        {
            string secret = knownSecret ?? mfa.GetSecret(pending.EmployeeId);
            bool enrolling = string.IsNullOrWhiteSpace(secret);
            if (enrolling) { secret = Convert.ToString(Session["PendingMfaSecret"]); if (string.IsNullOrWhiteSpace(secret)) { secret = mfa.CreateSecret(); Session["PendingMfaSecret"] = secret; } }
            pnlSetup.Visible = enrolling;
            litHeading.Text = enrolling ? "Set up your authenticator" : "Enter your verification code";
            litInstructions.Text = enrolling ? "Add the setup key below to your authenticator app, then enter the generated code." : "Open your authenticator app and enter the current code.";
            litSecret.Text = HttpUtility.HtmlEncode(FormatSecret(secret));
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            PendingLogin pending = Session["PendingLogin"] as PendingLogin;
            if (pending == null) { Response.Redirect("~/Login.aspx", true); return; }
            string storedSecret = mfa.GetSecret(pending.EmployeeId);
            bool enrolling = string.IsNullOrWhiteSpace(storedSecret);
            string secret = enrolling ? Convert.ToString(Session["PendingMfaSecret"]) : storedSecret;
            if (!mfa.Verify(secret, Request.Form["otpCode"])) { dvError.Attributes["class"] = "alert alert-danger"; dvError.InnerText = "The verification code is invalid or expired. Please try again."; BindState(pending, secret); return; }
            if (enrolling) mfa.Enroll(pending.EmployeeId, secret);
            Session.Remove("PendingLogin"); Session.Remove("PendingMfaSecret");
            Login.IssueAuthenticationTicket(pending.EmployeeId, pending.Role, pending.RememberMe);
            Login.RedirectAfterAuthentication(pending.ReturnUrl);
        }

        private static string FormatSecret(string value) { if (string.IsNullOrEmpty(value)) return string.Empty; System.Text.StringBuilder b = new System.Text.StringBuilder(); for (int i=0;i<value.Length;i++){if(i>0&&i%4==0)b.Append(' ');b.Append(value[i]);} return b.ToString(); }
    }
}
