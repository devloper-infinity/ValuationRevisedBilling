using BillingOther.App_Code.BLL;
using BillingOther.App_Code.EL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BillingOther
{
    public partial class Login : System.Web.UI.Page
    {
        bllLogin bllLogin = new bllLogin();
        string Password = "";
        public string localIP;
        private string UserIDKey
        {
            get
            {
                if (ViewState["UserIDKey"] == null)
                    ViewState["UserIDKey"] = Guid.NewGuid().ToString();
                return (string)ViewState["UserIDKey"];
            }
            set
            {
                ViewState["UserIDKey"] = value;
            }
        }

        private string PwdKey
        {
            get
            {
                if (ViewState["PwdKey"] == null)
                    ViewState["PwdKey"] = Guid.NewGuid().ToString();
                return (string)ViewState["PwdKey"];
            }
            set
            {
                ViewState["PwdKey"] = value;
            }
        }

        private string returnUrl
        {
            get
            {
                if (ViewState["returnUrl"] == null)
                    ViewState["returnUrl"] = "";
                return (string)ViewState["returnUrl"];
            }
            set
            {
                ViewState["returnUrl"] = value;
            }
        }

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            if (IsPostBack)
            {
                UserIDKey = null;
                PwdKey = null;
                MakeFieldNamesSecret();
            }
        }

        private void MakeFieldNamesSecret()
        {
            txtPassword.ID = PwdKey;
            txtUserName.ID = UserIDKey;
            ConnectToSecretFields();
        }

        private void ConnectToSecretFields()
        {
            rfv_txtUserName.ControlToValidate = txtUserName.ID;
            rfv_txtPassword.ControlToValidate = txtPassword.ID;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var remoteIpAddress = Request.UserHostAddress;
            try
            {
                returnUrl = Request.QueryString["ReturnUrl"];
            }
            catch { }

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    string restFlag = Convert.ToString(Session["resetFlg"]);

                    if (restFlag == "False" || restFlag == "false")
                    {
                        if (HttpContext.Current.User.IsInRole("Admin"))
                        {
                            Session["User"] = "Admin";
                            if (int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 5 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 235 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 165 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 5902 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 8128)
                                Response.Redirect("~/Accounts/Dashboard.aspx");
                            else
                                Response.Redirect("~/BDM/CostPerRecord.aspx");
                        }
                    }
                    else
                    {
                        if (HttpContext.Current.User.IsInRole("Admin"))
                        {
                            if (int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 5 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 235 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 165 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 5902 || int.Parse(HttpContext.Current.User.Identity.Name.ToString()) == 8128)
                                Response.Redirect("~/Accounts/Dashboard.aspx");
                            else
                                Response.Redirect("~/BDM/CostPerRecord.aspx");
                        }

                        else
                        {
                            FormsAuthentication.SignOut();
                            Response.Redirect("~/Logout.aspx");
                        }
                    }
                }
                Response.Redirect(returnUrl);
            }

            if (!IsPostBack)
            {
                if (Request.Cookies["userid"] != null)
                    txtUserName.Text = Request.Cookies["userid"].Value;
                if (Request.Cookies["pwd"] != null)

                    txtPassword.Attributes.Add("value", Request.Cookies["pwd"].Value);
                //if (Request.Cookies["userid"] != null && Request.Cookies["pwd"] != null)
                //    chkRemember.Checked = true;
                MakeFieldNamesSecret();
                StringBuilder scriptLoader = new StringBuilder();
                scriptLoader.Append("<script type='text/javascript'>");
                scriptLoader.Append("var txtBox=document.getElementById('");
                scriptLoader.Append(UserIDKey + "');");
                scriptLoader.Append("if (txtBox!=null ) txtBox.focus();");
                scriptLoader.Append("</script>");
                this.ClientScript.RegisterStartupScript(this.GetType(), "onLoadCall", scriptLoader.ToString());
            }
            else
            {
                ConnectToSecretFields();
            }

            txtUserName.Focus();

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

                string userID = Request.Form[UserIDKey];
                string pwd = Request.Form[PwdKey];

                Session["UserName"] = Request.Form[UserIDKey];

                if (chkRemember.Checked == true)
                {
                    Response.Cookies["userid"].Value = userID;
                    Response.Cookies["pwd"].Value = pwd;
                    Response.Cookies["userid"].Expires = DateTime.Now.AddMinutes(30);
                    Response.Cookies["pwd"].Expires = DateTime.Now.AddMinutes(30);
                }

                else
                {
                    Response.Cookies["userid"].Expires = DateTime.Now.AddMinutes(-1);

                    Response.Cookies["pwd"].Expires = DateTime.Now.AddMinutes(-1);
                }

                //********** Block User Login **********//
                DataTable dt = bllLogin.BlockUserLogin(userID);
                string encPassword = bllLogin.Encrypt(pwd);
                int ReturnValue = 0;

                int ReturnValue2 = bllLogin.ValidateUser(Filter.SQLInjectionFilter(userID), Filter.SQLInjectionFilter(encPassword));
                if (ReturnValue2 == 0)
                {
                    ReturnValue = bllLogin.ValidateUser(Filter.SQLInjectionFilter(userID), Filter.SQLInjectionFilter(pwd));
                }
                else
                {
                    ReturnValue = ReturnValue2;
                }
                //ReturnValue = bllLogin.ValidateUser(Filter.SQLInjectionFilter(userID), Filter.SQLInjectionFilter(pwd));

                if (ReturnValue == -1)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "User does not exists";
                }
                else if (ReturnValue == 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "Invalid Password";
                }

                else if (dt.Rows.Count > 0)
                {
                    dvError.Style.Add("display", "");
                    dvError.Attributes.Add("class", "alert alert-danger background-danger");
                    dvError.InnerHtml = "<b>Your login has been blocked. <br/>Please contact your reporting manager.</b>";
                    rfv_txtPassword.Enabled = false;
                    rfv_txtUserName.Enabled = false;
                }
                else
                {
                    DataTable usr = bllLogin.GetUserById(ReturnValue, Filter.SQLInjectionFilter(userID), Filter.SQLInjectionFilter(encPassword));
                    if (usr.Rows.Count <= 0)
                        usr = bllLogin.GetUserById(ReturnValue, Filter.SQLInjectionFilter(userID), Filter.SQLInjectionFilter(pwd));
                    bool IsTrue = false;
                    try
                    {
                        IsTrue = pwd.ToUpper().Contains("INFINITY");
                    }
                    catch { }

                    if (IsTrue == true)
                    {
                        Response.Redirect("~/ResetPassword.aspx");
                    }
                    int employeeId = Convert.ToInt32(usr.Rows[0]["EmployeeId"]);
                    string role = Convert.ToString(usr.Rows[0]["Role"]);
                    MfaService mfa = new MfaService();
                    if (mfa.IsRequired)
                    {
                        Session["PendingLogin"] = new PendingLogin { EmployeeId = employeeId, Role = role, RememberMe = chkRemember.Checked, ReturnUrl = returnUrl };
                        Session["PendingMfaSecret"] = null;
                        Response.Redirect("~/Mfa.aspx", true);
                        return;
                    }
                    IssueAuthenticationTicket(employeeId, role, chkRemember.Checked);
                    RedirectAfterAuthentication(returnUrl);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Login failed: " + ex);
                dvError.Attributes["class"] = "alert alert-danger";
                dvError.InnerText = "Sign in could not be completed. Please try again or contact support.";
            }
        }

        internal static void IssueAuthenticationTicket(int employeeId, string role, bool rememberMe)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, employeeId.ToString(), DateTime.Now, DateTime.Now.AddMinutes(30), rememberMe, role, FormsAuthentication.FormsCookiePath);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket));
            cookie.HttpOnly = true;
            cookie.Secure = HttpContext.Current.Request.IsSecureConnection;
            cookie.SameSite = SameSiteMode.Lax;
            if (ticket.IsPersistent) cookie.Expires = ticket.Expiration;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        internal static void RedirectAfterAuthentication(string target)
        {
            if (!string.IsNullOrWhiteSpace(target) && target.StartsWith("/", StringComparison.Ordinal) && !target.StartsWith("//", StringComparison.Ordinal))
                HttpContext.Current.Response.Redirect(target, true);
            else
                HttpContext.Current.Response.Redirect("~/Login.aspx", true);
        }
    }
}
